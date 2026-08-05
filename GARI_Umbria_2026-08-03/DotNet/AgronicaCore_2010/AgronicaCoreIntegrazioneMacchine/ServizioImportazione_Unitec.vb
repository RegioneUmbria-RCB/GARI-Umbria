Imports System.IO
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ServizioImportazione_Unitec
    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _file As FileSystemHelper
    Private _txtParser As TXTParser
    Private _utilityIntegrMacchine As Utility_Integrazione_Macchine
    Private _utilityLavMacch As Utility_Lavorazioni_Macchina

    Public Sub New(configurazione_Servizio As Configurazione_Servizio,
                   configurazioneImportatore As ConfigurazioneImportatore,
                   objParametri As ObjParametri)
        _configurazioneServizio = configurazione_Servizio
        _configurazioneImportatore = configurazioneImportatore
        _objParametri = objParametri
        _logger = New Logger(_configurazioneServizio, _configurazioneImportatore, _objParametri)
        _file = New FileSystemHelper(configurazione_Servizio, _configurazioneImportatore)
        _txtParser = New TXTParser(_logger)
        _utilityIntegrMacchine = New Utility_Integrazione_Macchine(_objParametri.SuperServer, _objParametri.Server, _objParametri.Utenti, _configurazioneServizio)
        _utilityLavMacch = New Utility_Lavorazioni_Macchina(_objParametri, _logger)
    End Sub

    Public Function AvviaImportCalibratriceUnitec() As RispostaStandard
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
        Try
            ImportEsitoCalibraturaUnitec()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub ImportEsitoCalibraturaUnitec()
        Dim listaFileImport = _file.OttieniElencoFileDaImportare("*.txt")

        If listaFileImport.Count > 0 Then

            Dim nomeFile As String = String.Empty
            Dim msgErrore As String = String.Empty

            Dim codMacchinaLav = _utilityLavMacch.Ottieni_Macchina_Da_IdServizio(_configurazioneServizio.PivaSuperuser,
                                                                                 _configurazioneImportatore.IdServizio)

            _logger.Info("=== Inizio importazione macchina: " & codMacchinaLav)

            Dim parAggUnitec = CaricaParametriAggiuntiviUnitec(_configurazioneImportatore.ParametriAgg)

            For Each percorsoAssolutoFileImport In listaFileImport

                Try
                    nomeFile = _file.OttieneNomeFile(percorsoAssolutoFileImport)
                    msgErrore = String.Empty

                    _logger.Info("--- Lettura file: " & nomeFile)

                    Dim lavorazioneUnitec = _txtParser.LeggiCalibratureUnitec(percorsoAssolutoFileImport)

                    If String.IsNullOrEmpty(lavorazioneUnitec.lottoCalibrato) Then
                        Throw New Exception("Lotto non indicato nel file!")
                    End If

                    _logger.Info("Lettura ID lavorazione: " & lavorazioneUnitec.lottoCalibrato)

                    If lavorazioneUnitec.lottoCalibrato = parAggUnitec.LottoFittizio Then

                        _logger.Info("--- Lotto fittizio non elaborato")

                    Else

                        Dim piva = _configurazioneServizio.PivaSuperuser

                        Dim lavorazioneMacchina = creaLavorazioneMacchinaUnitec(piva, codMacchinaLav, lavorazioneUnitec,
                                                                                parAggUnitec, percorsoAssolutoFileImport)

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

                    End If

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

            _logger.Info("=== Fine importazione macchina: " & codMacchinaLav)

        Else

            _logger.Info("### Nessun file da importare")

        End If

    End Sub

    Private Function CaricaParametriAggiuntiviUnitec(parametriAggServizio As String) As parametriAggUnitec

        Dim parAggUnitec = New parametriAggUnitec

        If Not String.IsNullOrEmpty(_configurazioneImportatore.ParametriAgg) Then
            parAggUnitec = JsonConvert.DeserializeObject(Of parametriAggUnitec)(parametriAggServizio)
        End If

        Return parAggUnitec

    End Function

    Private Function creaLavorazioneMacchinaUnitec(piva As String,
                                                   codMacchinaLav As String,
                                                   lavUnitec As Lavorazione_Unitec,
                                                   parAggUnitec As parametriAggUnitec,
                                                   nomeFile As String
                                                   ) As cbl_Calibrature

        Dim lavMacchina As New cbl_Calibrature


        _utilityLavMacch.Imposta_Parametri_Agg(lavMacchina, parAggUnitec)

        lavMacchina.Piva = piva
        lavMacchina.Cod_Macchina_Lav = codMacchinaLav
        lavMacchina.Identif_Lavorazione = lavUnitec.lottoCalibrato

        lavMacchina.Data_Inizio = lavUnitec.dataOraInizio
        lavMacchina.Data_Fine = lavUnitec.dataOraFine

        Dim classeUscitaNonGestita As Boolean = False
        Dim classeUscitaCalibro As Boolean = False
        Dim classeUscitaQualita As Boolean = False

        Select Case parAggUnitec.ClasseUscita
            Case ParametriQualitativi_Calibro
                classeUscitaCalibro = True
            Case ParametriQualitativi_Qualita
                classeUscitaQualita = True
            Case Else
                classeUscitaNonGestita = True
        End Select

        If classeUscitaNonGestita Then

            lavMacchina.Lotto = lavUnitec.lottoCalibrato

            Dim rigaLavMacchina As New cbl_CalibratureXCalibri
            rigaLavMacchina.Peso = lavUnitec.esitoCalibratura.Sum(Function(s) s.pesoProdotti)
            rigaLavMacchina.Udm_Cod = enum_UnitaMisura.KG
            lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)

        Else

            lavMacchina.Lotto = String.Empty

            For Each esito In lavUnitec.esitoCalibratura

                Dim rigaLavMacchina As New cbl_CalibratureXCalibri
                If classeUscitaCalibro Then
                    rigaLavMacchina.Nome = esito.classe
                Else
                    rigaLavMacchina.Qualita = esito.classe
                End If
                rigaLavMacchina.Peso = esito.pesoProdotti
                rigaLavMacchina.Udm_Cod = enum_UnitaMisura.KG
                lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)

            Next

        End If

        'Caricamento log importazione

        Dim logLavMacchina As New cbl_LogImportazioni
        logLavMacchina.nomeFile = nomeFile
        lavMacchina.cbl_LogImportazioni.Add(logLavMacchina)

        Return lavMacchina

    End Function

