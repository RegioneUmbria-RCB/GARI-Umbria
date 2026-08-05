Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports System.Globalization
Imports Newtonsoft.Json
Imports System.Text

Public Class ServizioEsportazione_Malavasi

    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _fileWriter As FileSystemHelper
    Private _parametriDinamici As Object
    Private Const DefaultFileEsportMalavasiPrimoIngresso As String = "Load.dat"
    Private Const DefaultFileEsportMalavasiOrdine As String = "[IDAGENDA].txt"
    'PlaceHolder gestiti per il file esportazione ordini
    Private Const placeholderIdAgenda As String = "[IDAGENDA]"
    Private Const placeholderLotto As String = "[LOTTO]"

    Public Sub New(configurazione_Servizio As Configurazione_Servizio,
                   configurazioneImportatore As ConfigurazioneImportatore,
                   objParametri As ObjParametri,
                   ByVal parametriDinamici As Object)

        _configurazioneServizio = configurazione_Servizio
        _configurazioneImportatore = configurazioneImportatore
        _objParametri = objParametri
        _logger = New Logger(_configurazioneServizio, _configurazioneImportatore, _objParametri)
        _fileWriter = New FileSystemHelper(configurazione_Servizio, _configurazioneImportatore)
        _parametriDinamici = parametriDinamici

    End Sub

    Public Function AvviaEsportPrimoIngressoCalibratriceMalavasi() As RispostaStandard
        Dim r As New RispostaStandard()
        Try

            TentareEsportazionePrimoIngresso()
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

    Private Sub TentareEsportazionePrimoIngresso()
        Try
            _fileWriter.CrearePercorsiEsport()
            EsportPrimoIngressoCalibratriceMalavasi()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub EsportPrimoIngressoCalibratriceMalavasi()

        Dim lavorazione As Lavorazione = Nothing
        ' TODO Verificare flag Sottoscorta
        Dim helper As New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)

        Dim parametri As ParametriEsportatore = New ParametriEsportatore With
                                                    {
                                                        .Piva = _parametriDinamici.Piva,
                                                        .IdAgenda = _parametriDinamici.IdAgenda,
                                                        .SaCod = _parametriDinamici.SaCod,
                                                        .controllaSePrimoIngresso = _parametriDinamici.controllaSePrimoIngresso
                                                    }

        _logger.InfoFormat("--- Inizio esportazione primo ingresso (IdAgenda={0})", parametri.IdAgenda.ToString())

        lavorazione = helper.LeggiLavorazione(parametri.Piva, parametri.SaCod, parametri.IdAgenda)

        If IsNothing(lavorazione) Then
            _logger.Error("Lavorazione non trovata con ID " & parametri.IdAgenda.ToString())
        End If

        Dim eseguiEsportazione As Boolean = False

        If parametri.controllaSePrimoIngresso Then
            Dim movScarico = lavorazione.Movimenti.Find(Function(x) x.Cau_Mov = CAU_SCARICO)
            If IsNothing(movScarico) Then
                _logger.Error("Non trovati movimenti scarico per lavorazione con ID " & parametri.IdAgenda.ToString())
            End If
            Dim pesate = lavorazione.Movimenti_Dettagli.Where(Function(md) md.Id_Mov.Equals(movScarico.Id_Mov))
            If IsNothing(pesate) OrElse Not pesate.Any Then
                _logger.Error("Non trovati movimenti scarico per lavorazione (IdAgenda=" & parametri.IdAgenda.ToString() _
                                                                          & " IdMov=" & movScarico.Id_Mov.ToString() & ")")
            End If
            If pesate.Count = 1 Then
                eseguiEsportazione = True
            End If
        Else
            eseguiEsportazione = True
        End If

        If eseguiEsportazione Then

            Dim parAggMalavasi = CaricaParametriAggiuntiviMalavasi(_configurazioneImportatore.ParametriAgg)

            _logger.Info("Inizio esportazione file primo ingresso lotto " & lavorazione.Lotto)
            If String.IsNullOrEmpty(lavorazione.Lotto) Then
                _logger.Error("Lotto non impostato sulla lavorazione con ID " & lavorazione.IdAgenda.ToString())
            Else
                Dim NomeFileEsportTMP = _fileWriter.OttieneNomeFileAssolutoEsportTMP(parAggMalavasi.NomeFilePrimoIngresso)
                ' Scrittura riga file
                _logger.Info("Scrittura file " & NomeFileEsportTMP)
                Dim errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, lavorazione.Lotto, False, Encoding.UTF8)
                ' Spostamento finale file
                If String.IsNullOrEmpty(errore) Then
                    _fileWriter.SpostareFileDaTmpInArchivio(NomeFileEsportTMP, parAggMalavasi.NomeFilePrimoIngresso)
                    _logger.Info("Spostamento file in " & _fileWriter.GetPercorsoEsport())
                Else
                    _logger.Error("Errore in scrittura file: " & errore)
                End If
            End If

        Else

            ' Non devo produrre il file perchè non è la prima pesata
            _logger.InfoFormat("File relativo al primo ingresso lotto (LOTTO {0}) non prodotto perchè gia prodotto in precedenza", lavorazione.Lotto)

        End If

        _logger.Info("--- Fine esportazione primo ingresso")

    End Sub

    Public Function AvviaEsportOrdineCalibratriceMalavasi() As RispostaStandard
        Dim r As New RispostaStandard()
        Try
            TentareEsportazioneOrdine()
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

    Private Sub TentareEsportazioneOrdine()
        Try
            _fileWriter.CrearePercorsiEsport()
            EsportOrdineCalibratriceMalavasi()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub EsportOrdineCalibratriceMalavasi()

        Dim lavorazione As Lavorazione = Nothing
        ' TODO Verificare flag Sottoscorta
        Dim helper As New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)

        Dim parametri As ParametriEsportatore = New ParametriEsportatore With
                                                    {
                                                        .Piva = _parametriDinamici.Piva,
                                                        .IdAgenda = _parametriDinamici.IdAgenda,
                                                        .SaCod = _parametriDinamici.SaCod
                                                    }

        lavorazione = helper.LeggiOrdine(parametri.Piva, parametri.SaCod, parametri.IdAgenda)

        If IsNothing(lavorazione) Then
            _logger.Error("Ordine lavorazione non trovato con ID " & parametri.IdAgenda.ToString())
        End If

        _logger.Info(" Inizio esportazione file ordine lotto " & lavorazione.Lotto)
        If String.IsNullOrEmpty(lavorazione.Lotto) Then
            _logger.Error("Lotto non impostato su ordine con ID " & parametri.IdAgenda.ToString())
        Else
            Dim rigaFile As String
            Dim errore As String

            Dim parAggMalavasi = CaricaParametriAggiuntiviMalavasi(_configurazioneImportatore.ParametriAgg)
            Dim FileEsportOrdineMalavasi = DeterminaFileEsportOrdineMalavasi(parAggMalavasi, parametri, lavorazione)

            Dim NomeFileEsportTMP = _fileWriter.OttieneNomeFileAssolutoEsportTMP(FileEsportOrdineMalavasi)
            _logger.Info(" Scrittura file " & NomeFileEsportTMP)
            '--- 1^ riga file: Data
            Dim dataMov As DateTime = lavorazione.DataMovimento
            Dim giornoCreazione = dataMov.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo)
            rigaFile = "LotCreationDay = " & giornoCreazione
            errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, False, Encoding.UTF8)
            '--- 2^ riga file: Lotto
            If String.IsNullOrEmpty(errore) Then
                rigaFile = "LotCustomer = " & lavorazione.Lotto
                errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, True, Encoding.UTF8)
            End If
            '--- 3^ riga file: Descrizione
            If String.IsNullOrEmpty(errore) Then
                rigaFile = "Description = " & lavorazione.MatDes
                errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, True, Encoding.UTF8)
            End If
            '--- 4^ riga file: Fornitore
            If String.IsNullOrEmpty(errore) Then
                Dim idFornitore As Integer = (From mpc In lavorazione.MateriePrimeCampionature
                                              Where mpc.Tipo = ParametriQualitativi_Fornitore
                                              Select mpc.Tipo_Cod).Min
                Dim ragsocFornitore As String = String.Empty
                If IsNumeric(idFornitore) Then
                    Dim objLeggiRisorseUmane As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                    Dim dtLeggiRisUm = objLeggiRisorseUmane.Leggi3(lavorazione.Piva,
                                                                   Cod_Contatto:=String.Empty,
                                                                   idFornitore,
                                                                   Cod_Rapporto:=0,
                                                                   xFiltroAggiuntivo:=String.Empty,
                                                                   xOrderBy:=String.Empty,
                                                                   _objParametri.Server)
                    ragsocFornitore = dtLeggiRisUm.Rows(0).Item("rag_soc")
                End If
                rigaFile = "Supplier = " & ragsocFornitore
                errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, True, Encoding.UTF8)
            End If
            '--- 5^ riga file: Note
            If String.IsNullOrEmpty(errore) Then
                rigaFile = "Note = "
                errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, True, Encoding.UTF8)
            End If
            '--- 6^ riga file: Quantità
            If String.IsNullOrEmpty(errore) Then
                Dim totQtaScarico As Decimal = (From movDett In lavorazione.Movimenti_Dettagli
                                                Where movDett.Cau_Mov = enum_Agenda_Causali.SCARICO And movDett.Udm_Cod = enum_UnitaMisura.KG
                                                Select movDett.Qta).Sum

                rigaFile = "QuantityKG = " & Math.Round(totQtaScarico, 1).ToString()
                errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, rigaFile, True, Encoding.UTF8)
            End If
            ' Spostamento finale file
            If String.IsNullOrEmpty(errore) Then
                _fileWriter.SpostareFileDaTmpInArchivio(NomeFileEsportTMP, FileEsportOrdineMalavasi)
                _logger.Info("Spostamento file in " & _fileWriter.GetPercorsoEsport())
            Else
                _logger.Error("Errore in scrittura file: " & errore)
            End If
        End If
    End Sub

