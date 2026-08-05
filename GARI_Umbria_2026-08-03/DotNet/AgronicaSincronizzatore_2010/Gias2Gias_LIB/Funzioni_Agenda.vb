Imports System.Xml
Imports System.Text

Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreUtility.CulturaHelper

Imports Newtonsoft.Json
Imports AgronicaCoreG2GLocalDal

Partial Public Class Funzioni



#Region "Agenda"

    Public Sub Elabora_XML_Agenda_Salva(ByVal objOpzioniImportImpresa As clsImpresa,
                                        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                        ByVal objOpzioni As clsOpzioni,
                                        ByRef Log_Import As StringBuilder,
                                        ByRef Log_Errori As StringBuilder,
                                        ByRef Log_Riepilogo As StringBuilder,
                                        ByVal Piva_Origine As String,
                                        ByVal Piva_Destinazione As String,
                                        ByVal TipoOperazione_DB As enum_TipoOperazioneDB)

        Const nomeFunzione = "Elabora_XML_Agenda_Salva"

        Dim sXmlAgenda As String
        Dim leggiAgenda As New AgronicaCoreContabBIZ.Agenda_R

        Dim oConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda
        oConfigurazioneAgenda = ConfigurazioneG2GLeggiAgenda(objOpzioniImportImpresa)

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente As Date
            Dim FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio =
                objOpzioniImportImpresa.ValiditaInizio_agenda

            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine =
                objOpzioniImportImpresa.ValiditaFine_agenda

            Dim xFiltroAggiuntivo As String = ""
            If TipoOperazione_DB = enum_TipoOperazioneDB.Scrittura AndAlso oConfigurazioneAgenda.listaLavCod.Count > 0 Then
                xFiltroAggiuntivo = " Lav_cod in (" & String.Join(",", oConfigurazioneAgenda.listaLavCod) & ") "
            End If

            'lettura
            sXmlAgenda = leggiAgenda.Agenda_Leggi(Piva_Origine,
                                                  0,
                                                  0,
                                                  0,
                                                  False,
                                                  objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                  LeggiRiferimentiInversi:=False,
                                                  TipoG2G:=TipoOperazione_DB,
                                                  xFiltroAggiuntivo:=xFiltroAggiuntivo
                                                )

            'reimposto il fitro della finestra precedente
            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If sXmlAgenda <> "" Then
                Dim oDocAgenda As New XmlDocument
                oDocAgenda.LoadXml(sXmlAgenda)

                Dim scriviAgenda As New AgronicaCoreContabBIZ.Agenda_W
                Dim outputID_Agenda As Integer

                'COPIO LE MATERIE PRIME UTILIZZATE
                Dim listaMatCod As New List(Of String)
                For Each singleAgenda As XmlNode In oDocAgenda.SelectNodes("//DatiAgenda/Agenda")
                    If Not {"1008", "-1"}.Contains(singleAgenda.Attributes("lav_cod").Value) Then
                        For Each nMovimenti As XmlNode In singleAgenda.SelectNodes("DatiMovimenti/Movimento")
                            For Each nMovimentiDettagli As XmlNode In nMovimenti.SelectNodes("DatiMovimenti_Dettagli/Movimento_Dettaglio")
                                If {"10", "200", "201", "204", "205", "210", "301", "304", "305", "306", "307", "310", "400", "401", "500", "700"}.Contains(nMovimentiDettagli.Attributes("elem_cod").Value) _
                                    AndAlso Not listaMatCod.Contains("'" + nMovimentiDettagli.Attributes("mat_cod").Value + "'") AndAlso nMovimentiDettagli.Attributes("mat_cod").Value <> "0" Then
                                    listaMatCod.Add("'" + nMovimentiDettagli.Attributes("mat_cod").Value + "'")
                                End If
                            Next
                        Next
                    End If
                Next

                ' se versione WS le materie prime vengono passate con procedura apposita 
                Dim elabora_materie_prime As Boolean = If(NuovaLogicaRecodeAG, Not objOpzioniImportImpresa.Flagimporta_materieprime, True)
                If elabora_materie_prime AndAlso listaMatCod.Count > 0 Then
                    Dim filtro As String = If(listaMatCod.Count > 0, " Materie_Prime.Mat_Cod IN (" + String.Join(",", listaMatCod) + ")", "")
                    Dim objMP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim DT_MP As DataTable = objMP_R.Leggi3("",
                                                            0,
                                                            SACOD_NOFILTRO,
                                                            0, "",
                                                            True,
                                                            filtro, "",
                                                            objOpzioni.objParametri_Server_GIAS_ORIGINE)

                    Me.Elabora_XML_Imprese_MateriePrime_Salva_NEW(objOpzioni,
                                                                  Log_Import,
                                                                  Log_Errori,
                                                                  Log_Riepilogo,
                                                                  Piva_Origine,
                                                                  Piva_Destinazione,
                                                                  DT_MP)
                End If


                Dim conteggioOperazioni As Integer = 0

                'COPIO I LE OPERAZIONI D'AGENDA
                For Each singleAgenda As XmlNode In oDocAgenda.SelectNodes("//DatiAgenda/Agenda")

                    Dim saltaAgenda As Boolean = False

                    'Se sono stati impostati dei fabbricati come filtro, controllo che tutte le destinazioni dell'operazione di agenda
                    'utilizzino fabbricati di questo elenco, in caso contrario non sincronizzo questa operazione
                    If objOpzioniImportImpresa.Fabbricati.Count > 0 Then

                        Dim xmlMovimenti = singleAgenda.SelectNodes("DatiMovimenti/Movimento")

                        Dim listaXmlMovDestCarichiScarichi As New List(Of XmlNodeList)()

                        For Each xmlMov As XmlNode In xmlMovimenti

                            If {CAU_CARICO, CAU_SCARICO}.Contains(xmlMov.Attributes("cau_mov").Value) Then

                                listaXmlMovDestCarichiScarichi.Add(xmlMov.SelectNodes("DatiMovimenti_Dettagli/Movimento_Dettaglio/Movimento_Destinazione"))

                            End If

                        Next

                        For Each xmlMovDestCS As XmlNodeList In listaXmlMovDestCarichiScarichi

                            For Each movDest As XmlNode In xmlMovDestCS
                                Dim pivaDest = movDest.Attributes("piva").Value
                                Dim sacodDest = movDest.Attributes("sa_cod").Value
                                Dim fabcodDest = movDest.Attributes("id_destinazione").Value

                                If (From i In objOpzioniImportImpresa.Fabbricati Where i.Piva = pivaDest And i.Sa_Cod = sacodDest And i.Fabbricato_Cod = fabcodDest).ToList.Count = 0 Then
                                    saltaAgenda = True
                                    Exit For
                                End If
                            Next

                            If saltaAgenda = True Then
                                Exit For
                            End If

                        Next
                    End If

                    If saltaAgenda = True Then
                        Continue For
                    End If

                    Dim lavCodCorrente As String = singleAgenda.Attributes("lav_cod").Value

                    Dim DataOperazione As Date?
                    AgendaOttieniDataOperazioneDaXML(singleAgenda, DataOperazione)

                    ' VAnni: 5/7/2019:Data Operazione Nothing non dovrebbe mai succedere, nel caso comunque invio lo stesso, 
                    '        altrimenti mi affido ad intervallo di date..:
                    If IsNothing(DataOperazione) _
                        OrElse (objOpzioniImportImpresa.ValiditaInizio_agenda <= DataOperazione AndAlso objOpzioniImportImpresa.ValiditaFine_agenda >= DataOperazione) _
                        OrElse singleAgenda.Attributes("des_lib").Value = "G2G Delete" Then

                        'testa quali sono i lav_cod da inviare..:
                        If lavCodCorrente <> "1008" AndAlso lavCodCorrente <> "-1" AndAlso (oConfigurazioneAgenda.listaLavCod.Count = 0 OrElse oConfigurazioneAgenda.listaLavCod.Contains(CInt(lavCodCorrente))) Then

                            Dim inputID_Agenda As Integer = CInt(singleAgenda.Attributes("id_agenda").Value)
                            Dim inputSa_Cod_Agenda As Integer = CInt(singleAgenda.Attributes("sa_cod").Value)

                            Try

                                Dim CodiciRimappati As String = EstraiCodiciDaXmlOrigine(oConfigurazioneAgenda, objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, singleAgenda)


                                Dim xRecodeAgenda As G2G_Recode_Agenda = (From aa In efG2G.G2G_Recode_Agenda
                                                                          Where aa.FromId_Agenda = inputID_Agenda AndAlso
                                                                          aa.FromPiva = Piva_Origine AndAlso
                                                                          aa.FromSa_cod = inputSa_Cod_Agenda AndAlso
                                                                          aa.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser
                                                                          ).FirstOrDefault

                                outputID_Agenda = 0
                                If Not xRecodeAgenda Is Nothing Then
                                    outputID_Agenda = xRecodeAgenda.ToId_Agenda
                                End If

                                Dim ProcediConIlSalvataggio As Boolean = True

                                'Ricodifico il raccoglitore_cod
                                Dim inputRaccoglitore_Cod As Integer = If(IsNumeric(singleAgenda.Attributes("raccoglitore_cod").Value), CInt(singleAgenda.Attributes("raccoglitore_cod").Value), 0)
                                Dim outpuRaccoglitore_Cod As Integer = 0
                                If inputRaccoglitore_Cod > 0 Then


                                    Dim xRecodeRaccoglitore As G2G_Recode_Raccoglitore = (
                                        From aa In efG2G.G2G_Recode_Raccoglitore
                                        Where aa.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                        And aa.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                        And aa.From_Piva = Piva_Origine _
                                        And aa.From_Raccoglitore_Cod = inputRaccoglitore_Cod
                                    ).FirstOrDefault

                                    'Se non lo trovo nei recode, controllo se è stato inserito in questa transazione
                                    If IsNothing(xRecodeRaccoglitore) AndAlso Not NuovaLogicaRecodeAG Then

                                        xRecodeRaccoglitore = (
                                            From aa In _Raccoglitore
                                            Where aa.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                            And aa.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                            And aa.From_Piva = Piva_Origine _
                                            And aa.From_Raccoglitore_Cod = inputRaccoglitore_Cod
                                        ).FirstOrDefault

                                    End If

                                    If Not IsNothing(xRecodeRaccoglitore) Then
                                        outpuRaccoglitore_Cod = xRecodeRaccoglitore.To_Raccoglitore_Cod
                                    Else
                                        outpuRaccoglitore_Cod = -1
                                    End If

                                End If

                                Dim sStringaDaSalvare As String =
                                    "<DatiAgenda>" &
                                        Elabora_XML_Agenda_SistemaXml(
                                                objOpzioniImportImpresa,
                                                objOpzioni,
                                                Log_Import,
                                                Log_Errori,
                                                Log_Riepilogo,
                                                singleAgenda.OuterXml,
                                                Piva_Origine,
                                                Piva_Destinazione,
                                                outputID_Agenda,
                                                outpuRaccoglitore_Cod,
                                                TipoOperazione_DB,
                                                ProcediConIlSalvataggio
                                            ) _
                                        & "</DatiAgenda>"

                                ' controllo per evitare di passare id agenda = 0 in cancellazione
                                If TipoOperazione_DB = enum_TipoOperazioneDB.Cancellazione AndAlso outputID_Agenda = 0 Then
                                    Log_Import.AppendLine(CStr(Date.Now) + " - Errore, recode dell'agenda " & inputID_Agenda & " non trovato ")
                                    ProcediConIlSalvataggio = False
                                End If

                                If ProcediConIlSalvataggio Then

                                    If objOpzioni.isGias2Gias_local Then
                                        scriviAgenda.Agenda_Scrivi(sStringaDaSalvare,
                                            outputID_Agenda,
                                            0,
                                            0,
                                            0,
                                            objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                            CodiciRimappati
                                    )
                                    Else

                                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                                        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                                        outputID_Agenda = wsImportazione.Scrivi_AgendaXmlPrivato(Str_Credenziali_WS, CodiciRimappati, sStringaDaSalvare, NuovaLogicaRecodeAG)
                                    End If

                                    If outputID_Agenda <> -1 OrElse TipoOperazione_DB <> enum_TipoOperazioneDB.Scrittura Then
                                        If NuovaLogicaRecodeAG Then
                                            G2GUtility.Scrivi_G2G_Recode_Agenda(CodiciRimappati, TipoOperazione_DB, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                                        Else
                                            SalvataggioCodiciRimappati(CodiciRimappati, TipoOperazione_DB)
                                        End If
                                        conteggioOperazioni += 1
                                    Else
                                        Log_Import.AppendLine(CStr(Date.Now) + " - Errore, l'agenda " & inputID_Agenda & " non è stata trasferita ")
                                    End If

                                End If
                                'end procedi con il salvataggio

                            Catch ex As Exception

                                Log_Import.AppendLine(CStr(Date.Now) + " - Errore, l'agenda " & inputID_Agenda & " non è stata trasferita ")

                            End Try

                        End If

                    End If
                    'test per capire l'intervallo di validità è valido

                Next
                'operazione di agenda

                Dim xTesto As String = ""
                Select Case TipoOperazione_DB
                    Case enum_TipoOperazioneDB.Scrittura
                        xTesto = "nuove"
                    Case enum_TipoOperazioneDB.Modifica
                        xTesto = "modificate"
                    Case enum_TipoOperazioneDB.Cancellazione
                        xTesto = "eliminate"
                End Select

                Log_Import.AppendLine(CStr(Date.Now) + " - Trasferimento Agenda: " & conteggioOperazioni & " operazioni " & xTesto)

            End If
        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Agenda_SalvaReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                        ByVal efG2G As AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities,
                                        ByVal objOpzioni As clsOpzioni,
                                        ByRef Log_Import As StringBuilder,
                                        ByRef Log_Errori As StringBuilder,
                                        ByRef Log_Riepilogo As StringBuilder,
                                        ByVal Piva_Origine As String,
                                        ByVal Piva_Destinazione As String,
                                        ByVal TipoOperazione_DB As enum_TipoOperazioneDB)

        Const nomeFunzione = "Elabora_XML_Agenda_SalvaReverse"

        Dim sXmlAgenda As String
        Dim leggiAgenda As New AgronicaCoreContabBIZ.Agenda_R

        Dim oConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda
        oConfigurazioneAgenda = ConfigurazioneG2GLeggiAgenda(objOpzioniImportImpresa)

        Try

            'memorizza il dato precedente per successivo recupero
            Dim FinestraTemporaleFinePrecedente As Date
            Dim FinestraTemporaleInizioPrecedente As Date

            FinestraTemporaleRecupera(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            'imposta il filtro temporale
            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleInizio =
                objOpzioniImportImpresa.ValiditaInizio_agenda

            objOpzioni.objParametri_Server_GIAS_ORIGINE.FinestraTemporaleFine =
                objOpzioniImportImpresa.ValiditaFine_agenda

            Dim xFiltroAggiuntivo As String = ""
            If TipoOperazione_DB = enum_TipoOperazioneDB.Scrittura AndAlso oConfigurazioneAgenda.listaLavCod.Count > 0 Then
                xFiltroAggiuntivo = " Lav_cod in (" & String.Join(",", oConfigurazioneAgenda.listaLavCod) & ") "
            End If

            'lettura
            sXmlAgenda = leggiAgenda.Agenda_LeggiReverse(Piva_Origine,
                                                  0,
                                                  0,
                                                  0,
                                                  False,
                                                  objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                  LeggiRiferimentiInversi:=False,
                                                  TipoG2G:=TipoOperazione_DB,
                                                  xFiltroAggiuntivo:=xFiltroAggiuntivo,
                                                  From_PivaSuperUser:=objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
                                                )

            'reimposto il fitro della finestra precedente
            FinestraTemporaleImpostaValori(objOpzioni, FinestraTemporaleInizioPrecedente, FinestraTemporaleFinePrecedente)

            If sXmlAgenda <> "" Then
                Dim oDocAgenda As New XmlDocument
                oDocAgenda.LoadXml(sXmlAgenda)

                Dim scriviAgenda As New AgronicaCoreContabBIZ.Agenda_W
                Dim outputID_Agenda As Integer

                'COPIO LE MATERIE PRIME UTILIZZATE
                Dim listaMatCod As New List(Of String)
                For Each singleAgenda As XmlNode In oDocAgenda.SelectNodes("//DatiAgenda/Agenda")
                    If Not {"1008", "-1"}.Contains(singleAgenda.Attributes("lav_cod").Value) Then
                        For Each nMovimenti As XmlNode In singleAgenda.SelectNodes("DatiMovimenti/Movimento")
                            For Each nMovimentiDettagli As XmlNode In nMovimenti.SelectNodes("DatiMovimenti_Dettagli/Movimento_Dettaglio")
                                If {"10", "200", "201", "204", "205", "210", "301", "304", "305", "306", "307", "310", "400", "401", "500", "700"}.Contains(nMovimentiDettagli.Attributes("elem_cod").Value) _
                                    AndAlso Not listaMatCod.Contains("'" + nMovimentiDettagli.Attributes("mat_cod").Value + "'") AndAlso nMovimentiDettagli.Attributes("mat_cod").Value <> "0" Then
                                    listaMatCod.Add("'" + nMovimentiDettagli.Attributes("mat_cod").Value + "'")
                                End If
                            Next
                        Next
                    End If
                Next

                ' se versione WS le materie prime vengono passate con procedura apposita 
                Dim elabora_materie_prime As Boolean = If(NuovaLogicaRecodeAG, Not objOpzioniImportImpresa.Flagimporta_materieprime, True)
                If elabora_materie_prime AndAlso listaMatCod.Count > 0 Then
                    Dim filtro As String = If(listaMatCod.Count > 0, " Materie_Prime.Mat_Cod IN (" + String.Join(",", listaMatCod) + ")", "")
                    Dim objMP_R As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
                    Dim DT_MP As DataTable = objMP_R.Leggi3("",
                                                            0,
                                                            SACOD_NOFILTRO,
                                                            0, "",
                                                            True,
                                                            filtro, "",
                                                            objOpzioni.objParametri_Server_GIAS_ORIGINE)

                    Me.Elabora_XML_Imprese_MateriePrime_Salva_NEW(objOpzioni,
                                                                  Log_Import,
                                                                  Log_Errori,
                                                                  Log_Riepilogo,
                                                                  Piva_Origine,
                                                                  Piva_Destinazione,
                                                                  DT_MP)
                End If


                Dim conteggioOperazioni As Integer = 0

                'COPIO I LE OPERAZIONI D'AGENDA
                For Each singleAgenda As XmlNode In oDocAgenda.SelectNodes("//DatiAgenda/Agenda")

                    Dim lavCodCorrente As String = singleAgenda.Attributes("lav_cod").Value

                    Dim DataOperazione As Date?
                    AgendaOttieniDataOperazioneDaXML(singleAgenda, DataOperazione)


                    ' VAnni: 5/7/2019:Data Operazione Nothing non dovrebbe mai succedere, nel caso comunque invio lo stesso, 
                    '        altrimenti mi affido ad intervallo di date..:
                    If IsNothing(DataOperazione) _
                        OrElse (objOpzioniImportImpresa.ValiditaInizio_agenda <= DataOperazione AndAlso objOpzioniImportImpresa.ValiditaFine_agenda >= DataOperazione) _
                        OrElse singleAgenda.Attributes("des_lib").Value = "G2G Delete" Then

                        'testa quali sono i lav_cod da inviare..:
                        If lavCodCorrente <> "1008" AndAlso lavCodCorrente <> "-1" AndAlso (oConfigurazioneAgenda.listaLavCod.Count = 0 OrElse oConfigurazioneAgenda.listaLavCod.Contains(CInt(lavCodCorrente))) Then

                            Dim inputID_Agenda As Integer = CInt(singleAgenda.Attributes("id_agenda").Value)
                            Dim inputSa_Cod_Agenda As Integer = CInt(singleAgenda.Attributes("sa_cod").Value)

                            Try

                                Dim CodiciRimappati As String = EstraiCodiciDaXmlOrigine(oConfigurazioneAgenda, objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser, singleAgenda)


                                Dim xRecodeAgenda As G2G_Recode_Agenda = (From aa In efG2G.G2G_Recode_Agenda
                                                                          Where aa.ToId_Agenda = inputID_Agenda AndAlso
                                                                          aa.FromPiva = Piva_Origine AndAlso
                                                                          aa.ToSa_cod = inputSa_Cod_Agenda AndAlso
                                                                          aa.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser
                                                                          ).FirstOrDefault

                                outputID_Agenda = 0
                                If Not xRecodeAgenda Is Nothing Then
                                    outputID_Agenda = xRecodeAgenda.FromId_Agenda
                                End If

                                Dim ProcediConIlSalvataggio As Boolean = True

                                'Ricodifico il raccoglitore_cod
                                Dim inputRaccoglitore_Cod As Integer = If(IsNumeric(singleAgenda.Attributes("raccoglitore_cod").Value), CInt(singleAgenda.Attributes("raccoglitore_cod").Value), 0)
                                Dim outpuRaccoglitore_Cod As Integer = 0
                                If inputRaccoglitore_Cod > 0 Then


                                    Dim xRecodeRaccoglitore As G2G_Recode_Raccoglitore = (
                                        From aa In efG2G.G2G_Recode_Raccoglitore
                                        Where aa.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                        And aa.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                        And aa.From_Piva = Piva_Origine _
                                        And aa.To_Raccoglitore_Cod = inputRaccoglitore_Cod
                                    ).FirstOrDefault

                                    'Se non lo trovo nei recode, controllo se è stato inserito in questa transazione
                                    If IsNothing(xRecodeRaccoglitore) AndAlso Not NuovaLogicaRecodeAG Then

                                        xRecodeRaccoglitore = (
                                            From aa In _Raccoglitore
                                            Where aa.To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser _
                                            And aa.From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser _
                                            And aa.From_Piva = Piva_Origine _
                                            And aa.To_Raccoglitore_Cod = inputRaccoglitore_Cod
                                        ).FirstOrDefault

                                    End If

                                    If Not IsNothing(xRecodeRaccoglitore) Then
                                        outpuRaccoglitore_Cod = xRecodeRaccoglitore.From_Raccoglitore_Cod
                                    Else
                                        outpuRaccoglitore_Cod = -1
                                    End If

                                End If

                                Dim sStringaDaSalvare As String =
                                    "<DatiAgenda>" &
                                        Elabora_XML_Agenda_SistemaXmlReverse(
                                                objOpzioniImportImpresa,
                                                objOpzioni,
                                                Log_Import,
                                                Log_Errori,
                                                Log_Riepilogo,
                                                singleAgenda.OuterXml,
                                                Piva_Origine,
                                                Piva_Destinazione,
                                                outputID_Agenda,
                                                outpuRaccoglitore_Cod,
                                                TipoOperazione_DB,
                                                ProcediConIlSalvataggio
                                            ) _
                                        & "</DatiAgenda>"

                                ' controllo per evitare di passare id agenda = 0 in cancellazione
                                If TipoOperazione_DB = enum_TipoOperazioneDB.Cancellazione AndAlso outputID_Agenda = 0 Then
                                    Log_Import.AppendLine(CStr(Date.Now) + " - Errore, recode dell'agenda " & inputID_Agenda & " non trovato ")
                                    ProcediConIlSalvataggio = False
                                End If

                                If ProcediConIlSalvataggio Then

                                    If objOpzioni.isGias2Gias_local Then
                                        scriviAgenda.Agenda_Scrivi(sStringaDaSalvare,
                                            outputID_Agenda,
                                            0,
                                            0,
                                            0,
                                            objOpzioni.SuperUser_CodFiscale_DESTINAZIONE,
                                            objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                            CodiciRimappati
                                    )
                                    Else

                                        '  Marco Grilli, 17/06/2014 14:57:29: zippo la stringa in gzip per velocizzare il trasferimento
                                        sStringaDaSalvare = AgronicaCoreUtility.AgroZip.CompressioneBase64(1, sStringaDaSalvare)

                                        Dim wsImportazione As New WS_Importa_GIAS_2014.ImportaWS With {.Url = objOpzioni.wsimportaGiasURl, .Timeout = _timeout_ws_Importa}
                                        Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)
                                        outputID_Agenda = wsImportazione.Scrivi_AgendaXmlPrivatoReverse(Str_Credenziali_WS, CodiciRimappati, sStringaDaSalvare, NuovaLogicaRecodeAG)
                                    End If

                                    If outputID_Agenda <> -1 OrElse TipoOperazione_DB <> enum_TipoOperazioneDB.Scrittura Then
                                        If NuovaLogicaRecodeAG Then
                                            G2GUtility.Scrivi_G2G_Recode_AgendaReverse(CodiciRimappati, TipoOperazione_DB, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                                        Else
                                            SalvataggioCodiciRimappati(CodiciRimappati, TipoOperazione_DB)
                                        End If
                                        conteggioOperazioni += 1
                                    Else
                                        Log_Import.AppendLine(CStr(Date.Now) + " - Errore, l'agenda " & inputID_Agenda & " non è stata trasferita ")
                                    End If

                                End If
                                'end procedi con il salvataggio

                            Catch ex As Exception

                                Log_Import.AppendLine(CStr(Date.Now) + " - Errore, l'agenda " & inputID_Agenda & " non è stata trasferita ")

                            End Try

                        End If

                    End If
                    'test per capire l'intervallo di validità è valido

                Next
                'operazione di agenda

                Dim xTesto As String = ""
                Select Case TipoOperazione_DB
                    Case enum_TipoOperazioneDB.Scrittura
                        xTesto = "nuove"
                    Case enum_TipoOperazioneDB.Modifica
                        xTesto = "modificate"
                    Case enum_TipoOperazioneDB.Cancellazione
                        xTesto = "eliminate"
                End Select

                Log_Import.AppendLine(CStr(Date.Now) + " - Trasferimento Agenda: " & conteggioOperazioni & " operazioni " & xTesto)

            End If
        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) + " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Private Shared Sub AgendaOttieniDataOperazioneDaXML(singleAgenda As XmlNode, ByRef DataOperazione As Date?)
        Dim nodoPrimoMovimento As XmlNode = singleAgenda.SelectSingleNode("DatiMovimenti/Movimento")
        If Not nodoPrimoMovimento Is Nothing Then
            Dim attrData As XmlAttribute = nodoPrimoMovimento.Attributes("data_movimento")
            If Not attrData Is Nothing Then
                Dim d1 As Date
                Dim esito As Boolean =
                    DateTime.TryParse(attrData.Value, New Globalization.CultureInfo("it-IT"), Globalization.DateTimeStyles.None, d1)
                If esito Then
                    DataOperazione = d1
                End If
            End If
        End If

        If DataOperazione Is Nothing Then
            Dim attrDataVal As XmlAttribute = singleAgenda.Attributes("validita_inizio")
            If Not attrDataVal Is Nothing Then
                Dim d1 As Date
                Dim esito As Boolean =
                    DateTime.TryParse(attrDataVal.Value, New Globalization.CultureInfo("it-IT"), Globalization.DateTimeStyles.None, d1)
                If esito Then
                    DataOperazione = d1
                End If
            End If
        End If

    End Sub

    Private Shared Function ConfigurazioneG2GLeggiAgenda(objOpzioniImportImpresa As clsImpresa) As G2G_Configurazione_FiltriReq_Agenda
        Dim oConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda

        If Not String.IsNullOrEmpty(objOpzioniImportImpresa.configurazione_agenda) Then
            oConfigurazioneAgenda = JsonConvert.DeserializeObject(Of G2G_Configurazione_FiltriReq_Agenda)(objOpzioniImportImpresa.configurazione_agenda)
        Else
            oConfigurazioneAgenda = New G2G_Configurazione_FiltriReq_Agenda
            oConfigurazioneAgenda.listaCauMovEsclusi = New List(Of String)
            oConfigurazioneAgenda.listaLavCod = New List(Of Integer)
        End If

        Return oConfigurazioneAgenda
    End Function

    Private Function EstraiCodiciDaXmlOrigine(ByVal ConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda, ByVal pivaSuperUser As String, ByVal SingleAgenda As XmlNode) As String

        Dim rval As String = "Agenda:" & pivaSuperUser & "," & SingleAgenda.Attributes("piva").Value & "," & SingleAgenda.Attributes("sa_cod").Value & "," & SingleAgenda.Attributes("id_agenda").Value & "|"

        If IsNumeric(SingleAgenda.Attributes("raccoglitore_cod").Value) AndAlso SingleAgenda.Attributes("raccoglitore_cod").Value <> "0" Then
            rval &= "Raccoglitore:" & pivaSuperUser & "," & SingleAgenda.Attributes("piva").Value & "," & SingleAgenda.Attributes("raccoglitore_cod").Value & "|"
        End If

        For Each nMovimenti As XmlNode In SingleAgenda.SelectNodes("DatiMovimenti/Movimento")


            ' VAnni: 21/9/2018: escludo i nodi di tipo Contatto e di tipo Macchina..
            ' VAnni: 8/7/2019: ora i nodi da non inviare sono memorizzati nelle impostazioni...
            'If Not ({CAU_IMPUTAZIONE_MANODOPERA, CAU_IMPUTAZIONE_PARCOMACCHINE, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE, CAU_IMPUTAZIONE_TERZISTI}).Contains(nMovimenti.Attributes("cau_mov").Value) Then
            If Not ConfigurazioneAgenda.listaCauMovEsclusi.Contains(nMovimenti.Attributes("cau_mov").Value) Then

                rval &= "Movimento:" & pivaSuperUser & "," &
                nMovimenti.Attributes("piva").Value & "," &
                nMovimenti.Attributes("sa_cod").Value & "," &
                nMovimenti.Attributes("id_agenda").Value & "," &
                nMovimenti.Attributes("id_mov").Value & "|"

                For Each nMovimentiDettagli As XmlNode In nMovimenti.SelectNodes("DatiMovimenti_Dettagli/Movimento_Dettaglio")
                    rval &= "Movimento_Dettaglio:" & pivaSuperUser & "," &
                    nMovimentiDettagli.Attributes("piva").Value & "," &
                    nMovimentiDettagli.Attributes("sa_cod").Value & "," &
                    nMovimentiDettagli.Attributes("id_agenda").Value & "," &
                    nMovimentiDettagli.Attributes("id_mov").Value & "," &
                    nMovimentiDettagli.Attributes("id_mov_det").Value & "|"
                Next
            End If

        Next


        Return rval.TrimEnd("|")

    End Function



    Private Sub SalvataggioCodiciRimappati(ByVal CodiciRimappati As String, ByVal TipoOperazioneBD As enum_TipoOperazioneDB)

        Dim vCodiciRimappati As String() = CodiciRimappati.Split("|")

        For Each c In vCodiciRimappati

            Dim vCod As String() = c.Split(":")
            Dim Valori As String() = vCod(1).Split("*")

            If vCod(0) <> "Agenda" And TipoOperazioneBD = enum_TipoOperazioneDB.Modifica Then
                Exit Sub
            End If

            Dim vvVecchi As String() = Valori(0).Split(",")
            Dim vvNuovi As String() = Valori(1).Split(",")

            Select Case vCod(0)
                Case "Agenda"

                    If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then
                        AgendaADD(
                        New G2G_Recode_Agenda With {
                            .From_PivaSuperUser = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.SuperUser),
                            .To_PivaSuperUser = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.SuperUser),
                            .FromPiva = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.piva),
                            .FromSa_cod = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.sa_Cod),
                            .FromId_Agenda = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.id_Agenda),
                            .ToPiva = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.piva),
                            .ToSa_cod = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.sa_Cod),
                            .ToId_Agenda = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.id_Agenda),
                            .RiferimentiElaborati = 0
                    })
                    Else
                        AgendaDelete(
                        New G2G_Recode_Agenda With {
                            .From_PivaSuperUser = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.SuperUser),
                            .To_PivaSuperUser = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.SuperUser),
                            .FromPiva = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.piva),
                            .FromSa_cod = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.sa_Cod),
                            .FromId_Agenda = vvVecchi(EnumFunzioni.Enum_G2G_Agenda.id_Agenda),
                            .ToPiva = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.piva),
                            .ToSa_cod = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.sa_Cod),
                            .ToId_Agenda = vvNuovi(EnumFunzioni.Enum_G2G_Agenda.id_Agenda),
                            .RiferimentiElaborati = 0
                        })

                    End If

                Case "Movimento"

                    If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then
                        MovimentiADD(
                        New G2G_Recode_Movimenti With {
                            .From_PivaSuperUser = vvVecchi(EnumFunzioni.Enum_G2G_Movimenti.SuperUser),
                            .To_PivaSuperUser = vvNuovi(EnumFunzioni.Enum_G2G_Movimenti.SuperUser),
                            .FromPiva = vvVecchi(EnumFunzioni.Enum_G2G_Movimenti.piva),
                            .FromSa_cod = vvVecchi(EnumFunzioni.Enum_G2G_Movimenti.sa_Cod),
                            .FromId_Agenda = vvVecchi(EnumFunzioni.Enum_G2G_Movimenti.id_Agenda),
                            .FromId_mov = vvVecchi(EnumFunzioni.Enum_G2G_Movimenti.id_mov),
                            .ToPiva = vvNuovi(EnumFunzioni.Enum_G2G_Movimenti.piva),
                            .ToSa_cod = vvNuovi(EnumFunzioni.Enum_G2G_Movimenti.sa_Cod),
                            .ToId_Agenda = vvNuovi(EnumFunzioni.Enum_G2G_Movimenti.id_Agenda),
                            .ToId_mov = vvNuovi(EnumFunzioni.Enum_G2G_Movimenti.id_mov)
                        })
                    End If

                Case "Movimento_Dettaglio"

                    If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then
                        Mov_DettagliADD(
                        New G2G_Recode_Mov_Dettagli With {
                            .From_PivaSuperUser = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.SuperUser),
                            .To_PivaSuperUser = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.SuperUser),
                            .FromPiva = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.piva),
                            .FromSa_cod = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.sa_Cod),
                            .FromId_Agenda = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_Agenda),
                            .FromId_mov = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_mov),
                            .FromId_mov_det = vvVecchi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_mov_det),
                            .ToPiva = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.piva),
                            .ToSa_cod = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.sa_Cod),
                            .ToId_Agenda = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_Agenda),
                            .ToId_mov = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_mov),
                            .ToId_mov_det = vvNuovi(EnumFunzioni.Enum_G2G_Mov_Dettagli.id_mov_det)
                        })

                    End If

                Case "Raccoglitore"

                    If TipoOperazioneBD = enum_TipoOperazioneDB.Scrittura Then
                        RaccoglitoreADD(
                        New G2G_Recode_Raccoglitore With {
                            .From_PivaSuperUser = vvVecchi(EnumFunzioni.Enum_G2G_Raccoglitore.SuperUser),
                            .To_PivaSuperUser = vvNuovi(EnumFunzioni.Enum_G2G_Raccoglitore.SuperUser),
                            .From_Piva = vvVecchi(EnumFunzioni.Enum_G2G_Raccoglitore.piva),
                            .From_Raccoglitore_Cod = vvVecchi(EnumFunzioni.Enum_G2G_Raccoglitore.raccoglitore_cod),
                            .To_Piva = vvNuovi(EnumFunzioni.Enum_G2G_Raccoglitore.piva),
                            .To_Raccoglitore_Cod = vvNuovi(EnumFunzioni.Enum_G2G_Raccoglitore.raccoglitore_cod)
                        })

                    End If

            End Select
        Next
    End Sub

    Public Function Elabora_XML_Agenda_SistemaXml(ByVal objOpzioniImportImpresa As clsImpresa,
                                                  ByVal objOpzioni As clsOpzioni,
                                                  ByRef Log_Import As StringBuilder,
                                                  ByRef Log_Errori As StringBuilder,
                                                  ByRef Log_Riepilogo As StringBuilder,
                                                  ByRef Str_XML_Agenda As String,
                                                  ByVal Piva_Origine As String,
                                                  ByVal Piva_Destinazione As String,
                                                  ByVal To_idAgenda As Integer,
                                                  ByVal To_Raccoglitore_Cod As Integer,
                                                  ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                  ByRef ProcediConIlSalvataggio As Boolean
                                                  ) As String

        Const nomeFunzione = "Elabora_XML_Agenda_SistemaXml"

        ProcediConIlSalvataggio = True

        Dim XmlDocCont As New XmlDocument

        Dim Str_XML_Output As String = ""
        XmlDocCont.LoadXml(Str_XML_Agenda)

        '  Vanni, 05/06/2017 16:48:43: in caso di modifica rimuovo tutti i figli per evitare di eliminare record di tipo movimento, mov_dettagli ecc... sul server.        
        If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

            Dim xL As XmlNodeList = XmlDocCont.SelectNodes("//Agenda")
            For Each xE In xL
                xE.IsEmpty = True
            Next

        End If


        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Dim LavCodOperazioneG2G As Integer = -1

        Try

            Dim oConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda = ConfigurazioneG2GLeggiAgenda(objOpzioniImportImpresa)

            Dim piva_SuperUser_DESTINAZIONE As String = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE


            'Rimuovo ciascun cau_Mov come da configurazione...
            For Each cCauMovDaEsculdere As String In oConfigurazioneAgenda.listaCauMovEsclusi

                RimuovoElementiMovimentoPerCauMov(XmlDocCont, cCauMovDaEsculdere)

            Next

            ' VAnni: 21/9/2018: escludo i nodi di tipo Contatto e di tipo Macchina..
            ' VAnni: 4/7/2019: ora che esiste la configurazione questi cau_mov da escludere vengono letti da elenco
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_MANODOPERA)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_TERZISTI)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_PARCOMACCHINE)

            '  Vanni, 09/06/2014 11:17:39: escludo i nodi dei riferimenti sulle operazioni riferite.


            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi(i)
                If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", "3")
                Else
                    XML_Nodo.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
                End If

            Next


            'rimuovo le date vuote "00.00.00" e le stringhe vuote
            RimuovoAttributiPerValore(XmlDocCont, XMLs_Nodi, XML_Nodo, i, objOpzioni.FormatoOraZero)
            RimuovoAttributiPerValore(XmlDocCont, XMLs_Nodi, XML_Nodo, i, "")


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_agenda]")
            LavCodOperazioneG2G = XMLs_Nodi(0).Attributes("lav_cod").Value
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                If TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                    XML_Nodo.SetAttribute("id_agenda", 0)
                Else
                    XML_Nodo.SetAttribute("id_agenda", To_idAgenda)
                End If
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@piva]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim ricodificaSA_COD As Integer = GetRicodificaSA_COD(Piva_Origine,
                                                                      XML_Nodo.GetAttribute("sa_cod"),
                                                                      objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                                      piva_SuperUser_DESTINAZIONE)

                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", ricodificaSA_COD)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@mat_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("mat_cod",
                                      recode_return_MatCod(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime))

            Next

            'DRUDI 2019-07-11   Cal_Cod
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cal_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim old_cal_cod As Integer = XML_Nodo.Attributes("cal_cod").Value

                XML_Nodo.SetAttribute("cal_cod",
                                      recode_return_CalCod(old_cal_cod, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))
            Next

            'DRUDI 2019-10-21 ID_Attivita
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_attivita]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("id_attivita",
                                      recode_return_ID_Attivita(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ID_Attivita]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("ID_Attivita",
                                      recode_return_ID_Attivita(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))

            Next

            ' VAnni: 21/9/2018: todo: sostituire anche: movimenti: -- [dbo].[Mov_Dettaglio_Tecnico_Extra]: [Agente_Cod], [CapoArea_Cod]


            Elabora_XML_SostituzioneContattoIndirizzo("cod_risum", "cod_indirizzorisum", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzo("cod_destinazione", "cod_indirizzodestinazione", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzo("cod_vettore", "cod_indirizzovettore", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzo("cod_risum_aggiuntivo", "cod_indirizzo_aggiuntivo", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzo("Cod_RisUm_Altro", "", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)


            Dim DestinazioniRimosse As Integer = 0
            Dim DestinazioniTotali As Integer = 0
            Dim MovDestinazioniAppoggioSommateDet As New List(Of Movimenti_dettagli)
            Dim DestinazioniTotale_Destinazione_Qta2 As Decimal
            Dim DestinazioniTotale_Destinazione_Qta2_Elaborata As Boolean = False
            Dim CauMov_ScaricoSuImpianti As String

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@raccoglitore_cod]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("raccoglitore_cod", To_Raccoglitore_Cod)
            Next

            Dim Prec_From_Id_Mov As Integer = -1
            Dim Prec_From_Id_Mov_Det As Integer = -1

            'Ricodifico le destinazioni (Impianti e fabbricati)
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_destinazione]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim lsa_cod_Ricodificato As String = XML_Nodo.GetAttribute("sa_cod")
                Dim lsa_Cod_From = GetOrigineSA_COD(Piva_Destinazione, lsa_cod_Ricodificato,
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                    piva_SuperUser_DESTINAZIONE)

                Select Case CInt(XML_Nodo.Attributes("tipo_destinazione").Value)
                    Case 0 ' impianto

                        DestinazioniTotali += 1

                        CauMov_ScaricoSuImpianti = XML_Nodo.ParentNode.Attributes("cau_mov").Value

                        If Not DestinazioniTotale_Destinazione_Qta2_Elaborata Then
                            DestinazioniTotale_Destinazione_Qta2 += CDec(XML_Nodo.Attributes("qta2").Value.Replace(",", SeparatoreDecimaleVB))
                        End If

                        Dim _To_Appezza As Integer
                        Dim _To_Id_reg As Integer
                        Dim _From_Id_Mov As Integer = CInt(XML_Nodo.Attributes("id_mov").Value)
                        Dim _From_Id_Mov_det As Integer = CInt(XML_Nodo.Attributes("id_mov_det").Value)

                        If Prec_From_Id_Mov_Det <> _From_Id_Mov_det Then
                            If Prec_From_Id_Mov_Det > 0 Then
                                DestinazioniTotale_Destinazione_Qta2_Elaborata = True
                            End If
                            Prec_From_Id_Mov_Det = _From_Id_Mov_det
                        End If

                        Dim _From_Appezza As Integer = CInt(XML_Nodo.Attributes("appezza").Value)
                        Dim _From_Id_reg As Integer = CInt(XML_Nodo.Attributes("id_destinazione").Value)

                        Dim impiantoNew As G2G_Recode_Impianti = recode_return_Impianto(Piva_Origine, lsa_Cod_From,
                                                                                        _From_Appezza, _From_Id_reg,
                                                                                        piva_SuperUser_DESTINAZIONE)

                        ' VAnni: 20/5/2019: se il nodo relativo ad impianto non è presente nella lista filtro degli impianti allora lo rimuovo.
                        '   tutto work in progress...
                        Dim xRimuoviNodo As Boolean = ((
                            From imp1 In objOpzioniImportImpresa.Impianti
                            Where imp1.Piva = Piva_Origine _
                                And imp1.Sa_Cod = lsa_Cod_From _
                                And imp1.Appezza = _From_Appezza _
                                And imp1.ID_Reg = _From_Id_reg
                            ).ToList.Count = 0)


                        Dim MessaggioAggiuntivo As String = "Nessun filtro impostato sugli impianti"
                        If objOpzioniImportImpresa.Impianti.Count > 0 Then
                            MessaggioAggiuntivo = "Esiste un filtro impostato sugli impianti"
                        End If

                        If objOpzioniImportImpresa.Impianti.Count > 0 AndAlso xRimuoviNodo Then

                            'da rimuovere
                            DestinazioniRimosse += 1

                            Dim rimuovi As XmlNode = XML_Nodo.ParentNode
                            rimuovi.RemoveChild(XML_Nodo)

                        Else

                            'non esiste recode ed il nodo non è da rimuovere
                            If impiantoNew Is Nothing Then
                                Dim messaggioErroreImpiantoNonMappato As String
                                messaggioErroreImpiantoNonMappato = MessaggioErroreImpiantoNonMappato1(MessaggioAggiuntivo, Piva_Origine, lsa_Cod_From, _From_Appezza, _From_Id_reg)
                                Throw New Exception(messaggioErroreImpiantoNonMappato)
                            End If

                            'nulla da rimuovere!
                            If CInt(XML_Nodo.Attributes("appezza").Value) = 0 Then
                                _To_Appezza = 0
                            Else
                                _To_Appezza = impiantoNew.To_Appezza
                            End If

                            If CInt(XML_Nodo.Attributes("id_destinazione").Value) = 0 Then
                                _To_Id_reg = 0
                            Else
                                _To_Id_reg = impiantoNew.To_Id_Reg
                            End If

                            XML_Nodo.SetAttribute("appezza", _To_Appezza)
                            XML_Nodo.SetAttribute("id_destinazione", _To_Id_reg)
                        End If


                    Case 20, 15 'magazzino, stalla

                        'la stalla è equiparata al magazzino perché esiste uno Sta_Num in Stalla che è uguale
                        ' al Fabbricato_Cod in Fabbricati, perciò lo mappo come se fosse un magazzino normale

                        Dim fromFab_Cod As Integer = CInt(XML_Nodo.Attributes("id_destinazione").Value)
                        Dim objFabbricato As G2G_Recode_Fabbricati = recode_return_Fabbricato(Piva_Origine, lsa_Cod_From,
                                                                                              fromFab_Cod,
                                                                                              piva_SuperUser_DESTINAZIONE)

                        If objFabbricato Is Nothing Then
                            Throw New Exception("Fabbricato non trovato in Recode")
                        End If

                        XML_Nodo.SetAttribute("appezza", 0)
                        XML_Nodo.SetAttribute("id_destinazione", objFabbricato.To_FabbricatoCod)

                End Select


            Next
            'destinazione ... x ricodifica

            'ho rimosso tutte le destinazioni, non devo procedere al salvataggio
            'se entrambi le variabili valgono zero, potrebbe trattarsi di operazioni senza impianti, quindi da trasferire: 
            ' da qui il test su DestinazioniRimosse > 0.
            If DestinazioniRimosse = DestinazioniTotali And DestinazioniRimosse > 0 Then
                ProcediConIlSalvataggio = False
            End If

            If ProcediConIlSalvataggio Then

                'per le sole operazioni previste e se è stato rimossa una destinazione
                If DestinazioniRimosse > 0 AndAlso DecidiSeInfoDettaglioVannoAggiornate(LavCodOperazioneG2G) Then

                    ''Reimposto lo scarico in seguito a rimozione di impianti da destinazione

                    Dim DatiMov_Dettagli_Tecnici_elaborati As Boolean = False

                    Dim xmlNodeListaMovimentiDettagli As XmlNodeList =
                        XmlDocCont.SelectNodes("//Movimento[@cau_mov='" & CauMov_ScaricoSuImpianti & "']/DatiMovimenti_Dettagli/Movimento_Dettaglio")

                    For Each xmlNodeMovDet As XmlNode In xmlNodeListaMovimentiDettagli

                        Dim xmlNodeListaDestinazioni As XmlNodeList =
                            xmlNodeMovDet.SelectNodes("Movimento_Destinazione")

                        Dim DestinazioneAppoggioSommate As New Movimenti_dettagli
                        DestinazioneAppoggioSommate.Qta_Dettaglio1 = 0
                        DestinazioneAppoggioSommate.Qta_Dettaglio2 = 0
                        DestinazioneAppoggioSommate.Elem_Cod = xmlNodeMovDet.Attributes("elem_cod").Value
                        DestinazioneAppoggioSommate.Pro_Cod = xmlNodeMovDet.Attributes("pro_cod").Value
                        DestinazioneAppoggioSommate.Mat_Cod = xmlNodeMovDet.Attributes("mat_cod").Value


                        For Each xmlNodeMovDestinazione As XmlNode In xmlNodeListaDestinazioni

                            DestinazioneAppoggioSommate.Qta_Dettaglio1 += CDec(xmlNodeMovDestinazione.Attributes("qta").Value.Replace(",", SeparatoreDecimaleVB))
                            DestinazioneAppoggioSommate.Qta_Dettaglio2 += CDec(xmlNodeMovDestinazione.Attributes("qta2").Value.Replace(",", SeparatoreDecimaleVB))

                        Next
                        'destinazione

                        If DestinazioneAppoggioSommate.Qta_Dettaglio1 > 0 Then


                            'Qta (Dose_Ha_Reale)	
                            'Qta_Extra (Dose_Hl_Reale)

                            'Qta_Extra_totale (Dose_Totale_Reale)	
                            xmlNodeMovDet.Attributes("qta_extra_totale").Value =
                                DestinazioneAppoggioSommate.Qta_Dettaglio1.ToString.Replace(SeparatoreDecimaleVB, ",")

                            'Udm_cod_Extra (Dose_QtaTotale)(Se vale 11 è "Quantità Dose", se vale 10 è "Totale"	
                            'Qta_Ril (Acqua (negativa = /ha; positiva = tot)/Dose x irrigazione/Pioggia x rilievo piogge)
                            Dim xmlNodeH2O As XmlElement = XmlDocCont.SelectSingleNode("//DatiMov_Dettagli_Tecnici/Movimento_Dettaglio_Tecnico")
                            If Not xmlNodeH2O Is Nothing AndAlso Not xmlNodeH2O.Attributes("qta_ril").Value.StartsWith("-") AndAlso Not DatiMov_Dettagli_Tecnici_elaborati Then
                                DatiMov_Dettagli_Tecnici_elaborati = True

                                Dim h2o_TotOld As Decimal =
                                    xmlNodeH2O.Attributes("qta_ril").Value.Replace(",", SeparatoreDecimaleVB)

                                h2o_TotOld = Math.Round(h2o_TotOld * (CDec(DestinazioneAppoggioSommate.Qta_Dettaglio2) / DestinazioniTotale_Destinazione_Qta2), 4)
                                xmlNodeH2O.SetAttribute("qta_ril", h2o_TotOld.ToString.Replace(SeparatoreDecimaleVB, ","))

                            End If

                        End If
                        'se una destinazione ce l'ha .. 

                        MovDestinazioniAppoggioSommateDet.Add(DestinazioneAppoggioSommate)

                    Next
                    'dettaglio di tipo distribuzione



                    'Reimposto lo scarico di magazzino in seguito a rimozione di prodotto (se il nodo era stato rimosso in precedenza viene ignorato)
                    Dim xmlNodeListaMovimentiDettagliMagazzino As XmlNodeList =
                        XmlDocCont.SelectNodes("//Movimento[@cau_mov='" & CAU_SCARICO & "']/DatiMovimenti_Dettagli/Movimento_Dettaglio")

                    For Each xmlNodeDettaglioMagazzino As XmlNode In xmlNodeListaMovimentiDettagliMagazzino

                        Dim detMage_elem_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("elem_cod").Value
                        Dim detMage_pro_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("pro_cod").Value
                        Dim detMage_mat_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("mat_cod").Value

                        Dim qtaNuova As Decimal = (From d In MovDestinazioniAppoggioSommateDet
                                                   Where d.Elem_Cod = detMage_elem_cod _
                                                       And d.Pro_Cod = detMage_pro_cod _
                                                       And d.Mat_Cod = detMage_mat_cod).First.Qta_Dettaglio1

                        xmlNodeDettaglioMagazzino.Attributes("qta").Value = qtaNuova.ToString.Replace(SeparatoreDecimaleVB, ",")

                        Dim xDestinazioneMagazzino As XmlElement = xmlNodeDettaglioMagazzino.SelectSingleNode("Movimento_Destinazione")
                        xDestinazioneMagazzino.SetAttribute("qta", qtaNuova.ToString.Replace(SeparatoreDecimaleVB, ","))

                    Next
                    'fine Reimposto lo scarico di magazzino in seguito a rimozione di prodotto


                End If
                'fine per le sole operazioni previste e se è stato rimossa una destinazione



                ' VAnni: 20/8/2019: azzeramento degli id
                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_mov]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_mov", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_mov_det]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_mov_det", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_reg_dettaglio]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_reg_dettaglio", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cod_pagamento]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("cod_pagamento", 0)
                Next
                'Ricodifico il Cod_Progetto (Distinta)
                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cod_progetto]")
                For i = 0 To XMLs_Nodi.Count - 1
                    XML_Nodo = XMLs_Nodi.Item(i)

                    Dim fromCodProgetto As Integer = CInt(XML_Nodo.Attributes("cod_progetto").Value)
                    If fromCodProgetto <> 0 Then

                        'TODO: se l'elem_cod è uno di quelli del mondo animale, allora qui potrei avere la distinta dell'animale (Zoo_Animali_Distinte)
                        Dim objDistinta As G2G_Recode_Distinta = recode_return_Distinta(Piva_Origine, fromCodProgetto, piva_SuperUser_DESTINAZIONE)

                        If objDistinta Is Nothing Then
                            Throw New Exception("Distinta non trovato in Recode")
                        End If

                        XML_Nodo.SetAttribute("cod_progetto", objDistinta.To_Progetto_cod)
                    End If
                Next


            End If
            'se procedo con il salvataggio

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.ToString
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output


    End Function

    Public Function Elabora_XML_Agenda_SistemaXmlReverse(ByVal objOpzioniImportImpresa As clsImpresa,
                                                  ByVal objOpzioni As clsOpzioni,
                                                  ByRef Log_Import As StringBuilder,
                                                  ByRef Log_Errori As StringBuilder,
                                                  ByRef Log_Riepilogo As StringBuilder,
                                                  ByRef Str_XML_Agenda As String,
                                                  ByVal Piva_Origine As String,
                                                  ByVal Piva_Destinazione As String,
                                                  ByVal From_idAgenda As Integer,
                                                  ByVal From_Raccoglitore_Cod As Integer,
                                                  ByVal TipoOperazioneDB As enum_TipoOperazioneDB,
                                                  ByRef ProcediConIlSalvataggio As Boolean
                                                  ) As String

        Const nomeFunzione = "Elabora_XML_Agenda_SistemaXmlReverse"

        ProcediConIlSalvataggio = True

        Dim XmlDocCont As New XmlDocument

        Dim Str_XML_Output As String = ""
        XmlDocCont.LoadXml(Str_XML_Agenda)

        '  Vanni, 05/06/2017 16:48:43: in caso di modifica rimuovo tutti i figli per evitare di eliminare record di tipo movimento, mov_dettagli ecc... sul server.        
        If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then

            Dim xL As XmlNodeList = XmlDocCont.SelectNodes("//Agenda")
            For Each xE In xL
                xE.IsEmpty = True
            Next

        End If


        Dim XMLs_Nodi As XmlNodeList
        Dim XML_Nodo As XmlElement
        Dim i As Integer

        Dim LavCodOperazioneG2G As Integer = -1

        Try

            Dim oConfigurazioneAgenda As G2G_Configurazione_FiltriReq_Agenda = ConfigurazioneG2GLeggiAgenda(objOpzioniImportImpresa)

            Dim piva_SuperUser_DESTINAZIONE As String = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE


            'Rimuovo ciascun cau_Mov come da configurazione...
            For Each cCauMovDaEsculdere As String In oConfigurazioneAgenda.listaCauMovEsclusi

                RimuovoElementiMovimentoPerCauMov(XmlDocCont, cCauMovDaEsculdere)

            Next

            ' VAnni: 21/9/2018: escludo i nodi di tipo Contatto e di tipo Macchina..
            ' VAnni: 4/7/2019: ora che esiste la configurazione questi cau_mov da escludere vengono letti da elenco
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_MANODOPERA)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_TECNICO_RESPONSABILE)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_TERZISTI)
            'RimuovoElementiMovimentoPerCauMov(XmlDocCont, CAU_IMPUTAZIONE_PARCOMACCHINE)

            '  Vanni, 09/06/2014 11:17:39: escludo i nodi dei riferimenti sulle operazioni riferite.


            'tipo operazione = scrittura
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@TipoOperazioneDB]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi(i)
                If TipoOperazioneDB = enum_TipoOperazioneDB.Modifica Then
                    XML_Nodo.SetAttribute("TipoOperazioneDB", "3")
                Else
                    XML_Nodo.SetAttribute("TipoOperazioneDB", TipoOperazioneDB)
                End If

            Next


            'rimuovo le date vuote "00.00.00" e le stringhe vuote
            RimuovoAttributiPerValore(XmlDocCont, XMLs_Nodi, XML_Nodo, i, objOpzioni.FormatoOraZero)
            RimuovoAttributiPerValore(XmlDocCont, XMLs_Nodi, XML_Nodo, i, "")


            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_agenda]")
            LavCodOperazioneG2G = XMLs_Nodi(0).Attributes("lav_cod").Value
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)
                'azzero il cod_indirizzo che verrà assegnato dal core
                If TipoOperazioneDB = enum_TipoOperazioneDB.Scrittura Then
                    XML_Nodo.SetAttribute("id_agenda", 0)
                Else
                    XML_Nodo.SetAttribute("id_agenda", From_idAgenda)
                End If
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@piva]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim ricodificaSA_COD As Integer = GetRicodificaSA_CODReverse(Piva_Origine,
                                                                      XML_Nodo.GetAttribute("sa_cod"),
                                                                      objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                                      piva_SuperUser_DESTINAZIONE)

                XML_Nodo.SetAttribute("piva", Piva_Destinazione)
                XML_Nodo.SetAttribute("sa_cod", ricodificaSA_COD)
                XML_Nodo.SetAttribute("basecode", objOpzioni.BaseCode_DESTINAZIONE)
                XML_Nodo.SetAttribute("topcode", objOpzioni.TopCode_DESTINAZIONE)

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@mat_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("mat_cod",
                                      recode_return_MatCodReverse(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE, objOpzioniImportImpresa.Flagimporta_materieprime))

            Next

            'DRUDI 2019-07-11   Cal_Cod
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cal_cod]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim old_cal_cod As Integer = XML_Nodo.Attributes("cal_cod").Value

                XML_Nodo.SetAttribute("cal_cod",
                                      recode_return_CalCodReverse(old_cal_cod, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))
            Next

            'DRUDI 2019-10-21 ID_Attivita
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_attivita]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("id_attivita",
                                      recode_return_ID_AttivitaReverse(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))

            Next

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@ID_Attivita]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                'XML_Nodo.SetAttribute("mat_cod", recode_dammi_MatCod(Piva_Origine, sa_cod_origine, XML_Nodo))
                XML_Nodo.SetAttribute("ID_Attivita",
                                      recode_return_ID_AttivitaReverse(XML_Nodo, Piva_Destinazione, piva_SuperUser_DESTINAZIONE))

            Next

            ' VAnni: 21/9/2018: todo: sostituire anche: movimenti: -- [dbo].[Mov_Dettaglio_Tecnico_Extra]: [Agente_Cod], [CapoArea_Cod]


            Elabora_XML_SostituzioneContattoIndirizzoReverse("cod_risum", "cod_indirizzorisum", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzoReverse("cod_destinazione", "cod_indirizzodestinazione", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzoReverse("cod_vettore", "cod_indirizzovettore", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzoReverse("cod_risum_aggiuntivo", "cod_indirizzo_aggiuntivo", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)
            Elabora_XML_SostituzioneContattoIndirizzoReverse("Cod_RisUm_Altro", "", XmlDocCont, XMLs_Nodi, XML_Nodo, piva_SuperUser_DESTINAZIONE)


            Dim DestinazioniRimosse As Integer = 0
            Dim DestinazioniTotali As Integer = 0
            Dim MovDestinazioniAppoggioSommateDet As New List(Of Movimenti_dettagli)
            Dim DestinazioniTotale_Destinazione_Qta2 As Decimal
            Dim DestinazioniTotale_Destinazione_Qta2_Elaborata As Boolean = False
            Dim CauMov_ScaricoSuImpianti As String

            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@raccoglitore_cod]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("raccoglitore_cod", From_Raccoglitore_Cod)
            Next

            Dim Prec_From_Id_Mov As Integer = -1
            Dim Prec_From_Id_Mov_Det As Integer = -1

            'Ricodifico le destinazioni (Impianti e fabbricati)
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_destinazione]")
            For i = 0 To XMLs_Nodi.Count - 1
                XML_Nodo = XMLs_Nodi.Item(i)

                Dim lsa_cod_Ricodificato As String = XML_Nodo.GetAttribute("sa_cod")
                Dim lsa_Cod_From = GetOrigineSA_CODReverse(Piva_Destinazione, lsa_cod_Ricodificato,
                                                    objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                                                    piva_SuperUser_DESTINAZIONE)

                Select Case CInt(XML_Nodo.Attributes("tipo_destinazione").Value)
                    Case 0 ' impianto

                        DestinazioniTotali += 1

                        CauMov_ScaricoSuImpianti = XML_Nodo.ParentNode.Attributes("cau_mov").Value

                        If Not DestinazioniTotale_Destinazione_Qta2_Elaborata Then
                            DestinazioniTotale_Destinazione_Qta2 += CDec(XML_Nodo.Attributes("qta2").Value.Replace(",", SeparatoreDecimaleVB))
                        End If

                        Dim _To_Appezza As Integer
                        Dim _To_Id_reg As Integer
                        Dim _From_Id_Mov As Integer = CInt(XML_Nodo.Attributes("id_mov").Value)
                        Dim _From_Id_Mov_det As Integer = CInt(XML_Nodo.Attributes("id_mov_det").Value)

                        If Prec_From_Id_Mov_Det <> _From_Id_Mov_det Then
                            If Prec_From_Id_Mov_Det > 0 Then
                                DestinazioniTotale_Destinazione_Qta2_Elaborata = True
                            End If
                            Prec_From_Id_Mov_Det = _From_Id_Mov_det
                        End If

                        Dim _From_Appezza As Integer = CInt(XML_Nodo.Attributes("appezza").Value)
                        Dim _From_Id_reg As Integer = CInt(XML_Nodo.Attributes("id_destinazione").Value)

                        Dim impiantoNew As G2G_Recode_Impianti = recode_return_ImpiantoReverse(Piva_Origine, lsa_Cod_From,
                                                                                        _From_Appezza, _From_Id_reg,
                                                                                        piva_SuperUser_DESTINAZIONE)

                        ' VAnni: 20/5/2019: se il nodo relativo ad impianto non è presente nella lista filtro degli impianti allora lo rimuovo.
                        '   tutto work in progress...
                        Dim xRimuoviNodo As Boolean = ((
                            From imp1 In objOpzioniImportImpresa.Impianti
                            Where imp1.Piva = Piva_Origine _
                                And imp1.Sa_Cod = lsa_Cod_From _
                                And imp1.Appezza = _From_Appezza _
                                And imp1.ID_Reg = _From_Id_reg
                            ).ToList.Count = 0)


                        Dim MessaggioAggiuntivo As String = "Nessun filtro impostato sugli impianti"
                        If objOpzioniImportImpresa.Impianti.Count > 0 Then
                            MessaggioAggiuntivo = "Esiste un filtro impostato sugli impianti"
                        End If

                        'non esiste recode ed il nodo non è da rimuovere
                        If impiantoNew Is Nothing AndAlso Not xRimuoviNodo Then
                            Dim messaggioErroreImpiantoNonMappato As String
                            messaggioErroreImpiantoNonMappato = MessaggioErroreImpiantoNonMappato1(MessaggioAggiuntivo, Piva_Origine, lsa_Cod_From, _From_Appezza, _From_Id_reg)
                            Throw New Exception(messaggioErroreImpiantoNonMappato)
                        End If

                        If objOpzioniImportImpresa.Impianti.Count > 0 AndAlso xRimuoviNodo Then

                            'da rimuovere
                            DestinazioniRimosse += 1

                            Dim rimuovi As XmlNode = XML_Nodo.ParentNode
                            rimuovi.RemoveChild(XML_Nodo)

                        Else
                            'nulla da rimuovere!
                            If CInt(XML_Nodo.Attributes("appezza").Value) = 0 Then
                                _To_Appezza = 0
                            Else
                                _To_Appezza = impiantoNew.From_Appezza
                            End If

                            If CInt(XML_Nodo.Attributes("id_destinazione").Value) = 0 Then
                                _To_Id_reg = 0
                            Else
                                _To_Id_reg = impiantoNew.From_Id_Reg
                            End If

                            XML_Nodo.SetAttribute("appezza", _To_Appezza)
                            XML_Nodo.SetAttribute("id_destinazione", _To_Id_reg)
                        End If


                    Case 20, 15 'magazzino, stalla

                        'la stalla è equiparata al magazzino perché esiste uno Sta_Num in Stalla che è uguale
                        ' al Fabbricato_Cod in Fabbricati, perciò lo mappo come se fosse un magazzino normale

                        Dim fromFab_Cod As Integer = CInt(XML_Nodo.Attributes("id_destinazione").Value)
                        Dim objFabbricato As G2G_Recode_Fabbricati = recode_return_FabbricatoReverse(Piva_Origine, lsa_Cod_From,
                                                                                              fromFab_Cod,
                                                                                              piva_SuperUser_DESTINAZIONE)

                        If objFabbricato Is Nothing Then
                            Throw New Exception("Fabbricato non trovato in Recode")
                        End If

                        XML_Nodo.SetAttribute("appezza", 0)
                        XML_Nodo.SetAttribute("id_destinazione", objFabbricato.From_FabbricatoCod)

                End Select


            Next
            'destinazione ... x ricodifica

            'ho rimosso tutte le destinazioni, non devo procedere al salvataggio
            'se entrambi le variabili valgono zero, potrebbe trattarsi di operazioni senza impianti, quindi da trasferire: 
            ' da qui il test su DestinazioniRimosse > 0.
            If DestinazioniRimosse = DestinazioniTotali And DestinazioniRimosse > 0 Then
                ProcediConIlSalvataggio = False
            End If

            If ProcediConIlSalvataggio Then

                'per le sole operazioni previste e se è stato rimossa una destinazione
                If DestinazioniRimosse > 0 AndAlso DecidiSeInfoDettaglioVannoAggiornate(LavCodOperazioneG2G) Then

                    ''Reimposto lo scarico in seguito a rimozione di impianti da destinazione

                    Dim DatiMov_Dettagli_Tecnici_elaborati As Boolean = False

                    Dim xmlNodeListaMovimentiDettagli As XmlNodeList =
                        XmlDocCont.SelectNodes("//Movimento[@cau_mov='" & CauMov_ScaricoSuImpianti & "']/DatiMovimenti_Dettagli/Movimento_Dettaglio")

                    For Each xmlNodeMovDet As XmlNode In xmlNodeListaMovimentiDettagli

                        Dim xmlNodeListaDestinazioni As XmlNodeList =
                            xmlNodeMovDet.SelectNodes("Movimento_Destinazione")

                        Dim DestinazioneAppoggioSommate As New Movimenti_dettagli
                        DestinazioneAppoggioSommate.Qta_Dettaglio1 = 0
                        DestinazioneAppoggioSommate.Qta_Dettaglio2 = 0
                        DestinazioneAppoggioSommate.Elem_Cod = xmlNodeMovDet.Attributes("elem_cod").Value
                        DestinazioneAppoggioSommate.Pro_Cod = xmlNodeMovDet.Attributes("pro_cod").Value
                        DestinazioneAppoggioSommate.Mat_Cod = xmlNodeMovDet.Attributes("mat_cod").Value


                        For Each xmlNodeMovDestinazione As XmlNode In xmlNodeListaDestinazioni

                            DestinazioneAppoggioSommate.Qta_Dettaglio1 += CDec(xmlNodeMovDestinazione.Attributes("qta").Value.Replace(",", SeparatoreDecimaleVB))
                            DestinazioneAppoggioSommate.Qta_Dettaglio2 += CDec(xmlNodeMovDestinazione.Attributes("qta2").Value.Replace(",", SeparatoreDecimaleVB))

                        Next
                        'destinazione

                        If DestinazioneAppoggioSommate.Qta_Dettaglio1 > 0 Then


                            'Qta (Dose_Ha_Reale)	
                            'Qta_Extra (Dose_Hl_Reale)

                            'Qta_Extra_totale (Dose_Totale_Reale)	
                            xmlNodeMovDet.Attributes("qta_extra_totale").Value =
                                DestinazioneAppoggioSommate.Qta_Dettaglio1.ToString.Replace(SeparatoreDecimaleVB, ",")

                            'Udm_cod_Extra (Dose_QtaTotale)(Se vale 11 è "Quantità Dose", se vale 10 è "Totale"	
                            'Qta_Ril (Acqua (negativa = /ha; positiva = tot)/Dose x irrigazione/Pioggia x rilievo piogge)
                            Dim xmlNodeH2O As XmlElement = XmlDocCont.SelectSingleNode("//DatiMov_Dettagli_Tecnici/Movimento_Dettaglio_Tecnico")
                            If Not xmlNodeH2O Is Nothing AndAlso Not xmlNodeH2O.Attributes("qta_ril").Value.StartsWith("-") AndAlso Not DatiMov_Dettagli_Tecnici_elaborati Then
                                DatiMov_Dettagli_Tecnici_elaborati = True

                                Dim h2o_TotOld As Decimal =
                                    xmlNodeH2O.Attributes("qta_ril").Value.Replace(",", SeparatoreDecimaleVB)

                                h2o_TotOld = Math.Round(h2o_TotOld * (CDec(DestinazioneAppoggioSommate.Qta_Dettaglio2) / DestinazioniTotale_Destinazione_Qta2), 4)
                                xmlNodeH2O.SetAttribute("qta_ril", h2o_TotOld.ToString.Replace(SeparatoreDecimaleVB, ","))

                            End If

                        End If
                        'se una destinazione ce l'ha .. 

                        MovDestinazioniAppoggioSommateDet.Add(DestinazioneAppoggioSommate)

                    Next
                    'dettaglio di tipo distribuzione



                    'Reimposto lo scarico di magazzino in seguito a rimozione di prodotto (se il nodo era stato rimosso in precedenza viene ignorato)
                    Dim xmlNodeListaMovimentiDettagliMagazzino As XmlNodeList =
                        XmlDocCont.SelectNodes("//Movimento[@cau_mov='" & CAU_SCARICO & "']/DatiMovimenti_Dettagli/Movimento_Dettaglio")

                    For Each xmlNodeDettaglioMagazzino As XmlNode In xmlNodeListaMovimentiDettagliMagazzino

                        Dim detMage_elem_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("elem_cod").Value
                        Dim detMage_pro_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("pro_cod").Value
                        Dim detMage_mat_cod As Integer = xmlNodeDettaglioMagazzino.Attributes("mat_cod").Value

                        Dim qtaNuova As Decimal = (From d In MovDestinazioniAppoggioSommateDet
                                                   Where d.Elem_Cod = detMage_elem_cod _
                                                       And d.Pro_Cod = detMage_pro_cod _
                                                       And d.Mat_Cod = detMage_mat_cod).First.Qta_Dettaglio1

                        xmlNodeDettaglioMagazzino.Attributes("qta").Value = qtaNuova.ToString.Replace(SeparatoreDecimaleVB, ",")

                        Dim xDestinazioneMagazzino As XmlElement = xmlNodeDettaglioMagazzino.SelectSingleNode("Movimento_Destinazione")
                        xDestinazioneMagazzino.SetAttribute("qta", qtaNuova.ToString.Replace(SeparatoreDecimaleVB, ","))

                    Next
                    'fine Reimposto lo scarico di magazzino in seguito a rimozione di prodotto


                End If
                'fine per le sole operazioni previste e se è stato rimossa una destinazione



                ' VAnni: 20/8/2019: azzeramento degli id
                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_mov]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_mov", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_mov_det]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_mov_det", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@id_reg_dettaglio]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("id_reg_dettaglio", 0)
                Next

                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cod_pagamento]")
                For i = 0 To XMLs_Nodi.Count - 1

                    XML_Nodo = XMLs_Nodi.Item(i)
                    XML_Nodo.SetAttribute("cod_pagamento", 0)
                Next
                'Ricodifico il Cod_Progetto (Distinta)
                XMLs_Nodi = XmlDocCont.SelectNodes("//*[@cod_progetto]")
                For i = 0 To XMLs_Nodi.Count - 1
                    XML_Nodo = XMLs_Nodi.Item(i)

                    Dim fromCodProgetto As Integer = CInt(XML_Nodo.Attributes("cod_progetto").Value)
                    If fromCodProgetto <> 0 Then

                        'TODO: se l'elem_cod è uno di quelli del mondo animale, allora qui potrei avere la distinta dell'animale (Zoo_Animali_Distinte)
                        Dim objDistinta As G2G_Recode_Distinta = recode_return_DistintaReverse(Piva_Origine, fromCodProgetto, piva_SuperUser_DESTINAZIONE)

                        If objDistinta Is Nothing Then
                            Throw New Exception("Distinta non trovato in Recode")
                        End If

                        XML_Nodo.SetAttribute("cod_progetto", objDistinta.From_Progetto_cod)
                    End If
                Next


            End If
            'se procedo con il salvataggio

            Str_XML_Output = XmlDocCont.OuterXml

        Catch ex As Exception
            Dim msg As String = CStr(Date.Now) & " - " & nomeFunzione & " Si è verificato il seguente errore: " & ex.Message
            Log_Import.AppendLine(msg)
            Log_Errori.AppendLine(msg)
            Throw New Exception(msg)
        End Try

        Return Str_XML_Output


    End Function

    Private Function DecidiSeInfoDettaglioVannoAggiornate(lav_cod As Integer) As Boolean


        If {LAVCOD_DISTRIBUZIONE_CONCIME,
            LAVCOD_SARCHIATURA_CONCIMAZIONE,
            LAVCOD_DISTRIBUZIONE_AMMENDANTI,
            LAVCOD_FERTIRRIGAZIONE,
            LAVCOD_CONCIMAZIONE_FOGLIARE,
            LAVCOD_TRATTAMENTO_ANTIBUTTERATURA,
            LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
            LAVCOD_TRATTAMENTO_FITOREGOLATORE,
            LAVCOD_DISERBO,
            LAVCOD_GEODISINFESTAZIONE,
            LAVCOD_CONCIA_SEME,
            LAVCOD_DISSECCAMENTO}.Contains(lav_cod) Then
            Return True
        End If

        Return False
    End Function

    Private Shared Function MessaggioErroreImpiantoNonMappato1(MessaggioAggiuntivo As String, Piva_Origine As String, lsa_Cod_From As Integer, _From_Appezza As Integer, _From_Id_reg As Integer) As String
        Return "Elabora_XML_Agenda_SistemaXML: non è stata trovata alcuna decodifica per l'impianto (" & MessaggioAggiuntivo & ") con chiave: " &
            Piva_Origine & ", sa_Cod: " & lsa_Cod_From & ", appezza: " & _From_Appezza & ", id_reg:" & _From_Id_reg
    End Function

    Private Sub Elabora_XML_SostituzioneContattoIndirizzo(ByVal AttributoRisum As String, ByVal AttributoIndirizzo As String, XmlDocCont As XmlDocument, ByRef XMLs_Nodi As XmlNodeList, ByRef XML_Nodo As XmlElement, piva_SuperUser_DESTINAZIONE As String)
        Dim i As Integer
        XMLs_Nodi = XmlDocCont.SelectNodes("//*[@" & AttributoRisum & "]")
        For i = 0 To XMLs_Nodi.Count - 1

            XML_Nodo = XMLs_Nodi.Item(i)
            XML_Nodo.SetAttribute("" & AttributoRisum & "",
                                  recode_return_CodRisUm(XML_Nodo.Attributes("" & AttributoRisum & "").Value, piva_SuperUser_DESTINAZIONE))
        Next

        If Not String.IsNullOrEmpty(AttributoIndirizzo) Then
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@" & AttributoIndirizzo & "]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("" & AttributoIndirizzo & "",
                                      recode_return_Cod_Indirizzo(XML_Nodo.Attributes("" & AttributoIndirizzo & "").Value, piva_SuperUser_DESTINAZIONE))
            Next
        End If

    End Sub

    Private Sub Elabora_XML_SostituzioneContattoIndirizzoReverse(ByVal AttributoRisum As String, ByVal AttributoIndirizzo As String, XmlDocCont As XmlDocument, ByRef XMLs_Nodi As XmlNodeList, ByRef XML_Nodo As XmlElement, piva_SuperUser_DESTINAZIONE As String)
        Dim i As Integer
        XMLs_Nodi = XmlDocCont.SelectNodes("//*[@" & AttributoRisum & "]")
        For i = 0 To XMLs_Nodi.Count - 1

            XML_Nodo = XMLs_Nodi.Item(i)
            XML_Nodo.SetAttribute("" & AttributoRisum & "",
                                  recode_return_CodRisUmReverse(XML_Nodo.Attributes("" & AttributoRisum & "").Value, piva_SuperUser_DESTINAZIONE))
        Next

        If Not String.IsNullOrEmpty(AttributoIndirizzo) Then
            XMLs_Nodi = XmlDocCont.SelectNodes("//*[@" & AttributoIndirizzo & "]")
            For i = 0 To XMLs_Nodi.Count - 1

                XML_Nodo = XMLs_Nodi.Item(i)
                XML_Nodo.SetAttribute("" & AttributoIndirizzo & "",
                                      recode_return_Cod_IndirizzoReverse(XML_Nodo.Attributes("" & AttributoIndirizzo & "").Value, piva_SuperUser_DESTINAZIONE))
            Next
        End If

    End Sub

    Private Shared Sub RimuovoElementiMovimentoPerCauMov(ByRef XML_Doc As XmlDocument, ByVal Cau_Mov As String)

        Dim XMLs_Nodi As XmlNodeList = XML_Doc.SelectNodes("//Agenda/DatiMovimenti/Movimento[@cau_mov='" & Cau_Mov & "']")
        Dim XMLs_Padre As XmlNode = XML_Doc.SelectSingleNode("//Agenda/DatiMovimenti")

        For Each singleNode In XMLs_Nodi
            XMLs_Padre.RemoveChild(singleNode)
        Next

    End Sub

    Private Shared Sub RimuovoAttributiPerValore(ByVal XmlDocCont As XmlDocument, ByVal XMLs_Nodi As XmlNodeList, ByRef XML_Nodo As XmlElement, ByVal i As Integer, ByVal valoreDaSostituire As String)
        XMLs_Nodi = XmlDocCont.SelectNodes("//*[@*='" & valoreDaSostituire & "']")
        For i = 0 To XMLs_Nodi.Count - 1
            XML_Nodo = XMLs_Nodi(i)

            Dim toRemove As New List(Of String)
            For Each attrib As XmlAttribute In XML_Nodo.Attributes
                If attrib.Value = valoreDaSostituire Then
                    toRemove.Add(attrib.Name)
                End If
            Next

            For Each del In toRemove
                XML_Nodo.RemoveAttribute(del)
            Next

        Next

    End Sub


    Private Function GetRicodificaSA_COD(ByVal Piva_Origine As String,
                                         ByVal lsa_cod_letto As String,
                                         ByVal piva_SuperUser_origine As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim ricodificaSA_COD As Integer = 0

        If lsa_cod_letto <> "0" Then
            ricodificaSA_COD = If(_efG2G Is Nothing,
                                  (From sa In _funzioniGLOBAL.Imprese
                                   Where sa.FROM_Piva = Piva_Origine AndAlso
                                         sa.FROM_SaCod = lsa_cod_letto AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.TO_SaCod).FirstOrDefault,
                                  (From sa In _efG2G.G2G_Recode_Imprese
                                   Where sa.FROM_Piva = Piva_Origine AndAlso
                                         sa.FROM_SaCod = lsa_cod_letto AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.TO_SaCod).FirstOrDefault)
        End If

        Return ricodificaSA_COD

    End Function

    Private Function GetRicodificaSA_CODReverse(ByVal Piva_Origine As String,
                                         ByVal lsa_cod_letto As String,
                                         ByVal piva_SuperUser_origine As String,
                                         ByVal piva_SuperUser_destinazione As String
                                         ) As Integer

        Dim ricodificaSA_COD As Integer = 0

        If lsa_cod_letto <> "0" Then
            ricodificaSA_COD = If(_efG2G Is Nothing,
                                  (From sa In _funzioniGLOBAL.Imprese
                                   Where sa.FROM_Piva = Piva_Origine AndAlso
                                         sa.TO_SaCod = lsa_cod_letto AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.FROM_SaCod).FirstOrDefault,
                                  (From sa In _efG2G.G2G_Recode_Imprese
                                   Where sa.FROM_Piva = Piva_Origine AndAlso
                                         sa.TO_SaCod = lsa_cod_letto AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.FROM_SaCod).FirstOrDefault)
        End If

        Return ricodificaSA_COD

    End Function

    Private Function GetOrigineSA_COD(ByVal Piva_Destinazione As String,
                                      ByVal lsa_cod_letto As String,
                                      ByVal piva_SuperUser_origine As String,
                                      ByVal piva_SuperUser_destinazione As String
                                      ) As Integer

        Dim ricodificaSA_COD As Integer = 0
        If lsa_cod_letto <> "0" Then
            ricodificaSA_COD = If(_efG2G Is Nothing,
                                  (From sa In _funzioniGLOBAL.Imprese
                                   Where sa.To_Piva = Piva_Destinazione AndAlso
                                         sa.TO_SaCod = lsa_cod_letto AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.FROM_SaCod).FirstOrDefault,
                                  (From sa In _efG2G.G2G_Recode_Imprese
                                   Where sa.To_Piva = Piva_Destinazione AndAlso
                                         sa.TO_SaCod = lsa_cod_letto AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.FROM_SaCod).FirstOrDefault)
        End If

        Return ricodificaSA_COD

    End Function

    Private Function GetOrigineSA_CODReverse(ByVal Piva_Destinazione As String,
                                      ByVal lsa_cod_letto As String,
                                      ByVal piva_SuperUser_origine As String,
                                      ByVal piva_SuperUser_destinazione As String
                                      ) As Integer

        Dim ricodificaSA_COD As Integer = 0
        If lsa_cod_letto <> "0" Then
            ricodificaSA_COD = If(_efG2G Is Nothing,
                                  (From sa In _funzioniGLOBAL.Imprese
                                   Where sa.To_Piva = Piva_Destinazione AndAlso
                                         sa.FROM_SaCod = lsa_cod_letto AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.TO_SaCod).FirstOrDefault,
                                  (From sa In _efG2G.G2G_Recode_Imprese
                                   Where sa.To_Piva = Piva_Destinazione AndAlso
                                         sa.FROM_SaCod = lsa_cod_letto AndAlso
                                         sa.To_PivaSuperUser = piva_SuperUser_origine AndAlso
                                         sa.From_PivaSuperUser = piva_SuperUser_destinazione
                                   Select sa.TO_SaCod).FirstOrDefault)
        End If

        Return ricodificaSA_COD

    End Function

#End Region


End Class
