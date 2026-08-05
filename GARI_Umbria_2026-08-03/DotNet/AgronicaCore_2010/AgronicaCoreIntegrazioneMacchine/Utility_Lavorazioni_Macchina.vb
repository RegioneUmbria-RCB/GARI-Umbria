Imports System.Data.Entity
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreModello
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

''' <summary>
''' Classe per gestione lavorazioni macchina da tabelle di lavoro.
''' Le principali tabelle coinvolte sono:
''' 1. cbl_Calibrature: testata lavorazione macchina;
''' 2. cbl_CalibratureXCalibri: dettaglio lavorazione macchina;
''' 3. cbl_LogImportazioni: log importazione lavorazione macchina da file.
''' </summary>

Public Class Utility_Lavorazioni_Macchina : Implements IDisposable
    Private _objParametri As ObjParametri
    Private _queryHelper As QueryHelper
    Private _lavHelper As LavorazioniHelper
    Private ReadOnly _logger As Logger

    Public Sub New(objParametri As ObjParametri, logger As Logger)
        _objParametri = objParametri
        _queryHelper = New QueryHelper(_objParametri)
        _lavHelper = New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)
        If IsNothing(logger) Then
            Dim logName = (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).ToString
            _logger = New Logger(objParametri, logName)
        Else
            _logger = logger
        End If
        _logger.Info("GetLogger: " & _logger.Logger.Logger.Name)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Public Sub Dispose(eseguiPulizia As Boolean)
        If eseguiPulizia Then
            'Ulteriori azioni pulizia classe
        End If
    End Sub

    Public Function ApplicaFormatoLotto(stringaFormatoLotto As String,
                                        lavMacchina As cbl_Calibrature,
                                        ByRef messaggioErrore As String) As String

        Dim lotto = FormatoLotto.LeggiApplicaFormatoLotto(stringaFormatoLotto, lavMacchina, messaggioErrore)

        Return lotto

    End Function

    ''' <summary>
    ''' Scrive la tabella di lavoro relativamente ad una lavorazione macchina
    ''' </summary>
    ''' <param name="lavorazioneMacchina"></param>
    ''' <returns>ID tabella lavoro scritta</returns>

    Public Function Scrivi_Lavorazione_Macchina(ByRef lavorazioneMacchina As cbl_Calibrature) As Integer

        Const nomeRoutine = "Utility_Lavorazioni_Macchina.Scrivi_Lavorazione_Macchina()"

        _logger.Info("Scrittura lavorazione macchina")

        lavorazioneMacchina.ID = 0

        Try

            ControllaTransazione(nomeRoutine)

            ControlliPreliminariScrittura(lavorazioneMacchina)

            Dim objSeqTab As New Agro_Sequenze

            lavorazioneMacchina.ID = NuovoIdTabella("cbl_Calibrature", objSeqTab)

            'Dati impostati in automatico
            lavorazioneMacchina.Stato = statoImportazione.fileImportato

            Dim usernameCreaz = _objParametri.Server.UsernameOperazione
            Dim dataCreaz As DateTime = Date.Now()
            Dim dataInizio As DateTime = lavorazioneMacchina.Data_Inizio
            Dim dataFine As DateTime
            If IsNothing(lavorazioneMacchina.Data_Fine) Then
                dataFine = dataInizio
            Else
                dataFine = lavorazioneMacchina.Data_Fine
            End If

            lavorazioneMacchina.Username_Creazione = usernameCreaz
            lavorazioneMacchina.Data_Creazione = dataCreaz
            lavorazioneMacchina.Username_Modifica = usernameCreaz
            lavorazioneMacchina.Data_Modifica = dataCreaz
            lavorazioneMacchina.inviato = 0
            lavorazioneMacchina.Validita_Inizio = dataInizio.Date
            lavorazioneMacchina.Validita_Fine = AGRODATAFINE

            If Not IsNothing(dataInizio) AndAlso Not IsNothing(dataFine) Then
                lavorazioneMacchina.Durata = DateDiff(DateInterval.Second, dataInizio, dataFine)
            End If

            lavorazioneMacchina.NumeroTot = 0
            lavorazioneMacchina.PesoTot = 0

            For Each rigaLavorazioneMacchina In lavorazioneMacchina.cbl_CalibratureXCalibri
                'Impostazione collegamento a testata
                rigaLavorazioneMacchina.IDCalibro = lavorazioneMacchina.ID
                'Copia datii da testata
                rigaLavorazioneMacchina.Username_Creazione = lavorazioneMacchina.Username_Creazione
                rigaLavorazioneMacchina.Data_Creazione = lavorazioneMacchina.Data_Creazione
                rigaLavorazioneMacchina.Username_Modifica = lavorazioneMacchina.Username_Modifica
                rigaLavorazioneMacchina.Data_Modifica = lavorazioneMacchina.Data_Modifica
                rigaLavorazioneMacchina.inviato = 0
                rigaLavorazioneMacchina.Validita_Inizio = lavorazioneMacchina.Validita_Inizio
                rigaLavorazioneMacchina.Validita_Fine = lavorazioneMacchina.Validita_Fine
                'Popola ID dettaglio
                rigaLavorazioneMacchina.ID = NuovoIdTabella("cbl_CalibratureXCalibri", objSeqTab)
                'Polola Totali
                If IsNothing(rigaLavorazioneMacchina.Num) Then
                    rigaLavorazioneMacchina.Num = 0
                End If
                If IsNothing(rigaLavorazioneMacchina.Peso) Then
                    rigaLavorazioneMacchina.Peso = 0
                End If
                If IsNothing(rigaLavorazioneMacchina.Tara) Then
                    rigaLavorazioneMacchina.Tara = 0
                End If
                lavorazioneMacchina.NumeroTot += rigaLavorazioneMacchina.Num
                lavorazioneMacchina.PesoTot += rigaLavorazioneMacchina.Peso
            Next

            For Each logLavorazioneMacchina In lavorazioneMacchina.cbl_LogImportazioni
                'Impostazione collegamento a testata
                logLavorazioneMacchina.idCalibratura = lavorazioneMacchina.ID
                'Copia datii da testata
                logLavorazioneMacchina.data = lavorazioneMacchina.Data_Creazione
                logLavorazioneMacchina.Username_Creazione = lavorazioneMacchina.Username_Creazione
                logLavorazioneMacchina.Data_Creazione = lavorazioneMacchina.Data_Creazione
                logLavorazioneMacchina.Username_Modifica = lavorazioneMacchina.Username_Modifica
                logLavorazioneMacchina.Data_Modifica = lavorazioneMacchina.Data_Modifica
                logLavorazioneMacchina.inviato = 0
                logLavorazioneMacchina.Validita_Inizio = lavorazioneMacchina.Validita_Inizio
                logLavorazioneMacchina.Validita_Fine = lavorazioneMacchina.Validita_Fine
                'Popola ID log
                logLavorazioneMacchina.id = NuovoIdTabella("cbl_LogImportazioni", objSeqTab)
            Next

            Dim opzioniTransazione = CreaOpzioniTransazione()

            Dim gefutils As New Gias_EF_Utility
            Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri.Server.StringaConnessione)

            Using scopeScritturaLavorazione As New TransactionScope(TransactionScopeOption.Required, opzioniTransazione)

                Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

                    GiasContext.cbl_Calibrature.Attach(lavorazioneMacchina)
                    GiasContext.Entry(lavorazioneMacchina).State = EntityState.Added

                    For Each rigaLavorazioneMacchina In lavorazioneMacchina.cbl_CalibratureXCalibri
                        GiasContext.cbl_CalibratureXCalibri.Attach(rigaLavorazioneMacchina)
                        GiasContext.Entry(rigaLavorazioneMacchina).State = EntityState.Added
                    Next

                    For Each logLavorazioneMacchina In lavorazioneMacchina.cbl_LogImportazioni
                        GiasContext.cbl_LogImportazioni.Attach(logLavorazioneMacchina)
                        GiasContext.Entry(logLavorazioneMacchina).State = EntityState.Added
                    Next

                    GiasContext.SaveChanges() 'SCRITTURA SU DB

                End Using

                scopeScritturaLavorazione.Complete() 'COMMIT SCOPE

            End Using

        Catch ex As Exception

            Throw New Exception(String.Format("[ {0} ] : {1}", nomeRoutine, ex.Message))

        End Try

        Return lavorazioneMacchina.ID

    End Function

    Private Sub ControllaTransazione(nomeRoutine As String)

        If Not IsNothing(_objParametri.Server.objTransazione) Then
            Dim testo = "ERRORE! Transazione già aperta. L'oggetto corrente utilizza una transazione distribuita -> "
            Dim messaggio = String.Format(testo, nomeRoutine)
            Throw New Exception(messaggio)
        End If

        '### Non è possibile gestire la transazione del database [_objParametri.Server.objTransazione] insieme
        '### alla TransactionScope usata da EntityFramework.
        '### Il controllo sopra serve per evidenziare casi in cui la transazione viene aperta dal chiamante.
        '### La routine corrente DEVE invece poter gestire l'intera transazione tramite TransactionScope.
        '### E' comunque possibile aprire la TransactionScope dal chiamante.

    End Sub

    Public Function CreaOpzioniTransazione() As TransactionOptions

        Dim opzioniTransazione As New TransactionOptions()

        opzioniTransazione.IsolationLevel = IsolationLevel.ReadCommitted
        opzioniTransazione.Timeout = TransactionManager.MaximumTimeout

        Return opzioniTransazione

    End Function

    Private Sub ControlliPreliminariScrittura(lavorazioneMacchina As cbl_Calibrature)

        If IsNothing(lavorazioneMacchina) Then
            Throw New Exception("Dati lavorazione macchina nulli")
        End If

        If IsNothing(lavorazioneMacchina.cbl_LogImportazioni) Then
            Throw New Exception("Dati log importazione lavorazione macchina nulli")
        Else
            If String.IsNullOrEmpty(lavorazioneMacchina.cbl_LogImportazioni(0).nomeFile) Then
                Throw New Exception("Nome file importazione lavorazione macchina non definito")
            End If
        End If

        'Controllo se lo stesso file è già stato caricato

        Dim logImport = lavorazioneMacchina.cbl_LogImportazioni.FirstOrDefault()

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri.Server.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim calJoinLog = (From cal In GiasContext.cbl_Calibrature
                              Join log In GiasContext.cbl_LogImportazioni
                              On log.idCalibratura Equals cal.ID
                              Where (log.nomeFile = logImport.nomeFile)
                              Select cal, log).FirstOrDefault

            If Not IsNothing(calJoinLog) Then
                Dim formato = "File già importato: {0} - Stato: {1} ({2})"
                Dim statoImport = calJoinLog.cal.Stato
                Dim decodStatoImport = [Enum].GetName(GetType(statoImportazione), statoImport)
                Dim messaggio = String.Format(formato, logImport.nomeFile, statoImport, decodStatoImport)
                Throw New Exception(messaggio)
            End If

        End Using

    End Sub

    Private Function NuovoIdTabella(nomeTabella As String, objAgroSeq As Agro_Sequenze) As Integer

        Dim idTabella As Integer

        Try

            'Restituisce nuovo ID tabella con apertura connessione e transazione indipendente
            idTabella = objAgroSeq.NuovoId_Tabella(nomeTabella, 0, 2000000000, _objParametri.Server)

        Catch ex As Exception

            Throw New Exception(ex.Message & " - Tabella: " & nomeTabella)

        End Try

        Return idTabella

    End Function

    Private Function LeggiParamAggDaConfigServizio(objLavMacch As cbl_Calibrature) As String

        Dim paramAggImportatore As String

        'Lettura configurazione servizio
        Dim configurazioneServizio = LeggiConfigurazioneServizio(_objParametri.SuperServer.PivaSuperUser,
                                                                 enum_Id_Servizio.GiasOnline,
                                                                 enum_Tipi_Servizi_Background.Integrazione_Macchine_Lavorazione,
                                                                 _objParametri.SuperServer)

        Dim elencoImportatori As Importatori = Nothing
        Dim importatore As ConfigurazioneImportatore = Nothing

        If IsNothing(configurazioneServizio) Then
            Throw New Exception("Configurazione servizio non trovata")
        Else
            'Lettura elenco importatori da servizio
            elencoImportatori = JsonConvert.DeserializeObject(Of Importatori)(configurazioneServizio.Parametri_Extra)
        End If

        If IsNothing(elencoImportatori) Then
            Throw New Exception("Elenco importatori non trovato in parametri extra")
        Else
            'Lettura IdServizio da Macchina
            Dim piva = objLavMacch.Piva
            Dim codMacchinaLav = objLavMacch.Cod_Macchina_Lav
            Dim idServizio = _queryHelper.LeggiIdServizioDaMacchinaLav(piva, codMacchinaLav)

            'Lettura imporatore corrente
            importatore = elencoImportatori.Configurazioni.FirstOrDefault(Function(i) i.IdServizio.ToLower.Equals(idServizio.ToLower))
        End If

        If IsNothing(importatore) Then
            Throw New Exception("Importatore non trovato in parametri extra")
        Else
            'Salvataggio parametri aggiuntivi
            paramAggImportatore = importatore.ParametriAgg
        End If

        Return paramAggImportatore

    End Function

    Private Function CaricaParametriAggiuntivi(paramAggImportatore As String
                                               ) As Utility_Lavorazioni_Macchina_Param_Agg

        Dim objParamAgg As New Utility_Lavorazioni_Macchina_Param_Agg

        If Not IsNothing(paramAggImportatore) Then
            objParamAgg = JsonConvert.DeserializeObject(Of Utility_Lavorazioni_Macchina_Param_Agg)(paramAggImportatore)
        End If

        Return objParamAgg

    End Function

    Private Function LeggiConfigurazioneServizio(ByVal pivaSuperuser As String,
                                                 ByVal idServizio As enum_Id_Servizio,
                                                 ByVal tipoSincro As enum_Tipi_Servizi_Background,
                                                 ByVal objParametriSuperServer As AgronicaCoreParametri
                                                 ) As Configurazione_Servizio

        Dim configServizio As Configurazione_Servizio = Nothing

        Dim Configurazione_Servizi_R = New AgronicaCoreVarieDAL.Configurazione_Servizi_R

        configServizio = Configurazione_Servizi_R.LeggiSingolo(pivaSuperuser, idServizio, tipoSincro,
                                                               0, "",
                                                               objParametriSuperServer)

        Return configServizio

    End Function

    ''' <summary>
    ''' Importa uscite lavorazioni da elenco lavorazioni macchina leggendole dalla tabella lavoro.
    ''' Funzione predisposta per utilizzo da interfaccia in caso di importazione differita.
    ''' </summary>
    ''' <param name="elencoLavorazioniMacchina">Elenco lavorazioni macchina da importare</param>
    ''' <returns>Numero lavorazioni inserite</returns>

    Public Function Importa_Elenco_Lavorazioni_Macchina(elencoLavorazioniMacchina As List(Of Integer),
                                                        pathLog4netConfig As String
                                                        ) As esitoElencoImportazioniUsciteLavorazioni

        Dim esitoElencoImportazioni As New esitoElencoImportazioniUsciteLavorazioni

        If IsNothing(elencoLavorazioniMacchina) Then
            Throw New Exception("Elenco lavorazioni da importare non definito")
        End If

        For Each idLavMacch In elencoLavorazioniMacchina

            Dim objLavorazioneMacchina As cbl_Calibrature = Nothing

            Try

                'Leggi dati da tabella lavoro e popola relativo oggetto

                objLavorazioneMacchina = Leggi_Lavorazione_Macchina(idLavMacch)

                If IsNothing(objLavorazioneMacchina) Then

                    Throw New Exception("Dati lavorazione macchina non trovati")

                Else

                    'Importazione lavorazione macchina

                    Try

                        Dim objEsitoScriviUscSingola = Importa_Lavorazione_Macchina(objLavorazioneMacchina)

                        LogImportazioneOK(esitoElencoImportazioni, objEsitoScriviUscSingola)

                        SeInvioPrimoIngressoLavIntegrMacchine(objLavorazioneMacchina, objEsitoScriviUscSingola, pathLog4netConfig)

                    Catch ex As Exception

                        Throw New Exception(ex.Message)

                    End Try

                End If

            Catch ex As Exception

                LogImportazioneERRATA(esitoElencoImportazioni, ex.Message, idLavMacch, objLavorazioneMacchina)

            End Try

        Next

        Return esitoElencoImportazioni

    End Function

    Private Sub SeInvioPrimoIngressoLavIntegrMacchine(objLavorazioneMacchina As cbl_Calibrature,
                                                      objEsitoScriviUscSingola As esitoScritturaUsciteLavorazioni,
                                                      pathLog4netConfig As String)

        If objEsitoScriviUscSingola.primoMovScaricoCollegPrimoIngresso Then

            Using objUtilIntegrMacchine As New Utility_Integrazione_Macchine(_objParametri.SuperServer,
                                                                             _objParametri.Server,
                                                                             _objParametri.Utenti,
                                                                             pathLog4netConfig)

                Dim piva = objLavorazioneMacchina.Piva

                objUtilIntegrMacchine.SeInvioPrimoIngressoLavIntegrMacchine(objEsitoScriviUscSingola.primoIdMovDetScaricoColleg,
                                                                            Nothing,
                                                                            objEsitoScriviUscSingola.primoIdAgendaScaricoColleg,
                                                                            piva,
                                                                            False)

            End Using

        End If

    End Sub

    Private Sub LogImportazioneOK(ByRef esitoElencoImportazioni As esitoElencoImportazioniUsciteLavorazioni,
                                  objEsitoScriviUscSingola As esitoScritturaUsciteLavorazioni)

        esitoElencoImportazioni.numImportazioniOk += 1
        esitoElencoImportazioni.totNumRigheImportate += objEsitoScriviUscSingola.numRighe
        esitoElencoImportazioni.elencoEsitiScritture.Add(objEsitoScriviUscSingola)

    End Sub

    Private Sub LogImportazioneERRATA(ByRef esitoElencoImportazioni As esitoElencoImportazioniUsciteLavorazioni,
                                      messaggioErrore As String,
                                      idLavMacch As Integer,
                                      objLavorazioneMacchina As cbl_Calibrature)

        esitoElencoImportazioni.numImportazioniErrate += 1

        Dim identificativoLavorazione As String = String.Empty

        If Not IsNothing(objLavorazioneMacchina) Then
            identificativoLavorazione = objLavorazioneMacchina.Identif_Lavorazione
        End If

        Dim formato = "--- ERRORE IN IMPORTAZIONE LAVORAZIONE MACCHINA --- ID: {0} --- Identificativo: {1} ---"

        Dim messaggioIntestazione = String.Format(formato, idLavMacch, identificativoLavorazione)

        _logger.Error(messaggioIntestazione)
        _logger.Error(messaggioErrore)

        If String.IsNullOrEmpty(esitoElencoImportazioni.messaggioErrore) Then
            esitoElencoImportazioni.messaggioErrore += messaggioIntestazione
        Else
            esitoElencoImportazioni.messaggioErrore += vbCrLf & messaggioIntestazione
        End If
        esitoElencoImportazioni.messaggioErrore += vbCrLf & messaggioErrore

    End Sub

    ''' <summary>
    ''' Legge una lavorazione macchina da tabella lavoro
    ''' </summary>
    ''' <param name="IdLavorazioneMacchina"></param>
    ''' <returns>Oggetto lavorazione macchina</returns>

    Public Function Leggi_Lavorazione_Macchina(IdLavorazioneMacchina As Integer) As cbl_Calibrature

        Dim objLavMacch As cbl_Calibrature = Nothing

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri.Server.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            objLavMacch = (From calibrature In GiasContext.cbl_Calibrature.Include("cbl_CalibratureXCalibri")
                           Where (calibrature.ID = IdLavorazioneMacchina)
                           Select calibrature).FirstOrDefault()

        End Using

        Return objLavMacch

    End Function

    ''' <summary>
    ''' Importa uscite lavorazioni da una singola lavorazione macchina.
    ''' Casistiche integrate:
    ''' 1. Importatore Malavasi (Calibratrice+Rovesciatore);
    ''' 2. Importatore Unitec (Calibratrice);
    ''' 3. Importatore RS (Confezionatrice);
    ''' 4. Importatore Standard.
    ''' </summary>
    ''' <param name="lavorazioneMacchina"></param>
    ''' <param name="paramAggImportatore"></param>
    ''' <returns></returns>

    Public Function Importa_Lavorazione_Macchina(lavorazioneMacchina As cbl_Calibrature
                                                 ) As esitoScritturaUsciteLavorazioni

        Const nomeRoutine = "Utility_Lavorazioni_Macchina.Importa_Lavorazione_Macchina()"

        Dim objEsitoScriviUsc As New esitoScritturaUsciteLavorazioni
        Dim objAggTabLav As New datiAggiornamentoTabellaLavoro

        Try

            ControllaTransazione(nomeRoutine)

            Dim opzioniTransazione = CreaOpzioniTransazione()

            Using scopeImportaLavorazione As New TransactionScope(TransactionScopeOption.Required, opzioniTransazione)

                Dim idLavorazione = ControlliPreliminariImportazione(lavorazioneMacchina)

                Dim piva = lavorazioneMacchina.Piva

                Dim lavMacchParamAgg = CaricaParametriAggiuntivi(lavorazioneMacchina.Parametri_Agg)

                Dim lavDaImportare = New Lavorazione With
                {
                    .Piva = piva,
                    .Lotto = idLavorazione
                }

                If IsNothing(lavorazioneMacchina.Data_Inizio) Then
                    lavDaImportare.DataMovimento = Date.Today()
                Else
                    Dim dataInizio As Date = lavorazioneMacchina.Data_Inizio
                    lavDaImportare.DataMovimento = dataInizio.Date
                End If

                lavDaImportare.DescrizioneAggiuntiva = lavorazioneMacchina.Note

                Dim DtMovLav As DataTable = Nothing
                Dim DtMatPriCampLav As DataTable = Nothing
                Dim esisteLav As Boolean = False

                'Ricerca lavorazione aperta in base a lotto
                Dim idAgenda = RicercaLavorazione(piva, idLavorazione, DtMovLav, DtMatPriCampLav, esisteLav)

                Dim DtOrdLav As DataTable = Nothing
                Dim DtMatPriCampOrd As DataTable = Nothing
                Dim idAgendaOrd As Integer
                Dim idNuovaAgenda As Integer

                If esisteLav Then

                    ControllaCompatibilitaMacchina(lavorazioneMacchina, esisteLav, piva, idAgenda)

                    'Se previsto aggancio con uscita ordine lavorazione viene ricercato
                    idAgendaOrd = SeRicercaOrdineGeneratore(lavMacchParamAgg, piva, idAgenda)

                Else

                    'Ricerca ordine lavorazione aperto in base a lotto
                    idAgendaOrd = RicercaOrdineLavorazione(piva, idLavorazione, DtOrdLav, DtMatPriCampOrd)

                    ControllaCompatibilitaMacchina(lavorazioneMacchina, esisteLav, piva, idAgendaOrd)

                    'Carico materie prime campionature da ordine
                    lavDaImportare.MateriePrimeCampionature = PopolaMatPrimeCampTestata(DtMatPriCampOrd)

                    'Inserisce nuova lavorazione da ordine
                    idNuovaAgenda = InserisciNuovaLavorazioneDaOrdine(DtOrdLav, lavDaImportare)

                End If

                Dim DtLavDaImp As DataTable = Nothing
                Dim DtMatPriCampDaImp As DataTable = Nothing

                CompletaDatiLavDaImportare(lavDaImportare, esisteLav,
                                           DtLavDaImp, DtMatPriCampDaImp,
                                           idAgenda, DtMovLav, DtMatPriCampLav,
                                           idNuovaAgenda, DtOrdLav, DtMatPriCampOrd)

                objAggTabLav.idAgenda = lavDaImportare.IdAgenda

                objEsitoScriviUsc = ScriviUsciteMacchina(lavDaImportare, DtLavDaImp, DtMatPriCampDaImp,
                                                         lavorazioneMacchina, objAggTabLav, lavMacchParamAgg,
                                                         idAgendaOrd)

                If objEsitoScriviUsc.numRighe = 0 Then
                    Throw New Exception("Nessuna riga importata per ID lavorazione: " & idLavorazione)
                End If

                If lavMacchParamAgg.ChiudiLavorazione Then
                    _logger.Info("Chiusura lavorazione")
                    _lavHelper.ChiudiLavorazione(lavDaImportare.Piva, lavDaImportare.IdAgenda)
                End If

                AggiornaDatiTabellaLavoroOK(lavorazioneMacchina.ID, objAggTabLav)

                scopeImportaLavorazione.Complete()

            End Using

        Catch ex As Exception

            AggiornaDatiTabellaLavoroERRORE(lavorazioneMacchina.ID, objAggTabLav, ex.Message)

            Throw New Exception(String.Format("[ {0} ] : {1}", nomeRoutine, ex.Message))

        End Try

        Return objEsitoScriviUsc

    End Function

    Private Function ControlliPreliminariImportazione(lavorazioneMacchina As cbl_Calibrature) As String

        Dim idLavorazione As String

        If IsNothing(lavorazioneMacchina) Then
            Throw New Exception("Dati lavorazione macchina nulli")
        End If

        idLavorazione = lavorazioneMacchina.Identif_Lavorazione

        If String.IsNullOrEmpty(idLavorazione) Then
            Throw New Exception("Identificativo lavorazione macchina non impostato")
        End If

        ControllaSeGiaImportato(lavorazioneMacchina)

        Return idLavorazione

    End Function

    Private Sub ControllaSeGiaImportato(lavorazioneMacchina As cbl_Calibrature)

        If lavorazioneMacchina.Stato = statoImportazione.Importato_In_GIAS Then
            Throw New Exception("Lavorazione macchina già importata in GIAS")
        End If

    End Sub

    Private Sub ControllaCompatibilitaMacchina(lavorazioneMacchina As cbl_Calibrature,
                                               seLavorazione As Boolean,
                                               piva As String,
                                               idAgenda As Integer)

        Dim codMacchImport = lavorazioneMacchina.Cod_Macchina_Lav
        Dim codMacchAgenda = _queryHelper.LeggiMacchinaLav(piva, idAgenda)

        Dim tipoAgenda As String
        If seLavorazione Then
            tipoAgenda = "Lavorazione"
        Else
            tipoAgenda = "Ordine lavorazione"
        End If

        If codMacchImport <> codMacchAgenda Then
            Dim formato = "Codice macchina da importare ({0}) diverso da codice macchina {1} ({2})"
            Throw New Exception(String.Format(formato, codMacchImport, tipoAgenda, codMacchAgenda))
        End If

    End Sub

    Private Function RicercaLavorazione(piva As String,
                                        idLavorazione As String,
                                        ByRef DtMovLav As DataTable,
                                        ByRef DtMatPriCampLav As DataTable,
                                        ByRef esisteLav As Boolean
                                        ) As Integer

        Dim idAgenda As Integer
        Dim desAgenda As String

        DtMovLav = _queryHelper.leggiLavorazioneLotto(piva, idLavorazione)

        If DtMovLav.Rows.Count = 1 Then

            esisteLav = True
            idAgenda = DtMovLav.Rows(0).Item("Id_Agenda")
            desAgenda = DtMovLav.Rows(0).Item("des_lib")
            _logger.InfoFormat("Lettura lavorazione: {0} (ID={1})", desAgenda, idAgenda)
            'Lettura materie prime campionature e relativo caricamento
            DtMatPriCampLav = _queryHelper.LeggiMateriePrimeCampionature(DtMovLav.Rows(0).Item("cal_cod"))

        Else

            If DtMovLav.Rows.Count > 1 Then
                Throw New Exception("Più lavorazioni aperte relative a ID lavorazione: " & idLavorazione)
            End If

        End If

        Return idAgenda

    End Function

    Private Function SeRicercaOrdineGeneratore(lavMacchParamAgg As Utility_Lavorazioni_Macchina_Param_Agg,
                                               piva As String,
                                               idAgenda As Integer) As Integer

        Dim idAgendaOrd As Integer

        If lavMacchParamAgg.AggancioUscitaOrdineLav = Utility_Lavorazioni_Macchina_Costanti.AggancioUscitaOrdineLavPrimaRiga Then

            Dim objLavBiz As New AgronicaCoreContabBIZ.FF_LavorazioneBIZ
            idAgendaOrd = objLavBiz.Ricerca_Ord_Generatore(piva, idAgenda, _objParametri.Server)

        End If

        Return idAgendaOrd

    End Function

    Private Function RicercaOrdineLavorazione(piva As String,
                                              idLavorazione As String,
                                              ByRef DtOrdLav As DataTable,
                                              ByRef DtMatPriCampOrd As DataTable
                                              ) As Integer

        Dim msgErrore As String = String.Empty

        Dim idAgenda As Integer
        Dim desAgenda As String

        DtOrdLav = _queryHelper.leggiOrdineLavoroLotto(piva, idLavorazione)

        If DtOrdLav.Rows.Count = 1 Then

            idAgenda = DtOrdLav.Rows(0).Item("Id_Agenda")
            desAgenda = DtOrdLav.Rows(0).Item("des_lib")
            _logger.InfoFormat("Lettura ordine lavoro: {0} (ID={1})", desAgenda, idAgenda)

        Else

            If DtOrdLav.Rows.Count = 0 Then
                msgErrore = "Nessun ordine lavoro aperto relativo a ID lavorazione: " & idLavorazione
            Else
                msgErrore = "Più ordini lavoro aperti relativi a ID lavorazione: " & idLavorazione
            End If

            Throw New Exception(msgErrore)

        End If

        'Lettura materie prime campionature e relativo caricamento
        DtMatPriCampOrd = _queryHelper.LeggiMateriePrimeCampionature(DtOrdLav.Rows(0).Item("cal_cod"))

        Return idAgenda

    End Function

    Private Function PopolaMatPrimeCampTestata(DtMatPriCamp As DataTable)

        Dim ElencoMateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)

        If DtMatPriCamp.Rows.Count > 0 Then

            For Each riga As DataRow In DtMatPriCamp.Rows

                Select Case riga.Item("tipo")

                    Case ParametriQualitativi_Fornitore
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        mpc.Tipo_Cod = riga.Item("tipo_cod")
                        ElencoMateriePrimeCampionature.Add(mpc)

                    Case ParametriQualitativi_Imballaggio,
                         ParametriQualitativi_Contenitore,
                         ParametriQualitativi_Confezione
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        mpc.Tipo_Cod = riga.Item("tipo_cod")
                        mpc.ChkTara_Campionatura = 1
                        mpc.Tara_Campionatura = riga.Item("tara_campionatura")
                        ElencoMateriePrimeCampionature.Add(mpc)

                End Select

            Next riga

        End If

        Return ElencoMateriePrimeCampionature

    End Function

    Private Function InserisciNuovaLavorazioneDaOrdine(DtOrdLav As DataTable, lav As Lavorazione) As Integer

        'Creo oggetto lavorazione dai dati ordine lavorazione rintracciato tramite il lotto di collegamento

        Dim matDes As String = _queryHelper.LeggiMatDes(DtOrdLav.Rows(0).Item("Piva"),
                                                        DtOrdLav.Rows(0).Item("elem_Cod"),
                                                        DtOrdLav.Rows(0).Item("mat_Cod"))

        Dim preparazioneDes As String = _queryHelper.LeggiPreparazioneDes(DtOrdLav.Rows(0).Item("Piva"),
                                                                          DtOrdLav.Rows(0).Item("preparazione_cod"))

        Dim codMacchinaLav = _queryHelper.LeggiMacchinaLav(DtOrdLav.Rows(0).Item("Piva"),
                                                           DtOrdLav.Rows(0).Item("id_Agenda"))

        Dim lavNuovaLav As New Lavorazione With {.Piva = DtOrdLav.Rows(0).Item("Piva"),
                                                 .SaCod = DtOrdLav.Rows(0).Item("sa_Cod"),
                                                 .ElemCod = DtOrdLav.Rows(0).Item("elem_Cod"),
                                                 .MatCod = DtOrdLav.Rows(0).Item("mat_Cod"),
                                                 .IdDestinazione = DtOrdLav.Rows(0).Item("extra_int"),
                                                 .TipoDestinazione = Convert.ToInt32(DtOrdLav.Rows(0).Item("extra_str")),
                                                 .PreparazioneCod = DtOrdLav.Rows(0).Item("preparazione_cod"),
                                                 .LineaCod = DtOrdLav.Rows(0).Item("linea_cod"),
                                                 .TipoLavorazioneDes = preparazioneDes,
                                                 .MatDes = matDes,
                                                 .UdmCod = enum_UnitaMisura.KG,
                                                 .UdmCodExtra = enum_UnitaMisura.KG,
                                                 .IdTrasformazione = 0,
                                                 .NumImballaggi = 0,
                                                 .NumContenitori = 0,
                                                 .JollyInt = MagazzinoNONMovimentato,
                                                 .Qta = 0,
                                                 .QtaExtra = 1,
                                                 .QtaExtraTotale = 0,
                                                 .DataMovimento = lav.DataMovimento,
                                                 .Lotto = lav.Lotto,
                                                 .MateriePrimeCampionature = lav.MateriePrimeCampionature,
                                                 .CodMacchinaLav = codMacchinaLav,
                                                 .IdOrdCollegato = DtOrdLav.Rows(0).Item("id_agenda")
                                                 }

        Dim msgErroriLav As String = String.Empty

        Dim idNuovaAgenda = _lavHelper.ScriviTestataLavorazione(lavNuovaLav, True, msgErroriLav, LAVCOD_TRASFORMAZIONI)

        _logger.Info("Creata lavorazione: " & idNuovaAgenda.ToString())

        Return idNuovaAgenda

    End Function

    Private Sub CompletaDatiLavDaImportare(lavDaImportare As Lavorazione,
                                           esisteLav As Boolean,
                                           ByRef DtLavDaImp As DataTable,
                                           ByRef DtMatPriCampDaImp As DataTable,
                                           idAgenda As Integer,
                                           ByRef DtMovLav As DataTable,
                                           ByRef DtMatPriCampLav As DataTable,
                                           idNuovaAgenda As Integer,
                                           ByRef DtOrdLav As DataTable,
                                           ByRef DtMatPriCampOrd As DataTable)

        If esisteLav = True Then

            lavDaImportare.IdAgenda = idAgenda
            lavDaImportare.SaCod = DtMovLav.Rows(0).Item("sa_Cod")
            lavDaImportare.ElemCod = DtMovLav.Rows(0).Item("elem_Cod")
            lavDaImportare.MatCod = DtMovLav.Rows(0).Item("mat_Cod")
            lavDaImportare.IdDestinazione = DtMovLav.Rows(0).Item("extra_int")
            lavDaImportare.TipoDestinazione = Convert.ToInt32(DtMovLav.Rows(0).Item("extra_str"))
            DtLavDaImp = DtMovLav
            DtMatPriCampDaImp = DtMatPriCampLav

        Else

            lavDaImportare.IdAgenda = idNuovaAgenda
            lavDaImportare.SaCod = DtOrdLav.Rows(0).Item("sa_Cod")
            lavDaImportare.ElemCod = DtOrdLav.Rows(0).Item("elem_Cod")
            lavDaImportare.MatCod = DtOrdLav.Rows(0).Item("mat_Cod")
            lavDaImportare.IdDestinazione = DtOrdLav.Rows(0).Item("extra_int")
            lavDaImportare.TipoDestinazione = Convert.ToInt32(DtOrdLav.Rows(0).Item("extra_str"))
            DtLavDaImp = DtOrdLav
            DtMatPriCampDaImp = DtMatPriCampOrd

        End If

    End Sub

    Private Function ScriviUsciteMacchina(lavUscita As Lavorazione,
                                          DtLav As DataTable,
                                          DtMatPriCamp As DataTable,
                                          lavMacchina As cbl_Calibrature,
                                          ByRef objAggTabLav As datiAggiornamentoTabellaLavoro,
                                          lavMacchParamAgg As Utility_Lavorazioni_Macchina_Param_Agg,
                                          idAgendaOrd As Integer
                                          ) As esitoScritturaUsciteLavorazioni

        Dim objEsitoScriviUsc As New esitoScritturaUsciteLavorazioni

        Dim vegCod As Integer
        Dim culCod As Integer

        _queryHelper.LeggiMateriPrime(lavUscita.Piva, lavUscita.MatCod, vegCod, culCod)

        objAggTabLav.vegCodGIAS = vegCod
        objAggTabLav.culCodGIAS = culCod

        Dim objRifUscitaOrdLav As New RiferimentiUscitaOrdineLavorazione

        If lavMacchParamAgg.AggancioUscitaOrdineLav = Utility_Lavorazioni_Macchina_Costanti.AggancioUscitaOrdineLavPrimaRiga Then
            CaricaAggancioUscitaOrdineLavorazione(lavUscita, idAgendaOrd, objRifUscitaOrdLav)
        End If

        For Each rigaLavMacchina In lavMacchina.cbl_CalibratureXCalibri

            Dim lav As Lavorazione = Nothing

            ControllaDatiRiga(rigaLavMacchina)

            If lavMacchParamAgg.SeTipoTracciatoRovesciatore() Then
                lav = CaricaDatiRovesciatore(lavUscita, DtMatPriCamp, vegCod, culCod, rigaLavMacchina.Num)
            Else
                lav = CaricaDatiLavorazioneMacchina(lavUscita, rigaLavMacchina)
            End If

            If String.IsNullOrEmpty(rigaLavMacchina.Nome) And String.IsNullOrEmpty(rigaLavMacchina.Qualita) Then
                _logger.WarnFormat("Calibro e Qualità non indicati ( ID riga: {0} )", rigaLavMacchina.ID)
            End If

            Dim tipoCodCalibro = CaricaCalibroUscita(rigaLavMacchina, lavMacchParamAgg, lav, vegCod, culCod)

            Dim tipoCodQualita = CaricaQualitaUscita(rigaLavMacchina, lavMacchParamAgg, lav, vegCod, culCod)

            CaricaMateriePrimeCampionatureUscita(lav,
                                                 tipoCodCalibro,
                                                 tipoCodQualita,
                                                 lavMacchParamAgg,
                                                 rigaLavMacchina,
                                                 DtMatPriCamp)

            If String.IsNullOrEmpty(lavMacchina.Lotto) Then

                'Generazione lotto di uscita
                Dim errCodeLotto As Integer
                GeneraNuovoLottoUscita(DtLav, lav, tipoCodCalibro, tipoCodQualita, errCodeLotto)

            Else

                'Utilizzo lotto caricato in precedenza
                lav.Lotto = lavMacchina.Lotto

            End If

            'Se previsto, aggancio uscita lavorazione a uscita ordine lavorazione
            If lavMacchParamAgg.AggancioUscitaOrdineLav = Utility_Lavorazioni_Macchina_Costanti.AggancioUscitaOrdineLavPrimaRiga Then
                lav.pivaRif = objRifUscitaOrdLav.pivaRif
                lav.saCodRif = objRifUscitaOrdLav.saCodRif
                lav.idAgendaRif = objRifUscitaOrdLav.idAgendaRif
                lav.idMovRif = objRifUscitaOrdLav.idMovRif
                lav.idMovDetRif = objRifUscitaOrdLav.idMovDetRif
                lav.lavCodRif = objRifUscitaOrdLav.lavCodRif
            End If

            ScriviUscitaLavorazione(rigaLavMacchina, lav, objEsitoScriviUsc, objAggTabLav)

        Next

        _logger.Info("Uscite lavorazioni inserite: " & objEsitoScriviUsc.numRighe.ToString())

        Return objEsitoScriviUsc

    End Function

    Private Sub CaricaAggancioUscitaOrdineLavorazione(lavUscita As Lavorazione,
                                                      idAgendaOrd As Integer,
                                                      ByRef objRifUscitaOrdLav As RiferimentiUscitaOrdineLavorazione)

        Dim objMovimentiDettagli = New AgronicaCoreContabDAL.Movimenti_Dettagli_R
        Dim filtroAggiuntivo = "Movimenti_Dettagli.Extra_Str = '' AND Movimenti_Dettagli.Extra_Int = 0"
        Dim orderBy = "Movimenti_Dettagli.Id_Mov_Det"
        Dim dtMovimentiDettagli = objMovimentiDettagli.Leggi(lavUscita.Piva, 0, idAgendaOrd,
                                                             0, 0, 0, 0,
                                                             0, CAU_CARICO, 0, 0,
                                                             0, 0, 0, 0,
                                                             enumSelezioneVariabile.Selezione_JoinCompleta,
                                                             filtroAggiuntivo,
                                                             orderBy,
                                                             _objParametri.Server)

        If dtMovimentiDettagli.Rows.Count > 0 Then
            objRifUscitaOrdLav.pivaRif = lavUscita.Piva
            objRifUscitaOrdLav.saCodRif = 0
            objRifUscitaOrdLav.idAgendaRif = idAgendaOrd
            objRifUscitaOrdLav.idMovRif = dtMovimentiDettagli.Rows(0).Item("Id_Mov")
            objRifUscitaOrdLav.idMovDetRif = dtMovimentiDettagli.Rows(0).Item("Id_Mov_Det")
            objRifUscitaOrdLav.lavCodRif = LAVCOD_TESTATE_ORDINE_LAVORAZIONE
        End If

    End Sub

    Private Sub ControllaDatiRiga(rigaLavMacchina As cbl_CalibratureXCalibri)

        Try

            Controlla_Unita_Misura(rigaLavMacchina.Udm_Cod)

        Catch ex As Exception

            Dim messaggio = "{0} - Riga: {1}"
            Throw New Exception(String.Format(messaggio, ex.Message, rigaLavMacchina.ID))

        End Try

    End Sub

    Public Sub Controlla_Unita_Misura(udmCod As Integer)

        Select Case udmCod

            Case enum_UnitaMisura.KG, enum_UnitaMisura.Numero
                'Unità di misura gestite

            Case Else
                Dim messaggio = "Unità di misura non gestita. Codice: {0} - Descrizione: {1}"
                Dim udmDesc = [Enum].GetName(GetType(enum_UnitaMisura), udmCod)
                If IsNothing(udmDesc) Then
                    udmDesc = String.Empty
                End If
                Throw New Exception(String.Format(messaggio, udmCod, udmDesc))

        End Select

    End Sub

    Private Function CaricaDatiRovesciatore(ByRef lavUscita As Lavorazione,
                                       DtMatPriCamp As DataTable,
                                       vegCod As Integer,
                                       culCod As Integer,
                                       numeroBins As Integer
                                       ) As Lavorazione

        Dim DrRigheImballo() As DataRow
        Dim matCodImballo As Integer
        Dim tabellaParCod As Integer
        Dim taraUnitaria As Integer

        DrRigheImballo = DtMatPriCamp.Select("tipo = '" & ParametriQualitativi_Imballaggio & "'")

        'Lettura dati imballo

        If DrRigheImballo.Length = 1 Then
            tabellaParCod = DrRigheImballo(0).Item("tipo_cod")
            taraUnitaria = DrRigheImballo(0).Item("tara_campionatura")
        Else
            If DrRigheImballo.Length = 0 Then
                Throw New Exception("Dati imballo non trovati")
            Else
                Throw New Exception("Dati imballo con più corrispondenze")
            End If
        End If

        matCodImballo = _queryHelper.LeggiMatCodImballo(lavUscita.Piva, vegCod, culCod, tabellaParCod)

        Dim pesoTeoricoImballo As Decimal
        pesoTeoricoImballo = _queryHelper.LeggiPesoTeoricoDaConfigImballi(lavUscita.Piva, matCodImballo, vegCod, culCod)
        If pesoTeoricoImballo = 0 Then
            Throw New Exception("Peso teorico bins non trovato")
        End If

        Dim lav = CreaOggettoLavorazione(lavUscita)

        'Carico peso moltiplicando il numero dei bins per il peso teorico

        Dim qtaLav = numeroBins * pesoTeoricoImballo

        lav.UdmCod = enum_UnitaMisura.KG
        lav.Qta = qtaLav
        lav.QtaExtraTotale = qtaLav
        lav.Tara = numeroBins * taraUnitaria
        lav.NumImballaggi = numeroBins

        Return lav

    End Function

    Private Function CaricaDatiLavorazioneMacchina(lavUscita As Lavorazione,
                                                   rigaLavMacch As cbl_CalibratureXCalibri) As Lavorazione

        Dim lav = CreaOggettoLavorazione(lavUscita)

        If rigaLavMacch.Udm_Cod = enum_UnitaMisura.Numero Then

            'Confezioni
            lav.UdmCod = enum_UnitaMisura.Numero
            lav.Qta = rigaLavMacch.Num

            'Peso totale
            lav.UdmCodExtra = enum_UnitaMisura.KG
            lav.QtaExtraTotale = rigaLavMacch.Peso

            'Peso unitario
            If lav.QtaExtraTotale <> 0 And lav.Qta <> 0 Then
                lav.QtaExtra = Math.Round(Convert.ToDecimal(lav.QtaExtraTotale / lav.Qta), 6)
            End If

        Else

            'Peso totale
            lav.UdmCod = enum_UnitaMisura.KG
            Dim qtaLav = rigaLavMacch.Peso
            lav.Qta = qtaLav
            lav.QtaExtraTotale = qtaLav

        End If

        'Tara totale
        lav.Tara = Ottieni_Tara(rigaLavMacch)

        'Controlli in caso di unità misura Numero
        If rigaLavMacch.Udm_Cod = enum_UnitaMisura.Numero Then

            If rigaLavMacch.Num = 0 Then
                _logger.WarnFormat("Numero confezioni non indicato (Riga: {0})", rigaLavMacch.ID)
            End If

            If lav.Tara = 0 Then
                _logger.WarnFormat("Tara complessiva confezioni non indicata (Riga: {0})", rigaLavMacch.ID)
            End If

        End If

        Return lav

    End Function

    Private Function CreaOggettoLavorazione(lavUscita As Lavorazione)

        ' Creo oggetto lavorazione dai dati della lavorazione rintracciata tramite il lotto di collegamento

        Dim lav As New Lavorazione With {.Piva = lavUscita.Piva,
                                         .SaCod = lavUscita.SaCod,
                                         .IdAgenda = lavUscita.IdAgenda,
                                         .ElemCod = lavUscita.ElemCod,
                                         .MatCod = lavUscita.MatCod,
                                         .UdmCod = enum_UnitaMisura.KG,
                                         .UdmCodExtra = 0,
                                         .IdDestinazione = lavUscita.IdDestinazione,
                                         .TipoDestinazione = lavUscita.TipoDestinazione,
                                         .DataMovimento = lavUscita.DataMovimento,
                                         .DataCreazioneMovDett = lavUscita.DataCreazioneMovDett,
                                         .DataModificaMovDett = lavUscita.DataModificaMovDett
                                         }

        ' Imposto il flag che regola l'aggiornamento del magazzino

        lav.JollyInt = MagazzinoMovimentato

        Return lav

    End Function

    Private Function CaricaCalibroUscita(rigaLavMacchina As cbl_CalibratureXCalibri,
                                         lavMacchParamAgg As Utility_Lavorazioni_Macchina_Param_Agg,
                                         lav As Lavorazione,
                                         vegCod As Integer,
                                         culCod As Integer
                                         ) As Integer

        Dim tipoCodCalibro As Integer

        If IsNothing(rigaLavMacchina.Calibro_par_cod_GIAS) OrElse rigaLavMacchina.Calibro_par_cod_GIAS = 0 Then

            If Not String.IsNullOrEmpty(rigaLavMacchina.Nome) Then

                If lavMacchParamAgg.SeColonnaClasseDescrizione() Then
                    tipoCodCalibro = _queryHelper.LeggiCalibroDaDescrizione(lav.Piva, vegCod, culCod, rigaLavMacchina.Nome)
                Else
                    tipoCodCalibro = _queryHelper.LeggiCalibroDaSigla(lav.Piva, vegCod, culCod, rigaLavMacchina.Nome)
                End If

            End If

        Else

            tipoCodCalibro = rigaLavMacchina.Calibro_par_cod_GIAS

        End If

        Return tipoCodCalibro

    End Function

    Private Function CaricaQualitaUscita(rigaLavMacchina As cbl_CalibratureXCalibri,
                                         lavMacchParamAgg As Utility_Lavorazioni_Macchina_Param_Agg,
                                         lav As Lavorazione,
                                         vegCod As Integer,
                                         culCod As Integer
                                         ) As Integer

        Dim tipoCodQualita As Integer

        If Not String.IsNullOrEmpty(rigaLavMacchina.Qualita) Then

            If lavMacchParamAgg.SeColonnaClasseDescrizione() Then
                tipoCodQualita = _queryHelper.LeggiQualitaDaDescrizione(lav.Piva, vegCod, culCod, rigaLavMacchina.Qualita)
            Else
                tipoCodQualita = _queryHelper.LeggiQualitaDaSigla(lav.Piva, vegCod, culCod, rigaLavMacchina.Qualita)
            End If

        End If

        Return tipoCodQualita

    End Function

    Private Sub CaricaMateriePrimeCampionatureUscita(lav As Lavorazione,
                                                     tipoCodCalibro As Integer,
                                                     tipoCodQualita As Integer,
                                                     lavMacchParamAgg As Utility_Lavorazioni_Macchina_Param_Agg,
                                                     rigaLavMacchina As cbl_CalibratureXCalibri,
                                                     DtMatPriCamp As DataTable
                                                     )

        Dim parametriMPC As New parametriMateriePrimeCampionature

        parametriMPC.tipoCodCalibro = tipoCodCalibro
        parametriMPC.tipoCodQualita = tipoCodQualita

        If lavMacchParamAgg.SeTipoTracciatoRovesciatore() Then

            parametriMPC.imballaggio.gestione = tipoGestDato.DaLavorazione
            parametriMPC.imballaggio.gestioneTara = tipoGestDato.DaLavorazione

        Else

            'Se unità misura Numero assumo che siano confezioni

            If lav.UdmCod = enum_UnitaMisura.Numero Then
                parametriMPC.confezione.gestione = tipoGestDato.DaLavorazione
                parametriMPC.confezione.gestioneTara = tipoGestDato.ValoreSpecifico
                parametriMPC.confezione.tara = CalcolaTaraUnitaria(rigaLavMacchina)
            End If

        End If

        lav.MateriePrimeCampionature = PopolaMatPrimeCampUscita(DtMatPriCamp, parametriMPC)

        'Controlli su imballaggi e confezioni

        If lavMacchParamAgg.SeTipoTracciatoRovesciatore() Then

            If parametriMPC.imballaggio.tipoCodImpostato = 0 Then
                _logger.WarnFormat("Codice imballaggio non impostato (Riga: {0})", rigaLavMacchina.ID)
            End If
            If parametriMPC.imballaggio.taraImpostata = 0 Then
                _logger.WarnFormat("Tara unitaria imballaggio non impostata (Riga: {0})", rigaLavMacchina.ID)
            End If

        Else

            If lav.UdmCod = enum_UnitaMisura.Numero Then
                If parametriMPC.confezione.tipoCodImpostato = 0 Then
                    _logger.WarnFormat("Codice confezione non impostato (Riga: {0})", rigaLavMacchina.ID)
                End If
                If parametriMPC.confezione.taraImpostata = 0 Then
                    _logger.WarnFormat("Tara unitaria confezione non impostata (Riga: {0})", rigaLavMacchina.ID)
                End If
            End If

        End If

    End Sub

    ''' <summary>
    ''' Casistiche integrate: 
    ''' 1. Malavasi;
    ''' 2. Unitec;
    ''' 3. RS;
    ''' 4. Standard.
    ''' </summary>
    ''' <param name="DtMatPriCamp"></param>
    ''' <param name="tipoCodCalibro"></param>
    ''' <param name="tipoCodQualita"></param>
    ''' <param name="impostaImballo"></param>
    ''' <returns></returns>

    Private Function PopolaMatPrimeCampUscita(ByVal DtMatPriCamp As DataTable,
                                              ByRef parametriMPC As parametriMateriePrimeCampionature
                                              ) As List(Of Materia_Prima_Campionatura)

        Dim elencoMateriePrimeCampionature = New List(Of Materia_Prima_Campionatura)

        If DtMatPriCamp.Rows.Count > 0 Then

            For Each riga As DataRow In DtMatPriCamp.Rows

                Select Case riga.Item("tipo")

                    Case ParametriQualitativi_Fornitore
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        mpc.Tipo_Cod = riga.Item("tipo_cod")
                        elencoMateriePrimeCampionature.Add(mpc)

                    Case ParametriQualitativi_Imballaggio
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        impostaDatiConfezionamento(mpc, parametriMPC.imballaggio, riga)
                        elencoMateriePrimeCampionature.Add(mpc)

                    Case ParametriQualitativi_Contenitore
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        impostaDatiConfezionamento(mpc, parametriMPC.contenitore, riga)
                        elencoMateriePrimeCampionature.Add(mpc)

                    Case ParametriQualitativi_Confezione
                        Dim mpc = CreaMatPriCampDaRiga(riga)
                        impostaDatiConfezionamento(mpc, parametriMPC.confezione, riga)
                        elencoMateriePrimeCampionature.Add(mpc)

                    Case ParametriQualitativi_Calibro
                        If parametriMPC.tipoCodCalibro <> 0 Then
                            Dim mpc = CreaMatPriCampDaRiga(riga)
                            mpc.Tipo_Cod = parametriMPC.tipoCodCalibro
                            elencoMateriePrimeCampionature.Add(mpc)
                        End If

                    Case ParametriQualitativi_Qualita
                        If parametriMPC.tipoCodQualita <> 0 Then
                            Dim mpc = CreaMatPriCampDaRiga(riga)
                            mpc.Tipo_Cod = parametriMPC.tipoCodQualita
                            elencoMateriePrimeCampionature.Add(mpc)
                        End If

                End Select

            Next riga

        End If

        Return elencoMateriePrimeCampionature

    End Function

    Private Function CreaMatPriCampDaRiga(riga As DataRow) As Materia_Prima_Campionatura

        Dim mpc As New Materia_Prima_Campionatura With {.Tipo = riga.Item("tipo")}

        Return mpc

    End Function

    Private Sub impostaDatiConfezionamento(ByRef mpc As Materia_Prima_Campionatura,
                                           ByRef param As parametriConfezionamento,
                                           riga As DataRow)

        Select Case param.gestione

            Case tipoGestDato.No
                mpc.Tipo_Cod = 0

            Case tipoGestDato.DaLavorazione
                mpc.Tipo_Cod = riga.Item("tipo_cod")

            Case tipoGestDato.ValoreSpecifico
                mpc.Tipo_Cod = param.tipoCod

        End Select

        param.tipoCodImpostato = mpc.Tipo_Cod

        mpc.ChkTara_Campionatura = 1

        Select Case param.gestioneTara

            Case tipoGestDato.No
                mpc.Tara_Campionatura = 0

            Case tipoGestDato.DaLavorazione
                mpc.Tara_Campionatura = riga.Item("tara_campionatura")

            Case tipoGestDato.ValoreSpecifico
                mpc.Tara_Campionatura = param.tara

        End Select

        param.taraImpostata = mpc.Tara_Campionatura

    End Sub

    Private Sub GeneraNuovoLottoUscita(ByRef DtLav As DataTable,
                                       ByRef lav As Lavorazione,
                                       ByVal calibro_cod As Integer,
                                       ByVal qualita_cod As Integer,
                                       ByRef errCodeLotto As Integer)

        Dim DtPrep = _queryHelper.LeggiPreparazione(DtLav.Rows(0).Item("Piva"),
                                                    DtLav.Rows(0).Item("preparazione_cod"))
        Select Case True
            Case DtPrep.Rows.Count = 0
                Throw New Exception("Preparazione non trovata")
            Case DtPrep.Rows.Count > 1
                Throw New Exception("Trovate più preparazioni")
        End Select

        Dim risultatoLotto As String
        Dim dataLav As Date = lav.DataMovimento
        Dim objLotto As New AgronicaCoreAnagrafeBIZ.Lotto_AssegnaxRisumSpeVarQualCert_BIZ
        risultatoLotto = objLotto.Impostazione_LottoLavorazioni(DtLav.Rows(0).Item("Piva"),
                                                                2,
                                                                DtLav.Rows(0).Item("mat_Cod"),
                                                                0,
                                                                dataLav,
                                                                qualita_cod,
                                                                calibro_cod,
                                                                String.Empty, String.Empty,
                                                                DtPrep.Rows(0).Item("Codice_Generazione"),
                                                                String.Empty,
                                                                _objParametri.Server,
                                                                errCodeLotto,
                                                                DtLav.Rows(0).Item("Lotto"))

        If errCodeLotto <> 0 Then
            Throw New Exception(risultatoLotto)
        Else
            lav.Lotto = risultatoLotto
            _logger.Info("Generato lotto: " & risultatoLotto)
        End If

    End Sub

    Private Sub ScriviUscitaLavorazione(ByVal rigaLavMacchina As cbl_CalibratureXCalibri,
                                        ByVal lav As Lavorazione,
                                        ByRef esitoScritturaUscLav As esitoScritturaUsciteLavorazioni,
                                        ByRef objAggTabLav As datiAggiornamentoTabellaLavoro)

        Dim messaggio As String = "Inserimento uscita lavorazione"
        If Not String.IsNullOrEmpty(rigaLavMacchina.Nome) Then
            messaggio += " - Calibro: " & rigaLavMacchina.Nome
        End If
        If Not String.IsNullOrEmpty(rigaLavMacchina.Qualita) Then
            messaggio += " - Qualità: " & rigaLavMacchina.Qualita
        End If
        _logger.Info(messaggio)

        Dim idAgendaScaricoColleg As Integer = 0
        Dim idMovDetScaricoColleg As Integer = 0
        Dim msgErroriLav = String.Empty

        Dim idMovDet = _lavHelper.ScriviDettaglioCarico(lav,
                                                        True,
                                                        msgErroriLav,
                                                        idAgendaScaricoColleg:=idAgendaScaricoColleg,
                                                        idMovDetScaricoColleg:=idMovDetScaricoColleg,
                                                        lavCod:=LAVCOD_TRASFORMAZIONI)

        If idAgendaScaricoColleg > 0 AndAlso idMovDetScaricoColleg > 0 Then
            If esitoScritturaUscLav.primoIdAgendaScaricoColleg = 0 AndAlso esitoScritturaUscLav.primoIdMovDetScaricoColleg = 0 Then
                esitoScritturaUscLav.primoIdAgendaScaricoColleg = idAgendaScaricoColleg
                esitoScritturaUscLav.primoIdMovDetScaricoColleg = idMovDetScaricoColleg
                Dim objMovDet As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                Dim dtMovDet = objMovDet.LeggiCaricoScarico_New(lav.Piva, lav.SaCod, idAgendaScaricoColleg,
                                                                0, 0, 0, 0, 0,
                                                                0, 0, 0, 0,
                                                                CAU_SCARICO,
                                                                0, 0, 0, 0, "",
                                                                0, "", "", "",
                                                                _objParametri.Server, leggiLavorazioni:=True)
                If dtMovDet.Rows.Count = 1 Then
                    esitoScritturaUscLav.primoMovScaricoCollegPrimoIngresso = True
                End If
            End If
            esitoScritturaUscLav.numScarColleg += 1
        End If

        esitoScritturaUscLav.numRighe += 1

        'Salvataggio riferimento idMovDet per aggiornamento finale tabella
        Dim detDatiAggTabLav As New dettaglioDatiAggTabLavoro
        detDatiAggTabLav.idRiga = rigaLavMacchina.ID
        detDatiAggTabLav.idMovDet = idMovDet
        objAggTabLav.listaDatiDettaglio.Add(detDatiAggTabLav)

    End Sub

    Private Sub AggiornaDatiTabellaLavoroOK(idLavMacch As Integer,
                                            ByRef datiAggTabLav As datiAggiornamentoTabellaLavoro)

        datiAggTabLav.Stato = statoImportazione.Importato_In_GIAS

        AggiornaDatiTabellaLavoro(idLavMacch, datiAggTabLav)

    End Sub

    Private Sub AggiornaDatiTabellaLavoroERRORE(idLavMacch As Integer,
                                                ByRef datiAggTabLav As datiAggiornamentoTabellaLavoro,
                                                messaggioErrore As String)

        datiAggTabLav.Stato = statoImportazione.Errori_Durante_Importazione_In_GIAS
        datiAggTabLav.Errore = messaggioErrore

        Dim opzioniTransazione = CreaOpzioniTransazione()

        Using scopeAggTabLavoroErrore As New TransactionScope(TransactionScopeOption.Suppress, opzioniTransazione)

            AggiornaDatiTabellaLavoro(idLavMacch, datiAggTabLav)

            scopeAggTabLavoroErrore.Complete()

        End Using

    End Sub

    Private Sub AggiornaDatiTabellaLavoro(idLavMacch As Integer,
                                          datiAggTabLav As datiAggiornamentoTabellaLavoro)

        Dim gefutils As New Gias_EF_Utility
        Dim EFConnString As String = gefutils.GetEntityConnectionString(_objParametri.Server.StringaConnessione)

        Using GiasContext As New Gias_DeveloperServer_Entities(EFConnString)

            Dim objLavMacch = (From calibrature In GiasContext.cbl_Calibrature.Include("cbl_CalibratureXCalibri")
                               Where (calibrature.ID = idLavMacch)
                               Select calibrature).FirstOrDefault()

            If IsNothing(objLavMacch) Then

                If datiAggTabLav.Stato = statoImportazione.Errori_Durante_Importazione_In_GIAS Then
                    'In caso di aggiornamento stato errato non vengono eseguiti controlli in quanto
                    'il record in tabella potrebbe non esistere a fronte di una abort transazione
                Else
                    Throw New Exception("Errori in fase di aggiornamento tabella (cbl_Calibrature)")
                End If

            Else

                'Se devo aggiornare lo stato uguale a importato...
                If datiAggTabLav.Stato = statoImportazione.Importato_In_GIAS Then
                    '...controllo che non lo sia già
                    ControllaSeGiaImportato(objLavMacch)
                End If

                Dim dataMod As Date = Date.Now()
                Dim userMod As String = _objParametri.Server.UsernameOperazione

                objLavMacch.Stato = datiAggTabLav.Stato
                objLavMacch.Errore = datiAggTabLav.Errore
                objLavMacch.Veg_cod_GIAS = datiAggTabLav.vegCodGIAS
                objLavMacch.Cul_cod_GIAS = datiAggTabLav.culCodGIAS
                objLavMacch.Data_Modifica = dataMod
                objLavMacch.Username_Modifica = userMod

                GiasContext.cbl_Calibrature.Attach(objLavMacch)
                GiasContext.Entry(objLavMacch).State = EntityState.Modified

                If datiAggTabLav.Stato = statoImportazione.Importato_In_GIAS Then

                    For Each objRigaLavMacch In objLavMacch.cbl_CalibratureXCalibri
                        Dim rigaDettaglio = datiAggTabLav.listaDatiDettaglio.FirstOrDefault(Function(l) l.idRiga.Equals(objRigaLavMacch.ID))
                        objRigaLavMacch.Id_Mov_Det = rigaDettaglio.idMovDet
                        objRigaLavMacch.Data_Modifica = dataMod
                        objRigaLavMacch.Username_Modifica = userMod
                        GiasContext.cbl_CalibratureXCalibri.Attach(objRigaLavMacch)
                        GiasContext.Entry(objRigaLavMacch).State = EntityState.Modified
                    Next

                End If

                GiasContext.SaveChanges()

            End If

        End Using

    End Sub

    Public Function Se_Esegui_Importazione_Immediata(importazioneImmediata As Boolean,
                                                     lavorazioneMacchina As cbl_Calibrature
                                                     ) As esitoScritturaUsciteLavorazioni

        Dim esitoImportLavMacch As New esitoScritturaUsciteLavorazioni

        If importazioneImmediata Then

            esitoImportLavMacch = Importa_Lavorazione_Macchina(lavorazioneMacchina)

            If esitoImportLavMacch.numRighe = 0 Then
                Dim idLavorazione = lavorazioneMacchina.Identif_Lavorazione
                Throw New Exception("Errori in scrittura lavorazioni. ID lavorazione: " & idLavorazione)
            End If

        End If

        Return esitoImportLavMacch

    End Function

    Public Sub Se_Invio_Primo_Ingresso(importazioneImmediata As Boolean,
                                       lavorazioneMacchina As cbl_Calibrature,
                                       esitoImportLavMacch As esitoScritturaUsciteLavorazioni,
                                       utilityIntegrMacchine As Utility_Integrazione_Macchine)

        If importazioneImmediata Then

            If esitoImportLavMacch.primoMovScaricoCollegPrimoIngresso Then
                Dim piva = lavorazioneMacchina.Piva
                utilityIntegrMacchine.SeInvioPrimoIngressoLavIntegrMacchine(esitoImportLavMacch.primoIdMovDetScaricoColleg,
                                                                            Nothing,
                                                                            esitoImportLavMacch.primoIdAgendaScaricoColleg,
                                                                            piva,
                                                                            False)
            End If

        End If

    End Sub

    Private Function CalcolaTaraUnitaria(rigaLavorazioneMacchina As cbl_CalibratureXCalibri) As Decimal

        Dim taraUnitaria As Decimal

        Dim tara = Ottieni_Tara(rigaLavorazioneMacchina)

        Dim numero As Integer
        If IsNumeric(rigaLavorazioneMacchina.Num) Then
            numero = rigaLavorazioneMacchina.Num
        End If

        If tara > 0 And numero > 0 Then
            taraUnitaria = Math.Round(Convert.ToDecimal(tara / numero), 6)
        End If

        Return taraUnitaria

    End Function

