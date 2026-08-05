Imports System.Globalization
Imports System.Text
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ServizioImportazione_RsService_Ulma
    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _file As FileSystemHelper
    Private _xmlParser As XMLParser
    Private _logger As Logger
    Private _utilityIntegrMacchine As Utility_Integrazione_Macchine
    Private _utilityLavMacch As Utility_Lavorazioni_Macchina

    Public Sub New(configurazione_Servizio As Configurazione_Servizio,
                   configurazioneImportatore As ConfigurazioneImportatore,
                   objParametri As ObjParametri
                )
        _configurazioneServizio = configurazione_Servizio
        _configurazioneImportatore = configurazioneImportatore
        _objParametri = objParametri
        _file = New FileSystemHelper(configurazione_Servizio, _configurazioneImportatore)
        _logger = New Logger(_configurazioneServizio, _configurazioneImportatore, objParametri)
        _xmlParser = New XMLParser(_logger)
        _utilityIntegrMacchine = New Utility_Integrazione_Macchine(_objParametri.SuperServer, _objParametri.Server, _objParametri.Utenti, _configurazioneServizio)
        _utilityLavMacch = New Utility_Lavorazioni_Macchina(_objParametri, _logger)
    End Sub

    Public Function AvviaImportBilanciaDati() As RispostaStandard
        Dim r As New RispostaStandard()
        Try

            TentareImportazione()
            r.RispostaOK = True
            r.RispostaStringa = _logger.GetLogErrori()
            If String.IsNullOrEmpty(r.RispostaStringa) Then
                r.RispostaStringa = "Elaborazione terminata correttamente."
            End If
        Catch ex As Exception
            r.Errore = "Errore durante l'operazione: " & vbCrLf & Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, True)
            r.RispostaOK = False
            _logger.Error(r.Errore)
        End Try
        Return r
    End Function

    Private Sub TentareImportazione()
        _file.CrearePercorsiImport()
        ImportEsitoBilanciaRS()
    End Sub

    Private Sub ImportEsitoBilanciaRS()
        Dim listaFileImport = _file.OttieniElencoFileDaImportare("*.csv")

        If listaFileImport.Count > 0 Then

            Dim msgErrore As String = String.Empty
            Dim nomeFile As String = String.Empty

            Dim codMacchinaLav = _utilityLavMacch.Ottieni_Macchina_Da_IdServizio(_configurazioneServizio.PivaSuperuser,
                                                                                 _configurazioneImportatore.IdServizio)

            _logger.Info("=== Inizio importazione macchina: " & codMacchinaLav)

            Dim parAggRS = CaricaParametriAggiuntiviRS(_configurazioneImportatore.ParametriAgg)

            For Each percorsoAssolutoFileImport In listaFileImport

                Try
                    msgErrore = String.Empty
                    nomeFile = _file.OttieneNomeFile(percorsoAssolutoFileImport)

                    _logger.Info("--- Lettura file: " & nomeFile)

                    Dim listaLavorazioniRS = _xmlParser.LeggiLavorazioni(percorsoAssolutoFileImport)

                    If listaLavorazioniRS.Count = 0 Then
                        Throw New Exception("File senza righe da elaborare: " & nomeFile)
                    End If

                    Dim piva = _configurazioneServizio.PivaSuperuser

                    Dim listaLavorazioniMacchina = creaListaLavorazioniMacchinaRS(piva,
                                                                                  codMacchinaLav,
                                                                                  listaLavorazioniRS,
                                                                                  parAggRS,
                                                                                  percorsoAssolutoFileImport)

                    For Each lavorazioneMacchina In listaLavorazioniMacchina

                        Dim opzioniTransazione = _utilityLavMacch.CreaOpzioniTransazione

                        Dim esitoImportLavMacch As New esitoScritturaUsciteLavorazioni

                        Using scopeImportazione As New TransactionScope(TransactionScopeOption.Required, opzioniTransazione)

                            _utilityLavMacch.Scrivi_Lavorazione_Macchina(lavorazioneMacchina)

                            esitoImportLavMacch = _utilityLavMacch.Se_Esegui_Importazione_Immediata(_configurazioneImportatore.ImportazioneImmediata,
                                                                                                    lavorazioneMacchina)

                            scopeImportazione.Complete()

                        End Using

                        _utilityLavMacch.Se_Invio_Primo_Ingresso(_configurazioneImportatore.ImportazioneImmediata,
                                                                 lavorazioneMacchina,
                                                                 esitoImportLavMacch,
                                                                 _utilityIntegrMacchine)

                    Next

                    _logger.Info("--- Terminata importazione file: " & nomeFile)

                Catch ex As Exception
                    msgErrore = ex.Message
                Finally
                    If msgErrore = String.Empty Then
                        _file.SpostareFileInArchivioOK(percorsoAssolutoFileImport, nomeFile)
                    Else
                        _logger.Error(msgErrore)
                        _file.SpostareFileInArchivioERR(percorsoAssolutoFileImport, nomeFile)
                    End If
                End Try
            Next
        Else

            _logger.Info("### Nessun file da importare")

        End If

    End Sub

    Private Function CaricaParametriAggiuntiviRS(parametriAggServizio As String) As parametriAggRS

        Dim parAggRS = New parametriAggRS

        If Not String.IsNullOrEmpty(_configurazioneImportatore.ParametriAgg) Then
            parAggRS = JsonConvert.DeserializeObject(Of parametriAggRS)(parametriAggServizio)
        End If

        Return parAggRS

    End Function

    Private Function creaListaLavorazioniMacchinaRS(piva As String,
                                                    codMacchinaLav As String,
                                                    listaLavorazioniRS As List(Of Lavorazione_RSService),
                                                    parAggRS As parametriAggRS,
                                                    nomeFile As String
                                                    ) As List(Of cbl_Calibrature)

        Dim elencolavMacchina As New List(Of cbl_Calibrature)

        Dim numPesateTotali = listaLavorazioniRS.Count

        'Filtro solo elementi che rispettano la soglia

        listaLavorazioniRS = listaLavorazioniRS.Where(Function(s) s.PesoInSoglia = 1).ToList()

        Dim numPesateInSoglia = listaLavorazioniRS.Count

        _logger.InfoFormat("Pesate Totali: {0} - Pesate in soglia: {1}", numPesateTotali, numPesateInSoglia)

        'Raggruppo per i dati significativi

        Dim gruppi = listaLavorazioniRS.GroupBy(Function(s) New With {Key s.CodePLUStore,
                                                                          Key s.TaraStore,
                                                                          Key s.LottoStore,
                                                                          Key s.CodProduzioneStore,
                                                                          Key s.DescrizioneStore,
                                                                          Key s.DataStore})

        For Each gruppo In gruppi

            Dim codProduzione = Trim(gruppo.Key.CodProduzioneStore)

            If String.IsNullOrEmpty(codProduzione) Then
                Throw New Exception("Codice produzione non indicato")
            End If

            _logger.Info("Lettura ID lavorazione: " & codProduzione)

            'Caricamento dati testata

            Dim lavMacchina As New cbl_Calibrature

            _utilityLavMacch.Imposta_Parametri_Agg(lavMacchina, parAggRS)

            lavMacchina.Piva = piva
            lavMacchina.Cod_Macchina_Lav = codMacchinaLav
            lavMacchina.Identif_Lavorazione = codProduzione

            lavMacchina.Lotto = gruppo.Key.LottoStore
            lavMacchina.Note = gruppo.Key.DescrizioneStore

            Dim data = gruppo.Key.DataStore
            Dim oraMin = gruppo.Min(Function(s) s.OraStore.ToDatetime())
            lavMacchina.Data_Inizio = New DateTime(data.Year, data.Month, data.Day, oraMin.Hour, oraMin.Minute, oraMin.Second)

            Dim oraMax = gruppo.Max(Function(s) s.OraStore.ToDatetime())
            lavMacchina.Data_Fine = New DateTime(data.Year, data.Month, data.Day, oraMax.Hour, oraMax.Minute, oraMax.Second)

            'Caricamento dati riga

            Dim numRigheGruppo = gruppo.Count
            Dim sommaPesoGruppo = gruppo.Sum(Function(s) s.PesoScStore)
            Dim tara = gruppo.Key.TaraStore * numRigheGruppo

            Dim rigaLavMacchina As New cbl_CalibratureXCalibri
            rigaLavMacchina.Nome = ""
            rigaLavMacchina.Qualita = ""
            rigaLavMacchina.Num = numRigheGruppo
            rigaLavMacchina.Peso = sommaPesoGruppo
            rigaLavMacchina.Tara = tara
            rigaLavMacchina.Udm_Cod = enum_UnitaMisura.Numero
            lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)

            'Caricamento log importazione

            Dim logLavMacchina As New cbl_LogImportazioni
            logLavMacchina.nomeFile = nomeFile
            lavMacchina.cbl_LogImportazioni.Add(logLavMacchina)

            'Applica eventuale formato lotto

            If Not String.IsNullOrEmpty(parAggRS.FormatoLotto) Then

                Dim messaggioErroreLotto As String = String.Empty
                lavMacchina.Lotto = _utilityLavMacch.ApplicaFormatoLotto(parAggRS.FormatoLotto,
                                                                         lavMacchina,
                                                                         messaggioErroreLotto)
                If Not String.IsNullOrEmpty(messaggioErroreLotto) Then
                    Throw New Exception(messaggioErroreLotto)
                End If

            End If

            elencolavMacchina.Add(lavMacchina)

        Next

        Return elencolavMacchina

    End Function

