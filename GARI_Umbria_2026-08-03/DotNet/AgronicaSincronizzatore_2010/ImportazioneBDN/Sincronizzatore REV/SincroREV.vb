Imports AgronicaCoreDataProvider
Imports ImportazioneBDN.VetInfoResponseModel
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreContabDAL
Imports AgronicaCoreMapper
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.attivita.dettagli
Imports AgronicaCoreEntityFramework
Imports System.Data.Entity
Imports Newtonsoft.Json
Imports AgronicaCoreUtility
Imports System.Text
Imports ImportazioneBDN.SincroREV
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.Zoo
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreEntityFramework_POCO
Imports System.Security.Cryptography
Imports ImportazioneBDN.VetInfoModel
Imports System.Reflection
Imports ImportazioneBDN.wsVetInfo
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreModelsSTD.exceptions

Public Class SincroREV

    Dim objP_Server As AgronicaCoreParametri
    Dim objP_Utenti As AgronicaCoreParametri

    Dim logDirectory As String
    Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider
    Dim customLOGParams As CustomLOGParams

    Dim GiasContext As Gias_DeveloperServer_Entities
    Dim dictFarmaci_AicToProCod As Dictionary(Of String, Integer)
    Public Shared dictFarmaci_ProCodToAic As Dictionary(Of Integer, String)

    Dim zooAnimali_R As AgronicaCoreAnagrafeDAL.Zoo_Animali

    Dim ws As wsVetInfo

    Public Enum enum_StatoPrescrizione
        UNDEFINED = 0
        Confermato = 1
    End Enum

    Public Enum enum_DiagnosiPrescrizione
        UNDEFINED = 0
        Antiparassitari = 1
        Trattamento_Zoo = 2
        Cutanee = 3
        Asciutta = 4
        Respiratorie = 5
        Mammarie = 6
    End Enum

    Public Enum enum_GeneriAnimale
        UNDEFINED = 0
        Bovino = 1
        Suino = 2
        Ovino = 3
        Caprino = 4
        Equino = 5
        Avicolo = 6
    End Enum

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, Username As String)
        Me.objP_Server = objParametriServer
        Me.objP_Utenti = objParametriUtenti
        ws = New wsVetInfo(objParametriServer, objParametriUtenti, Username)
        logDirectory = objParametriServer.LogDirectory & "\BDN\"

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(Me.objP_Server.StringaConnessione)
        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)

        dictFarmaci_AicToProCod = GiasContext.Farmaci.ToDictionary(Function(f) f.AIC, Function(f) f.Farm_Cod)
        dictFarmaci_ProCodToAic = GiasContext.Farmaci.ToDictionary(Function(f) f.Farm_Cod, Function(f) f.AIC)

        zooAnimali_R = New AgronicaCoreAnagrafeDAL.Zoo_Animali
    End Sub

    ''' <summary>
    ''' Sincronizza i trattamenti dei capi animali presenti su VetInfo
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="aziendaCodice">codice azienda BDN su tab Stalla</param>
    ''' <param name="allevIdFiscale">allevamento id fiscale su tab Stalla</param>
    ''' <param name="propIdFiscale">idfiscale del proprietario dell'allevamento</param>
    ''' <returns></returns>
    Public Function Sincronizza_REV_VetInfo(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Sta_Num As Integer,
                                            ByVal aziendaCodice As String,
                                            ByVal allevIdFiscale As String,
                                            ByVal propIdFiscale As String,
                                            ByVal Data_Inizio As Date,
                                            ByVal Data_Fine As Date,
                                            ByVal Chk_Sincronizza_Trattamenti As Boolean,
                                            ByVal Chk_Sincronizza_Protocolli As Boolean,
                                            ByVal Chk_Sincronizza_Indicazioni_Terapeutiche As Boolean,
                                            ByVal Chk_Sincronizza_Ricette_Veterinarie As Boolean) As Sincronizza_REV_VetInfo_Response
        'aziendaCodice = "076BG013"
        'propIdFiscale = "02060480163"
        Dim resp As New Sincronizza_REV_VetInfo_Response
        resp.trattamenti = New OggettiVetInfo
        resp.protocolli = New OggettiVetInfo
        resp.indicazioniTerapeutiche = New OggettiVetInfo
        resp.ricetteVeterinarie = New OggettiVetInfo

        If propIdFiscale = "" Then
            propIdFiscale = allevIdFiscale
        End If

        Dim idLogSincro As Integer = 0
        Try
            'ricava la stalla configurata sul codice presente in BDN
            'Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
            'Dim dtStalla As DataTable
            'dtStalla = objStalla_R.Leggi(Piva, 0, 0, 1,
            '                         "Stalla.BDN_Allev_IdFiscale = '" & allevIdFiscale &
            '                         "' AND Stalla.BDN_Codice_Azienda = '" & aziendaCodice & "' ",
            '                         "", objP_Server)

            'If dtStalla.Rows.Count = 0 Then
            '    dtStalla = objStalla_R.Leggi(Piva, 0, 0, 1,
            '                             " Stalla.BDN_Codice_Azienda = '" & aziendaCodice &
            '                             "' AND (Stalla.BDN_Allev_IdFiscale = '' OR Stalla.BDN_Allev_IdFiscale IS NULL) ",
            '                             "", objP_Server)
            'End If

            'If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
            '    Throw New Exception("Stalla non configurata su GIAS")
            'ElseIf dtStalla.Rows.Count > 1 Then
            '    Throw New GiasException("Allevamento " & aziendaCodice & " configurato più volte su GIAS")
            'End If

            'legge i capi nella stalla (DB) su cui poi eseguirà lo scarico dei farmaci
            Dim obj_ZooAnimali_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali
            'Dim dt_GiacenzaCapi_Stalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
            '                                                                          0, 0, Date.Now,
            '                                                                          objParametriServer)

            'If IsNothing(dt_GiacenzaCapi_Stalla) OrElse dt_GiacenzaCapi_Stalla.Rows.Count = 0 Then
            '    'Throw New Exception("Nessun capo presente in stalla")
            'End If
            'scrittura log per inizio sincronizzazione
            idLogSincro = logSincroStalla(Piva, Sa_Cod, Sta_Num, 0, "", resp)

            Dim objResp_Account = ws.Account()

            Me.logFileName = Date.Now.Year & Date.Now.Month.ToString("D2") & Date.Now.Day.ToString("D2") & " " & aziendaCodice & " " & allevIdFiscale & ".txt"

            If Chk_Sincronizza_Protocolli Then
                Dim Protocolli = Sincronizza_Prescrizioni(Piva, Sa_Cod, Sta_Num, aziendaCodice, propIdFiscale, idLogSincro, AGRODATAINIZIO, AGRODATAFINE, "PROTOCOLLO", True, False)
                resp.protocolli = Protocolli.result
            End If

            If Chk_Sincronizza_Indicazioni_Terapeutiche Then
                Dim Indicazioni_Terapeutiche = Sincronizza_Prescrizioni(Piva, Sa_Cod, Sta_Num, aziendaCodice, propIdFiscale, idLogSincro, Data_Inizio, Data_Fine, "PRESTRATSCORTA", True, False)
                resp.indicazioniTerapeutiche = Indicazioni_Terapeutiche.result
            End If

            If Chk_Sincronizza_Ricette_Veterinarie Then
                Dim Ricette_Veterinarie = Sincronizza_Prescrizioni(Piva, Sa_Cod, Sta_Num, aziendaCodice, propIdFiscale, idLogSincro, Data_Inizio, Data_Fine, "PRESVET", True, True)
                resp.ricetteVeterinarie = Ricette_Veterinarie.result
            End If


            If Chk_Sincronizza_Trattamenti Then
                Dim trattamenti = Sincronizza_Trattamenti(Piva, Sa_Cod, Sta_Num, aziendaCodice, propIdFiscale, idLogSincro, Data_Inizio, Data_Fine)
                resp.trattamenti = trattamenti
            End If

            'scrittura log per fine sincronizzazione trattamenti (riuscita)
            If idLogSincro <> 0 Then
                logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, "", resp)
            End If

        Catch ex As Exception
            'scrittura log per fine sincronizzazione (non riuscita)
            If idLogSincro <> 0 Then
                logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, ex.Message, resp)
            End If

            Throw New Exception("Sincronizza_REV_VetInfo() -> " & ex.Message)
        End Try

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = objP_Server.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = Date.Now.Year & Date.Now.Month.ToString("D2") & Date.Now.Day.ToString("D2") & " " & aziendaCodice & " " & allevIdFiscale & ".txt"
        }

        objLog.Scrivi_LOG(objP_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio sincronizzazione trattamenti",
                          CustomLOGParams:=customLOGParams)
        Return resp

    End Function

    Private Function Sincronizza_Trattamenti(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal Sta_Num As Integer,
                                             ByVal AziendaCodice As String,
                                             ByVal PropIdFiscale As String,
                                             ByVal IdLogSincro As Integer,
                                             ByVal Data_Inizio As Date,
                                             ByVal Data_Fine As Date) As OggettiVetInfo
        Dim lista_RegistroTratt As List(Of RegistroTrattamenti_Response) = ws.RegistroTrattamenti(AziendaCodice, PropIdFiscale, Data_Inizio, Data_Fine)
        Dim res = New OggettiVetInfo
        If lista_RegistroTratt Is Nothing Then
            res.nTrovati = 0
        Else
            res.nTrovati = lista_RegistroTratt.Count
        End If
        res.dettaglio = ""

        If (lista_RegistroTratt IsNot Nothing) Then
            objLog.Scrivi_LOG(objP_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Trovati n." & lista_RegistroTratt.Count & " trattamenti",
                          CustomLOGParams:=customLOGParams)
        Else
            objLog.Scrivi_LOG(objP_Server,
                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                      "Trovati n.0 trattamenti",
                      CustomLOGParams:=customLOGParams)
        End If

        If Not IsNothing(lista_RegistroTratt) AndAlso lista_RegistroTratt.Count > 0 Then
            'Aggiorno visibilita utente per servizio notturno (riutilizzo refresh e access token)
            AggiornaVisibilitaUtentixFabbricati(objP_Server.UtenteUsername, Piva, Sa_Cod, Sta_Num, objP_Server)
            Try
                'resp.NumeroTrattamentiEliminati = EliminaTrattamentiDaGias(Piva, Sa_Cod, Sta_Num, Data_Inizio, Data_Fine, lista_RegistroTratt)
            Catch ex As Exception
                Throw New Exception("Sincronizza_REV_VetInfo EliminaTrattamentiDaGias () -> " & ex.Message)
            End Try


            For Each tratt In lista_RegistroTratt
                Dim trattNumero As String = tratt.presrigaNumero
                Dim presNumero As String = tratt.presNumero
                Dim presDtEmissione As Date = tratt.presDtEmissione
                Try
                    Dim errori = 0
                    Dim capiNonTrovati = 0
                    Dim capiTrovati = 0
                    If (Not IsNothing(presNumero) OrElse presNumero <> "") _
                    AndAlso tratt.presAziendaCodice = AziendaCodice AndAlso tratt.presProprietarioIdFiscale = PropIdFiscale Then

                        Dim lista_ElencoProdPres As List(Of ProdottoPerPrescrizione_Response) = ws.ElencoProdotti_Prescrizione(presNumero)

                        If Not IsNothing(lista_ElencoProdPres) AndAlso lista_ElencoProdPres.Count > 0 Then
                            For Each prodPres In lista_ElencoProdPres
                                'ws.Ricava_FarmaciInMagazzino(prodPres.presrigaNumero, aziendaCodice, propIdFiscale)
                                Try
                                    Dim dataInizio As DateTime = If(IsNothing(tratt.tratDtInizio), AGRODATAINIZIO, tratt.tratDtInizio)
                                    Dim dataFine As DateTime = If(IsNothing(tratt.tratDtFine), AGRODATAFINE, tratt.tratDtFine)

                                    Dim sincronizzato = Sincronizza_Somministrazioni(Piva, Sa_Cod, Sta_Num, prodPres.presrigaNumero, trattNumero,
                                                                                     tratt.tratDtInizio, IIf(tratt.tratDtFine Is Nothing, AGRODATAFINE, tratt.tratDtFine),
                                                                                     presDtEmissione, capiNonTrovati, capiTrovati, AziendaCodice, PropIdFiscale)

                                    If sincronizzato Then
                                        res.nSincronizzati += 1
                                    ElseIf capiNonTrovati = 0 Then
                                        res.nGiaPresenti += 1
                                    ElseIf capiNonTrovati > 0 Then
                                        res.nNonSincronizzati += 1
                                        res.dettaglio &= tratt.presNumero & ", " & vbCrLf
                                    End If

                                Catch ex As Exception
                                    errori += 1
                                    objLog.Scrivi_LOG(objP_Server,
                                                      System.Reflection.MethodBase.GetCurrentMethod().Name,
                                                      "Errore importazione somministrazione " & tratt.presrigaNumero & " " & tratt.presNumero & ": " & ex.Message,
                                                      CustomLOGParams:=customLOGParams)
                                End Try

                            Next
                        End If

                    End If

                    If errori = 0 Then
                        Dim messaggio = "Trattamento " & tratt.presrigaNumero & " " & tratt.presNumero & " importato correttamente "
                        If capiNonTrovati > 0 Then
                            messaggio &= "con " & capiNonTrovati & " capi non trovati"
                        End If
                        objLog.Scrivi_LOG(objP_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              messaggio,
                              CustomLOGParams:=customLOGParams)
                    Else
                        res.nNonSincronizzati += 1
                        res.dettaglio &= tratt.presNumero & ", " & vbCrLf
                        objLog.Scrivi_LOG(objP_Server,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Trattamento " & tratt.presrigaNumero & " " & tratt.presNumero & " importato con errori ",
                              CustomLOGParams:=customLOGParams)
                    End If
                Catch ex As Exception
                    res.nNonSincronizzati += 1
                    res.dettaglio &= tratt.presNumero & ", " & vbCrLf
                End Try
            Next

        End If
        Return res

    End Function

    Private Function EliminaTrattamentiDaGias(Piva As String,
                                              Sa_Cod As Integer,
                                              Sta_Num As Integer,
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                              lista_RegistroTratt As List(Of RegistroTrattamenti_Response)) As Integer
        Dim numeroTrattamentiEliminati = 0
        Dim objZoo As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        Dim dtTrattamentiGIAS = objZoo.Leggi_Testata_Trattamenti(Piva, Sa_Cod, Sta_Num, Data_Inizio, Data_Fine, False, objP_Server)
        If dtTrattamentiGIAS.Rows.Count > 0 Then
            Dim listaTrattamentiVetInfo As List(Of String) = (From t In lista_RegistroTratt Select t.tratNumero).ToList
            Dim listaTrattamentiGIAS As List(Of String) = (From t As DataRow In dtTrattamentiGIAS.Rows Select CStr(t("Num_Trattamento"))).ToList

            Dim listaTrattamentiDaEliminare As List(Of String) = listaTrattamentiGIAS.Except(listaTrattamentiVetInfo).ToList
            For Each trattamento_n In listaTrattamentiDaEliminare
                Dim rowTrattamento = dtTrattamentiGIAS.Select(" Num_Trattamento ='" & trattamento_n & "' ")
                If rowTrattamento.Length > 0 Then
                    Dim id_agenda As Integer = rowTrattamento(0)("ID_Agenda")
                    Dim objAgenda As New AgronicaCoreContabBIZ.Agenda_W
                    objAgenda.Elimina_InteraOperazione(Piva, id_agenda, objP_Server)
                    numeroTrattamentiEliminati = numeroTrattamentiEliminati + 1
                End If
            Next
        End If
        Return numeroTrattamentiEliminati

    End Function



    ''' <summary>
    ''' Ricava l'elenco delle somministrazioni con scarichi e capi per ognuna di esse
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sta_Num"></param>
    ''' <param name="rigaNumero"></param>
    ''' <param name="trattNumero"></param>
    ''' <param name="Data_Inizio_Trattamento"></param>
    ''' <param name="Data_Fine_Trattamento"></param>
    ''' <param name="Data_Prescrizione"></param>
    ''' <param name="capiNonTrovati"></param>
    ''' <param name="capiTrovati"></param>
    ''' <returns></returns>
    Private Function Sincronizza_Somministrazioni(ByVal Piva As String,
                                                  ByVal Sa_Cod As Integer,
                                                  ByVal Sta_Num As Integer,
                                                  ByVal rigaNumero As String,
                                                  ByVal trattNumero As String,
                                                  ByVal Data_Inizio_Trattamento As Date,
                                                  ByVal Data_Fine_Trattamento As Date,
                                                  ByVal Data_Prescrizione As Date,
                                                  ByRef capiNonTrovati As Integer,
                                                  ByRef capiTrovati As Integer,
                                                  ByVal AziendaCodice As String,
                                                  ByVal PropIdFiscale As String) As Boolean
        Dim sincronizzato As Boolean = False

        Dim lista_RegistroSomm As List(Of RegistroSomministrazioni_Response) = ws.RegistroSomministrazioni(rigaNumero)

        'lettura somministrazioni su DB effettuate per escludere dalla sincronizzazione
        'le somministrazioni già sincronizzate su GIAS
        Dim objR_MovimentiDettagli As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim dtGiacenzeSomm As DataTable = objR_MovimentiDettagli.Leggi(Piva, 0, 0, 0, 0,
                                                                       CostantiPersonalizzate.FARMACI, 0, 0, CAU_TRATTAMENTO_ZOO,
                                                                       0, 0, 0, NONCONTABILE, 0, 0,
                                                                       enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                       "", "", objP_Server)

        Dim listaGiacenzeSomm As List(Of String) = (From somm In dtGiacenzeSomm
                                                    Select somm.Field(Of String)("Extra_Str")).ToList()
        Dim listaGiacenzeID_Agenda As List(Of Integer) = (From somm In dtGiacenzeSomm
                                                          Select somm.Field(Of Integer)("ID_Agenda")).ToList()

        Dim listaMatricoleCapi_Somm As New List(Of String)
        If Not IsNothing(lista_RegistroSomm) AndAlso lista_RegistroSomm.Count > 0 Then

            'controllo vecchie operazioni da cancellare non più presenti nell'elenco delle somministrazioni su VetInfo
            Dim listaSommNumero_Registro As List(Of String) = lista_RegistroSomm.Select(Function(somm) somm.somNumero).ToList()
            Dim listaOperazioni_Delete As New List(Of String)

            'For Each sommGiacenza In listaGiacenzeSomm
            '	If Not listaSommNumero_Registro.Contains(sommGiacenza) Then
            '		Dim operazione = dtGiacenzeSomm.Select("Extra_Str = " & sommGiacenza).FirstOrDefault
            '		Dim chiaveCanc As String = operazione("Data_Creazione") & "_" & operazione("ID_Agenda") & "_" & LAVCOD_CUREMEDICAMENTI_ANIMALI & "_0_0_0_0_" & Piva

            '		listaOperazioni_Delete.Add(chiaveCanc)
            '	End If
            'Next

            'elimino le vecchie operazioni non più presenti
            If Not IsNothing(listaOperazioni_Delete) And listaOperazioni_Delete.Count > 0 Then
                Dim objOperazioniCancella As New AgronicaCoreModello.Utility_Operazioni
                Dim listaOpDelete_ToString As String = "|" & String.Join(",", listaOperazioni_Delete)
                Dim resp = AgronicaCoreModello.Utility_Operazioni.elimina_operazione_multipla(listaOpDelete_ToString, True,
                                                                                              objP_Server, objP_Utenti)

            End If

            For Each somministrazione In lista_RegistroSomm
                Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                objAttivita.inizio = CDate(somministrazione.somDtSomministrazione)
                objAttivita.fine = AGRODATAFINE
                objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_CUREMEDICAMENTI_ANIMALI, "")
                objAttivita.centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva))
                objAttivita.fabbricatoCod = Sta_Num

                Dim obj_ZooAnimali_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                Dim dtGiacenzeStalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
                                                                                    0, 0, objAttivita.inizio,
                                                                                    objP_Server)

                Dim somNumero As String = somministrazione.somNumero

                Dim lista_ElencoScarichiSomm As List(Of ScarichiPerSomministrazione_Response) = ws.ElencoScarichi_Somministrazione(somNumero)
                Dim uguali As Boolean = False
                If (Not IsNothing(lista_ElencoScarichiSomm) OrElse lista_ElencoScarichiSomm.Count > 0) _
                    AndAlso listaGiacenzeSomm.Contains(somNumero) Then
                    Dim index = listaGiacenzeSomm.IndexOf(somNumero)
                    Dim Id_Agendapresente = listaGiacenzeID_Agenda(index)
                    'uguali = ConfrontoOperazioneDBOperazioneWS(somministrazione, lista_ElencoScarichiSomm,
                    '                                           Id_Agendapresente, Piva, Sa_Cod, Sta_Num)
                    uguali = True
                    If Not uguali Then
                        Dim aaa = 0
                        'QUI devo eliminare la vecchia operazione
                    End If
                End If

                'esegue lo scarico della somministrazione solo se non è già presente su DB
                If Not uguali Then

                    'If lista_ElencoScarichiSomm.Count > 1 Then
                    '    Throw New Exception("più scarichi di somministrazioni!!!")
                    'End If
                    Dim obj_SommScarico_Capo As AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni
                    'salva gli scarichi delle somministrazioni
                    objAttivita.risorse = New List(Of AgronicaCoreModelsSTD.attivita.risorse.Risorsa)
                    For Each scaricoSomm In lista_ElencoScarichiSomm
                        obj_SommScarico_Capo = New AgronicaCoreModelsSTD.attivita.dettagli.DettaglioRegistroSomministrazioni
                        obj_SommScarico_Capo.codice = somNumero
                        obj_SommScarico_Capo.numTrattamento = trattNumero
                        obj_SommScarico_Capo.codiceAIC = scaricoSomm.regscoProdottoAic
                        Select Case scaricoSomm.regscoUnitaMisuraCodice
                            Case "ml"
                                obj_SommScarico_Capo.unitaDiMisura = New metaschema.UnitaDiMisura(enum_UnitaMisura.Millilitri)
                            Case "g"
                                obj_SommScarico_Capo.unitaDiMisura = New metaschema.UnitaDiMisura(enum_UnitaMisura.Grammi)
                            Case "PEZZO"
                                obj_SommScarico_Capo.unitaDiMisura = New metaschema.UnitaDiMisura(enum_UnitaMisura.Numero)
                            Case Else
                                obj_SommScarico_Capo.unitaDiMisura = New metaschema.UnitaDiMisura(0)
                        End Select

                        obj_SommScarico_Capo.quantitaTotaleReale = CDec(scaricoSomm.regscoQuantitativo)
                        obj_SommScarico_Capo.validita = New anagrafiche.IntervalloTemporale(Data_Inizio_Trattamento, Data_Fine_Trattamento)
                        obj_SommScarico_Capo.dataPrescrizione = Data_Prescrizione

                        objAttivita.risorse.Add(obj_SommScarico_Capo)

                        'TODO Al momento aggiunge risorsa per effettuare uno scarico da magazzino del farmaco utilizzato (oltre allo scarico sull'animale)
                        If True Then
                            Dim obj_SommScarico_Magazzino As New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto

                            obj_SommScarico_Magazzino.prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto()
                            obj_SommScarico_Magazzino.prodotto.codice_alfanumerico = somNumero
                            obj_SommScarico_Magazzino.prodotto.elemCod = CostantiPersonalizzate.FARMACI

                            obj_SommScarico_Magazzino.prodotto.tipo = New AgronicaCoreModelsSTD.attivita.risorse.TipoRisorsa(CostantiPersonalizzate.FARMACI)
                            obj_SommScarico_Magazzino.MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino)
                            Dim obj_MagazzinoMovimentazione As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino
                            obj_MagazzinoMovimentazione.Prodotto = obj_SommScarico_Magazzino.prodotto
                            obj_MagazzinoMovimentazione.Magazzino = New anagrafiche.Fabbricato

                            Dim obj_Fabbricati_R As New AgronicaCoreAnagrafeDAL.Fabbricati_R
                            Dim objGiacienzeFarmaco_R As New AgronicaCoreStampeDAL.Magazzino
                            Dim DTMagazzinoFarmaci = obj_Fabbricati_R.LeggiMagazzinoFarmaci(Piva, 0, AziendaCodice, PropIdFiscale, objP_Server)

                            If DTMagazzinoFarmaci.Rows.Count > 0 Then

                                Dim sa_cod_magazzino As Integer = DTMagazzinoFarmaci.Rows(0)("sa_cod")
                                Dim fabbricato_cod_magazzino As Integer = DTMagazzinoFarmaci.Rows(0)("Fabbricato_Cod")

                                obj_MagazzinoMovimentazione.Magazzino.primaryKey = New anagrafiche.Fabbricato.PK()
                                obj_MagazzinoMovimentazione.Magazzino.primaryKey.codice = fabbricato_cod_magazzino
                                obj_MagazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK = New anagrafiche.CentroAziendale.PK(sa_cod_magazzino, Piva)

                                obj_MagazzinoMovimentazione.Magazzino.tipo = CostantiPersonalizzate.MAGAZZINO

                                Dim dtGiacenzeFarmaco = getGiacenzeFarmaco(objGiacienzeFarmaco_R,
                                                                           Piva, sa_cod_magazzino, fabbricato_cod_magazzino,
                                                                           somministrazione.somDtSomministrazione, scaricoSomm.regscoProdottoAic)

                                If Not IsNothing(dtGiacenzeFarmaco) AndAlso dtGiacenzeFarmaco.Rows.Count > 0 Then

                                    Dim total As Double = Convert.ToDouble(dtGiacenzeFarmaco.Compute("SUM(Giacenza)", String.Empty))

                                    If total >= obj_SommScarico_Capo.quantitaTotaleReale Then

                                        Dim totaleScaricato = 0
                                        Dim totaleDaScaricare = obj_SommScarico_Capo.quantitaTotaleReale
                                        For Each rowGiacenza In dtGiacenzeFarmaco.Rows
                                            Dim obj_SommScarico_Magazzino_add = JsonConvert.DeserializeObject(Of AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto)(JsonConvert.SerializeObject(obj_SommScarico_Magazzino))
                                            Dim Giacenza As Double = rowGiacenza("Giacenza")
                                            Dim lotto As String = rowGiacenza("Lotto")
                                            Dim qtaDaScaricare As Double
                                            If obj_SommScarico_Capo.quantitaTotaleReale < Giacenza Then
                                                qtaDaScaricare = obj_SommScarico_Capo.quantitaTotaleReale - totaleScaricato
                                                totaleScaricato += obj_SommScarico_Capo.quantitaTotaleReale - totaleScaricato
                                            ElseIf totaleDaScaricare < Giacenza Then
                                                totaleScaricato += totaleDaScaricare
                                                qtaDaScaricare = totaleDaScaricare
                                            Else
                                                totaleScaricato += Giacenza
                                                qtaDaScaricare = Giacenza
                                            End If
                                            Dim magazzinoMovimentazione = New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino()

                                            magazzinoMovimentazione.Qta = qtaDaScaricare
                                            magazzinoMovimentazione.Lotto = lotto
                                            magazzinoMovimentazione.udm = New metaschema.UnitaDiMisura(dtGiacenzeFarmaco(dtGiacenzeFarmaco.Rows.Count - 1)("Udm_Cod"))
                                            magazzinoMovimentazione.Prodotto = obj_SommScarico_Magazzino.prodotto
                                            magazzinoMovimentazione.Magazzino = New anagrafiche.Fabbricato

                                            magazzinoMovimentazione.Magazzino.primaryKey = New anagrafiche.Fabbricato.PK()
                                            magazzinoMovimentazione.Magazzino.primaryKey.codice = fabbricato_cod_magazzino
                                            magazzinoMovimentazione.Magazzino.primaryKey.centroAziendalePK = New anagrafiche.CentroAziendale.PK(sa_cod_magazzino, Piva)

                                            magazzinoMovimentazione.Magazzino.tipo = CostantiPersonalizzate.MAGAZZINO

                                            totaleDaScaricare = totaleDaScaricare - qtaDaScaricare

                                            obj_SommScarico_Magazzino_add.MagazziniMovimentazioni.Add(magazzinoMovimentazione)
                                            objAttivita.risorse.Add(obj_SommScarico_Magazzino_add)

                                            If totaleDaScaricare = 0 Then
                                                Exit For
                                            End If

                                        Next

                                    End If

                                End If

                            End If
                        End If
                    Next

                    Dim lista_ElencoCapiSomm As List(Of CapiPerSomministrazione_Response) = ws.ElencoAnimali_Somministrazione(somNumero)

                    Dim tempoSospensioneCarne = 0
                    Dim tempoSospensioneLatte = 0

                    If obj_SommScarico_Capo IsNot Nothing AndAlso lista_ElencoCapiSomm.Count > 0 Then
                        Dim listaSospensioni As New List(Of TempiSospensione)
                        Dim _tempoSospensioneCarne As TempiSospensione
                        Dim _tempoSospensioneLatte As TempiSospensione
                        For Each elementoCapo In lista_ElencoCapiSomm
                            If elementoCapo.prescapoTempiSospensione IsNot Nothing Then
                                For Each sospensione In elementoCapo.prescapoTempiSospensione
                                    If sospensione.tempososTipoAlimentoCodice = "CARNE" Then
                                        If tempoSospensioneCarne = 0 Then
                                            tempoSospensioneCarne = sospensione.tempososValore
                                            Dim giorniSospensione = 0
                                            If sospensione.tempososUnitaMisuraCodice = "G" Then
                                                giorniSospensione = sospensione.tempososValore
                                            ElseIf sospensione.tempososUnitaMisuraCodice = "H" Then
                                                giorniSospensione = CInt(sospensione.tempososValore / 24)
                                            ElseIf sospensione.tempososUnitaMisuraCodice = "F" Then
                                                giorniSospensione = sospensione.tempososValore
                                            Else
                                                Dim gino2 = 0
                                            End If
                                            _tempoSospensioneCarne = New TempiSospensione() With {
                                                .Alimento = New baseClass.BaseCodeDescr(1, "Carne"),
                                                .tempoSospensione = giorniSospensione
                                            }
                                        End If

                                        If tempoSospensioneCarne <> sospensione.tempososValore Then
                                            Throw New Exception("Tempi di sospensione diversi CARNE!")
                                        End If

                                    End If
                                    If sospensione.tempososTipoAlimentoCodice = "LATTE" Then
                                        If tempoSospensioneLatte = 0 Then
                                            tempoSospensioneLatte = sospensione.tempososValore
                                            Dim giorniSospensione = 0
                                            If sospensione.tempososUnitaMisuraCodice = "G" Then
                                                giorniSospensione = sospensione.tempososValore
                                            ElseIf sospensione.tempososUnitaMisuraCodice = "H" Then
                                                giorniSospensione = CInt(sospensione.tempososValore / 24)
                                            ElseIf sospensione.tempososUnitaMisuraCodice = "F" Then
                                                giorniSospensione = sospensione.tempososValore
                                            Else
                                                Dim gino2 = 0
                                            End If
                                            _tempoSospensioneLatte = New TempiSospensione() With {
                                                .Alimento = New baseClass.BaseCodeDescr(2, "Latte"),
                                                .tempoSospensione = giorniSospensione
                                            }
                                        End If

                                        If tempoSospensioneLatte <> sospensione.tempososValore Then
                                            Throw New Exception("Tempi di sospensione diversi LATTE!")
                                        End If

                                    End If
                                Next
                            End If

                        Next

                        If _tempoSospensioneCarne IsNot Nothing Then
                            listaSospensioni.Add(_tempoSospensioneCarne)
                        End If

                        If _tempoSospensioneLatte IsNot Nothing Then
                            listaSospensioni.Add(_tempoSospensioneLatte)
                        End If

                        obj_SommScarico_Capo.sospensione = listaSospensioni.ToArray
                    End If

                    'lista dei capi su cui si effettua lo scarico della somministrazione
                    Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                    Dim capiTrovatiInSomministrazioni = 0
                    Dim cod_animaleNonTrovato = -1
                    Dim dtGiacenzeStallaGiornoSuccessivo As DataTable
                    For Each capo In lista_ElencoCapiSomm
                        Dim Matricola_Capo As String = capo.prescapoIdentificativo

                        Dim capoPresente As Boolean = False
                        Dim Cod_Animale As Integer = 0
                        If Not IsNothing(dtGiacenzeStalla.Select("Matricola = '" & Matricola_Capo & "'")) AndAlso
                            dtGiacenzeStalla.Select("Matricola = '" & Matricola_Capo & "'").Length > 0 Then
                            capoPresente = True
                            Dim drCapo As DataRow = dtGiacenzeStalla.Select("Matricola = '" & Matricola_Capo & "'").First
                            Cod_Animale = drCapo("Cod_Animale")
                        End If
                        If capoPresente = False Then
                            Dim dataGiacenza = objAttivita.inizio.AddDays(1).AddSeconds(-1)
                            If dtGiacenzeStallaGiornoSuccessivo Is Nothing Then
                                dtGiacenzeStallaGiornoSuccessivo = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
                                                                                    0, 0, dataGiacenza,
                                                                                    objP_Server)
                            End If

                            If Not IsNothing(dtGiacenzeStallaGiornoSuccessivo.Select("Matricola = '" & Matricola_Capo & "'")) AndAlso
                            dtGiacenzeStallaGiornoSuccessivo.Select("Matricola = '" & Matricola_Capo & "'").Length > 0 Then
                                capoPresente = True
                                Dim drCapo As DataRow = dtGiacenzeStallaGiornoSuccessivo.Select("Matricola = '" & Matricola_Capo & "'").First
                                Cod_Animale = drCapo("Cod_Animale")
                            End If

                        End If

                        'cerca il capo nella stalla su DB
                        If capoPresente AndAlso Cod_Animale > 0 Then

                            Dim obj_CapoAnimaleCDC As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC
                            obj_CapoAnimaleCDC.codice = New AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto.CodeType(Sa_Cod)
                            obj_CapoAnimaleCDC.capoAnimale = New anagrafiche.CapoAnimale(Piva, Cod_Animale, Matricola_Capo)

                            lista_capoAnimaleCDC.Add(obj_CapoAnimaleCDC)
                            listaMatricoleCapi_Somm.Add(Matricola_Capo)
                            capiTrovati += 1
                            capiTrovatiInSomministrazioni += 1
                        Else
                            objLog.Scrivi_LOG(objP_Server,
                                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                                              "Trattamento " & rigaNumero & " " & trattNumero & ": Somministrazione " & objAttivita.inizio.ToShortDateString & " capo " & Matricola_Capo & " non trovato",
                                              CustomLOGParams:=customLOGParams)

                            'scarica il farmaco sul capo anche se non è presente su DB
                            capiNonTrovati += 1

                            Dim obj_CapoAnimaleCDC As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC
                            obj_CapoAnimaleCDC.codice = New AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto.CodeType(Sa_Cod)
                            obj_CapoAnimaleCDC.capoAnimale = New anagrafiche.CapoAnimale(Piva, cod_animaleNonTrovato, Matricola_Capo)
                            cod_animaleNonTrovato = cod_animaleNonTrovato - 1
                            lista_capoAnimaleCDC.Add(obj_CapoAnimaleCDC)
                            listaMatricoleCapi_Somm.Add(Matricola_Capo)
                        End If

                    Next

                    'se almeno un capo è presente su DB, crea l'attività legata alla somministrazione
                    If objAttivita.risorse.Count > 0 AndAlso lista_capoAnimaleCDC.Count > 0 AndAlso capiTrovatiInSomministrazioni > 0 Then
                        objAttivita.centriDiCosto = lista_capoAnimaleCDC

                        'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                        Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                        Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                              objP_Server,
                                                                                                              objP_Utenti)
                        'Return True
                        sincronizzato = True

                    Else
                        objLog.Scrivi_LOG(objP_Server,
                                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                                          "Trattamento " & rigaNumero & " " & trattNumero & ": Somministrazione non importata perchè non ci sono capi in allevamento",
                                          CustomLOGParams:=customLOGParams)
                    End If

                End If

            Next

        End If

        Return sincronizzato

    End Function

    Private Function getGiacenzeFarmaco(objGiacienzeFarmaco_R As AgronicaCoreStampeDAL.Magazzino,
                                        Piva As String, sa_cod_magazzino As Integer, fabbricato_cod_magazzino As Integer,
                                        somDtSomministrazione As String, regscoProdottoAic As String) As DataTable

        Dim dtGiacenzeFarmacoAllaData As DataTable = objGiacienzeFarmaco_R.SchedaGiacenzeMagazzino(CDate(somDtSomministrazione),
                                                                                                               Piva,
                                                                                                               sa_cod_magazzino, fabbricato_cod_magazzino,
                                                                                                               CostantiPersonalizzate.FARMACI,
                                                                                                               0, 0, 0, 0,
                                                                                                               0, 0, LOTTO_NONDEFINITO,
                                                                                                               False, "",
                                                                                                               "", "",
                                                                                                               "", "",
                                                                                                               "", "",
                                                                                                               "", "",
                                                                                                               "", "",
                                                                                                               "", "Lotto ASC ",
                                                                                                               objP_Server,
                                                                                                               objP_Utenti,
                                                                                                               "", "",
                                                                                                               False, False,
                                                                                                               "", True,
                                                                                                               True, CStr(regscoProdottoAic))

        Dim dtGiacenzeFarmacoAdOggi As DataTable = objGiacienzeFarmaco_R.SchedaGiacenzeMagazzino(DateTime.Now,
                                                                                       Piva,
                                                                                       sa_cod_magazzino, fabbricato_cod_magazzino,
                                                                                       CostantiPersonalizzate.FARMACI,
                                                                                       0, 0, 0, 0,
                                                                                       0, 0, LOTTO_NONDEFINITO,
                                                                                       False, "",
                                                                                       "", "",
                                                                                       "", "",
                                                                                       "", "",
                                                                                       "", "",
                                                                                       "", "",
                                                                                       "", "Lotto ASC ",
                                                                                       objP_Server,
                                                                                       objP_Utenti,
                                                                                       "", "",
                                                                                       False, False,
                                                                                       "", True,
                                                                                       True, CStr(regscoProdottoAic))

        Return getDistinctGiacenzeFarmaco(dtGiacenzeFarmacoAllaData, dtGiacenzeFarmacoAdOggi)

    End Function

    Private Function getDistinctGiacenzeFarmaco(dtGiacenzeFarmacoAllaData As DataTable, dtGiacenzeFarmacoAdOggi As DataTable) As DataTable
        ' Crea un DataTable combinato con il minimo delle giacenze
        Dim dtGiacenzeFarmaco As New DataTable()
        dtGiacenzeFarmaco.Columns.Add("Lotto", GetType(String))
        dtGiacenzeFarmaco.Columns.Add("Udm_Cod", GetType(Integer))
        dtGiacenzeFarmaco.Columns.Add("Giacenza", GetType(Double))

        ' Crea un dizionario per raggruppare per Lotto e Udm_Cod
        Dim giacenzeDict As New Dictionary(Of String, Double)
        Dim giacenzeDictAllaData As New Dictionary(Of String, Double)
        Dim giacenzeDictAdOggi As New Dictionary(Of String, Double)

        ' Processa le giacenze alla data
        If Not IsNothing(dtGiacenzeFarmacoAllaData) AndAlso dtGiacenzeFarmacoAllaData.Rows.Count > 0 Then
            For Each row As DataRow In dtGiacenzeFarmacoAllaData.Rows
                Dim chiave As String = row("Lotto").ToString() & "|" & row("Udm_Cod").ToString()
                Dim giacenza As Double = If(IsDBNull(row("Giacenza")), 0, CDbl(row("Giacenza")))

                If Not giacenzeDictAllaData.ContainsKey(chiave) Then
                    giacenzeDictAllaData.Add(chiave, giacenza)
                End If
            Next
        End If

        ' Processa le giacenze ad oggi
        If Not IsNothing(dtGiacenzeFarmacoAdOggi) AndAlso dtGiacenzeFarmacoAdOggi.Rows.Count > 0 Then
            For Each row As DataRow In dtGiacenzeFarmacoAdOggi.Rows
                Dim chiave As String = row("Lotto").ToString() & "|" & row("Udm_Cod").ToString()
                Dim giacenza As Double = If(IsDBNull(row("Giacenza")), 0, CDbl(row("Giacenza")))

                If Not giacenzeDictAdOggi.ContainsKey(chiave) Then
                    giacenzeDictAdOggi.Add(chiave, giacenza)
                End If
            Next
        End If

        ' Calcola il minimo solo per le chiavi presenti in ENTRAMBI i dizionari
        For Each chiave In giacenzeDictAllaData.Keys
            If giacenzeDictAdOggi.ContainsKey(chiave) Then
                ' La chiave esiste in entrambi i dizionari: prendi il minimo
                Dim minGiacenza As Double = Math.Min(giacenzeDictAllaData(chiave), giacenzeDictAdOggi(chiave))
                giacenzeDict.Add(chiave, minGiacenza)
            End If
        Next

        ' Popola il DataTable finale solo con le giacenze presenti in entrambe le date
        For Each kvp In giacenzeDict
            Dim parti As String() = kvp.Key.Split("|"c)
            Dim newRow As DataRow = dtGiacenzeFarmaco.NewRow()
            newRow("Lotto") = parti(0)
            newRow("Udm_Cod") = CInt(parti(1))
            newRow("Giacenza") = kvp.Value
            dtGiacenzeFarmaco.Rows.Add(newRow)
        Next

        ' Ordina per Lotto
        Dim dv As DataView = dtGiacenzeFarmaco.DefaultView
        dv.Sort = "Lotto ASC"
        dtGiacenzeFarmaco = dv.ToTable()

        Return dtGiacenzeFarmaco
    End Function

#Region "Prescrizioni"

    ''' <summary>
    ''' Crea le rispettive righe di Ricette_Zoo partendo da una prescrizione.
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sta_Num"></param>
    ''' <param name="pres"></param>
    ''' <returns></returns>
    Private Function CreateRicetteZooFromPrescrizione(ByVal Piva As String,
                                                      ByVal Sa_Cod As Integer,
                                                      ByVal Sta_Num As Integer,
                                                      ByVal protocollo As Boolean,
                                                      ByVal pres As Zoo.Prescrizione) As RispostaStandard
        Dim resp As New RispostaStandard
        Dim idRicetta As Integer = 0

        Try
            'RICETTA_ZOO
            Dim newPres As Boolean = False
            Dim ricettaZoo_W As New AgronicaCoreContabBIZ.Ricette_Zoo_W

            Dim ricettaZoo_old As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo
            Dim ricettaZoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo = GiasContext.Ricette_Zoo.
                    Where(Function(rz) rz.Piva = Piva AndAlso rz.Sa_Cod = Sa_Cod AndAlso rz.Sta_Num = Sta_Num AndAlso rz.Numero = pres.PresNumero).FirstOrDefault()

            If IsNothing(ricettaZoo) Then
                newPres = True
                ricettaZoo = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo
                ricettaZoo.IdRicetta = 0
                ricettaZoo.Piva = Piva
                ricettaZoo.Sa_Cod = Sa_Cod
                ricettaZoo.Sta_Num = Sta_Num
            Else
                If protocollo Then
                    resp.RispostaOK = True
                    resp.ParametroDue = False
                    resp.RispostaStringa = ricettaZoo.IdRicetta
                    Return resp
                End If
                ricettaZoo_old = ricettaZoo.DeepCloneObject()
            End If
            prescrizioneToRicettaZoo(pres, ricettaZoo)

            If newPres OrElse HasPrescrizioneChanged(ricettaZoo_old, ricettaZoo) Then
                idRicetta = ricettaZoo_W.Scrivi_Modifica(ricettaZoo, GiasContext, objP_Server)
                GiasContext.SaveChanges()
            Else
                idRicetta = ricettaZoo.IdRicetta
            End If
            ricettaZoo_old = Nothing

            'elimina le righe della prescrizione non piu' presenti 
            If Not newPres Then EliminaRighePrescrizione(Piva, Sa_Cod, idRicetta, pres.RighePrescrizione)

            For Each riga In pres.RighePrescrizione
                'RICETTA_ZOO_AGENDA
                Dim newPresRiga As Boolean = False
                Dim ricettaZooAg_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W
                Dim ricettaZooAg As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda
                Dim ricettaZooAg_old As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda

                convertUdmRigaPrescrizione(riga)

                If Not newPres Then
                    ricettaZooAg = GiasContext.Ricette_Zoo_Agenda.Where(Function(rza) rza.Piva = Piva AndAlso rza.Sa_Cod = Sa_Cod AndAlso
                                rza.IdRicetta = idRicetta AndAlso rza.Numero = riga.PresrigaNumero).FirstOrDefault()
                End If

                If IsNothing(ricettaZooAg) OrElse ricettaZooAg.IdAgenda = 0 Then
                    newPresRiga = True
                    ricettaZooAg = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda
                    ricettaZooAg.IdAgenda = 0
                    ricettaZooAg.IdRicetta = idRicetta
                    ricettaZooAg.Piva = Piva
                    ricettaZooAg.Sa_Cod = Sa_Cod
                Else
                    ricettaZooAg_old = ricettaZooAg.DeepCloneObject()
                End If
                rigaPresToRicettaZooAgenda(riga, ricettaZooAg, ricettaZoo.TipoCodice)

                Dim idAgenda As Integer = 0
                If newPresRiga OrElse HasRigaPresChanged(ricettaZooAg_old, ricettaZooAg) Then
                    idAgenda = ricettaZooAg_W.Scrivi_Modifica(ricettaZooAg, GiasContext, objP_Server)
                    GiasContext.SaveChanges()
                Else
                    idAgenda = ricettaZooAg.IdAgenda
                End If
                ricettaZooAg_old = Nothing

                'RICETTA_ZOO_MOVIMENTI
                Dim ricettaZooMov_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Movimenti_W
                Dim ricettaZooMov As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti

                If Not newPresRiga Then
                    ricettaZooMov = GiasContext.Ricette_Zoo_Movimenti.Where(Function(rzm) rzm.Piva = Piva AndAlso rzm.Sa_Cod = Sa_Cod AndAlso
                                rzm.IdRicetta = idRicetta AndAlso rzm.IdAgenda = idAgenda).FirstOrDefault()
                End If

                'in questo caso non c'e' bisogno della possibile modifica ad ogni sincronizzazione poiche' Ricette_Zoo_Movimenti
                'al momento non contiene campi della riga di prescrizione, ma solo campi della Movimenti gias
                Dim idMov As Integer = 0
                If IsNothing(ricettaZooMov) OrElse ricettaZooMov.IdMov = 0 Then
                    ricettaZooMov = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Movimenti
                    ricettaZooMov.IdMov = 0
                    ricettaZooMov.IdAgenda = idAgenda
                    ricettaZooMov.IdRicetta = idRicetta
                    ricettaZooMov.Piva = Piva
                    ricettaZooMov.Sa_Cod = Sa_Cod
                    ricettaZooMov.Data_Movimento = ricettaZooAg.DataInizioTrattamento
                    ricettaZooMov.Cau_Mov = CAU_TRATTAMENTO_ZOO
                    ricettaZooMov.Mov_Desc = "Cure e medicamenti"

                    idMov = ricettaZooMov_W.Scrivi_Modifica(ricettaZooMov, GiasContext, objP_Server)
                    GiasContext.SaveChanges()
                Else
                    idMov = ricettaZooMov.IdMov
                End If

                'RICETTA_ZOO_DETTAGLI
                Dim ricettaZooDett_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettagli_W
                Dim ricettaZooDett As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli
                Dim ricettaZooDett_old As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli

                If Not newPresRiga Then
                    ricettaZooDett = GiasContext.Ricette_Zoo_Dettagli.Where(Function(rzdtt) rzdtt.Piva = Piva AndAlso rzdtt.Sa_Cod = Sa_Cod AndAlso
                                rzdtt.IdRicetta = idRicetta AndAlso rzdtt.IdAgenda = idAgenda AndAlso rzdtt.IdMov = idMov).FirstOrDefault()
                End If

                If IsNothing(ricettaZooDett) OrElse ricettaZooDett.IdDettaglio = 0 Then
                    ricettaZooDett = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli
                    ricettaZooDett.IdDettaglio = 0
                    ricettaZooDett.IdMov = idMov
                    ricettaZooDett.IdAgenda = idAgenda
                    ricettaZooDett.IdRicetta = idRicetta
                    ricettaZooDett.Piva = Piva
                    ricettaZooDett.Sa_Cod = Sa_Cod
                Else
                    ricettaZooDett_old = ricettaZooDett.DeepCloneObject()
                End If
                rigaPresToRicettaZooDettagli(riga, ricettaZooDett)

                Dim idDettaglio As Integer = 0
                If newPresRiga OrElse HasRigaPresDettChanged(ricettaZooDett_old, ricettaZooDett) Then
                    idDettaglio = ricettaZooDett_W.Scrivi_Modifica(ricettaZooDett, GiasContext, objP_Server)
                    GiasContext.SaveChanges()
                Else
                    idDettaglio = ricettaZooDett.IdDettaglio
                End If
                ricettaZooDett_old = Nothing

                Dim regSco_Numero As String = ricettaZooDett.RegScoNumero

                'RICETTE_ZOO_DETTAGLIO_TECNICO
                Dim tempiSosProdotto = getTempiSospensioneFromCapiRigaPres(riga.CapiRigaPrescrizione)
                For Each sosp In tempiSosProdotto
                    Dim ricettaZoo_DettTecn_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Dettaglio_Tecnico_W
                    Dim ricettaZooDettTecn As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico
                    Dim ricettaZooDettTecn_old As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico

                    If Not newPresRiga Then
                        ricettaZooDettTecn = GiasContext.Ricette_Zoo_Dettaglio_Tecnico.
                                Where(Function(rzdtc) rzdtc.Piva = Piva AndAlso rzdtc.Sa_Cod = Sa_Cod AndAlso rzdtc.Id_Ricetta = idRicetta AndAlso
                                    rzdtc.Id_Agenda = idAgenda AndAlso rzdtc.Id_Mov = idMov AndAlso rzdtc.Id_Mov_Det = idDettaglio AndAlso rzdtc.Dett_Cod = sosp.Alimento.codice).FirstOrDefault()
                    End If

                    If IsNothing(ricettaZooDettTecn) OrElse ricettaZooDettTecn.Id_Reg_Dettaglio = 0 Then
                        ricettaZooDettTecn = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico
                        ricettaZooDettTecn.Id_Reg_Dettaglio = 0
                        ricettaZooDettTecn.Piva = Piva
                        ricettaZooDettTecn.Sa_Cod = Sa_Cod
                        ricettaZooDettTecn.Id_Ricetta = idRicetta
                        ricettaZooDettTecn.Id_Agenda = idAgenda
                        ricettaZooDettTecn.Id_Mov = idMov
                        ricettaZooDettTecn.Id_Mov_Det = idDettaglio
                    Else
                        ricettaZooDettTecn_old = ricettaZooDettTecn.DeepCloneObject()
                    End If
                    tempoSospensioneToRicettaZooDettaglioTecnico(sosp, ricettaZooDettTecn)

                    Dim idRegDettaglio As Integer = 0
                    If newPresRiga OrElse HasRigaPresTempoSospChanged(ricettaZooDettTecn_old, ricettaZooDettTecn) Then
                        idRegDettaglio = ricettaZoo_DettTecn_W.Scrivi_Modifica(ricettaZooDettTecn, GiasContext, objP_Server)
                        GiasContext.SaveChanges()
                    Else
                        idRegDettaglio = ricettaZooDettTecn.Id_Reg_Dettaglio
                    End If

                    ricettaZooDettTecn_old = Nothing
                Next

                'nel caso di vecchia riga di prescrizione modificata elimina i capi non piu' presenti
                If Not newPresRiga Then EliminaCapiPrescrizione(Piva, Sa_Cod, idRicetta, idAgenda, idDettaglio, riga.CapiRigaPrescrizione)

                For Each capo In riga.CapiRigaPrescrizione
                    'RICETTA_ZOO_DESTINAZIONI
                    Dim newPresRigaCapo As Boolean = False
                    Dim ricettaZooDest_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W
                    Dim ricettaZooDest As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni
                    Dim ricettaZooDest_old As New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni

                    'se non e' presente l'identificativo (matricola) del capo lo salta
                    'TODO l'identificativo potrebbe essere non presente se si tratta di un gruppo di animali
                    If IsNothing(capo.PrescapoIdentificativo) Then Continue For

                    'se e' un nuovo record e il capo e' presente in giacenza mette il Cod_Progetto del capo, 
                    'se il record esiste mette il codice gia' presente
                    Dim codAnimale As Integer = 0
                    If Not newPresRiga Then
                        ricettaZooDest = GiasContext.Ricette_Zoo_Destinazioni.Where(Function(rzdst) rzdst.Piva = Piva AndAlso rzdst.Sa_Cod = Sa_Cod AndAlso
                                rzdst.IdRicetta = idRicetta AndAlso rzdst.IdAgenda = idAgenda AndAlso rzdst.IdMov = idMov AndAlso rzdst.IdDettaglio = idDettaglio AndAlso
                                rzdst.Numero = capo.PrescapoNumero).FirstOrDefault()
                        If ricettaZooDest IsNot Nothing Then codAnimale = ricettaZooDest.CodAnimale
                    Else
                        newPresRigaCapo = True
                    End If

                    If codAnimale = 0 Then
                        'se il capo non e' presente in stalla lo salta
                        Dim capoInGiacenza = GiasContext.Zoo_Animali.Where(Function(cp) cp.PIVA = Piva AndAlso cp.Matricola = capo.PrescapoIdentificativo).FirstOrDefault
                        If IsNothing(capoInGiacenza) Then
                            Continue For
                        End If
                        codAnimale = capoInGiacenza.Cod_Progetto
                    End If

                    If IsNothing(ricettaZooDest) OrElse ricettaZooDest.IdDestinazione = 0 Then
                        ricettaZooDest = New AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni
                        ricettaZooDest.IdDestinazione = 0
                        ricettaZooDest.IdDettaglio = idDettaglio
                        ricettaZooDest.IdMov = idMov
                        ricettaZooDest.IdAgenda = idAgenda
                        ricettaZooDest.IdRicetta = idRicetta
                        ricettaZooDest.Piva = Piva
                        ricettaZooDest.Sa_Cod = Sa_Cod
                        ricettaZooDest.CodAnimale = codAnimale
                        ricettaZooDest.RegSco_Numero = regSco_Numero
                    Else
                        ricettaZooDest_old = ricettaZooDest.DeepCloneObject()
                    End If
                    capoRigaPresToRicettaZooDestinazioni(capo, ricettaZooDest)

                    Dim idDestinazione As Integer = 0
                    If newPresRigaCapo OrElse HasRigaPresCapoChanged(ricettaZooDest_old, ricettaZooDest) Then
                        idDestinazione = ricettaZooDest_W.Scrivi_Modifica(ricettaZooDest, GiasContext, objP_Server)
                        GiasContext.SaveChanges()
                    Else
                        idDestinazione = ricettaZooDest.IdDestinazione
                    End If
                    ricettaZooDest_old = Nothing

                Next
            Next

            resp.ParametroDue = newPres

        Catch ex As Exception
            resp.Errore = ex.Message
            resp.RispostaOK = False
        End Try

        resp.RispostaOK = True
        resp.RispostaStringa = idRicetta

        Return resp

    End Function

    Private Sub convertUdmRigaPrescrizione(ByRef rigaPrescrizione As RigaPrescrizione)
        'provo a convertire scat con il relativo valore
        If Not IsNothing(rigaPrescrizione.PresrigaQuantitativo) AndAlso
                    rigaPrescrizione.PresrigaQuantitativo > 0 AndAlso
                    Not IsNothing(rigaPrescrizione.PresrigaProdottoUnitaMisuraCodice) AndAlso
                    rigaPrescrizione.PresrigaProdottoUnitaMisuraCodice.ToLower() = "scat" Then
            Dim aic = rigaPrescrizione.PresrigaProdottoAic
            Dim objfarmaciXudm As New AgronicaCoreMetaSchemaDAL.FarmacixUnitaMisura
            Dim dtUdmScato = objfarmaciXudm.DefaultFromFarmacoAic(aic, objP_Server)
            Dim qtaOld = rigaPrescrizione.PresrigaQuantitativo
            If dtUdmScato IsNot Nothing AndAlso dtUdmScato.Rows.Count > 0 Then
                Dim udmScatola As String = dtUdmScato.Rows(0)("Udm_Cod").ToString()
                Dim qtaxUnita As Integer = dtUdmScato.Rows(0)("QtaxUnita").ToString()
                Select Case udmScatola
                    Case enum_UnitaMisura.Numero
                        rigaPrescrizione.PresrigaProdottoUnitaMisuraCodice = "pezzo"
                    Case enum_UnitaMisura.Grammi
                        rigaPrescrizione.PresrigaProdottoUnitaMisuraCodice = "g"
                    Case enum_UnitaMisura.Millilitri
                        rigaPrescrizione.PresrigaProdottoUnitaMisuraCodice = "ml"
                End Select
                Dim qtaNew = qtaOld * qtaxUnita
                rigaPrescrizione.PresrigaQuantitativo = qtaNew
            End If
        End If
    End Sub

    Private Function Sincronizza_Prescrizioni_Old(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Sta_Num As Integer,
                                              ByVal AziendaCodice As String,
                                              ByVal PropIdFiscale As String,
                                              ByVal IdLogSincro As Integer,
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                              ByRef resp As Sincronizza_REV_VetInfo_Response) As List(Of Zoo.Prescrizione)
        ' Inizio sincro prescrizioni
        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Inizio sincronizzazione prescrizioni.",
                          CustomLOGParams:=customLOGParams)

        Dim listaPrescrizioni As List(Of Zoo.Prescrizione) = LeggiPrescrizioniVetInfo(AziendaCodice, PropIdFiscale, Data_Inizio, Data_Fine)

        ' Elimina le vecchie prescrizioni 
        'resp.NumeroPrescrizioniEliminate = EliminaPrescrizioni(Piva, Sa_Cod, Sta_Num, Data_Inizio, Data_Fine, listaPrescrizioni)

        For Each pres In listaPrescrizioni
            Dim r = CreateRicetteZooFromPrescrizione(Piva, Sa_Cod, Sta_Num, False, pres)

            If r.RispostaOK Then
                objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Prescrizione " & pres.PresNumero & " sincronizzata.",
                                  CustomLOGParams:=customLOGParams)
            Else
                objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Errore sincronizzazione prescrizione " & pres.PresNumero & ": " & r.Errore,
                          CustomLOGParams:=customLOGParams)
            End If
        Next

        ' Fine sincro prescrizioni
        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Fine sincronizzazione prescrizioni.",
                          CustomLOGParams:=customLOGParams)

        Return listaPrescrizioni

    End Function

    Private Function Sincronizza_Prescrizioni(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Sta_Num As Integer,
                                              ByVal AziendaCodice As String,
                                              ByVal PropIdFiscale As String,
                                              ByVal IdLogSincro As Integer,
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                              ByVal TipoPrescrizione As String,
                                              ByVal protocolli As Boolean,
                                              ByVal ricetteVet As Boolean) As resultSincronizzazionePrescrizioniVetInfo
        Dim resp = New resultSincronizzazionePrescrizioniVetInfo
        resp.result = New OggettiVetInfo
        ' Inizio sincro prescrizioni
        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Inizio sincronizzazione prescrizioni " & TipoPrescrizione & ".",
                          CustomLOGParams:=customLOGParams)

        Dim filters As List(Of VetInfoModel.VetInfoFilter)
        If TipoPrescrizione <> "" Then
            filters = New List(Of VetInfoFilter)
            filters.Add(New VetInfoFilter With {
                .field = "presTipoCodice",
                .op = "EQUALS",
                .value1 = TipoPrescrizione
            })
        End If

        Dim listaPrescrizioni As List(Of Zoo.Prescrizione) = LeggiPrescrizioniVetInfo(AziendaCodice, PropIdFiscale, Data_Inizio, Data_Fine, filters, Nothing)
        resp.result.nTrovati = listaPrescrizioni.Count
        ' Elimina le vecchie prescrizioni 
        'resp.NumeroPrescrizioniEliminate = EliminaPrescrizioni(Piva, Sa_Cod, Sta_Num, Data_Inizio, Data_Fine, listaPrescrizioni)

        For Each pres In listaPrescrizioni
            'If prescrizioneGiaTrasformata(pres) Then
            '	Continue For
            'End If
            'If ricetteVet Then
            '	If ricettaGiaTrasformata(pres) Then
            '		Continue For
            '	End If
            'End If
            Dim r = CreateRicetteZooFromPrescrizione(Piva, Sa_Cod, Sta_Num, protocolli, pres)

            If r.RispostaOK Then
                If r.ParametroDue Then
                    resp.result.nSincronizzati += 1
                Else
                    resp.result.nGiaPresenti += 1
                End If
                objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                  "Prescrizione " & pres.PresNumero & " sincronizzata.",
                                  CustomLOGParams:=customLOGParams)
            Else
                resp.result.nNonSincronizzati += 1
                If String.IsNullOrEmpty(resp.result.dettaglio) Then
                    resp.result.dettaglio = pres.PresNumero
                Else
                    resp.result.dettaglio &= ", " & pres.PresNumero
                End If

                objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Errore sincronizzazione prescrizione " & pres.PresNumero & ": " & r.Errore,
                          CustomLOGParams:=customLOGParams)
            End If
        Next

        ' Fine sincro prescrizioni
        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Fine sincronizzazione prescrizioni " & TipoPrescrizione & ".",
                          CustomLOGParams:=customLOGParams)
        resp.listPrescrizioni = listaPrescrizioni
        Return resp

    End Function

    Private Function prescrizioneGiaTrasformata(pres As Zoo.Prescrizione) As Boolean
        If pres.RighePrescrizione IsNot Nothing AndAlso pres.RighePrescrizione.Count = 1 Then
            If pres.RighePrescrizione(0).PresrigaDtInizioTrattamento > AGRODATAINIZIO AndAlso pres.RighePrescrizione(0).PresrigaDtFineTrattamento > AGRODATAINIZIO Then
                Return True
            End If
        End If

        Return False
    End Function

    Private Function ricettaGiaTrasformata(pres As Zoo.Prescrizione) As Boolean
        If pres.RighePrescrizione IsNot Nothing AndAlso pres.RighePrescrizione.Count = 1 Then
            If pres.RighePrescrizione(0).PresrigaQuantitativo = 0 Then
                Return True
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Legge le prescrizioni da sincronizzare da VetInfo
    ''' </summary>
    ''' <param name="AziendaCodice"></param>
    ''' <param name="PropIdFiscale"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <returns></returns>
    Private Function LeggiPrescrizioniVetInfo(ByVal AziendaCodice As String,
                                              ByVal PropIdFiscale As String,
                                              ByVal Data_Inizio As Date,
                                              ByVal Data_Fine As Date,
                                              Optional filters As List(Of VetInfoModel.VetInfoFilter) = Nothing,
                                              Optional paginations As VetInfoModel.VetInfoPagination = Nothing) As List(Of Zoo.Prescrizione)
        Dim listaPrescrizioniVetInfo = ws.RegistroPrescrizioni(AziendaCodice, PropIdFiscale, Data_Inizio, Data_Fine, filters, paginations)

        If listaPrescrizioniVetInfo Is Nothing OrElse listaPrescrizioniVetInfo.Count = 0 Then
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              "Trovate n.0 prescrizioni.",
                              CustomLOGParams:=customLOGParams)
            Return New List(Of Prescrizione)
        End If

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              "Trovate n." & listaPrescrizioniVetInfo.Count & " prescrizioni.",
                              CustomLOGParams:=customLOGParams)

        ' Rimuove le prescrizioni di tipo Rifornimento_Scorta
        listaPrescrizioniVetInfo =
            listaPrescrizioniVetInfo.Where(Function(pres) tipoCodiceToEnum(pres.presTipoCodice) <> enum_TipoPrescrizione.Rifornimento_Scorta).ToList

        Dim listaPrescrizioni As New List(Of Zoo.Prescrizione)
        listaPrescrizioniVetInfo.ForEach(Sub(prescrizioneVetInfo)
                                             Dim prescrizione As Zoo.Prescrizione = ProcessPrescrizione(prescrizioneVetInfo)
                                             If prescrizione IsNot Nothing Then listaPrescrizioni.Add(prescrizione)
                                         End Sub)

        Return listaPrescrizioni

    End Function

    ''' <summary>
    ''' Legge una prescrizione da VetInfo dato il Pres_Numero
    ''' </summary>
    ''' <param name="AziendaCodice"></param>
    ''' <param name="PropIdFiscale"></param>
    ''' <param name="Pres_Numero"></param>
    ''' <returns></returns>
    Private Function Read_PrescrizioneVetInfo(ByVal AziendaCodice As String,
                                              ByVal PropIdFiscale As String,
                                              ByVal Pres_Numero As String) As Zoo.Prescrizione
        Dim prescrizioneVetInfo = ws.Read_Prescrizione(AziendaCodice, PropIdFiscale, Pres_Numero)

        If prescrizioneVetInfo Is Nothing Then
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              "Trovate n.0 prescrizioni.",
                              CustomLOGParams:=customLOGParams)
            Return New Prescrizione
        End If

        Dim prescrizione As Zoo.Prescrizione = ProcessPrescrizione(prescrizioneVetInfo)

        Return prescrizione

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="prescrizioneVetInfo"></param>
    ''' <returns></returns>
    Private Function ProcessPrescrizione(ByVal prescrizioneVetInfo As RegistroPrescrizioni_Response) As Zoo.Prescrizione
        Dim prescrizione As New Zoo.Prescrizione()
        prescrizione.RighePrescrizione = New List(Of RigaPrescrizione)

        If prescrizioneVetInfo.presNote Is Nothing Then prescrizioneVetInfo.presNote = ""
        If prescrizioneVetInfo.presStrutturaCodice Is Nothing Then prescrizioneVetInfo.presStrutturaCodice = ""
        If prescrizioneVetInfo.presDetentoreIdFiscale Is Nothing Then prescrizioneVetInfo.presDetentoreIdFiscale = ""
        If prescrizioneVetInfo.protCatClassyfarmCodice Is Nothing Then prescrizioneVetInfo.protCatClassyfarmCodice = ""
        If prescrizioneVetInfo.presStrutturaDenominazione Is Nothing Then prescrizioneVetInfo.presStrutturaDenominazione = ""

        Dim listaProdottiPrescrizioneVetInfo = ws.ElencoProdotti_Prescrizione(prescrizioneVetInfo.presNumero)
        If listaProdottiPrescrizioneVetInfo IsNot Nothing AndAlso listaProdottiPrescrizioneVetInfo.Count > 0 Then
            listaProdottiPrescrizioneVetInfo.ForEach(Sub(rigaPresVetInfo)
                                                         Dim rigaPres As Zoo.RigaPrescrizione =
                                                            ProcessRigaPrescrizione(rigaPresVetInfo, prescrizioneVetInfo.presNumero.EndsWith("T"))
                                                         If rigaPres IsNot Nothing Then prescrizione.RighePrescrizione.Add(rigaPres)
                                                     End Sub)
        End If

        Return prescrizioneVetInfo.FieldToPropertyCopier(prescrizione)

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="rigaPresVetInfo"></param>
    ''' <param name="isProtocollo"></param>
    ''' <returns></returns>
    Private Function ProcessRigaPrescrizione(ByVal rigaPresVetInfo As ProdottoPerPrescrizione_Response,
                                             ByVal isProtocollo As Boolean) As Zoo.RigaPrescrizione
        Dim rigaPres As New Zoo.RigaPrescrizione()
        rigaPres.CapiRigaPrescrizione = New List(Of CapoRigaPrescrizione)

        If rigaPresVetInfo.presrigaNote Is Nothing Then rigaPresVetInfo.presrigaNote = ""
        If rigaPresVetInfo.presrigaProdottoAic Is Nothing Then rigaPresVetInfo.presrigaProdottoAic = ""
        If rigaPresVetInfo.presrigaMangimeComposizione Is Nothing Then rigaPresVetInfo.presrigaMangimeComposizione = ""
        If rigaPresVetInfo.presrigaMangimeDenominazione Is Nothing Then rigaPresVetInfo.presrigaMangimeDenominazione = ""

        Dim listaCapiProdottoPrescrizioneVetInfo = ws.ElencoAnimali_ProdottoPres(rigaPresVetInfo.presrigaNumero)
        If listaCapiProdottoPrescrizioneVetInfo IsNot Nothing AndAlso listaCapiProdottoPrescrizioneVetInfo.Count > 0 Then
            listaCapiProdottoPrescrizioneVetInfo.ForEach(Sub(capoRigaPresVetInfo)
                                                             Dim capoRigaPres As New Zoo.CapoRigaPrescrizione
                                                             If capoRigaPresVetInfo.prescapoCodificaCodice Is Nothing Then capoRigaPresVetInfo.prescapoCodificaCodice = ""

                                                             capoRigaPresVetInfo.FieldToPropertyCopier(capoRigaPres)
                                                             capoRigaPres.PrescapoTempiSospensione = ProcessTempiSospensione(capoRigaPresVetInfo)
                                                             rigaPres.CapiRigaPrescrizione.Add(capoRigaPres)
                                                         End Sub)
        End If

        ' Se si tratta di un Protocollo, non legge la Scorta del Farmaco avendo solo la Famiglia AIC del Prodotto
        If Not isProtocollo Then
            Dim scortaProd_daPrescrizione = ws.Leggi_ScortaProdotto_perSomministrazione({rigaPresVetInfo.presrigaNumero}.ToList)
            If scortaProd_daPrescrizione IsNot Nothing AndAlso scortaProd_daPrescrizione.Count > 0 Then
                rigaPres.RegscoNumero = scortaProd_daPrescrizione.First.regscoNumero
            End If
        End If

        rigaPresVetInfo.FieldToPropertyCopier(rigaPres)

        If Not IsNothing(rigaPresVetInfo.presrigaDtInizioTrattamento) AndAlso rigaPresVetInfo.presrigaDtInizioTrattamento > AGRODATAINIZIO Then
            rigaPres.PresrigaDtInizioTrattamento = rigaPresVetInfo.presrigaDtInizioTrattamento
        End If

        If Not IsNothing(rigaPresVetInfo.presrigaDtFineTrattamento) AndAlso rigaPresVetInfo.presrigaDtFineTrattamento > AGRODATAINIZIO Then
            rigaPres.PresrigaDtFineTrattamento = rigaPresVetInfo.presrigaDtFineTrattamento
        End If

        If Not IsNothing(rigaPresVetInfo.presrigaDurataTrattamento) AndAlso rigaPresVetInfo.presrigaDurataTrattamento > 0 Then
            rigaPres.PresrigaDurataTrattamento = rigaPresVetInfo.presrigaDurataTrattamento
        End If

        Return rigaPres

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="capoRigaPresVetInfo"></param>
    ''' <returns></returns>
    Private Function ProcessTempiSospensione(ByVal capoRigaPresVetInfo As CapoPerProdotto_Response) As List(Of TempoSospensioneCapo)
        Return capoRigaPresVetInfo.prescapoTempiSospensione.Select(Function(sosp) New TempoSospensioneCapo With {
                                                                           .TempososTipoAlimentoCodice = sosp.tempososTipoAlimentoCodice,
                                                                           .TempososTipoAlimentoDescrizione = sosp.tempososTipoAlimentoDescrizione,
                                                                           .TempososUnitaMisuraCodice = sosp.tempososUnitaMisuraCodice,
                                                                           .TempososUnitaMisuraDescrizione = sosp.tempososUnitaMisuraDescrizione,
                                                                           .TempososValore = sosp.tempososValore
                                                                       }).ToList
    End Function

    ''' <summary>
    ''' Legge le prescrizioni (da Ricette_Zoo) in giacenza ed elimina quelle non piu' presenti  
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sta_Num"></param>
    ''' <param name="Data_Inizio"></param>
    ''' <param name="Data_Fine"></param>
    ''' <param name="listaPrescrizioni"></param>
    Private Function EliminaPrescrizioni(ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Sta_Num As Integer,
                                         ByVal Data_Inizio As Date,
                                         ByVal Data_Fine As Date,
                                         ByVal listaPrescrizioni As List(Of Zoo.Prescrizione)) As Integer
        Dim ricetteCancellate As Integer

        Try
            Dim ricetteZooGiacenza = GiasContext.Ricette_Zoo.
                Where(Function(rz) rz.Piva = Piva AndAlso rz.Sa_Cod = Sa_Cod AndAlso rz.Sta_Num = Sta_Num AndAlso
                    rz.DataEmissione >= Data_Inizio AndAlso rz.DataEmissione <= Data_Fine).ToList
            Dim presNumeroToDelete As List(Of String) = ricetteZooGiacenza.Select(Function(rz) rz.Numero).
                    Except(listaPrescrizioni.Select(Function(pr) pr.PresNumero).Distinct.ToList).ToList

            If presNumeroToDelete.Count > 0 Then
                ' Ricava i trattamenti creati dalle possibili prescrizioni in cancellazione
                Dim ricetteZooIds = ricetteZooGiacenza.Select(Function(rz) rz.IdRicetta).ToList
                Dim trattFromPres As List(Of Integer) = GiasContext.Ricette_ZooxAgenda.
                    Where(Function(rzxa) ricetteZooIds.Contains(rzxa.Id_Ricetta)).
                    Select(Function(rzxa) rzxa.Id_Ricetta).ToList

                ' In questo modo elimina le vecchie prescrizioni non piu' presenti su VetInfo,
                ' ma non quelle legate a un trattamento gia' creato da esse
                Dim listRicetteZooToDelete = GiasContext.Ricette_Zoo.
                    Where(Function(rz) rz.Piva = Piva AndAlso rz.Sa_Cod = Sa_Cod AndAlso rz.Sta_Num = Sta_Num AndAlso
                        Not trattFromPres.Contains(rz.IdRicetta) AndAlso presNumeroToDelete.Contains(rz.Numero)).ToArray

                Dim ricettaZoo_W As New AgronicaCoreContabBIZ.Ricette_Zoo_W
                ricettaZoo_W.EliminaRicetteCollegate(listRicetteZooToDelete, GiasContext, objP_Server)
                GiasContext.SaveChanges()

                ricetteCancellate = listRicetteZooToDelete.Count
            End If
        Catch ex As Exception
            ricetteCancellate = 0
            Throw New Exception("Errore durante la cancellazione delle prescrizioni: " + ex.Message)
        End Try

        Return ricetteCancellate

    End Function

    ''' <summary>
    ''' Elimina la riga di prescrizione e i capi associati non piu' appartenenti alla prescrizione (se presenti)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="_idRicetta"></param>
    ''' <param name="listaRighePres"></param>
    ''' <returns></returns>
    Private Function EliminaRighePrescrizione(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal _idRicetta As Integer,
                                              ByVal listaRighePres As List(Of Zoo.RigaPrescrizione)) As Integer
        Dim righeCancellate As Integer = 0

        Try
            Dim righeRicetteZooGiacenza = GiasContext.Ricette_Zoo_Agenda.
                Where(Function(rza) rza.Piva = Piva AndAlso rza.Sa_Cod = Sa_Cod AndAlso rza.IdRicetta = _idRicetta).ToList

            Dim presRigaNumeroToDelete As List(Of String) = righeRicetteZooGiacenza.Select(Function(rza) rza.Numero).
                Except(listaRighePres.Select(Function(pres) pres.PresrigaNumero)).Distinct.ToList

            If presRigaNumeroToDelete.Count > 0 Then
                ' Ricava le righe di trattamenti create dalle possibili righe di prescrizioni in cancellazione
                Dim righeRicetteZooKeys = righeRicetteZooGiacenza.
                          Select(Function(rz) New With {rz.IdRicetta, rz.IdAgenda}).ToList

                Dim trattFromPres = GiasContext.Ricette_ZooxAgenda.Join(righeRicetteZooGiacenza,
                         Function(rzxa) New With {Key .IdRicetta = rzxa.Id_Ricetta, Key .IdAgenda = rzxa.Id_RigaRicetta},
                         Function(rz) New With {Key .IdRicetta = rz.IdRicetta, Key .IdAgenda = rz.IdAgenda},
                         Function(rzxa, rz) New With {rz.IdRicetta, rz.IdAgenda}).ToList

                ' In questo modo elimina le vecchie righe di prescrizioni non piu' presenti su VetInfo,
                ' ma non quelle legate a un trattamento gia' creato da esse
                Dim listRicetteZooAgendaToDelete = GiasContext.Ricette_Zoo_Agenda.
                    Where(Function(rca) rca.Piva = Piva AndAlso rca.Sa_Cod = Sa_Cod AndAlso rca.IdRicetta = _idRicetta AndAlso
                        presRigaNumeroToDelete.Contains(rca.Numero)).
                    Where(Function(rca) Not trattFromPres.Any(Function(tp) tp.IdRicetta = rca.IdRicetta AndAlso tp.IdAgenda = rca.IdAgenda)).
                    ToArray()

                Dim ricetteZooAg_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W
                ricetteZooAg_W.EliminaRicetteCollegate(listRicetteZooAgendaToDelete, GiasContext, objP_Server)
                GiasContext.SaveChanges()

                righeCancellate = listRicetteZooAgendaToDelete.Count
            End If
        Catch ex As Exception
            righeCancellate = 0
            Throw New Exception("Errore durante la cancellazione delle righe della prescrizione: " + ex.Message)
        End Try

        Return righeCancellate

    End Function

    ''' <summary>
    ''' Elimina i capi non piu' appartententi non piu' appartenenti alla riga di prescrizione (se presenti)
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="idRicetta"></param>
    ''' <param name="idAgenda"></param>
    ''' <param name="idDettaglio"></param>
    ''' <param name="listaCapiPres"></param>
    ''' <returns></returns>
    Private Function EliminaCapiPrescrizione(ByVal Piva As String,
                                             ByVal Sa_Cod As Integer,
                                             ByVal idRicetta As Integer,
                                             ByVal idAgenda As Integer,
                                             ByVal idDettaglio As Integer,
                                             ByVal listaCapiPres As List(Of Zoo.CapoRigaPrescrizione)) As Integer
        Dim capiCancellati As Integer = 0
        Try
            Dim listNumeroCapoGiacenza As List(Of String) = GiasContext.Ricette_Zoo_Destinazioni.
                Where(Function(rzd) rzd.Piva = Piva AndAlso rzd.Sa_Cod = Sa_Cod AndAlso rzd.IdRicetta = idRicetta AndAlso rzd.IdAgenda = idAgenda AndAlso rzd.IdDettaglio = idDettaglio).
                Select(Function(rzd) rzd.Numero).ToList()

            Dim presRigaCapoNumeroToDelete As List(Of String) = listNumeroCapoGiacenza.Except(listaCapiPres.Select(Function(prc) prc.PrescapoNumero)).Distinct().ToList
            If presRigaCapoNumeroToDelete.Count > 0 Then
                Dim listRicetteZooDestToDelete = GiasContext.Ricette_Zoo_Destinazioni.
                    Where(Function(rzd) rzd.Piva = Piva AndAlso rzd.Sa_Cod = Sa_Cod AndAlso rzd.IdRicetta = idRicetta AndAlso rzd.IdAgenda = idAgenda AndAlso
                              rzd.IdDettaglio = idDettaglio AndAlso presRigaCapoNumeroToDelete.Contains(rzd.Numero)).ToArray()

                Dim ricetteZooDest_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Destinazioni_W
                ricetteZooDest_W.Elimina(listRicetteZooDestToDelete, GiasContext, objP_Server)
                GiasContext.SaveChanges()

                capiCancellati = listRicetteZooDestToDelete.Count
            End If
        Catch ex As Exception
            capiCancellati = 0
            Throw New Exception("Errore durante la cancellazione dei capi nella riga della prescrizione: " + ex.Message)
        End Try

        Return capiCancellati

    End Function

    ''' <summary>
    ''' Valorizza i campi di Ricetta_Zoo da un oggetto Prescrizione
    ''' </summary>
    ''' <param name="prescrizione"></param>
    ''' <param name="ricettaZoo"></param>
    Private Sub prescrizioneToRicettaZoo(ByVal prescrizione As Zoo.Prescrizione,
                                         ByRef ricettaZoo As AgronicaCoreEntityFramework_POCO.Ricette_Zoo)
        ricettaZoo.Numero = prescrizione.PresNumero
        ricettaZoo.Pin = prescrizione.PresPin
        ricettaZoo.Note = prescrizione.PresNote
        ricettaZoo.RigaCardinalita = prescrizione.PresRigaCardinalita
        ricettaZoo.DetentoreIdFiscale = prescrizione.PresDetentoreIdFiscale
        ricettaZoo.ProprietarioIdFiscale = prescrizione.PresProprietarioIdFiscale
        ricettaZoo.VeterinarioIdFiscale = prescrizione.PresVeterinarioIdFiscale
        ricettaZoo.DataEmissione = If(IsNothing(prescrizione.PresDtEmissione), AGRODATAINIZIO, prescrizione.PresDtEmissione)
        ricettaZoo.StatoCodice = If(prescrizione.PresStatoCodice.Equals(""), 0, 1)
        ricettaZoo.TipoCodice = tipoCodiceToEnum(prescrizione.PresTipoCodice)
        ricettaZoo.StrutturaCodice = prescrizione.PresStrutturaCodice
        ricettaZoo.StrutturaDenominazione = prescrizione.PresStrutturaDenominazione
        ricettaZoo.ProtocolloCodice = prescrizione.ProtCatClassyfarmCodice
        ricettaZoo.Id_Protocollo = 0
        ricettaZoo.Blocco_Data = AGRODATAINIZIO
    End Sub

    ''' <summary>
    ''' Valorizza i campi di Ricetta_Zoo_Agenda da un oggetto RigaPrescrizione
    ''' </summary>
    ''' <param name="rigaPres"></param>
    ''' <param name="ricettaZooAg"></param>
    Private Sub rigaPresToRicettaZooAgenda(ByVal rigaPres As Zoo.RigaPrescrizione,
                                           ByRef ricettaZooAg As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
                                           ByVal tipoPres As enum_TipoPrescrizione)
        ricettaZooAg.Numero = rigaPres.PresrigaNumero
        ricettaZooAg.DataInizioTrattamento = If(IsNothing(rigaPres.PresrigaDtInizioTrattamento) OrElse rigaPres.PresrigaDtInizioTrattamento < AGRODATAINIZIO, AGRODATAINIZIO, rigaPres.PresrigaDtInizioTrattamento)
        ricettaZooAg.DataFineTrattamento = If(IsNothing(rigaPres.PresrigaDtFineTrattamento) OrElse rigaPres.PresrigaDtFineTrattamento < AGRODATAINIZIO, AGRODATAFINE, rigaPres.PresrigaDtFineTrattamento)
        ricettaZooAg.Descrizione = rigaPres.PresrigaDescrizione
        ricettaZooAg.Lav_Cod = LAVCOD_CUREMEDICAMENTI_ANIMALI
        ricettaZooAg.Des_Lib = "Cure e medicamenti"
        ricettaZooAg.DurataTrattamento = rigaPres.PresrigaDurataTrattamento
        ricettaZooAg.Note = rigaPres.PresrigaNote
        ricettaZooAg.Blocco_Data = AGRODATAINIZIO

        If {enum_TipoPrescrizione.Veterinaria, enum_TipoPrescrizione.Indicazione_Terapeutica}.Contains(tipoPres) Then
            ricettaZooAg.Intervallo_Somm = 1
            If rigaPres.PresrigaDurataTrattamento > 0 Then
                ricettaZooAg.Numero_Somm = rigaPres.PresrigaDurataTrattamento
            Else
                If ricettaZooAg.DataInizioTrattamento.Value > AGRODATAINIZIO AndAlso ricettaZooAg.DataFineTrattamento.Value < AGRODATAFINE Then
                    Dim giorniDifferenza As Integer = (ricettaZooAg.DataFineTrattamento.Value - ricettaZooAg.DataInizioTrattamento.Value).Days + 1
                    ricettaZooAg.Numero_Somm = giorniDifferenza
                Else
                    ricettaZooAg.Numero_Somm = 1
                End If
            End If


        End If
    End Sub

    ''' <summary>
    ''' Valorizza i campi di Ricetta_Zoo_Dettagli da un oggetto RigaPrescrizione
    ''' </summary>
    ''' <param name="rigaPres"></param>
    ''' <param name="ricettaZooDett"></param>
    Private Sub rigaPresToRicettaZooDettagli(ByVal rigaPres As Zoo.RigaPrescrizione,
                                             ByRef ricettaZooDett As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli)
        ricettaZooDett.RegScoNumero = rigaPres.RegscoNumero
        ricettaZooDett.FlTipoMedicinale = tipoMedicinaleToEnum(rigaPres.PresrigaFlTipoMedicinale)
        ricettaZooDett.FlAntimicrobico = If(IsNothing(rigaPres.PresrigaFlAntimicrobico) OrElse Not rigaPres.PresrigaFlAntimicrobico, 0, 1)
        ricettaZooDett.FlOrmonale = If(IsNothing(rigaPres.PresrigaFlOrmonale) OrElse Not rigaPres.PresrigaFlOrmonale, 0, 1)
        ricettaZooDett.FlVaccino = If(IsNothing(rigaPres.PresrigaFlVaccino) OrElse Not rigaPres.PresrigaFlVaccino, 0, 1)
        ricettaZooDett.MangimeComposizione = rigaPres.PresrigaMangimeComposizione
        ricettaZooDett.MangimeDenominazione = rigaPres.PresrigaMangimeDenominazione
        ricettaZooDett.Posologia = rigaPres.PresrigaPosologia
        ' Se manca il prodotto (codice AIC), la prescrizione potrebbe essere un Protocollo ed avere la famiglia AIC del prodotto
        ricettaZooDett.ProdottoAic = If(String.IsNullOrEmpty(rigaPres.PresrigaProdottoAic), rigaPres.PresrigaProdottoFamigliaAic, rigaPres.PresrigaProdottoAic)
        ricettaZooDett.Quantitativo = rigaPres.PresrigaQuantitativo
        ricettaZooDett.Udm_Cod = If(IsNothing(rigaPres.PresrigaProdottoUnitaMisuraCodice), 0, tipoUnitaMisuraToEnum(rigaPres.PresrigaProdottoUnitaMisuraCodice))
        ricettaZooDett.RegScoNumero = rigaPres.RegscoNumero
        ricettaZooDett.Qta_Dose = 0
        ricettaZooDett.Udm_Dose = 0
        ricettaZooDett.Arrotondamento_Peso = 0
        ricettaZooDett.Massivo = 0

        ' Distinzione tra prodotti - farmaci e mangimi (quest'ultimi non gestiti)
        If CInt(ricettaZooDett.FlTipoMedicinale) > 5 Then
            ricettaZooDett.Elem_Cod = MANGIMI
            ricettaZooDett.Mat_Cod = 0
        ElseIf CInt(ricettaZooDett.FlTipoMedicinale) <> 0 Then
            ricettaZooDett.Elem_Cod = CostantiPersonalizzate.FARMACI
            Dim proCod As Integer
            ricettaZooDett.Pro_Cod = If(IsNothing(ricettaZooDett.ProdottoAic), 0, If(dictFarmaci_AicToProCod.TryGetValue(ricettaZooDett.ProdottoAic, proCod), proCod, 0))
        End If
    End Sub

    ''' <summary>
    ''' Valorizza i campi di Ricetta_Zoo_Destinazione da un oggetto CapoRigaPrescrizione
    ''' </summary>
    ''' <param name="capoRigaPres"></param>
    ''' <param name="ricettaZooDest"></param>
    Private Sub capoRigaPresToRicettaZooDestinazioni(ByVal capoRigaPres As Zoo.CapoRigaPrescrizione,
                                                     ByRef ricettaZooDest As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni)
        ricettaZooDest.Matricola = capoRigaPres.PrescapoIdentificativo
        ricettaZooDest.DataNascita = If(IsNothing(capoRigaPres.PrescapoDtNascita), AGRODATAINIZIO, CDate(capoRigaPres.PrescapoDtNascita))
        ricettaZooDest.CodificaCodice = capoRigaPres.PrescapoCodificaCodice
        ricettaZooDest.CodificaDescrizione = capoRigaPres.PrescapoCodificaDescrizione
        ricettaZooDest.DiagnosiCodice = capoRigaPres.PrescapoDiagnosiCodice
        ricettaZooDest.DiagnosiDescrizione = capoRigaPres.PrescapoDiagnosiDescrizione
        ricettaZooDest.FlStatoAnomalia = capoRigaPres.PrescapoFlStatoAnomalia
        ricettaZooDest.Identificativo = capoRigaPres.PrescapoIdentificativo
        ricettaZooDest.Numero = capoRigaPres.PrescapoNumero 'TODO nel caso di gruppo di animali
        ricettaZooDest.Note = capoRigaPres.PrescapoNote
        ricettaZooDest.NumeroAnimali = If(IsNumeric(capoRigaPres.PrescapoNumeroAnimali), CInt(capoRigaPres.PrescapoNumeroAnimali), 0)
        ricettaZooDest.Sesso = capoRigaPres.PrescapoSesso
        ricettaZooDest.SomministrazioneCodice = capoRigaPres.PrescapoSommministrazioneCodice
        ricettaZooDest.SottocategoriaCodice = capoRigaPres.PrescapoSottocategoriaCodice
        ricettaZooDest.SpecieCodice = capoRigaPres.PrescapoSpecieCodice
        ricettaZooDest.Cardinalita = 0 'TODO come gestire la cardinalita tra diverse righe?
        ricettaZooDest.RegSco_Numero = ""
    End Sub

    Private Sub tempoSospensioneToRicettaZooDettaglioTecnico(ByVal tempoSos As TempiSospensione,
                                                             ByRef ricettaZooDettTecn As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico)
        ricettaZooDettTecn.Dett_Cod = tempoSos.Alimento.codice
        ricettaZooDettTecn.Lotto = ""
        ricettaZooDettTecn.Extra_Str = ""
        ricettaZooDettTecn.Extra_Int = tempoSos.tempoSospensione
    End Sub

    Private Function convertTempoSosp(ByVal tempoSosUdm As String, ByVal tempoSosVal As Integer)
        If tempoSosUdm = "H" Then
            Return CInt(tempoSosVal / 24)
        Else
            Return tempoSosVal
        End If
    End Function

    Private Function getTempiSospensioneFromCapiRigaPres(ByVal capiFromRigaPres As List(Of Zoo.CapoRigaPrescrizione)) As List(Of TempiSospensione)
        Dim tempiSospensione As New List(Of TempiSospensione)

        capiFromRigaPres = capiFromRigaPres.
            Where(Function(capo) capo.PrescapoTempiSospensione IsNot Nothing AndAlso capo.PrescapoTempiSospensione.Count > 0).ToList
        If capiFromRigaPres.Count > 0 Then
            Dim sospCarne As TempiSospensione = capiFromRigaPres.
                SelectMany(Function(capo) capo.PrescapoTempiSospensione).
                Where(Function(sosp) sosp.TempososTipoAlimentoCodice = "CARNE").
                Select(Function(sosp) New TempiSospensione With {
                    .Alimento = New baseClass.BaseCodeDescr(1, "Carne"),
                    .tempoSospensione = convertTempoSosp(sosp.TempososUnitaMisuraCodice, sosp.TempososValore)
                }).FirstOrDefault

            If sospCarne IsNot Nothing Then tempiSospensione.Add(sospCarne)

            Dim sospLatte As TempiSospensione = capiFromRigaPres.
                SelectMany(Function(capo) capo.PrescapoTempiSospensione).
                Where(Function(sosp) sosp.TempososTipoAlimentoCodice = "LATTE").
                Select(Function(sosp) New TempiSospensione With {
                    .Alimento = New baseClass.BaseCodeDescr(2, "Latte"),
                    .tempoSospensione = convertTempoSosp(sosp.TempososUnitaMisuraCodice, sosp.TempososValore)
                }).FirstOrDefault

            If sospLatte IsNot Nothing Then tempiSospensione.Add(sospLatte)

        End If

        Return tempiSospensione

    End Function

    ''' <summary>
    ''' Verifica modifiche sui campi di Ricette_Zoo (prescrizione)
    ''' </summary>
    ''' <param name="original"></param>
    ''' <param name="updated"></param>
    ''' <param name="fieldsToExclude"></param>
    ''' <returns></returns>
    Private Function HasPrescrizioneChanged(ByVal original As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
                                            ByVal updated As AgronicaCoreEntityFramework_POCO.Ricette_Zoo,
                                            Optional ByVal fieldsToExclude As List(Of String) = Nothing) As Boolean
        If IsNothing(fieldsToExclude) OrElse fieldsToExclude.Count = 0 Then
            fieldsToExclude = {"IdRicetta", "Piva", "Sa_Cod", "Sta_Num", "Numero", "Blocco_Flag", "Blocco_Data", "Blocco_Username", "Inviato", "DataInvio", "Data_Creazione", "Data_Modifica", "Username_Creazione", "Username_Modifica", "Validita_Inizio", "Validita_Fine"}.ToList
        End If

        For Each prop In GetType(AgronicaCoreEntityFramework_POCO.Ricette_Zoo).GetProperties()
            If fieldsToExclude.Contains(prop.Name) Then Continue For

            Dim origValue = prop.GetValue(original)
            Dim updValue = prop.GetValue(updated)

            If Not Equals(origValue, updValue) Then Return True
        Next

        Return False

    End Function

    ''' <summary>
    ''' Verifica modifiche sui campi di Ricette_Zoo_Agenda (riga prescrizione)
    ''' </summary>
    ''' <param name="original"></param>
    ''' <param name="updated"></param>
    ''' <param name="fieldsToExclude"></param>
    ''' <returns></returns>
    Private Function HasRigaPresChanged(ByVal original As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
                                        ByVal updated As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda,
                                        Optional ByVal fieldsToExclude As List(Of String) = Nothing) As Boolean
        If IsNothing(fieldsToExclude) OrElse fieldsToExclude.Count = 0 Then
            fieldsToExclude = {"IdAgenda", "IdRicetta", "Piva", "Sa_Cod", "Lav_Cod", "Des_Lib", "Numero", "Blocco_Flag", "Blocco_Data", "Blocco_Username", "Inviato", "DataInvio", "Data_Creazione", "Data_Modifica", "Username_Creazione", "Username_Modifica", "Validita_Inizio", "Validita_Fine"}.ToList
        End If

        For Each prop In GetType(AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Agenda).GetProperties()
            If fieldsToExclude.Contains(prop.Name) Then Continue For

            Dim origValue = prop.GetValue(original)
            Dim updValue = prop.GetValue(updated)

            If Not Equals(origValue, updValue) Then Return True
        Next

        Return False

    End Function

    ''' <summary>
    ''' Verifica modifiche sui campi di Ricette_Zoo_Dettagli (riga prescrizione)
    ''' </summary>
    ''' <param name="original"></param>
    ''' <param name="updated"></param>
    ''' <param name="fieldsToExclude"></param>
    ''' <returns></returns>
    Private Function HasRigaPresDettChanged(ByVal original As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
                                            ByVal updated As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli,
                                            Optional ByVal fieldsToExclude As List(Of String) = Nothing) As Boolean
        If IsNothing(fieldsToExclude) OrElse fieldsToExclude.Count = 0 Then
            fieldsToExclude = {"IdDettaglio", "IdMov", "IdAgenda", "IdRicetta", "Piva", "Sa_Cod", "Inviato", "DataInvio", "Data_Creazione", "Data_Modifica", "Username_Creazione", "Username_Modifica", "Validita_Inizio", "Validita_Fine"}.ToList
        End If

        For Each prop In GetType(AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettagli).GetProperties()
            If fieldsToExclude.Contains(prop.Name) Then Continue For

            Dim origValue = prop.GetValue(original)
            Dim updValue = prop.GetValue(updated)

            If Not Equals(origValue, updValue) Then Return True
        Next

        Return False

    End Function

    ''' <summary>
    ''' Verifica modifiche sui campi di Ricette_Zoo_Dettaglio_Tecnico (tempi sospensione prodotto)
    ''' </summary>
    ''' <param name="original"></param>
    ''' <param name="updated"></param>
    ''' <param name="fieldsToExclude"></param>
    ''' <returns></returns>
    Private Function HasRigaPresTempoSospChanged(ByVal original As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
                                                 ByVal updated As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico,
                                                 Optional ByVal fieldsToExclude As List(Of String) = Nothing) As Boolean
        If IsNothing(fieldsToExclude) OrElse fieldsToExclude.Count = 0 Then
            fieldsToExclude = {"Id_Reg_Dettaglio", "Id_Mov_Det", "Id_Mov", "Id_Agenda", "Id_Ricetta", "Piva", "Sa_Cod", "Inviato", "DataInvio", "Data_Creazione", "Data_Modifica", "Username_Creazione", "Username_Modifica", "Validita_Inizio", "Validita_Fine"}.ToList
        End If

        For Each prop In GetType(AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Dettaglio_Tecnico).GetProperties()
            If fieldsToExclude.Contains(prop.Name) Then Continue For

            Dim origValue = prop.GetValue(original)
            Dim updValue = prop.GetValue(updated)

            If Not Equals(origValue, updValue) Then Return True
        Next

        Return False

    End Function

    ''' <summary>
    ''' Verifica modifiche sui campi di Ricette_Zoo_Destinazioni (capo riga prescrizione)
    ''' </summary>
    ''' <param name="original"></param>
    ''' <param name="updated"></param>
    ''' <param name="fieldsToExclude"></param>
    ''' <returns></returns>
    Private Function HasRigaPresCapoChanged(ByVal original As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
                                            ByVal updated As AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni,
                                            Optional ByVal fieldsToExclude As List(Of String) = Nothing) As Boolean
        If IsNothing(fieldsToExclude) OrElse fieldsToExclude.Count = 0 Then
            fieldsToExclude = {"IdDestinazione", "IdDettaglio", "IdMov", "IdAgenda", "IdRicetta", "Piva", "Sa_Cod", "Cod_Animale", "Matricola", "Sesso", "SpecieCodice", "Inviato", "DataInvio", "Data_Creazione", "Data_Modifica", "Username_Creazione", "Username_Modifica", "Validita_Inizio", "Validita_Fine"}.ToList
        End If

        For Each prop In GetType(AgronicaCoreEntityFramework_POCO.Ricette_Zoo_Destinazioni).GetProperties()
            If fieldsToExclude.Contains(prop.Name) Then Continue For

            Dim origValue = prop.GetValue(original)
            Dim updValue = prop.GetValue(updated)

            If Not Equals(origValue, updValue) Then Return True
        Next

        Return False

    End Function

    ''' <summary>
    ''' Restituisce il valore tipo di Ricetta_Zoo dal campo TipoCodice di un oggetto Prescrizione
    ''' </summary>
    ''' <param name="tipoCodice"></param>
    ''' <returns></returns>
    Private Function tipoCodiceToEnum(ByVal tipoCodice As String) As Integer
        Dim t As Integer = enum_TipoPrescrizione.UNDEFINED
        Select Case tipoCodice
            Case "DA_PROTOCOLLO"
                t = enum_TipoPrescrizione.Da_Protocollo
            Case "PROTOCOLLO"
                t = enum_TipoPrescrizione.Protocollo_Terapeutico
            Case "PRESTRATSCORTA"
                t = enum_TipoPrescrizione.Indicazione_Terapeutica
            Case "PRESVET"
                t = enum_TipoPrescrizione.Veterinaria
            Case "PRESSCORTIMP"
                t = enum_TipoPrescrizione.Rifornimento_Scorta
            Case Else
                t = enum_TipoPrescrizione.UNDEFINED
        End Select

        Return t
    End Function

    ''' <summary>
    ''' Restituisce il valore tipo medicinale di Ricette_Zoo_Dettagli dal campo TipoMedicinale di un oggetto RigaPrescrizione
    ''' </summary>
    ''' <param name="tipoMed"></param>
    ''' <returns></returns>
    Private Function tipoMedicinaleToEnum(ByVal tipoMed As String) As Integer
        Dim t As Integer = enum_TipoProdotto_Prescrizione.UNDEFINED
        Select Case tipoMed
            Case "N"
                t = enum_TipoProdotto_Prescrizione.Prodotto_Medicinale_AIC
            Case "E"
                t = enum_TipoProdotto_Prescrizione.Medicinale_Estero
            Case "O"
                t = enum_TipoProdotto_Prescrizione.Prodotto_Omeopatico
            Case "S"
                t = enum_TipoProdotto_Prescrizione.Prodotto_Galenico
            Case "C"
                t = enum_TipoProdotto_Prescrizione.Mangime_Completo
            Case "X"
                t = enum_TipoProdotto_Prescrizione.Mangime_Complementare
            Case "I"
                t = enum_TipoProdotto_Prescrizione.Mangime_Intermedio
            Case "J"
                t = enum_TipoProdotto_Prescrizione.Mangime_Estero
        End Select

        Return t
    End Function

    Private Function tipoUnitaMisuraToEnum(ByVal tipoUdm As String) As Integer
        Dim t As Integer = enum_UnitaMisura.Numero
        Select Case tipoUdm.ToLower
            Case "kg"
                t = enum_UnitaMisura.KG
            Case "scat"
                t = enum_UnitaMisura.Numero
            Case "ml"
                t = enum_UnitaMisura.Millilitri
            Case "pezzo"
                t = enum_UnitaMisura.Numero
            Case "g"
                t = enum_UnitaMisura.Grammi
        End Select

        Return t

    End Function

#End Region

#Region "Scrittura/Modifica Trattamenti"

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="stato"></param>
    ''' <returns></returns>
    Private Function statoDesToStatoCod(ByVal stato As String)
        Select Case stato
            Case "APERTO"
                Return enum_StatoTrattamento.Aperto
            Case "IN_CORSO", "CONFERMATO", "IN_CORSO_RIAPERTO"
                Return enum_StatoTrattamento.InCorso
            Case "CHIUSO"
                Return enum_StatoTrattamento.Chiuso
            Case "CHIUSO_CON_ANOMALIA"
                Return enum_StatoTrattamento.Chiuso_Anomalia
            Case Else
                Return ""
        End Select
    End Function

    ''' <summary>
    ''' Aggiorna il campo Note del Trattamento nel caso di modifica inline prima dell'invio
    ''' </summary>
    ''' <param name="tratt"></param>
    Private Sub UpdateNoteTrattamento(tratt As Trattamento_ToSend)
        Dim idAgenda As Integer = tratt.IdAgenda

        Dim movZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
        Dim movZoo = (From mzoo In GiasContext.Movimenti_Zoo
                      Join mov In GiasContext.Movimenti On mov.Id_Agenda Equals mzoo.Id_Agenda And mov.Id_Mov Equals mzoo.Id_Mov
                      Where mov.Cau_Mov = CAU_TRATTAMENTO_ZOO And mov.Id_Agenda = idAgenda
                      Select mzoo).FirstOrDefault

        movZoo.Note = tratt.Note
        movZoo_W.Scrivi_Modifica(movZoo, GiasContext, objP_Server)
        GiasContext.SaveChanges()
    End Sub

    ''' <summary>
    ''' Verifica se un Trattamento/Somministrazione della Ricetta associata ha gia' creato l'Indicazione Terapeutica dal Protocollo Terapeutico
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function HandleExistingIndicazioneTerapeutica(ByRef tratt As Trattamento_ToSend) As CreateTrattamentoFromProtocollo_Response
        Dim gruppoRicetta As Integer = tratt.GruppoRicetta
        Dim sommsGroup = GiasContext.Ricette_Zoo.Where(Function(r) r.Gruppo_Ricetta = gruppoRicetta).ToList

        If Not sommsGroup.Any(Function(s) s.Numero <> "") Then Return New CreateTrattamentoFromProtocollo_Response(False, presNumero:="")

        Dim firstSomm = sommsGroup.FirstOrDefault(Function(s) s.Numero <> "")
        Dim firstSommId As Integer = firstSomm.IdRicetta
        Dim indTerapNum As String = firstSomm.Numero

        Dim indTerapRigaNum As String = GiasContext.Ricette_Zoo_Agenda.Where(Function(r) r.IdRicetta = firstSommId).FirstOrDefault.Numero

        tratt.PresNumero = indTerapNum
        tratt.PresRigaNumero = indTerapRigaNum
        Return New CreateTrattamentoFromProtocollo_Response(True, presNumero:=indTerapNum, presRigaNumeri:={indTerapRigaNum}.ToList, famiglieAic:={tratt.FamigliaAic}.ToList)

    End Function

    ''' <summary>
    ''' Verifica da VetInfo se il trattamento è chiuso
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function IsTrattamentoClosed(ByRef tratt As Trattamento_ToSend)
        Dim isClosed As Boolean = False
        Dim trattResp As RegistroTrattamenti_Response =
                ws.Read_Trattamento(tratt.AziendaCodice, tratt.PropIdFiscale, tratt.TrattNumero)

        If Not IsNothing(trattResp) Then isClosed =
                {enum_StatoTrattamento.Chiuso, enum_StatoTrattamento.Chiuso_Anomalia}.Contains(statoDesToStatoCod(trattResp.tratStatoCodice))

        Return isClosed

    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <param name="msg"></param>
    Private Sub DoNotCloseTrattamento(ByRef tratt As Trattamento_ToSend,
                                      Optional ByVal msg As String = "")
        If tratt.ToClose Then
            tratt.ToClose = False

            If String.IsNullOrEmpty(tratt.msgErrore) Then tratt.msgErrore =
                If(String.IsNullOrEmpty(msg), "Il trattamento non puo' essere chiuso.", msg)
        End If
    End Sub

    ''' <summary>
    ''' Recupera la Scorta (RegScoNumero) dei Prodotti utilizzati nel Trattamento generato dal Protocollo Terapeutico
    ''' </summary>
    ''' <param name="indTerap">Indicazione Terapeutica creata dal Protocollo Terapeutico</param>
    ''' <param name="tratt"></param>
    Private Function SetScortaTrattamentoDaProtocollo(ByRef indTerap As CreateTrattamentoFromProtocollo_Response,
                                                      ByRef tratt As Trattamento_ToSend) As Boolean
        Dim hasScorta As Boolean

        ' RegScoNumero potrebbe essere gia' valorizzato
        If tratt.MultiAIC Then
            If tratt.Prodotti.All(Function(p) Not String.IsNullOrEmpty(p.RegScoNumero)) Then Return True

            For Each prod In tratt.Prodotti.Where(Function(p) String.IsNullOrEmpty(p.RegScoNumero))
                Dim famigliaAic As String = tratt.FamigliaAic
                Dim presRigaNum = indTerap.presRigaNumeri(indTerap.famiglieAic.IndexOf(famigliaAic))
                If Not IsNothing(presRigaNum) Then
                    Dim aicToFind As String = prod.Aic
                    Dim filterAic As New VetInfoFilter With {
                        .field = "regscoProdottoAic",
                        .op = "EQUALS",
                        .value1 = aicToFind
                    }

                    prod.RegScoNumero = ""

                    Dim scortaProd_daPrescrizione = ws.Leggi_ScortaProdotto_perSomministrazione({presRigaNum}.ToList, {filterAic}.ToList)
                    If scortaProd_daPrescrizione IsNot Nothing AndAlso scortaProd_daPrescrizione.Count > 0 Then
                        Dim idAgenda As Integer = tratt.IdAgenda
                        Dim prodCod As Integer = prod.Pro_Cod

                        Dim scortaProd = scortaProd_daPrescrizione.Find(Function(scorta) scorta.regscoQuantitativo > 0)
                        If scortaProd Is Nothing Then Throw New Exception($"Scorta non presente o vuota per il farmaco {prod.Pro_Des}.")
                        Dim regScoNum As String = scortaProd.regscoNumero

                        'rigaPres.RegscoNumero = regScoNum
                        prod.RegScoNumero = regScoNum

                        ' Salva il Registro di Scorta se presente
                        GiasContext.Movimenti_dettagli.
                            Where(Function(mdtt) mdtt.Id_Agenda = idAgenda And mdtt.Pro_Cod = prodCod).ToList.
                            ForEach(Sub(movdett)
                                        movdett.RegSco_Numero = regScoNum
                                        GiasContext.Entry(movdett).State = EntityState.Modified
                                    End Sub)
                        GiasContext.SaveChanges()
                    End If
                End If
            Next

            hasScorta = tratt.Prodotti.All(Function(p) Not String.IsNullOrEmpty(p.RegScoNumero))
        Else
            If Not String.IsNullOrEmpty(tratt.RegScoNumero) Then Return True

            Dim famigliaAic As String = tratt.FamigliaAic
            Dim presRigaNum = indTerap.presRigaNumeri(indTerap.famiglieAic.IndexOf(famigliaAic))
            If Not IsNothing(presRigaNum) Then
                Dim aicToFind As String = tratt.Aic
                Dim filterAic As New VetInfoFilter With {
                    .field = "regscoProdottoAic",
                    .op = "EQUALS",
                    .value1 = aicToFind
                }

                tratt.RegScoNumero = ""
                'rigaPres.RegscoNumero = ""

                Dim scortaProd_daPrescrizione = ws.Leggi_ScortaProdotto_perSomministrazione({presRigaNum}.ToList, {filterAic}.ToList)
                If scortaProd_daPrescrizione IsNot Nothing AndAlso scortaProd_daPrescrizione.Count > 0 Then
                    Dim idAgenda As Integer = tratt.IdAgenda
                    Dim regScoNum As String = scortaProd_daPrescrizione.Find(Function(scorta) scorta.regscoQuantitativo > 0).regscoNumero

                    'rigaPres.RegscoNumero = regScoNum
                    tratt.RegScoNumero = regScoNum

                    ' Salva il Registro di Scorta se presente
                    GiasContext.Movimenti_dettagli.
                        Where(Function(mdtt) mdtt.Id_Agenda = idAgenda).ToList.
                        ForEach(Sub(movdett)
                                    movdett.RegSco_Numero = regScoNum
                                    GiasContext.Entry(movdett).State = EntityState.Modified
                                End Sub)
                    GiasContext.SaveChanges()

                    hasScorta = True
                Else
                    hasScorta = False
                End If
            End If
        End If

        Return hasScorta

    End Function

    ''' <summary>
    ''' Linka il Trattamento all'Indicazione Terapeutica appena creata dal Protocollo Terapeutico di partenza	
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <param name="indTerap"></param>
    Private Sub LinkTrattamentoToIndicazioneTerapeutica(ByVal indTerap As CreateTrattamentoFromProtocollo_Response,
                                                        ByRef tratt As Trattamento_ToSend)
        Dim famigliaAic As String = ""

        If tratt.MultiAIC Then
            If tratt.Prodotti.Select(Function(p) p.FamigliaAic).Distinct.Count = 1 Then
                famigliaAic = tratt.Prodotti.First().FamigliaAic
            Else
                Throw New Exception("Impossibile collegare il trattamento all'indicazione terapeutica generata: il trattamento contiene prodotti con famiglie AIC differenti.")
            End If
        Else
            famigliaAic = tratt.FamigliaAic
        End If

        tratt.PresNumero = indTerap.presNumero

        If indTerap.famiglieAic.Any(Function(fAIC) fAIC = famigliaAic) Then
            Dim presRigaNum = indTerap.presRigaNumeri(indTerap.famiglieAic.IndexOf(indTerap.famiglieAic.FirstOrDefault(Function(fAIC) fAIC = famigliaAic)))
            If Not IsNothing(presRigaNum) Then
                tratt.PresRigaNumero = presRigaNum

                Dim gruppoRicetta As Integer = tratt.GruppoRicetta
                Dim sommsToLink = GiasContext.Ricette_Zoo.Where(Function(rz) rz.Gruppo_Ricetta = gruppoRicetta).ToList

                ' Linka i record di Agenda del Trattamento (TODO ora tutte le Somministrazioni appartenenti a quel gruppo, verificare che sia corretto)
                ' creato da Protocollo alla nuova Indicazione Terapeutica generata.
                ' Non potendo cambiare idRicetta e idRigaRicetta perche' chiavi della tabella,
                ' elimina il link tra Trattamento e Protocollo Terapeutico e lo collega all'Indicazione Terapeutica

                Dim ricetteZoo_W As New AgronicaCoreContabBIZ.Ricette_Zoo_W
                For Each rzIndTerap In sommsToLink
                    Dim ricZxAg = GiasContext.Ricette_ZooxAgenda.
                        Where(Function(rzxa) rzxa.Id_Ricetta = rzIndTerap.IdRicetta).
                        FirstOrDefault

                    If Not IsNothing(ricZxAg) Then
                        Dim idAgenda As Integer = ricZxAg.Id_Agenda

                        Dim movZoo = GiasContext.Movimenti_Zoo.Where(Function(mz) mz.Id_Agenda = idAgenda).FirstOrDefault
                        If movZoo IsNot Nothing Then
                            movZoo.Pres_Numero = indTerap.presNumero
                            movZoo.PresRiga_Numero = presRigaNum
                            movZoo.Tipo_Trattamento = tratt.Tipo
                            movZoo.Stato_Trattamento = tratt.Stato

                            GiasContext.Entry(movZoo).State = EntityState.Modified
                            GiasContext.SaveChanges()
                        End If
                    End If

                    ' TODO Ritestare dall'inizio se cosi' linka correttamente, dovrebbe linkare tutte le somm del gruppo
                    ' alla stessa ind terap quando si crea alla prima somm
                    rzIndTerap.Numero = indTerap.presNumero
                    'rzIndTerap.Pin = If(indTerap.PresPin, "")
                    ricetteZoo_W.Scrivi_Modifica(rzIndTerap, GiasContext, objP_Server)
                    GiasContext.SaveChanges()

                    Dim idRicetta As Integer = rzIndTerap.IdRicetta

                    Dim rzaIndTerap = GiasContext.Ricette_Zoo_Agenda.Where(Function(rza) rza.IdRicetta = idRicetta).FirstOrDefault
                    Dim ricetteZooAg_W As New AgronicaCoreContabBIZ.Ricette_Zoo_Agenda_W
                    rzaIndTerap.Numero = presRigaNum
                    ricetteZooAg_W.Scrivi_Modifica(rzaIndTerap, GiasContext, objP_Server)
                    GiasContext.SaveChanges()
                Next
            End If
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="trattamenti"></param>
    ''' <returns></returns>
    Private Function GetRespSincro(trattamenti As List(Of Trattamento_ToSend)) As String
        Dim r As String = JsonConvert.SerializeObject(
            trattamenti.Select(Function(tratt) New With {
                                    .idRicetta = tratt.IdRicetta,
                                    .idAgenda = tratt.IdAgenda,
                                    .error = tratt.msgErrore
                                }).ToList)

        Return r

    End Function

    ''' <summary>
    ''' Crea un'Indicazione Terapeutica (prescrizione) partendo da un Protocollo Terapeutico; 
    ''' poi crea il Trattamento a partire dall'Indicazione Terapeutica creata.
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function SendTrattamento_Protocollo(ByRef tratt As Trattamento_ToSend) As Boolean
        Dim r As Boolean = False

        Dim idProtocollo As Integer = tratt.IdProtocollo
        Dim protNumero As String = If(tratt.ProtNumero.EndsWith("T"), tratt.ProtNumero, tratt.ProtNumero & "T")

        Try
            ' Verifica se esiste un'Indicazione Terapeutica creata da un'altra Somministrazione appartenente al gruppo ricetta
            Dim respIndTerap = HandleExistingIndicazioneTerapeutica(tratt)

            Dim newIndTerap = Not respIndTerap.result
            If newIndTerap Then
                ' Crea l'Indicazione Terapeutica partendo dal Protocollo Terapeutico
                Dim listCapiProtocollo As List(Of String) = tratt.Animali.Select(Function(capo) capo.Identificativo).Distinct.ToList
                respIndTerap = ws.Insert_Trattamento_Protocollo_Terapeutico(tratt.Piva, tratt.SaCod, tratt.StaNum, tratt.IdAgenda, protNumero, listCapiProtocollo)
            End If

            If respIndTerap.result Then
                ' Indicazione Terapeutica trovata o generata correttamente, procede a creare il Trattamento da essa

                Dim indTerapNumero As String = respIndTerap.presNumero
                'Dim indTerapNumeriRiga As List(Of String) = respIndTerap.presrigaNumeri
                'Dim indTerapFamiglieAic As List(Of String) = respIndTerap.famiglieAic

                ' Legge da VetInfo l'Indicazione Terapeutica appena generata dal Protocollo 
                'Dim indTerap As Zoo.Prescrizione = Read_PrescrizioneVetInfo(tratt.AziendaCodice, tratt.PropIdFiscale, indTerapNumero)

                If indTerapNumero = "" Then
                    tratt.msgErrore = "Errore nella creazione del Trattamento da Protocollo."
                    Throw New Exception("Errore nella creazione dell'Indicazione Terapeutica da Protocollo.")
                End If

                ' Collega il Trattamento all'Indicazione Terapeutica creata (precedentemente il Trattamento e' collegato al Protocollo Terapeutico)
                ' se non era ancora stata collegata
                If newIndTerap Then LinkTrattamentoToIndicazioneTerapeutica(respIndTerap, tratt)

                ' Se ci sono delle Somministrazioni senza riga di Prescrizione associata, il Trattamento non può essere chiuso
                ' poiche' quelle Somministrazioni non possono essere inviate
                If String.IsNullOrEmpty(tratt.PresRigaNumero) Then
                    tratt.msgErrore = $"Somministrazione non contenuta nel Protocollo Terapeutico di partenza."
                    DoNotCloseTrattamento(tratt)
                End If

                ' Valorizza la Scorta dell'Indicazione Terapeutica e delle Somministrazioni del Trattamento
                Dim hasScorta = True
                If tratt.MultiAIC Then
                    If tratt.Prodotti.Any(Function(p) String.IsNullOrEmpty(p.RegScoNumero)) Then hasScorta = SetScortaTrattamentoDaProtocollo(respIndTerap, tratt)
                Else
                    If String.IsNullOrEmpty(tratt.RegScoNumero) Then hasScorta = SetScortaTrattamentoDaProtocollo(respIndTerap, tratt)
                End If

                ' Se ci sono delle Somministrazioni senza riga di Prescrizione associata, il Trattamento non può essere chiuso
                ' poiche' quelle Somministrazioni non possono essere inviate
                If Not hasScorta Then
                    If tratt.MultiAIC Then
                        tratt.msgErrore = $"Non è presente il Registro di Scorta del Prodotto ({String.Join(", ", tratt.Prodotti.Select(Function(p) p.Pro_Des))})"
                    Else
                        tratt.msgErrore = $"Non è presente il Registro di Scorta del Prodotto."
                    End If

                    Throw New Exception(tratt.msgErrore)
                End If

                r = SendSomministrazione_InTrattamento(tratt)

            Else
                tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), $"Errore generazione Indicazione Terapeutica da Protocollo: {respIndTerap.errore}", tratt.msgErrore)
                DoNotCloseTrattamento(tratt)
                Throw New Exception($"Errore generazione Indicazione Terapeutica da Protocollo {protNumero} : {respIndTerap.errore}")
            End If

        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              ex.Message, CustomLOGParams:=customLOGParams)
            tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), $"Errore generazione Indicazione Terapeutica da Protocollo: {ex.Message}", tratt.msgErrore)
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Al momento non utilizzata.
    ''' </summary>
    ''' <param name="pres"></param>
    ''' <returns></returns>
    Private Function SendTrattamento_Semplificato(ByRef pres As Trattamento_ToSend)
        'Dim idRicetta As Integer = pres.IdRicetta

        '' TODO Ricavare categoria classy farm

        'Dim catClassyFarm = "BOVINI"

        'For Each riga In pres.Somministrazioni
        '	Dim resp = ws.Insert_Trattamento_Semplificato(riga.RegScoNumero, riga.Quantitativo, pres.Specie, catClassyFarm, pres.Note)

        '	Dim st = 0
        'Next

    End Function

    Private Function SendTrattamento_Protocollo_Semplificato(ByRef tratt As Trattamento_ToSend)
        Dim r As Boolean = False

        Dim idProtocollo As Integer = tratt.IdProtocollo
        Dim protNumero As String = If(tratt.ProtNumero.EndsWith("T"), tratt.ProtNumero, tratt.ProtNumero & "T")

        Try

            Dim hasScorta = False
            If String.IsNullOrEmpty(tratt.RegScoNumero) Then
                Dim listaPresRiga As New List(Of String)
                listaPresRiga.Add(protNumero)
                Dim filter As New List(Of VetInfoModel.VetInfoFilter)
                filter.Add(New VetInfoFilter With {
                           .field = "regscoProdottoAic",
                           .op = "EQUALS",
                           .value1 = tratt.Aic
                })
                Dim resp = ws.RegistroDisponibilitaMedicinali(tratt.AziendaCodice, tratt.PropIdFiscale, filter)
                If resp IsNot Nothing AndAlso resp.Count > 0 Then
                    tratt.RegScoNumero = resp(0).regscoNumero
                    hasScorta = True
                End If
            End If

            ' Se ci sono delle Somministrazioni senza riga di Prescrizione associata, il Trattamento non può essere chiuso
            ' poiche' quelle Somministrazioni non possono essere inviate
            If Not hasScorta Then
                tratt.msgErrore = $"Non è presente il Registro di Scorta del Prodotto."
                Throw New Exception("Non è presente il Registro di Scorta del Prodotto.")
            End If

            Dim prescapoLista As New List(Of presCapo)
            tratt.Animali.ForEach(Sub(capo)
                                      prescapoLista.Add(New presCapo(capo.Identificativo) With {
                                          .prescapoSesso = capo.Sesso
                                      })
                                  End Sub)
            Dim regSco_Scarico = New regSco_Scarico(tratt.RegScoNumero, tratt.Quantitativo)
            Dim objTratt As New InsertTrattamento_Protocollo_Semplificato(protNumero, tratt.DataInizio, tratt.DataFine, prescapoLista, regSco_Scarico, "")

            ws.Insert_Trattamento_Protocollo_Semplificato(objTratt)

        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              ex.Message, CustomLOGParams:=customLOGParams)
            tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), $"Errore generazione Indicazione Terapeutica da Protocollo: {ex.Message}", tratt.msgErrore)
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Al momento non utilizzata.
    ''' Il trattamento da protocollo deve avere una sola somministrazione
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function SendTrattamento_ProtocolloSemplificato(ByRef tratt As Trattamento_ToSend)
        'If tratt.Somministrazioni.Count > 1 Then Throw New Exception($"Il trattamento da protocollo semplificato deve avere una sola somministrazione.")
        'Dim rigaSomm = tratt.Somministrazioni.First

        'Dim protNumero As String = If(tratt.PresNumero.EndsWith("T"), tratt.PresNumero, tratt.PresNumero & "T")
        'Dim resp = ws.Insert_Trattamento_Protocollo_Semplificato(protNumero, rigaSomm.DataInizio, rigaSomm.DataFine,
        '														 rigaSomm.Animali.Select(Function(capo) capo.Identificativo).ToList,
        '														 New Tuple(Of String, Double)(rigaSomm.RegScoNumero, rigaSomm.Quantitativo),
        '														 tratt.Note)

        'Dim st = 0

    End Function

    ''' <summary>
    ''' Al momento non utilizzata.
    ''' </summary>
    ''' <param name="pres"></param>
    ''' <returns></returns>
    Private Function SendTrattamento_Massivo(ByRef pres As Trattamento_ToSend)
        'For Each riga In pres.Somministrazioni
        '	Dim respMassivo = ws.Insert_Trattamenti_Massivo(riga.PresRigaNumero, riga.DataInizio, riga.DataFine,
        '													{New Tuple(Of String, Double)(riga.RegScoNumero, riga.Quantitativo)}.ToList)

        '	Dim st = 0
        'Next

    End Function

    ''' <summary>
    ''' Genera o modifica le Somministrazioni di un Trattamento 
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function SendSomministrazione_InTrattamento(ByRef tratt As Trattamento_ToSend) As Boolean
        Dim r As Boolean = False

        Dim idRicetta As Integer = tratt.IdRicetta

        ' Se ci sono delle Somministrazioni senza Registro di Scarico associato, il Trattamento non può essere chiuso
        ' poiche' quelle Somministrazioni non possono essere inviate
        If tratt.MultiAIC Then
            If tratt.Prodotti.Any(Function(p) String.IsNullOrEmpty(p.RegScoNumero)) Then
                tratt.msgErrore = $"Il Prodotto {tratt.Prodotti.First(Function(p) String.IsNullOrEmpty(p.RegScoNumero)).Pro_Des} della Somministrazione non ha uno Scarico associato."
                DoNotCloseTrattamento(tratt)
                Return False
            End If
        Else
            If String.IsNullOrEmpty(tratt.RegScoNumero) Then
                Dim scortaTrovata = False
                Dim scortaProd_daPrescrizione = ws.Leggi_ScortaProdotto_perSomministrazione({tratt.PresRigaNumero}.ToList)
                If scortaProd_daPrescrizione IsNot Nothing AndAlso scortaProd_daPrescrizione.Count > 0 Then
                    Dim id_agenda = tratt.IdAgenda
                    Dim movimento = (From m In GiasContext.Movimenti Where m.Id_Agenda = id_agenda And m.Cau_Mov = CAU_TRATTAMENTO_ZOO).FirstOrDefault
                    If movimento IsNot Nothing Then
                        Dim movDett = GiasContext.Movimenti_dettagli.Where(Function(md) md.Id_Agenda = id_agenda And md.Id_Mov = movimento.Id_Mov).FirstOrDefault
                        If movDett IsNot Nothing Then
                            tratt.RegScoNumero = scortaProd_daPrescrizione.First.regscoNumero
                            movDett.RegSco_Numero = scortaProd_daPrescrizione.First.regscoNumero
                            GiasContext.Entry(movDett).State = EntityState.Modified
                            GiasContext.SaveChanges()
                            tratt.RegScoNumero = scortaProd_daPrescrizione.First.regscoNumero
                            scortaTrovata = True
                        End If
                    End If
                End If
                If Not scortaTrovata Then
                    tratt.msgErrore = $"Il Prodotto della Somministrazione non ha uno Scarico associato."
                    DoNotCloseTrattamento(tratt)
                    Return False
                End If
            End If
        End If

        ' Verifica se il Trattamento e' gia' stato chiuso
        Dim isClosed As Boolean = False
        If tratt.IsSync Then isClosed = IsTrattamentoClosed(tratt)
        If isClosed Then Return False

        Try
            Dim idAgenda As Integer = tratt.IdAgenda

            ' Se non e' gia' presente, inserisce la nuova Somministrazione nel Trattamento (se e' la prima ne crea uno nuovo)
            If Not tratt.IsSync Then
                Dim respInsSomm As InsertTrattamento_Response

                Dim listProds = If(tratt.MultiAIC, tratt.Prodotti.Select(Function(p) New Tuple(Of String, Double)(p.RegScoNumero, p.Quantitativo)).ToList, {New Tuple(Of String, Double)(tratt.RegScoNumero, tratt.Quantitativo)}.ToList)
                If tratt.GenereAnimaliInTratt = enum_GeneriAnimale.Suino Then
                    respInsSomm = ws.Insert_Somministrazione_In_Trattamento(tratt.Piva, tratt.SaCod, tratt.StaNum, tratt.IdAgenda,
                                                                            tratt.PresRigaNumero, tratt.DataInizio,
                                                                            listProds, tratt.Note,
                                                                            tratt.AnimaliToDelete.Select(Function(c) New presCapoRiduzione(c.Identificativo, c.NumeroCapi)).ToList)
                Else
                    respInsSomm = ws.Insert_Somministrazione_In_Trattamento(tratt.Piva, tratt.SaCod, tratt.StaNum, tratt.IdAgenda,
                                                                            tratt.PresRigaNumero, tratt.DataInizio,
                                                                            listProds, tratt.Note,
                                                                            tratt.AnimaliToDelete.Select(Function(c) New presCapoRiduzione(c.Identificativo)).ToList)
                End If

                If respInsSomm.result Then
                    Dim trattNumero As String = respInsSomm.trattNumero
                    Dim sommNumero As String = respInsSomm.sommNumero

                    ' Verifica che il Trattamento sia stato inserito
                    Dim trattResp = ws.Read_Trattamento(tratt.AziendaCodice, tratt.PropIdFiscale, trattNumero)

                    If IsNothing(trattResp) Then
                        tratt.msgErrore = $"La Prescrizione {tratt.PresNumero} (Riga {tratt.PresRigaNumero}) non ha generato alcun Trattamento."
                        Throw New Exception($"La Prescrizione {tratt.PresNumero} (Riga {tratt.PresRigaNumero}) non ha generato nessun Trattamento.")
                    End If

                    Dim movDett_W As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
                    Dim listDettScaricoAnimali = (From mdett In GiasContext.Movimenti_dettagli
                                                  Join mov In GiasContext.Movimenti On mov.Id_Agenda Equals mdett.Id_Agenda And mov.Id_Mov Equals mdett.Id_Mov
                                                  Join movz In GiasContext.Movimenti_Zoo On movz.Id_Agenda Equals mov.Id_Agenda And movz.Id_Mov Equals mov.Id_Mov
                                                  Where mov.Cau_Mov = CAU_TRATTAMENTO_ZOO And mov.Id_Agenda = idAgenda
                                                  Select mdett).ToList

                    ' Aggiorna i dettagli degli scarichi del farmaco sull'animale 
                    For Each dettaglio In listDettScaricoAnimali
                        dettaglio.Extra_Str = sommNumero
                        dettaglio.Rif_Esterno = trattNumero

                        Dim idDett = movDett_W.Scrivi_Modifica(dettaglio, GiasContext, objP_Server)
                    Next
                    GiasContext.SaveChanges()

                    Dim movZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
                    Dim movimentiZoo = (From mzoo In GiasContext.Movimenti_Zoo
                                        Join mov In GiasContext.Movimenti On mov.Id_Agenda Equals mzoo.Id_Agenda And mov.Id_Mov Equals mzoo.Id_Mov
                                        Where mov.Cau_Mov = CAU_TRATTAMENTO_ZOO And mov.Id_Agenda = idAgenda
                                        Select mzoo).FirstOrDefault

                    movimentiZoo.Stato_Trattamento = enum_StatoTrattamento.InCorso
                    movZoo_W.Scrivi_Modifica(movimentiZoo, GiasContext, objP_Server)
                    GiasContext.SaveChanges()

                    tratt.TrattNumero = trattNumero
                    tratt.SommNumero = sommNumero

                    r = True
                Else
                    tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore),
                        $"Errore inserimento Somministrazione da Prescrizione: {respInsSomm.errore}", tratt.msgErrore)

                    ' Se non tutte le Somministrazioni sono state inserite il Trattamento non puo' essere chiuso	
                    tratt.ToClose = False

                    Throw New Exception($"Errore inserimento Somministrazione da Prescrizione {tratt.PresNumero} - Riga Prescrizione {tratt.PresRigaNumero}: {respInsSomm.errore}")
                End If

            Else
                ' TODO Modifica della somministrazione per Trattamento aperto

            End If

        Catch ex As BDNException
            Throw ex
        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name, ex.Message, CustomLOGParams:=customLOGParams)
        End Try

        Return r

    End Function

    ''' <summary>
    ''' Chiude i Trattamenti e le relative Somministrazioni
    ''' </summary>
    ''' <param name="tratt"></param>
    ''' <returns></returns>
    Private Function CloseTrattamento(ByRef tratt As Trattamento_ToSend) As Boolean
        Dim r As Boolean = False

        If tratt.IsClosed Then Return True

        If tratt.ToClose Then
            Dim aziendaCodice As String = tratt.AziendaCodice
            Dim propIdFiscale As String = tratt.PropIdFiscale

            If IsTrattamentoClosed(tratt) Then
                ' Il Trattamento potrebbe essere già stato chiuso
                tratt.IsClosed = True
            Else
                Try
                    Dim presNumero As String = tratt.PresNumero
                    Dim trattNumero As String = tratt.TrattNumero

                    'If tratt.DataFine.Date > Date.Now.Date Then
                    '	Dim msg = "La data di chiusura è maggiore della data odierna, non è possibile chiudere il Trattamento."
                    '	tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), msg, tratt.msgErrore)
                    '	Throw New Exception(msg)
                    'End If

                    ' Verifica che il Trattamento esista su VetInfo
                    Dim trattResp = ws.Read_Trattamento(aziendaCodice, propIdFiscale, trattNumero)

                    If IsNothing(trattResp) Then
                        tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), "Il Trattamento non è presente su VetInfo.", tratt.msgErrore)
                        Throw New Exception($"Il Trattamento {trattNumero} non è presente su VetInfo.")
                    End If

                    Dim trattFine = (tratt.DataFine).ToString("dd-MM-yyyy")
                    Dim trattStato = "CHIUSO" ' TODO Verificare se c'e' la necessita' di casi in cui chiudere con CHIUSO_CON_ANOMALIA (obbligatorio tratNote in questo caso)
                    Dim trattNote = If(IsNothing(trattResp.tratNote), "", trattResp.tratNote)

                    Dim respClose As CloseTrattamento_Response
                    If {enum_TipoPrescrizione.Da_Protocollo, enum_TipoPrescrizione.Protocollo_Terapeutico, enum_TipoPrescrizione.Da_Protocollo_GIAS}.Contains(tratt.Tipo) Then
                        ' Chiude i Trattamenti generati a partire dal Protocollo Terapeutico
                        respClose = ws.Close_Trattamento_Protocollo_Terapeutico(tratt.Piva, tratt.SaCod, tratt.StaNum, tratt.IdAgenda, presNumero, trattFine, trattStato, trattNote)
                    Else
                        ' Chiude tutte le Somministrazioni legate al Trattamento appena chiuso 
                        respClose = ws.Close_Trattamento_Prescrizione(tratt.Piva, tratt.SaCod, tratt.StaNum, tratt.IdAgenda, trattNumero, trattFine, trattStato, trattNote)
                    End If

                    If respClose.result Then
                        tratt.IsClosed = True
                    Else
                        Dim msg = $"Errore chiusura Trattamento: {respClose.errore}"
                        tratt.msgErrore = If(String.IsNullOrEmpty(tratt.msgErrore), msg, tratt.msgErrore)
                        Throw New Exception(msg)
                    End If

                Catch ex As Exception
                    objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                                      ex.Message, CustomLOGParams:=customLOGParams)
                End Try
            End If

            If tratt.IsClosed Then
                ' TODO Come gestire la chiusura? Serve salvare tutti i movimenti_zoo come chiusi per ogni somm o solo l'ultima?
                ' Nel primo caso, come comprendo in che situazione mi trovo:
                ' - ho inviato tutte le somministrazioni o solo una parte?
                ' - ho chiuso una somministrazione prima di inviare le successive?

                If tratt.GruppoRicetta <> 0 Then
                    Dim gruppoRicetta As Integer = tratt.GruppoRicetta
                    Dim idRzFromGruppo = GiasContext.Ricette_Zoo.
                        Where(Function(row) row.Gruppo_Ricetta = gruppoRicetta).
                        Select(Function(row) row.IdRicetta).ToList
                    Dim idAgendaFromGruppo = GiasContext.Ricette_ZooxAgenda.
                        Where(Function(row) idRzFromGruppo.Contains(row.Id_Ricetta)).
                        Select(Function(row) row.Id_Agenda).ToList

                    For Each idAgenda In idAgendaFromGruppo
                        Dim movZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
                        Dim movsZooFromAgenda = GiasContext.Movimenti_Zoo.
                            Where(Function(movz) movz.Id_Agenda = idAgenda).ToList
                        For Each movZoo In movsZooFromAgenda
                            movZoo.Stato_Trattamento = enum_StatoTrattamento.Chiuso
                            movZoo_W.Scrivi_Modifica(movZoo, GiasContext, objP_Server)
                        Next

                        Dim agenda_W As New AgronicaCoreContabBIZ.Agenda_W
                        Dim agenda = GiasContext.Agenda.Where(Function(a) a.Id_Agenda = idAgenda).FirstOrDefault
                        agenda.Blocco_Flag = 1
                        agenda.Blocco_Data = Date.Now
                        agenda.Blocco_Username = objP_Server.UsernameOperazione
                        agenda_W.Scrivi_Modifica(agenda, GiasContext, objP_Server)
                    Next

                Else
                    Dim idAgenda As Integer = tratt.IdAgenda
                    Dim movZoo_W As New AgronicaCoreContabBIZ.Movimenti_Zoo_W
                    Dim listMovZoo = GiasContext.Movimenti_Zoo.Where(Function(movz) movz.Id_Agenda = idAgenda).ToList

                    For Each movZoo In listMovZoo
                        movZoo.Stato_Trattamento = enum_StatoTrattamento.Chiuso
                        movZoo_W.Scrivi_Modifica(movZoo, GiasContext, objP_Server)
                    Next

                    Dim agenda_W As New AgronicaCoreContabBIZ.Agenda_W
                    Dim agenda = GiasContext.Agenda.Where(Function(a) a.Id_Agenda = idAgenda).FirstOrDefault
                    agenda.Blocco_Flag = 1
                    agenda.Blocco_Data = Date.Now
                    agenda.Blocco_Username = objP_Server.UsernameOperazione
                    agenda_W.Scrivi_Modifica(agenda, GiasContext, objP_Server)

                End If

                GiasContext.SaveChanges()

                r = True
            End If
        End If

        Return r

    End Function
    Private Sub SetGruppoForCapo(ByRef trattGroup As List(Of Trattamento_ToSend))
        Dim tratt = trattGroup.First

        Dim giacenzeTratt = zooAnimali_R.Leggi_Giacenze(
            tratt.Piva, tratt.SaCod, tratt.StaNum, 0,
            0, tratt.DataInizio, objP_Server,
            listMatricola_Animali:=tratt.Animali.Select(Function(c) c.Identificativo).ToList
        )

        trattGroup.
            ForEach(Sub(tt) tt.Animali.ForEach(Sub(capo)
                                                   Dim giacenzaCapo = giacenzeTratt.AsEnumerable.Where(Function(g) g("Matricola") = capo.Identificativo).FirstOrDefault
                                                   If giacenzaCapo IsNot Nothing Then capo.Gruppo = giacenzaCapo("Raggruppamento_Des")
                                               End Sub)
                                               )
    End Sub
    Private Sub CheckAnimalsFromPreviousSomm(ByRef somm As Trattamento_ToSend)
        If String.IsNullOrEmpty(somm.PresRigaNumero) Then Return
        Dim capiInPres = ws.ElencoAnimali_ProdottoPres(somm.PresRigaNumero, True)
        If IsNothing(capiInPres) OrElse capiInPres.Count = 0 Then Return

        If somm.GenereAnimaliInTratt = enum_GeneriAnimale.Suino Then
            'Dim groupedAnimals = somm.Animali.
            '	GroupBy(Function(x) New With {
            '		Key .Grp = If(x.Gruppo, "").Trim().ToUpper(),
            '		Key .Sex = If(x.Sesso, "").Trim().ToUpper()
            '	}).
            '	Select(Function(g) New With {
            '		.Gruppo = g.Key.Grp,
            '		.Sesso = g.Key.Sex,
            '		.TotaleCapi = g.Sum(Function(x) x.NumeroCapi)
            '	}).ToList

            'For Each animalGroup In groupedAnimals
            '	Dim matchByGroup = capiInPres.FirstOrDefault(Function(a) a.prescapoIdentificativo = animalGroup.Gruppo)
            '	Dim matchByGroupAndSex = capiInPres.FirstOrDefault(Function(a) a.prescapoIdentificativo = animalGroup.Gruppo AndAlso a.prescapoSesso = animalGroup.Sesso)

            '	If matchByGroup Is Nothing Then
            '		somm.AnimaliToDelete.Add(New Animali_ToDelete(animalGroup.Gruppo))
            '	Else
            '		If matchByGroupAndSex Is Nothing Then
            '			' TODO 

            '		Else
            '			Dim numCapi As Integer = CInt(matchByGroup.prescapoNumeroAnimali)
            '			If numCapi <> animalGroup.TotaleCapi Then
            '				If somm.AnimaliToDelete.Where(Function(atd) atd.Identificativo = animalGroup.Gruppo).Count > 0 Then
            '					somm.AnimaliToDelete.FirstOrDefault(Function(atd) atd.Identificativo = animalGroup.Gruppo).NumeroCapi += animalGroup.TotaleCapi
            '				Else
            '					somm.AnimaliToDelete.Add(New Animali_ToDelete(animalGroup.Gruppo, animalGroup.TotaleCapi))
            '				End If
            '			End If
            '		End If

            '	End If
            'Next
        Else
            Dim animalsSomm = somm.Animali.Select(Function(c) c.Identificativo).ToList
            somm.AnimaliToDelete = capiInPres.
                Where(Function(capo) Not animalsSomm.Contains(capo.prescapoIdentificativo)).
                Select(Function(capo) New Animali_ToDelete(capo.prescapoNumero)).
                ToList
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="listTrattamenti"></param>
    ''' <returns></returns>
    Public Function ScriviModifica_Trattamenti_VetInfo(ByVal listTrattamenti As List(Of Trattamento_ToSend))
        Dim listTrattamentiProtocolli = listTrattamenti.Where(Function(x) x.Tipo = enum_TipoPrescrizione.Protocollo_Terapeutico Or x.Tipo = enum_TipoPrescrizione.Da_Protocollo_GIAS).ToList()

        Dim groupedSomministrazioni = listTrattamenti.
            OrderBy(Function(s) s.DataInizio).
            GroupBy(Function(s) If(s.Tipo = enum_TipoPrescrizione.Da_Protocollo_GIAS, s.GruppoRicetta, s.PresNumero))

        For Each group In groupedSomministrazioni

            Select Case group.First.GenereAnimaliInTratt
                Case enum_GeneriAnimale.Suino
                    SetGruppoForCapo(group.ToList)

                Case Else

            End Select

            For Each somm In group
                UpdateNoteTrattamento(somm)

                If Not somm.IsSync Then
                    Select Case somm.Tipo
                        Case enum_TipoPrescrizione.Veterinaria, enum_TipoPrescrizione.Da_Protocollo, enum_TipoPrescrizione.Indicazione_Terapeutica
                            somm.IsSync = SendSomministrazione_InTrattamento(somm)

                        Case enum_TipoPrescrizione.Protocollo_Terapeutico, enum_TipoPrescrizione.Da_Protocollo_GIAS
                            CheckAnimalsFromPreviousSomm(somm)

                            somm.IsSync = SendTrattamento_Protocollo(somm)

                            If Not somm.IsSync Then
                                group.ToList.
                                    Where(Function(s) s.IdAgenda <> somm.IdAgenda AndAlso s.DataInizio > somm.DataInizio).ToList.
                                    ForEach(Sub(s) s.msgErrore = $"Errore nella creazione della Somministrazione a partire dalla prima o una precedente del gruppo: {somm.msgErrore}")
                                Exit For
                            Else
                                group.ToList.ForEach(Sub(s)
                                                         s.PresNumero = somm.PresNumero
                                                         s.PresRigaNumero = somm.PresRigaNumero
                                                     End Sub)
                            End If

                        Case Else
                            'SendTrattamento_Protocollo(tratt)

                            'SendTrattamento_Semplificato(tratt)

                            'SendTrattamento_ProtocolloSemplificato(tratt)

                            'SendTrattamento_Massivo(tratt)

                            'SendSomministrazione_InTrattamento(tratt)

                    End Select
                End If

                If somm.IsSync AndAlso somm.ToClose Then Dim closed = CloseTrattamento(somm)
            Next
        Next

        Dim r As New RispostaStandard With {
            .RispostaStringa = GetRespSincro(listTrattamenti),
            .RispostaOK = True
        }

        Return r

    End Function