#Region "Classi private"

    Protected Class ParametriEsportatore

        Public Piva As String
        Public IdAgenda As Integer
        Public SaCod As Integer
        Public controllaSePrimoIngresso As Boolean = False

    End Class

    Private Function CaricaParametriAggiuntiviMalavasi(parametriAggServizio As String) As parametriAggMalavasi
        Dim parAggMalavasi = New parametriAggMalavasi
        If Not String.IsNullOrEmpty(parametriAggServizio) Then
            parAggMalavasi = JsonConvert.DeserializeObject(Of parametriAggMalavasi)(parametriAggServizio)
        End If
        If String.IsNullOrEmpty(parAggMalavasi.NomeFilePrimoIngresso) Then
            parAggMalavasi.NomeFilePrimoIngresso = DefaultFileEsportMalavasiPrimoIngresso
        End If
        If String.IsNullOrEmpty(parAggMalavasi.NomeFileOrdine) Then
            parAggMalavasi.NomeFileOrdine = DefaultFileEsportMalavasiOrdine
        End If
        Return parAggMalavasi
    End Function

    Private Function DeterminaFileEsportOrdineMalavasi(parAggMalavasi As parametriAggMalavasi,
                                                       parametri As ParametriEsportatore,
                                                       lavorazione As Lavorazione)
        Dim nomeFileOrdine = parAggMalavasi.NomeFileOrdine

        If nomeFileOrdine.Contains(placeholderIdAgenda) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderIdAgenda, Trim(parametri.IdAgenda.ToString()))
        End If

        If nomeFileOrdine.Contains(placeholderLotto) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderLotto, Trim(lavorazione.Lotto))
        End If

        Return nomeFileOrdine
    End Function

    Private Class parametriAggMalavasi
        Public NomeFilePrimoIngresso As String
        Public NomeFileOrdine As String
    End Class

#End Region

End Class

