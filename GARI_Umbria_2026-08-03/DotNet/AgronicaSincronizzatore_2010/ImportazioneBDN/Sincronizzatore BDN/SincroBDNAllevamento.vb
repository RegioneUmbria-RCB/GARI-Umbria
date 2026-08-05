Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD
Imports AgronicaCoreMapper
Imports AgronicaCoreAnagrafeDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.attivita.centri_di_costo
Imports AgronicaCoreEntityFramework
Imports System.Transactions
Imports System.Dynamic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Runtime.CompilerServices
Imports AgronicaCoreMetaSchemaDAL
Imports AgronicaCoreModelsSTD.exceptions

Public Class SincroBDNAllevamento
    Dim objParametriServer As AgronicaCoreParametri
    Dim objParametriUtenti As AgronicaCoreParametri

    Dim ZooBIZ As AgronicaCoreAnagrafeDAL.Zoo_Animali

    Dim wsRegistroStalla As ChiamawsRegistroStallaQry
    Dim wsAnagraficaCapo As ChiamawsAnagraficaCapoQry
    Dim wsAziende As ChiamawsAziendeQry
    Dim wsCodici As ChiamawsCodiciQry
    Dim wsIdentificativi As ChiamawsIdentificativiGet
    Dim wsStrutture As ChiamawsStruttureQry
    Dim wsTerritorio As ChiamawsTerritorioQry
    Dim wsGestioneAssConsorzi As ChiamawsGestioneAssConsorzi
    Dim wsInterrogazioniModello4 As wsInterrogazioneModello4

    Dim GiasContext As Gias_DeveloperServer_Entities
    Dim sincronizzatoreAnimale As SincroBDNAnimale

    Dim logDirectory As String
    'Dim logFileName As String
    Dim objLog As New AgronicaCoreDataProvider.LogProvider
    Dim customLOGParams As CustomLOGParams

    Dim objScrivi_Zoo As AgronicaCoreAnagrafeBIZ.Zoo
    Dim obj_ZooAnimali_R As Zoo_Animali

    Dim Codifica_RazzeAnimali_DT As DataTable

    Dim razzeBDNGeneriche As List(Of String) = New List(Of String) From {"UNK", "ZZZ", "BFL"}

    'enum per tab Metaschema BDN_Causali
    Private Enum enumCausaliBDN
        ImpAnagraficaBDN = 1
        ImpModello4Uscita = 2
        ImpMovIngressoBDN = 3
        ImpMovUscitaBDN = 4

        InvAnagraficaBDN = 5
        InvMovIngressoBDN = 6
        InvMovUscitaModello4 = 7
        InvMovUscitaBDN = 8
        InvMorteBDN = 9
        ImpModello4Ingresso = 10
    End Enum

    Public Sub New(objParametriServer As AgronicaCoreParametri, objParametriUtenti As AgronicaCoreParametri, token As String, ruolo As String, valore_ruolo_codice As String)
        Me.objParametriServer = objParametriServer
        Me.objParametriUtenti = objParametriUtenti
        logDirectory = objParametriServer.LogDirectory & "\BDN\"

        customLOGParams = New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = "logBDN.txt"
        }

        ZooBIZ = New AgronicaCoreAnagrafeDAL.Zoo_Animali

        Try
            Me.wsRegistroStalla = New ChiamawsRegistroStallaQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore creazione wsRegistroStalla: " & ex.Message,
                              CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsAnagraficaCapo = New ChiamawsAnagraficaCapoQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsAnagraficaCapo: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try


        Try
            Me.wsAziende = New ChiamawsAziendeQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsAziende: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsCodici = New ChiamawsCodiciQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsCodici: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsIdentificativi = New ChiamawsIdentificativiGet(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsIdentificativi: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsStrutture = New ChiamawsStruttureQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsStrutture: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsTerritorio = New ChiamawsTerritorioQry(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsTerritorio: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.wsGestioneAssConsorzi = New ChiamawsGestioneAssConsorzi(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsGestioneAssConsorzi: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Try
            Me.sincronizzatoreAnimale = New SincroBDNAnimale(objParametriServer, objParametriServer, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione sincronizzatoreAnimale: " & ex.Message,
                          CustomLOGParams:=customLOGParams)

            If ex.InnerException IsNot Nothing Then
                objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione sincronizzatoreAnimale innerException: " & ex.InnerException.Message,
                          CustomLOGParams:=customLOGParams)
            End If

        End Try

        Try
            Me.wsInterrogazioniModello4 = New wsInterrogazioneModello4(objParametriServer, objParametriUtenti, token, ruolo, valore_ruolo_codice)
        Catch ex As Exception
            objLog.Scrivi_LOG(objParametriServer, System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore creazione wsInterrogazioniModello4: " & ex.Message,
                          CustomLOGParams:=customLOGParams)
        End Try

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(Me.objParametriServer.StringaConnessione)
        GiasContext = New Gias_DeveloperServer_Entities(EFConnString)

        objScrivi_Zoo = New AgronicaCoreAnagrafeBIZ.Zoo()
        obj_ZooAnimali_R = New Zoo_Animali()


        Try
            Dim obj_RazzeAnimali_R As New Codifica_BDN_RazzeAnimali
            Me.Codifica_RazzeAnimali_DT = obj_RazzeAnimali_R.leggi(Me.objParametriServer,
                                                                                "", "",
                                                                                "", "", "",
                                                                                enum_Esportazioni_Sistema_Cod.BDN)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Sincronizza una stalla per i diversi detentori
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sta_Num"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="SpeCod"></param>
    ''' <returns></returns>
    Public Function SincronizzaStallaGIAS(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Sta_Num As Integer,
                                          ByVal Codice_Azienda As String,
                                          ByVal SpeCod As String) As List(Of SincroBDN_Allevamento_Response)
        Dim listResult As New List(Of SincroBDN_Allevamento_Response)

        Try
            Dim dtAllevamenti As New DataTable

            'Dim a = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", "INA")

            Dim objConfStallaBDN_R As New Stalla_Configurazioni_BDN_R
            Dim dtConfStallaBDN As DataTable = objConfStallaBDN_R.Leggi(0, Piva, Sa_Cod, Sta_Num,
                                                                        "", "",
                                                                        Date.Now, AGRODATAFINE,
                                                                        enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                                                        "", "", objParametriServer)
            If IsNothing(dtConfStallaBDN) OrElse dtConfStallaBDN.Rows.Count = 0 Then
                Throw New GiasException("Stalla " & Codice_Azienda & " non configurata correttamente")
            End If

            Dim StallaMultipla = False
            If dtConfStallaBDN.Rows.Count > 1 Then
                StallaMultipla = True
            End If

            Dim idLogSincro As Integer = 0
            For Each drAllevamento In dtConfStallaBDN.Rows
                Try
                    Dim cf_detentore As String = drAllevamento("CF_DETENTORE")
                    Dim ragSoc_detentore As String = drAllevamento("RagSoc_Detentore")
                    Dim cf_proprietario As String = drAllevamento("CF_PROPRIETARIO")
                    Dim ragSoc_proprietario As String = drAllevamento("RagSoc_Proprietario")

                    customLOGParams = New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
                        .LogDirectory = logDirectory,
                        .LogFileName = "logBDN.txt"
                    }

                    'scrittura log per inizio sincronizzazione
                    idLogSincro = logSincroStalla(Piva, Sa_Cod, Sta_Num, 0, "", Nothing)

                    Dim result As SincroBDN_Allevamento_Response = SincronizzaAllevamento(Piva, Sa_Cod,
                                                                                          Sta_Num, Codice_Azienda,
                                                                                          cf_proprietario, cf_detentore, SpeCod, StallaMultipla)

                    result.cf_Detentore = cf_detentore
                    result.Detentore = ragSoc_detentore
                    result.cf_Proprietario = cf_proprietario
                    result.Proprietario = ragSoc_proprietario

                    customLOGParams = New CustomLOGParams With {
                        .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
                        .LogDirectory = logDirectory,
                        .LogFileName = "logBDN.txt"
                    }

                    'scrittura log per fine sincronizzazione
                    If idLogSincro <> 0 Then
                        logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, "", result)
                    End If

                    listResult.Add(result)

                Catch ex As ExpiredTokenBDNException
                    objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizza stalla GIAS: Piva" & Piva & ", Sa_Cod: " & Sa_Cod & " Sta_Num:" & Sta_Num & " Eccezione completa: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

                    logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, ex.Message, Nothing)
                    Throw ex
                Catch ex As GiasException
                    objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizza stalla GIAS: Piva" & Piva & ", Sa_Cod: " & Sa_Cod & " Sta_Num:" & Sta_Num & " Eccezione completa: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

                    logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, ex.Message, Nothing)
                    If dtConfStallaBDN.Rows.Count = 1 Then
                        Throw New GiasException("Errore sincronizzazione allevamento " & Codice_Azienda & " - " & drAllevamento("CF_PROPRIETARIO") & " - " & SpeCod & ": " & ex.Message)
                    End If
                    'Throw New GiasException("Errore sincronizzazione allevamento " & Codice_Azienda & " - " & drAllevamento("ID_FISCALE_PROP") & " - " & SpeCod & ": " & ex.Message)
                Catch ex As BDNException
                    objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizza stalla GIAS: Piva" & Piva & ", Sa_Cod: " & Sa_Cod & " Sta_Num:" & Sta_Num & " Eccezione completa: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

                    logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, ex.Message, Nothing)

                    If dtConfStallaBDN.Rows.Count = 1 Then
                        Throw New BDNException("Errore sincronizzazione allevamento " & Codice_Azienda & " - " & drAllevamento("CF_PROPRIETARIO") & " - " & SpeCod & ": " & ex.Message)
                    End If

                Catch ex As Exception
                    objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizza stalla GIAS: Piva" & Piva & ", Sa_Cod: " & Sa_Cod & " Sta_Num:" & Sta_Num & " Eccezione completa: " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

                    logSincroStalla(Piva, Sa_Cod, Sta_Num, idLogSincro, ex.Message, Nothing)

                    If dtConfStallaBDN.Rows.Count = 1 Then
                        Throw New Exception("Errore su Sincronizza Allevamento " & Piva & " " & Codice_Azienda & " " & drAllevamento("CF_PROPRIETARIO") & " " & SpeCod)
                    End If

                End Try
            Next

            'Dim AziendeConsorzio = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", "INA")
            'dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")

            'Dim drAllevamentiDetentore As DataRow()
            'If dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
            '    drAllevamentiDetentore = dtAllevamenti.Select(" (ID_FISCALE_DETEN = '" + IdFiscale_Detentore + "' OR ID_FISCALE = '" + IdFiscale_Detentore + "' ) AND SPE_CODICE = '" + SpeCod + "' AND DT_FINE_ATTIVITA IS NULL ")
            'Else
            '    drAllevamentiDetentore = dtAllevamenti.Select(" (ID_FISCALE_DETEN = '" + IdFiscale_Detentore + "' OR ID_FISCALE = '" + IdFiscale_Detentore + "' ) AND SPE_CODICE = '" + SpeCod + "' ")
            'End If

            'If drAllevamentiDetentore.Length > 0 Then
            '    Dim dtAllevamentiDetentore = drAllevamentiDetentore.CopyToDataTable

            '    For Each drAllevamento In drAllevamentiDetentore

            '    Next
            'End If
        Catch ex As ExpiredTokenBDNException
            Throw ex
        Catch ex As GiasException
            Throw ex
        Catch ex As BDNException
            Throw ex
        Catch ex As Exception
            Throw New Exception("Errore su SincronizzaStallaGIAS " & Piva & " " & Codice_Azienda & " " & SpeCod)
        End Try

        Return listResult

    End Function

    ''' <summary>
    ''' Scrittura log per inizio/fine sincronizzazione BDN
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="staNum"></param>
    ''' <param name="idLog">se 0 in creazione, se diverso da 0 in modifica</param>
    ''' <param name="msg">valorizzato se log di errore</param>
    ''' <param name="resultSincro"></param>
    ''' <returns></returns>
    Private Function logSincroStalla(ByVal piva As String,
                                     ByVal saCod As Integer,
                                     ByVal staNum As Integer,
                                     ByVal idLog As Integer,
                                     ByVal msg As String,
                                     ByVal resultSincro As SincroBDN_Allevamento_Response) As Integer
        Dim objLogInvioChiamate_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAnagrafe_W As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Dim chiaveStalla As String = piva & "_" & saCod & "_" & staNum

        Try
            If idLog = 0 Then
                'AGRONICA LOG INVIO CHIAMATE
                Dim IDLogInvioChiamata = objLogInvioChiamate_W.Scrivi(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                                "", DateTime.Now, "",
                                                                                                "", 0, 0,
                                                                                                objParametriServer)
                idLog = IDLogInvioChiamata

                'AGRONICA LOG INVIO ANAGRAFE
                Dim LogInvioAnagrafe As Boolean = objLogInvioAnagrafe_W.Scrivi(enum_Esportazioni_Sistema_Cod.BDN, idLog,
                                                                               -1, "SincroBDN", chiaveStalla, piva, saCod,
                                                                               0, 0, 0, "Sincronizzazione BDN iniziata",
                                                                               DateTime.Now, objParametriServer,
                                                                               Fabbricato_Cod:=staNum, Causale_Cod:=0)
            Else
                Dim esito As String = ""
                Dim note As String = ""
                Dim datiRicevuti As String = ""

                If IsNothing(resultSincro) Then
                    esito = "Non eseguita"
                    note = IIf(msg <> "", msg, "Sincronizzazione BDN terminata in errore")
                Else
                    esito = "Eseguita"
                    note = "Sincronizzazione BDN terminata - Capi in ingresso: " & resultSincro.nCapi_Ingresso & " Capi in uscita: " & resultSincro.nCapi_Uscita
                    datiRicevuti = JsonConvert.SerializeObject(resultSincro).ToString
                End If

                'AGRONICA LOG INVIO CHIAMATE
                Dim LogInvioChiamata = GiasContext.Agronica_Log_Invio_Chiamate.Where(Function(row) row.ID = idLog).FirstOrDefault
                If Not IsNothing(LogInvioChiamata) Then
                    LogInvioChiamata.Esito = esito
                    LogInvioChiamata.Data_Modifica = DateTime.Now
                    LogInvioChiamata.Username_Modifica = objParametriServer.UtenteCodFiscale
                    LogInvioChiamata.Dati_Ricevuti = datiRicevuti
                    GiasContext.Entry(LogInvioChiamata).State = Entity.EntityState.Modified

                    GiasContext.SaveChanges()
                End If

                'AGRONICA LOG INVIO ANAGRAFE
                Dim logInvioAnagrafe = objLogInvioAnagrafe_W.Modifica(enum_Esportazioni_Sistema_Cod.BDN, idLog, -1, "SincroBDN",
                                                                      chiaveStalla, piva, saCod, 0, 0, staNum,
                                                                      note, DateTime.Now, "", objParametriServer,
                                                                      "", 0, staNum, 0)
            End If

        Catch ex As Exception
            Dim msgErrore = "Errore scrittura log sincronizzazione BDN: " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & " eccezione:" & JsonConvert.SerializeObject(ex)
            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              msgErrore,
                              CustomLOGParams:=customLOGParams)
            Return 0
        End Try

        Return idLog

    End Function



    ''' <summary>
    ''' Sincronizzazione entrate e uscite capi da stalla
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="IdFiscale_Proprietario"></param>
    ''' <param name="SpeCod"></param>
    ''' <returns></returns>
    Public Function SincronizzaModello4(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Sta_Num As Integer,
                                           ByVal Codice_Azienda As String,
                                           ByVal IdFiscale_Proprietario As String,
                                           ByVal SpeCod As String,
                                           ByVal StallaMultipla As Boolean,
                                           ByVal dataInizio As Date,
                                           ByVal dataFine As Date,
                                           ByVal ListaModelli4 As List(Of Tuple(Of String, String)),
                                           ByVal ListaModello4Pascolo As List(Of Tuple(Of String, String))) As SincroBDN_Allevamento_Response
        Dim sincroAllev_Response As New SincroBDN_Allevamento_Response

        customLOGParams.LogFileName = Date.Now.Year & Date.Now.Month & Date.Now.Day & " " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ".txt"
        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod)

        Try

            Dim dtAllevamenti As New DataTable

            'Dim AziendeConsorzio = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", "INA")

            dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")

            'Dim aa = wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Proprietario, SpeCod)

            Dim dtAllevamentiFiltered As DataTable
            If Not dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            Else
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' AND ( DT_FINE_ATTIVITA = '' OR DT_FINE_ATTIVITA IS NULL ) ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            End If

            If IsNothing(dtAllevamentiFiltered) Then
                Throw New GiasException("Allevamento non trovato")
            End If

            If dtAllevamentiFiltered IsNot Nothing AndAlso dtAllevamentiFiltered.Rows.Count = 0 Then
                Throw New GiasException("Allevamento non trovato")
            End If

            Dim drAllevamento = dtAllevamentiFiltered(0)

            Dim Allev_Id = drAllevamento("ALLEV_ID")
            Dim IdFiscale_Detentore = drAllevamento("ID_FISCALE_DETEN")
            Dim Azienda_id = drAllevamento("AZIENDA_ID")
            Dim Asl_Codice = drAllevamento("CODICE_ASL")

            'ricava il raggruppamento configurato su GIAS come appoggio per la sincronizzazione con BDN
            Dim objRaggruppStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
            Dim dtRaggruppamentoStalla = objRaggruppStalla_R.Leggi_x_anagrafica(objParametriServer.PivaSuperUser,
                                                                                Piva, Sa_Cod, Sta_Num, 0,
                                                                                "Stalla_Raggruppamenti.Flag_BDN = 1",
                                                                                "", objParametriServer)

            If IsNothing(dtRaggruppamentoStalla) OrElse dtRaggruppamentoStalla.Rows.Count = 0 Then
                Throw New GiasException("L'allevamento " & Codice_Azienda & " non ha raggruppamenti presenti su GIAS")

            End If

            Dim Raggruppamento_Cod As Integer = dtRaggruppamentoStalla.Rows(0)("raggruppamento_cod")
            Dim CentroAzienda_Cod As Integer = 0

            'legge i capi in stalla (BDN)
            'Dim dt_ConsistenzaStalla_BDN As DataTable
            'Try
            '    dt_ConsistenzaStalla_BDN = wsRegistroStalla.Capi_In_Stalla(Allev_Id)
            'Catch ex As BDNException
            '    Throw ex
            'Catch ex As Exception
            '    If ex.Message.Contains("timeout") Then
            '        Throw ex
            '    End If
            'End Try

            'If IsNothing(dt_ConsistenzaStalla_BDN) Then
            '    objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "Non sono presenti capi in BDN per l'allevamento: " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod)
            'End If

            'Dim lista_ConsistenzeStalla_BDN = New List(Of IDictionary(Of String, Object)) 'dt_ConsistenzaStalla_BDN.ToExpandoObject.ToList
            'sincroAllev_Response.NumeroCapiBDN = 0
            'If dt_ConsistenzaStalla_BDN IsNot Nothing Then
            '    sincroAllev_Response.NumeroCapiBDN = dt_ConsistenzaStalla_BDN.Rows.Count
            '    lista_ConsistenzeStalla_BDN = dt_ConsistenzaStalla_BDN.ToExpandoObject.ToList
            'End If

            'lista matricole dei capi (BDN) presenti
            'Dim lista_Matricole_BDN As List(Of String) = (From cp In lista_ConsistenzeStalla_BDN
            '                                              Select CStr(cp.Item("MARCHIO"))).ToList()

            'legge i capi nella stalla (DB)            
            Dim dt_CapiPresenti_Stalla As DataTable = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)

            'Dim listaCarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)
            'Dim listaScarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)

            'Dim listaCarichiNonSincronizzatiStr As List(Of String) = (From cp In listaCarichiNonSincronizzatiDT
            '                                                          Select CStr(cp.Item("Matricola"))).ToList()

            'Dim listaScarichiNonSincronizzatiStr As List(Of String) = (From cp In listaScarichiNonSincronizzatiDT
            '                                                           Select CStr(cp.Item("Matricola"))).ToList()

            'SistemaIDBDN(dt_CapiPresenti_Stalla, dt_ConsistenzaStalla_BDN)
            'dt_CapiPresenti_Stalla = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)
            Dim lista_ConsistenzaStalla_DB As List(Of IDictionary(Of String, Object)) = dt_CapiPresenti_Stalla.ToExpandoObject.ToList

            'lista matricole dei capi (DB) presenti
            Dim lista_Matricole_DB As List(Of String) = (From cp In lista_ConsistenzaStalla_DB
                                                         Select CStr(cp.Item(key:="Matricola"))).ToList()

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "INIZIO CARICO MODELLO 4",
                              CustomLOGParams:=customLOGParams)

            '==================================================================================CARICO=======================================================================================================

            'esclude i capi già presenti su DB
            'Dim listaMatricole_Carico_BDN As List(Of String) = lista_Matricole_BDN.Except(lista_Matricole_DB).ToList()
            'listaMatricole_Carico_BDN.AddRange(lista_Matricole_DB)
            Dim lista_Matricole_SincroModello4_Carico As New List(Of String)
            Try
                If Not IsNothing(ListaModelli4) AndAlso ListaModelli4.Any Then
                    'lista_Matricole_SincroModello4_Carico = Leggi_Modelli4Ingresso(listaMatricole_Carico_BDN,
                    lista_Matricole_SincroModello4_Carico = Leggi_Modelli4Ingresso(lista_Matricole_DB,
                                                                                    lista_ConsistenzaStalla_DB,
                                                                                    Piva,
                                                                                    Sa_Cod,
                                                                                    Sta_Num,
                                                                                    Asl_Codice,
                                                                                    Codice_Azienda,
                                                                                    Azienda_id,
                                                                                    IdFiscale_Proprietario,
                                                                                    Allev_Id,
                                                                                    SpeCod,
                                                                                    IdFiscale_Detentore,
                                                                                    Raggruppamento_Cod,
                                                                                    dataInizio,
                                                                                    dataFine,
                                                                                    ListaModelli4,
                                                                                    True)
                End If

                If Not IsNothing(ListaModello4Pascolo) AndAlso ListaModello4Pascolo.Any Then
                    Dim lista_matricole_sincromodello4_carico_pascolo As List(Of String) = Leggi_Modelli4IngressoPascolo(lista_Matricole_DB,
                                                                                   lista_ConsistenzaStalla_DB,
                                                                                                      Piva,
                                                                                                      Sa_Cod,
                                                                                                      Sta_Num,
                                                                                                      Asl_Codice,
                                                                                                      Codice_Azienda,
                                                                                                      Azienda_id,
                                                                                                      IdFiscale_Proprietario,
                                                                                                      Allev_Id,
                                                                                                      SpeCod,
                                                                                                      IdFiscale_Detentore,
                                                                                                      Raggruppamento_Cod,
                                                                                                      dataInizio,
                                                                                                      dataFine,
                                                                                                      ListaModello4Pascolo,
                                                                                                      True)

                    lista_Matricole_SincroModello4_Carico.AddRange(lista_matricole_sincromodello4_carico_pascolo)
                End If

                sincroAllev_Response.listaCapi_Ingresso = lista_Matricole_SincroModello4_Carico.OfType(Of Object).ToList

            Catch ex As Exception
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Modello 4 Ingresso per Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            End Try
        Catch ex As ExpiredTokenBDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore ExpiredTokenBDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As GiasException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore GiasException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw New GiasException(ex.Message)

        Catch ex As BDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore BDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As Exception

            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore Exception(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

            Throw ex
        End Try


        Return sincroAllev_Response

    End Function



    ''' <summary>
    ''' Sincronizzazione entrate e uscite capi da stalla
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="IdFiscale_Proprietario"></param>
    ''' <param name="SpeCod"></param>
    ''' <returns></returns>
    Public Function SincronizzaAllevamento(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Sta_Num As Integer,
                                           ByVal Codice_Azienda As String,
                                           ByVal IdFiscale_Proprietario As String,
                                           ByVal IdFiscale_DetentoreConfigurato As String,
                                           ByVal SpeCod As String,
                                           ByVal StallaMultipla As Boolean) As SincroBDN_Allevamento_Response
        Dim sincroAllev_Response As New SincroBDN_Allevamento_Response

        customLOGParams.LogFileName = Date.Now.Year & Date.Now.Month & Date.Now.Day & " " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ".txt"
        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod,
                              CustomLOGParams:=customLOGParams)

        Try

            Dim dtAllevamenti As New DataTable

            'Dim AziendeConsorzio = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", "INA")

            dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")

            Dim aa = wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Proprietario, SpeCod)

            Dim dtAllevamentiFiltered As DataTable
            If Not dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            Else
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' AND ( DT_FINE_ATTIVITA = '' OR DT_FINE_ATTIVITA IS NULL ) ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            End If

            If IsNothing(dtAllevamentiFiltered) Then
                Throw New GiasException("Allevamento non trovato")
            End If

            If dtAllevamentiFiltered IsNot Nothing AndAlso dtAllevamentiFiltered.Rows.Count = 0 Then
                Throw New GiasException("Allevamento non trovato")
            End If

            Dim drAllevamento = dtAllevamentiFiltered(0)

            Dim Allev_Id = drAllevamento("ALLEV_ID")
            Dim IdFiscale_Detentore = drAllevamento("ID_FISCALE_DETEN")
            Dim Azienda_id = drAllevamento("AZIENDA_ID")
            Dim Asl_Codice = drAllevamento("CODICE_ASL")

            If IdFiscale_DetentoreConfigurato <> "" AndAlso IdFiscale_Detentore <> IdFiscale_DetentoreConfigurato Then
                Throw New GiasException("Il detentore dell'allevamento (" & IdFiscale_Detentore & ") non corrisponde a quello configurato (" & IdFiscale_DetentoreConfigurato & ")")
            End If

            'ricava il raggruppamento configurato su GIAS come appoggio per la sincronizzazione con BDN
            Dim objRaggruppStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
            Dim dtRaggruppamentoStalla = objRaggruppStalla_R.Leggi_x_anagrafica(objParametriServer.PivaSuperUser,
                                                                                Piva, Sa_Cod, Sta_Num, 0,
                                                                                "Stalla_Raggruppamenti.Flag_BDN = 1",
                                                                                "", objParametriServer)

            If IsNothing(dtRaggruppamentoStalla) OrElse dtRaggruppamentoStalla.Rows.Count = 0 Then
                Throw New GiasException("L'allevamento " & Codice_Azienda & " non ha raggruppamenti presenti su GIAS")

            End If

            Dim Raggruppamento_Cod As Integer = dtRaggruppamentoStalla.Rows(0)("raggruppamento_cod")
            Dim CentroAzienda_Cod As Integer = 0

            'legge i capi in stalla (BDN)
            Dim dt_ConsistenzaStalla_BDN As DataTable
            Try
                dt_ConsistenzaStalla_BDN = wsRegistroStalla.Capi_In_Stalla(Allev_Id)
            Catch ex As BDNException
                Throw ex
            Catch ex As Exception
                If ex.Message.Contains("timeout") Then
                    Throw ex
                End If
            End Try

            If IsNothing(dt_ConsistenzaStalla_BDN) Then
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Non sono presenti capi in BDN per l'allevamento: " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod,
                              CustomLOGParams:=customLOGParams)
            End If

            Dim lista_ConsistenzeStalla_BDN = New List(Of IDictionary(Of String, Object)) 'dt_ConsistenzaStalla_BDN.ToExpandoObject.ToList
            sincroAllev_Response.NumeroCapiBDN = 0
            If dt_ConsistenzaStalla_BDN IsNot Nothing Then
                sincroAllev_Response.NumeroCapiBDN = dt_ConsistenzaStalla_BDN.Rows.Count
                lista_ConsistenzeStalla_BDN = dt_ConsistenzaStalla_BDN.ToExpandoObject.ToList
            End If

            'lista matricole dei capi (BDN) presenti
            Dim lista_Matricole_BDN As List(Of String) = (From cp In lista_ConsistenzeStalla_BDN
                                                          Select CStr(cp.Item("MARCHIO"))).ToList()

            'legge i capi nella stalla (DB)            
            Dim dt_CapiPresenti_Stalla As DataTable = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)

            Dim listaCarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)
            Dim listaScarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)

            Dim listaCarichiNonSincronizzatiStr As List(Of String) = (From cp In listaCarichiNonSincronizzatiDT
                                                                      Select CStr(cp.Item("Matricola"))).ToList()

            Dim listaScarichiNonSincronizzatiStr As List(Of String) = (From cp In listaScarichiNonSincronizzatiDT
                                                                       Select CStr(cp.Item("Matricola"))).ToList()

            SistemaIDBDN(dt_CapiPresenti_Stalla, dt_ConsistenzaStalla_BDN)
            dt_CapiPresenti_Stalla = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)
            Dim lista_ConsistenzaStalla_DB As List(Of IDictionary(Of String, Object)) = dt_CapiPresenti_Stalla.ToExpandoObject.ToList

            'lista matricole dei capi (DB) presenti
            Dim lista_Matricole_DB As List(Of String) = (From cp In lista_ConsistenzaStalla_DB
                                                         Select CStr(cp.Item("Matricola"))).ToList()

            'objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "INIZIO CARICO MODELLO 4")

            '==================================================================================CARICO=======================================================================================================

            'esclude i capi già presenti su DB
            Dim listaMatricole_Carico_BDN As List(Of String) = lista_Matricole_BDN.Except(lista_Matricole_DB).ToList()

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "lista_Matricole_BDN: " & JsonConvert.SerializeObject(lista_Matricole_BDN),
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "lista_Matricole_DB: " & JsonConvert.SerializeObject(lista_Matricole_DB),
                              CustomLOGParams:=customLOGParams)

            listaMatricole_Carico_BDN.AddRange(lista_Matricole_DB)
            'Dim lista_Matricole_SincroModello4_Carico As New List(Of String)
            'Try
            '    lista_Matricole_SincroModello4_Carico = Leggi_Modelli4Ingresso(listaMatricole_Carico_BDN,
            '                                                                   lista_ConsistenzaStalla_DB,
            '                                                                                      Piva,
            '                                                                                      Sa_Cod,
            '                                                                                      Sta_Num,
            '                                                                                      Asl_Codice,
            '                                                                                      Codice_Azienda,
            '                                                                                      Azienda_id,
            '                                                                                      IdFiscale_Proprietario,
            '                                                                                      Allev_Id,
            '                                                                                      SpeCod,
            '                                                                                      IdFiscale_Detentore,
            '                                                                                      Raggruppamento_Cod)
            '    Dim lista_matricole_sincromodello4_carico_pascolo As List(Of String) = Leggi_Modelli4IngressoPascolo(listaMatricole_Carico_BDN,
            '                                                                   lista_ConsistenzaStalla_DB,
            '                                                                                      Piva,
            '                                                                                      Sa_Cod,
            '                                                                                      Sta_Num,
            '                                                                                      Asl_Codice,
            '                                                                                      Codice_Azienda,
            '                                                                                      Azienda_id,
            '                                                                                      IdFiscale_Proprietario,
            '                                                                                      Allev_Id,
            '                                                                                      SpeCod,
            '                                                                                      IdFiscale_Detentore,
            '                                                                                      Raggruppamento_Cod)

            '    lista_Matricole_SincroModello4_Carico.AddRange(lista_matricole_sincromodello4_carico_pascolo)

            'Catch ex As Exception
            '    objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "Errore Sincronizzazione Modello 4 Ingresso per Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
            'End Try

            'objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "FINE CARICO MODELLO 4")

            'capi su DB già sincronizzati con BDN
            Dim listaMatricole_SincroBDN_DB As List(Of String) = (From cp In lista_ConsistenzaStalla_DB
                                                                  Where cp.Item("Id_Capo_BDN") <> 0
                                                                  Select CStr(cp.Item("Matricola"))).ToList()



            'ricava matricole dei capi su DB già sincronizzati con Modello4, ma non con la BDN
            listaMatricole_Carico_BDN = listaMatricole_Carico_BDN.Except(listaMatricole_SincroBDN_DB).ToList()



            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Totale Capi BDN: " & lista_Matricole_BDN.Count,
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "INIZIO CARICO BDN",
                              CustomLOGParams:=customLOGParams)

            Dim lista_Matricole_SincroBDN_Carico As New List(Of Object)
            If Not IsNothing(listaMatricole_Carico_BDN) AndAlso listaMatricole_Carico_BDN.Count > 0 Then
                'filtra le consistenze lette da BDN, escludendo i capi già presenti su DB ma senza Id_BDN valorizzato
                Dim MatFiltered_CaricoCapi = (From cp In lista_ConsistenzeStalla_BDN
                                              Where listaMatricole_Carico_BDN.Contains(cp.Item("MARCHIO")) AndAlso
                                                  (Not listaScarichiNonSincronizzatiStr.Contains(cp.Item("MARCHIO")))
                                              Select cp).ToList()

                Dim lista_ConsistenzeStalla_BDN_str = JsonConvert.SerializeObject(lista_ConsistenzeStalla_BDN)
                Dim listaScarichiNonSincronizzatiStr_str = JsonConvert.SerializeObject(listaScarichiNonSincronizzatiStr)

                'carica i capi non sincronizzati su DB con BDN
                lista_Matricole_SincroBDN_Carico = ConsistenzaCapiBDN_Carico(MatFiltered_CaricoCapi,
                                                                             dt_CapiPresenti_Stalla.ToExpandoObject.ToList(),
                                                                             Piva,
                                                                             Sa_Cod,
                                                                             Sta_Num,
                                                                             Raggruppamento_Cod,
                                                                             Codice_Azienda,
                                                                             IdFiscale_Proprietario,
                                                                             SpeCod,
                                                                             Allev_Id,
                                                                             IdFiscale_Detentore,
                                                                             Asl_Codice,
                                                                             CentroAzienda_Cod)
            End If

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "FINE CARICO BDN",
                              CustomLOGParams:=customLOGParams)

            ''aggiunge quelli sincronizzati col Modello4 a quelli sincronizzati con la BDN
            'lista_Matricole_SincroBDN_Carico.AddRange(lista_Matricole_SincroModello4_Carico)

            sincroAllev_Response.listaCapi_Ingresso = lista_Matricole_SincroBDN_Carico

            SincronizzaMovimentiCarico(listaCarichiNonSincronizzatiStr, listaCarichiNonSincronizzatiDT, IdFiscale_Proprietario, Codice_Azienda, SpeCod)

            '==================================================================================SCARICO=====================================================================================
            ''capi su DB sincronizzati con DB e non usciti tramite Modello4
            'lista_ConsistenzaStalla_DB = (From cp In (dt_CapiPresenti_Stalla.ToExpandoObject.ToList())
            '                              Where (IsDBNull(cp.Item("Modello4_Uscita_Numero")) OrElse cp.Item("Modello4_Uscita_Numero") = "") And cp.Item("Id_Capo_BDN") <> 0
            '                              Select cp).ToList()

            'objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "INIZIO SCARICO MODELLO 4")

            'Dim lista_Matricole_SincroModello4_Scarico As New List(Of String)
            'If Not IsNothing(lista_ConsistenzaStalla_DB) AndAlso lista_ConsistenzaStalla_DB.Count > 0 Then
            '    Try
            '        lista_Matricole_SincroModello4_Scarico = Leggi_Modelli4Uscita(lista_ConsistenzaStalla_DB,
            '                                                                  Piva,
            '                                                                  Sa_Cod,
            '                                                                  Sta_Num,
            '                                                                  Asl_Codice,
            '                                                                  Codice_Azienda,
            '                                                                  Azienda_id,
            '                                                                  IdFiscale_Proprietario,
            '                                                                  Allev_Id,
            '                                                                  SpeCod,
            '                                                                  IdFiscale_Detentore)
            '    Catch ex As Exception
            '        objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "Errore Sincronizzazione Modello 4 Uscita per Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True))
            '    End Try
            'End If

            'objLog.Scrivi_LOG(logDirectory,
            '                  logFileName,
            '                  objParametriServer.LogDescrizioneUtente,
            '                  System.Reflection.MethodBase.GetCurrentMethod().Name,
            '                  "FINE SCARICO MODELLO 4")

            'prima scarica quelle non più presenti su BDN ma presenti ancora su DB
            Dim listaMatricole_Scarico_BDN = lista_Matricole_DB.Except(lista_Matricole_BDN).ToList()

            'successivamente rimuove dalle matricole in BDN quelle già scaricate dal Modello4 precedentemente
            Dim lista_Matricole_ScaricoCapi = listaMatricole_Scarico_BDN '.Except(lista_Matricole_SincroModello4_Scarico).ToList()

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "INIZIO SCARICO BDN",
                              CustomLOGParams:=customLOGParams)

            Dim lista_Matricole_SincroBDN_Scarico As New List(Of Object)
            If Not IsNothing(lista_Matricole_ScaricoCapi) AndAlso lista_Matricole_ScaricoCapi.Count > 0 Then
                'filtra le consistenze lette da BDN, escludendo i capi già presenti su DB e non scaricati già dal Modello4
                Dim MatFiltered_ScaricoCapi = (From cp In lista_ConsistenzaStalla_DB
                                               Where lista_Matricole_ScaricoCapi.Contains(cp.Item("Matricola")) _
                                                   AndAlso (IsDBNull(cp.Item("Modello4_Uscita_Numero")) OrElse cp.Item("Modello4_Uscita_Numero") = "")
                                               Select cp).ToList()

                Dim MatFiltered_ScaricoCapi2 = (From cp In lista_ConsistenzaStalla_DB
                                                Where lista_Matricole_ScaricoCapi.Contains(cp.Item("Matricola"))
                                                Select cp).ToList()

                If Not IsNothing(MatFiltered_ScaricoCapi) AndAlso MatFiltered_ScaricoCapi.Count > 0 Then

                    'scarica i capi non sincronizzati su DB con la BDN e col Modello4
                    lista_Matricole_SincroBDN_Scarico = ConsistenzaCapiBDN_Scarico(MatFiltered_ScaricoCapi,
                                                                                   Piva,
                                                                                   Sa_Cod,
                                                                                   Sta_Num,
                                                                                   Raggruppamento_Cod,
                                                                                   Codice_Azienda,
                                                                                   IdFiscale_Proprietario,
                                                                                   SpeCod,
                                                                                   Allev_Id,
                                                                                   IdFiscale_Detentore,
                                                                                   Asl_Codice,
                                                                                   CentroAzienda_Cod)
                End If

            End If

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "FINE SCARICO BDN",
                              CustomLOGParams:=customLOGParams)

            ''aggiunge i capi scaricati tramite Modello4
            'lista_Matricole_SincroBDN_Scarico.AddRange(lista_Matricole_SincroModello4_Scarico)

            sincroAllev_Response.listaCapi_Uscita = lista_Matricole_SincroBDN_Scarico


            SincronizzaMovimentiScarico(listaScarichiNonSincronizzatiStr, listaScarichiNonSincronizzatiDT, IdFiscale_Proprietario, Codice_Azienda, SpeCod)


            sincroAllev_Response.capiDaControllare = SistemazioniFinali(Piva, Sa_Cod, Sta_Num, SpeCod, Codice_Azienda, IdFiscale_Proprietario, lista_ConsistenzeStalla_BDN, lista_ConsistenzaStalla_DB)

            Try
                Dim dt_CapiPresenti_StallaxAnomalie As DataTable = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)
                Dim fixAziendaNascita = fixAziendaNascitaStalla(dt_CapiPresenti_StallaxAnomalie, SpeCod)
                Dim fixMapRazza = fixMappatureBDN(dt_ConsistenzaStalla_BDN, dt_CapiPresenti_StallaxAnomalie, SpeCod)
                If fixAziendaNascita Or fixMapRazza Then
                    GiasContext.SaveChanges()
                    dt_CapiPresenti_StallaxAnomalie = CaricaCapiPresentiInStalla(Piva, Sa_Cod, Sta_Num, IdFiscale_Proprietario, StallaMultipla)
                End If
                Dim dtAnomalie = AnomalieAnagrafica(dt_ConsistenzaStalla_BDN, dt_CapiPresenti_StallaxAnomalie, SpeCod)
                sincroAllev_Response.Anomalie = dtAnomalie
            Catch ex As Exception

            End Try

            If (sincroAllev_Response.listaCapi_Ingresso IsNot Nothing) Then
                sincroAllev_Response.nCapi_Ingresso = sincroAllev_Response.listaCapi_Ingresso.Count
            End If
            If (sincroAllev_Response.listaCapi_Uscita IsNot Nothing) Then
                sincroAllev_Response.nCapi_Uscita = sincroAllev_Response.listaCapi_Uscita.Count
            End If

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Risultato finale importazione:" & JsonConvert.SerializeObject(sincroAllev_Response),
                              CustomLOGParams:=customLOGParams)

            'Dim consistenza_stalla_GIAS 
            'Dim getSoccidari = wsAziende.GetSoccidari(p_azienda_codice,
            '                                          p_allev_idfiscale,
            '                                          p_spe_codice, "")

            'Dim infoStrutture = wsStrutture.getInfo_Strutture(p_azienda_codice,
            '                                                  p_allev_idfiscale,
            '                                                  p_spe_codice)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Fine Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod,
                              CustomLOGParams:=customLOGParams)

            Dim A = 0
        Catch ex As ExpiredTokenBDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore ExpiredTokenBDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As GiasException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore GiasException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw New GiasException(ex.Message)

        Catch ex As BDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore BDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As Exception

            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore Exception(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        End Try

        Return sincroAllev_Response

    End Function

    Private Function fixMappatureBDN(DTConsistenzeBDN As DataTable, DTConsistenzeGias As DataTable, SpeCod As String)
        Dim modificataAlmenoUna = False

        For Each rowBDN In DTConsistenzeBDN.Rows
            Dim rowGias = DTConsistenzeGias.Select(" Matricola = '" & CStr(rowBDN("MARCHIO")) & "' ")
            If rowGias.Length > 0 Then
                Dim matricola = CStr(rowGias(0)("Matricola"))
                Dim cod_animale = CInt(rowGias(0)("Cod_Animale"))
                Dim azienda_nascita = CStr(rowGias(0)("AUSL_AZI_NASCITA"))
                Dim razza_Gias = CInt(rowGias(0)("RAZ_COD"))
                Dim razza_BDN = CStr(rowBDN("Razza_Codice"))
                Dim sesso_Gias = CStr(rowGias(0)("Sesso"))
                Dim sesso_BDN = CStr(rowBDN("Sesso"))

                Dim dataNascita_Gias = CDate(rowGias(0)("Dat_Nascita"))
                Dim dataNascita_BDN = CDate(rowBDN("DT_Nascita"))

                Dim matricolaMadre_Gias = CStr(rowGias(0)("Mat_Madre"))
                Dim matricolaMadre_BDN = CStr(rowBDN("Codice_Madre"))

                Dim anomaliaRazza As Boolean = False
                Dim anomaliaSesso As Boolean = False
                Dim anomaliaMatricolaMadre As Boolean = False
                Dim anomaliaDataNascita As Boolean = False

                If checkAnomaliaRazza(razza_Gias, razza_BDN) Then
                    anomaliaRazza = True
                End If

                If razza_Gias = 0 AndAlso (Not razzeBDNGeneriche.Contains(razza_BDN)) Then
                    anomaliaRazza = True
                End If

                If sesso_Gias.ToUpper() <> sesso_BDN.ToUpper() Then
                    anomaliaSesso = True
                End If

                If matricolaMadre_Gias.ToUpper().Trim() <> matricolaMadre_BDN.ToUpper().Trim() Then
                    anomaliaMatricolaMadre = True
                End If

                If dataNascita_Gias <> dataNascita_BDN Then
                    anomaliaDataNascita = True
                End If

                If anomaliaRazza Or anomaliaSesso Or anomaliaDataNascita Or anomaliaMatricolaMadre Then
                    Dim zoo_animali = getZooAnimali(matricola, cod_animale, SpeCod, razza_BDN)

                    Dim anomaliaStr = ""
                    anomaliaStr = Date.Now.ToShortDateString() & " BDN:"


                    If zoo_animali Is Nothing Then
                        Continue For
                    End If

                    If anomaliaRazza Then
                        Dim dtCodifica = sincronizzatoreAnimale.getDTCodificaRazzaCodice(SpeCod, razza_BDN)
                        If dtCodifica IsNot Nothing Then
                            zoo_animali.RAZ_COD = dtCodifica.Rows(0)("RAZ_COD")
                            zoo_animali.Data_Modifica = DateTime.Now
                            GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                            modificataAlmenoUna = True
                        End If
                        anomaliaStr &= " Razza (" & razza_BDN & ")"
                    End If

                    If anomaliaSesso Then
                        zoo_animali.Sesso = sesso_BDN
                        zoo_animali.Data_Modifica = DateTime.Now
                        GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                        modificataAlmenoUna = True
                        anomaliaStr &= " Sesso (" & sesso_BDN & ")"
                    End If

                    If anomaliaMatricolaMadre Then
                        zoo_animali.MAT_MADRE = matricolaMadre_BDN
                        zoo_animali.Data_Modifica = DateTime.Now
                        GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                        modificataAlmenoUna = True
                        anomaliaStr &= " Mat. Madre (" & matricolaMadre_BDN & ")"
                    End If

                    If anomaliaDataNascita Then
                        zoo_animali.DAT_NASCITA = dataNascita_BDN
                        zoo_animali.Data_Modifica = DateTime.Now
                        GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                        modificataAlmenoUna = True
                        anomaliaStr &= " Dat. Nascita (" & dataNascita_BDN.ToShortDateString() & ")"
                    End If

                    If zoo_animali.Anomalie_Note Is Nothing OrElse zoo_animali.Anomalie_Note = "" Then
                        zoo_animali.Anomalie_Note = anomaliaStr
                    Else
                        zoo_animali.Anomalie_Note &= vbCrLf & anomaliaStr
                    End If

                End If

            End If
        Next
        Return modificataAlmenoUna
    End Function

    Private Function aggiornaRazza(Codice_Capo As String, Cod_Animale As Integer, SpeID As String, RazzaBDN As String) As Boolean

        Dim zoo_animali = (From z In GiasContext.Zoo_Animali Where z.Matricola = Codice_Capo And z.Cod_Progetto = Cod_Animale).FirstOrDefault()
        If zoo_animali IsNot Nothing Then
            Dim dtCodifica = sincronizzatoreAnimale.getDTCodificaRazzaCodice(SpeID, RazzaBDN)
            If dtCodifica IsNot Nothing Then
                zoo_animali.RAZ_COD = dtCodifica.Rows(0)("RAZ_COD")
                GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                Return True
            End If
        End If
        Return False
    End Function

    Private Function getZooAnimali(Codice_Capo As String, Cod_Animale As Integer, SpeID As String, RazzaBDN As String) As AgronicaCoreEntityFramework_POCO.Zoo_Animali
        Dim zoo_animali = (From z In GiasContext.Zoo_Animali Where z.Matricola = Codice_Capo And z.Cod_Progetto = Cod_Animale).FirstOrDefault()
        Return zoo_animali
    End Function

    Private Function AnomalieAnagrafica(DTConsistenzeBDN As DataTable, DTConsistenzeGias As DataTable, SpeCod As String) As DataTable
        Dim dt = CreateDTAnomalieStructure()

        For Each rowBDN In DTConsistenzeBDN.Rows
            Dim rowGias = DTConsistenzeGias.Select(" Matricola = '" & CStr(rowBDN("MARCHIO")) & "' ")
            If rowGias.Length > 0 Then
                Dim rowAnomalia = checkAnomalia(dt, rowBDN, rowGias(0))
                If rowAnomalia IsNot Nothing Then
                    dt.Rows.Add(rowAnomalia)
                End If
            End If
        Next
        Return dt
    End Function

    Private Function fixAziendaNascitaStalla(DTConsistenzeGias As DataTable, SpeCod As String)
        Dim inseritaAlmenoUna = False
        For Each rowGias In DTConsistenzeGias.Rows
            Dim matricola = CStr(rowGias("Matricola"))
            Dim cod_animale = CInt(rowGias("Cod_Animale"))
            Dim azienda_nascita = CStr(rowGias("AUSL_AZI_NASCITA"))
            If azienda_nascita = "" AndAlso matricola.Contains("IT") Then
                Dim azienda_NascitaRecuperata = sincronizzatoreAnimale.RecuperaAziendaNascita(cod_animale, SpeCod, matricola)
                If azienda_NascitaRecuperata <> "" Then
                    Dim zoo_animali = (From z In GiasContext.Zoo_Animali Where z.Matricola = matricola And z.Cod_Progetto = cod_animale).FirstOrDefault()
                    If zoo_animali IsNot Nothing Then
                        zoo_animali.AUSL_AZI_NASCITA = azienda_NascitaRecuperata
                        zoo_animali.Data_Modifica = DateTime.Now
                        GiasContext.Entry(zoo_animali).State = Entity.EntityState.Modified
                    End If
                End If
                inseritaAlmenoUna = True
            End If
        Next
        Return inseritaAlmenoUna
    End Function

    Private Function checkAnomalia(dt As DataTable, rowBDN As DataRow, rowGias As DataRow) As DataRow
        Dim anomalia = ""

        Dim anomaliaRazza As Boolean = False
        Dim anomaliaSesso As Boolean = False
        Dim anomaliaMatricolaMadre As Boolean = False
        Dim anomaliaDataNascita As Boolean = False

        Dim razza_Gias = CInt(rowGias("RAZ_COD"))
        Dim razza_BDN = CStr(rowBDN("Razza_Codice"))

        If checkAnomaliaRazza(razza_Gias, razza_BDN) Then
            anomaliaRazza = True
        End If

        Dim sesso_Gias = CStr(rowGias("Sesso"))
        Dim sesso_BDN = CStr(rowBDN("Sesso"))

        If sesso_Gias.ToUpper() <> sesso_BDN.ToUpper() Then
            anomaliaSesso = True
        End If

        Dim matricolaMadre_Gias = CStr(rowGias("Mat_Madre"))
        Dim matricolaMadre_BDN = CStr(rowBDN("Codice_Madre"))

        If matricolaMadre_Gias.ToUpper().Trim() <> matricolaMadre_BDN.ToUpper().Trim() Then
            anomaliaMatricolaMadre = True
        End If

        Dim dataNascita_Gias = CDate(rowGias("Dat_Nascita"))
        Dim dataNascita_BDN = CDate(rowBDN("DT_Nascita"))

        If dataNascita_Gias <> dataNascita_BDN Then
            anomaliaDataNascita = True
        End If

        If anomaliaRazza Or anomaliaSesso Or anomaliaDataNascita Or anomaliaMatricolaMadre Then
            Dim newRow = dt.NewRow
            Dim listaAnomalie As New List(Of String)
            If anomaliaRazza Then
                listaAnomalie.Add("Anomalia Razza")
            End If
            If anomaliaSesso Then
                listaAnomalie.Add("Anomalia Sesso")
            End If
            If anomaliaMatricolaMadre Then
                listaAnomalie.Add("Anomalia Matricola Madre")
            End If
            If anomaliaDataNascita Then
                listaAnomalie.Add("Anomalia Data Nascita")
            End If
            newRow("Cod_Progetto") = CInt(rowGias("Cod_Progetto"))
            newRow("Matricola") = CStr(rowGias("Matricola"))
            newRow("Azienda_Nascita") = CStr(rowGias("AUSL_AZI_NASCITA"))

            newRow("Razza_Cod_BDN") = razza_BDN
            newRow("Razza_Des_BDN") = CStr(rowBDN("Razza_Denominazione"))

            newRow("Razza_Cod_GIAS") = razza_Gias
            newRow("Razza_Des_GIAS") = CStr(rowGias("RAZ_DES"))

            newRow("Sesso_BDN") = sesso_BDN
            newRow("Sesso_GIAS") = sesso_Gias

            newRow("Data_Nascita_BDN") = dataNascita_BDN
            newRow("Data_Nascita_GIAS") = dataNascita_Gias

            newRow("Matricola_Madre_BDN") = matricolaMadre_BDN
            newRow("Matricola_Madre_GIAS") = matricolaMadre_Gias

            newRow("Anomalia_Cod") = 1
            newRow("Anomalia_Des") = String.Join(",", listaAnomalie)
            Return newRow
        End If
        Return Nothing
    End Function

    Private Function checkAnomaliaRazza(razza_Gias As Integer, razza_BDN As String) As Boolean

        Dim rowCodifica_RazzeAnimali = Codifica_RazzeAnimali_DT.Select(" CODICE = '" & razza_BDN & "' ").CopyToDataTable

        If (rowCodifica_RazzeAnimali.Rows.Count > 0) Then
            Dim trovata = False
            For Each row In rowCodifica_RazzeAnimali.Rows
                If row("RAZ_COD") = razza_Gias Then
                    Return False
                End If
            Next
        End If

        Return True
    End Function

    Private Function CreateDTAnomalieStructure() As DataTable
        Dim dt As New DataTable
        dt.Columns.Add(New DataColumn("Cod_Progetto", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Matricola", GetType(String)))
        dt.Columns.Add(New DataColumn("Azienda_Nascita", GetType(String)))

        dt.Columns.Add(New DataColumn("Razza_Cod_BDN", GetType(String)))
        dt.Columns.Add(New DataColumn("Razza_Des_BDN", GetType(String)))

        dt.Columns.Add(New DataColumn("Razza_Cod_GIAS", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Razza_Des_GIAS", GetType(String)))

        dt.Columns.Add(New DataColumn("Sesso_BDN", GetType(String)))
        dt.Columns.Add(New DataColumn("Sesso_GIAS", GetType(String)))

        dt.Columns.Add(New DataColumn("Matricola_Madre_BDN", GetType(String)))
        dt.Columns.Add(New DataColumn("Matricola_Madre_GIAS", GetType(String)))

        dt.Columns.Add(New DataColumn("Data_Nascita_BDN", GetType(Date)))
        dt.Columns.Add(New DataColumn("Data_Nascita_GIAS", GetType(Date)))

        dt.Columns.Add(New DataColumn("Anomalia_Cod", GetType(Integer)))
        dt.Columns.Add(New DataColumn("Anomalia_Des", GetType(String)))
        Return dt
    End Function

    Private Sub SistemaIDBDN(dt_CapiPresenti_Stalla As DataTable, dt_ConsistenzaStalla_BDN As DataTable)

        If dt_CapiPresenti_Stalla.Rows.Count > 0 Then

            Dim listaMatricole_SincroBDN_DB_r = dt_CapiPresenti_Stalla.Select(" Id_Capo_BDN = 0 ")

            If listaMatricole_SincroBDN_DB_r.Length > 0 Then
                Dim listaMatricole_SincroBDN_DB = listaMatricole_SincroBDN_DB_r.CopyToDataTable
                For Each row In listaMatricole_SincroBDN_DB.Rows
                    Dim Matricola = CStr(row("Matricola"))
                    Dim Cod_Progetto = CInt(row("Cod_Animale"))
                    Dim rowConsistenza = dt_ConsistenzaStalla_BDN.Select(" MARCHIO = '" & Matricola & "' ")
                    If rowConsistenza.Length > 0 Then
                        Dim idBDN = CInt(rowConsistenza(0)("CAPO_ID"))
                        Dim zoo_animale = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = Cod_Progetto).FirstOrDefault
                        If zoo_animale IsNot Nothing Then
                            zoo_animale.Id_Capo_BDN = idBDN
                        End If
                    Else
                        Try
                            Dim dtAnimale = wsAnagraficaCapo.getCapo(Matricola)
                            Dim idBDN = CInt(dtAnimale.Rows(0)("CAPO_ID"))
                            Dim zoo_animale = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = Cod_Progetto).FirstOrDefault
                            If zoo_animale IsNot Nothing Then
                                zoo_animale.Id_Capo_BDN = idBDN
                            End If
                        Catch ex As Exception

                        End Try
                    End If
                Next

                GiasContext.SaveChanges()

            End If

        End If

    End Sub

    Private Function CaricaCapiPresentiInStalla(Piva As String, Sa_Cod As Integer, Sta_Num As Integer, IdFiscale_Proprietario As String, StallaMultipla As Boolean) As DataTable
        Dim dt_CapiPresenti_Stalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
                                                                                      0, 0, Date.Now,
                                                                                      objParametriServer)

        Dim dr_capiPresenti = dt_CapiPresenti_Stalla.Select(" CF_PROPRIETARIO = '" & IdFiscale_Proprietario & "' ")

        If dr_capiPresenti.Count > 0 Then
            If Not StallaMultipla Then
                'SISTEMAZIONE CF_PROPRIETARIO
                SistemazioneCF_Proprietario(Piva, IdFiscale_Proprietario, dt_CapiPresenti_Stalla)
            End If

            dt_CapiPresenti_Stalla = dt_CapiPresenti_Stalla.Select(" CF_PROPRIETARIO = '" & IdFiscale_Proprietario & "' ").CopyToDataTable
        Else
            dt_CapiPresenti_Stalla = New DataTable
        End If

        dt_CapiPresenti_Stalla.Columns.Add(New DataColumn("Data_Uscita") With {.DefaultValue = ""})
        Return dt_CapiPresenti_Stalla
    End Function

    Private Function SincronizzaMovimentiCarico(listaCarichiNonSincronizzatiStr As List(Of String),
                                                listaCarichiNonSincronizzatiDT As DataTable,
                                                IdFiscale_Proprietario As String,
                                                Codice_Azienda As String,
                                                Spe_Codice As String)
        If listaCarichiNonSincronizzatiStr.Count > 0 Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            transactionOptions.Timeout = TransactionManager.MaximumTimeout

            Dim StartTransaction As DateTime
            Dim EndTransaction As DateTime

            Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            StartTransaction = DateTime.Now

            Try
                For Each matricolaCarico In listaCarichiNonSincronizzatiStr
                    Try
                        Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Ingresso(IdFiscale_Proprietario, Codice_Azienda, matricolaCarico)
                        If id_Ingresso <> 0 Then
                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                            If drCarico.Length > 0 Then
                                Dim PivaCarico As String = drCarico(0)("Piva")
                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli Where g.PIVA = PivaCarico And
                                                                                                       g.Id_Agenda = ID_AgendaCarico And
                                                                                                       g.Id_Mov = ID_MovCarico And
                                                                                                       g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                                movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                                GiasContext.Entry(movDettaglioCarico).State = Entity.EntityState.Modified
                            End If
                        Else
                            Dim modello4Numero As String = ""
                            Dim modello4Prenotazione As String = ""
                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                            If drCarico.Length > 0 Then
                                modello4Numero = IIf(IsDBNull(drCarico(0)("Modello4_Ingresso_Numero")), "", drCarico(0)("Modello4_Ingresso_Numero"))
                                modello4Prenotazione = IIf(IsDBNull(drCarico(0)("Modello4_Ingresso_Prenotazione")), "", drCarico(0)("Modello4_Ingresso_Prenotazione"))
                                Dim PivaCarico As String = drCarico(0)("Piva")
                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                Dim Cod_Animale As Integer = drCarico(0)("Cod_Progetto")
                                Dim Validita_Inizio As Date = drCarico(0)("Data_Movimento")
                                'If modello4Prenotazione <> "" Then
                                '    Dim capoPresente = capoPresenteInModello4(matricolaCarico, modello4Numero, modello4Prenotazione, Spe_Codice)
                                '    If Not capoPresente Then
                                '        'Se anche id_ingresso = 0 allora il capo non è mai entrato in stalla e lo elimino
                                '        If Not objScrivi_Zoo.Animale_Movimentato(PivaCarico, Cod_Animale, Validita_Inizio, objParametriServer) Then
                                '            objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(PivaCarico, Cod_Animale, objParametriServer, GiasContext, False)
                                '        End If
                                '    End If
                                'End If
                            End If
                        End If
                    Catch ex As Exception
                        Dim a = 0
                    End Try
                Next
                GiasContext.SaveChanges()
                GiasContext.Core.AcceptAllChanges()
                scope.Complete()
                scope.Dispose()

            Catch ex As Exception
                scope.Dispose()
            Finally
                GiasContext.Dispose()
            End Try

        End If
    End Function


    Private Function capoPresenteInModello4(matricola As String,
                                            modello4Numero As String,
                                            modello4Prenotazione As String,
                                            Codice_Specie As String) As Boolean
        Dim presente = True
        Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModello(modello4Prenotazione,
                                                                                            Codice_Specie, False)

        Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
        Dim listaMatricoleCapi_Modello4 As List(Of String) = (From cp In listaCapi_Modello4
                                                              Select CStr(cp.Item("CAPO_CODICE"))).ToList()

        If listaMatricoleCapi_Modello4.Contains(matricola) Then
            presente = True
        Else
            presente = False
        End If

        Return presente
    End Function


    Private Function CancellaMovimentiCarico(listaCarichiNonSincronizzatiStr As List(Of String),
                                                listaCarichiNonSincronizzatiDT As DataTable,
                                                IdFiscale_Proprietario As String,
                                                Codice_Azienda As String)
        If listaCarichiNonSincronizzatiStr.Count > 0 Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            transactionOptions.Timeout = TransactionManager.MaximumTimeout

            Dim StartTransaction As DateTime
            Dim EndTransaction As DateTime

            Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            StartTransaction = DateTime.Now

            Try
                For Each matricolaCarico In listaCarichiNonSincronizzatiStr
                    Try
                        Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Ingresso(IdFiscale_Proprietario, Codice_Azienda, matricolaCarico)
                        If id_Ingresso <> 0 Then
                            Dim drCarico = listaCarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                            If drCarico.Length > 0 Then
                                Dim PivaCarico As String = drCarico(0)("Piva")
                                Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                                Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                                Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                                Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli Where g.PIVA = PivaCarico And
                                                                                                       g.Id_Agenda = ID_AgendaCarico And
                                                                                                       g.Id_Mov = ID_MovCarico And
                                                                                                       g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                                movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                            End If
                        End If
                    Catch ex As Exception
                        Dim a = 0
                    End Try
                Next
                GiasContext.SaveChanges()
                GiasContext.Core.AcceptAllChanges()
                scope.Complete()
                scope.Dispose()

            Catch ex As Exception
                scope.Dispose()
            Finally
                GiasContext.Dispose()
            End Try

        End If
    End Function

    Private Function SincronizzaMovimentiScarico(listaScarichiNonSincronizzatiStr As List(Of String),
                                                listaScarichiNonSincronizzatiDT As DataTable,
                                                IdFiscale_Proprietario As String,
                                                Codice_Azienda As String,
                                                Spe_Codice As String)
        If listaScarichiNonSincronizzatiStr.Count > 0 Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            transactionOptions.Timeout = TransactionManager.MaximumTimeout

            Dim StartTransaction As DateTime
            Dim EndTransaction As DateTime

            Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            StartTransaction = DateTime.Now

            Try
                For Each matricolaCarico In listaScarichiNonSincronizzatiStr

                    Dim id_uscita = sincronizzatoreAnimale.trovaID_Uscita(IdFiscale_Proprietario, Codice_Azienda, matricolaCarico)
                    If id_uscita <> 0 Then
                        Dim drCarico = listaScarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                        If drCarico.Length > 0 Then
                            Dim PivaCarico As String = drCarico(0)("Piva")
                            Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                            Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                            Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                            Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli Where g.PIVA = PivaCarico And
                                                                                                   g.Id_Agenda = ID_AgendaCarico And
                                                                                                   g.Id_Mov = ID_MovCarico And
                                                                                                   g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                            movDettaglioCarico.Id_Mov_Esterno = id_uscita
                            GiasContext.Entry(movDettaglioCarico).State = Entity.EntityState.Modified
                        End If

                        Try
                            Dim respDecesso As DataTable = wsAnagraficaCapo.getDecesso(Codice_Azienda, IdFiscale_Proprietario, Spe_Codice, matricolaCarico)

                            If Not IsNothing(respDecesso) AndAlso respDecesso.Rows.Count > 0 Then
                                Dim cauMorteBDN As String = respDecesso(0)("CAUMOR_CODICE")
                                Dim causaleMorte = GiasContext.Lista_Causali_Morte.Where(Function(row) row.Codice_BDN = cauMorteBDN).FirstOrDefault.Cod
                                Dim cod_progetto As Integer = drCarico(0)("Cod_Progetto")
                                Dim zooAnimale = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = cod_progetto).FirstOrDefault()
                                If zooAnimale IsNot Nothing Then
                                    zooAnimale.Causale_Morte = causaleMorte
                                    GiasContext.Entry(zooAnimale).State = Entity.EntityState.Modified
                                End If
                            End If
                        Catch ex As Exception

                        End Try
                    Else


                        Dim modello4Numero As String = ""
                        Dim modello4Prenotazione As String = ""
                        Dim drCarico = listaScarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                        If drCarico.Length > 0 Then
                            modello4Numero = IIf(IsDBNull(drCarico(0)("Modello4_Uscita_Numero")), "", drCarico(0)("Modello4_Uscita_Numero"))
                            modello4Prenotazione = IIf(IsDBNull(drCarico(0)("Modello4_Uscita_Prenotazione")), "", drCarico(0)("Modello4_Uscita_Prenotazione"))
                            Dim PivaCarico As String = drCarico(0)("Piva")
                            Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                            Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                            Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                            Dim Cod_Animale As Integer = drCarico(0)("Cod_Progetto")
                            Dim Validita_Inizio As Date = drCarico(0)("Data_Movimento")
                            'If modello4Prenotazione <> "" Then
                            '    Dim capoPresente = capoPresenteInModello4(matricolaCarico, modello4Numero, modello4Prenotazione, Spe_Codice)
                            '    If Not capoPresente Then
                            '        ''Se anche id_ingresso = 0 allora il capo non è mai entrato in stalla e lo elimino
                            '        'If Not objScrivi_Zoo.Animale_Movimentato(PivaCarico, Cod_Animale, Validita_Inizio, objParametriServer) Then
                            '        '    objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(PivaCarico, Cod_Animale, objParametriServer, GiasContext, False)
                            '        'End If
                            '        objScrivi_Zoo.Riapri_Animale_Con_Del_Operazione_Scarico(PivaCarico, Cod_Animale, objParametriServer, GiasContext, False)
                            '    End If
                            'End If
                        End If

                        'Dim id_ingresso = sincronizzatoreAnimale.trovaID_Ingresso(IdFiscale_Proprietario, Codice_Azienda, matricolaCarico)

                        'If id_ingresso = 0 Then
                        '    Dim a = 0
                        '    ''Se anche id_ingresso = 0 allora il capo non è mai entrato in stalla e lo elimino
                        '    'If Not objScrivi_Zoo.Animale_Movimentato(Piva, capo("Cod_Animale"), capo("Validita_Inizio"), objParametriServer) Then
                        '    '    objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(Piva, capo("Cod_Animale"), objParametriServer, GiasContext, False)
                        '    'End If
                        'End If
                    End If
                Next
                GiasContext.SaveChanges()
                GiasContext.Core.AcceptAllChanges()
                scope.Complete()
                scope.Dispose()

            Catch ex As Exception
                scope.Dispose()
            Finally
                GiasContext.Dispose()
            End Try

        End If
    End Function

    Private Function CancellaMovimentiScarico(listaScarichiNonSincronizzatiStr As List(Of String),
                                                listaScarichiNonSincronizzatiDT As DataTable,
                                                IdFiscale_Proprietario As String,
                                                Codice_Azienda As String)
        If listaScarichiNonSincronizzatiStr.Count > 0 Then
            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
            Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
            Dim transactionOptions As New TransactionOptions()
            transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
            transactionOptions.Timeout = TransactionManager.MaximumTimeout

            Dim StartTransaction As DateTime
            Dim EndTransaction As DateTime

            Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
            StartTransaction = DateTime.Now

            Try
                For Each matricolaCarico In listaScarichiNonSincronizzatiStr

                    Dim id_Ingresso = sincronizzatoreAnimale.trovaID_Uscita(IdFiscale_Proprietario, Codice_Azienda, matricolaCarico)
                    If id_Ingresso <> 0 Then
                        Dim drCarico = listaScarichiNonSincronizzatiDT.Select(" Matricola = '" & matricolaCarico & "' ")
                        If drCarico.Length > 0 Then
                            Dim PivaCarico As String = drCarico(0)("Piva")
                            Dim ID_AgendaCarico As Integer = drCarico(0)("id_agenda")
                            Dim ID_MovCarico As Integer = drCarico(0)("id_mov")
                            Dim ID_Mov_DetCarico As Integer = drCarico(0)("id_Mov_Det")
                            Dim movDettaglioCarico = (From g In GiasContext.Movimenti_dettagli Where g.PIVA = PivaCarico And
                                                                                                   g.Id_Agenda = ID_AgendaCarico And
                                                                                                   g.Id_Mov = ID_MovCarico And
                                                                                                   g.Id_Mov_Det = ID_Mov_DetCarico).FirstOrDefault()
                            movDettaglioCarico.Id_Mov_Esterno = id_Ingresso
                        End If
                    End If
                Next
                GiasContext.SaveChanges()
                GiasContext.Core.AcceptAllChanges()
                scope.Complete()
                scope.Dispose()

            Catch ex As Exception
                scope.Dispose()
            Finally
                GiasContext.Dispose()
            End Try

        End If
    End Function


    ''' <summary>
    ''' Esegue un'operazione di incremento consistenze capi da BDN scrivendo l'attività relativa
    ''' </summary>
    ''' <param name="lista_CaricoCapi"></param>
    ''' <param name="listaCapiStalla_DB"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Raggruppamento_Cod"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="IdFiscale_Allev"></param>
    ''' <param name="Spe_Codice"></param>
    ''' <param name="Allev_Id"></param>
    ''' <param name="IdFiscale_Detentore"></param>
    ''' <param name="CentroAzienda_Cod"></param>
    ''' <returns></returns>
    Private Function ConsistenzaCapiBDN_Carico(ByVal lista_CaricoCapi As List(Of IDictionary(Of String, Object)),
                                               ByVal listaCapiStalla_DB As List(Of IDictionary(Of String, Object)),
                                               ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Sta_Num As Integer,
                                               ByVal Raggruppamento_Cod As Integer,
                                               ByVal Codice_Azienda As String,
                                               ByVal IdFiscale_Allev As String,
                                               ByVal Spe_Codice As String,
                                               ByVal Allev_Id As String,
                                               ByVal IdFiscale_Detentore As String,
                                               ByVal Codice_Asl As String,
                                               ByVal CentroAzienda_Cod As Integer) As List(Of Object)

        Dim listaMatricole_Carico As New List(Of Object)
        Dim objZoo_Animali_DAL_R As New AgronicaCoreAnagrafeDAL.Zoo_Animali
        'ottiene la lista delle date di ingresso in stalla dei capi (BDN)
        Dim lista_DateIngresso_Stalla = From cp In lista_CaricoCapi
                                        Select cp.Item("DT_INGRESSO") Distinct.ToList()

        lista_DateIngresso_Stalla.Sort(Function(d1, d2)
                                           Return d1 < d2
                                       End Function)

        'aggiunge i capi su DB, raggruppandoli per data di ingresso in stalla
        For Each ingresso In lista_DateIngresso_Stalla

            Try
                Dim listaMatricole_CaricoxGiorno As New List(Of Object)
                Dim lista_capoAnimaleCDC_Acquisto As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                Dim lista_capoAnimaleCDC_Nascita As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                Dim lista_capoAnimaleCDC_PrimaIscrizione As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)

                'filtra le consistenze lette da BDN per data di ingresso in stalla
                Dim DateFiltered_CaricoCapi = lista_CaricoCapi.Where(Function(cp)
                                                                         Return cp.Item("DT_INGRESSO") = ingresso
                                                                     End Function).ToList

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Inizio carico " & DateFiltered_CaricoCapi.Count & " capi in data " & CDate(ingresso).ToShortDateString(),
                              CustomLOGParams:=customLOGParams)

                For Each capo In DateFiltered_CaricoCapi
                    Dim Codice_Capo As String = capo("CAPO_ID")
                    Dim Matricola_Capo As String = capo("MARCHIO")
                    Dim obj_CapoAnimaleCDC As CapoAnimaleCDC
                    Try
                        'controlla se il capo è già presente su DB (leggo i movimenti di carico del capo)
                        Dim movimentiCaricoDB = (From a In GiasContext.Agenda
                                                 Join m In GiasContext.Movimenti On a.Id_Agenda Equals m.Id_Agenda
                                                 Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And m.Id_Mov Equals md.Id_Mov
                                                 Join mdest In GiasContext.Mov_Destinazioni On mdest.Id_Agenda Equals md.Id_Agenda And mdest.Id_Mov Equals md.Id_Mov And mdest.Id_Mov_Det Equals md.Id_Mov_Det
                                                 Join ragg In GiasContext.Stalla_Raggruppamenti On mdest.Piva Equals ragg.PIVA And mdest.Sa_Cod Equals ragg.sa_cod And mdest.Id_Destinazione Equals ragg.Raggruppamento_Cod
                                                 Join zoo In GiasContext.Zoo_Animali On md.Cod_Progetto Equals zoo.Cod_Progetto
                                                 Where m.Cau_Mov = CAU_CARICO_CONSISTENZE And
                                                     md.Elem_Cod = 300 And
                                                     ragg.sa_cod = Sa_Cod And
                                                     ragg.STA_NUM = Sta_Num And
                                                     ragg.PIVA = Piva And
                                                     zoo.Matricola = Matricola_Capo And
                                                     zoo.CF_PROPRIETARIO = IdFiscale_Allev).ToList

                        'Dim dtAnimaleDB = objZoo_Animali_DAL_R.Leggi(Piva, 0, Matricola_Capo, Codice_Capo, objParametriServer)
                        If movimentiCaricoDB.Count > 0 Then

                            Dim codProgetto = movimentiCaricoDB(0).md.Cod_Progetto

                            Dim capoAgg = (From x In GiasContext.Zoo_Animali Where x.Cod_Progetto = codProgetto).FirstOrDefault
                            If capoAgg IsNot Nothing AndAlso capoAgg.Id_Capo_BDN = 0 AndAlso Codice_Capo <> 0 Then
                                capoAgg.Id_Capo_BDN = Codice_Capo
                                GiasContext.SaveChanges()
                            End If

                            objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Matricola " & Matricola_Capo & " già presente in stalla",
                              CustomLOGParams:=customLOGParams)
                            Continue For
                        End If

                        'Dim gefutils As New AgronicaCoreEntityFramework.Gias_EF_Utility
                        'Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
                        'Dim GiasContext = New AgronicaCoreEntityFramework.Gias_DeveloperServer_Entities(EFConnString)

                        'Dim capo_fromZooAnimali As New AgronicaCoreEntityFramework_POCO.Zoo_Animali
                        'capo_fromZooAnimali = (From cp In GiasContext.Zoo_Animali
                        '                       Where cp.Matricola = Matricola_Capo
                        '                       Select cp).First

                        Dim capo_fromZooAnimali As IDictionary(Of String, Object)
                        If Not IsNothing(listaCapiStalla_DB) AndAlso listaCapiStalla_DB.Count > 0 Then
                            capo_fromZooAnimali = (From cp In listaCapiStalla_DB
                                                   Where cp.Item("Matricola") = Matricola_Capo
                                                   Select cp).FirstOrDefault
                        End If

                        Dim str = JsonConvert.SerializeObject(capo_fromZooAnimali)
                        'objLog.Scrivi_LOG(logDirectory,
                        '      logFileName,
                        '      objParametriServer.LogDescrizioneUtente,
                        '      System.Reflection.MethodBase.GetCurrentMethod().Name,
                        '      "Qui ci arrivo: " & Matricola_Capo)

                        'se il capo è già presente su DB, ne modifica solamente i dati riguardanti la BDN;
                        'altrimenti passa alla sincronizzazione per poter effettuare il carico
                        If Not IsNothing(capo_fromZooAnimali) AndAlso capo_fromZooAnimali.Item("Id_Capo_BDN") = 0 Then
                            'objLog.Scrivi_LOG(logDirectory,
                            '  logFileName,
                            '  objParametriServer.LogDescrizioneUtente,
                            '  System.Reflection.MethodBase.GetCurrentMethod().Name,
                            '  "Qui ci arrivo 1: " & Matricola_Capo)
                            'Dim objCapoAnimale As New anagrafiche.CapoAnimale(capo_fromZooAnimali.Item("Piva"),
                            '                                              capo_fromZooAnimali.Item("Cod_Progetto"),
                            '                                              capo_fromZooAnimali.Item("Matricola"))

                            'objCapoAnimale.idCapo_BDN = Codice_Capo


                            'objScrivi_Zoo.Converti_Animale_DT(objCapoAnimale, objParametriServer,
                            '                              Nothing, True)

                            Dim codProgetto = capo_fromZooAnimali.Item("Cod_Progetto")

                            Dim capoAgg = (From x In GiasContext.Zoo_Animali Where x.Cod_Progetto = codProgetto).FirstOrDefault
                            If capoAgg IsNot Nothing AndAlso capoAgg.Id_Capo_BDN = 0 AndAlso Codice_Capo <> 0 Then
                                capoAgg.Id_Capo_BDN = Codice_Capo
                                GiasContext.SaveChanges()
                            End If

                            objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Matricola " & Matricola_Capo & " già presente in stalla",
                              CustomLOGParams:=customLOGParams)

                        Else

                            Dim Motivo_Ingresso As String = ""
                            Dim Motivo_Uscita As String = ""
                            Dim ID_Ingresso As Integer = 0
                            Dim ID_Uscita As Integer = 0

                            obj_CapoAnimaleCDC = sincronizzatoreAnimale.SincronizzaAnimale(Piva, Sa_Cod, Sta_Num,
                                                                                           Codice_Azienda,
                                                                                           IdFiscale_Allev,
                                                                                           Spe_Codice, Allev_Id,
                                                                                           Matricola_Capo,
                                                                                           IdFiscale_Detentore,
                                                                                           Motivo_Ingresso,
                                                                                           Motivo_Uscita,
                                                                                           ID_Ingresso,
                                                                                           ID_Uscita,
                                                                                           Codice_Asl,
                                                                                           objLog,
                                                                                           customLOGParams)

                            'If obj_CapoAnimaleCDC IsNot Nothing Then
                            '    objLog.Scrivi_LOG(logDirectory,
                            '        logFileName,
                            '        objParametriServer.LogDescrizioneUtente,
                            '        System.Reflection.MethodBase.GetCurrentMethod().Name,
                            '        "Qui ci arrivo 2.2: " & Matricola_Capo & JsonConvert.SerializeObject(obj_CapoAnimaleCDC))
                            'End If

                            'aggiungo il Raggruppamento_Cod di Stalla_Raggruppamenti
                            obj_CapoAnimaleCDC.sottogruppoStalla_ingresso = New anagrafiche.SottogruppoStalla
                            obj_CapoAnimaleCDC.sottogruppoStalla_ingresso.codice = Raggruppamento_Cod

                            obj_CapoAnimaleCDC.sottogruppoStalla_uscita = New anagrafiche.SottogruppoStalla
                            obj_CapoAnimaleCDC.sottogruppoStalla_uscita.codice = Raggruppamento_Cod

                            obj_CapoAnimaleCDC.id_movimentazione_BDN = ID_Ingresso

                            If obj_CapoAnimaleCDC.capoAnimale.validita.inizio <> ingresso Then
                                obj_CapoAnimaleCDC.capoAnimale.validita.inizio = ingresso
                            End If
                            If IsNothing(obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_data_prenotazione) OrElse
                               obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_data_prenotazione < AGRODATAINIZIO Then
                                obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_data_prenotazione = ingresso
                            End If
                            Select Case Motivo_Ingresso
                                Case "T", "M", "E", "F"
                                    lista_capoAnimaleCDC_Acquisto.Add(obj_CapoAnimaleCDC)
                                Case "R"
                                    lista_capoAnimaleCDC_PrimaIscrizione.Add(obj_CapoAnimaleCDC)
                                Case "N", "W"
                                    lista_capoAnimaleCDC_Nascita.Add(obj_CapoAnimaleCDC)
                                Case Else
                                    lista_capoAnimaleCDC_Acquisto.Add(obj_CapoAnimaleCDC)
                            End Select
                        End If
                    Catch ex As ExpiredTokenBDNException
                        Throw ex
                    Catch ex As Exception
                        Dim msgExtra = ""
                        If obj_CapoAnimaleCDC IsNot Nothing Then
                            msgExtra = JsonConvert.SerializeObject(obj_CapoAnimaleCDC)
                        End If

                        objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore ConsistenzaCapiBDN_Carico matricola:" & Matricola_Capo & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & vbCrLf & "ObjAnimale: " & msgExtra,
                              CustomLOGParams:=customLOGParams)
                    End Try

                    'GiasContext.Dispose()
                Next

                'se almeno un capo è in carico, parte la scrittura dell'attività
                'e il relativo carico di uno o più capi
                If lista_capoAnimaleCDC_Acquisto.Count > 0 Then
                    Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                    objAttivita.fine = AGRODATAFINE
                    objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_ACQUISTO_ANIMALI, "")
                    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                            .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                    }
                    objAttivita.fabbricatoCod = Sta_Num
                    objAttivita.inizio = CDate(ingresso)
                    objAttivita.centriDiCosto = lista_capoAnimaleCDC_Acquisto

                    'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                    Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                          objParametriServer)

                    BloccaOperazione(Piva, Id_Agenda, True)

                    For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                        logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_ACQUISTO_ANIMALI, capo.capoAnimale.matricola,
                                             enumCausaliBDN.ImpMovIngressoBDN, capo.id_movimentazione_BDN,
                                             capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                    Next
                    listaMatricole_Carico.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_Acquisto Select a.capoAnimale.matricola).ToList())
                    listaMatricole_CaricoxGiorno.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_Acquisto Select a.capoAnimale.matricola).ToList())
                End If

                If lista_capoAnimaleCDC_Nascita.Count > 0 Then
                    Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                    objAttivita.fine = AGRODATAFINE
                    objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_NASCITA_ANIMALI, "")
                    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                            .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                    }
                    objAttivita.fabbricatoCod = Sta_Num
                    objAttivita.inizio = CDate(ingresso)
                    objAttivita.centriDiCosto = lista_capoAnimaleCDC_Nascita

                    'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                    Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                          objParametriServer)
                    BloccaOperazione(Piva, Id_Agenda, True)
                    For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                        logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_NASCITA_ANIMALI, capo.capoAnimale.matricola,
                                             enumCausaliBDN.ImpMovIngressoBDN, capo.id_movimentazione_BDN,
                                             capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                    Next

                    listaMatricole_Carico.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_Nascita Select a.capoAnimale.matricola).ToList())
                    listaMatricole_CaricoxGiorno.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_Nascita Select a.capoAnimale.matricola).ToList())
                End If

                If lista_capoAnimaleCDC_PrimaIscrizione.Count > 0 Then
                    Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                    objAttivita.fine = AGRODATAFINE
                    objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_INCREMENTO_CONSISTENZE_ZOO, "")
                    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                            .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                    }
                    objAttivita.fabbricatoCod = Sta_Num
                    objAttivita.inizio = CDate(ingresso)
                    objAttivita.centriDiCosto = lista_capoAnimaleCDC_PrimaIscrizione

                    'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                    Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                          objParametriServer)
                    BloccaOperazione(Piva, Id_Agenda, True)
                    For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                        logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_ACQUISTO_ANIMALI, capo.capoAnimale.matricola,
                                             enumCausaliBDN.ImpMovIngressoBDN, capo.id_movimentazione_BDN,
                                             capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                    Next

                    listaMatricole_Carico.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_PrimaIscrizione Select a.capoAnimale.matricola).ToList())
                    listaMatricole_CaricoxGiorno.AddRange((From a As AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC In lista_capoAnimaleCDC_PrimaIscrizione Select a.capoAnimale.matricola).ToList())
                End If

                Dim listaMatricoleInsert = (From a In DateFiltered_CaricoCapi Select CStr(a("MARCHIO"))).ToList

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine carico " & listaMatricole_CaricoxGiorno.Count & " capi in data " & CDate(ingresso).ToShortDateString() & ": " & String.Join(",", listaMatricole_CaricoxGiorno),
                              CustomLOGParams:=customLOGParams)
            Catch ex As ExpiredTokenBDNException
                Throw ex
            Catch ex As Exception
                Dim msgEx = ex.Message
                If ex.InnerException IsNot Nothing Then
                    msgEx &= " Inner Exception:" & ex.InnerException.Message
                End If

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Errore carico in data " & CDate(ingresso).ToShortDateString() & vbCrLf &
                                  " Errore:" & msgEx & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                                  CustomLOGParams:=customLOGParams)
            End Try

        Next

        Return listaMatricole_Carico

    End Function

    Private Function BloccaOperazione(Piva As String, Id_Agenda As Integer, SettaTipoAccettazione1 As Boolean)
        Dim agendaDal As New AgronicaCoreContabDAL.Agenda_W
        agendaDal.Agenda_Blocca(Piva, 0, Id_Agenda, "", objParametriServer)

        If SettaTipoAccettazione1 Then
            agendaDal.ModificaPuntuale(Piva, 0, Id_Agenda, objParametriServer, Tipo_Accettazione:=1)
        End If

    End Function

    ''' <summary>
    ''' Esegue un'operazione di decremento consistenze capi da BDN scrivendo l'attività relativa
    ''' </summary>
    ''' <param name="lista_ScaricoCapi"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Raggruppamento_Cod"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="IdFiscale_Allev"></param>
    ''' <param name="Spe_Codice"></param>
    ''' <param name="Allev_Id"></param>
    ''' <param name="IdFiscale_Detentore"></param>
    ''' <param name="CentroAzienda_Cod"></param>
    ''' <returns></returns>
    Private Function ConsistenzaCapiBDN_Scarico(ByVal lista_ScaricoCapi As List(Of IDictionary(Of String, Object)),
                                                ByVal Piva As String,
                                                ByVal Sa_Cod As Integer,
                                                ByVal Sta_Num As Integer,
                                                ByVal Raggruppamento_Cod As Integer,
                                                ByVal Codice_Azienda As String,
                                                ByVal IdFiscale_Allev As String,
                                                ByVal Spe_Codice As String,
                                                ByVal Allev_Id As String,
                                                ByVal IdFiscale_Detentore As String,
                                                ByVal Codice_Asl As String,
                                                ByVal CentroAzienda_Cod As Integer) As List(Of Object)
        Dim listaMatricole_Scarico As New List(Of Object)
        Dim lista_DateUscita_Stalla As New List(Of Object)

        'estrae le date (distinte) di uscita dei capi animale dalla stalla corrente
        For Each capo In lista_ScaricoCapi
            Dim Matricola_Capo As String = capo("Matricola")
            Try
                Dim lista_MovimentazioniCapo As List(Of IDictionary(Of String, Object))
                Try
                    lista_MovimentazioniCapo = (wsRegistroStalla.getMovimentazioniCapo(Matricola_Capo)).ToExpandoObject.ToList()
                Catch ex As Exception
                    Throw New Exception("Errore lettura lista_MovimentazioniCapo: " & Matricola_Capo)
                End Try

                Dim Movimenti_Uscita_Capo = (From m In lista_MovimentazioniCapo
                                             Select m
                                             Where m.Item("ALLEV_ID_FISCALE") = IdFiscale_Allev And m.Item("AZIENDA_CODICE") = Codice_Azienda).ToList
                'Non ho trovato il movimento del capo
                If Movimenti_Uscita_Capo.Count = 0 Then
                    Continue For
                    ''Elimino il capo perché non esiste in BDN sulla stalla
                    'Dim codProgetto As Integer = CInt(capo("Cod_Animale"))
                    'Dim capoAnimale = (From Zoo_Animali In GiasContext.Zoo_Animali Where Zoo_Animali.Cod_Progetto = codProgetto).FirstOrDefault
                    'Dim dataRiferimeno = Date.Now.AddDays(-7)

                    'If capoAnimale IsNot Nothing AndAlso capoAnimale.Validita_Inizio < dataRiferimeno Then
                    '    If Not objScrivi_Zoo.Animale_Movimentato(Piva, capo("Cod_Animale"), capo("Validita_Inizio"), objParametriServer) Then
                    '        objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(Piva, capo("Cod_Animale"), objParametriServer, GiasContext, False)
                    '        Continue For
                    '    End If
                    'Else
                    '    Continue For
                    'End If
                End If
                Dim dataUscita = (From m In Movimenti_Uscita_Capo
                                  Select m
                                  Order By m.Item("DT_USCITA") Descending).First.Item("DT_USCITA")

                capo.Item("Data_Uscita") = dataUscita

                If Not lista_DateUscita_Stalla.Contains(dataUscita) Then
                    lista_DateUscita_Stalla.Add(dataUscita)

                End If
            Catch ex As ExpiredTokenBDNException
                Throw ex
            Catch ex As Exception
                capo.Item("Data_Uscita") = AGRODATAFINE
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore ConsistenzaCapiBDN_Scarico matricola:" & Matricola_Capo & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & ": ",
                              CustomLOGParams:=customLOGParams)
            End Try

        Next

        lista_DateUscita_Stalla.Sort(Function(d1, d2)
                                         Return d1 < d2
                                     End Function)

        lista_ScaricoCapi = lista_ScaricoCapi.Where(Function(x)
                                                        Return CStr(x.Item("Data_Uscita")) <> ""
                                                    End Function).ToList

        'rimuove i capi su DB, raggruppandoli per data di uscita da stalla
        For Each uscita In lista_DateUscita_Stalla
            Try
                Dim lista_capoAnimaleCDC_Macellazione As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                Dim lista_capoAnimaleCDC_Morte As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                Dim lista_capoAnimaleCDC_Vendita As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)
                Dim DateFiltered_ScaricoCapi = lista_ScaricoCapi.Where(Function(x)
                                                                           Return x.Item("Data_Uscita") = uscita
                                                                       End Function).ToList

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Inizio scarico " & DateFiltered_ScaricoCapi.Count & " capi in data " & CDate(uscita).ToShortDateString(),
                                  CustomLOGParams:=customLOGParams)


                Dim listaCausaliMorte As New List(Of Integer)
                For Each capo In DateFiltered_ScaricoCapi
                    Try
                        Dim Matricola_Capo As String = capo("Matricola")
                        Dim Raggruppamento_Capo As String = capo("Raggruppamento_Cod")
                        Dim Motivo_Ingresso As String = ""
                        Dim Motivo_Uscita As String = ""
                        Dim ID_Ingresso As Integer = 0
                        Dim ID_Uscita As Integer = 0

                        Dim obj_CapoAnimaleCDC = sincronizzatoreAnimale.SincronizzaAnimale(Piva, Sa_Cod, Sta_Num,
                                                                                           Codice_Azienda,
                                                                                           IdFiscale_Allev,
                                                                                           Spe_Codice, Allev_Id,
                                                                                           Matricola_Capo,
                                                                                           IdFiscale_Detentore,
                                                                                           Motivo_Ingresso,
                                                                                       Motivo_Uscita, ID_Ingresso, ID_Uscita, Codice_Asl,
                                                                                       objLog, customLOGParams)

                        Try
                            Dim respDecesso As DataTable = wsAnagraficaCapo.getDecesso(Codice_Azienda, IdFiscale_Allev, Spe_Codice, Matricola_Capo)

                            If Not IsNothing(respDecesso) AndAlso respDecesso.Rows.Count > 0 Then
                                Dim cauMorteBDN As String = respDecesso(0)("CAUMOR_CODICE")

                                If Not IsDBNull(cauMorteBDN) AndAlso cauMorteBDN <> "" Then
                                    obj_CapoAnimaleCDC.capoAnimale.causaleMorte = GiasContext.Lista_Causali_Morte.Where(Function(row) row.Codice_BDN = cauMorteBDN).FirstOrDefault.Cod

                                    If Not listaCausaliMorte.Contains(obj_CapoAnimaleCDC.capoAnimale.causaleMorte) Then
                                        listaCausaliMorte.Add(obj_CapoAnimaleCDC.capoAnimale.causaleMorte)
                                    End If
                                End If
                            End If
                        Catch ex As Exception

                        End Try


                        'aggiungo il Raggruppamento_Cod di Stalla_Raggruppamenti
                        obj_CapoAnimaleCDC.sottogruppoStalla_ingresso = New anagrafiche.SottogruppoStalla
                        obj_CapoAnimaleCDC.sottogruppoStalla_ingresso.codice = Raggruppamento_Capo

                        obj_CapoAnimaleCDC.sottogruppoStalla_uscita = New anagrafiche.SottogruppoStalla
                        obj_CapoAnimaleCDC.sottogruppoStalla_uscita.codice = Raggruppamento_Capo

                        obj_CapoAnimaleCDC.capoAnimale.validita.fine = CDate(uscita)

                        If obj_CapoAnimaleCDC.capoAnimale.esercizi.Count = 1 Then
                            obj_CapoAnimaleCDC.capoAnimale.esercizi(0).validita.fine = CDate(uscita)
                        End If

                        obj_CapoAnimaleCDC.id_movimentazione_BDN = ID_Uscita

                        Select Case Motivo_Uscita
                            Case "D", "A", "Y", "2", "3", "4", "5", "6", "7", "W"
                                lista_capoAnimaleCDC_Morte.Add(obj_CapoAnimaleCDC)
                            Case "E", "V", "N", "1", "0", "J", "X", "H", "T", "R", "I"
                                lista_capoAnimaleCDC_Vendita.Add(obj_CapoAnimaleCDC)
                            Case "M", "Q"
                                lista_capoAnimaleCDC_Macellazione.Add(obj_CapoAnimaleCDC)
                            Case Else
                                lista_capoAnimaleCDC_Vendita.Add(obj_CapoAnimaleCDC)
                        End Select
                        listaMatricole_Scarico.Add(Matricola_Capo)
                    Catch ex As Exception

                    End Try

                Next

                If lista_capoAnimaleCDC_Morte.Count > 0 Then
                    For Each cauMorte In listaCausaliMorte
                        Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                        objAttivita.fine = AGRODATAFINE
                        objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_MORTE_ANIMALI, "")
                        objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                            .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                        }
                        objAttivita.fabbricatoCod = Sta_Num
                        objAttivita.modalita = cauMorte
                        objAttivita.inizio = CDate(uscita)
                        objAttivita.centriDiCosto = lista_capoAnimaleCDC_Morte

                        'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                        Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                        Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                              objParametriServer)
                        BloccaOperazione(Piva, Id_Agenda, True)
                        For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                            logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_MORTE_ANIMALI, capo.capoAnimale.matricola,
                                                 enumCausaliBDN.ImpMovUscitaBDN, capo.id_movimentazione_BDN,
                                                 capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                        Next

                    Next
                End If

                If lista_capoAnimaleCDC_Vendita.Count > 0 Then

                    Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                    objAttivita.fine = AGRODATAFINE
                    objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_VENDITA_ANIMALI, "")
                    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                        .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                    }
                    objAttivita.fabbricatoCod = Sta_Num
                    objAttivita.inizio = CDate(uscita)
                    objAttivita.centriDiCosto = lista_capoAnimaleCDC_Vendita

                    'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                    Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                          objParametriServer)
                    BloccaOperazione(Piva, Id_Agenda, True)
                    For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                        logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_VENDITA_ANIMALI, capo.capoAnimale.matricola,
                                             enumCausaliBDN.ImpMovUscitaBDN, capo.id_movimentazione_BDN,
                                             capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                    Next

                End If

                If lista_capoAnimaleCDC_Macellazione.Count > 0 Then

                    Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                    objAttivita.fine = AGRODATAFINE
                    objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_MACELLAZIONE_ANIMALI, "")
                    objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {
                        .primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)
                    }
                    objAttivita.fabbricatoCod = Sta_Num
                    objAttivita.inizio = CDate(uscita)
                    objAttivita.centriDiCosto = lista_capoAnimaleCDC_Macellazione

                    'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                    Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                    Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                          objParametriServer)
                    BloccaOperazione(Piva, Id_Agenda, True)
                    For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                        logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_MACELLAZIONE_ANIMALI, capo.capoAnimale.matricola,
                                             enumCausaliBDN.ImpMovUscitaBDN, capo.id_movimentazione_BDN,
                                             capo.capoAnimale.idCapo_BDN, "", "", True, GiasContext)
                    Next

                End If

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine scarico " & DateFiltered_ScaricoCapi.Count & " capi in data " & CDate(uscita).ToShortDateString(),
                                  CustomLOGParams:=customLOGParams)

            Catch ex As ExpiredTokenBDNException
                Throw ex
            Catch ex As Exception
                Dim msgEx = ex.Message
                If ex.InnerException IsNot Nothing Then
                    msgEx &= " Inner Exception:" & ex.InnerException.Message
                End If

                objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Errore scarico in data " & CDate(uscita).ToShortDateString() & vbCrLf & " Errore:" & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                                  CustomLOGParams:=customLOGParams)
            End Try
        Next

        Return listaMatricole_Scarico

    End Function

    ''' <summary>
    ''' Esegue un'operazione di incremento consistenze capi leggendoli dal Modello4 
    ''' </summary>
    ''' <param name="listamatricole_BDN"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Codice_Asl"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="Azienda_Id"></param>
    ''' <param name="IdFiscale_Allev"></param>
    ''' <param name="Allev_Id"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="IdFiscale_Detentore"></param>
    ''' <param name="Raggruppamento_Cod"></param>
    ''' <returns></returns>
    Public Function Leggi_Modelli4Ingresso(ByVal listamatricole_BDN As List(Of String),
                                           lista_ConsistenzaStalla_DB As List(Of IDictionary(Of String, Object)),
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Sta_Num As Integer,
                                           ByVal Codice_Asl As String,
                                           ByVal Codice_Azienda As String,
                                           ByVal Azienda_Id As String,
                                           ByVal IdFiscale_Allev As String,
                                           ByVal Allev_Id As String,
                                           ByVal Spe_Cod As String,
                                           ByVal IdFiscale_Detentore As String,
                                           ByVal Raggruppamento_Cod As Integer,
                                           ByVal dataInizio As Date,
                                           ByVal dataFine As Date,
                                           Optional ListaModelli4 As List(Of Tuple(Of String, String)) = Nothing,
                                           Optional nuovaSincronizzizazioneModelloo4 As Boolean = False) As List(Of String)
        Dim listaMatricoleCapi_Carico As New List(Of String)

        'Dim listaModelli_Ingresso As New List(Of CaricaModelli4_Response)

        Dim listaModelli_Ingresso = Leggi_ModelliIngressoLista(Codice_Asl, Codice_Azienda, IdFiscale_Allev, Spe_Cod, IdFiscale_Detentore, ListaModelli4)
        'If Not IsNothing(ListaModelli4) Then
        '    listaModelli_Ingresso = listaModelli_Ingresso.
        '        Where(Function(modello) ListaModelli4.
        '                  Any(Function(tuple) tuple.Item1 = modello.documentoId AndAlso tuple.Item2 = modello.prenotazioneId)).
        '        ToList()

        'End If


        Try

            If IsNothing(listaModelli_Ingresso) OrElse listaModelli_Ingresso.Count = 0 Then
                Exit Try
            End If

            For Each modello In listaModelli_Ingresso
                Try
                    'seleziona i capi che risultano in ingresso nel Modello4 recuperando i dati da BDN
                    Dim listaMatricole_Carico_Modello4 = modello.listaMatricoleCapi.Except(listamatricole_BDN).ToList()

                    If Not IsNothing(listaMatricole_Carico_Modello4) AndAlso listaMatricole_Carico_Modello4.Count > 0 Then


                        Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                        objAttivita.fine = AGRODATAFINE
                        objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_ACQUISTO_ANIMALI, "")
                        objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {.primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)}
                        objAttivita.fabbricatoCod = Sta_Num

                        Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)


                        Dim data_Arrivo = getDataArrivo(modello.dataUscita, modello.oraPartenza, modello.durataViaggio)
                        objAttivita.inizio = data_Arrivo

                        If data_Arrivo > DateTime.Now Then
                            Continue For
                        End If


                        For Each mat In listaMatricole_Carico_Modello4
                            Dim Matricola_Capo As String = mat
                            Dim Motivo_Ingresso As String = ""
                            Dim Motivo_Uscita As String = ""
                            Dim ID_Ingresso As Integer = 0
                            Dim ID_Uscita As Integer = 0

                            'Esiste un movimento di scarico sulla stessa stalla superiore a questa data.
                            If EsisteMovimentoScarico(mat, data_Arrivo, Piva, Sa_Cod, Sta_Num) Then
                                Continue For
                            End If

                            If esisteMatricolaInStallaESistemazioneProprietario(Piva, Sa_Cod, Sta_Num, Matricola_Capo, IdFiscale_Allev, IdFiscale_Detentore) Then
                                Continue For
                            End If

                            Dim Ingresso_Modello4_Prenotazione = modello.prenotazioneId
                            Dim Ingresso_Modello4_Numero = modello.numModello
                            Dim obj_CapoAnimaleCDC As CapoAnimaleCDC = SincronizzaAnimale(Piva, Sa_Cod, Sta_Num, Codice_Asl, Codice_Azienda,
                                                                                          IdFiscale_Allev, Allev_Id, Spe_Cod, IdFiscale_Detentore,
                                                                                          Raggruppamento_Cod, modello, Matricola_Capo, Motivo_Ingresso,
                                                                                          Motivo_Uscita, ID_Ingresso, ID_Uscita, Ingresso_Modello4_Prenotazione,
                                                                                          Ingresso_Modello4_Numero, data_Arrivo)


                            lista_capoAnimaleCDC.Add(obj_CapoAnimaleCDC)

                            'matricole dei capi che scarica col modello4
                            listaMatricoleCapi_Carico.Add(Matricola_Capo)
                        Next


                        If lista_capoAnimaleCDC.Count = 0 Then
                            Continue For
                        End If

                        objAttivita.centriDiCosto = lista_capoAnimaleCDC

                        'TODO il lav_cod varia in base a destinazione?
                        Dim listaParamsExtra_Attivita As New List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita)
                        listaParamsExtra_Attivita.Add(New AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita With {
                                                      .operazione = New AgronicaCoreModelsSTD.attivita.Lavorazione(LAVCOD_ACQUISTO_ANIMALI),
                                                      .key = "sincro_modello4",
                                                      .value = JsonConvert.SerializeObject(modello)
                                                      })

                        'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                        Dim datiModello4DB = obj_ZooAnimali_R.LeggiModello4Inviati(Piva, modello.numModello, modello.prenotazioneId, objParametriServer)
                        Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                        Dim Id_Agenda As Integer
                        If datiModello4DB.Rows.Count > 0 Then
                            'Se il modello 4 era gia' stato importato aggiorno la data modifica sulle varie righe interessate e aggiungo animali
                            Id_Agenda = obj_AttivitaZooToAgenda_w.AggiornaAttivitaZootecnica(objAttivita,
                                                                                             datiModello4DB(0),
                                                                                                  objParametriServer,
                                                                                                  Nothing,
                                                                                                  listaParamsExtra_Attivita)
                        Else
                            Id_Agenda = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                              objParametriServer,
                                                                                                              Nothing,
                                                                                                              listaParamsExtra_Attivita)
                        End If

                        Dim jObjModello = JsonConvert.SerializeObject(modello)
                        For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                            logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_ACQUISTO_ANIMALI, capo.capoAnimale.matricola,
                                                 enumCausaliBDN.ImpModello4Ingresso, capo.id_movimentazione_BDN, capo.capoAnimale.ingresso_modello4_numero,
                                                 Nothing, jObjModello.ToString, True, GiasContext)
                        Next

                        objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine carico " & modello.numModello & " con " & listaMatricole_Carico_Modello4.Count & " capi in data " & CDate(modello.dataUscita).ToShortDateString() & ": " & String.Join(",", listaMatricole_Carico_Modello4),
                                  CustomLOGParams:=customLOGParams)

                    End If

                    If Not nuovaSincronizzizazioneModelloo4 Then
                        'TODO eliminare da GIAS I capi che non fanno più parte di questo modello 4 ma che erano stati importati
                        Dim modello4Numero As String = modello.numModello
                        Dim capiModelloGias As List(Of String) = (From z In GiasContext.Zoo_Animali Where z.PIVA = Piva And z.Modello4_Ingresso = modello4Numero Select z.Matricola).ToList
                        Dim matricoleDaCancellare As List(Of String) = capiModelloGias.Except(listamatricole_BDN).ToList

                        For Each matricola In matricoleDaCancellare
                            Dim movimentiCaricoDB = (From a In GiasContext.Agenda
                                                     Join m In GiasContext.Movimenti On a.Id_Agenda Equals m.Id_Agenda
                                                     Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And m.Id_Mov Equals md.Id_Mov
                                                     Join mdest In GiasContext.Mov_Destinazioni On mdest.Id_Agenda Equals md.Id_Agenda And mdest.Id_Mov Equals md.Id_Mov And mdest.Id_Mov_Det Equals md.Id_Mov_Det
                                                     Join ragg In GiasContext.Stalla_Raggruppamenti On mdest.Piva Equals ragg.PIVA And mdest.Sa_Cod Equals ragg.sa_cod And mdest.Id_Destinazione Equals ragg.Raggruppamento_Cod
                                                     Join zoo In GiasContext.Zoo_Animali On md.Cod_Progetto Equals zoo.Cod_Progetto
                                                     Where m.Cau_Mov = CAU_CARICO_CONSISTENZE And
                                                         md.Elem_Cod = 300 And
                                                         ragg.sa_cod = Sa_Cod And
                                                         ragg.STA_NUM = Sta_Num And
                                                         zoo.Matricola = matricola).ToList

                            If movimentiCaricoDB.Count > 0 Then

                                Dim cod_progetto As Integer = movimentiCaricoDB(0).zoo.Cod_Progetto
                                objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(Piva, cod_progetto, objParametriServer, GiasContext, False)

                            End If

                        Next
                    End If

                Catch ex As ExpiredTokenBDNException
                    Throw ex
                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(modello)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore importazione modello 4 ingresso " & ex.Message & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
                End Try
            Next

        Catch ex As Exception

        End Try


        Try
            Dim dataLastWeek As Date = Date.Now.AddDays(-7)
            Dim listaModelliCaricati = (From c In lista_ConsistenzaStalla_DB Where c("Validita_Inizio") >= dataLastWeek And
                                                                                   c("CF_PROPRIETARIO") = IdFiscale_Allev And
                                                                                   c("Modello4_Ingresso_Numero") <> ""
                                        Select CStr(c("Modello4_Ingresso_Numero"))).Distinct.ToList
            Dim listaModelli_Ingresso_str = (From m In listaModelli_Ingresso Select m.numModello).ToList

            Dim listaModelli4Cancellati = listaModelliCaricati.Except(listaModelli_Ingresso_str).ToList
            Dim a = 0

        Catch ex As Exception

        End Try

        Return listaMatricoleCapi_Carico

    End Function

    Private Function SincronizzaAnimale(Piva As String, Sa_Cod As Integer, Sta_Num As Integer,
                                   ByRef Codice_Asl As String, Codice_Azienda As String,
                                   IdFiscale_Allev As String, Allev_Id As String, Spe_Cod As String,
                                   IdFiscale_Detentore As String, Raggruppamento_Cod As Integer, modello As CaricaModelli4_Response,
                                   Matricola_Capo As String, ByRef Motivo_Ingresso As String, ByRef Motivo_Uscita As String,
                                   ByRef ID_Ingresso As Integer, ByRef ID_Uscita As Integer,
                                   Optional ByVal Ingresso_Modello4_Prenotazione As String = "", Optional ByVal Ingresso_Modello4_Numero As String = "", Optional data_Arrivo As Date = AGRODATAINIZIO) As CapoAnimaleCDC
        'ByRef obj_CapoAnimaleCDC As CapoAnimaleCDC
        Dim obj_CapoAnimaleCDC = sincronizzatoreAnimale.SincronizzaAnimale(Piva,
                                                                           Sa_Cod,
                                                                           Sta_Num,
                                                                           Codice_Azienda,
                                                                           IdFiscale_Allev,
                                                                           Spe_Cod,
                                                                           Allev_Id,
                                                                           Matricola_Capo,
                                                                           IdFiscale_Detentore,
                                                                           Motivo_Ingresso,
                                                                           Motivo_Uscita,
                                                                           ID_Ingresso,
                                                                           ID_Uscita,
                                                                           Codice_Asl,
                                                                           objLog,
                                                                           customLOGParams,
                                                                           Ingresso_Modello4_Prenotazione,
                                                                           Ingresso_Modello4_Numero)



        'aggiungo il Raggruppamento_Cod di Stalla_Raggruppamenti
        obj_CapoAnimaleCDC.sottogruppoStalla_ingresso = New anagrafiche.SottogruppoStalla
        obj_CapoAnimaleCDC.sottogruppoStalla_ingresso.codice = Raggruppamento_Cod

        obj_CapoAnimaleCDC.sottogruppoStalla_uscita = New anagrafiche.SottogruppoStalla
        obj_CapoAnimaleCDC.sottogruppoStalla_uscita.codice = Raggruppamento_Cod

        If data_Arrivo <> AGRODATAINIZIO AndAlso obj_CapoAnimaleCDC.capoAnimale.validita.inizio <> data_Arrivo Then
            obj_CapoAnimaleCDC.capoAnimale.validita.inizio = data_Arrivo
            obj_CapoAnimaleCDC.capoAnimale.esercizi(0).validita.inizio = data_Arrivo
        End If

        'con ingresso tramite Modello4 valorizza Modello4_Ingresso e non Id_Capo_BDN
        obj_CapoAnimaleCDC.capoAnimale.idCapo_BDN = 0
        obj_CapoAnimaleCDC.capoAnimale.ingresso_mm_id = modello.documentoId
        obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_numero = modello.numModello
        obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_prenotazione = modello.prenotazioneId
        obj_CapoAnimaleCDC.capoAnimale.ingresso_modello4_data_prenotazione = modello.dataDocumento
        obj_CapoAnimaleCDC.capoAnimale.codiceAziendaFornitore = modello.codAzienda_Prov
        obj_CapoAnimaleCDC.capoAnimale.validita.fine = AGRODATAFINE
        If obj_CapoAnimaleCDC.capoAnimale.esercizi.Count = 1 Then
            obj_CapoAnimaleCDC.capoAnimale.esercizi(0).validita.fine = AGRODATAFINE
        End If
        obj_CapoAnimaleCDC.id_movimentazione_BDN = ID_Ingresso

        Return obj_CapoAnimaleCDC
    End Function

    Public Function leggiAllevamento(Codice_Azienda As String, IdFiscale_Allev As String, Spe_Cod As String)
        Dim aa = wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Allev, Spe_Cod)
        Return aa
    End Function
    Private Function Leggi_ModelliIngresso(Codice_Asl As String,
                                           Codice_Azienda As String,
                                           IdFiscale_Allev As String,
                                           Spe_Cod As String,
                                           IdFiscale_Detentore As String,
                                           dataInizio As Date,
                                           dataFine As Date) As List(Of CaricaModelli4_Response)
        Dim listaModelli_Ingresso As New List(Of CaricaModelli4_Response)
        Dim dataA = Date.Now
        Dim dataDa = dataA.AddDays(-7)
        'Dim aa As DataTable = Nothing
        'wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Allev, Spe_Cod)

        'Dim id_Allevamento = CInt(aa(0)("ALLEV_ID"))
        'Dim PROP_ID_FISCALE = CStr(aa.Rows(0)("PROP_ID_FISCALE"))
        'Dim DETEN_ID_FISCALE = CStr(aa.Rows(0)("DETEN_ID_FISCALE"))
        Dim id_Allevamento = 0
        Dim PROP_ID_FISCALE = ""
        Dim DETEN_ID_FISCALE = ""

        'controlla i capi in entrata con Modello4 nell'arco di un mese
        'While True
        '    If dataDa < Date.Now.AddDays(-8) Then
        '        Exit While
        '    End If

        'Con codice asl
        Dim dtModelli4_Ingresso = wsInterrogazioniModello4.getListaPrenotazioniModelli(p_data_da:=dataInizio,
                                                                                           p_data_a:=dataFine,
                                                                                           p_asl_codice_prov:="",
                                                                                           p_azienda_codice_prov:="",
                                                                                           p_tipo_dest:="AL",
                                                                                           p_stato_modello:="C",
                                                                                           p_asl_codice_dest:=Codice_Asl,
                                                                                           p_codice_struttura_dest:=Codice_Azienda,
                                                                                           p_regione_codice_dest:="",
                                                                                           True)


        If dtModelli4_Ingresso IsNot Nothing Then
            Try
                If dtModelli4_Ingresso.Rows.Count > 0 Then

                    If id_Allevamento = 0 AndAlso PROP_ID_FISCALE = "" AndAlso DETEN_ID_FISCALE = "" Then ' Leggo solo la prima volta e se sno presenti dei modelli 4
                        Dim aa = wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Allev, Spe_Cod)

                        id_Allevamento = CInt(aa(0)("ALLEV_ID"))
                        PROP_ID_FISCALE = CStr(aa.Rows(0)("PROP_ID_FISCALE"))
                        DETEN_ID_FISCALE = CStr(aa.Rows(0)("DETEN_ID_FISCALE"))
                    End If
                    Dim modelli = Carica_Modelli4Ingresso(dtModelli4_Ingresso, IdFiscale_Allev, Spe_Cod,
                                                              Codice_Azienda, IdFiscale_Detentore, id_Allevamento, PROP_ID_FISCALE, DETEN_ID_FISCALE)
                    listaModelli_Ingresso.AddRange(modelli)
                End If

                'valorizza la data di prenotazione del modello
                For i = 0 To dtModelli4_Ingresso.Rows.Count - 1
                    For j = 0 To listaModelli_Ingresso.Count - 1
                        If dtModelli4_Ingresso(i)("PRENOTAZIONE_ID") = listaModelli_Ingresso(j).prenotazioneId Then
                            listaModelli_Ingresso(j).dataPrenotazione = dtModelli4_Ingresso(i)("DT_MODELLO")
                            Exit For
                        End If
                    Next
                Next
            Catch ex As Exception
                Dim modelloStr = JsonConvert.SerializeObject(dtModelli4_Ingresso)
                objLog.Scrivi_LOG(objParametriServer,
System.Reflection.MethodBase.GetCurrentMethod().Name,
"Errore caricamento modello 4 ingresso " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & ": " & modelloStr,
CustomLOGParams:=customLOGParams)
            End Try
        End If

        '    dataA = dataDa
        '    dataDa = dataA.AddDays(-2)

        'End While

        If Not IsNothing(listaModelli_Ingresso) AndAlso listaModelli_Ingresso.Any Then
            listaModelli_Ingresso.Sort(Function(m1, m2)
                                           Return m1.dataIngresso < m2.dataIngresso
                                       End Function)
        End If
        Return listaModelli_Ingresso
    End Function

    Private Function Leggi_ModelliIngressoLista(Codice_Asl As String,
                                           Codice_Azienda As String,
                                           IdFiscale_Allev As String,
                                           Spe_Cod As String,
                                           IdFiscale_Detentore As String,
                                           ListaModelli4 As List(Of Tuple(Of String, String))) As List(Of CaricaModelli4_Response)
        Dim listaModelli_Ingresso As New List(Of CaricaModelli4_Response)
        Dim dataA = Date.Now
        Dim dataDa = dataA.AddDays(-7)
        'Dim aa As DataTable = Nothing
        'wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Allev, Spe_Cod)
        For Each elem In ListaModelli4
            Dim prenotazioneId = elem.Item2
            Dim modello4 = wsInterrogazioniModello4.caricaModello4(prenotazioneId, Spe_Cod, False)
            listaModelli_Ingresso.Add(modello4)

        Next

        Return listaModelli_Ingresso

        If Not IsNothing(listaModelli_Ingresso) AndAlso listaModelli_Ingresso.Any Then
            listaModelli_Ingresso.Sort(Function(m1, m2)
                                           Return m1.dataIngresso < m2.dataIngresso
                                       End Function)
        End If

        Return listaModelli_Ingresso
    End Function

    Public Function Leggi_Modelli4IngressoPascolo(ByVal listamatricole_BDN As List(Of String),
                                           lista_ConsistenzaStalla_DB As List(Of IDictionary(Of String, Object)),
                                           ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Sta_Num As Integer,
                                           ByVal Codice_Asl As String,
                                           ByVal Codice_Azienda As String,
                                           ByVal Azienda_Id As String,
                                           ByVal IdFiscale_Allev As String,
                                           ByVal Allev_Id As String,
                                           ByVal Spe_Cod As String,
                                           ByVal IdFiscale_Detentore As String,
                                           ByVal Raggruppamento_Cod As Integer,
                                           ByVal dataInizio As Date,
                                           ByVal dataFine As Date,
                                           Optional ListaModelli4 As List(Of Tuple(Of String, String)) = Nothing,
                                           Optional nuovaSincronizzizazioneModelloo4 As Boolean = False) As List(Of String)

        Dim listaMatricoleCapi_Carico As New List(Of String)

        Dim listaModelli_Ingresso As List(Of CaricaModelli4_Response) = Leggi_Modelli4IngressoPascoloLista(Codice_Asl, Codice_Azienda, IdFiscale_Allev, Spe_Cod, IdFiscale_Detentore, ListaModelli4)
        'If Not IsNothing(ListaModelli4) Then
        '    listaModelli_Ingresso = listaModelli_Ingresso.
        '        Where(Function(modello) ListaModelli4.
        '                  Any(Function(tuple) tuple.Item1 = modello.documentoId AndAlso tuple.Item2 = modello.prenotazioneId)).
        '        ToList()

        'End If

        Try

            If IsNothing(listaModelli_Ingresso) OrElse listaModelli_Ingresso.Count = 0 Then
                Exit Try
            End If

            For Each modello In listaModelli_Ingresso
                Try
                    'seleziona i capi che risultano in ingresso nel Modello4 recuperando i dati da BDN
                    Dim listaMatricole_Carico_Modello4 = modello.listaMatricoleCapi.Except(listamatricole_BDN).ToList()

                    If Not IsNothing(listaMatricole_Carico_Modello4) AndAlso listaMatricole_Carico_Modello4.Count > 0 Then


                        Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                        objAttivita.fine = AGRODATAFINE
                        objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_ACQUISTO_ANIMALI, "")
                        objAttivita.centroAziendale = New anagrafiche.CentroAziendale With {.primaryKey = New anagrafiche.CentroAziendale.PK(Sa_Cod, Piva)}
                        objAttivita.fabbricatoCod = Sta_Num
                        Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)


                        Dim data_Arrivo = getDataArrivo(modello.dataUscita, modello.oraPartenza, modello.durataViaggio)

                        objAttivita.inizio = data_Arrivo

                        If data_Arrivo > DateTime.Now Then
                            Continue For
                        End If


                        For Each mat In listaMatricole_Carico_Modello4
                            Dim Matricola_Capo As String = mat
                            Dim Motivo_Ingresso As String = ""
                            Dim Motivo_Uscita As String = ""
                            Dim ID_Ingresso As Integer = 0
                            Dim ID_Uscita As Integer = 0

                            'Esiste un movimento di scarico sulla stessa stalla superiore a questa data.
                            If EsisteMovimentoScarico(mat, data_Arrivo, Piva, Sa_Cod, Sta_Num) Then
                                Continue For
                            End If

                            If esisteMatricolaInStallaESistemazioneProprietario(Piva, Sa_Cod, Sta_Num, Matricola_Capo, IdFiscale_Allev, IdFiscale_Detentore) Then
                                Continue For
                            End If
                            Dim Ingresso_Modello4_Prenotazione = modello.prenotazioneId
                            Dim Ingresso_Modello4_Numero = modello.numModello

                            Dim obj_CapoAnimaleCDC As CapoAnimaleCDC = SincronizzaAnimale(Piva, Sa_Cod, Sta_Num, Codice_Asl, Codice_Azienda,
                                                                                          IdFiscale_Allev, Allev_Id, Spe_Cod, IdFiscale_Detentore,
                                                                                          Raggruppamento_Cod, modello, Matricola_Capo, Motivo_Ingresso,
                                                                                          Motivo_Uscita, ID_Ingresso, ID_Uscita, Ingresso_Modello4_Prenotazione,
                                                                                          Ingresso_Modello4_Numero, data_Arrivo)

                            lista_capoAnimaleCDC.Add(obj_CapoAnimaleCDC)

                            'matricole dei capi che scarica col modello4
                            listaMatricoleCapi_Carico.Add(Matricola_Capo)

                        Next

                        If lista_capoAnimaleCDC.Count = 0 Then
                            Continue For
                        End If

                        objAttivita.centriDiCosto = lista_capoAnimaleCDC
                        Dim jObjModello = JsonConvert.SerializeObject(modello)

                        'TODO il lav_cod varia in base a destinazione?
                        Dim listaParamsExtra_Attivita As New List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita)
                        listaParamsExtra_Attivita.Add(New AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita With {
                                                        .operazione = New AgronicaCoreModelsSTD.attivita.Lavorazione(LAVCOD_ACQUISTO_ANIMALI),
                                                        .key = "sincro_modello4",
                                                        .value = jObjModello
                                                      })

                        'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                        Dim datiModello4DB = obj_ZooAnimali_R.LeggiModello4Inviati(Piva, modello.numModello, modello.prenotazioneId, objParametriServer)
                        Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                        Dim Id_Agenda As Integer
                        If datiModello4DB.Rows.Count > 0 Then
                            'Se il modello 4 era gia' stato importato aggiorno la data modifica sulle varie righe interessate e aggiungo animali
                            Id_Agenda = obj_AttivitaZooToAgenda_w.AggiornaAttivitaZootecnica(objAttivita,
                                                                                             datiModello4DB(0),
                                                                                                  objParametriServer,
                                                                                                  Nothing,
                                                                                                  listaParamsExtra_Attivita)
                        Else
                            Id_Agenda = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
                                                                                                              objParametriServer,
                                                                                                              Nothing,
                                                                                                              listaParamsExtra_Attivita)
                        End If

                        For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                            logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, LAVCOD_ACQUISTO_ANIMALI, capo.capoAnimale.matricola,
                                                 enumCausaliBDN.ImpModello4Ingresso, capo.id_movimentazione_BDN, capo.capoAnimale.ingresso_modello4_numero,
                                                 Nothing, jObjModello.ToString, True, GiasContext)
                        Next

                        objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine carico " & modello.numModello & " con " & listaMatricole_Carico_Modello4.Count & " capi in data " & CDate(modello.dataUscita).ToShortDateString() & ": " & String.Join(",", listaMatricole_Carico_Modello4),
                                  CustomLOGParams:=customLOGParams)

                    End If

                    'TODO eliminare da GIAS I capi che non fanno più parte di questo modello 4 ma che erano stati importati

                    If Not nuovaSincronizzizazioneModelloo4 Then

                        Dim modello4Numero As String = modello.numModello
                        Dim capiModelloGias As List(Of String) = (From z In GiasContext.Zoo_Animali Where z.PIVA = Piva And z.Modello4_Ingresso = modello4Numero Select z.Matricola).ToList
                        Dim matricoleDaCancellare As List(Of String) = capiModelloGias.Except(listamatricole_BDN).ToList

                        For Each matricola In matricoleDaCancellare
                            Dim movimentiCaricoDB = (From a In GiasContext.Agenda
                                                     Join m In GiasContext.Movimenti On a.Id_Agenda Equals m.Id_Agenda
                                                     Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And m.Id_Mov Equals md.Id_Mov
                                                     Join mdest In GiasContext.Mov_Destinazioni On mdest.Id_Agenda Equals md.Id_Agenda And mdest.Id_Mov Equals md.Id_Mov And mdest.Id_Mov_Det Equals md.Id_Mov_Det
                                                     Join ragg In GiasContext.Stalla_Raggruppamenti On mdest.Piva Equals ragg.PIVA And mdest.Sa_Cod Equals ragg.sa_cod And mdest.Id_Destinazione Equals ragg.Raggruppamento_Cod
                                                     Join zoo In GiasContext.Zoo_Animali On md.Cod_Progetto Equals zoo.Cod_Progetto
                                                     Where m.Cau_Mov = CAU_CARICO_CONSISTENZE And
                                                     md.Elem_Cod = 300 And
                                                     ragg.sa_cod = Sa_Cod And
                                                     ragg.STA_NUM = Sta_Num And
                                                     zoo.Matricola = matricola).ToList

                            If movimentiCaricoDB.Count > 0 Then

                                Dim cod_progetto As Integer = movimentiCaricoDB(0).zoo.Cod_Progetto
                                objScrivi_Zoo.Elimina_Animale_Con_Operazione_Carico(Piva, cod_progetto, objParametriServer, GiasContext, False)

                            End If

                        Next

                    End If
                Catch ex As ExpiredTokenBDNException
                    Throw ex
                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(modello)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore importazione modello 4 ingresso " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & ": " & modelloStr,
                          CustomLOGParams:=customLOGParams)
                End Try
            Next

        Catch ex As Exception

        End Try


        Try
            Dim dataLastWeek As Date = Date.Now.AddDays(-7)
            Dim listaModelliCaricati = (From c In lista_ConsistenzaStalla_DB Where c("Validita_Inizio") >= dataLastWeek And
                                                                                   c("CF_PROPRIETARIO") = IdFiscale_Allev And
                                                                                   c("Modello4_Ingresso_Numero") <> ""
                                        Select CStr(c("Modello4_Ingresso_Numero"))).Distinct.ToList
            Dim listaModelli_Ingresso_str = (From m In listaModelli_Ingresso Select m.numModello).ToList

            Dim listaModelli4Cancellati = listaModelliCaricati.Except(listaModelli_Ingresso_str).ToList
            Dim a = 0

        Catch ex As Exception

        End Try

        Return listaMatricoleCapi_Carico

    End Function

    Private Function Leggi_Modelli4IngressoPascoloLista(Codice_Asl As String,
                                           Codice_Azienda As String,
                                           IdFiscale_Allev As String,
                                           Spe_Cod As String,
                                           IdFiscale_Detentore As String,
                                           ListaModelli4 As List(Of Tuple(Of String, String))) As List(Of CaricaModelli4_Response)
        Dim listaModelli_Ingresso As New List(Of CaricaModelli4_Response)
        Dim dataA = Date.Now
        Dim dataDa = dataA.AddDays(-7)
        'Dim aa As DataTable = Nothing
        'wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Allev, Spe_Cod)
        For Each elem In ListaModelli4
            Dim prenotazioneId = elem.Item2
            Dim modello4 = Carica_Modello4PascoloIngresso(prenotazioneId, Spe_Cod)
            listaModelli_Ingresso.Add(modello4)

        Next

        Return listaModelli_Ingresso

        If Not IsNothing(listaModelli_Ingresso) AndAlso listaModelli_Ingresso.Any Then
            listaModelli_Ingresso.Sort(Function(m1, m2)
                                           Return m1.dataIngresso < m2.dataIngresso
                                       End Function)
        End If

        Return listaModelli_Ingresso
    End Function

    Private Function Leggi_ModelliIngressoPascolo(Codice_Asl As String,
                                           Codice_Azienda As String,
                                           IdFiscale_Allev As String,
                                           Spe_Cod As String,
                                           IdFiscale_Detentore As String,
                                           dataInizio As Date,
                                           dataFine As Date) As List(Of CaricaModelli4_Response)
        Dim listaModelli_Ingresso As New List(Of CaricaModelli4_Response)

        Dim dataA = Date.Now
        Dim dataDa = dataA.AddDays(-7)

        'controlla i capi in entrata con Modello4 nell'arco di un mese
        'While True
        '    If dataDa < Date.Now.AddDays(-8) Then
        '        Exit While
        '    End If

        'Con codice asl
        Dim dtModelli4_Ingresso = wsInterrogazioniModello4.getListaPrenotazioniModelliPascolo(p_data_da:=dataInizio,
                                                                                           p_data_a:=dataFine,
                                                                                           p_asl_codice_prov:="",
                                                                                           p_pascolo_codice_prov:="",
                                                                                           p_tipo_dest:="AL",
                                                                                           p_stato_modello:="C",
                                                                                           p_asl_codice_dest:=Codice_Asl,
                                                                                           p_codice_struttura_dest:=Codice_Azienda,
                                                                                           p_regione_codice_dest:="")

        If dtModelli4_Ingresso IsNot Nothing Then
            Try
                listaModelli_Ingresso.AddRange(Carica_Modelli4PascoloIngresso(dtModelli4_Ingresso, IdFiscale_Allev, Spe_Cod, Codice_Azienda, IdFiscale_Detentore))

                'valorizza la data di prenotazione del modello
                For i = 0 To dtModelli4_Ingresso.Rows.Count - 1
                    For j = 0 To listaModelli_Ingresso.Count - 1
                        If dtModelli4_Ingresso(i)("PRENOTAZIONE_ID") = listaModelli_Ingresso(j).prenotazioneId Then
                            listaModelli_Ingresso(j).dataPrenotazione = dtModelli4_Ingresso(i)("DT_MODELLO")
                            Exit For
                        End If
                    Next
                Next
            Catch ex As Exception
                Dim modelloStr = JsonConvert.SerializeObject(dtModelli4_Ingresso)
                objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore caricamento modello 4 ingresso " & ex.Message & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
            End Try
        End If

        '    dataA = dataDa
        '    dataDa = dataA.AddDays(-2)

        'End While

        listaModelli_Ingresso.Sort(Function(m1, m2)
                                       Return m1.dataIngresso < m2.dataIngresso
                                   End Function)
        Return listaModelli_Ingresso
    End Function
    Private Function getDataArrivo(data_partenza As Date, ora_partenza As String, durataViaggio As String) As DateTime
        Dim data_arrivo As DateTime

        Dim data_ora_partenza As DateTime = data_partenza
        Try
            If ora_partenza.Contains(":") Then
                Dim hpartenza As Integer = ora_partenza.Split(":")(0)
                Dim mpartenza As Integer = 0
                If ora_partenza.Split(":").Length > 1 Then
                    mpartenza = ora_partenza.Split(":")(1)
                End If
                If hpartenza < 24 Then
                    data_ora_partenza = data_ora_partenza.AddHours(hpartenza)
                End If

                If mpartenza < 60 Then
                    data_ora_partenza = data_ora_partenza.AddMinutes(mpartenza)
                End If
            End If
        Catch ex As Exception

        End Try

        Dim gViaggio As Integer = durataViaggio.Split("g")(0)
        Dim hViaggio As Integer = (durataViaggio.Split("g")(1)).Split("h")(0)
        Dim mViaggio As Integer = (durataViaggio.Split("g")(1)).Split("h")(1).Replace("m", "")

        data_arrivo = data_ora_partenza
        data_arrivo = data_arrivo.AddDays(gViaggio)
        data_arrivo = data_arrivo.AddHours(hViaggio)
        data_arrivo = data_arrivo.AddMinutes(mViaggio)

        Return data_arrivo
    End Function

    ''' <summary>
    ''' Esegue un'operazione di decremento consistenze capi leggendoli dal Modello4
    ''' </summary>
    ''' <param name="listaConsistenzaDB"></param>
    ''' <param name="Piva"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Codice_Asl"></param>
    ''' <param name="Codice_Azienda"></param>
    ''' <param name="Azienda_Id"></param>
    ''' <param name="Allev_IdFiscale"></param>
    ''' <param name="Allev_Id"></param>
    ''' <param name="Spe_Cod"></param>
    ''' <param name="IdFiscale_Detentore"></param>
    ''' <returns></returns>
    Public Function Leggi_Modelli4Uscita(ByVal listaConsistenzaDB As List(Of IDictionary(Of String, Object)),
                                         ByVal Piva As String,
                                         ByVal Sa_Cod As Integer,
                                         ByVal Sta_Num As Integer,
                                         ByVal Codice_Asl As String,
                                         ByVal Codice_Azienda As String,
                                         ByVal Azienda_Id As String,
                                         ByVal Allev_IdFiscale As String,
                                         ByVal Allev_Id As String,
                                         ByVal Spe_Cod As String,
                                         ByVal IdFiscale_Detentore As String) As List(Of String)
        Dim listaMatricoleCapi_Scarico As New List(Of String)
        Dim obj_ZooAnimali_R As New Zoo_Animali
        Dim listaModelli_Uscita As New List(Of CaricaModelli4_Response)

        Dim dataA = Date.Now
        Dim dataDa = dataA.AddDays(-7)

        'controlla i capi in uscita con Modello4 nell'arco di un mese
        While True
            If dataDa <= Date.Now.AddDays(-8) Then
                Exit While
            End If

            'Con codice asl
            Dim dtModelli4_Uscita = wsInterrogazioniModello4.getListaPrenotazioniModelli(p_data_da:=dataDa,
                                                                                         p_data_a:=dataA,
                                                                                         p_asl_codice_prov:=Codice_Asl,
                                                                                         p_azienda_codice_prov:=Codice_Azienda,
                                                                                         p_tipo_dest:="",
                                                                                         p_stato_modello:="C",
                                                                                         p_asl_codice_dest:="",
                                                                                         p_codice_struttura_dest:="",
                                                                                         p_regione_codice_dest:="",
                                                                                         True)

            If dtModelli4_Uscita IsNot Nothing Then
                Try
                    listaModelli_Uscita.AddRange(Carica_Modelli4Uscita(dtModelli4_Uscita, Allev_IdFiscale, Spe_Cod, Codice_Azienda, IdFiscale_Detentore))

                    'valorizza la data di prenotazione del modello
                    For i = 0 To dtModelli4_Uscita.Rows.Count - 1
                        For j = 0 To listaModelli_Uscita.Count - 1
                            If dtModelli4_Uscita(i)("PRENOTAZIONE_ID") = listaModelli_Uscita(j).prenotazioneId Then

                                If dtModelli4_Uscita.Columns.Contains("DT_MODELLO") AndAlso Not IsDBNull(dtModelli4_Uscita(i)("DT_MODELLO")) AndAlso dtModelli4_Uscita(i)("DT_MODELLO") <> "" AndAlso IsDate(dtModelli4_Uscita(i)("DT_MODELLO")) Then
                                    listaModelli_Uscita(j).dataPrenotazione = dtModelli4_Uscita(i)("DT_MODELLO")
                                ElseIf dtModelli4_Uscita.Columns.Contains("DT_RICHIESTA") AndAlso Not IsDBNull(dtModelli4_Uscita(i)("DT_RICHIESTA")) AndAlso dtModelli4_Uscita(i)("DT_RICHIESTA") <> "" AndAlso IsDate(dtModelli4_Uscita(i)("DT_RICHIESTA")) Then
                                    listaModelli_Uscita(j).dataPrenotazione = dtModelli4_Uscita(i)("DT_RICHIESTA")
                                ElseIf dtModelli4_Uscita.Columns.Contains("DT_USCITA") AndAlso Not IsDBNull(dtModelli4_Uscita(i)("DT_USCITA")) AndAlso dtModelli4_Uscita(i)("DT_USCITA") <> "" AndAlso IsDate(dtModelli4_Uscita(i)("DT_USCITA")) Then
                                    listaModelli_Uscita(j).dataPrenotazione = dtModelli4_Uscita(i)("DT_USCITA")
                                Else
                                    listaModelli_Uscita(j).dataPrenotazione = AGRODATAINIZIO
                                End If

                                Exit For
                            End If
                        Next
                    Next
                Catch ex As ExpiredTokenBDNException
                    Throw ex
                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(dtModelli4_Uscita)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore caricamento modello 4 uscita " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & ": " & modelloStr,
                          CustomLOGParams:=customLOGParams)
                End Try

            End If

            dataA = dataDa
            dataDa = dataA.AddDays(-10)

        End While

        listaModelli_Uscita.Sort(Function(m1, m2)
                                     Return m1.dataUscita < m2.dataUscita
                                 End Function)

        Try

            If IsNothing(listaModelli_Uscita) OrElse listaModelli_Uscita.Count = 0 Then
                Exit Try
            End If

            For Each modello In listaModelli_Uscita
                Try
                    'seleziona i capi che risultano scaricati nel Modello4 e che non sono ancora stati scaricati col Modello4,
                    'ma sono sincronizzati con la BDN
                    Dim listaMatricoleScarico_Modello4 = (From cp In listaConsistenzaDB
                                                          Where modello.listaMatricoleCapi.Contains(cp.Item("Matricola"))
                                                          Select cp).ToList()

                    If Not IsNothing(listaMatricoleScarico_Modello4) AndAlso listaMatricoleScarico_Modello4.Count > 0 Then
                        Dim lav_cod As Integer

                        Select Case modello.tipoDestinazione
                            Case "MACELLO"
                                lav_cod = LAVCOD_MACELLAZIONE_ANIMALI
                            Case Else
                                lav_cod = LAVCOD_VENDITA_ANIMALI
                        End Select

                        Dim objAttivita As New AgronicaCoreModelsSTD.attivita.Attivita
                        objAttivita.fine = AGRODATAFINE
                        objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(lav_cod, "")
                        objAttivita.centroAziendale = New anagrafiche.CentroAziendale(New anagrafiche.CentroAziendale.PK(0, Piva))

                        Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)

                        Dim listaCausaliMorte As New List(Of Integer)

                        For Each capo In listaMatricoleScarico_Modello4
                            Dim Matricola_Capo As String = capo("Matricola")
                            Dim Raggruppamento_Capo As String = capo("Raggruppamento_Cod")
                            Dim Cod_Animale As Integer = capo("Cod_Animale")



                            Dim obj_CapoAnimaleCDC As New AgronicaCoreModelsSTD.attivita.centri_di_costo.CapoAnimaleCDC
                            obj_CapoAnimaleCDC.codice = New AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto.CodeType(Sa_Cod)
                            obj_CapoAnimaleCDC.capoAnimale = New anagrafiche.CapoAnimale(Piva, Cod_Animale, Matricola_Capo)

                            obj_CapoAnimaleCDC.capoAnimale = obj_ZooAnimali_R.LeggiCapoAnimale(Cod_Animale, objParametriServer)

                            obj_CapoAnimaleCDC.capoAnimale.uscita_mm_id = modello.documentoId
                            obj_CapoAnimaleCDC.capoAnimale.uscita_modello4_numero = modello.numModello
                            obj_CapoAnimaleCDC.capoAnimale.uscita_modello4_prenotazione = modello.prenotazioneId
                            obj_CapoAnimaleCDC.capoAnimale.Codice_Azienda_Uscita = modello.codAzienda_Dest
                            obj_CapoAnimaleCDC.capoAnimale.uscita_modello4_data_prenotazione = modello.dataPrenotazione
                            obj_CapoAnimaleCDC.capoAnimale.codiceAziendaFornitore = modello.codAzienda_Prov

                            obj_CapoAnimaleCDC.capoAnimale.validita.fine = modello.dataUscita

                            If obj_CapoAnimaleCDC.capoAnimale.esercizi.Count = 1 Then
                                obj_CapoAnimaleCDC.capoAnimale.esercizi(0).validita.fine = modello.dataUscita
                            End If

                            obj_CapoAnimaleCDC.capoAnimale.specie = New metaschema.utilizzi.Specie(capo("SPE_COD"), capo("SPE_DES"))
                            obj_CapoAnimaleCDC.capoAnimale.razza = New metaschema.Razza(capo("RAZ_COD"), capo("RAZ_DES"))

                            'aggiunge il Raggruppamento_Cod di Stalla_Raggruppamenti
                            obj_CapoAnimaleCDC.sottogruppoStalla_ingresso = New anagrafiche.SottogruppoStalla
                            obj_CapoAnimaleCDC.sottogruppoStalla_ingresso.codice = Raggruppamento_Capo

                            obj_CapoAnimaleCDC.sottogruppoStalla_uscita = New anagrafiche.SottogruppoStalla
                            obj_CapoAnimaleCDC.sottogruppoStalla_uscita.codice = Raggruppamento_Capo

                            Try
                                Dim respDecesso As DataTable = wsAnagraficaCapo.getDecesso(Codice_Azienda, Allev_IdFiscale, Spe_Cod, Matricola_Capo)

                                If Not IsNothing(respDecesso) AndAlso respDecesso.Rows.Count > 0 Then
                                    Dim cauMorteBDN As String = respDecesso(0)("CAUMOR_CODICE")

                                    If Not IsDBNull(cauMorteBDN) AndAlso cauMorteBDN <> "" Then
                                        obj_CapoAnimaleCDC.capoAnimale.causaleMorte = GiasContext.Lista_Causali_Morte.Where(Function(row) row.Codice_BDN = cauMorteBDN).FirstOrDefault.Cod

                                        If Not listaCausaliMorte.Contains(obj_CapoAnimaleCDC.capoAnimale.causaleMorte) Then
                                            listaCausaliMorte.Add(obj_CapoAnimaleCDC.capoAnimale.causaleMorte)
                                        End If
                                    End If
                                End If
                            Catch ex As Exception

                            End Try

                            obj_CapoAnimaleCDC.id_movimentazione_BDN = sincronizzatoreAnimale.trovaID_Uscita(Allev_IdFiscale, Codice_Azienda, Matricola_Capo)

                            lista_capoAnimaleCDC.Add(obj_CapoAnimaleCDC)

                            'matricole dei capi che scarica col modello4
                            listaMatricoleCapi_Scarico.Add(Matricola_Capo)
                        Next

                        objAttivita.centriDiCosto = lista_capoAnimaleCDC
                        objAttivita.inizio = modello.dataUscita

                        If listaCausaliMorte.Count > 0 Then
                            objAttivita.job = New AgronicaCoreModelsSTD.attivita.Zootecnia(LAVCOD_MORTE_ANIMALI, "")
                        End If
                        'gestioneDestinazione_UscitaModello(Piva, Sa_Cod, modello)

                        Dim jObjModello = JsonConvert.SerializeObject(modello)

                        'TODO il lav_cod varia in base a provenienza?
                        Dim listaParamsExtra_Attivita As New List(Of AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita)
                        listaParamsExtra_Attivita.Add(New AgronicaCoreModelsSTD.attivita.Parametri_Aggiuntivi_Attivita With {
                                                        .operazione = New AgronicaCoreModelsSTD.attivita.Lavorazione(lav_cod),
                                                        .key = "sincro_modello4",
                                                        .value = jObjModello
                                                      })

                        'aggiunge l'attivita su DB (scrive Agenda, Movimenti e Zoo_Animali)
                        Dim obj_AttivitaZooToAgenda_w As New AttivitaZootecnicaToAgenda
                        Dim Id_Agenda As Integer = obj_AttivitaZooToAgenda_w.ScriviAttivitaZootecnicaToAgenda(objAttivita,
objParametriServer,
                                                                                                              Nothing,
                                                                                                              listaParamsExtra_Attivita)

                        For Each capo As CapoAnimaleCDC In objAttivita.centriDiCosto
                            logMovimentazioniBDN(Piva, Sa_Cod, Id_Agenda, lav_cod, capo.capoAnimale.matricola, enumCausaliBDN.ImpModello4Uscita,
capo.id_movimentazione_BDN, capo.capoAnimale.ingresso_modello4_numero,
Nothing, jObjModello.ToString, True, GiasContext)
                        Next

                        objLog.Scrivi_LOG(objParametriServer,
                                  System.Reflection.MethodBase.GetCurrentMethod().Name,
                                  "Fine scarico " & modello.numModello & " con " & listaMatricoleScarico_Modello4.Count & " capi in data " & CDate(modello.dataUscita).ToShortDateString() & ": " & String.Join(",", listaMatricoleScarico_Modello4),
                                  CustomLOGParams:=customLOGParams)

                    End If


                    'TODO Rimette 'in vita' i capi che erano stati scaricati dal modello 4 e che sono stati successivamente eliminati
                    Dim modello4Numero As String = modello.numModello
                    Dim capiModelloGias As List(Of String) = (From z In GiasContext.Zoo_Animali Where z.PIVA = Piva And z.Modello4_Uscita_Numero = modello4Numero Select z.Matricola).ToList
                    Dim listamatricole_BDN As List(Of String) = modello.listaMatricoleCapi
                    Dim matricoleDaRimettereInVita As List(Of String) = capiModelloGias.Except(listamatricole_BDN).ToList

                    For Each matricola In matricoleDaRimettereInVita
                        Dim movimentiScaricoDB = (From a In GiasContext.Agenda
                                                  Join m In GiasContext.Movimenti On a.Id_Agenda Equals m.Id_Agenda
                                                  Join md In GiasContext.Movimenti_dettagli On m.Id_Agenda Equals md.Id_Agenda And m.Id_Mov Equals md.Id_Mov
                                                  Join mdest In GiasContext.Mov_Destinazioni On mdest.Id_Agenda Equals md.Id_Agenda And mdest.Id_Mov Equals md.Id_Mov And mdest.Id_Mov_Det Equals md.Id_Mov_Det
                                                  Join ragg In GiasContext.Stalla_Raggruppamenti On mdest.Piva Equals ragg.PIVA And mdest.Sa_Cod Equals ragg.sa_cod And mdest.Id_Destinazione Equals ragg.Raggruppamento_Cod
                                                  Join zoo In GiasContext.Zoo_Animali On md.Cod_Progetto Equals zoo.Cod_Progetto
                                                  Where m.Cau_Mov = CAU_SCARICO_CONSISTENZE And
                                                     md.Elem_Cod = 300 And
                                                     ragg.sa_cod = Sa_Cod And
                                                     ragg.STA_NUM = Sta_Num And
                                                     zoo.Matricola = matricola).ToList

                        If movimentiScaricoDB.Count > 0 Then
                            Dim cod_progetto As Integer = movimentiScaricoDB(0).zoo.Cod_Progetto
                            objScrivi_Zoo.Riapri_Animale_Con_Del_Operazione_Scarico(Piva, cod_progetto, objParametriServer, GiasContext, False)
                        End If

                    Next

                Catch ex As ExpiredTokenBDNException
                    Throw ex
                Catch ex As Exception
                    Dim modelloStr = JsonConvert.SerializeObject(modello)
                    objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore importazione modello 4 uscita " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True) & ": " & modelloStr,
                          CustomLOGParams:=customLOGParams)
                End Try
            Next

        Catch ex As Exception

        End Try

        Return listaMatricoleCapi_Scarico

    End Function

    Public Function EsisteMovimentoScarico(Matricola As String, DataRiferimento As Date, Piva As String, sa_cod As Integer, Sta_Num As Integer) As Boolean
        Dim DT = ZooBIZ.Leggi_Scarico_Capi(Piva, sa_cod, Sta_Num, 0, 0, DataRiferimento, AGRODATAFINE, Matricola, False, New List(Of Integer), 0, False, False, objParametriServer)
        If DT.Rows.Count > 0 Then
            Return True
        End If
        Return False
    End Function

    ''' <summary>
    ''' Legge i dati dei Modello4 passati, ricavando anche i capi relativi al modello
    ''' </summary>
    ''' <param name="dtModelli">dt con Modelli4</param>
    ''' <param name="Allev_IdFiscale"></param>
    ''' <param name="GrSpe_Cod"></param>
    ''' <returns></returns>
    Public Function Carica_Modelli4Ingresso(ByVal dtModelli As DataTable,
                                    ByVal Allev_IdFiscale As String,
                                    ByVal GrSpe_Cod As String,
                                    Codice_Azienda As String,
                                    ByVal IdFiscale_Detentore As String,
                                    ByVal id_Allevamento As Integer,
                                    ByVal PROP_ID_FISCALE As String,
                                    ByVal DETEN_ID_FISCALE As String) As List(Of CaricaModelli4_Response)
        Dim objResp_CaricaModelli As New List(Of CaricaModelli4_Response)

        Try

            If dtModelli IsNot Nothing AndAlso dtModelli.Rows.Count > 0 Then
                'filtra i modelli per Allevamento non potendo farlo direttamente sulla chiamata 
                Dim listaModelliP = dtModelli.Select()
                'Dim aa = wsAziende.getAllevamento(Codice_Azienda, Allev_IdFiscale, GrSpe_Cod)
                For Each rowModello In listaModelliP


                    'Dim id_Allevamento = CInt(aa(0)("ALLEV_ID"))
                    'Dim PROP_ID_FISCALE = CStr(aa.Rows(0)("PROP_ID_FISCALE"))
                    'Dim DETEN_ID_FISCALE = CStr(aa.Rows(0)("DETEN_ID_FISCALE"))

                    Dim EstremoModello = CStr(rowModello("ESTREMI_MODELLO"))
                    Dim id_AllevamentoModello = rowModello("DESTINAZIONE_ID")
                    If id_Allevamento = id_AllevamentoModello AndAlso
                            PROP_ID_FISCALE = Allev_IdFiscale AndAlso
                            DETEN_ID_FISCALE = IdFiscale_Detentore Then
                        Try
                            Dim objModelli4 = wsInterrogazioniModello4.caricaModello4(rowModello("PRENOTAZIONE_ID"), GrSpe_Cod, True)
                            objResp_CaricaModelli.Add(objModelli4)
                        Catch ex As Exception

                            objScrivi_Zoo.Scrivi_LOG(objParametriServer, "Carica_Modelli4Ingresso", "Errore caricamento modello:" & EstremoModello & " Prenotazione ID:" & CStr(rowModello("PRENOTAZIONE_ID")))
                        End Try
                    End If



                Next

            End If

        Catch ex As Exception

        End Try

        Return objResp_CaricaModelli

    End Function

    Public Function Carica_Modelli4Uscita(ByVal dtModelli As DataTable,
                                    ByVal Allev_IdFiscale As String,
                                    ByVal GrSpe_Cod As String,
                                    Codice_Azienda As String,
                                    ByVal IdFiscale_Detentore As String) As List(Of CaricaModelli4_Response)
        Dim objResp_CaricaModelli As New List(Of CaricaModelli4_Response)

        Try

            If dtModelli IsNot Nothing AndAlso dtModelli.Rows.Count > 0 Then
                'filtra i modelli per Allevamento non potendo farlo direttamente sulla chiamata 
                Dim listaModelliP = dtModelli.Select()
                Dim aa = wsAziende.getAllevamento(Codice_Azienda, Allev_IdFiscale, GrSpe_Cod)
                For Each rowModello In listaModelliP
                    Dim objModelli4 = wsInterrogazioniModello4.caricaModello4(rowModello("PRENOTAZIONE_ID"), GrSpe_Cod, True)
                    objResp_CaricaModelli.Add(objModelli4)
                Next

            End If

        Catch ex As Exception

        End Try

        Return objResp_CaricaModelli

    End Function

    Public Function Carica_Modelli4PascoloIngresso(ByVal dtModelli As DataTable,
                                    ByVal Allev_IdFiscale As String,
                                    ByVal GrSpe_Cod As String,
                                    ByVal Codice_Azienda As String,
                                    ByVal IdFiscale_Detentore As String) As List(Of CaricaModelli4_Response)
        Dim objResp_CaricaModelli As New List(Of CaricaModelli4_Response)

        Try

            If dtModelli IsNot Nothing AndAlso dtModelli.Rows.Count > 0 Then
                'filtra i modelli per Allevamento non potendo farlo direttamente sulla chiamata 
                Dim listaModelliP = dtModelli.Select()
                Dim aa = wsAziende.getAllevamento(Codice_Azienda, Allev_IdFiscale, GrSpe_Cod)
                For Each rowModello In listaModelliP
                    Dim objModelli4 As New CaricaModelli4_Response
                    objModelli4.listaMatricoleCapi = New List(Of String)

                    Dim id_AllevamentoModello As String
                    If dtModelli.Columns.Contains("DESTINAZIONE_ID") Then
                        id_AllevamentoModello = rowModello("DESTINAZIONE_ID")
                    ElseIf dtModelli.Columns.Contains("DESTINAZIONE") Then
                        id_AllevamentoModello = rowModello("DESTINAZIONE")
                    End If

                    Dim id_Allevamento = CInt(aa(0)("ALLEV_ID"))
                    Dim PROP_ID_FISCALE = CStr(aa.Rows(0)("PROP_ID_FISCALE"))
                    Dim DETEN_ID_FISCALE = CStr(aa.Rows(0)("DETEN_ID_FISCALE"))
                    If Codice_Azienda = id_AllevamentoModello AndAlso
                            PROP_ID_FISCALE = Allev_IdFiscale AndAlso
                            DETEN_ID_FISCALE = IdFiscale_Detentore Then
                        objModelli4 = Carica_Modello4PascoloIngresso(rowModello("PRENOTAZIONE_ID"), GrSpe_Cod)
                        objResp_CaricaModelli.Add(objModelli4)
                    End If

                Next

            End If

        Catch ex As Exception

        End Try

        Return objResp_CaricaModelli

    End Function


    Public Function Carica_Modello4PascoloIngresso(PrenotazioneID As String, GrSpe_Cod As String) As CaricaModelli4_Response
        Dim dsPrenotazioneModello = wsInterrogazioniModello4.getPrenotazioneModelloPascolo(PrenotazioneID,
                                                                                                GrSpe_Cod)
        Dim objModelli4 As New CaricaModelli4_Response
        objModelli4.listaMatricoleCapi = New List(Of String)
        Try
            Dim dtModello4 As DataTable = dsPrenotazioneModello.Tables(0)
            objModelli4.prenotazioneId = IIf(dtModello4.Columns.Contains("PRENOTAZIONE_ID"), dtModello4.Rows(0).Item("PRENOTAZIONE_ID"), "")
            objModelli4.documentoId = IIf(dtModello4.Columns.Contains("DOCUMENTO_ID"), dtModello4.Rows(0).Item("DOCUMENTO_ID"), "")
            objModelli4.numModello = IIf(dtModello4.Columns.Contains("NUM_MODELLO"), dtModello4.Rows(0).Item("NUM_MODELLO"), "")
            If dtModello4.Columns.Contains("GIORNI_VALIDITA") Then
                objModelli4.giorniValidita = dtModello4.Rows(0).Item("GIORNI_VALIDITA")
            Else
                objModelli4.giorniValidita = ""
            End If

            objModelli4.tipoDestinazione = IIf(dtModello4.Columns.Contains("TIPOLOGIA_DEST"), dtModello4.Rows(0).Item("TIPOLOGIA_DEST"), "")

            If dtModello4.Columns.Contains("PASCOLO_CODICE") Then
                objModelli4.codAzienda_Prov = dtModello4.Rows(0).Item("PASCOLO_CODICE")
            Else
                objModelli4.codAzienda_Prov = ""
            End If

            objModelli4.oraPartenza = IIf(dtModello4.Columns.Contains("ORA_PARTENZA"), dtModello4.Rows(0).Item("ORA_PARTENZA"), "")
            If objModelli4.tipoDestinazione = "MACELLO" Then
                objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("MACELLO_CODICE"), dtModello4.Rows(0).Item("MACELLO_CODICE"), "")
            Else
                objModelli4.codAzienda_Dest = IIf(dtModello4.Columns.Contains("DEST_AZIENDA_CODICE"), dtModello4.Rows(0).Item("DEST_AZIENDA_CODICE"), "")
                objModelli4.idFiscaleAzienda_Dest = IIf(dtModello4.Columns.Contains("DEST_ALLEV_ID_FISCALE"), dtModello4.Rows(0).Item("DEST_ALLEV_ID_FISCALE"), "")
            End If

            If dtModello4.Columns.Contains("COD_ASL_TRASP") AndAlso Not IsNothing(dtModello4.Rows(0).Item("COD_ASL_TRASP")) AndAlso dtModello4.Rows(0).Item("COD_ASL_TRASP") = "" Then
                objModelli4.codAsl_Trasp = dtModello4.Rows(0).Item("COD_ASL_TRASP")
            End If

            objModelli4.durataViaggio = IIf(dtModello4.Columns.Contains("DURATA_VIAGGIO"), dtModello4.Rows(0).Item("DURATA_VIAGGIO"), "")

            If dtModello4.Columns.Contains("NUM_AUTORIZZAZIONE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")) AndAlso dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE") = "" Then
                objModelli4.numAutorizzazione = dtModello4.Rows(0).Item("NUM_AUTORIZZAZIONE")
            End If

            If dtModello4.Columns.Contains("TARGA_MOTRICE") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_MOTRICE")) AndAlso dtModello4.Rows(0).Item("TARGA_MOTRICE") = "" Then
                objModelli4.targa = dtModello4.Rows(0).Item("TARGA_MOTRICE")
            End If
            If dtModello4.Columns.Contains("TARGA_RIMORCHIO") AndAlso Not IsNothing(dtModello4.Rows(0).Item("TARGA_RIMORCHIO")) AndAlso dtModello4.Rows(0).Item("TARGA_RIMORCHIO") = "" Then
                objModelli4.targa_rimorchio = dtModello4.Rows(0).Item("TARGA_RIMORCHIO")
            End If

            objModelli4.dataUscita = IIf(dtModello4.Columns.Contains("DT_USCITA"), dtModello4.Rows(0).Item("DT_USCITA"), AGRODATAINIZIO)
            objModelli4.dataPrenotazione = Nothing

            Dim listaCapi_Modello4 = dsPrenotazioneModello.Tables(1).ToExpandoObject.ToList()
            Dim listaMatricoleCapi_Modello4 = (From cp In listaCapi_Modello4
                                               Select cp.Item("CAPO_CODICE")).ToList()

            For Each mat In listaMatricoleCapi_Modello4
                objModelli4.listaMatricoleCapi.Add(mat.ToString)
            Next
        Catch ex As Exception

        End Try

        Return objModelli4

    End Function

    Private Function SistemazioneCF_Proprietario(Piva As String, CF_Proprietario As String, dt_CapiPresenti_Stalla As DataTable)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(objParametriServer.StringaConnessione)
        Dim GiasContext As New Gias_DeveloperServer_Entities(EFConnString)
        Dim transactionOptions As New TransactionOptions()
        transactionOptions.IsolationLevel = IsolationLevel.ReadUncommitted
        transactionOptions.Timeout = TransactionManager.MaximumTimeout

        Dim StartTransaction As DateTime
        Dim EndTransaction As DateTime

        Dim scope As New TransactionScope(TransactionScopeOption.Required, transactionOptions)
        StartTransaction = DateTime.Now

        Try
            Dim drCapiSenzaProprietario = dt_CapiPresenti_Stalla.Select(" CF_PROPRIETARIO = '' ")
            If drCapiSenzaProprietario.Count > 0 Then
                For Each capo In drCapiSenzaProprietario
                    Dim matricola As String = capo("Matricola")
                    Dim Zoo_Animali = (From z In GiasContext.Zoo_Animali Where z.PIVA = Piva And z.Matricola = matricola).FirstOrDefault
                    If Zoo_Animali IsNot Nothing Then
                        Zoo_Animali.CF_PROPRIETARIO = CF_Proprietario
                    End If
                Next
            End If
            GiasContext.SaveChanges()
            GiasContext.Core.AcceptAllChanges()
            scope.Complete()
            scope.Dispose()

        Catch ex As Exception
            scope.Dispose()
        Finally
            GiasContext.Dispose()
        End Try
    End Function

    Private Sub gestioneDestinazione_UscitaModello(ByVal Piva As String,
                                                        ByVal SaCod As Integer,
                                                        ByVal objModello As CaricaModelli4_Response)
        Dim seq As New Agro_Sequenze()

        '---------CREA DESTINAZIONE---------
        Dim tipoDestinazione As Integer = 0
        If objModello.tipoDestinazione = "MACELLO" Then
            tipoDestinazione = COD_MACELLO
        Else
            tipoDestinazione = COD_ALLEVATORE
        End If

        If objModello.idFiscaleAzienda_Dest Is Nothing Then
            objModello.idFiscaleAzienda_Dest = ""
        End If

        Dim objContatti_R As New AgronicaCoreAnagrafeDAL.Contatti_R
        Dim dtContatto As DataTable =
                                objContatti_R.Leggi(Piva, objModello.idFiscaleAzienda_Dest,
                                                    0, tipoDestinazione, True, True,
                                                    0, 0, False, 0, CInt(-99),
                                                    0, "", False, 0,
                                                    0, 0, 0, 0, AGRODATAINIZIO,
                                                    AGRODATAFINE, False,
                                                    "", "", objParametriServer)

        If dtContatto.Rows.Count > 0 And objModello.tipoDestinazione = "MACELLO" Then
            Dim dtContattoFiltered = dtContatto.Select(" Settore_Des = '" & objModello.codAzienda_Dest & "' ").CopyToDataTable
            If dtContatto.Rows.Count > 0 Then
                objModello.idFiscaleAzienda_Dest = dtContattoFiltered(0)("Cod_Contatto")
            End If
            'codAzienda_Dest
        End If

        Dim codDestinazione As Integer = 0
        If IsNothing(dtContatto) OrElse dtContatto.Rows.Count = 0 Then
            '------CONTATTO------
            Dim objContatti_W As New AgronicaCoreAnagrafeDAL.Contatti_W
            objContatti_W.Scrivi(Piva, SaCod,
                                 objModello.idFiscaleAzienda_Dest,
                                 1, objModello.codAzienda_Dest,
                                 objModello.idFiscaleAzienda_Dest,
                                 "", 0, "", "",
                                 AGRODATAINIZIO, "", "",
                                 AGRODATAINIZIO, AGRODATAFINE,
                                 objParametriServer)


            '------RISORSA UMANA------
            'creo la risorsa umana
            Dim Cod_RisUm As Integer = seq.NuovoId_Tabella("Risorse_Umane", 0, 2000000000, objParametriServer)

            Dim objRisUm_W As New AgronicaCoreAnagrafeDAL.Risorse_Umane_W
            objRisUm_W.Scrivi(Piva, SaCod, Cod_RisUm,
                              objModello.idFiscaleAzienda_Dest, tipoDestinazione,
                              objModello.codAzienda_Dest, "", 0,
                              0, 0, 0, 0,
                              0, 0, "",
                              AGRODATAINIZIO, AGRODATAFINE,
                              "", 0, "",
                              0, 0, 0,
                              AGRODATAINIZIO, AGRODATAFINE,
                              objParametriServer)

        End If
    End Sub

    Private Function esisteMatricolaInStallaESistemazioneProprietario(Piva As String, Sa_Cod As Integer, Sta_Num As Integer, Matricola As String, CF_Proprietario As String, CF_Detentore As String) As Boolean
        Dim esiste As Boolean = False

        Dim dtGiacenze As DataTable = ZooBIZ.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num, 0, 0, DateTime.Now, objParametriServer, False, False, Nothing, True, False, False, False, "", Matricola)
        If dtGiacenze.Rows.Count > 0 Then
            esiste = True
            Dim Cod_Animale As Integer = CInt(dtGiacenze.Rows(0)("Cod_Animale"))
            Dim zoo_animale = (From z In GiasContext.Zoo_Animali Where z.Cod_Progetto = Cod_Animale).FirstOrDefault
            If zoo_animale IsNot Nothing Then
                zoo_animale.CF_PROPRIETARIO = CF_Proprietario
                zoo_animale.CF_DETENTORE = CF_Detentore
            End If
        End If

        Return esiste
    End Function

    Public Function leggiAllevamentiAttivi(Codice_Azienda As String) As DataTable
        Try

            Dim dtAllevamenti As New DataTable
            dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")
            Dim drAllevamentiDetentore As DataRow()

            If dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                drAllevamentiDetentore = dtAllevamenti.Select(" DT_FINE_ATTIVITA IS NULL ")
            Else
                drAllevamentiDetentore = dtAllevamenti.Select("")
            End If

            dtAllevamenti = drAllevamentiDetentore.CopyToDataTable

            Return dtAllevamenti

        Catch ex As Exception

            Return Nothing

        End Try
    End Function

    Public Function leggiAllevamentiAttiviPresentiSuGias(Piva As String, Codice_Azienda As String) As DataTable
        Try

            Dim dtAllevamenti As New DataTable
            Dim dtAllevamentiResponse As New DataTable
            dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")
            Dim drAllevamentiDetentore As DataRow()
            dtAllevamentiResponse = dtAllevamenti.Clone()
            If dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                drAllevamentiDetentore = dtAllevamenti.Select(" DT_FINE_ATTIVITA IS NULL ")
            Else
                drAllevamentiDetentore = dtAllevamenti.Select("")
            End If
            If drAllevamentiDetentore.Count = 0 Then
                Return Nothing
            End If
            dtAllevamenti = drAllevamentiDetentore.CopyToDataTable

            Dim dtStalle_R As New AgronicaCoreAnagrafeDAL.Stalla_R
            Dim dtStalleConfigurazioni_R As New AgronicaCoreAnagrafeDAL.Stalla_Configurazioni_BDN_R
            Dim dtStalla = dtStalle_R.Leggi(Piva, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametriServer, Codice_Azienda)
            If dtStalla.Rows.Count = 0 Then
                Return Nothing
            Else

                Dim sa_cod As Integer = dtStalla(0)("sa_cod")
                Dim sta_num As Integer = dtStalla(0)("sta_num")
                Dim dtConfigurazioni = dtStalleConfigurazioni_R.Leggi(0, Piva, sa_cod, sta_num, "", "",
                                                                      Date.Now, Date.Now,
                                                                      enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                                      "", "", objParametriServer)

                If dtConfigurazioni.Rows.Count = 0 Then
                    Return Nothing
                Else
                    Dim listProprietari As List(Of String) = (From r In dtConfigurazioni.Rows Select CStr(r("CF_Proprietario"))).ToList()
                    For Each rowAllevamento In dtAllevamenti.Rows
                        If listProprietari.Contains(CStr(rowAllevamento("ID_Fiscale"))) Then
                            Dim newRow = dtAllevamentiResponse.NewRow
                            newRow("DENOMINAZIONE") = rowAllevamento("DENOMINAZIONE_PROP") & " (" & rowAllevamento("ID_FISCALE") & ")"
                            newRow("ID_FISCALE") = rowAllevamento("ID_FISCALE")
                            dtAllevamentiResponse.Rows.Add(newRow)
                        End If
                    Next
                End If
            End If

            Return dtAllevamentiResponse

        Catch ex As Exception

            Return Nothing

        End Try
    End Function

    Private Function SistemazioniFinali(Piva As String,
                                        Sa_Cod As Integer,
                                        Sta_Num As Integer,
                                        Spe_Cod As String,
                                        Codice_Azienda_BDN As String,
                                        ID_Fiscale_Proprietario As String,
                                        lista_ConsistenzeStalla_BDN As List(Of IDictionary(Of String, Object)),
                                        lista_ConsistenzeStalla_DB As List(Of IDictionary(Of String, Object))) As SincroBDN_CapiDaControllare_Response

        Dim resp As New SincroBDN_CapiDaControllare_Response

        Dim listaCarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Carico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)
        Dim listaScarichiNonSincronizzatiDT = obj_ZooAnimali_R.BDN_Leggi_Movimenti_Scarico_Non_Sincronizzati(Piva, Sa_Cod, Sta_Num, 0, 0, AGRODATAINIZIO, AGRODATAFINE, False, New List(Of Integer), objParametriServer)

        Dim dt_CapiPresenti_Stalla As DataTable = obj_ZooAnimali_R.Leggi_Giacenze(Piva, Sa_Cod, Sta_Num,
                                                                                      0, 0, Date.Now,
                                                                                      objParametriServer)

        Dim dr_capiPresenti = dt_CapiPresenti_Stalla.Select(" CF_PROPRIETARIO = '" & ID_Fiscale_Proprietario & "' ")

        If dr_capiPresenti.Count > 0 Then
            dt_CapiPresenti_Stalla = dt_CapiPresenti_Stalla.Select(" CF_PROPRIETARIO = '" & ID_Fiscale_Proprietario & "' ").CopyToDataTable
        Else
            dt_CapiPresenti_Stalla = New DataTable
        End If

        dt_CapiPresenti_Stalla.Columns.Add(New DataColumn("Data_Uscita") With {.DefaultValue = ""})
        Dim lista_ConsistenzaStalla_DB = dt_CapiPresenti_Stalla.ToExpandoObject.ToList

        Dim lista_Matricole_DB As List(Of String) = (From cp In lista_ConsistenzaStalla_DB
                                                     Select CStr(cp.Item("Matricola"))).ToList()

        Dim lista_Matricole_BDN As List(Of String) = (From cp In lista_ConsistenzeStalla_BDN
                                                      Select CStr(cp.Item("MARCHIO"))).ToList()


        Dim listaMatricoleCarichiNonSincronizzatiDT As List(Of String) = (From cp In listaCarichiNonSincronizzatiDT.Rows Select CStr(cp("Matricola"))).ToList
        Dim listaMatricoleScarichiNonSincronizzatiDT As List(Of String) = (From cp In listaScarichiNonSincronizzatiDT.Rows Select CStr(cp("Matricola"))).ToList


        Dim matricoleDuplicate As List(Of String) = lista_Matricole_DB.GroupBy(Function(m) m) _
                 .Where(Function(g) g.Count() > 1) _
                 .Select(Function(g) g.Key).ToList


        Dim listaMatricoleProiezione_DaControllare As List(Of String) = lista_Matricole_DB.Select(Function(x) x).ToList()
        listaMatricoleProiezione_DaControllare.AddRange(listaMatricoleScarichiNonSincronizzatiDT)
        listaMatricoleProiezione_DaControllare = listaMatricoleProiezione_DaControllare.Except(listaMatricoleCarichiNonSincronizzatiDT).ToList
        listaMatricoleProiezione_DaControllare = listaMatricoleProiezione_DaControllare.Except(lista_Matricole_BDN).ToList

        Dim listaMatricoleDB_DaControllare = listaMatricoleProiezione_DaControllare
        Dim listaMatricoleBDN_DaControllare = lista_Matricole_BDN.Except(listaMatricoleProiezione_DaControllare).ToList
        listaMatricoleBDN_DaControllare = lista_Matricole_BDN.Except(lista_Matricole_DB).ToList
        listaMatricoleBDN_DaControllare = listaMatricoleBDN_DaControllare.Except(listaMatricoleScarichiNonSincronizzatiDT).ToList


        resp.capiBDN = listaMatricoleBDN_DaControllare
        resp.capiDB = listaMatricoleDB_DaControllare
        resp.MatricoleDuplicate = matricoleDuplicate
        Return resp

    End Function

    ''' <summary>
    ''' Creazione log per movimentazioni BDN
    ''' </summary>
    ''' <param name="piva"></param>
    ''' <param name="saCod"></param>
    ''' <param name="idAgenda"></param>
    ''' <param name="lavCod"></param>
    ''' <param name="matricola"></param>
    ''' <param name="causaleBDN"></param>
    ''' <param name="idMovimentoBDN"></param>
    ''' <param name="idCapoSincro"></param>
    ''' <param name="datiInviati"></param>
    ''' <param name="datiRicevuti"></param>
    ''' <param name="esito"></param>
    ''' <param name="GiasContext"></param>
    ''' <returns></returns>
    Private Function logMovimentazioniBDN(ByVal piva As String,
                                          ByVal saCod As Integer,
                                          ByVal idAgenda As Integer,
                                          ByVal lavCod As Integer,
                                          ByVal matricola As String,
                                          ByVal causaleBDN As enumCausaliBDN,
                                          ByVal idMovimentoBDN As Integer,
                                          ByVal idCapoSincro As String,
                                          ByVal datiInviati As String,
                                          ByVal datiRicevuti As String,
                                          ByVal esito As Boolean,
                                          ByRef GiasContext As Gias_DeveloperServer_Entities) As Boolean
        Dim objLogInvioChiamate As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Chiamate_W
        Dim objLogInvioAgenda As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Agenda_W
        Dim objLogInvioAnagrafe As New AgronicaCoreVarieDAL.Agronica_Log_Invio_Anagrafe_W

        Try
            Dim opBDN = (From a In GiasContext.Agenda
                         Join mdt In GiasContext.Movimenti_dettagli On a.Id_Agenda Equals mdt.Id_Agenda
                         Join z In GiasContext.Zoo_Animali On mdt.Cod_Progetto Equals z.Cod_Progetto
                         Where a.PIVA = piva And a.Id_Agenda = idAgenda And z.Matricola = matricola
                         Select z.Cod_Progetto, mdt.Id_Mov, mdt.Id_Mov_Det, mdt.Id_Mov_Esterno).FirstOrDefault

            Dim chiaveCapo As String = piva & "_" & saCod & "_" & opBDN.Cod_Progetto
            Dim idMovDest As Integer = 0
            Dim msgEsito As String = IIf(esito, "Eseguita", "Errore")

            If idMovimentoBDN = 0 Then
                idMovimentoBDN = opBDN.Id_Mov_Esterno
            End If

            'AGRONICA LOG INVIO CHIAMATE
            'Dim LogInvioChiamata = objLogInvioChiamate.Create_Agronica_Log_Invio_Chiamate(enum_Esportazioni_Sistema_Cod.BDN,
            '                                                                              datiInviati, DateTime.Now, msgEsito,
            '                                                                              datiRicevuti, 0, lavCod,
            '                                                                              objParametriServer, GiasContext)
            Dim LogInvioChiamata = objLogInvioChiamate.Scrivi(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                          datiInviati, DateTime.Now, msgEsito,
                                                                                          datiRicevuti, 0, lavCod,
                                                                                          objParametriServer)

            'AGRONICA LOG INVIO AGENDA
            Dim LogInvioAgenda = objLogInvioAgenda.Create_Agronica_Log_Invio_Agenda(enum_Esportazioni_Sistema_Cod.BDN,
                                                                                    idAgenda, idMovimentoBDN,
                                                                                    LogInvioChiamata, objParametriServer,
                                                                                    GiasContext, opBDN.Id_Mov, opBDN.Id_Mov_Det,
                                                                                    idMovDest, causaleBDN)

            'AGRONICA LOG INVIO ANAGRAFE
            Dim LogInvioAnagrafe As Boolean = objLogInvioAnagrafe.Scrivi(enum_Esportazioni_Sistema_Cod.BDN, LogInvioChiamata,
                                                                         esito, "BDN", chiaveCapo, piva, saCod, 0, 0,
                                                                         opBDN.Cod_Progetto, "", DateTime.Now, objParametriServer,
                                                                         idCapoSincro, , , causaleBDN)

            'per ora con eccezione ritorna semplicemente falso
        Catch ex As Exception
            Return False
        End Try

        Return True

    End Function



    Public Class Modello4DTO
        Public modello As CaricaModelli4_Response
        Public animaliModello As List(Of AnimaleModello4)
        Public nAnimaliModello As Integer
        Public isPascolo As Boolean
        Public piva As String
        Public sa_cod As String
        Public sta_num As String
        Public denominazione As String
        Public isModelloLetto As Boolean
        Public isPresenteAnimaleNonSincronizzato As Boolean
    End Class

    Public Class AnimaleModello4
        Public matricola As String
        Public isOnGias As Boolean
        Public isOnModello As Boolean
        Public numModello As String
        Public prenotazioneId As String
    End Class

    Public Function checkIsOnGiasAndModello(Piva As String, sa_cod As Integer, sta_num As Integer, matricole As List(Of String)) As Dictionary(Of String, Tuple(Of Boolean, String, String))
        Dim dt = ZooBIZ.Leggi_Giacenze(Piva, sa_cod, sta_num, 0, 0, DateTime.Now, objParametriServer, False, False, Nothing, True, False, False, False, "", "", False, matricole)

        Dim dict As New Dictionary(Of String, Tuple(Of Boolean, String, String))

        Dim defaultObj = New Tuple(Of Boolean, String, String)(False, "", "") 'riciclo lo stesso oggetto per risparmiare risorse
        For Each item As String In matricole
            dict.Add(item, defaultObj)
        Next

        For Each animale In dt.Rows
            Dim matricola = animale("Matricola")
            dict(matricola) = New Tuple(Of Boolean, String, String)(True,
                                         animale("Modello4_Ingresso_Numero"),
                                         animale("Modello4_Ingresso_Prenotazione")
                                       )
        Next

        Return dict
    End Function

    Public Function checkModelloLettoeAnimaliNonSincronizzati(Piva As String, numModello As String, prenotazioneId As String, animali As List(Of AnimaleModello4)) As Tuple(Of Boolean, Boolean)
        Dim isModelloLetto = False
        Dim isPresenteAnimaleNonSincronizzato = False

        Dim dt = ZooBIZ.LeggiModello4Inviati(Piva, numModello, prenotazioneId, objParametriServer).ToExpandoObject
        If dt.Any Then
            isModelloLetto = True
        End If
        isPresenteAnimaleNonSincronizzato = animali.Any(Function(x) Not x.isOnGias)

        Return New Tuple(Of Boolean, Boolean)(isModelloLetto, isPresenteAnimaleNonSincronizzato)
    End Function


    Public Function LeggiBDN(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Sta_Num As Integer,
                                       ByVal Codice_Azienda As String,
                                       ByVal IdFiscale_Proprietario As String,
                                       ByVal SpeCod As String,
                                       ByVal dataInizio As Date,
                                       ByVal dataFine As Date,
                                       ByVal StallaMultipla As Boolean) As List(Of Modello4DTO)
        Dim sincroAllev_Response As New SincroBDN_Allevamento_Response

        customLOGParams.LogFileName = Date.Now.Year & Date.Now.Month & Date.Now.Day & " " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ".txt"
        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Inizio Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod,
                          CustomLOGParams:=customLOGParams)

        Try
            Dim dtAllevamenti As New DataTable

            'Dim AziendeConsorzio = wsGestioneAssConsorzi.Get_Aziende_Consorzio("", "INA")

            dtAllevamenti = wsAziende.FindAllevamento(Codice_Azienda, "", "")

            Dim aa = wsAziende.getAllevamento(Codice_Azienda, IdFiscale_Proprietario, SpeCod)

            Dim dtAllevamentiFiltered As DataTable
            If Not dtAllevamenti.Columns.Contains("DT_FINE_ATTIVITA") Then
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            Else
                Dim _drAllevamento = dtAllevamenti.Select(" ID_FISCALE_PROP = '" + IdFiscale_Proprietario + "' AND SPE_CODICE = '" + SpeCod + "' AND ( DT_FINE_ATTIVITA = '' OR DT_FINE_ATTIVITA IS NULL ) ")
                If _drAllevamento.Count > 0 Then
                    dtAllevamentiFiltered = _drAllevamento.CopyToDataTable
                End If
            End If

            If IsNothing(dtAllevamentiFiltered) Then
                Throw New GiasException("Allevamento non trovato")
            End If

            If dtAllevamentiFiltered IsNot Nothing AndAlso dtAllevamentiFiltered.Rows.Count = 0 Then
                Throw New GiasException("Allevamento non trovato")
            End If

            Dim drAllevamento = dtAllevamentiFiltered(0)

            Dim Allev_Id = drAllevamento("ALLEV_ID")
            Dim IdFiscale_Detentore = drAllevamento("ID_FISCALE_DETEN")
            Dim Azienda_id = drAllevamento("AZIENDA_ID")
            Dim Asl_Codice = drAllevamento("CODICE_ASL")

            'ricava il raggruppamento configurato su GIAS come appoggio per la sincronizzazione con BDN
            Dim objRaggruppStalla_R As New AgronicaCoreAnagrafeDAL.Stalla_Raggruppamenti_R
            Dim dtRaggruppamentoStalla = objRaggruppStalla_R.Leggi_x_anagrafica(objParametriServer.PivaSuperUser,
                                                                                Piva, Sa_Cod, Sta_Num, 0,
                                                                                "Stalla_Raggruppamenti.Flag_BDN = 1",
                                                                                "", objParametriServer)

            If IsNothing(dtRaggruppamentoStalla) OrElse dtRaggruppamentoStalla.Rows.Count = 0 Then
                Throw New GiasException("L'allevamento " & Codice_Azienda & " non ha raggruppamenti presenti su GIAS")

            End If

            Dim Raggruppamento_Cod As Integer = dtRaggruppamentoStalla.Rows(0)("raggruppamento_cod")
            Dim CentroAzienda_Cod As Integer = 0

            'legge i capi in stalla (BDN)
            'Dim dt_ConsistenzaStalla_BDN As DataTable
            'Try
            '    dt_ConsistenzaStalla_BDN = wsRegistroStalla.Capi_In_Stalla(Allev_Id)
            'Catch ex As BDNException
            '    Throw ex
            'Catch ex As Exception
            '    If ex.Message.Contains("timeout") Then
            '        Throw ex
            '    End If
            'End Try

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "INIZIO CARICO MODELLO 4",
                              CustomLOGParams:=customLOGParams)

            '==================================================================================CARICO=======================================================================================================

            Dim lista_Matricole_SincroModello4_Carico As New List(Of String)

            Dim dataOut As New List(Of Modello4DTO)
            Try

                Dim listaMatricoleCapi_Carico As New List(Of String)


                Dim listaModelli_Ingresso = Leggi_ModelliIngresso(Asl_Codice, Codice_Azienda, IdFiscale_Proprietario, SpeCod, IdFiscale_Detentore, dataInizio, dataFine)

                Dim listaModelli_IngressoPascolo = Leggi_ModelliIngressoPascolo(Asl_Codice, Codice_Azienda, IdFiscale_Proprietario, SpeCod, IdFiscale_Detentore, dataInizio, dataFine)


                If IsNothing(listaModelli_Ingresso) AndAlso IsNothing(listaModelli_IngressoPascolo) Then
                    Exit Try
                End If

                Dim listaMatricoleComplete = listaModelli_Ingresso. 'Leggo tutte le matricole in una volta per fare un check unico e ridure chiamate al db
                    SelectMany(Of String)(Function(modello) modello.listaMatricoleCapi).
                    Union(listaModelli_IngressoPascolo.
                        SelectMany(Of String)(Function(modello) modello.listaMatricoleCapi)).
                    ToList


                Dim dictRis = checkIsOnGiasAndModello(Piva, Sa_Cod, Sta_Num, listaMatricoleComplete)

                For Each modello In listaModelli_Ingresso
                    Try
                        Dim listaMatricole_Carico_Modello4 = modello.listaMatricoleCapi '.ToList()

                        If Not IsNothing(listaMatricole_Carico_Modello4) AndAlso listaMatricole_Carico_Modello4.Count > 0 Then

                            Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)


                            Dim data_Arrivo = getDataArrivo(modello.dataUscita, modello.oraPartenza, modello.durataViaggio)


                            If data_Arrivo > DateTime.Now Then
                                Continue For
                            End If




                            Dim animaliModello = listaMatricole_Carico_Modello4.
                                Select(
                                Function(mat)
                                    Dim ris = dictRis(mat)
                                    Dim isOnGias = ris.Item1
                                    Dim isOnModello = isOnGias AndAlso ris.Item2 = modello.numModello AndAlso ris.Item3 = modello.prenotazioneId
                                    Return New AnimaleModello4 With {
                                        .isOnGias = isOnGias,
                                        .isOnModello = isOnModello,
                                        .matricola = mat,
                                        .numModello = modello.numModello,
                                        .prenotazioneId = modello.prenotazioneId
                                    }
                                End Function).ToList

                            If animaliModello.Count = 0 Then
                                Continue For
                            End If


                            Dim ris2 = checkModelloLettoeAnimaliNonSincronizzati(Piva, modello.numModello, modello.prenotazioneId, animaliModello)

                            dataOut.Add(New Modello4DTO With {
                                .animaliModello = animaliModello,
                                .nAnimaliModello = animaliModello.Count,
                                .modello = modello,
                                .isPascolo = False,
                                .piva = Piva,
                                .sa_cod = Sa_Cod,
                                .sta_num = Sta_Num,
                                .isModelloLetto = ris2.Item1,
                                .isPresenteAnimaleNonSincronizzato = ris2.Item2
                            })


                        End If

                    Catch ex As ExpiredTokenBDNException
                        Throw ex
                    Catch ex As Exception
                        Dim modelloStr = JsonConvert.SerializeObject(modello)
                        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore importazione modello 4 ingresso " & ex.Message & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
                    End Try
                Next


                For Each modello In listaModelli_IngressoPascolo
                    Try
                        'seleziona i capi che risultano in ingresso nel Modello4 recuperando i dati da BDN
                        Dim listaMatricole_Carico_Modello4 = modello.listaMatricoleCapi.ToList()

                        If Not IsNothing(listaMatricole_Carico_Modello4) AndAlso listaMatricole_Carico_Modello4.Count > 0 Then

                            Dim lista_capoAnimaleCDC As New List(Of AgronicaCoreModelsSTD.attivita.centri_di_costo.CentroDiCosto)


                            Dim data_Arrivo = getDataArrivo(modello.dataUscita, modello.oraPartenza, modello.durataViaggio)


                            If data_Arrivo > DateTime.Now Then
                                Continue For
                            End If

                            Dim animaliModello = listaMatricole_Carico_Modello4.
                                Select(
                                Function(mat)
                                    Dim ris = dictRis(mat)
                                    Dim isOnGias = ris.Item1
                                    Dim isOnModello = isOnGias AndAlso ris.Item2 = modello.numModello AndAlso ris.Item3 = modello.prenotazioneId
                                    Return New AnimaleModello4 With {
                                        .isOnGias = isOnGias,
                                        .isOnModello = isOnModello,
                                        .matricola = mat,
                                        .numModello = modello.numModello,
                                        .prenotazioneId = modello.prenotazioneId
                                    }
                                End Function).ToList

                            If animaliModello.Count = 0 Then
                                Continue For
                            End If
                            Dim ris2 = checkModelloLettoeAnimaliNonSincronizzati(Piva, modello.numModello, modello.prenotazioneId, animaliModello)
                            dataOut.Add(New Modello4DTO With {
                                .animaliModello = animaliModello,
                                .nAnimaliModello = animaliModello.Count,
                                .modello = modello,
                                .isPascolo = True,
                                .piva = Piva,
                                .sa_cod = Sa_Cod,
                                .sta_num = Sta_Num,
                                .isModelloLetto = ris2.Item1,
                                .isPresenteAnimaleNonSincronizzato = ris2.Item2
                            })



                        End If

                    Catch ex As ExpiredTokenBDNException
                        Throw ex
                    Catch ex As Exception
                        Dim modelloStr = JsonConvert.SerializeObject(modello)
                        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore importazione modello 4 ingresso " & ex.Message & ": " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                          CustomLOGParams:=customLOGParams)
                    End Try
                Next

            Catch ex As BDNException
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Modello 4 Ingresso per Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
                Throw ex

            Catch ex As Exception
                objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Modello 4 Ingresso per Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
                Throw ex
            End Try

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "FINE CARICO MODELLO 4",
                              CustomLOGParams:=customLOGParams)

            Return dataOut

        Catch ex As ExpiredTokenBDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore ExpiredTokenBDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As GiasException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore GiasException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw New GiasException(ex.Message)

        Catch ex As BDNException
            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore BDNException(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        Catch ex As Exception

            objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          "Errore Exception(Piva:" & Piva &
                          " - sa_cod:" & Sa_Cod &
                          " - Sta_Num:" & Sta_Num &
                          " - CodiceAzienda: " & Codice_Azienda &
                          " - IdFiscale_Proprietario: " & IdFiscale_Proprietario &
                          " - SpeCod:" & SpeCod & " --> " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)

            objLog.Scrivi_LOG(objParametriServer,
                              System.Reflection.MethodBase.GetCurrentMethod().Name,
                              "Errore Sincronizzazione Allevamento " & Codice_Azienda & " " & IdFiscale_Proprietario & " " & SpeCod & ": " & ex.Message & " " & AgronicaCoreUtility.Gestione_Eccezioni.MessaggioCompletoDataEccezione(ex, True),
                              CustomLOGParams:=customLOGParams)
            Throw ex
        End Try

    End Function


    Sub ScrivLog(messaggio As String)
        Dim _customLOGParams As New CustomLOGParams With {
            .LogDescrizioneUtente = objParametriServer.LogDescrizioneUtente,
            .LogDirectory = logDirectory,
            .LogFileName = "logBDN.txt"
        }

        objLog.Scrivi_LOG(objParametriServer,
                          System.Reflection.MethodBase.GetCurrentMethod().Name,
                          messaggio,
                          CustomLOGParams:=_customLOGParams)
    End Sub
End Class

