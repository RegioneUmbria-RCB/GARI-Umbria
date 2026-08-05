Imports System.IO
Imports System.Transactions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ServizioImportazione_Malavasi
    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _file As FileSystemHelper
    Private _malavasiParser As MalavasiParser
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
        _malavasiParser = New MalavasiParser(_logger)
        _utilityIntegrMacchine = New Utility_Integrazione_Macchine(_objParametri.SuperServer, _objParametri.Server, _objParametri.Utenti, _configurazioneServizio)
        _utilityLavMacch = New Utility_Lavorazioni_Macchina(_objParametri, _logger)
    End Sub

    Public Function AvviaImportCalibratriceMalavasi() As RispostaStandard
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
            ImportEsitoCalibraturaMalavasi()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub ImportEsitoCalibraturaMalavasi()
        Dim listaFileImport = _file.OttieniElencoFileDaImportare("*.csv")

        If listaFileImport.Count > 0 Then

            Dim nomeFile As String = String.Empty
            Dim msgErrore As String = String.Empty

            Dim codMacchinaLav = _utilityLavMacch.Ottieni_Macchina_Da_IdServizio(_configurazioneServizio.PivaSuperuser,
                                                                                 _configurazioneImportatore.IdServizio)

            _logger.Info("=== Inizio importazione macchina: " & codMacchinaLav)

            Dim parAggMalavasi = CaricaParametriAggiuntiviMalavasi(_configurazioneImportatore.ParametriAgg)

            For Each percorsoAssolutoFileImport In listaFileImport

                Try
                    nomeFile = _file.OttieneNomeFile(percorsoAssolutoFileImport)
                    msgErrore = String.Empty

                    _logger.Info("--- Lettura file: " & nomeFile)

                    Dim lavorazioneMalavasi = _malavasiParser.LeggiCalibratureMalavasi(percorsoAssolutoFileImport, parAggMalavasi)

                    If String.IsNullOrEmpty(lavorazioneMalavasi.lottoCalibrato) Then
                        Throw New Exception("Lotto non indicato nel file!")
                    End If

                    _logger.Info("Lettura ID lavorazione: " & lavorazioneMalavasi.lottoCalibrato)

                    Dim piva = _configurazioneServizio.PivaSuperuser

                    Dim lavorazioneMacchina = creaLavorazioneMacchinaMalavasi(piva, codMacchinaLav, lavorazioneMalavasi,
                                                                              parAggMalavasi, percorsoAssolutoFileImport)

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

    Private Function CaricaParametriAggiuntiviMalavasi(parametriAggServizio As String) As parametriAggMalavasi

        Dim parAggMalavasi = New parametriAggMalavasi

        If Not String.IsNullOrEmpty(parametriAggServizio) Then
            parAggMalavasi = JsonConvert.DeserializeObject(Of parametriAggMalavasi)(parametriAggServizio)
        End If

        Return parAggMalavasi

    End Function

    Private Function creaLavorazioneMacchinaMalavasi(piva As String,
                                                     codMacchinaLav As String,
                                                     lavMalavasi As Lavorazione_Malavasi,
                                                     parAggMalavasi As parametriAggMalavasi,
                                                     nomeFile As String
                                                     ) As cbl_Calibrature

        Dim lavMacchina As New cbl_Calibrature

        _utilityLavMacch.Imposta_Parametri_Agg(lavMacchina, parAggMalavasi)

        lavMacchina.Piva = piva
        lavMacchina.Cod_Macchina_Lav = codMacchinaLav
        lavMacchina.Identif_Lavorazione = lavMalavasi.lottoCalibrato

        lavMacchina.Lotto = String.Empty
        lavMacchina.Data_Inizio = lavMalavasi.dataInizioCalibratura
        lavMacchina.Data_Fine = lavMalavasi.dataFineCalibratura

        If parAggMalavasi.TipoTracciato = Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoRovesciatore Then

            'Rovesciatore

            If lavMalavasi.numeroBins > 0 Then
                Dim rigaLavMacchina As New cbl_CalibratureXCalibri
                rigaLavMacchina.Nome = parAggMalavasi.CalibroRovesciatore
                rigaLavMacchina.Qualita = parAggMalavasi.QualitaRovesciatore
                rigaLavMacchina.Num = lavMalavasi.numeroBins
                rigaLavMacchina.Udm_Cod = enum_UnitaMisura.KG
                lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)
            Else
                Throw New Exception("Numero bins non indicato")
            End If

        Else

            'Calibratrice

            For Each esito In lavMalavasi.esitoCalibratura
                Dim rigaLavMacchina As New cbl_CalibratureXCalibri
                rigaLavMacchina.Nome = esito.calibro
                rigaLavMacchina.Qualita = parAggMalavasi.QualitaPrimaScelta
                rigaLavMacchina.Peso = esito.pesoProdotti
                rigaLavMacchina.Udm_Cod = enum_UnitaMisura.KG
                lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)
            Next

            If lavMalavasi.pesoSecondaScelta > 0 Then
                Dim rigaLavMacchina As New cbl_CalibratureXCalibri
                rigaLavMacchina.Nome = parAggMalavasi.CalibroSecondaScelta
                rigaLavMacchina.Qualita = parAggMalavasi.QualitaSecondaScelta
                rigaLavMacchina.Peso = lavMalavasi.pesoSecondaScelta
                rigaLavMacchina.Udm_Cod = enum_UnitaMisura.KG
                lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)
            End If

        End If

        'Caricamento log importazione

        Dim logLavMacchina As New cbl_LogImportazioni
        logLavMacchina.nomeFile = nomeFile
        lavMacchina.cbl_LogImportazioni.Add(logLavMacchina)

        Return lavMacchina

    End Function