#Region "Funzioni Imposta/Ottieni"

    Public Function Ottieni_Tara(rigaLavorazioneMacchina As cbl_CalibratureXCalibri) As Decimal

        Dim tara As Decimal

        If IsNumeric(rigaLavorazioneMacchina.Tara) Then
            tara = rigaLavorazioneMacchina.Tara
        End If

        Return tara

    End Function

    Public Sub Imposta_Parametri_Agg(ByRef lavorazioneMacchina As cbl_Calibrature,
                                     objParamAggSpecifico As Object)

        Dim stringaParametriAgg As String = JsonConvert.SerializeObject(objParamAggSpecifico)

        Dim objParamAggCentralizzato = JsonConvert.DeserializeObject(Of Utility_Lavorazioni_Macchina_Param_Agg)(stringaParametriAgg)

        lavorazioneMacchina.Parametri_Agg = JsonConvert.SerializeObject(objParamAggCentralizzato)

    End Sub

    Public Function Ottieni_Macchina_Da_IdServizio(piva As String, idServizio As String) As String

        Dim codMacchinaLav As String

        codMacchinaLav = _queryHelper.LeggiMacchinaLavDaIdServizio(piva, idServizio)

        If String.IsNullOrEmpty(codMacchinaLav) Then
            Throw New Exception("Nessuna macchina trovata per il servizio " & idServizio)
        End If

        Return codMacchinaLav

    End Function