#Region "Classi private"

    Private Class XMLParser

        Private _logger As Logger

        Sub New(ByVal logger As Logger)
            _logger = logger
        End Sub

        Public Function LeggiLavorazioni(percorsoAssoluto As String) As List(Of Lavorazione_RSService)

            Dim elencoLavorazioni As New List(Of Lavorazione_RSService)

            Dim lavorazione As New Lavorazione_RSService

            Using parser As New FileIO.TextFieldParser(percorsoAssoluto,
                                                       New UnicodeEncoding(True, True)
                                                       )

                parser.HasFieldsEnclosedInQuotes = False
                parser.Delimiters = {";"}
                Dim numrow As Integer = 0
                parser.ReadFields()

                While Not parser.EndOfData
                    Try
                        numrow += 1
                        Dim lav As Lavorazione_RSService = LeggiRiga(parser.ReadFields())
                        elencoLavorazioni.Add(lav)
                    Catch ex As FileIO.MalformedLineException
                        _logger.Error("Formato record non valido alla riga " & numrow & " - Errore: " & ex.Message)
                    End Try
                End While

                Return elencoLavorazioni

            End Using

        End Function

        Private Function LeggiRiga(campi As String()) As Lavorazione_RSService
            Dim lavorazione As New Lavorazione_RSService()
            lavorazione.IdStore = Integer.Parse(campi(0))
            lavorazione.DeviceStore = campi(1)
            lavorazione.DataStore = campi(2)
            lavorazione.OraStore = New Tempo(campi(3))
            lavorazione.ScadenzaStore = campi(4)
            lavorazione.CodePLUStore = campi(5)
            lavorazione.DescrizioneStore = campi(6)
            lavorazione.CodeTraceabilityStore = campi(7)
            lavorazione.IdClienteStore = campi(8)
            lavorazione.LottoStore = campi(9)
            lavorazione.CodProduzioneStore = campi(10)
            lavorazione.TaraStore = Double.Parse(campi(11), CultureInfo.InvariantCulture)
            lavorazione.ScartoStore = Double.Parse(campi(12), CultureInfo.InvariantCulture)
            lavorazione.PesoScStore = Double.Parse(campi(13), CultureInfo.InvariantCulture)
            lavorazione.ImportoScStore = Double.Parse(campi(14), CultureInfo.InvariantCulture)
            lavorazione.PesoInSoglia = campi(15)
            lavorazione.CheckStore = campi(16)
            Return lavorazione
        End Function
    End Class

    Private Class parametriAggRS : Inherits Utility_Lavorazioni_Macchina_Param_Agg
        Public FormatoLotto As String
    End Class

#End Region

End Class