#End Region

#Region "Giacenze Prodotti VetInfo"

    Public Function LeggiGiacenzeProdottiVetInfo(ByVal AziendaCodice As String,
                                                 ByVal PropIdFiscale As String,
                                                 ByRef objP_Server As AgronicaCoreParametri) As List(Of DisponibilitaMedicinale)
        Dim listaGiacenzeFarmaci = ws.RegistroDisponibilitaMedicinali(AziendaCodice, PropIdFiscale)

        If listaGiacenzeFarmaci Is Nothing OrElse listaGiacenzeFarmaci.Count = 0 Then
            objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                              "Non sono presenti giacenze.", CustomLOGParams:=customLOGParams)
            Return New List(Of DisponibilitaMedicinale)
        End If

        objLog.Scrivi_LOG(objP_Server, MethodBase.GetCurrentMethod().Name,
                          "Trovate n." & listaGiacenzeFarmaci.Count & " giacenze.",
                          CustomLOGParams:=customLOGParams)

        Return listaGiacenzeFarmaci

    End Function

#End Region

    Private Function ConfrontoOperazioneDBOperazioneWS(somministrazione As RegistroSomministrazioni_Response,
                                                       lista_ElencoScarichiSomm As List(Of ScarichiPerSomministrazione_Response),
                                                       Id_Agenda As Integer, Piva As String, Sa_Cod As Integer, Sta_Num As Integer) As Boolean
        Dim uguali As Boolean = True
        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objP_Server.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim lista_ElencoCapiSomm As List(Of CapiPerSomministrazione_Response) = ws.ElencoAnimali_Somministrazione(somministrazione.somNumero)
        Dim lista_Farmaci = GetFarmaciFromID_Agenda(Id_Agenda, GiasContext)
        Dim lista_CapiAnimali = GetCapiFromID_Agenda(Id_Agenda, GiasContext)

        For Each FarmacoWS In lista_ElencoScarichiSomm
            Dim trovato As Boolean = False
            For Each farmacoDB In lista_Farmaci
                If FarmacoWS.regscoProdottoAic = farmacoDB.AIC Then
                    trovato = True
                    If farmacoDB.Qta <> FarmacoWS.regscoQuantitativo Then
                        Return False
                    End If
                End If
            Next
            If Not trovato Then
                Return False
            End If
        Next

        For Each CapoAnimaleWS In lista_ElencoCapiSomm
            Dim trovato As Boolean = False
            For Each CapoAnimaleDB In lista_CapiAnimali
                If CapoAnimaleDB.Matricola = CapoAnimaleWS.prescapoIdentificativo Then
                    trovato = True
                End If
            Next
            If Not trovato Then
                Dim dataSomministrazione = CDate(somministrazione.somDtSomministrazione)
                Dim obj_ZooAnimali_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali
                Dim dtGiacenzeStalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
                                                                                                      0, 0, dataSomministrazione,
                                                                                                      objP_Server)
                If dtGiacenzeStalla.Rows.Count > 0 Then
                    Dim drAnimale = dtGiacenzeStalla.Select(" Matricola = '" & CapoAnimaleWS.prescapoIdentificativo & "' ")
                    If drAnimale.Length > 0 Then
                        Throw New Exception("ESISTE ANIMALE")
                    End If
                End If
            End If
        Next


        Return uguali
    End Function

    Private Function GetCapiFromID_Agenda(Id_Agenda As Integer, GiasContext As Gias_DeveloperServer_Entities) As List(Of Capi_OpAgenda)
        Dim list As New List(Of Capi_OpAgenda)
        Dim Mov_Destinazioni_List = (From a In GiasContext.Mov_Destinazioni Where a.Tipo_Destinazione = 1 And a.Id_Agenda = Id_Agenda).ToList
        For Each dest In Mov_Destinazioni_List
            Dim objAnimale As New Capi_OpAgenda With {
                .Matricola = dest.mov_destinazioni_graphickey,
                .Cod_Animale = dest.Id_Destinazione
            }
            list.Add(objAnimale)
        Next
        Return list
    End Function

    Private Function GetFarmaciFromID_Agenda(Id_Agenda As Integer, GiasContext As Gias_DeveloperServer_Entities) As List(Of Farmaci_OpAgenda)
        Dim list As New List(Of Farmaci_OpAgenda)
        Dim Mov_Destinazioni_List = (From a In GiasContext.Movimenti_dettagli
                                     Join f In GiasContext.Farmaci On a.Pro_Cod Equals f.Farm_Cod
                                     Where a.Elem_Cod = CostantiPersonalizzate.FARMACI And
                                         a.Id_Agenda = Id_Agenda And
                                         a.Rif_Esterno <> ""
                                     Select a.Qta_Extra_Totale, f.AIC).ToList
        For Each det In Mov_Destinazioni_List
            Dim objFarmaco As New Farmaci_OpAgenda With {
                .AIC = det.AIC,
                .Qta = det.Qta_Extra_Totale
            }
            list.Add(objFarmaco)
        Next
        Return list
    End Function

    Private Sub AggiornaVisibilitaUtentixFabbricati(Username As String, Piva As String, Sa_Cod As Integer, Fabbricato_Cod As Integer, objparametri_Server As AgronicaCoreParametri)
        Try
            Dim objUtentixFabbricati_R As New AgronicaCoreAnagrafeDAL.UtentixFabbricati_R
            Dim objUtentixFabbricati_W As New AgronicaCoreAnagrafeDAL.UtentixFabbricati_W

            Dim dt = objUtentixFabbricati_R.Leggi(Username, Piva, Sa_Cod, Fabbricato_Cod, "", "", objparametri_Server)
            If dt.Rows.Count = 0 Then
                objUtentixFabbricati_W.Scrivi(Username, Piva, Sa_Cod, Fabbricato_Cod, AGRODATAINIZIO, AGRODATAFINE, objparametri_Server)
            End If
        Catch ex As Exception
            objLog.Scrivi_LOG(objP_Server,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore in AggiornaVisibilitaUtentixFabbricati: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
        End Try

    End Sub

    ''' <summary>
    ''' Lettura somministrazioni su DB effettuate per cancellare
    ''' le somministrazioni non all'interno dell'intervallo temporale scelto
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="DataInizio"></param>
    ''' <param name="DataFine"></param>
    Private Sub Cancella_OperazioniTrattamenti(ByVal Piva As String,
                                               ByVal DataInizio As Date,
                                               ByVal DataFine As Date)

        Try
            'lettura operazioni agenda e movimenti
            Dim agenda_delete = (From a In GiasContext.Agenda
                                 Select a Where a.Validita_Inizio < DataInizio And
                                              a.Validita_Fine > DataFine And
                                              a.PIVA = Piva And
                                              a.Lav_Cod = LAVCOD_CUREMEDICAMENTI_ANIMALI)

            If Not IsNothing(agenda_delete) AndAlso agenda_delete.Count > 0 Then
                Dim listaIdAgenda_Delete As List(Of Integer) = (From somm In agenda_delete
                                                                Select somm.Id_Agenda).ToList()

                Dim movimenti_delete = (From m In GiasContext.Movimenti
                                        Select m Where m.PIVA = Piva And
                                                     listaIdAgenda_Delete.Contains(m.Id_Agenda))

                Dim listaIdMov_Delete As List(Of Integer) = (From somm In movimenti_delete
                                                             Select somm.Id_Mov).ToList()

                Dim movDettagli_delete = (From md In GiasContext.Movimenti_dettagli
                                          Select md Where md.PIVA = Piva And
                                                        listaIdAgenda_Delete.Contains(md.Id_Agenda) And
                                                        listaIdMov_Delete.Contains(md.Id_Mov))
                Dim movDestinazioni_delete = (From md In GiasContext.Mov_Destinazioni
                                              Select md Where md.Piva = Piva And
                                                        listaIdAgenda_Delete.Contains(md.Id_Agenda) And
                                                        listaIdMov_Delete.Contains(md.Id_Mov))
                Dim movDettTecnExtra_delete = (From md In GiasContext.Mov_Dettaglio_Tecnico_Extra
                                               Select md Where md.Piva = Piva And
                                                        listaIdAgenda_Delete.Contains(md.Id_Agenda) And
                                                        listaIdMov_Delete.Contains(md.Id_Mov))

                'cancellazione
                GiasContext.Entry(movDettagli_delete).State = EntityState.Deleted
                GiasContext.Entry(movDestinazioni_delete).State = EntityState.Deleted
                GiasContext.Entry(movDettTecnExtra_delete).State = EntityState.Deleted
                GiasContext.Entry(movimenti_delete).State = EntityState.Deleted
                GiasContext.Entry(agenda_delete).State = EntityState.Deleted
            End If

            GiasContext.SaveChanges()

        Catch ex As Exception
            Throw New Exception("SincroREV.Cancella_OperazioniTrattamenti() -> " & ex.Message)
        End Try

    End Sub

    ''' <summary>
    ''' Scrittura log per inizio/fine sincronizzazione VetInfo (trattamento)
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="staNum"></param>
    ''' <param name="idLog">se 0 in creazione, se diverso da 0 in modifica</param>
    ''' <param name="msg">valorizzato se log di errore</param>
    ''' <param name="datiSincronizzati">Tipo di dati sincronizzati e relativo numero di sincronizzati/eliminati</param>
    ''' <returns></returns>
    Private Function logSincroStalla(ByVal piva As String,
                                     ByVal saCod As Integer,
                                     ByVal staNum As Integer,
                                     ByVal idLog As Integer,
                                     ByVal msg As String,
                                     ByVal datiSincronizzati As Sincronizza_REV_VetInfo_Response) As Integer
        Dim objLogInvioChiamate_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAnagrafe_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Dim chiaveStalla As String = piva & "_" & saCod & "_" & staNum
        Dim causaleCod As Integer = 0 'Causale_Cod per importazione sincro VetInfo

        Try
            If idLog = 0 Then
                'AGRONICA LOG INVIO CHIAMATE
                Dim LogInvioChiamata = objLogInvioChiamate_W.Create_Agronica_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                                "", DateTime.Now, "",
                                                                                                "", 0, 0,
                                                                                                objP_Server, GiasContext)
                idLog = LogInvioChiamata.ID
                'AGRONICA LOG INVIO ANAGRAFE
                Dim LogInvioAnagrafe As Boolean = objLogInvioAnagrafe_W.Scrivi(enum_Esportazioni_Sistema_Cod.BDN, idLog,
                                                                               -1, "SincroVetInfo", chiaveStalla, piva, saCod,
                                                                               0, 0, staNum, "Sincronizzazione VetInfo iniziata",
                                                                               LogInvioChiamata.Data_Invio, objP_Server,
                                                                               Causale_Cod:=causaleCod, Fabbricato_Cod:=staNum)
            Else
                Dim esito As String = ""
                Dim note As String = ""
                Dim datiRicevuti As String = ""

                If msg <> "" Then
                    esito = "Non eseguita"
                    note = msg
                Else
                    If datiSincronizzati.trattamenti IsNot Nothing Then
                        If datiSincronizzati.trattamenti.nSincronizzati <> 0 Or datiSincronizzati.trattamenti.nEliminati <> 0 Then
                            note &= " Trattamenti sincronizzati: " & datiSincronizzati.trattamenti.nSincronizzati & ". Trattamenti eliminati:" & datiSincronizzati.trattamenti.nEliminati & "."
                        End If
                    End If

                    If datiSincronizzati.protocolli IsNot Nothing Then
                        If datiSincronizzati.protocolli.nSincronizzati <> 0 Then
                            note &= " Protocolli sincronizzati: " & datiSincronizzati.protocolli.nSincronizzati & ". "
                        End If
                    End If

                    If datiSincronizzati.indicazioniTerapeutiche IsNot Nothing Then
                        If datiSincronizzati.indicazioniTerapeutiche.nSincronizzati <> 0 Then
                            note &= " Indicazioni Terapeutiche sincronizzate: " & datiSincronizzati.indicazioniTerapeutiche.nSincronizzati & ". "
                        End If
                    End If

                    If datiSincronizzati.ricetteVeterinarie IsNot Nothing Then
                        If datiSincronizzati.ricetteVeterinarie.nSincronizzati <> 0 Then
                            note &= " Ricette sincronizzate: " & datiSincronizzati.ricetteVeterinarie.nSincronizzati & ". "
                        End If
                    End If


                    If note = "" Then
                        note = "Sincronizzazione VetInfo terminata - Nessun trattamento o prescrizione trovati."
                    Else
                        note = "Sincronizzazione VetInfo terminata - " & note
                    End If

                    esito = "Eseguita"
                End If

                'AGRONICA LOG INVIO CHIAMATE
                Dim LogInvioChiamata = GiasContext.Agronica_Log_Invio_Chiamate.Where(Function(row) row.ID = idLog).FirstOrDefault
                If Not IsNothing(LogInvioChiamata) Then
                    LogInvioChiamata.Esito = esito
                    LogInvioChiamata.Data_Modifica = Date.Now
                    LogInvioChiamata.Username_Modifica = objP_Server.UtenteCodFiscale
                    GiasContext.Entry(LogInvioChiamata).State = Entity.EntityState.Modified

                    GiasContext.SaveChanges()
                End If

                'AGRONICA LOG INVIO ANAGRAFE
                Dim logInvioAnagrafe = objLogInvioAnagrafe_W.Modifica(enum_Esportazioni_Sistema_Cod.BDN, idLog, -1, "SincroVetInfo",
                                                                      chiaveStalla, piva, saCod, 0, 0, staNum,
                                                                      note, DateTime.Now, "", objP_Server,
                                                                      "", 0, staNum, causaleCod)
            End If
        Catch ex As Exception
            Return 0
        End Try

        Return idLog

    End Function

