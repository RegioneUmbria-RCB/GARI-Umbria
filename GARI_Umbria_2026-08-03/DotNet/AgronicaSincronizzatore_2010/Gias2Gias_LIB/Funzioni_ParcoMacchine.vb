Imports System.Xml
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports System.Text
Imports AgronicaCoreModello.Anagrafe
Imports AgronicaCoreModello
Imports AgronicaCoreG2GLocalDal
Imports AgronicaCoreUtility
Imports AgronicaCoreEntityFramework_POCO

Partial Public Class Funzioni


#Region "Parco Macchine"

    Public Function LeggiParcoMacchineXML(ByVal Piva_Origine As String,
                                          ByVal Piva_Destinazione As String,
                                          ByRef objOpzioni As clsOpzioni,
                                          ByVal Flag_Pubblico As Boolean,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per le macchine pubbliche forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GParcoMacchine_R
        Dim g2g = objLeggi.Nuovo_Parco_Macchine_G2G(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)

        If objLeggi.Leggi_Parco_Macchine_G2G(From_Piva, Flag_Pubblico, listaImprese, dataValidita, g2g, objParametri) Then
            Return XMLUtility.SerializzaOggetto(Of G2G_Parco_Macchine)(g2g, "")
        End If

        Return ""

    End Function

    Public Function LeggiParcoMacchineXMLReverse(ByVal Piva_Origine As String,
                                          ByVal Piva_Destinazione As String,
                                          ByRef objOpzioni As clsOpzioni,
                                          ByVal Flag_Pubblico As Boolean,
                                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                          ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine) As String

        Dim From_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_ORIGINE
        Dim To_PivaSuperUser = objOpzioni.SuperUser_CodFiscale_DESTINAZIONE
        Dim From_Piva = Piva_Origine
        Dim To_Piva = Piva_Destinazione

        ' per le macchine pubbliche forzo la piva destinazione
        If String.IsNullOrEmpty(Piva_Destinazione) Then
            To_Piva = To_PivaSuperUser
        End If

        Dim listaImprese = New List(Of String)
        Dim dataValidita = AGRODATAINIZIO

        If Configurazione IsNot Nothing Then
            listaImprese = Configurazione.listaImprese
            dataValidita = Configurazione.dataValidita
        End If

        Dim objLeggi As New G2GParcoMacchine_R
        Dim g2g = objLeggi.Nuovo_Parco_Macchine_G2GReverse(From_PivaSuperUser, To_PivaSuperUser, From_Piva, To_Piva)

        If objLeggi.Leggi_Parco_Macchine_G2GReverse(From_Piva, Flag_Pubblico, listaImprese, dataValidita, g2g, objParametri) Then
            Return XMLUtility.SerializzaOggetto(Of G2G_Parco_Macchine_Reverse)(g2g, "")
        End If

        Return ""

    End Function

    Public Sub Elabora_XML_Parco_Macchine_Salva(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine)

        Const NomeFunzione As String = "Elabora_XML_Parco_Macchine_Salva"
        Dim MessaggioErrore As String = ""

        Try

            ' leggo struttura dati serializzata su origine contenente il parco macchine azienda da inserire/modificare/cancellare 
            Dim sXmlParcoMacchine = LeggiParcoMacchineXML(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, objOpzioni.objParametri_Server_GIAS_ORIGINE, Configurazione)

            If Not String.IsNullOrEmpty(sXmlParcoMacchine) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParcoMacchine, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiParcoMacchine")
                Dim sXmlRisposta As String = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParcoMacchine As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiParcoMacchine")
                Dim rispostaParcoMacchine = xmlRispostaParcoMacchine.FirstNode.ToString
                'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParcoMacchine, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento parco macchine: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento parco macchine ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento parco macchine: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_XML_Parco_Macchine_SalvaReverse(
                            ByVal objOpzioni As clsOpzioni,
                            ByRef Log_Import As StringBuilder,
                            ByRef Log_Errori As StringBuilder,
                            ByRef Log_Riepilogo As StringBuilder,
                            ByVal Piva_Origine As String,
                            ByVal Piva_Destinazione As String,
                            ByVal Flag_Pubblico As Boolean,
                            ByVal Configurazione As G2G_Configurazione_FiltriReq_Macchine)

        Const NomeFunzione As String = "Elabora_XML_Parco_Macchine_SalvaReverse"
        Dim MessaggioErrore As String = ""
        Dim sXmlParcoMacchine As String = ""
        Dim sXmlRisposta As String = ""
        Try

            ' leggo struttura dati serializzata su origine contenente il parco macchine azienda da inserire/modificare/cancellare 
            sXmlParcoMacchine = LeggiParcoMacchineXMLReverse(Piva_Origine, Piva_Destinazione, objOpzioni, Flag_Pubblico, objOpzioni.objParametri_Server_GIAS_ORIGINE, Configurazione)

            If Not String.IsNullOrEmpty(sXmlParcoMacchine) Then

                ' scrivo dati serializzati richiamando web service destinazione
                Dim wsimportazione As New WS_Importa_GIAS_2014.ImportaWS
                wsimportazione.Url = objOpzioni.wsimportaGiasURl
                wsimportazione.Timeout = _timeout_ws_Importa
                Dim Str_Credenziali_WS As String = Get_Str_Credenziali_WS(objOpzioni)

                Dim mainDoc = Funzioni_MasterG2G.XmlMasterDataPiva(enum_TipoOperazioneDB.Scrittura, Piva_Origine, "", objOpzioni)
                mainDoc = XDocUtils.IniettaSottoAlberoDaStringaXml(mainDoc, sXmlParcoMacchine, "//utente/" & If(Flag_Pubblico, "DatiGlobali", "DatiImprese/Impresa"), "DatiParcoMacchine")
                sXmlRisposta = wsimportazione.Scrivi_XmlFull(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, mainDoc))

                'leggo la struttura restituita contenente i recode da inserire/modificare/cancellare
                Dim xmlRisposta As XDocument = XDocument.Parse(sXmlRisposta)
                Dim xmlRispostaParcoMacchine As XElement = xmlRisposta.Element("Risposta").Element("Risposta_DatiParcoMacchine")
                Dim rispostaParcoMacchine = xmlRispostaParcoMacchine.FirstNode.ToString
                'Dim rispostaParcoMacchine = AgroZip.DeCompressioneBase64(1, wsimportazione.Scrivi_ParcoMacchineXmlPrivato(Str_Credenziali_WS, AgroZip.CompressioneBase64(1, sXmlParcoMacchine)))
                Dim risposta As G2G_Recode = XMLUtility.DeserializzaOggetto(Of G2G_Recode)(rispostaParcoMacchine, "")

                ' se la scrittura su destinazione è andata a buon fine aggiorno i recode sull'origine
                If String.IsNullOrEmpty(risposta.MessaggioErrore) Then
                    MessaggioErrore = G2GUtility.Scrivi_G2G_Recode(risposta, objOpzioni.objParametri_Server_GIAS_ORIGINE)
                Else
                    MessaggioErrore = risposta.MessaggioErrore
                End If

                ' log trasferimento dati
                If String.IsNullOrEmpty(MessaggioErrore) Then
                    G2GUtility.Log(Log_Import, "Trasferimento parco macchine: " & risposta.LogRecode)
                Else
                    G2GUtility.Log(Log_Import, "Trasferimento parco macchine ERRORE: " & MessaggioErrore)
                End If

            Else

                G2GUtility.Log(Log_Import, "Trasferimento parco macchine: nessun dato da trasferire")

            End If

        Catch ex As Exception
            Dim msg As String
            msg = CStr(Date.Now) + " - " + NomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            If ex.InnerException IsNot Nothing Then
                msg &= vbCrLf & "InnerException:" & ex.InnerException.Message
            End If
            msg &= vbCrLf & "sXmlParcoMacchine:" & sXmlParcoMacchine
            msg &= vbCrLf & "sXmlRisposta:" & sXmlRisposta
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub

    Public Sub Elabora_ParcoMacchine_Salva(
        ByVal objOpzioniImportImpresa As clsImpresa,
        ByVal objOpzioni As clsOpzioni,
        ByRef Log_Import As StringBuilder,
        ByRef Log_Errori As StringBuilder,
        ByRef Log_Riepilogo As StringBuilder,
        ByVal Piva_Origine As String,
        ByVal Piva_Destinazione As String,
        ByVal DT_Macchine As DataTable,
        ByVal DT_MacchineCod As DataTable,
        ByVal DT_Costi As DataTable,
        ByVal Dt_MovDet As DataTable
    )

        Const nomeFunzione As String = "Elabora_ParcoMacchine_Salva"

        Try

            Dim objMacchine_W As New AgronicaCoreContabDAL.Parco_Macchine_W
            Dim objMacchineCod_W As New AgronicaCoreContabDAL.Parco_Macchine_Codici_W
            Dim objCosti_W As New AgronicaCoreContabDAL.Prodotti_Costi_W

            Dim strXmlAgenda As String
            Dim Agenda_R As New AgronicaCoreContabBIZ.Agenda_R
            Dim Agenda_W As New AgronicaCoreContabBIZ.Agenda_W


            Dim Dr_Costi(), Dr_Codici(), Dr_MovDet() As DataRow
            Dim Mac_Cod_ORIGINE As Integer
            Dim Mac_Cod_DESTINAZIONE As Integer
            ' Dim Str_Filtro_MacCod As String = ""

            Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

            Dim id_agenda As Integer
            Dim outputID_Agenda As Integer = 0


            For Each DrMacchine In DT_Macchine.Rows

                Mac_Cod_ORIGINE = CType(DrMacchine("Mac_Cod"), Integer)

                'Str_Filtro_MacCod += CStr(Mac_Cod_ORIGINE) + ","

                Mac_Cod_DESTINAZIONE = ObjSequenze.NuovoId_Tabella(
                                           "Parco_Macchine",
                                           objOpzioni.BaseCode_DESTINAZIONE,
                                           objOpzioni.TopCode_DESTINAZIONE,
                                           objOpzioni.objParametri_Server_GIAS_DESTINAZIONE
                                           )

                Dim taratura_ugello As String = Agro_SQL_Load(DrMacchine("Taratura_Ugello"))
                If String.IsNullOrEmpty(taratura_ugello) Then
                    taratura_ugello = "0"
                End If

                objMacchine_W.Scrivi(Piva_Destinazione,
                                      CType(DrMacchine("Sa_Cod"), Integer),
                                      Mac_Cod_DESTINAZIONE,
                                      CType(DrMacchine("Cod_contatto"), String),
                                      CType(DrMacchine("Class_Code"), String),
                                     CType(DrMacchine("Tipo"), Integer),
                                     CType(DrMacchine("Mac_Des"), String),
                                     CType(DrMacchine("Costo_Acquisto"), Double),
                                     CType(DrMacchine("Targa"), String),
                                     CType(DrMacchine("Telaio"), String),
                                     CType(DrMacchine("Ditta_Cod"), Integer),
                                     CType(DrMacchine("Modello"), String),
                                     CType(DrMacchine("Potenza"), String),
                                     CType(DrMacchine("Ammortamento"), Double),
                                     CType(DrMacchine("Data_Immatricolazione"), Date),
                                     CType(DrMacchine("Ultima_Manutenzione"), Date),
                                     CType(DrMacchine("Ultima_Revisione"), Date),
                                     CType(DrMacchine("Stato_Utilizzo"), String),
                                     CType(DrMacchine("Note"), String),
                                     CType(DrMacchine("N_Immatricolazione"), String),
                                     CType(DrMacchine("N_Immatricolazione_Rimorchio"), String),
                                     CType(DrMacchine("N_Autorizzazione_Trasporto"), String),
                                     CType(DrMacchine("Data_Rilascio_Autorizzazione"), Date),
                                     CType(DrMacchine("Peso"), Double),
                                     CType(DrMacchine("Portata_Max"), Double),
                                     CType(DrMacchine("ChkDefault"), Integer),
                                     CType(DrMacchine("Alimentazione_Cod"), Integer),
                                     CType(DrMacchine("Potenza_Udm_Cod"), Integer),
                                     CType(DrMacchine("Mac_Cod_Origine"), Integer),
                                     CType(DrMacchine("Piva_SuperUser_Origine"), String),
                                     CType(DrMacchine("CUAA_Proprietario"), String),
                                     CType(DrMacchine("Denominazione_Proprietario"), String),
                                     CType(DrMacchine("Tipo_Targa_Cod"), Integer),
                                     CType(DrMacchine("Tipo_Trazione_Cod"), Integer),
                                     CType(DrMacchine("N_Omologazione"), String),
                                     CType(DrMacchine("Ditta_Cod_Motore"), Integer),
                                     CType(DrMacchine("Tipo_Motore"), String),
                                     CType(DrMacchine("Matricola_Motore"), String),
                                     CType(DrMacchine("Data_Reimmatricolazione"), Date),
                                     CType(DrMacchine("Data_Carico"), Date),
                                     CType(DrMacchine("Data_Scarico"), Date),
                                     CType(DrMacchine("TitoloPossesso"), Integer),
                                     CType(DrMacchine("Flag_Attrezzatura_Macchina"), String),
                                     CType(DrMacchine("Validita_Inizio"), Date),
                                      CType(DrMacchine("Validita_Fine"), Date),
                                      objOpzioni.objParametri_Server_GIAS_DESTINAZIONE,
                                      CType(taratura_ugello, Double)
                            )


                FunzioniGLOBAL.ParcoMacchine.Add(
                        New G2G_Recode_Parco_Macchine With {
                        .From_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_ORIGINE.PivaSuperUser,
                        .To_PivaSuperUser = objOpzioni.objParametri_Server_GIAS_DESTINAZIONE.PivaSuperUser,
                        .From_Mac_Cod = Mac_Cod_ORIGINE,
                        .To_Mac_Cod = Mac_Cod_DESTINAZIONE
                    })

                Dr_Costi = DT_Costi.Select(" Mat_Cod = " + Agro_SQL_SaveNum(Mac_Cod_ORIGINE))

                If Not IsNothing(Dr_Costi) AndAlso Dr_Costi.Length > 0 Then

                    For j = 0 To Dr_Costi.Length - 1

                        objCosti_W.Scrivi(Piva_Destinazione,
                                           Dr_Costi(j).Item("Riferimento"),
                                                Dr_Costi(j).Item("Elem_cod"),
                                                Dr_Costi(j).Item("Pro_Cod"),
                                                Mac_Cod_DESTINAZIONE,
                                                Dr_Costi(j).Item("Udm_Cod"),
                                                Dr_Costi(j).Item("Mezzo"),
                                               Dr_Costi(j).Item("Prezzo_Unitario"),
                                               Dr_Costi(j).Item("Veg_Cod"),
                                               Dr_Costi(j).Item("Cul_Cod"),
                                               Dr_Costi(j).Item("Validita_Inizio"),
                                               Dr_Costi(j).Item("Validita_Fine"),
                                               objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)
                    Next
                End If

                Dr_Codici = DT_MacchineCod.Select(" Mac_Cod =" + Agro_SQL_SaveNum(Mac_Cod_ORIGINE))
                If Not IsNothing(Dr_Codici) AndAlso Dr_Codici.Length > 0 Then
                    For j = 0 To Dr_Codici.Length - 1

                        objMacchineCod_W.Scrivi(Piva_Destinazione,
                                                Dr_Codici(j).Item("Sa_Cod"),
                                                Mac_Cod_DESTINAZIONE,
                                                Dr_Codici(j).Item("Id_Cod"),
                                                Dr_Codici(j).Item("Val_Cod"),
                                                Dr_Codici(j).Item("Validita_Inizio"),
                                                Dr_Codici(j).Item("Validita_Fine"),
                                                objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)


                    Next
                End If

                Dr_MovDet = Dt_MovDet.Select(" Mat_Cod =" + Agro_SQL_SaveNum(Mac_Cod_ORIGINE))
                If Not IsNothing(Dr_MovDet) AndAlso Dr_MovDet.Length > 0 Then
                    For j = 0 To Dr_MovDet.Length - 1

                        id_agenda = Dr_MovDet(j).Item("Id_Agenda")

                        strXmlAgenda = Agenda_R.Agenda_Leggi(Piva_Origine,
                                                               0,
                                                               id_agenda,
                                                               0,
                                                               False,
                                                               objOpzioni.objParametri_Server_GIAS_ORIGINE,
                                                               LeggiRiferimentiInversi:=False)

                        If strXmlAgenda <> "" Then

                            Dim idleProcedi As Boolean
                            strXmlAgenda = Elabora_XML_Agenda_SistemaXml(
                                objOpzioniImportImpresa,
                                objOpzioni,
                                Log_Import,
                                Log_Errori,
                                Log_Riepilogo,
                                strXmlAgenda,
                                Piva_Origine,
                                Piva_Destinazione,
                                0, 0,
                                enum_TipoOperazioneDB.Scrittura,
                                idleProcedi
                                )

                            Agenda_W.Agenda_Scrivi(strXmlAgenda,
                                                   outputID_Agenda,
                                                   0,
                                                   5,
                                                   0,
                                                    "",
                                                   objOpzioni.objParametri_Server_GIAS_DESTINAZIONE)

                        End If

                    Next

                End If

            Next

            ' Str_Filtro_MacCod = " Mac_Cod IN (" + Left(Str_Filtro_MacCod, Str_Filtro_MacCod.Length - 1) + ")"


        Catch ex As Exception

            Dim msg As String
            msg = CStr(Date.Now) + " - " + nomeFunzione + " Si è verificato il seguente errore: " + ex.Message + vbCrLf
            Log_Import.Append(msg)
            Log_Errori.Append(msg)
            Throw New Exception(msg)
        End Try

    End Sub


    Public Function recode_return_MacCod(ByVal oldMacCod As Integer, ByVal piva_SuperUser_Destinazione As String) As Integer

        Return If(_efG2G Is Nothing,
             (
                From pm In FunzioniGLOBAL.ParcoMacchine
                Where pm.From_Mac_Cod = oldMacCod _
                    And pm.To_PivaSuperUser = piva_SuperUser_Destinazione
                Select pm.To_Mac_Cod).FirstOrDefault,
            (
                From pm In _efG2G.G2G_Recode_Parco_Macchine
                Where pm.From_Mac_Cod = oldMacCod _
                    And pm.To_PivaSuperUser = piva_SuperUser_Destinazione
                Select pm.To_Mac_Cod).FirstOrDefault)

    End Function


#End Region





End Class