#End Region

    Private Class datiAggiornamentoTabellaLavoro

        Public vegCodGIAS As Integer = Nothing
        Public culCodGIAS As Integer = Nothing
        Public idAgenda As Integer = 0
        Public Errore As String = Nothing
        Public Stato As Integer = Nothing
        Public listaDatiDettaglio As New List(Of dettaglioDatiAggTabLavoro)

    End Class

    Private Class dettaglioDatiAggTabLavoro

        Public idRiga As Integer
        Public idMovDet As Integer

    End Class

    Private Class parametriMateriePrimeCampionature

        Public tipoCodCalibro As Integer
        Public tipoCodQualita As Integer

        Public imballaggio As New parametriConfezionamento
        Public contenitore As New parametriConfezionamento
        Public confezione As New parametriConfezionamento

    End Class

    Private Class parametriConfezionamento

        Public gestione As Integer = tipoGestDato.No
        Public tipoCod As Integer
        Public tipoCodImpostato As Integer = 0
        Public gestioneTara As Integer = tipoGestDato.No
        Public tara As Decimal
        Public taraImpostata As Decimal = 0

    End Class

    Private Enum tipoGestDato

        No = 0
        DaLavorazione = 1
        ValoreSpecifico = 2

    End Enum

    ''' <summary>
    ''' Classe privata per riferimenti uscita ordine lavorazione
    ''' </summary>

    Private Class RiferimentiUscitaOrdineLavorazione

        Public pivaRif As String

        Public saCodRif As Integer

        Public idAgendaRif As Integer

        Public idMovRif As Integer

        Public idMovDetRif As Integer

        Public lavCodRif As Integer

    End Class

    ''' <summary>
    ''' Classe privata per applicazione formato lotto
    ''' </summary>

    Private Class FormatoLotto

        'carattere inizio place holder
        Public Const carInizPH As String = "["

        'carattere fine place holder
        Public Const carFinePH As String = "]"

        'separatore parametri place holder
        Public Const sepParametriPH As String = ":"

        'parametri place holder specifici
        Public Const paramPH_GiornoDellAnno As String = "#GDA#" 'Per PH di tipo data

        'place holder gestiti

        '1. relativi a dati lavorazione macchina
        Public Const phLotto = "LOTTO"
        Public Const phDataInizio = "DATAINIZIO"
        Public Const phDataFine = "DATAFINE"
        Public Const phNote = "NOTE"

        '2. di altra natura
        Public Const phData = "DATA" 'Data/ora corrente

        Public Shared Function LeggiApplicaFormatoLotto(stringaFormatoLotto As String,
                                                        lavMacchina As cbl_Calibrature,
                                                        ByRef messaggioErrore As String
                                                        ) As String

            Dim lotto As String = stringaFormatoLotto

            messaggioErrore = String.Empty

            Dim posInizioRicerca As Integer = 1

            Dim fineStringa As Boolean = False
            Dim posInizPH As Integer
            Dim posFinePH As Integer

            Do

                posInizPH = InStr(posInizioRicerca, stringaFormatoLotto, carInizPH)

                If posInizPH > 0 Then

                    posInizioRicerca = posInizPH + 1

                    posFinePH = InStr(posInizioRicerca, stringaFormatoLotto, carFinePH)

                    If posFinePH > 0 Then

                        posInizioRicerca = posFinePH + 1

                        Dim placeHolder = estraiStringa(stringaFormatoLotto, posInizPH, posFinePH)

                        Dim parametriPH As String = String.Empty

                        Dim nomePlaceHolder = determinaNomePH(placeHolder, parametriPH)

                        Dim nuovoPlaceHolder = valorizzaPH(nomePlaceHolder, parametriPH, lavMacchina, messaggioErrore)

                        lotto = Replace(lotto, placeHolder, nuovoPlaceHolder, 1, 1)

                    Else

                        fineStringa = True

                    End If

                Else

                    fineStringa = True

                End If

            Loop Until fineStringa

            If Not String.IsNullOrEmpty(messaggioErrore) Then

                Dim messaggio As String

                messaggio = "Formato lotto iniziale: " & stringaFormatoLotto
                accodaErrore(messaggio, messaggioErrore)

                messaggio = "Lotto elaborato con errori: " & lotto
                accodaErrore(messaggio, messaggioErrore)

                lotto = ""

            End If

            Return lotto

        End Function

        Private Shared Function estraiStringa(stringa As String,
                                             posInizio As Integer,
                                             posFine As Integer
                                             ) As String

            Dim stringaFinale As String

            Dim posInizSubstring = posInizio - 1 'Substring parte da indice ZERO
            Dim lungSubstring = posFine - posInizio + 1

            stringaFinale = stringa.Substring(posInizSubstring, lungSubstring)

            Return stringaFinale

        End Function

        Private Shared Function valorizzaPH(nomePlaceHolder As String,
                                            parametriPH As String,
                                            lavMacchina As cbl_Calibrature,
                                            ByRef messaggioErrore As String) As String

            Dim placeHolderValorizzato As String = String.Empty

            Select Case nomePlaceHolder

                Case phLotto
                    Dim stringa As String = lavMacchina.Lotto
                    placeHolderValorizzato = valorizzaPH_Stringa(stringa, parametriPH, nomePlaceHolder, messaggioErrore)

                Case phNote
                    Dim stringa As String = lavMacchina.Note
                    placeHolderValorizzato = valorizzaPH_Stringa(stringa, parametriPH, nomePlaceHolder, messaggioErrore)

                Case phDataInizio
                    Dim data As Date = lavMacchina.Data_Inizio
                    placeHolderValorizzato = valorizzaPH_Data(data, parametriPH, nomePlaceHolder, messaggioErrore)

                Case phDataFine
                    Dim data As Date = lavMacchina.Data_Fine
                    placeHolderValorizzato = valorizzaPH_Data(data, parametriPH, nomePlaceHolder, messaggioErrore)

                Case phData
                    Dim data As Date = Now()
                    placeHolderValorizzato = valorizzaPH_Data(data, parametriPH, nomePlaceHolder, messaggioErrore)

                Case Else
                    Dim messaggio = "Placeholder non gestito: " & nomePlaceHolder
                    accodaErrore(messaggio, messaggioErrore)

            End Select

            Return placeHolderValorizzato

        End Function

        Private Shared Function determinaNomePH(placeHolder As String,
                                                ByRef parametriPH As String
                                                ) As String

            parametriPH = String.Empty

            Dim posSep As Integer
            Dim posIniz As Integer = 2
            Dim posFine As Integer

            posSep = InStr(placeHolder, sepParametriPH)

            If posSep = 0 Then
                posFine = placeHolder.Length - 1
            Else
                posFine = posSep - 1
                parametriPH = estraiStringa(placeHolder, posSep + 1, placeHolder.Length - 1)
            End If

            Dim nomePlaceHolder = estraiStringa(placeHolder, posIniz, posFine)

            Return nomePlaceHolder

        End Function

        Private Shared Function valorizzaPH_Stringa(stringa As String,
                                                    parametriPH As String,
                                                    nomePlaceHolder As String,
                                                    ByRef messaggioErrore As String
                                                    ) As String

            Dim stringaFinale As String = String.Empty

            If String.IsNullOrEmpty(parametriPH) OrElse String.IsNullOrEmpty(stringa) Then
                stringaFinale = stringa
            Else
                Dim inizSubstring As Integer
                Dim lungSubstring As Integer
                Dim lungStringa = stringa.Length
                Dim elencoParPH = Split(parametriPH, ",")
                Dim numParPH = elencoParPH.Count
                Dim erroriParametri As Boolean = False

                Select Case numParPH
                    Case 1
                        If IsNumeric(elencoParPH(0)) Then
                            inizSubstring = 0
                            lungSubstring = elencoParPH(0)
                        Else
                            erroriParametri = True
                        End If

                    Case 2
                        If IsNumeric(elencoParPH(0)) Then
                            inizSubstring = elencoParPH(0) - 1
                        Else
                            erroriParametri = True
                        End If
                        If Not erroriParametri AndAlso IsNumeric(elencoParPH(1)) Then
                            lungSubstring = elencoParPH(1)
                        Else
                            erroriParametri = True
                        End If

                    Case Else
                        erroriParametri = True

                End Select

                If erroriParametri Then
                    Dim messaggio = String.Format("Formato parametri stringa errato: {0} - NomePH: {1}",
                                                  parametriPH,
                                                  nomePlaceHolder)
                    accodaErrore(messaggio, messaggioErrore)
                Else
                    Try
                        'Se inizio sottostringa superiore alla lunghezza stringa input,
                        'la stringa finale rimarrà vuota
                        If inizSubstring + 1 <= lungStringa Then
                            'Se inizio sottostringa + lunghezza sottostringa superiore a lunghezza stringa input,
                            'adatto in automatico la lunghezza sottostringa
                            If inizSubstring + 1 + lungSubstring > lungStringa Then
                                lungSubstring = lungStringa - inizSubstring
                            End If
                            stringaFinale = stringa.Substring(inizSubstring, lungSubstring)
                        End If
                    Catch ex As Exception
                        Dim messaggio = String.Format("Errore in Substring: {0} - NomePH: {1}",
                                                  parametriPH,
                                                  nomePlaceHolder)
                        accodaErrore(messaggio, messaggioErrore)
                    End Try
                End If

            End If

            Return Trim(stringaFinale)

        End Function

        Private Shared Function valorizzaPH_Data(data As Date,
                                                 parametriPH As String,
                                                 nomePlaceHolder As String,
                                                 ByRef messaggioErrore As String
                                                 ) As String

            Dim stringaFinale As String = String.Empty

            If String.IsNullOrEmpty(parametriPH) Then
                stringaFinale = data.ToString("yyyyMMdd")
            Else
                Try
                    If parametriPH = paramPH_GiornoDellAnno Then
                        stringaFinale = data.DayOfYear.ToString()
                    Else
                        stringaFinale = data.ToString(parametriPH)
                    End If
                Catch ex As Exception
                    Dim messaggio = String.Format("Errore in ToString: {0} - NomePH: {1}",
                                                  parametriPH,
                                                  nomePlaceHolder)
                    accodaErrore(messaggio, messaggioErrore)
                End Try

            End If

            Return Trim(stringaFinale)

        End Function

        Private Shared Sub accodaErrore(messaggio As String, ByRef messaggioErrore As String)

            If String.IsNullOrEmpty(messaggioErrore) Then
                messaggioErrore += "Errori in applicazione formato lotto: "
            End If

            messaggioErrore += vbCrLf & messaggio

        End Sub

    End Class