#Region "TEST"

    ''' <summary>
    ''' TEST
    ''' </summary>
    Private Class RespGiacenza
        Public lotto As String
        Public giacenzaPresente As Boolean

        Public Sub New()
            lotto = ""
            giacenzaPresente = False
        End Sub
    End Class

    ''' <summary>
    ''' TEST
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="codiceAic"></param>
    ''' <param name="qta"></param>
    ''' <returns></returns>
    Private Function getGiacenzaFarmaco(ByVal piva As String, ByVal codiceAic As String, ByVal qta As Double) As RespGiacenza
        Dim resp As New RespGiacenza

        Dim objGiacienzeFarmaco_R As New AgronicaCoreStampeDAL.Magazzino
        Dim dtGiacenzeFarmaco As DataTable = objGiacienzeFarmaco_R.SchedaGiacenzeMagazzino(Date.Now, piva,
                                                                                           0, 0,
                                                                                           CostantiPersonalizzate.FARMACI,
                                                                                           0, 0, 0, 0,
                                                                                           0, 0, LOTTO_NONDEFINITO,
                                                                                           False, "",
                                                                                           "", "",
                                                                                           "", "",
                                                                                           "", "",
                                                                                           "", "",
                                                                                           "", "",
                                                                                           "", "",
                                                                                           objP_Server, objP_Utenti,
                                                                                           "", "",
                                                                                           False, False,
                                                                                           "", False,
                                                                                           True, CStr(codiceAic))

        If Not IsNothing(dtGiacenzeFarmaco) AndAlso dtGiacenzeFarmaco.Rows.Count > 0 Then
            resp.giacenzaPresente = True

            dtGiacenzeFarmaco.Select("WHERE Giacenza >= " & qta)
            If dtGiacenzeFarmaco.Rows.Count > 0 Then resp.lotto = dtGiacenzeFarmaco(dtGiacenzeFarmaco.Rows.Count - 1)("Lotto")

        End If

        Return resp

    End Function

    Private Function getTipoAlimento(ByVal codAlimento As Integer) As String
        Select Case codAlimento
            Case 1
                Return "CARNE"
            Case 2
                Return "LATTE"
            Case Else
                Return ""
        End Select
    End Function

    ''' <summary>
    ''' TEST Solo per creazione Trattamento GIAS da Prescrizione o Protocollo sincronizzato
    ''' </summary>
    ''' <param name="Inizio"></param>
    ''' <param name="Fine"></param>
    ''' <param name="Durata"></param>
    ''' <param name="numSomm"></param>
    ''' <returns></returns>
    Private Function getDurataTrattamento(ByVal Inizio As Date,
                                          ByVal Fine As Date,
                                          ByVal Durata As Integer,
                                          Optional ByVal numSomm As Integer = 1) As anagrafiche.IntervalloTemporale
        If IsNothing(Inizio) OrElse Inizio <= AGRODATAINIZIO Then Inizio = DateTime.Now

        If IsNothing(Fine) OrElse Fine >= AGRODATAFINE Then Fine = DateTime.Now

        Inizio = Inizio.AddDays(Durata * (numSomm - 1))

        If Durata > 0 Then
            Fine = Inizio.AddDays(Durata - 1)
        Else
            'imposta l'orario alla mezzanotte del giorno
            Fine = New DateTime(Inizio.Year, Inizio.Month, Inizio.Day + 1)
        End If

        Return New anagrafiche.IntervalloTemporale With {
            .inizio = Inizio,
            .fine = Fine
        }

    End Function

    ''' <summary>
    ''' TEST
    ''' Per modificare una ricetta gia' esistente inserire il Ricetta_Cod e il/gli Id_Agenda 
    ''' </summary>
    ''' <param name="idRicetta"></param>
    ''' <returns></returns>
    Public Function PopolaAttivitaFromRicettaZoo(ByVal idRicetta As Integer,
                                                 Optional ByVal numSommxProd As Integer = 1) As AgronicaCoreModelsSTD.attivita.Attivita
        Dim ricettaZoo = GiasContext.Ricette_Zoo.Where(Function(rz) rz.IdRicetta = idRicetta).FirstOrDefault
        Dim piva As String = ricettaZoo.Piva
        Dim saCod As Integer = ricettaZoo.Sa_Cod
        Dim staNum As Integer = ricettaZoo.Sta_Num
        Dim fabbricati_R As New Fabbricati_R
        Dim fabbCod As Integer = fabbricati_R.LeggiMagazzino(piva, saCod, objP_Server)

        Dim attivitaTrattamentoZoo As New AgronicaCoreModelsSTD.attivita.Attivita With {
            .codice = 0, '0, 'Ricetta_Cod
            .tipo = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta,
            .tipoRicetta = enum_TipoRicetta.Zoo,
            .codiceOperazioneRicetta = idRicetta,
            .job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_CUREMEDICAMENTI_ANIMALI, ""),
            .centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(saCod, piva)),
            .fabbricatoCod = staNum,
            .inizio = DateTime.Now,
            .fine = AGRODATAFINE,
            .attivitaCollegate = New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
        }

        Dim ricetteZoo_Agenda = GiasContext.Ricette_Zoo_Agenda.Where(Function(rza) rza.IdRicetta = idRicetta).ToList
        ricetteZoo_Agenda.ForEach(Sub(rza)
                                      ' Deve creare un Trattamento diverso per ogni prodotto della Prescrizione (ovvero per ogni Riga della Prescrizione)
                                      Dim idRza As Integer = rza.IdAgenda

                                      For Each sommxprod In Enumerable.Range(1, numSommxProd)
                                          Dim attivitaCureMedZoo As New AgronicaCoreModelsSTD.attivita.Attivita With {
                                              .codice = 0, '0, 'Id_Agenda
                                              .codiceOperazioneRicetta = idRza,
                                              .job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_CUREMEDICAMENTI_ANIMALI, ""),
                                              .centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(saCod, piva)),
                                              .fabbricatoCod = staNum,
                                              .inizio = DateTime.Now,
                                              .fine = AGRODATAFINE,
                                              .risorse = New List(Of AgronicaCoreModelsSTD.attivita.risorse.Risorsa),
                                              .centriDiCosto = New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                                          }

                                          ' Per ogni Riga di Prescrizione (Prodotto) crea n Somministrazioni 
                                          Dim ricetteZoo_Mov = GiasContext.Ricette_Zoo_Movimenti.Where(Function(rzm) rzm.IdRicetta = idRicetta And rzm.IdAgenda = idRza).FirstOrDefault
                                          Dim idRzm As Integer = ricetteZoo_Mov.IdMov

                                          Dim ricetteZoo_Dett = GiasContext.Ricette_Zoo_Dettagli.Where(Function(rzdtt) rzdtt.IdRicetta = idRicetta And rzdtt.IdAgenda = idRza And rzdtt.IdMov = idRzm).ToList
                                          ricetteZoo_Dett.ForEach(Sub(rzdtt)
                                                                      Dim idRzdtt As Integer = rzdtt.IdDettaglio

                                                                      Dim ricetteZoo_DettTecn = GiasContext.Ricette_Zoo_Dettaglio_Tecnico.Where(Function(rzdtc) rzdtc.Piva = piva AndAlso rzdtc.Sa_Cod = saCod AndAlso rzdtc.Id_Agenda = idRza AndAlso rzdtc.Id_Mov = idRzm AndAlso rzdtc.Id_Mov_Det = idRzdtt).ToList
                                                                      Dim tempiSos = ricetteZoo_DettTecn.Select(Function(sosp) New TempiSospensione With {
                                                                                                        .Alimento = New baseClass.BaseCodeDescr(sosp.Dett_Cod, getTipoAlimento(sosp.Dett_Cod)),
                                                                                                        .tempoSospensione = sosp.Extra_Int
                                                                                                      }).ToArray

                                                                      Dim qtaxSomm As Decimal = rzdtt.Quantitativo / numSommxProd
                                                                      Dim durataSomm As Integer = If(rza.DurataTrattamento >= numSommxProd, rza.DurataTrattamento / numSommxProd, rza.DurataTrattamento)

                                                                      Dim somministrazione As New DettaglioRegistroSomministrazioni With {
                                                                              .validita = getDurataTrattamento(rza.DataInizioTrattamento, rza.DataFineTrattamento, durataSomm, sommxprod),
                                                                              .codiceAIC = rzdtt.ProdottoAic,
                                                                              .unitaDiMisura = New metaschema.UnitaDiMisura(rzdtt.Udm_Cod),
                                                                              .quantitaTotaleReale = qtaxSomm,
                                                                              .codice = "", 'sommNumero
                                                                              .dataPrescrizione = ricettaZoo.DataEmissione,
                                                                              .numTrattamento = "", 'trattNumero
                                                                              .regSco_Numero = rzdtt.RegScoNumero
                                                                      }
                                                                      somministrazione.sospensione = tempiSos
                                                                      attivitaCureMedZoo.risorse.Add(somministrazione)

                                                                      Dim ricetteZoo_Dest = GiasContext.Ricette_Zoo_Destinazioni.Where(Function(rzdst) rzdst.IdRicetta = idRicetta And rzdst.IdAgenda = idRza And rzdst.IdMov = idRzm And rzdst.IdDettaglio = idRzdtt).ToList
                                                                      ricetteZoo_Dest.ForEach(Sub(rzdest)
                                                                                                  Dim capoCDC As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC()
                                                                                                  capoCDC.capoAnimale = New anagrafiche.CapoAnimale(piva, rzdest.CodAnimale, rzdest.Matricola)
                                                                                                  attivitaCureMedZoo.centriDiCosto.Add(capoCDC)
                                                                                              End Sub)

                                                                      Dim resp = getGiacenzaFarmaco(piva, rzdtt.ProdottoAic, rzdtt.Quantitativo)
                                                                      If resp.giacenzaPresente Then
                                                                          Dim scaricoFarmaco As New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto With {
                                                                              .MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino),
                                                                              .prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto With {
                                                                                  .tipo = New AgronicaCoreModelsSTD.attivita.risorse.TipoRisorsa(CostantiPersonalizzate.FARMACI),
                                                                                  .elemCod = CostantiPersonalizzate.FARMACI,
                                                                                  .codice_alfanumerico = "" 'sommNumero
                                                                              }
                                                                          }
                                                                          Dim magazzino As New anagrafiche.Fabbricato With {
                                                                              .primaryKey = New anagrafiche.FabbricatoLight.PK(piva, saCod, fabbCod), 'TODO Come decidere il magazzino su cui effettuare lo scarico nel caso di prodotto presente?
                                                                              .tipo = CostantiPersonalizzate.MAGAZZINO
                                                                          }
                                                                          Dim rilevMagazzino As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino With {
                                                                              .Prodotto = scaricoFarmaco.prodotto,
                                                                              .Magazzino = magazzino,
                                                                              .Lotto = resp.lotto
                                                                          }

                                                                          scaricoFarmaco.MagazziniMovimentazioni.Add(rilevMagazzino)
                                                                          attivitaCureMedZoo.risorse.Add(scaricoFarmaco)
                                                                      End If
                                                                  End Sub)

                                          'Aggiunge l'attivita da cui genera l'operazione agenda di trattamento zoo
                                          attivitaTrattamentoZoo.attivitaCollegate.Add(attivitaCureMedZoo)
                                      Next
                                  End Sub)

        Return attivitaTrattamentoZoo

    End Function

    ''' <summary>
    ''' TEST
    ''' Generare solo da Prescrizioni di tipo Protocollo Terapeutico
    ''' Per modificare una ricetta gia' esistente inserire il Ricetta_Cod e il/gli Id_Agenda 
    ''' </summary>
    ''' <param name="idRicetta"></param>
    ''' <param name="listaCodAnimalixSomm">Verranno aggiunti gli stessi capi a tutte le somministrazioni da creare.</param>
    ''' <param name="dataInizioTratt"></param>
    ''' <param name="listaProdottiUtilizzati">Devono esserre Famiglie AIC differenti tra loro.</param>
    ''' <returns></returns>
    Public Function PopolaAttivitaFromRicettaZoo_Prot(ByVal idRicetta As Integer,
                                                      ByVal listaCodAnimalixSomm As List(Of Integer),
                                                      Optional ByVal dataInizioTratt As Date = AGRODATAINIZIO,
                                                      Optional ByVal listaProdottiUtilizzati As List(Of String) = Nothing) As AgronicaCoreModelsSTD.attivita.Attivita
        If IsNothing(listaCodAnimalixSomm) Then Throw New Exception("Non e' possibile creare un trattamento da protocollo senza capi da trattare.")

        Dim ricettaZoo = GiasContext.Ricette_Zoo.Where(Function(rz) rz.IdRicetta = idRicetta).FirstOrDefault

        If ricettaZoo.TipoCodice <> enum_TipoPrescrizione.Protocollo_Terapeutico Then Throw New Exception("Tipo di ricetta non valido.")

        Dim piva As String = ricettaZoo.Piva
        Dim saCod As Integer = ricettaZoo.Sa_Cod
        Dim staNum As Integer = ricettaZoo.Sta_Num
        Dim fabbricati_R As New Fabbricati_R
        Dim fabbCod As Integer = fabbricati_R.LeggiMagazzino(piva, saCod, objP_Server)

        Dim attivitaRicettaZoo As New AgronicaCoreModelsSTD.attivita.Attivita With {
            .codice = 0, '0, 'Ricetta_Cod
            .tipo = AgronicaCoreModelsSTD.attivita.Attivita.Tipo_Attivita.Ricetta,
            .tipoRicetta = enum_TipoRicetta.Zoo,
            .codiceOperazioneRicetta = idRicetta,
            .job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_CUREMEDICAMENTI_ANIMALI, ""),
            .centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(saCod, piva)),
            .fabbricatoCod = staNum,
            .inizio = DateTime.Now,
            .fine = AGRODATAFINE,
            .attivitaCollegate = New List(Of AgronicaCoreModelsSTD.attivita.Attivita)
        }

        Dim ricetteZoo_Agenda = GiasContext.Ricette_Zoo_Agenda.Where(Function(rza) rza.IdRicetta = idRicetta).ToList
        ricetteZoo_Agenda.ForEach(Sub(rza)
                                      Dim idRza As Integer = rza.IdAgenda
                                      Dim attivitaCureMedZoo As New AgronicaCoreModelsSTD.attivita.Attivita With {
                                          .codice = 0, '0, 'Id_Agenda
                                          .codiceOperazioneRicetta = idRza,
                                          .job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_CUREMEDICAMENTI_ANIMALI, ""),
                                          .centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(saCod, piva)),
                                          .fabbricatoCod = staNum,
                                          .inizio = DateTime.Now,
                                          .fine = AGRODATAFINE,
                                          .risorse = New List(Of AgronicaCoreModelsSTD.attivita.risorse.Risorsa),
                                          .centriDiCosto = New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                                      }

                                      Dim ricetteZoo_Mov = GiasContext.Ricette_Zoo_Movimenti.Where(Function(rzm) rzm.IdRicetta = idRicetta And rzm.IdAgenda = idRza).FirstOrDefault
                                      Dim idRzm As Integer = ricetteZoo_Mov.IdMov

                                      Dim ricetteZoo_Dett = GiasContext.Ricette_Zoo_Dettagli.Where(Function(rzdtt) rzdtt.IdRicetta = idRicetta And rzdtt.IdAgenda = idRza And rzdtt.IdMov = idRzm).ToList
                                      ricetteZoo_Dett.ForEach(Sub(rzdtt)
                                                                  Dim idRzdtt As Integer = rzdtt.IdDettaglio

                                                                  ' Se sono stati indicati dei Prodotti da utilizzare, cerca quello corrispondente alla Famiglia AIC della Riga di Prescrizione corrente;
                                                                  ' altrimenti sceglie il primo Prodotto che trova appartenente a quella Famiglia AIC 
                                                                  Dim prodottoAic As String = ""
                                                                  If listaProdottiUtilizzati.Any(Function(prod) prod.Contains(rzdtt.ProdottoAic)) Then
                                                                      prodottoAic = listaProdottiUtilizzati.First(Function(prod) prod.Contains(rzdtt.ProdottoAic))
                                                                  Else
                                                                      prodottoAic = GiasContext.Farmaci.
                                                                          Where(Function(farm) DbFunctions.Left(farm.AIC, rzdtt.ProdottoAic.Length) = rzdtt.ProdottoAic).
                                                                          Select(Function(farm) farm.AIC).FirstOrDefault
                                                                  End If

                                                                  ' Per test calcolo la quantita' per somministrazione come 1 * durata (in giorni) * capi della somministrazione
                                                                  Dim qtaSomm As Decimal = rza.DurataTrattamento * listaCodAnimalixSomm.Count

                                                                  Dim somministrazione As New DettaglioRegistroSomministrazioni With {
                                                                      .validita = getDurataTrattamento(dataInizioTratt, AGRODATAFINE, rza.DurataTrattamento),
                                                                      .codiceAIC = prodottoAic,
                                                                      .unitaDiMisura = New metaschema.UnitaDiMisura(rzdtt.Udm_Cod),
                                                                      .quantitaTotaleReale = qtaSomm,
                                                                      .codice = "", 'sommNumero
                                                                      .dataPrescrizione = ricettaZoo.DataEmissione,
                                                                      .numTrattamento = "", 'trattNumero
                                                                      .regSco_Numero = "" ' il regScoNumero per i Trattamenti creati da Protocollo Terapeutico viene recuperato durante l'invio a VetInfo
                                                                  }
                                                                  Dim ricetteZoo_DettTecn = GiasContext.Ricette_Zoo_Dettaglio_Tecnico.Where(Function(rzdtc) rzdtc.Piva = piva AndAlso rzdtc.Sa_Cod = saCod AndAlso rzdtc.Id_Agenda = idRza AndAlso rzdtc.Id_Mov = idRzm AndAlso rzdtc.Id_Mov_Det = idRzdtt).ToList
                                                                  somministrazione.sospensione = ricetteZoo_DettTecn.Select(Function(sosp) New TempiSospensione With {
                                                                                                    .Alimento = New baseClass.BaseCodeDescr(sosp.Dett_Cod, getTipoAlimento(sosp.Dett_Cod)),
                                                                                                    .tempoSospensione = sosp.Extra_Int
                                                                                                  }).ToArray

                                                                  attivitaCureMedZoo.risorse.Add(somministrazione)

                                                                  Dim zooAnimali = GiasContext.Zoo_Animali.Where(Function(zoo) listaCodAnimalixSomm.Contains(zoo.Cod_Progetto)).ToList
                                                                  zooAnimali.ForEach(Sub(capo)
                                                                                         Dim capoCDC As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC()
                                                                                         capoCDC.capoAnimale = New anagrafiche.CapoAnimale(piva, capo.Cod_Progetto, capo.Matricola)
                                                                                         attivitaCureMedZoo.centriDiCosto.Add(capoCDC)
                                                                                     End Sub)

                                                                  Dim resp = getGiacenzaFarmaco(piva, rzdtt.ProdottoAic, rzdtt.Quantitativo)
                                                                  If resp.giacenzaPresente Then
                                                                      Dim scaricoFarmaco As New AgronicaCoreModelsSTD.attivita.risorse.RisorsaProdotto With {
                                                                          .MagazziniMovimentazioni = New List(Of AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino),
                                                                          .prodotto = New AgronicaCoreModelsSTD.attivita.risorse.Prodotto With {
                                                                              .tipo = New AgronicaCoreModelsSTD.attivita.risorse.TipoRisorsa(CostantiPersonalizzate.FARMACI),
                                                                              .elemCod = CostantiPersonalizzate.FARMACI,
                                                                              .codice_alfanumerico = "" 'sommNumero
                                                                          }
                                                                      }
                                                                      Dim magazzino As New anagrafiche.Fabbricato With {
                                                                          .primaryKey = New anagrafiche.FabbricatoLight.PK(piva, saCod, fabbCod), 'TODO Come decidere il magazzino su cui effettuare lo scarico nel caso di prodotto presente?
                                                                          .tipo = CostantiPersonalizzate.MAGAZZINO
                                                                      }
                                                                      Dim rilevMagazzino As New AgronicaCoreModelsSTD.attivita.RilevamentoDiMagazzino With {
                                                                          .Prodotto = scaricoFarmaco.prodotto,
                                                                          .Magazzino = magazzino,
                                                                          .Lotto = resp.lotto
                                                                      }

                                                                      scaricoFarmaco.MagazziniMovimentazioni.Add(rilevMagazzino)
                                                                      attivitaCureMedZoo.risorse.Add(scaricoFarmaco)
                                                                  End If
                                                              End Sub)

                                      'Aggiunge l'attivita da cui genera l'operazione agenda di trattamento zoo
                                      attivitaRicettaZoo.attivitaCollegate.Add(attivitaCureMedZoo)
                                  End Sub)

        Return attivitaRicettaZoo

    End Function

    ''' <summary>
    ''' TEST
    ''' </summary>
    Public Sub CreaTrattamentoDaPrescrizioneTest()
        ' Impostare l'idRicetta della prescrizione in Ricette_Zoo da cui ricavare il trattamento
        Dim idRicetta As Integer = 740
        'Dim attivita = PopolaAttivitaFromRicettaZoo(idRicetta)
        Dim attivita = PopolaAttivitaFromRicettaZoo_Prot(idRicetta, {285178, 285179}.ToList, CDate("22-01-2025"), {"104637032", "104002035"}.ToList)

        Dim attivitaZoo_W As New AttivitaZootecnicaToAgenda
        idRicetta = attivitaZoo_W.ScriviAgendaFromRicettaZootecnica(attivita, objP_Server, objP_Utenti, _idRicetta:=0)

    End Sub

    Public Function ChiamateTest(ByVal Piva As String,
                                 ByVal aziendaCodice As String,
                                 ByVal allevIdFiscale As String,
                                 ByVal propIdFiscale As String,
                                 ByVal Data_Inizio As Date,
                                 ByVal Data_Fine As Date) As String
        Dim resp As String = ""
        If propIdFiscale = "" Then
            propIdFiscale = allevIdFiscale
        End If
        Dim Sa_Cod As Integer = 0
        Dim Sta_Num As Integer = 0

        Try

            'ricava la stalla configurata sul codice presente in BDN
            Dim objStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_R
            Dim dtStalla As DataTable
            dtStalla = objStalla_R.Leggi(Piva, 0, 0, 1,
                                     "Stalla.BDN_Allev_IdFiscale = '" & allevIdFiscale &
                                     "' AND Stalla.BDN_Codice_Azienda = '" & aziendaCodice & "' ",
                                     "", objP_Server)

            If dtStalla.Rows.Count = 0 Then
                dtStalla = objStalla_R.Leggi(Piva, 0, 0, 1,
                                         " Stalla.BDN_Codice_Azienda = '" & aziendaCodice &
                                         "' AND (Stalla.BDN_Allev_IdFiscale = '' OR Stalla.BDN_Allev_IdFiscale IS NULL) ",
                                         "", objP_Server)
            End If

            If IsNothing(dtStalla) OrElse dtStalla.Rows.Count = 0 Then
                Throw New Exception("Stalla non configurata su GIAS")
            ElseIf dtStalla.Rows.Count > 1 Then
                Throw New GiasException("Allevamento " & aziendaCodice & " configurato più volte su GIAS")
            End If

            Sa_Cod = dtStalla.Rows(0)("sa_cod")
            Sta_Num = dtStalla.Rows(0)("STA_NUM")
            Dim obj_ZooAnimali_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali

            Dim objResp_Account = ws.Account()

            '	customLOGParams = New CustomLOGParams With {
            '	.LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            '	.LogDirectory = logDirectory,
            '	.LogFileName = Date.Now.Year & Date.Now.Month.ToString("D2") & Date.Now.Day.ToString("D2") & " " & aziendaCodice & " " & allevIdFiscale & ".txt"
            '}

            '	objLog.Scrivi_LOG(
            '				  objParametriServer,
            '				  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '				  "Inizio sincronizzazione trattamenti",
            '				  CustomLOGParams:=customLOGParams)

            Dim medicinali = ws.RegistroDisponibilitaMedicinali(aziendaCodice, propIdFiscale)
            Dim prescrizioni = ws.RegistroPrescrizioni(aziendaCodice, propIdFiscale, Data_Inizio, Data_Fine)
            Dim trattamentiDaValidare = ws.RegistroTrattamentiDaValidare(Data_Inizio, Data_Fine)

        Catch ex As Exception
        End Try

        Return resp
    End Function

