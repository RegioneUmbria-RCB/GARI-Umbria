Imports System.IO
Imports System.Transactions
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ServizioImportazione_Standard
    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _file As FileSystemHelper
    Private _xmlParser As xmlParser
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
        _xmlParser = New xmlParser(_logger)
        _utilityIntegrMacchine = New Utility_Integrazione_Macchine(_objParametri.SuperServer, _objParametri.Server, _objParametri.Utenti, _configurazioneServizio)
        _utilityLavMacch = New Utility_Lavorazioni_Macchina(_objParametri, _logger)
    End Sub

    Public Function AvviaImportMacchinaStandard() As RispostaStandard
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
            ImportEsitoMacchinaStandard()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub ImportEsitoMacchinaStandard()
        Dim listaFileImport = _file.OttieniElencoFileDaImportare("*.xml")

        If listaFileImport.Count > 0 Then

            Dim nomeFile As String = String.Empty
            Dim msgErrore As String = String.Empty

            Dim codMacchinaLav = _utilityLavMacch.Ottieni_Macchina_Da_IdServizio(_configurazioneServizio.PivaSuperuser,
                                                                                 _configurazioneImportatore.IdServizio)

            _logger.Info("=== Inizio importazione macchina: " & codMacchinaLav)

            Dim parAggStandard = CaricaParametriAggiuntiviStandard(_configurazioneImportatore.ParametriAgg)

            For Each percorsoAssolutoFileImport In listaFileImport

                Try
                    nomeFile = _file.OttieneNomeFile(percorsoAssolutoFileImport)
                    msgErrore = String.Empty

                    _logger.Info("--- Lettura file: " & nomeFile)

                    Dim lavorazioneStandard = _xmlParser.LeggiLavorazioneStandard(percorsoAssolutoFileImport, parAggStandard)

                    If String.IsNullOrEmpty(lavorazioneStandard.IdLavorazione) Then
                        Throw New Exception("Identificativo lavorazione non indicato nel file!")
                    End If

                    _logger.Info("Lettura ID lavorazione: " & lavorazioneStandard.IdLavorazione)

                    Dim piva = _configurazioneServizio.PivaSuperuser

                    Dim lavorazioneMacchina = CreaLavorazioneMacchinaStandard(piva, codMacchinaLav, lavorazioneStandard,
                                                                              parAggStandard, percorsoAssolutoFileImport)

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

    Private Function CaricaParametriAggiuntiviStandard(parametriAggServizio As String) As parametriAggStandard

        Dim parAggStandard = New parametriAggStandard

        If Not String.IsNullOrEmpty(parametriAggServizio) Then
            parAggStandard = JsonConvert.DeserializeObject(Of parametriAggStandard)(parametriAggServizio)
        End If

        Return parAggStandard

    End Function

    Private Function CreaLavorazioneMacchinaStandard(piva As String,
                                                     codMacchinaLav As String,
                                                     lavStandard As EsitoLavorazione,
                                                     parAggStandard As parametriAggStandard,
                                                     nomeFile As String
                                                     ) As cbl_Calibrature

        Dim messaggioErrore As String = String.Empty

        Dim lavMacchina As New cbl_Calibrature

        _utilityLavMacch.Imposta_Parametri_Agg(lavMacchina, parAggStandard)

        lavMacchina.Piva = piva
        lavMacchina.Cod_Macchina_Lav = codMacchinaLav
        lavMacchina.Identif_Lavorazione = lavStandard.IdLavorazione

        lavMacchina.Lotto = lavStandard.Lotto

        lavMacchina.Data_Inizio = CaricaDataOraDaXML("DataInizio", lavStandard.DataInizio,
                                                     "OraInizio", lavStandard.OraInizio,
                                                     messaggioErrore)

        If Not String.IsNullOrEmpty(lavStandard.DataFine) OrElse
           Not String.IsNullOrEmpty(lavStandard.OraFine) Then
            lavMacchina.Data_Fine = CaricaDataOraDaXML("DataFine", lavStandard.DataFine,
                                                       "OraFine", lavStandard.OraFine,
                                                       messaggioErrore)
        End If

        'Caricamento righe

        Dim riga As Integer = 0

        For Each rigaLavStd In lavStandard.Dettagli

            riga += 1
            Dim rigaLavMacchina As New cbl_CalibratureXCalibri

            rigaLavMacchina.Nome = rigaLavStd.Calibro
            rigaLavMacchina.Qualita = rigaLavStd.Qualita
            rigaLavMacchina.Peso = CaricaDatoNumericoDaXML("Peso", rigaLavStd.Peso, riga, messaggioErrore)
            rigaLavMacchina.Num = CaricaDatoNumericoDaXML("Numero", rigaLavStd.Numero, riga, messaggioErrore)
            rigaLavMacchina.Tara = CaricaDatoNumericoDaXML("Tara", rigaLavStd.Tara, riga, messaggioErrore)
            rigaLavMacchina.Udm_Cod = CaricaUnitaMisuraDaXML(rigaLavStd.UnitaMisura, riga, messaggioErrore)

            lavMacchina.cbl_CalibratureXCalibri.Add(rigaLavMacchina)

        Next

        'Caricamento log importazione

        Dim logLavMacchina As New cbl_LogImportazioni
        logLavMacchina.nomeFile = nomeFile
        lavMacchina.cbl_LogImportazioni.Add(logLavMacchina)

        If Not String.IsNullOrEmpty(messaggioErrore) Then
            Throw New Exception(messaggioErrore)
        End If

        Return lavMacchina

    End Function

    Private Function CaricaDataOraDaXML(nomeData As String,
                                        valoreData As String,
                                        nomeOra As String,
                                        valoreOra As String,
                                        ByRef messaggioErrore As String) As Date

        Dim data, dataOra As Date

        data = CaricaData(nomeData, valoreData, messaggioErrore)

        If Not String.IsNullOrEmpty(valoreOra) Then
            dataOra = CaricaDataOra(data, nomeOra, valoreOra, messaggioErrore)
        Else
            dataOra = data
        End If

        Return dataOra

    End Function

    Private Function CaricaData(nomeData As String,
                                valoreData As String,
                                ByRef messaggioErrore As String) As Date

        Dim data As Date
        Dim anno, mese, giorno As Integer
        Dim messaggio As String

        Try

            Select Case True

                Case String.IsNullOrEmpty(valoreData)
                    messaggio = String.Format("{0} non indicata", nomeData)
                    AccodaErrore(messaggio, messaggioErrore)

                Case Not IsNumeric(valoreData)
                    messaggio = String.Format("{0} non numerica: {1}", nomeData, valoreData)
                    AccodaErrore(messaggio, messaggioErrore)

                Case valoreData.Length = 8
                    anno = Mid(valoreData, 1, 4)
                    mese = Mid(valoreData, 5, 2)
                    giorno = Mid(valoreData, 7, 2)
                    data = New DateTime(anno, mese, giorno)

                Case Else
                    messaggio = String.Format("{0} con formato errato: {1}", nomeData, valoreData)
                    AccodaErrore(messaggio, messaggioErrore)

            End Select

        Catch ex As Exception

            messaggio = String.Format("{0} = {1} : {2}", nomeData, valoreData, ex.Message)
            AccodaErrore(messaggio, messaggioErrore)

        End Try

        Return data

    End Function

    Private Function CaricaDataOra(data As Date,
                                   nomeOra As String,
                                   valoreOra As String,
                                   ByRef messaggioErrore As String) As Date

        Dim dataInizializzata As Date
        Dim dataOra As Date
        Dim ore, min, sec As Integer
        Dim messaggio As String

        Try

            Select Case True
                Case data = dataInizializzata
                    messaggio = String.Format("{0} senza relativa data indicata", nomeOra)
                    AccodaErrore(messaggio, messaggioErrore)

                Case String.IsNullOrEmpty(valoreOra)
                    messaggio = String.Format("{0} non indicata", nomeOra)
                    AccodaErrore(messaggio, messaggioErrore)

                Case Not IsNumeric(valoreOra)
                    messaggio = String.Format("{0} non numerica: {1}", nomeOra, valoreOra)
                    AccodaErrore(messaggio, messaggioErrore)

                Case valoreOra.Length = 4
                    ore = Mid(valoreOra, 1, 2)
                    min = Mid(valoreOra, 3, 2)
                    sec = 0
                    dataOra = New DateTime(Year(data), Month(data), Day(data), ore, min, sec)

                Case valoreOra.Length = 6
                    ore = Mid(valoreOra, 1, 2)
                    min = Mid(valoreOra, 3, 2)
                    sec = Mid(valoreOra, 5, 2)
                    dataOra = New DateTime(Year(data), Month(data), Day(data), ore, min, sec)

                Case Else
                    messaggio = String.Format("{0} con formato errato: {1}", nomeOra, valoreOra)
                    AccodaErrore(messaggio, messaggioErrore)

            End Select

        Catch ex As Exception

            messaggio = String.Format("{0} = {1} : {2}", nomeOra, valoreOra, ex.Message)
            AccodaErrore(messaggio, messaggioErrore)

        End Try

        Return dataOra

    End Function

    Private Function CaricaDatoNumericoDaXML(nomeDatoNumerico As String,
                                             valoreDatoNumerico As String,
                                             riga As Integer,
                                             ByRef messaggioErrore As String) As Decimal

        Dim datoNumerico As Decimal = 0
        Dim messaggio As String

        Try

            Select Case True

                Case String.IsNullOrEmpty(valoreDatoNumerico)
                    datoNumerico = 0

                Case Not IsNumeric(valoreDatoNumerico)
                    messaggio = String.Format("{0} non numerico: {1}", nomeDatoNumerico, valoreDatoNumerico)
                    AccodaErroreRiga(riga, messaggio, messaggioErrore)

                Case Else
                    datoNumerico = valoreDatoNumerico

            End Select

        Catch ex As Exception

            messaggio = String.Format("{0} = {1} : {2}", nomeDatoNumerico, valoreDatoNumerico, ex.Message)
            AccodaErroreRiga(riga, messaggio, messaggioErrore)

        End Try

        Return datoNumerico

    End Function

    Private Function CaricaUnitaMisuraDaXML(unitaMisura As String,
                                            riga As Integer,
                                            ByRef messaggioErrore As String) As Integer

        Dim udmCod As Integer

        Try

            If IsNumeric(unitaMisura) Then

                udmCod = unitaMisura

            Else

                If [Enum].IsDefined(GetType(enum_UnitaMisura), unitaMisura) Then

                    udmCod = [Enum].Parse(GetType(enum_UnitaMisura), unitaMisura)

                Else

                    Throw New Exception("Unità di misura XML non riconosciuta: " & unitaMisura)

                End If

            End If

            _utilityLavMacch.Controlla_Unita_Misura(udmCod)

        Catch ex As Exception

            AccodaErroreRiga(riga, ex.Message, messaggioErrore)

        End Try

        Return udmCod

    End Function

    Private Sub AccodaErrore(messaggio As String, ByRef messaggioErrore As String)

        If messaggioErrore = String.Empty Then
            messaggioErrore = "Elenco errori XML:"
        End If
        messaggioErrore += vbCrLf & messaggio

    End Sub

    Private Sub AccodaErroreRiga(riga As Integer, messaggio As String, ByRef messaggioErrore As String)

        Dim messaggioRiga = String.Format("Riga {0} - {1}", riga, messaggio)

        AccodaErrore(messaggioRiga, messaggioErrore)

    End Sub

#Region "Classi private"
    Private Class xmlParser

        Private _logger As Logger

        Public Sub New(ByRef logger As Logger)
            _logger = logger
        End Sub

        Public Function LeggiLavorazioneStandard(percorsoAssoluto As String, parAggStandard As parametriAggStandard) As EsitoLavorazione

            Dim serializer As New XmlSerializer(GetType(EsitoLavorazione))
            Dim sr As New StreamReader(percorsoAssoluto)

            Dim lavorazioneStandard As New EsitoLavorazione()

            Using sr

                Try

                    If File.Exists(percorsoAssoluto) Then
                        lavorazioneStandard = serializer.Deserialize(sr)
                    Else
                        Throw New Exception("File inesistente: " & percorsoAssoluto)
                    End If

                Catch ex As Exception

                    Throw New Exception(ex.Message)

                End Try

            End Using

            If lavorazioneStandard.Versione < VersioneEsitoLavorazione.VersioneBase Then
                Throw New Exception("Versione XML errata: " & percorsoAssoluto)
            End If

            Return lavorazioneStandard

        End Function

    End Class

    Private Class parametriAggStandard : Inherits Utility_Lavorazioni_Macchina_Param_Agg

    End Class

#End Region

End Class