End Class

''' <summary>
''' Questa classe base raccoglie tutte le particolarità che sono gestite nei vari importatori tramite Parametri Aggiuntivi.
''' In caso di modifiche, verificare anche le seguenti classi private:
''' 1. ServizioImportazione_Malavasi.parametriAggMalavasi;
''' 2. ServizioImportazione_Unitec.parametriAggUnitec;
''' 3. ServizioImportazione_RsService_Ulma.parametriAggRS;
''' 4. ServizioEsportazione_Standard.parametriAggStandard.
''' </summary>

Public Class Utility_Lavorazioni_Macchina_Param_Agg

    Public ColonnaClasse As String = Utility_Lavorazioni_Macchina_Costanti.ColonnaClasseSigla

    Public TipoTracciato As String = Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoStandard

    Public ChiudiLavorazione As Boolean = False

    Public AggancioUscitaOrdineLav As Integer = Utility_Lavorazioni_Macchina_Costanti.AggancioUscitaOrdineLavDisabilitato

    Public Function SeColonnaClasseDescrizione() As Boolean

        Return (ColonnaClasse = Utility_Lavorazioni_Macchina_Costanti.ColonnaClasseDescrizione)

    End Function

    Public Function SeTipoTracciatoRovesciatore() As Boolean

        Return (TipoTracciato = Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoRovesciatore)

    End Function