#End Region

    Private Class Capi_OpAgenda
        Public Matricola As String
        Public Cod_Animale As Integer
    End Class

    Private Class Farmaci_OpAgenda
        Public AIC As String
        Public Qta As Decimal
    End Class

    Public Class ProdottoxGiacenza_ToSend
        Public Pro_Cod As Integer
        Public Pro_Des As String
        Public RegScoNumero As String
        Public Aic As String
        Public FamigliaAic As String
        Public Quantitativo As Double

        Public Sub New(ByVal _proCod As Integer,
                       ByVal _proDes As String,
                       ByVal _regScoNumero As String,
                       ByVal _aic As String,
                       ByVal _famigliaAic As String,
                       ByVal _quantitativo As Double)
            Pro_Cod = _proCod
            Pro_Des = _proDes
            RegScoNumero = _regScoNumero
            Aic = _aic
            FamigliaAic = _famigliaAic
            Quantitativo = _quantitativo
        End Sub

    End Class

    Public Class Trattamento_ToSend
        Public IdRicetta As Integer
        Public IdRigaRicetta As Integer
        Public IdAgenda As Integer
        Public TrattNumero As String
        Public SommNumero As String
        Public PresNumero As String
        Public PresRigaNumero As String
        Public ProtNumero As String
        Public IdProtocollo As Integer
        Public GruppoRicetta As Integer
        Public RegScoNumero As String

        Public Piva As String
        Public SaCod As Integer
        Public StaNum As Integer
        Public AziendaCodice As String
        Public PropIdFiscale As String

        Public MultiAIC As Boolean
        Public Tipo As Integer
        Public Stato As Integer
        Public ToClose As Boolean
        Public IsSync As Boolean
        Public IsClosed As Boolean

        Public GenereAnimaliInTratt As Integer

        Public DataInizio As Date
        Public DataFine As Date

        Public Aic As String
        Public FamigliaAic As String
        Public Quantitativo As Double

        Public Specie As String
        Public Note As String

        Public Prodotti As List(Of ProdottoxGiacenza_ToSend)
        Public Animali As List(Of Animali_ToSend)
        Public AnimaliToDelete As List(Of Animali_ToDelete)

        Public msgErrore As String

        Public Sub New(ByVal trattKendo As JObject)
            Me.New(trattKendo("idRicetta"),
                   trattKendo("idRigaRicetta"),
                   trattKendo("idAgenda"),
                   trattKendo("trattNumero"),
                   trattKendo("sommNumero"),
                   trattKendo("presNumero"),
                   trattKendo("presRigaNumero"),
                   trattKendo("protNumero"),
                   trattKendo("idProtocollo"),
                   trattKendo("gruppoRicetta"),
                   trattKendo("regScoNumero"),
                   trattKendo("piva"),
                   trattKendo("saCod"),
                   trattKendo("staNum"),
                   trattKendo("aziendaCodice"),
                   trattKendo("propIdFiscale"),
                   trattKendo("tipoCod"),
                   trattKendo("statoCod"),
                   trattKendo("toClose"),
                   trattKendo("isSync"),
                   trattKendo("isClosed"),
                   trattKendo("dataInizio").ToString,
                   trattKendo("dataFine").ToString,
                   trattKendo("proCod"),
                   trattKendo("qta"),
                   trattKendo("specie"),
                   trattKendo("note"))
            Dim capiTratt As List(Of JObject) = JsonConvert.DeserializeObject(Of List(Of JObject))(trattKendo("capi").ToString)
            Animali = capiTratt.Select(Function(row) New Animali_ToSend(row)).ToList
            AnimaliToDelete = New List(Of Animali_ToDelete)
        End Sub

        Public Sub New(ByVal _idRicetta As Integer,
                       ByVal _idRigaRicetta As Integer,
                       ByVal _idAgenda As Integer,
                       ByVal _trattNumero As String,
                       ByVal _sommNumero As String,
                       ByVal _presNumero As String,
                       ByVal _presRigaNumero As String,
                       ByVal _protNumero As String,
                       ByVal _idProtocollo As String,
                       ByVal _gruppoRicetta As String,
                       ByVal _regScoNumero As String,
                       ByVal _piva As String,
                       ByVal _saCod As Integer,
                       ByVal _staNum As Integer,
                       ByVal _aziendaCodice As String,
                       ByVal _propIdFiscale As String,
                       ByVal _tipo As Integer,
                       ByVal _stato As Integer,
                       ByVal _toClose As Boolean,
                       ByVal _isSync As Boolean,
                       ByVal _isClosed As Boolean,
                       ByVal _dataInizio As Date,
                       ByVal _dataFine As Date,
                       ByVal proCod As Integer,
                       ByVal _quantitativo As Double,
                       Optional ByVal _specie As String = "0121", ' di base la specie e' quella bovina
                       Optional ByVal _note As String = "",
                       Optional ByVal _capi As List(Of Animali_ToSend) = Nothing,
                       Optional ByVal _prodotti As List(Of ProdottoxGiacenza_ToSend) = Nothing,
                       Optional ByVal _aic As String = "",
                       Optional ByVal _famigliaAic As String = "",
                       Optional ByVal _multiAIC As Boolean = False)
            IdRicetta = _idRicetta
            IdRigaRicetta = _idRigaRicetta
            IdAgenda = _idAgenda
            TrattNumero = _trattNumero
            SommNumero = _sommNumero
            PresNumero = _presNumero
            PresRigaNumero = _presRigaNumero
            ProtNumero = _protNumero
            IdProtocollo = _idProtocollo
            GruppoRicetta = _gruppoRicetta

            Piva = _piva
            SaCod = _saCod
            StaNum = _staNum
            AziendaCodice = _aziendaCodice
            PropIdFiscale = _propIdFiscale

            Tipo = _tipo
            Stato = _stato
            ToClose = _toClose
            IsSync = _isSync
            IsClosed = _isClosed
            MultiAIC = _multiAIC

            DataInizio = If(dateNullOrOutOfBounds(_dataInizio), AGRODATAINIZIO, _dataInizio)
            DataFine = If(dateNullOrOutOfBounds(_dataFine), AGRODATAFINE, _dataFine)

            If MultiAIC Then
                Prodotti = _prodotti
                FamigliaAic = If(Prodotti.Select(Function(p) p.FamigliaAic).Distinct.Count = 1, Prodotti.First.FamigliaAic, "")
            Else
                RegScoNumero = _regScoNumero
                Aic = If(String.IsNullOrEmpty(_aic), getAicFarmaco(proCod), _aic)
                FamigliaAic = If(String.IsNullOrEmpty(Aic), "", Aic.Substring(0, 6))
                Quantitativo = _quantitativo
            End If

            Specie = _specie.PadLeft(4, "0"c)
            setAnimalGenre()

            Note = _note

            Animali = _capi

            AnimaliToDelete = New List(Of Animali_ToDelete)

            msgErrore = ""

        End Sub

        Private Function getAicFarmaco(proCod As Integer) As String
            Dim aic As String = ""
            Return If(dictFarmaci_ProCodToAic.TryGetValue(proCod, aic), aic, "")
        End Function

        Private Shared Function dateNullOrOutOfBounds(ByVal _date As Date)
            If IsNothing(_date) OrElse _date < AGRODATAINIZIO OrElse _date > AGRODATAFINE Then Return True
            Return False
        End Function

        Private Sub setAnimalGenre()
            Select Case Specie
                Case "0121"
                    GenereAnimaliInTratt = enum_GeneriAnimale.Bovino
                Case "0122"
                    GenereAnimaliInTratt = enum_GeneriAnimale.Suino
                Case Else
                    GenereAnimaliInTratt = enum_GeneriAnimale.UNDEFINED
            End Select
        End Sub

    End Class

    Public Class Animali_ToSend
        Public Identificativo As String
        Public NumeroCapi As Integer
        Public Sesso As String
        Public Gruppo As String

        Public SelMassiva_Cod As String
        Public SelMassiva_Op As String
        Public SelMassiva_Val1 As String
        Public SelMassiva_Val2 As String

        Public msgErrore As String

        Public Sub New(ByVal animKendo As JObject)
            Me.New(animKendo("matricola"), animKendo("sesso"))
        End Sub


        Public Sub New(ByVal _identificativo As String,
                       ByVal _sesso As String,
                       Optional _numeroCapi As Integer = 1)
            Identificativo = _identificativo
            Sesso = _sesso
            NumeroCapi = _numeroCapi

            'Inutilizzati
            SelMassiva_Cod = ""
            SelMassiva_Op = ""
            SelMassiva_Val1 = ""
            SelMassiva_Val2 = ""

            msgErrore = ""
        End Sub

    End Class

    Public Class Animali_ToDelete
        ' Nel caso di Bovini, è il singolo identificativo del capo
        ' Nel caso di Suini, è il codice del gruppo a cui appartongono i capi da rimuovere
        Public Identificativo As String

        ' Nel caso di Suini, è il numero di capi da rimuovere
        Public NumeroCapi As Integer

        Public Sub New(ByVal _identificativo As String)
            Identificativo = _identificativo
        End Sub

        Public Sub New(ByVal _identificativo As String,
                       ByVal _numeroCapi As Integer)
            Identificativo = _identificativo
            NumeroCapi = _numeroCapi
        End Sub

    End Class

End Class

Public Class Sincronizza_REV_VetInfo_Response
    Public trattamenti As OggettiVetInfo
    Public protocolli As OggettiVetInfo
    Public indicazioniTerapeutiche As OggettiVetInfo
    Public ricetteVeterinarie As OggettiVetInfo
End Class

Public Class OggettiVetInfo
    Public nTrovati As Integer
    Public nSincronizzati As Integer
    Public nNonSincronizzati As Integer
    Public nEliminati As Integer
    Public nGiaPresenti As Integer
    Public dettaglio As String
End Class

Public Class resultSincronizzazionePrescrizioniVetInfo
    Public result As OggettiVetInfo
    Public listPrescrizioni As List(Of Zoo.Prescrizione)
End Class