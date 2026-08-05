Imports System.Collections.Concurrent
Imports System.Net.Http
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDTOStd.InData.Demetra
Imports AgronicaCoreDTOStd.InData.importazioni
Imports AgronicaCoreDTOStd.InData.Notifiche
Imports AgronicaCoreModelsSTD.anagrafiche
Imports AgronicaCoreModelsSTD.exceptions
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json


Public Class Import
    Implements IDisposable

    Private _objParametriSuperServer As AgronicaCoreParametri
    Private _objParametriServer As AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreParametri

    Private _parametriTask As ParametriImportAnagrafiche

    Private _logName As String = DateTime.Now.ToString("yyyy-MM-dd") & ".log"
    Private _logger As LoggerManager
    Private _helper As Helper
    Private _LinkCoreWS As String

    Private _agronicacorelogDir As String
    Private _agronicacorelogFile As String

    Private Const ID_SERVIZIO_DEMETRA_QDC = 1014
    Private Const STATO_SERVIZIO_DEMETRA_QDC_UTENTE_OK = 101402
    Private Const STATO_SERVIZIO_DEMETRA_QDC_QUADERNO_OK = 101403


    Public Sub New(ByVal configurazioneServizio As Configurazione_Servizio,
                   ByVal objParametriSuperServer As AgronicaCoreParametri,
                   ByVal objParametriServer As AgronicaCoreParametri,
                   ByVal objParametriUtenti As AgronicaCoreParametri)
        _objParametriSuperServer = objParametriSuperServer
        _objParametriServer = objParametriServer
        _objParametriUtenti = objParametriUtenti
        _parametriTask = JsonConvert.DeserializeObject(Of ParametriImportAnagrafiche)(configurazioneServizio.Parametri_Extra)



        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim urlColdirettiLogger As String = objConfigSiti.Leggi_Valore(16, "Coldiretti_ElasticSearchUrl", "", "", _objParametriServer)

        _logger = New LoggerManager(_parametriTask.environment,
                                    configurazioneServizio.DirectoryLOG,
                                    _logName,
                                    _parametriTask.TagName,
                                    IIf(_parametriTask.cacheLog = 0, 50, _parametriTask.cacheLog),
                                    IIf(_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra, urlColdirettiLogger, ""),
                                    _parametriTask.ElasticSearch_Notify,
                                    _parametriTask.ElasticSearch_InviaSoloErrori)

        Dim basePath As String = objConfigSiti.Leggi_Valore(16, "LanToWebSiteBasePath", "", "", _objParametriServer)

        Dim url As String = objConfigSiti.Leggi_Valore(16, "GiasOnline_WS_Core_AgroWS_Core", "", "", _objParametriServer)

        If url.StartsWith("http") Then
            _LinkCoreWS = url
        Else
            _LinkCoreWS = basePath & url
        End If

        _helper = New Helper(_parametriTask.ExtSysRef, _objParametriServer, _logger)

        _agronicacorelogDir = configurazioneServizio.DirectoryLOG
        _agronicacorelogFile = IIf(_parametriTask.TagName = "", _logName, _parametriTask.TagName & "_" & _logName)

        'Lavez - 15/09/2025 - Creazioen cartella di log se non esiste
        If Not System.IO.Directory.Exists(_agronicacorelogDir) Then
            IO.Directory.CreateDirectory(_agronicacorelogDir)
            _logger.AppendLog(Nothing, "", String.Format("Creata cartella di log {0}", _agronicacorelogDir), LogType.Informazione, Nothing, True)
        End If

    End Sub

    Public Sub Avvia(ByVal TipoOperazione As Integer, ByRef RiepilogoMail As String)
        RiepilogoMail = ""
        Select Case TipoOperazione
            Case 1
                _logger.AppendLog(Nothing, "", String.Format("[Creazione] Avvio importazione anagrafiche e piano colturale da Demetra"), LogType.Informazione, Nothing, True)
            Case 2
                _logger.AppendLog(Nothing, "", String.Format("[Aggiornamento] Avvio importazione anagrafiche e piano colturale da Demetra"), LogType.Informazione, Nothing, True)
            Case Else
                _logger.AppendLog(Nothing, "GSB", String.Format("Tipo operazione per importazione anagrafiche e piano colturale da Demetra non mappata. impossibile proseguire"), LogType.Errore)
                Return
        End Select

        Dim listEsitiCUAA As New ConcurrentDictionary(Of String, Tuple(Of String, String))

        ''0 - verifica Pool corews avviato
        'If CheckStartPoolCoreWS() = False Then
        '    _logger.AppendLog(Nothing, "GSB", String.Format("Pool corews spento o in errore. Impossibile proseguire "))
        '    _logger.Flush("")
        '    Exit Sub
        'End If

        Dim listCUAA = _helper.GetCUAAList(TipoOperazione, _parametriTask.BatchSize, _parametriTask.TagName, _parametriTask.TipoAnagrafe)

        If _parametriTask.BatchSize > 0 Then
            _logger.AppendLog(Nothing, "GSB", String.Format("Elaborazione di {0} records ", listCUAA.Count.ToString()), LogType.Informazione, Nothing, True)
        End If

        For Each cuaa In listCUAA
            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] inizio elaborazione cuua con priorita {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.Priorita))
            Dim EsitoSync = New SyncAcknowledge() With {.processed = False}
            Dim _webapiDataPublish = New WebApiDataPublish(_parametriTask.DemetraBaseUrl, New Tuple(Of String, String)("APIKEY", _parametriTask.apikey))
            Dim NumeroAppezzamentiTrovati As Integer = 0
            Dim pivaAzienda As String = ""

            Try
                Select Case TipoOperazione
                    Case 1
                        'creazione
                        If cuaa.payload.Operazione = "U" Then
                            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Notifica di tipo aggiornamento. Flusso di elaborazione per creazione. Notifica ignorata ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning)
                        Else
                            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] creazione ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            EseguiCreazione(cuaa,
                                            _webapiDataPublish,
                                            EsitoSync,
                                            listEsitiCUAA,
                                            NumeroAppezzamentiTrovati,
                                            pivaAzienda,
                                            _parametriTask.ScriviLogAnagrafePianoColturale,
                                            _parametriTask.TagName,
                                            _parametriTask.ParametriContatti,
                                            _parametriTask.ReaplceInvalidPolygon,
                                            _parametriTask.SetAppezzaAddress,
                                            _parametriTask.EnableVerboseLogPCG,
                                            _parametriTask.CancellazioneLogica)
                        End If
                    Case 2
                        'aggiornamento
                        If cuaa.payload.Operazione = "U" Then
                            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] modifica ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            EseguiAggiornamento(cuaa,
                                                _webapiDataPublish,
                                                EsitoSync,
                                                listEsitiCUAA,
                                                pivaAzienda,
                                                _parametriTask.ScriviLogAnagrafePianoColturale,
                                                _parametriTask.TagName,
                                                _parametriTask.ParametriContatti,
                                                _parametriTask.ReaplceInvalidPolygon,
                                                _parametriTask.SetAppezzaAddress,
                                                _parametriTask.EnableVerboseLogPCG,
                                                _parametriTask.CancellazioneLogica)
                        Else
                            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Notifica di tipo creazione. Flusso di elaborazione per aggiornamenti. Notifica ignorata ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning)
                        End If
                    Case Else
                        _logger.AppendLog(cuaa, "", String.Format("Tipo operazione per importazione anagrafiche e piano colturale da Demetra non mappata. impossibile proseguire"), LogType.Errore)
                End Select
            Catch ex As ColdirettiIdentityProviderException
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "ColdirettiIdentityProvider", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
                EsitoSync.errors.Add(New SyncErrors() With {.code = "900", .msg = ex.Message, .key = ""})
            Catch ex As AgronicaCoreWSControllerException
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "AgronicaCoreWSController", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
                AddErroreGenericoSync(cuaa.payload.CUAA, EsitoSync, ex.Message)
            Catch ex As ColdirettiPDSException
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "ColdirettiPDS", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
                EsitoSync.errors.Add(New SyncErrors() With {.code = "800", .msg = ex.Message, .key = ""})
            Catch ex As DataPublishException
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
                EsitoSync.errors.Add(New SyncErrors() With {.code = "700", .msg = ex.Message, .key = ""})
            Catch ex As GiasException
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "Agronica", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore)
                EsitoSync.errors.Add(New SyncErrors() With {.code = "600", .msg = ex.Message, .key = ""})
            Catch ex As Exception
                RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore, ex)
                AddErroreGenericoSync(cuaa.payload.CUAA, EsitoSync, ex.Message)
            Finally
                EsitoSync.processed = If(EsitoSync.errors.Count > 0, False, True)
                If Not EsitoSync.processed Then

                    If _parametriTask.InviaAckEsito Then
                        Try
                            Dim notificaErrors = _webapiDataPublish.ChiamaWSNotificaOK(cuaa.payload.id_signal, EsitoSync)
                            If notificaErrors.ack Then
                                _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Notificata elaborazione in errore per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                                RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Notificata elaborazione in errore per id_signal {0}", cuaa.payload.id_signal), listEsitiCUAA)
                            Else
                                _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Errore in invio notifica elaborazione in errore per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id), LogType.Warning)
                                RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Errore in invio notifica elaborazione in errore per id_signal {0}", cuaa.payload.id_signal), listEsitiCUAA)
                            End If
                        Catch ex As DataPublishException
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                            _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore, ex)
                        Catch ex As Exception
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, True, ex.Message, listEsitiCUAA)
                            _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ex.Message), LogType.Errore, ex)
                        End Try
                    Else
                        _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Elaborazione ko per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                    End If

                    'in caso di errori loggo tutti gli errori riportati in esitosync per poter generare velocemente il report di elaborazione
                    Dim str_esito = ExtractDetailEsito(EsitoSync)
                    _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] riepilogo errori: {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, str_esito), bypassElastiSearch:=True)
                    _helper.SetEsitoNotifica(cuaa, str_esito.Replace(vbCrLf, "||"))
                Else
                    'reset esitotext se tutto ok
                    _helper.SetEsitoNotifica(cuaa, "")
                End If
            End Try

            'Lavez - 26/02/2024 - registrazione dettagli elaborazione notifica su agronica_log_invio_anagrafe
            Dim SyncEsitoAgronica As New SyncAcknowledge_Agronica With {
                .app_count = NumeroAppezzamentiTrovati,
                .entities = EsitoSync.entities,
                .processed = EsitoSync.processed
            }

            Dim dettaglio2 As New List(Of String)()
            If cuaa.payload.dettaglio.anagrafica IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.Imprese)
            End If
            If cuaa.payload.dettaglio.catasto IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.ParticelleCatastali)
            End If
            If cuaa.payload.dettaglio.pcg IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.Progetti)
            End If
            If cuaa.payload.dettaglio.pcg_catasto IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.AppezzamentiXParticelle)
            End If
            If cuaa.payload.dettaglio.equipaggiamenti IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.ParcoMacchine)
            End If
            If cuaa.payload.dettaglio.lavoratori IsNot Nothing Then
                dettaglio2.Add("Contatti")
            End If
            If cuaa.payload.dettaglio.gruppi_appezzamenti IsNot Nothing Then
                dettaglio2.Add(enum_TipoEntita_Des.Campi)

                If cuaa.payload.dettaglio.pcg Is Nothing Then
                    'La modifica ai campi aggiorna anche gli appezzamenti, quindi lo segno nella tabella di log, se non già segnato per modifica effettiva del PCG
                    dettaglio2.Add(enum_TipoEntita_Des.Progetti)
                End If

            End If

            Dim strDettaglio2 = String.Join("|", dettaglio2)

            RecordCUAA_Log(cuaa.id, cuaa.payload.CUAA, cuaa.payload.id_signal, cuaa.payload.Operazione, Not SyncEsitoAgronica.processed, SyncEsitoAgronica, EsitoSync.errors, _objParametriServer, pivaAzienda, pivaAzienda, strDettaglio2, "")

            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] fine elaborazione ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
            _logger.Flush(cuaa.payload.CUAA)
        Next


        RiepilogoMail = SummaryLogMail(TipoOperazione, listEsitiCUAA, _logger)
    End Sub

    Private Function CheckStartPoolCoreWS() As Boolean
        Dim ret As Boolean = False
        Dim hc As New HttpClient With {.BaseAddress = New Uri(_LinkCoreWS)}

        Dim resp As HttpResponseMessage = Nothing
        Try
            Dim req As New HttpRequestMessage(HttpMethod.Get, hc.BaseAddress.AbsoluteUri & "/Versione.aspx")

            resp = hc.SendAsync(req).GetAwaiter().GetResult()

            If Not resp.IsSuccessStatusCode Then
                Throw New Exception(resp.StatusCode.ToString() & " - " & resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
            End If
            ret = True
        Catch ex As Exception
            ret = False
            Throw New Exception(ex.Message, ex)

        End Try
        Return ret
    End Function

    Private Function ExtractDetailEsito(ByVal EsitoSync As SyncAcknowledge) As String
        Dim ret As String = ""
        Try
            For Each det In EsitoSync.errors
                ret &= String.Format("{0}" & vbCrLf, det.msg)
            Next
        Catch ex As Exception
            ret = ""
        End Try
        Return ret
    End Function

    Private Sub EseguiCreazione(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                ByRef _webapiDataPublish As WebApiDataPublish,
                                ByRef EsitoSync As SyncAcknowledge,
                                ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String)),
                                ByRef NumeroAppezzamentiTrovati As Integer,
                                ByRef pivaAzienda As String,
                                Optional ByVal ScriviLog As Boolean = True,
                                Optional ByVal TagName As String = "",
                                Optional ByVal ParametriContatti As ParametriInterscambioContatti = Nothing,
                                Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                Optional ByVal SetAppezzaAddress As Boolean = False,
                                Optional ByVal EnableVerboseLogPCG As Boolean = False,
                                Optional ByVal CancellazioneLogica As Boolean = False)


        Dim ObjParametri_Super_Server As AgronicaCoreParametri = _objParametriSuperServer.CreateDeepCopy(_objParametriSuperServer)
        Dim ObjParametri_Server As AgronicaCoreParametri = _objParametriServer.CreateDeepCopy(_objParametriServer)
        Dim ObjParametri_Utenti As AgronicaCoreParametri = _objParametriUtenti.CreateDeepCopy(_objParametriUtenti)

        ObjParametri_Super_Server.LogDirectory = _agronicacorelogDir
        ObjParametri_Server.LogDirectory = _agronicacorelogDir
        ObjParametri_Utenti.LogDirectory = _agronicacorelogDir
        ObjParametri_Super_Server.LogFileName = _agronicacorelogFile
        ObjParametri_Server.LogFileName = _agronicacorelogFile
        ObjParametri_Utenti.LogFileName = _agronicacorelogFile

        Dim _webProvisioningColdiretti As ColdirettiProvisioner = Nothing
        Dim _identityProvider As ColdirettiIdentityProvider = Nothing

        If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
            _webProvisioningColdiretti = New ColdirettiProvisioner(_parametriTask.ColdirettiProvisioningBaseUrl, New Tuple(Of String, String)("", ""))
            If _parametriTask.EnableIdentityProvider Then
                _identityProvider = New ColdirettiIdentityProvider(_parametriTask.ColdirettiAuthenticatorBaseUrl, _parametriTask.client_id, _parametriTask.grant_type, _parametriTask.username, _parametriTask.password)
            End If
        End If

        Dim _corews = New AgronicaCoreWSController(_LinkCoreWS,
                                       Nothing,
                                       ObjParametri_Super_Server,
                                       ObjParametri_Server,
                                       ObjParametri_Utenti,
                                       _logger)
        Dim _helper = New Helper(_parametriTask.ExtSysRef, ObjParametri_Server, _logger)

        Dim Pratica_cod As Integer = 0
        Dim keyCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK = Nothing

        Dim DataOraNotificaElaborazione As DateTime = Nothing

        'creazione
        cuaa.nretry += 1
        If cuaa.stato = 0 Then
            '0 - inserita -> crea anagrafica
            '0.1 - verifico se esiste l'impresa UZ dell'organismo detentore, se non c'è la creo
            Dim anag = _webapiDataPublish.ChiamaWSAnagrafica(cuaa.payload.CUAA)
            If anag IsNot Nothing Then
                'Lavez - 19/03/2025 - DataOraNotifica della reale elaborazione
                DataOraNotificaElaborazione = DateTime.UtcNow
                'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                If _helper.ControlliAnagrafica(cuaa, anag, _logger, EsitoSync, _parametriTask.TipoConfigurazione) Then
                    'Lavez - 24/02/2025 - Se sono in Regione Umbria devo il padre a pivasuperuser
                    If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria) Then
                        If anag.mandato Is Nothing Then
                            anag.mandato = New Mandato()
                        End If
                        'Lavez - 15/10/2025 - imposto detentore su regione umbria solo se non ho detentore
                        If anag.mandato.codice_detentore Is Nothing OrElse anag.mandato.codice_detentore.Trim = "" Then
                            anag.mandato.codice_detentore = ObjParametri_Server.PivaSuperUser
                        End If
                    Else
                        If Not _corews.VerificaScriviImpresaPadre(anag, _parametriTask.TipoConfigurazione) Then
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "011", .msg = "azienda organismo detentore non presente su Agronica", .key = anag.mandato.codice_detentore})
                            cuaa.stato = -1
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            Throw New GiasException(String.Format("[{0} - {1} - {2}] Errore in creazione azienda , azienda organismo detentore non presente su Agronica (cod_detentore: {3})", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, anag.mandato.codice_detentore))
                        End If
                    End If

                    If Not _corews.VerificaImpresa(anag) Then
                        Dim esito = _corews.ScriviModificaImpresa(cuaa, anag, keyCentro, _parametriTask.TipoConfigurazione)
                        If esito Then
                            EsitoSync.entities.Add(New SyncEntity() With {.entity = "azienda", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                            cuaa.stato = 1
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                            _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Creata impresa\centro\magazzino", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creata impresa\centro\magazzino", listEsitiCUAA)
                            'Lavez - 07/01/2025 - Solo se in modalità demetra
                            If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
                                If _corews.GeneraPratica_Gias(cuaa, keyCentro.partitaIva, cuaa.payload.Campagna, ID_SERVIZIO_DEMETRA_QDC, Pratica_cod) = False Then
                                    Throw New AgronicaCoreWSControllerException(String.Format("[{0} - {1} - {2}] Errore in generazione pratica workflow su agronica. impossibile proseguire", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                                End If
                            End If
                        End If

                    Else
                        cuaa.stato = 1
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Impresa già presente su Agronica", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                        RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Impresa già presente su Agronica", listEsitiCUAA)
                        'no workflow perchè esiste già l'azienda
                        If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
                            _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Generazione pratica ignorata per il servizio Demetra QDC", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                        Else
                            If cuaa.payload.dettaglio.anagrafica IsNot Nothing Then
                                EsitoSync.entities.Add(New SyncEntity() With {.entity = "azienda", .ref_timestamp = cuaa.payload.dettaglio.anagrafica.DataOraNotifica})
                            End If
                        End If
                    End If
                Else
                    cuaa.stato = -99
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New DataPublishException(String.Format("[{0} - {1} - {2}] Dati anagrafica azienda non conforme. Elaborazione notifica interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                End If
            Else
                cuaa.stato = -99
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                Throw New DataPublishException(String.Format("[{0} - {1} - {2}] Nessun dato di anagrafica restituito da DataPublish", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
            End If
        End If

        If keyCentro Is Nothing Then
            Try
                keyCentro = Helper.LeggiCentroPKDaCUAA(cuaa, _logger, ObjParametri_Server)
            Catch ex As DataPublishException
                cuaa.stato = -99
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                Throw New DataPublishException(ex.Message, ex)
            Finally
                If keyCentro Is Nothing Then
                    Throw New AgronicaCoreWSControllerException(String.Format("[{0} - {1} - {2}] Riferimenti centro aziendale non trovati. impossibile proseguire", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                End If
                pivaAzienda = keyCentro.partitaIva
            End Try
        End If
        'Lavez - 07/01/2025 - Solo se in modalità demetra
        If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
            If Pratica_cod = 0 Then
                _corews.RecuperaPraticaGias(keyCentro.partitaIva, ID_SERVIZIO_DEMETRA_QDC, New Date(cuaa.payload.Campagna, 1, 1), New Date(cuaa.payload.Campagna, 12, 31), Pratica_cod)
                If Pratica_cod = 0 Then
                    'lavez - 05/01/2024 - in caso di mancata presenza della pratica la rigenero
                    If _corews.GeneraPratica_Gias(cuaa, keyCentro.partitaIva, cuaa.payload.Campagna, ID_SERVIZIO_DEMETRA_QDC, Pratica_cod) = False Then
                        Throw New AgronicaCoreWSControllerException(String.Format("[{0} - {1} - {2}] Errore in generazione pratica workflow su agronica. impossibile proseguire", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    End If
                End If
            End If
        End If
        Dim bSkipCatasto As Boolean = False
        'Lavez - 04/03/2025 - nel caso di Regione umbria il metodo pcg_terreni non è disponibile
        If _parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria Then
            bSkipCatasto = True
        End If
        If cuaa.stato = 1 Then
            If cuaa.payload.dettaglio.catasto IsNot Nothing Then
                'Lavez - 19/03/2025 - DataOraNotifica della reale elaborazione
                DataOraNotificaElaborazione = DateTime.UtcNow
                '1- anagrafica inserita -> crea catasto
                Dim catasto = _webapiDataPublish.ChiamaWSTerreni(cuaa.payload.CUAA)
                If catasto IsNot Nothing Then
                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                    If _helper.ControlliCatasto(cuaa, catasto, _logger, EsitoSync) Then
                        If catasto.records.Count > 0 Then
                            Dim esito = _corews.ScriviCatasto(cuaa, catasto, keyCentro)
                            If esito Then
                                'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                                EsitoSync.entities.Add(New SyncEntity() With {.entity = "terreni", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                                cuaa.stato = 2
                                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Creato catasto ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            End If
                        Else
                            bSkipCatasto = True
                            cuaa.stato = 2
                            _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Catasto non presente ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning, bypassElastiSearch:=bSkipCatasto)
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Catasto non presente", listEsitiCUAA)
                        End If
                    Else
                        cuaa.stato = -2
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        Throw New DataPublishException(String.Format("[{0} - {1} - {2}] Catasto non conforme. Elaborazione notifica interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    End If
                Else
                    '30/10/2023 - catasto non obbligatorio
                    bSkipCatasto = True
                    cuaa.stato = 2
                    _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Catasto non presente ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning, bypassElastiSearch:=bSkipCatasto)
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Catasto non presente", listEsitiCUAA)
                End If
            Else
                bSkipCatasto = True
                cuaa.stato = 2
                _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Catasto non presente ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning, bypassElastiSearch:=bSkipCatasto)
            End If
        End If
        If cuaa.stato = 2 Then
            If cuaa.payload.dettaglio.pcg IsNot Nothing Then
                'Lavez - 19/03/2025 - DataOraNotifica della reale elaborazione
                DataOraNotificaElaborazione = DateTime.UtcNow
                '2- catasto inserito -> crea pcg
                Dim appezzamenti = _webapiDataPublish.ChiamaWSPianoColturaleGrafico(cuaa.payload.CUAA, cuaa.payload.Campagna)
                If appezzamenti IsNot Nothing Then
                    If appezzamenti.records.Count > 0 Then
                        'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                        _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )

                        If _parametriTask.EscludiPCGProvvisorio AndAlso appezzamenti.records.Where(Function(x) x.is_certified = False).ToList.Count > 0 Then
                            cuaa.stato = -50
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Rilevata notifica con appezzamenti provvisori. Elaborazione bloccata e notifica a Demetra non inviata", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Piano colturale PROVVISORIO non abilitato su GIAS. Elaborazione BLOCCATA", listEsitiCUAA)
                            EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg", .ref_timestamp = cuaa.payload.dettaglio.pcg.DataOraNotifica})
                            EsitoSync.errors.Add(New SyncErrors() With {.code = "555", .msg = "Piano colturale PROVVISORIO non abilitato su GIAS. Elaborazione BLOCCATA", .key = cuaa.payload.id_signal})
                        Else
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] operazione preliminare recupero anagrafica vincoli per associazione BIO\DPI se prensente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, appezzamenti.records.Count.ToString()), bypassElastiSearch:=True)
                            '0 - operazione preliminare - recupero elenco dei vincoli della campagna riferita alla notifica in elaborazione (onde evitare di floddare i webservices delle banche dati)
                            Dim VincoliList = AgronicaCoreWebService.Vincoli_WS.LeggiVincoli(New AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli() With {
                                                                                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale() With {
                                                                                    .inizio = New Date(cuaa.payload.Campagna, 1, 1),
                                                                                    .fine = New Date(cuaa.payload.Campagna, 12, 31)
                                                                                }
                                                                             },
                                                                             ObjParametri_Super_Server,
                                                                             ObjParametri_Server,
                                                                             ObjParametri_Utenti)
                            NumeroAppezzamentiTrovati = appezzamenti.records.Count
                            _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] trovati {3} appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, NumeroAppezzamentiTrovati.ToString()))
                            '0- precontrollo caricamento pcg
                            If Helper.ControlliPreCaricamentoPCG(cuaa, appezzamenti, ObjParametri_Server, _logger, EsitoSync, TagName, TipoConfigurazione:=_parametriTask.TipoConfigurazione) Then
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Controlli pre-elaborazione piano colturale OK", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)

                                Dim appezzamentiTerreni As AppezzamentiTerreni = Nothing
                                If Not bSkipCatasto Then
                                    appezzamentiTerreni = _webapiDataPublish.ChiamaWSAppezzamentiTerreni(cuaa.payload.CUAA,
                                                                                                         cuaa.payload.Campagna)
                                    If appezzamentiTerreni IsNot Nothing Then
                                        EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg-terreni", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                                    End If
                                End If
                                Dim elabAppezzamenti As Boolean = True
                                For Each appezzamento In appezzamenti.records
                                    Dim appezzamentiXParticelle As List(Of AppezzamentoTerreno) = Nothing
                                    If appezzamentiTerreni IsNot Nothing Then
                                        appezzamentiXParticelle = appezzamentiTerreni.records.Where(Function(x) x.id_appezzamento = appezzamento.id_appezzamento).ToList()
                                    Else
                                        _logger.AppendLog(cuaa, "pcg-terreni", String.Format("[{0} - {1} - {2}] ATTENZIONE - Ripartizione di catasto per appezzamento {3} non trovata. L'appezzamento verrà importato senza riferimenti alle particelle catastali", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, appezzamento.id_appezzamento), LogType.Warning, bypassElastiSearch:=True)
                                    End If


                                    Dim esito = _corews.ScriviModificaAppezzamenti(cuaa,
                                                   appezzamento,
                                                   appezzamentiXParticelle,
                                                   keyCentro,
                                                   ScriviLog:=ScriviLog,
                                                   VincoliList:=VincoliList,
                                                   TagName:=TagName,
                                                   ReplaceInvalidPolygon:=ReplaceInvalidPolygon,
                                                   TipoConfigurazione:=_parametriTask.TipoConfigurazione,
                                                   SetAppezzaAddress:=SetAppezzaAddress,
                                                   EnableVerboseLogPCG:=EnableVerboseLogPCG)
                                    If Not esito Then
                                        elabAppezzamenti = False
                                    End If
                                Next
                                If elabAppezzamenti Then
                                    EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                                    cuaa.stato = 3
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)

                                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               AGRODATAINIZIO,
                                                               DateTime.Now,
                                                               appezzamenti.records.Count,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               AGRODATAINIZIO,
                                                               AGRODATAFINE,
                                                               False
                                                               )

                                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Creato appezzamento/impianto/esercizio", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creato appezzamento/impianto/esercizio", listEsitiCUAA)
                                    'Lavez - 07/01/2025 - Solo se in modalità demetra
                                    If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
                                        If Pratica_cod > 0 Then
                                            'avanzo la pratica solo se ho il pratica_cod in linea e lo stato è, diversamente sono nel caso di un'azienda già esistente per cui non devo segnalare che il QDC è disponibile
                                            If _corews.VerificaPraticaGiasXAvanzamentoDiStato(Pratica_cod) Then
                                                If _corews.AvanzaPraticaGias(cuaa,
                                                                         keyCentro.partitaIva,
                                                                         ID_SERVIZIO_DEMETRA_QDC,
                                                                         Pratica_cod,
                                                                         STATO_SERVIZIO_DEMETRA_QDC_QUADERNO_OK) = False Then
                                                    Throw New AgronicaCoreWSControllerException(String.Format("[{0} - {1} - {2}] Errore In avanzamento di stato pratica ({3}) agronica (newState= {4}) impossibile proseguire", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, Pratica_cod.ToString(), STATO_SERVIZIO_DEMETRA_QDC_QUADERNO_OK))
                                                Else
                                                    If _parametriTask.EnableNotifyToProvisioning Then
                                                        If Not _webProvisioningColdiretti.NotificaQDC_OK(cuaa.payload.CUAA) Then
                                                            Throw New ColdirettiPDSException(String.Format("[{0} - {1} - {2}] Errore notifica QDC ok a provisioning coldiretti.", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                                                        End If
                                                    Else
                                                        _logger.AppendLog(cuaa, "ColdirettiPDS", String.Format("[{0} - {1} - {2}] Notifica azienda attiva a provisioning coldiretti disattivato.", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), LogType.Warning)
                                                    End If
                                                End If

                                            End If

                                        End If
                                    End If
                                End If
                            Else
                                cuaa.stato = -3
                                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                Throw New DataPublishException(String.Format("[{0} - {1} - {2}] Piano colturale grafico non conforme. Elaborazione notifica interrotta", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            End If
                        End If
                    Else
                        cuaa.stato = -3
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        Throw New DataPublishException(String.Format("[{0} - {1} - {2}] Nessun piano colturale grafico trovato", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    End If
                End If
            Else
                If cuaa.payload.dettaglio.equipaggiamenti Is Nothing AndAlso cuaa.payload.dettaglio.lavoratori Is Nothing AndAlso cuaa.payload.dettaglio.catasto Is Nothing Then
                    'Lavez - 13/05/2025 - disattivato errore notifica senza pcg per gestire i casi delle notifiche disgiunte tra anagrafica e pcg
                    'If _parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra Then
                    '    'lavez - 10/01/2024 - blocco solo nel caso in cui lavoratori ed equipaggiamenti siano entrambi a null, questo per poter gestire i casi dove arriva la notifica di creazione
                    '    '                     di un equipaggiamento o un lavoratore che devono essere importati su gias ma non sono legati alla campagna
                    '    cuaa.stato = -3
                    '    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    '    Throw New DataPublishException("Nessun piano colturale grafico trovato")
                    'Else
                    'Lavez - 26/02/2025 - Se sono in regione umbria le notifiche delle singole entita sono disgiunte e non devo bloccare
                    cuaa.stato = 3
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Nessun piano colturale grafico trovato", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Nessun piano colturale grafico trovato", listEsitiCUAA)
                    'End If
                Else
                    cuaa.stato = 3
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Rilevati catasto/equipaggiamenti/lavoratori senza piano colturale grafico, proseguo con l'elaborazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Catasto/Equipaggiamenti/Lavoratori senza piano colturale", listEsitiCUAA)
                End If
            End If
        End If
        If cuaa.stato = 3 Then
            'Lavez - 07/01/2025 - Solo se in modalità demetra
            If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra) Then
                Try
                    '2- anagrafica e catasto inseriti -> creo utente azienda agricola
                    Dim user = _webProvisioningColdiretti.GetUserFromCUAA_PersonaGiuridica(cuaa.payload.CUAA)
                    If user Is Nothing Then
                        user = _webProvisioningColdiretti.GetUserFromCUAA_PersonaFisica(cuaa.payload.CUAA)
                    End If
                    If user IsNot Nothing Then

                        'Dim userMail = _identityProvider.GetKeyCloackUserMail(_parametriTask.realm, _helper.GetUsernameFromUserColdiretti(user, _logger))
                        Dim userMail = _helper.GetUsernameFromUserColdiretti(cuaa, user, _logger)
                        If userMail = "" Then
                            cuaa.stato = 4
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            _logger.AppendLog(cuaa, "user", String.Format("[{0} - {1} - {2}] Impossibile recuperare i dati per la creazione dell'utente gias. Problema sui dati restituiti dal provisioning", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Impossibile recuperare i dati per la creazione dell'utente gias. Problema sui dati restituiti dal provisioning"), listEsitiCUAA)
                        Else
                            '-- verifica esistenza utente (lo creo solo se non esiste)
                            If Not _corews.CheckUtenteAziendaAgricola(cuaa, user, userMail) Then
                                '-- creo utente gias
                                Dim esito = _corews.CreaUtenteAziendaAgricola(cuaa, user, userMail, keyCentro.partitaIva, _parametriTask.DefaultProfile)
                                If esito Then
                                    cuaa.stato = 4
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                    _logger.AppendLog(cuaa, "user", String.Format("[{0} - {1} - {2}] Creato utente gias {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, userMail))
                                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creato utente gias", listEsitiCUAA)
                                Else
                                    Throw New AgronicaCoreWSControllerException("Errore creazione utente Agronica. impossibile proseguire.")
                                End If
                            Else
                                cuaa.stato = 4
                                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                _logger.AppendLog(cuaa, "user", String.Format("[{0} - {1} - {2}] Utente gias {3} già presente su Agronica", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, userMail), bypassElastiSearch:=True)
                                RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Utente già presente", listEsitiCUAA)
                            End If

                            'non più valida dopo la call del 12/10/2023
                            'If Pratica_cod > 0 Then
                            '    'avanzo la pratica solo se ho il pratica_cod in linea, diversamente sono nel caso di un'azienda già esistente per cui l'utente deve essere già a posto
                            '    If _corews.AvanzaPraticaGias(keyCentro.partitaIva, ID_SERVIZIO_DEMETRA_QDC, Pratica_cod, STATO_SERVIZIO_DEMETRA_QDC_UTENTE_OK) = False Then
                            '        Throw New AgronicaCoreWSControllerException(String.Format("[{0} - {1} - {2}] Errore in avanzamento di stato pratica ({3}) agronica (newState= {4}) impossibile proseguire", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, Pratica_cod.ToString(), STATO_SERVIZIO_DEMETRA_QDC_UTENTE_OK))
                            '    End If
                            'End If
                        End If
                    Else
                        cuaa.stato = 4
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        _logger.AppendLog(cuaa, "user", String.Format("[{0} - {1} - {2}] Impossibile recuperare codice tessera da CUAA {0} . Creazione utente non possibile ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                        RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Impossibile recuperare codice tessera da CUAA {0} . Creazione utente non possibile", cuaa.payload.CUAA), listEsitiCUAA)
                        'Throw New ColdirettiPDSException(String.Format("[{0} - {1} - {2}] Nessun utente coldiretti trovato. impossibile proseguire.", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    End If
                Catch ex As ColdirettiPDSException
                    cuaa.stato = 4
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "user", String.Format("[{0} - {1} - {2}] Impossibile creare utente gias. Errore in chiamata provisioning coldiretti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal) & vbCrLf & ex.Message, LogType.Errore, ex)
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Impossibile creare utente gias. Errore in chiamata provisioning coldiretti"), listEsitiCUAA)
                Catch ex As Exception
                    cuaa.stato = -4
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New Exception(ex.Message, ex)
                End Try
            Else
                cuaa.stato = 4
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            End If
        End If
        If cuaa.stato = 4 Then
            'macchine
            If cuaa.payload.dettaglio.equipaggiamenti IsNot Nothing Then
                Try
                    'Lavez - 19/03/2025 - DataOraNotifica della reale elaborazione
                    DataOraNotificaElaborazione = DateTime.UtcNow
                    Dim macchine = _webapiDataPublish.ChiamaWSEquipaggiamenti(cuaa.payload.CUAA)
                    If macchine IsNot Nothing Then
                        If macchine.records.Count > 0 Then
                            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                            _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] trovate {3} macchine", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchine.records.Count.ToString()))
                            '0- precontrollo caricamento equipaggiamenti
                            If Helper.ControlliPreCaricamentoEquipaggiamenti(cuaa, macchine, ObjParametri_Server, _logger, EsitoSync) Then
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Controlli pre-elaborazione macchine OK", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                                Dim elabMacchine As Boolean = True
                                For Each macchina In macchine.records
                                    Dim esito = _corews.ScriviModifica_Macchina(cuaa, keyCentro, macchina, EsitoSync)
                                    If Not esito Then
                                        elabMacchine = False
                                    End If
                                Next
                                If elabMacchine Then
                                    EsitoSync.entities.Add(New SyncEntity() With {.entity = "equipaggiamenti", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                                    cuaa.stato = 5
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                                    _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Creato parco macchine", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creato parco macchine", listEsitiCUAA)
                                Else
                                    cuaa.stato = -5
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                    Throw New DataPublishException("Errore in elaborazione elenco macchine.")
                                End If
                            Else
                                cuaa.stato = -5
                                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                Throw New DataPublishException("Elenco macchine non conforme. Elaborazione notifica interrotta")
                            End If
                        Else
                            cuaa.stato = 5
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Nessun elenco macchine disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Elenco macchine non presente"), listEsitiCUAA)
                        End If
                    Else
                        cuaa.stato = 5
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Nessun elenco macchine disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                        RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Elenco macchine non presente"), listEsitiCUAA)
                    End If
                Catch ex As Exception
                    cuaa.stato = -5
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New Exception(ex.Message, ex)
                End Try
            Else
                cuaa.stato = 5
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Nessun elenco macchine disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If cuaa.stato = 5 Then
            'operatori/contatti
            If cuaa.payload.dettaglio.lavoratori IsNot Nothing Then
                Try
                    'Lavez - 19/03/2025 - DataOraNotifica della reale elaborazione
                    DataOraNotificaElaborazione = DateTime.UtcNow
                    Dim contatti = _webapiDataPublish.ChiamaWSLavoratori(cuaa.payload.CUAA)
                    If contatti IsNot Nothing Then
                        If contatti.records.Count > 0 Then
                            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                            _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] trovati {3} lavoratori", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, contatti.records.Count.ToString()))
                            '0- precontrollo caricamento equipaggiamenti
                            If Helper.ControlliPreCaricamentoLavoratori(cuaa, contatti, ObjParametri_Server, _logger, EsitoSync) Then
                                _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Controlli pre-elaborazione lavoratori OK", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                                Dim elabContatti As Boolean = True
                                For Each contatto In contatti.records
                                    Dim ErroreImport As String = ""
                                    Dim esito = _corews.ScriviModificaCancella_Contatto(cuaa.payload.CUAA, contatto, ErroreImport, ParametriContatti)
                                    If Not esito Then
                                        _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Errore in creazione contatto: {4} - {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, ErroreImport, contatto.codice))
                                        elabContatti = False
                                    End If
                                Next
                                EsitoSync.entities.Add(New SyncEntity() With {.entity = "lavoratori", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                                If elabContatti Then
                                    cuaa.stato = 6
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                                    _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Creati contatti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creati contatti", listEsitiCUAA)
                                Else
                                    cuaa.stato = -6
                                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                    Throw New DataPublishException("Elenco lavoratori. Elaborazione notifica interrotta")
                                End If
                            Else
                                cuaa.stato = -6
                                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                                Throw New DataPublishException("Elenco lavoratori. Elaborazione notifica interrotta")
                            End If
                        Else
                            cuaa.stato = 6
                            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                            _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Nessun elenco lavoratori disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                            RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Elenco lavoratori non presente"), listEsitiCUAA)
                        End If
                    Else
                        cuaa.stato = 6
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Nessun elenco lavoratori disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                        RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Elenco lavoratori non presente"), listEsitiCUAA)
                    End If
                Catch ex As Exception
                    cuaa.stato = -6
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New Exception(ex.Message, ex)
                End Try
            Else
                cuaa.stato = 6
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Nessun elenco lavoratori disponibile", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If

        If cuaa.stato = 6 Then
            If cuaa.payload.dettaglio.gruppi_appezzamenti IsNot Nothing Then
                DataOraNotificaElaborazione = DateTime.UtcNow
                EseguiCreazioneGruppiAppezzamenti(cuaa,
                    keyCentro,
                    _corews,
                    _webapiDataPublish,
                    _helper,
                    ObjParametri_Server,
                    ObjParametri_Utenti,
                    EsitoSync,
                    listEsitiCUAA)
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "gruppi_appezzamenti", .ref_timestamp = DataOraNotificaElaborazione.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Creati gruppi appezzamenti (alias campi)", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Creato gruppi appezzamenti (alias campi)", listEsitiCUAA)
            Else
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Nessun elenco gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If

        If cuaa.stato = 7 Then
            EsitoSync.processed = True
            If _parametriTask.InviaAckEsito Then
                Dim notificaok = _webapiDataPublish.ChiamaWSNotificaOK(cuaa.payload.id_signal, EsitoSync)
                If notificaok.ack Then
                    cuaa.stato = 10
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Notificata elaborazione ok per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Notificata elaborazione ok per id_signal {0}", cuaa.payload.id_signal), listEsitiCUAA)
                Else
                    _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Notificata elaborazione ok per id_record {3} ma ricevuto acknowledge di ritorno a false", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Notificata elaborazione ok per id_signal {0} ma ricevuto acknowledge di ritorno a false", cuaa.payload.id_signal), listEsitiCUAA)
                End If
            Else
                cuaa.stato = 10
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Elaborazione completata per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
            End If
        End If
    End Sub


    Private Sub EseguiCreazioneGruppiAppezzamenti(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                    ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                    ByRef _corews As AgronicaCoreWSController,
                                                    ByRef _webapiDataPublish As WebApiDataPublish,
                                                    ByRef _helper As Helper,
                                                    ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                    ByRef ObjParametri_Utenti As AgronicaCoreParametri,
                                                    ByRef EsitoSync As SyncAcknowledge,
                                                    ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))
                                                    )

        Try
            Dim gruppi_appezzamenti = _webapiDataPublish.ChiamaWSGruppiAppezzamenti(cuaa.payload.CUAA)

            If gruppi_appezzamenti IsNot Nothing AndAlso gruppi_appezzamenti.records.Count > 0 Then
                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                -1,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                DateTime.Now,
                                                AGRODATAFINE,
                                                False
                                                )
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] trovati {3} gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, gruppi_appezzamenti.records.Count.ToString()))

                Dim handleIECampi As New AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti(ObjParametri_Server, ObjParametri_Utenti)

                For Each gruppo_appezzamento In gruppi_appezzamenti.records
                    EseguiImportGruppiAppezzamenti(cuaa,
                        gruppo_appezzamento,
                        CentroPK,
                        handleIECampi,
                        _corews,
                        _webapiDataPublish,
                        _helper,
                        ObjParametri_Server,
                        ObjParametri_Utenti,
                        EsitoSync,
                        listEsitiCUAA)
                Next

                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                -1,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                DateTime.Now,
                                                False
                                                )

            Else
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Nessun elenco gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If

        Catch ex As Exception
            cuaa.stato = -7
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException("Errore in elaborazione elenco gruppi appezzamenti: " & ex.Message)
        End Try

    End Sub


    Private Sub EseguiAggiornamento(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                    ByRef _webapiDataPublish As WebApiDataPublish,
                                    ByRef EsitoSync As SyncAcknowledge,
                                    ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String)),
                                    ByRef pivaAzienda As String,
                                    Optional ByVal ScriviLog As Boolean = True,
                                    Optional ByVal TagName As String = "",
                                    Optional ByVal ParametriContati As ParametriInterscambioContatti = Nothing,
                                    Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                    Optional ByVal SetAppezzaAddress As Boolean = False,
                                    Optional ByVal EnableVerboseLogPCG As Boolean = False,
                                    Optional ByVal CancellazioneLogica As Boolean = False)



        Dim ObjParametri_Super_Server As AgronicaCoreParametri = _objParametriSuperServer.CreateDeepCopy(_objParametriSuperServer)
        Dim ObjParametri_Server As AgronicaCoreParametri = _objParametriServer.CreateDeepCopy(_objParametriServer)
        Dim ObjParametri_Utenti As AgronicaCoreParametri = _objParametriUtenti.CreateDeepCopy(_objParametriUtenti)

        ObjParametri_Super_Server.LogDirectory = _agronicacorelogDir
        ObjParametri_Server.LogDirectory = _agronicacorelogDir
        ObjParametri_Utenti.LogDirectory = _agronicacorelogDir
        ObjParametri_Super_Server.LogFileName = _agronicacorelogFile
        ObjParametri_Server.LogFileName = _agronicacorelogFile
        ObjParametri_Utenti.LogFileName = _agronicacorelogFile

        Dim _webProvisioningColdiretti As ColdirettiProvisioner = Nothing
        Dim _identityProvider As ColdirettiIdentityProvider = Nothing

        If _parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.Demetra Then
            _webProvisioningColdiretti = New ColdirettiProvisioner(_parametriTask.ColdirettiProvisioningBaseUrl, New Tuple(Of String, String)("", ""))
            If _parametriTask.EnableIdentityProvider Then
                _identityProvider = New ColdirettiIdentityProvider(_parametriTask.ColdirettiAuthenticatorBaseUrl, _parametriTask.client_id, _parametriTask.grant_type, _parametriTask.username, _parametriTask.password)
            End If
        End If



        Dim _corews = New AgronicaCoreWSController(_LinkCoreWS,
                                       Nothing,
                                       ObjParametri_Super_Server,
                                       ObjParametri_Server,
                                       ObjParametri_Utenti,
                                       _logger)
        Dim _helper = New Helper(_parametriTask.ExtSysRef, ObjParametri_Server, _logger)

        Dim Pratica_cod As Integer = 0
        Dim keyCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK = Nothing

        Dim Ack = New SyncAcknowledge() With {.processed = True}

        '0- check esistenza record di tipo creazione per il CUAA in elaborazione
        If _helper.CheckIfExistCreationToElabForCUAA(cuaa.payload.CUAA, cuaa.payload.Campagna) = True Then
            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Aggiornamento - esiste un record di tipo creazione per lo stesso cuaa\campagna ({3}) in attesa di essere elaborato. Record ignorato", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.payload.Campagna), bypassElastiSearch:=True)
            Return
        End If

        'creazione
        cuaa.nretry += 1
        Dim esiti As New List(Of Boolean)
        If cuaa.stato = 0 Then
            If cuaa.payload.dettaglio.anagrafica IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoAnagrafica(cuaa, _corews, _webapiDataPublish, _helper, ObjParametri_Server, EsitoSync, listEsitiCUAA))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "azienda", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 1
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento anagrafico", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If keyCentro Is Nothing Then
            Try
                keyCentro = Helper.LeggiCentroPKDaCUAA(cuaa, _logger, ObjParametri_Server)
            Catch ex As DataPublishException
                cuaa.stato = -99
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                Throw New DataPublishException(ex.Message, ex)
            Finally
                If keyCentro Is Nothing Then
                    Throw New AgronicaCoreWSControllerException("Riferimenti centro aziendale non trovati. impossibile proseguire")
                End If
                pivaAzienda = keyCentro.partitaIva
            End Try
        End If
        If cuaa.stato = 1 Then
            If cuaa.payload.dettaglio.catasto IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoCatasto(cuaa, cuaa.payload.dettaglio.catasto, keyCentro, _corews, _webapiDataPublish, _helper, ObjParametri_Server, EsitoSync, listEsitiCUAA))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "terreni", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 2
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento catasto", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If cuaa.stato = 2 Then
            If cuaa.payload.dettaglio.pcg_catasto IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoRipartizioneCatastalePCG(cuaa, cuaa.payload.dettaglio.pcg_catasto, keyCentro, _corews, _webapiDataPublish, _helper, ObjParametri_Server, EsitoSync, listEsitiCUAA))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg-terreni", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 3
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "pcg_terreni", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento ripartizione catastale su piano colturale grafico", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If cuaa.stato = 3 Then
            If cuaa.payload.dettaglio.pcg IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoPCG(cuaa,
                                                 cuaa.payload.dettaglio.pcg,
                                                 keyCentro,
                                                 _corews,
                                                 _webapiDataPublish,
                                                 _helper,
                                                 ObjParametri_Super_Server,
                                                 ObjParametri_Server,
                                                 ObjParametri_Utenti,
                                                 EsitoSync,
                                                 listEsitiCUAA,
                                                 ScriviLog,
                                                 TagName,
                                                 ReplaceInvalidPolygon,
                                                 _parametriTask.TipoConfigurazione,
                                                 SetAppezzaAddress,
                                                 EnableVerboseLogPCG,
                                                 CancellazioneLogica))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 4
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento piano colturale grafico", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If cuaa.stato = 4 Then
            If cuaa.payload.dettaglio.equipaggiamenti IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoParcoMacchine(cuaa, cuaa.payload.dettaglio.equipaggiamenti, keyCentro, _corews, _webapiDataPublish, _helper, ObjParametri_Server, EsitoSync, listEsitiCUAA))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "equipaggiamenti", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 5
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento equipaggiamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If
        If cuaa.stato = 5 Then
            If cuaa.payload.dettaglio.lavoratori IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                esiti.Add(EseguiAggiornamentoLavoratori(cuaa, cuaa.payload.dettaglio.lavoratori, keyCentro, _corews, _webapiDataPublish, _helper, ObjParametri_Server, EsitoSync, listEsitiCUAA, ParametriContati))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "lavoratori", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
            Else
                cuaa.stato = 6
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "lavoratori", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento lavoratori", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If

        If cuaa.stato = 6 Then
            If cuaa.payload.dettaglio.gruppi_appezzamenti IsNot Nothing Then
                Dim timestampElab As DateTime = DateTime.UtcNow
                EseguiAggiornamentoGruppiAppezzamenti(cuaa,
                    cuaa.payload.dettaglio.gruppi_appezzamenti,
                    keyCentro,
                    _corews,
                    _webapiDataPublish,
                    _helper,
                    ObjParametri_Server,
                    ObjParametri_Utenti,
                    EsitoSync,
                    listEsitiCUAA)

                esiti.Add(True)
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "gruppi_appezzamenti", .ref_timestamp = timestampElab.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")})
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Modifica gruppi appezzamenti (alias campi) eseguita correttamente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
            Else
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Aggiornamento - Nessun aggiornamento gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If
        End If

        If cuaa.stato = 7 Then
            EsitoSync.processed = True
            If _parametriTask.InviaAckEsito Then
                Dim notificaok = _webapiDataPublish.ChiamaWSNotificaOK(cuaa.payload.id_signal, EsitoSync)
                If notificaok.ack Then
                    cuaa.stato = 10
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Notificata elaborazione ok per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Notificata elaborazione ok per id_signal {0}", cuaa.payload.id_signal), listEsitiCUAA)
                Else
                    _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Notificata elaborazione ok per id_record {3} ma ricevuto acknowledge di ritorno a false", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
                    RegistraEventoRiepilogo(cuaa.payload.CUAA, False, String.Format("Notificata elaborazione ok per id_signal {0} ma ricevuto acknowledge di ritorno a false", cuaa.payload.id_signal), listEsitiCUAA)
                End If
            Else
                cuaa.stato = 10
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "DataPublish", String.Format("[{0} - {1} - {2}] Elaborazione completata per id_record {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, cuaa.id))
            End If
        End If
        If esiti.Where(Function(x) x = False).ToList().Count > 0 Then
            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Aggiornamento - riscontrati errori In fase di elaborazione", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
        Else
            _logger.AppendLog(cuaa, "", String.Format("[{0} - {1} - {2}] Aggiornamento - Eseguito correttamente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
        End If
    End Sub

    Private Sub EseguiAggiornamentoGruppiAppezzamenti(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                    ByVal changeNotification As CUAAObj_Detail_Notification,
                                                    ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                    ByRef _corews As AgronicaCoreWSController,
                                                    ByRef _webapiDataPublish As WebApiDataPublish,
                                                    ByRef _helper As Helper,
                                                    ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                    ByRef ObjParametri_Utenti As AgronicaCoreParametri,
                                                    ByRef EsitoSync As SyncAcknowledge,
                                                    ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))
                                                    )

        Try
            Dim gruppi_appezzamenti = _webapiDataPublish.ChiamaWSVariazioniGruppiAppezzamenti(cuaa.payload.CUAA, changeNotification.DataOraNotifica)

            If gruppi_appezzamenti IsNot Nothing AndAlso gruppi_appezzamenti.records.Count > 0 Then
                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                -1,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                AGRODATAINIZIO,
                                                AGRODATAFINE,
                                                DateTime.Now,
                                                AGRODATAFINE,
                                                False
                                                )
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] trovati {3} gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, gruppi_appezzamenti.records.Count.ToString()))

                Dim handleIECampi As New AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti(ObjParametri_Server, ObjParametri_Utenti)

                For Each gruppo_appezzamento In gruppi_appezzamenti.records
                    EseguiImportGruppiAppezzamenti(cuaa,
                        gruppo_appezzamento,
                        CentroPK,
                        handleIECampi,
                        _corews,
                        _webapiDataPublish,
                        _helper,
                        ObjParametri_Server,
                        ObjParametri_Utenti,
                        EsitoSync,
                        listEsitiCUAA)
                Next


                _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                -1,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                AGRODATAINIZIO,
                                AGRODATAFINE,
                                AGRODATAINIZIO,
                                DateTime.Now,
                                False
                                )

            Else
                cuaa.stato = 7
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "gruppi_appezzamenti", String.Format("[{0} - {1} - {2}] Nessun elenco gruppi_appezzamenti", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
            End If

        Catch ex As Exception
            cuaa.stato = -7
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException("Errore in elaborazione elenco gruppi appezzamenti: " & ex.Message)
        End Try

    End Sub

    Private Function EseguiAggiornamentoAnagrafica(ByRef cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                   ByRef _corews As AgronicaCoreWSController,
                                                   ByRef _webapiDataPublish As WebApiDataPublish,
                                                   ByRef _helper As Helper,
                                                   ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                   ByRef EsitoSync As SyncAcknowledge,
                                                   ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))
                                                   ) As Boolean

        Dim ret As Boolean = False
        Dim anag = _webapiDataPublish.ChiamaWSAnagrafica(cuaa.payload.CUAA)
        If anag IsNot Nothing Then
            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
            If _helper.ControlliAnagrafica(cuaa, anag, _logger, EsitoSync, _parametriTask.TipoConfigurazione) Then
                'Lavez - 24/02/2025 - Se sono in Regione Umbria devo il padre a pivasuperuser
                If (_parametriTask.TipoConfigurazione = enum_DataPublish_Configurazione.RegioneUmbria) Then
                    If anag.mandato Is Nothing Then
                        anag.mandato = New Mandato
                    End If
                    'Lavez - 15/10/2025 - imposto detentore su regione umbria solo se non ho detentore
                    If anag.mandato.codice_detentore Is Nothing OrElse anag.mandato.codice_detentore.Trim = "" Then
                        _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Aggiornamento - Impostato organismo detentore a PivaSuperUser per Regione Umbria", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                        anag.mandato.codice_detentore = ObjParametri_Server.PivaSuperUser
                    End If
                Else
                    If Not _corews.VerificaScriviImpresaPadre(anag, _parametriTask.TipoConfigurazione) Then
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "011", .msg = "azienda organismo detentore non presente su Agronica", .key = anag.mandato.codice_detentore})
                        Throw New GiasException(String.Format("Errore In aggiornamento azienda , azienda organismo detentore non presente su Agronica (cod_detentore: {0})", anag.mandato.codice_detentore))
                    End If
                End If
                If _corews.VerificaImpresa(anag) Then
                    Dim keyCentro As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK = Nothing
                    Dim esito = _corews.ScriviModificaImpresa(cuaa, anag, keyCentro, _parametriTask.TipoConfigurazione)
                    If esito Then
                        cuaa.stato = 1
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                        _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                        _logger.AppendLog(cuaa, "anagrafica", String.Format("[{0} - {1} - {2}] Aggiornata impresa\centro", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                        RegistraEventoRiepilogo(cuaa.payload.CUAA, False, "Aggiornata impresa\centro", listEsitiCUAA)
                        ret = True
                    Else
                        cuaa.stato = -99
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        Throw New AgronicaCoreWSControllerException("Errore in aggiornamento azienda , dato non presente su Agronica ")
                    End If
                Else
                    cuaa.stato = -99
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New AgronicaCoreWSControllerException("Errore in aggiornamento azienda , dato non presente su Agronica ")
                End If
            Else
                cuaa.stato = -99
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                Throw New DataPublishException("Dati anagrafica azienda non conforme. Elaborazione notifica interrotta")
            End If
            ret = True
        Else
            cuaa.stato = -99
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Nessuna azienda trovata per il CUAA indicato {0} . Elaborazione notifica interrotta", cuaa.payload.CUAA))
        End If

        Return ret
    End Function

    Private Function EseguiAggiornamentoCatasto(ByRef cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                ByVal catasto_change_notification As CUAAObj_Detail_Notification,
                                                ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                ByRef _corews As AgronicaCoreWSController,
                                                ByRef _webapiDataPublish As WebApiDataPublish,
                                                ByRef _helper As Helper,
                                                ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                ByRef EsitoSync As SyncAcknowledge,
                                                ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))) As Boolean
        Dim ret As Boolean = False
        Dim catasto_changes = _webapiDataPublish.ChiamaWSVariazioniCatasto(cuaa.payload.CUAA, catasto_change_notification.DataOraNotifica)
        If catasto_changes Is Nothing OrElse catasto_changes.records.Count <= 0 Then
            cuaa.stato = -1
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Aggiornamento catasto timestamp_ref: {0}. Ricevuta notifica di aggiornamento senza alcun dettaglio", catasto_change_notification.DataOraNotifica))
        Else
            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
            If _helper.ControlliVariazioniCatasto(cuaa, catasto_changes, _logger, EsitoSync) Then
                Dim esito = _corews.ModificaCatasto(cuaa, catasto_changes, CentroPK)
                If esito Then
                    cuaa.stato = 2
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           DateTime.Now,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
                    _logger.AppendLog(cuaa, "terreni", String.Format("[{0} - {1} - {2}] Modificato catasto ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    ret = True
                Else
                    cuaa.stato = -1
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New DataPublishException("Errore in aggiornamento catasto. Elaborazione notifica interrotta")
                End If
            Else
                cuaa.stato = -1
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                Throw New DataPublishException("Catasto non conforme. Elaborazione notifica interrotta")
            End If
        End If

        Return ret
    End Function


    Private Function EseguiAggiornamentoPCG(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                            ByVal pcg_change_notification As CUAAObj_Detail_Notification,
                                            ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                            ByRef _corews As AgronicaCoreWSController,
                                            ByRef _webapiDataPublish As WebApiDataPublish,
                                            ByRef _helper As Helper,
                                            ByRef ObjParametri_SuperServer As AgronicaCoreParametri,
                                            ByRef ObjParametri_Server As AgronicaCoreParametri,
                                            ByRef ObjParametri_Utenti As AgronicaCoreParametri,
                                            ByRef EsitoSync As SyncAcknowledge,
                                            ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String)),
                                            Optional ScriviLog As Boolean = True,
                                            Optional ByVal TagName As String = "",
                                            Optional ByVal ReplaceInvalidPolygon As Boolean = True,
                                            Optional ByVal TipoConfigurazione As enum_DataPublish_Configurazione = enum_DataPublish_Configurazione.Demetra,
                                            Optional ByVal SetAppezzaAddress As Boolean = False,
                                            Optional ByVal EnableVerboseLogPCG As Boolean = False,
                                            Optional ByVal CancellazioneLogica As Boolean = False
                                            ) As Boolean
        Dim ret As Boolean = False

        Dim appezzamento_changes = _webapiDataPublish.ChiamaWSVariazioniPCG_CUAA(cuaa.payload.CUAA, cuaa.payload.Campagna, pcg_change_notification.DataOraNotifica)

        If appezzamento_changes Is Nothing OrElse appezzamento_changes.records.Count <= 0 Then
            cuaa.stato = -3
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Aggiornamento piano colturale grafico timestamp_ref: {0}. Ricevuta notifica di aggiornamento senza alcun dettaglio", pcg_change_notification.DataOraNotifica))
        Else
            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
            If _parametriTask.EscludiPCGProvvisorio AndAlso appezzamento_changes.records.Where(Function(x) x.is_certified = False AndAlso x.tipo_modifica <> "C").ToList.Count > 0 Then
                cuaa.stato = -50
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Rilevata notifica con appezzamenti provvisori. Elaborazione bloccata e notifica a Demetra non inviata", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                EsitoSync.entities.Add(New SyncEntity() With {.entity = "pcg", .ref_timestamp = cuaa.payload.dettaglio.pcg.DataOraNotifica})
                EsitoSync.errors.Add(New SyncErrors() With {.code = "555", .msg = "Piano colturale PROVVISORIO non abilitato su GIAS. Elaborazione BLOCCATA", .key = cuaa.payload.id_signal})
            Else
                If _helper.ControlliPreAggiornamentoPCG(cuaa, appezzamento_changes, ObjParametri_Server, _logger, EsitoSync, TagName, TipoConfigurazione) Then
                    _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] operazione preliminare recupero anagrafica vincoli per associazione BIO\DPI se prensente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal), bypassElastiSearch:=True)
                    '0 - operazione preliminare - recupero elenco dei vincoli della campagna riferita alla notifica in elaborazione (onde evitare di floddare i webservices delle banche dati)
                    Dim VincoliList = AgronicaCoreWebService.Vincoli_WS.LeggiVincoli(New AgronicaCoreDTOStd.InData.Metaschema.LeggiVincoli() With {
                                                                                .validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale() With {
                                                                                    .inizio = New Date(cuaa.payload.Campagna, 1, 1),
                                                                                    .fine = New Date(cuaa.payload.Campagna, 12, 31)
                                                                                }
                                                                             },
                                                                             ObjParametri_SuperServer,
                                                                             ObjParametri_Server,
                                                                             ObjParametri_Utenti)

                    Dim esitoAggiornamentoAppezzamenti As Boolean = True
                    Dim appezzamenti_list = appezzamento_changes.records.Where(Function(y) y.campagna = cuaa.payload.Campagna).ToList().Select(Of String)(Function(x) x.id_appezzamento).Distinct().ToList()
                    For Each id_appezzamento In appezzamenti_list
                        Dim pcg_change_appezzamento = appezzamento_changes.records.Where(Function(x) x.id_appezzamento = id_appezzamento).ToList()
                        For Each pcg_change In pcg_change_appezzamento
                            Try
                                Dim esito = _corews.ModificaAppezzamenti(cuaa,
                                                                     pcg_change,
                                                                     Nothing,
                                                                     CentroPK,
                                                                     ObjParametri_SuperServer,
                                                                     ObjParametri_Server,
                                                                     ObjParametri_Utenti,
                                                                     ScriviLog:=ScriviLog,
                                                                     VincoliList,
                                                                     TagName,
                                                                     ReplaceInvalidPolygon,
                                                                     TipoConfigurazione,
                                                                     SetAppezzaAddress,
                                                                     EnableVerboseLogPCG,
                                                                     CancellazioneLogica)

                                If Not esito Then
                                    esitoAggiornamentoAppezzamenti = False
                                End If
                            Catch ex As GiasException
                                esitoAggiornamentoAppezzamenti = False
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Errore in modifica appezzamento {3}: {4}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, id_appezzamento, ex.Message), LogType.Errore, ex)
                            Catch ex As DataPublishException
                                esitoAggiornamentoAppezzamenti = False
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Errore in modifica appezzamento {3}: {4}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, id_appezzamento, ex.Message), LogType.Errore, ex)
                            Catch ex As Exception
                                esitoAggiornamentoAppezzamenti = False
                                _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Errore in modifica appezzamento {3}: {4}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, id_appezzamento, ex.Message), LogType.Errore, ex)
                            End Try
                        Next
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Modificata piano colturare grafico sull'appezzamento {3} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, id_appezzamento))
                    Next

                    If esitoAggiornamentoAppezzamenti Then
                        cuaa.stato = 4
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                        _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        AGRODATAINIZIO,
                                                                        DateTime.Now,
                                                                        appezzamento_changes.records.Count,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        AGRODATAINIZIO,
                                                                        AGRODATAFINE,
                                                                        False
                                                                        )
                        ret = True
                    Else
                        cuaa.stato = -3
                        _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                        _logger.AppendLog(cuaa, "pcg", String.Format("[{0} - {1} - {2}] Errore in aggiornamento piano colturare grafico ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                        Throw New GiasException(String.Format("[{0} - {1} - {2}] Errore in aggiornamento piano colturare grafico ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    End If

                Else
                    cuaa.stato = -3
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New GiasException("Piano colturale grafico non conforme. Elaborazione notifica interrotta")
                End If
            End If
        End If
        Return ret
    End Function

    Private Function EseguiAggiornamentoRipartizioneCatastalePCG(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                                 ByVal pcg_catasto_change_notification As CUAAObj_Detail_Notification,
                                                                 ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                                 ByRef _corews As AgronicaCoreWSController,
                                                                 ByRef _webapiDataPublish As WebApiDataPublish,
                                                                 ByRef _helper As Helper,
                                                                 ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                                 ByRef EsitoSync As SyncAcknowledge,
                                                                 ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))) As Boolean
        Dim ret As Boolean = False

        Dim pcg_catasto_changes = _webapiDataPublish.ChiamaWSVariazioniPCGCatasto_CUAA(cuaa.payload.CUAA, cuaa.payload.Campagna, pcg_catasto_change_notification.DataOraNotifica)
        If pcg_catasto_changes Is Nothing OrElse pcg_catasto_changes.records.Count <= 0 Then
            cuaa.stato = -2
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Aggiornamento appezzamento-catasto timestamp_ref:  {0}. Ricevuta notifica di aggiornamento senza alcun dettaglio", pcg_catasto_change_notification.DataOraNotifica))
        Else
            If _helper.ControlliVariazioniRipartizioniCatastoAppezzamento(cuaa, pcg_catasto_changes, _logger, EsitoSync, CentroPK, ObjParametri_Server) = True Then
                'recupero l'elenco dei valori distinct degli id_appezzamento da aggiornare
                Dim esitoAggiornamentoCastato As Boolean = True
                Dim appezzamenti_list = pcg_catasto_changes.records.Select(Of String)(Function(x) x.id_appezzamento).Distinct().ToList()
                For Each id_appezzamento In appezzamenti_list
                    If id_appezzamento IsNot Nothing Then
                        Dim pcg_catasto_changes_xappezzamento = pcg_catasto_changes.records.Where(Function(x) x.id_appezzamento = id_appezzamento).ToList()
                        If pcg_catasto_changes_xappezzamento IsNot Nothing AndAlso pcg_catasto_changes_xappezzamento.Count > 0 Then
                            Dim esito = _corews.ModificaRipartizioneCatastaleAppezzamenti(id_appezzamento, pcg_catasto_changes_xappezzamento, CentroPK, ObjParametri_Server)
                            If Not esito Then
                                esitoAggiornamentoCastato = False
                            Else
                                _logger.AppendLog(cuaa, "pcg_terreni", String.Format("[{0} - {1} - {2}] Modificata ripartizione catastale sull'appezzamento {3} ", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, id_appezzamento))
                            End If
                        End If
                    End If
                Next
                If esitoAggiornamentoCastato Then
                    cuaa.stato = 3
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    _logger.AppendLog(cuaa, "pcg_terreni", String.Format("[{0} - {1} - {2}] Modifica ripartizione catastale eseguita correttamente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    ret = True
                Else
                    cuaa.stato = -2
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    Throw New GiasException(String.Format("[{0} - {1} - {2}] Modifica ripartizione catastale eseguita correttamente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                End If
            Else
                cuaa.stato = -2
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                ret = False
                Throw New DataPublishException("Modifica ripartizione catastale non conforme. Imposibile proseguire")
            End If
        End If
        Return ret
    End Function

    Private Function EseguiAggiornamentoParcoMacchine(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal equipaggiamenti_change_notification As CUAAObj_Detail_Notification,
                                                      ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                      ByRef _corews As AgronicaCoreWSController,
                                                      ByRef _webapiDataPublish As WebApiDataPublish,
                                                      ByRef _helper As Helper,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                      ByRef EsitoSync As SyncAcknowledge,
                                                      ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String))) As Boolean

        Dim ret As Boolean = False

        Dim equipaggiamenti_changes = _webapiDataPublish.ChiamaWSVariazioniEquipaggiamenti_CUAA(cuaa.payload.CUAA, equipaggiamenti_change_notification.DataOraNotifica)
        If equipaggiamenti_changes Is Nothing OrElse equipaggiamenti_changes.records.Count <= 0 Then
            cuaa.stato = -4
            _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Aggiornamento equipaggiamenti timestamp_ref: {0}. Ricevuta notifica di aggiornamento senza alcun dettaglio", equipaggiamenti_change_notification.DataOraNotifica))
        Else
            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
            _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           -1,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           DateTime.Now,
                                                           AGRODATAFINE,
                                                           AGRODATAINIZIO,
                                                           AGRODATAFINE,
                                                           False
                                                           )
            If _helper.ControlliVariazioniEquipaggiamento(cuaa, equipaggiamenti_changes, _logger, ObjParametri_Server, EsitoSync) = True Then
                'recupero l'elenco dei valori distinct degli id_appezzamento da aggiornare
                Dim esitoAggiornamentoParcoMacchine As Boolean = True
                For Each macchina_change In equipaggiamenti_changes.records
                    Dim esito = _corews.CreaModificaCancellaMacchina(cuaa, CentroPK, macchina_change, EsitoSync)
                    If Not esito Then
                        esitoAggiornamentoParcoMacchine = False

                        Select Case macchina_change.tipo_modifica
                            Case "I"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Fallito inserimento macchina {3} controllare tabella Agronica_Log_Invio_Chiamate", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                            Case "A"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Fallita modifica macchina {3} controllare tabella Agronica_Log_Invio_Chiamate", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                            Case "C"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Fallita cancellazione macchina {3} controllare tabella Agronica_Log_Invio_Chiamate", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                        End Select
                    Else
                        Select Case macchina_change.tipo_modifica
                            Case "I"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Inserita macchina {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                            Case "A"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Modificata macchina {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                            Case "C"
                                _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Cancellata macchina {3}", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal, macchina_change.codice))
                        End Select
                    End If
                Next

                If esitoAggiornamentoParcoMacchine Then
                    cuaa.stato = 5
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                    _helper.SetStatisticheElaborazioneStepNotifica(cuaa,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   -1,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   DateTime.Now,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   False
                                                                   )
                    _logger.AppendLog(cuaa, "equipaggiamenti", String.Format("[{0} - {1} - {2}] Modifica parco macchine eseguita correttamente", cuaa.payload.CUAA, cuaa.payload.Operazione, cuaa.payload.id_signal))
                    ret = True
                Else
                    cuaa.stato = -4
                    _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                    ret = False
                    Throw New GiasException("Modifiche parco macchine non eseguite. verificare tabella Agronica_Log_Invio_Chiamate per dettagli")
                End If
            Else
                cuaa.stato = -4
                _helper.SetStatoNotifica(cuaa, ObjParametri_Server)
                ret = False
                Throw New DataPublishException("Modifica parco macchine non conforme. Imposibile proseguire")
            End If
        End If
        Return ret
    End Function

    Private Function EseguiAggiornamentoLavoratori(ByVal CUAA As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByVal lavoratori_change_notification As CUAAObj_Detail_Notification,
                                                      ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                      ByRef _corews As AgronicaCoreWSController,
                                                      ByRef _webapiDataPublish As WebApiDataPublish,
                                                      ByRef _helper As Helper,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                      ByRef EsitoSync As SyncAcknowledge,
                                                      ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String)),
                                                   Optional ByVal ParametriContatti As ParametriInterscambioContatti = Nothing) As Boolean

        Dim ret As Boolean = False

        Dim lavoratori_changes = _webapiDataPublish.ChiamaWSVariazioniLavoratori_CUAA(CUAA.payload.CUAA, lavoratori_change_notification.DataOraNotifica)
        If lavoratori_changes Is Nothing OrElse lavoratori_changes.records.Count <= 0 Then
            CUAA.stato = -5
            _helper.SetStatoNotifica(CUAA, ObjParametri_Server)
            Throw New DataPublishException(String.Format("Aggiornamento contatti timestamp_ref: {0}. Ricevuta notifica di aggiornamento senza alcun dettaglio", lavoratori_change_notification.DataOraNotifica))
        Else
            'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
            _helper.SetStatisticheElaborazioneStepNotifica(CUAA,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            -1,
                                                            DateTime.Now,
                                                            AGRODATAFINE,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            AGRODATAINIZIO,
                                                            AGRODATAFINE,
                                                            False
                                                            )
            If _helper.ControlliVariazioniLavoratori(CUAA, lavoratori_changes, _logger, EsitoSync) Then
                'recupero l'elenco dei valori distinct degli id_appezzamento da aggiornare
                Dim esitoAggiornamentoLavoratori As Boolean = True
                For Each lavoratore_change In lavoratori_changes.records
                    Dim ErroreImport As String = ""
                    Dim esito = _corews.ScriviModificaCancella_Contatto(CUAA.payload.CUAA, lavoratore_change, ErroreImport, ParametriContatti)
                    If Not esito Then
                        EsitoSync.errors.Add(New SyncErrors() With {.code = "607", .msg = ErroreImport, .key = lavoratore_change.codice})
                        _logger.AppendLog(CUAA, "lavoratori", String.Format("[{0} - {1} - {2}] Errore in modifica contatto: {4} - {3}", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, ErroreImport, lavoratore_change.codice))
                        esitoAggiornamentoLavoratori = False
                    Else
                        _logger.AppendLog(CUAA, "lavoratori", String.Format("[{0} - {1} - {2}] Modificato contatto {3} ", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal, lavoratore_change.codice))
                    End If
                Next
                If esitoAggiornamentoLavoratori Then
                    CUAA.stato = 6
                    _helper.SetStatoNotifica(CUAA, ObjParametri_Server)
                    'lavez - 17/07/2024 - registro i tempi di inizio e fine elaborazione dei vari step per calcolo statistiche sui tempi
                    _helper.SetStatisticheElaborazioneStepNotifica(CUAA,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   -1,
                                                                   AGRODATAINIZIO,
                                                                   DateTime.Now,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   AGRODATAINIZIO,
                                                                   AGRODATAFINE,
                                                                   False
                                                                   )
                    _logger.AppendLog(CUAA, "lavoratori", String.Format("[{0} - {1} - {2}] Modifica contatti eseguita correttamente", CUAA.payload.CUAA, CUAA.payload.Operazione, CUAA.payload.id_signal))
                    ret = True
                Else
                    CUAA.stato = -5
                    _helper.SetStatoNotifica(CUAA, ObjParametri_Server)
                    ret = False
                    Throw New GiasException("Modifiche contatti non eseguite.")
                End If
            Else
                CUAA.stato = -5
                _helper.SetStatoNotifica(CUAA, ObjParametri_Server)
                ret = False
                Throw New DataPublishException("Modifica contatti non conforme. Imposibile proseguire")
            End If
        End If
        Return ret
    End Function

    Private Sub EseguiImportGruppiAppezzamenti(ByVal cuaa As ObjNotifica(Of AgronicaCoreDTOStd.InData.Notifiche.CUAAObj),
                                                      ByRef gruppo_appezzamento As GruppoAppezzamentoIntestazione,
                                                      ByVal CentroPK As AgronicaCoreModelsSTD.anagrafiche.CentroAziendale.PK,
                                                      ByRef handleIECampi As AgronicaCoreDemetraBIZ.IEGruppiAppezzamenti,
                                                      ByRef _corews As AgronicaCoreWSController,
                                                      ByRef _webapiDataPublish As WebApiDataPublish,
                                                      ByRef _helper As Helper,
                                                      ByRef ObjParametri_Server As AgronicaCoreParametri,
                                                      ByRef ObjParametri_Utenti As AgronicaCoreParametri,
                                                      ByRef EsitoSync As SyncAcknowledge,
                                                      ByRef listEsitiCUAA As ConcurrentDictionary(Of String, Tuple(Of String, String)))

        Dim campoToDB As New Campo(New Campo.PK(0, CentroPK))
        Dim codiceDemetra As String = gruppo_appezzamento.codice
        Dim codiceGias As String = gruppo_appezzamento.codice_esterno

        Dim pivaFiltro As String = CentroPK.partitaIva
        Dim saCodFiltro As Integer = CentroPK.codice
        Dim codiceFiltro As Integer = codiceDemetra
        Dim leggiDaCodGias As Boolean = False

        If Not String.IsNullOrWhiteSpace(codiceGias) Then
            Dim codGiasArr = codiceGias.Split("_")

            If codGiasArr.Length = 3 Then
                Dim pivaDaDemetra As String = codGiasArr(0)
                Dim saCodDaDemetra As Integer = codGiasArr(1)
                Dim campoCodDaDemetra As Integer = codGiasArr(2)

                If pivaDaDemetra = pivaFiltro AndAlso saCodDaDemetra = saCodFiltro Then
                    codiceFiltro = campoCodDaDemetra
                    leggiDaCodGias = True
                End If
            End If
        End If

        Dim checkCampo As Campo = Nothing

        If leggiDaCodGias Then
            checkCampo = handleIECampi.ReadCampoFromCampoCod(codiceFiltro,
                                                           pivaFiltro,
                                                           saCodFiltro)
        Else
            checkCampo = handleIECampi.ReadCampoFromCodiceEsterno(codiceFiltro,
                                                           pivaFiltro,
                                                           saCodFiltro)
        End If

        If checkCampo IsNot Nothing Then
            'Se il campo viene trovato devo sostituire la lista degli AppezzamentoCampo con quelli dalla notifica perché la funzione di scrittura
            'confronta la lista passata con quelli a database per associare/disassociare gli appezzamenti dal campo.
            campoToDB = checkCampo
        Else
            'Se il campo non viene trovato su Gias devo valorizzare i dati prendendo solo dall'oggetto di notifica
            campoToDB.validita = New AgronicaCoreModelsSTD.anagrafiche.IntervalloTemporale() With {
                .inizio = AGRODATAINIZIO,
                .fine = AGRODATAFINE
            }
            campoToDB.specie = New AgronicaCoreModelsSTD.metaschema.utilizzi.Specie(0)
        End If

        'Devo ciclare l'elenco degli appezzamenti con i loro id demetra e recuperare gli id gias se presenti, (valutare come gestire eventuali assenze)
        'da questi devo creare una lista di oggetti di classe AppezzamentoCampo da inserire nell'oggetto Campo
        campoToDB.appezzamentoCampo = New List(Of AppezzamentoCampo)()

        For Each appezzaDemetra As AppezzamentoInGruppo In gruppo_appezzamento.elemento_anagrafico.appezzamenti
            Dim impPK = AgronicaCoreAnagrafeBIZ.Impianto_R.VerificaEsistenzaImpiantoDaCodiceAnagrafe(CentroPK.partitaIva, CentroPK.codice, 0, enum_CodiceAnagrafe_Clienti.Demetra, appezzaDemetra.id_appezzamento, ObjParametri_Server)
            If impPK IsNot Nothing Then
                campoToDB.appezzamentoCampo.Add(New AppezzamentoCampo() With {
                        .piva = CentroPK.partitaIva,
                        .sa_cod = CentroPK.codice,
                        .appezza = impPK.appezza
                    })
            Else
                Throw New DataPublishException(String.Format("Impianto [{0}] da associare al campo [{1}-{2}-{3}] non trovato, elaborazione notifica interrotta", appezzaDemetra.id_appezzamento, CentroPK.partitaIva, CentroPK.codice, campoToDB.primaryKey.codice))
            End If
        Next

        Dim isCancellazione As Boolean = If(gruppo_appezzamento.tipo_modifica = "C", True, False)
        If gruppo_appezzamento.elemento_anagrafico.descrizione IsNot Nothing Then
            campoToDB.descrizione = gruppo_appezzamento.elemento_anagrafico.descrizione
        End If

        If isCancellazione AndAlso campoToDB.primaryKey.codice <> 0 Then
            handleIECampi.ImportCampo(campoToDB, enum_TipoOperazioneDB.Cancellazione, enum_SistemiEsterni.demetra, codiceDemetra)
        ElseIf campoToDB.primaryKey.codice = 0 AndAlso campoToDB.appezzamentoCampo.Count > 0 Then
            'Inserimento
            handleIECampi.ImportCampo(campoToDB, enum_TipoOperazioneDB.Scrittura, enum_SistemiEsterni.demetra, codiceDemetra)
        ElseIf campoToDB.primaryKey.codice <> 0 Then
            'Aggiornamento
            handleIECampi.ImportCampo(campoToDB, enum_TipoOperazioneDB.Modifica, enum_SistemiEsterni.demetra, codiceDemetra)
        End If

    End Sub

    Private Sub AddErroreGenericoSync(ByVal cuaa As String, ByRef EsitoSync As SyncAcknowledge, Optional ByVal msg As String = "")
        EsitoSync.errors.Add(New SyncErrors() With {.code = "-999", .msg = IIf(msg = "", "Errore lato Gias", msg), .key = cuaa})
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _logger.AppendLog(Nothing, "", String.Format("Fine importazione anagrafiche e piano colturale da Demetra. esecuzione dispose"), bypassElastiSearch:=True)
        _helper.Dispose()
        _logger.Dispose()
        _helper = Nothing
        _logger = Nothing
        _objParametriSuperServer = Nothing
        _objParametriServer = Nothing
        _objParametriUtenti = Nothing
        _parametriTask = Nothing
    End Sub

    Private Sub RecordCUAA_Log(ByVal id_elab As Integer,
                               ByVal cuaa As String,
                               ByVal guid As String,
                               ByVal operazione As String,
                               ByVal errors As Boolean,
                               ByVal data As SyncAcknowledge_Agronica,
                               ByVal errorList As List(Of SyncErrors),
                               ByRef ObjParametriServer As AgronicaCoreParametri,
                               piva As String,
                               Dettaglio1 As String,
                               Dettaglio2 As String,
                               Dettaglio3 As String)

        Dim key As String = id_elab.ToString() & "_" & cuaa

        Chiama_Scrivi_Log_Invio_NotificaCUAA(data,
                                             key,
                                             guid,
                                             If(operazione = "C", enum_TipoOperazioneDB.Scrittura, enum_TipoOperazioneDB.Modifica),
                                             If(errors, AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_KO, AgronicaCoreDemetraBIZ.Util_Costanti.ESITO_OK),
                                             If(errors, JsonConvert.SerializeObject(errorList), ""),
                                             DateTime.Now(),
                                             ObjParametriServer, piva, Dettaglio1, Dettaglio2, Dettaglio3)
    End Sub

    Private Shared Sub Chiama_Scrivi_Log_Invio_NotificaCUAA(pacchettoDaInviare As SyncAcknowledge_Agronica, Chiave As String, Chiave_Esterna As String, TipoOperazione As enum_TipoOperazioneDB, Esito As String, Dati_Ricevuti As String, Data_Invio As DateTime, objParametri_Server As AgronicaCoreParametri, piva As String, Dettaglio1 As String, Dettaglio2 As String, Dettaglio3 As String)

        Dim objlog_invioChiamate As New AgronicaCoreVarieBIZ.Agronica_Log_Invio_Chiamate_W
        Dim dati As String = ""
        dati = JsonConvert.SerializeObject(pacchettoDaInviare)

        objlog_invioChiamate.Scrivi_Log_Invio_Anagrafe(enum_Esportazioni_Sistema_Cod.Demetra_Import_DataPublish, dati, enum_TipoEntita_Des.DataPublish, Chiave, Chiave_Esterna, piva, 0, 0, 0, 0, 0, 0, "", TipoOperazione, Esito, Dati_Ricevuti, objParametri_Server, Data_Invio:=Data_Invio, Dettaglio1:=Dettaglio1, Dettaglio2:=Dettaglio2, Dettaglio3:=Dettaglio3)

    End Sub

    Private Function SummaryLogMail(ByVal TipoOperazione As Integer,
                                    ByVal elenco As ConcurrentDictionary(Of String, Tuple(Of String, String)),
                                    ByRef logger As LoggerManager) As String
        Dim StrBody As New StringBuilder
        StrBody.Length = 0

        Try
            StrBody.AppendLine("<table cellspacing=""0"" cellpadding=""0"" width=""100%"" border=""0"" style=""border-collapse: collapse;""> ")
            StrBody.AppendLine("    <tr>")
            StrBody.AppendLine("        <td style=""border: none;font-family: Tahoma;font-size: 12px;padding:0;border-width: 0 0 0 0;"">")
            StrBody.AppendLine("            <table cellspacing=""0"" cellpadding=""0"" width=""100%"" border=""0"" style=""border-collapse: collapse;"">")
            StrBody.AppendLine("                <tr>")
            StrBody.AppendLine("                    <td width=""100%"" style=""color: black;font-weight: bold;font-size: 16px;height: 70px;vertical-align: bottom;padding: 0 0 15px 15px;border: none;font-family: Tahoma;background-color: light-green;"">")
            Select Case TipoOperazione
                Case 1
                    StrBody.AppendLine("                        Riepilogo errori import nuove aziende\cuua da DEMETRA")
                Case 2
                    StrBody.AppendLine("                        Riepilogo errori import aggiornamenti aziende\catasto\pianocolturale da DEMETRA")
                Case Else
                    StrBody.AppendLine("                        !!!!!!Tipologia di flusso non mappata!!!!!")
            End Select

            StrBody.AppendLine("                    </td>")
            StrBody.AppendLine("                </tr>")
            StrBody.AppendLine("            </table>")
            StrBody.AppendLine("            <table cellspacing=""0"" cellpadding=""0"" width=""100%"" class=""data"" style=""border-collapse: collapse;"">")
            StrBody.AppendLine("                <tr>")
            StrBody.AppendLine("                    <td colspan=""3"" style=""font-family: Tahoma;height: 35px;background-color: #f3f4f4;font-size: 16px;vertical-align: middle;padding: 2px 3px 2px 3px;color: #626365;border: 1px solid #a7a9ac;border: 1px solid #a7a9ac;"" > Data elaborazione: " & DateTime.Now.ToString() & "</td>")
            StrBody.AppendLine("                </tr>")
            StrBody.AppendLine("                <tr>")
            StrBody.AppendLine("					<td style=""font-family: Tahoma;font-size: 12px;font-weight: bold;white-space: nowrap;width: 125px;padding: 2px 3px 2px 3px;border: 1px solid #a7a9ac;"">CUAA</td>")
            StrBody.AppendLine("					<td style=""font-family: Tahoma;font-size: 12px;font-weight: bold;white-space: nowrap;width: 125px;padding: 2px 3px 2px 3px;border: 1px solid #a7a9ac;"">Esito</td>")
            StrBody.AppendLine("					<td style=""font-family: Tahoma;font-size: 12px;font-weight: bold;white-space: nowrap;width: 125px;padding: 2px 3px 2px 3px;border: 1px solid #a7a9ac;"">Note</td>")
            StrBody.AppendLine("                </tr>")

            If elenco IsNot Nothing Then
                DettaglioElaborazione(elenco, StrBody)
            End If
            StrBody.AppendLine("            </table>")
            StrBody.AppendLine("        </td>")
            StrBody.AppendLine("    </tr>")
            StrBody.AppendLine("</table>")

        Catch ex As Exception
            StrBody.Length = 0
            logger.AppendLog(Nothing, "", ex.Message, LogType.Errore, ex, bypassElastiSearch:=True)
        End Try

        Return StrBody.ToString()
    End Function

    Private Sub DettaglioElaborazione(ByVal elenco As ConcurrentDictionary(Of String, Tuple(Of String, String)), ByRef StrBody As StringBuilder)
        For Each key In elenco.Keys
            Dim riga As Tuple(Of String, String) = Nothing
            If elenco.TryGetValue(key, riga) Then
                StrBody.AppendLine("				<tr>")
                StrBody.AppendLine("					<td>" & key & "</td>")
                StrBody.AppendLine("					<td " & IIf(riga.Item1.ToString() = "KO", "style=""background-color: #EE4B2B;text-align: center""", "style=""background-color: #90EE90;text-align: center""") & ">" & riga.Item1.ToString() & "</td>")
                StrBody.AppendLine("					<td>" & riga.Item2.ToString().Replace(vbCrLf, "<br>") & "</td>")
                StrBody.AppendLine("				</tr>")
            End If
        Next
    End Sub

    Private Sub RegistraEventoRiepilogo(ByVal key As String, ByVal Errore As Boolean, ByVal msg As String, ByRef elenco As ConcurrentDictionary(Of String, Tuple(Of String, String)))
        If Errore Then
            If elenco.ContainsKey(key) Then
                Dim chkValue As Tuple(Of String, String) = Nothing
                elenco.TryGetValue(key, chkValue)
                elenco.TryUpdate(key, New Tuple(Of String, String)(IIf(Errore, "KO", "OK"), elenco(key).Item2 & vbCrLf & msg), chkValue)
            Else
                elenco.TryAdd(key, New Tuple(Of String, String)(IIf(Errore, "KO", "OK"), msg))
            End If
        End If
    End Sub

End Class