End Class

Public Class Utility_Lavorazioni_Macchina_Costanti

    'Colonna classe
    Public Const ColonnaClasseSigla As String = "SIGLA"
    Public Const ColonnaClasseDescrizione As String = "DESCRIZIONE"

    'Tipo tracciato
    Public Const TipoTracciatoStandard As String = "STANDARD"
    Public Const TipoTracciatoRovesciatore As String = "ROVESCIATORE"

    'Aggancio uscita ordine lavorazione
    Public Const AggancioUscitaOrdineLavDisabilitato As Integer = 0
    Public Const AggancioUscitaOrdineLavPrimaRiga As Integer = 1


End Class

''' <summary>
''' Classe ritorno esito scrittura uscite lavorazioni
''' </summary>

Public Class esitoScritturaUsciteLavorazioni

    ''' <value>
    ''' Numero righe uscite lavorazioni inserite
    ''' </value>
    Public numRighe = 0

    ''' <value>
    ''' Id Agenda primo scarico collegato inserito
    ''' </value>
    Public primoIdAgendaScaricoColleg As Integer = 0

    ''' <value>
    ''' Id Movimento Dettaglio primo scarico collegato inserito
    ''' </value>
    Public primoIdMovDetScaricoColleg As Integer = 0

    ''' <value>
    ''' Indica se il primo movimento di scarico collegato inserito era il primo ingresso in assoluto
    ''' </value>
    Public primoMovScaricoCollegPrimoIngresso As Boolean = False

    ''' <value>
    ''' Numero scarichi collegati inseriti
    ''' </value>
    Public numScarColleg As Integer = 0

End Class

Public Class esitoElencoImportazioniUsciteLavorazioni

    Public numImportazioniOk As Integer

    Public numImportazioniErrate As Integer

    Public messaggioErrore As String

    Public totNumRigheImportate

    Public elencoEsitiScritture As New List(Of esitoScritturaUsciteLavorazioni)

End Class