#Region "Classi private"
    Private Class MalavasiParser

        Private _logger As Logger
        Public Sub New(ByRef logger As Logger)
            _logger = logger
        End Sub
        Public Function LeggiCalibratureMalavasi(percorsoAssoluto As String, parAggMalavasi As parametriAggMalavasi) As Lavorazione_Malavasi
            Dim lavorazione As New Lavorazione_Malavasi
            Dim elencoCalibrature As New List(Of Calibratura_Malavasi)

            If parAggMalavasi.TipoTracciato <> Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoStandard AndAlso
               parAggMalavasi.TipoTracciato <> Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoRovesciatore Then
                Throw New Exception("Tipo tracciato non previsto: " & parAggMalavasi.TipoTracciato)
            End If

            Dim nomeFile = percorsoAssoluto
            Dim righeDelFile As List(Of String) = New List(Of String)
            Dim rigaFile As String
            Dim rigaSplittata() As String
            Dim elemSplittati As Integer
            Dim calibro As String
            Dim peso As Decimal

            Dim trovatoLotto As Boolean = False
            Dim trovataIntestazioneCalibri As Boolean = False

            ' Lettura file
            If File.Exists(nomeFile) Then
                righeDelFile = File.ReadAllLines(nomeFile).ToList
            End If

            If righeDelFile.Count > 0 Then
                For indice As Integer = 0 To (righeDelFile.Count - 1)
                    rigaFile = righeDelFile(indice)
                    rigaSplittata = rigaFile.Split(";")
                    elemSplittati = rigaSplittata.Count
                    ' Cerco la riga di intestazione dei dati lotto: se la trovo, prendo il lotto dalla riga successiva
                    If (Not trovatoLotto) AndAlso elemSplittati > 0 AndAlso rigaSplittata(0) = parAggMalavasi.IntestazioneLotto Then
                        lavorazione.lottoCalibrato = ottieniLottoDaElementoSuccessivo(righeDelFile, indice)
                        trovatoLotto = True
                        ' Prendo data/ora di inizio/fine dalle righe 1 e 2
                        lavorazione.dataInizioCalibratura = ottieniDataDaElemento(righeDelFile, 0)
                        lavorazione.dataFineCalibratura = ottieniDataDaElemento(righeDelFile, 1)
                        ' Se rovesciatore, ottengo numero bins da riga 7
                        If parAggMalavasi.TipoTracciato = Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoRovesciatore Then
                            lavorazione.numeroBins = ottieniNumeroBinsDaElemento(righeDelFile, 6)
                        End If
                        Continue For
                    End If
                    If trovatoLotto And parAggMalavasi.TipoTracciato = Utility_Lavorazioni_Macchina_Costanti.TipoTracciatoStandard Then
                        Dim elencoColonneIntestazioneCalibri() As String = parAggMalavasi.IntestazioneCalibri.Split(";")
                        Dim colonneIntestazioneCalibri As Integer = elencoColonneIntestazioneCalibri.Count
                        If (Not trovataIntestazioneCalibri) Then
                            trovataIntestazioneCalibri = cercaIntestazioneCalibri(rigaFile,
                                                                                  elemSplittati,
                                                                                  rigaSplittata,
                                                                                  colonneIntestazioneCalibri,
                                                                                  elencoColonneIntestazioneCalibri)
                        Else
                            ' Se trovata intestazione e presente etichetta seconda scelta, carico il rispettivo peso
                            If elemSplittati >= 4 AndAlso rigaSplittata(2).Contains(parAggMalavasi.EtichettaSecondaScelta) Then
                                Dim pesoStringa As String = rigaSplittata(3)
                                If IsNumeric(pesoStringa) Then
                                    lavorazione.pesoSecondaScelta = pesoStringa
                                Else
                                    Throw New Exception("Peso seconda scelta non numerico")
                                End If
                            Else
                                ' Se trovata intestazione e sono valorizzati i campi obbligatori, carico il calibro
                                If elemSplittati >= 4 AndAlso (Not String.IsNullOrEmpty(Trim(rigaSplittata(0)))) AndAlso IsNumeric(rigaSplittata(1)) Then
                                    calibro = Trim(rigaSplittata(0))
                                    peso = rigaSplittata(3)
                                    If peso > 0 Then
                                        Dim calibratura As New Calibratura_Malavasi With {.calibro = calibro, .pesoProdotti = peso}
                                        elencoCalibrature.Add(calibratura)
                                    End If
                                End If
                            End If
                        End If
                    End If
                Next
            End If

            lavorazione.esitoCalibratura = elencoCalibrature

            Return lavorazione

        End Function

        Private Function ottieniLottoDaElementoSuccessivo(righeDelFile As List(Of String), indice As Integer) As String
            Dim lotto As String = String.Empty
            Dim indiceSucc As Integer
            Dim rigaFile As String
            Dim rigaSplittata() As String

            If righeDelFile.Count > indice Then
                indiceSucc = indice + 1
                rigaFile = righeDelFile(indiceSucc)
                rigaSplittata = rigaFile.Split(";")
                If rigaSplittata.Count >= 4 Then
                    lotto = Trim(rigaSplittata(3))
                End If
            End If

            Return lotto
        End Function

        Private Function ottieniDataDaElemento(righeDelFile As List(Of String), indice As Integer) As DateTime
            Dim dataCal As DateTime
            Dim dataStr As String
            Dim rigaFile As String
            Dim rigaSplittata() As String

            If righeDelFile.Count >= indice Then
                rigaFile = righeDelFile(indice)
                rigaSplittata = rigaFile.Split(";")
                If rigaSplittata.Count >= 2 Then
                    dataStr = Trim(rigaSplittata(1))
                    dataCal = ConvertiDataFile(dataStr)
                End If
            End If

            Return dataCal
        End Function

        Private Function ConvertiDataFile(dataStr As String) As DateTime
            Dim dataConvertita As DateTime = Nothing
            Try
                dataStr = Replace(dataStr, "-", "/")
                Dim formatoData As String = ""
                If dataStr.Substring(4, 1) = "/" AndAlso dataStr.Substring(7, 1) = "/" Then
                    formatoData = "yyyy/MM/dd"
                End If
                If dataStr.Substring(2, 1) = "/" AndAlso dataStr.Substring(5, 1) = "/" Then
                    formatoData = "dd/MM/yyyy"
                End If
                If Not String.IsNullOrEmpty(formatoData) Then
                    Select Case dataStr.Length
                        Case 19
                            formatoData += " HH:mm:ss"
                        Case 16
                            formatoData += " HH:mm"
                    End Select
                End If
                If Not String.IsNullOrEmpty(formatoData) Then
                    dataConvertita = DateTime.ParseExact(dataStr, formatoData, Nothing)
                End If
                If IsNothing(dataConvertita) Then
                    _logger.Warn("Errore in conversione data " & dataStr)
                    dataConvertita = Date.Today()
                End If
            Catch ex As Exception
                _logger.Warn("Errore inaspettato in conversione data " & dataStr & " - " & ex.Message)
                dataConvertita = Date.Today()
            End Try
            Return dataConvertita
        End Function

        Private Function ottieniNumeroBinsDaElemento(righeDelFile As List(Of String), indice As Integer) As Integer
            Dim numeroBins As Integer = 0
            Dim numeroBinsStr As String
            Dim rigaFile As String
            Dim rigaSplittata() As String

            If righeDelFile.Count >= indice Then
                rigaFile = righeDelFile(indice)
                rigaSplittata = rigaFile.Split(";")
                If rigaSplittata.Count >= 2 Then
                    numeroBinsStr = Trim(rigaSplittata(1))
                    If IsNumeric(numeroBinsStr) Then
                        numeroBins = numeroBinsStr
                    End If
                End If
            End If

            Return numeroBins
        End Function

        Private Function cercaIntestazioneCalibri(rigaFile As String,
                                                  elemSplittati As Integer,
                                                  rigaSplittata() As String,
                                                  colonneIntestazioneCalibri As Integer,
                                                  elencoColonneIntestazioneCalibri() As String)

            Dim trovataIntestazioneCalibri As Boolean = False

            If elemSplittati >= colonneIntestazioneCalibri Then
                Dim elemTrovati As Integer = 0
                For indIntCalibri As Integer = 0 To (colonneIntestazioneCalibri - 1)
                    If rigaFile.Contains(elencoColonneIntestazioneCalibri(indIntCalibri)) Then
                        For indRigaSplit As Integer = 0 To (elemSplittati - 1)
                            If Trim(elencoColonneIntestazioneCalibri(indIntCalibri)) = Trim(rigaSplittata(indRigaSplit)) Then
                                elemTrovati += 1
                                Exit For
                            End If
                        Next
                    Else
                        Exit For
                    End If
                Next
                If colonneIntestazioneCalibri = elemTrovati Then
                    trovataIntestazioneCalibri = True
                End If
            End If

            Return trovataIntestazioneCalibri
        End Function

    End Class

    Private Class parametriAggMalavasi : Inherits Utility_Lavorazioni_Macchina_Param_Agg
        Public IntestazioneLotto As String
        Public IntestazioneCalibri As String
        ' Prima scelta
        Public QualitaPrimaScelta As String
        ' Seconda scelta
        Public EtichettaSecondaScelta As String
        Public QualitaSecondaScelta As String
        Public CalibroSecondaScelta As String
        ' Rovesciatore
        Public QualitaRovesciatore As String
        Public CalibroRovesciatore As String
    End Class

#End Region

End Class