#Region "Classi private"
    Private Class TXTParser

        Private _logger As Logger
        Public Sub New(ByRef logger As Logger)
            _logger = logger
        End Sub
        Public Function LeggiCalibratureUnitec(percorsoAssoluto As String) As Lavorazione_Unitec
            Dim lavorazione As New Lavorazione_Unitec
            Dim elencoCalibrature As New List(Of Calibratura_Unitec)

            Dim nomeFile = percorsoAssoluto
            Dim righeDelFile As List(Of String) = New List(Of String)
            Dim rigaFile As String
            Dim rigaSplittata() As String
            Dim testaStringa As Object
            Dim elemSplittati As Integer
            Dim classe As String
            Dim numero As Integer
            Dim peso As Decimal

            ' Lettura file
            If File.Exists(nomeFile) Then
                righeDelFile = File.ReadAllLines(nomeFile).ToList
            End If

            If righeDelFile.Count >= 1 Then
                ' Nella prima riga del file è presente il codice del lotto calibrato
                lavorazione.lottoCalibrato = righeDelFile(0)
            End If

            If righeDelFile.Count >= 9 Then
                ' Dalla riga 6 alla 9 ci sono i sequenti dati:
                ' - riga 6 : Data inizio
                ' - riga 7 : Ora  inizio
                ' - riga 8 : Data fine
                ' - riga 9 : Ora  fine
                lavorazione.dataOraInizio = ConvertiDataOraFile(Trim(righeDelFile(5)), Trim(righeDelFile(6)), "inizio")
                lavorazione.dataOraFine = ConvertiDataOraFile(Trim(righeDelFile(7)), Trim(righeDelFile(8)), "fine")
            Else
                _logger.Warn("Data/ora inizio e fine non indicate")
            End If

            If righeDelFile.Count >= 11 Then
                ' Dalla riga 11 in poi ci sono gli esiti della calibratura
                For indice As Integer = 10 To (righeDelFile.Count - 1)
                    rigaFile = righeDelFile(indice)
                    rigaSplittata = rigaFile.Split(";")
                    elemSplittati = rigaSplittata.Count
                    If elemSplittati <> 3 Then Exit For
                    testaStringa = rigaSplittata(1)
                    If Not IsNumeric(testaStringa) Then Exit For
                    testaStringa = rigaSplittata(2)
                    If Not IsNumeric(testaStringa) Then Exit For
                    classe = rigaSplittata(0)
                    numero = rigaSplittata(1)
                    peso = rigaSplittata(2)
                    If Not String.IsNullOrEmpty(classe) AndAlso numero > 0 AndAlso peso > 0 Then
                        Dim calibratura As New Calibratura_Unitec With {.classe = classe, .numeroProdotti = numero, .pesoProdotti = peso}
                        elencoCalibrature.Add(calibratura)
                    End If
                Next
            End If

            lavorazione.esitoCalibratura = elencoCalibrature

            Return lavorazione

        End Function

        Private Function ConvertiDataOraFile(Data As String, Ora As String, Tipo As String) As DateTime
            Dim dataOraConvertita As DateTime = Nothing
            Try
                dataOraConvertita = DateTime.ParseExact(Data & " " & Ora, "dd/MM/yyyy HH.mm.ss", Nothing)
                If dataOraConvertita = Nothing Then
                    _logger.Warn("Errore in conversione data/ora " & Tipo & ": " & Data & " " & Ora)
                End If
            Catch ex As Exception
                _logger.Warn("Errore inaspettato in conversione data/ora " & Tipo & ": " & Data & " " & Ora)
            End Try
            Return dataOraConvertita
        End Function

    End Class

    Private Class parametriAggUnitec : Inherits Utility_Lavorazioni_Macchina_Param_Agg
        Public LottoFittizio As String
        Public ClasseUscita As String
    End Class

#End Region

End Class