Imports AgronicaCoreDataProvider
Imports AgronicaCoreModello
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports Newtonsoft.Json
Imports System.Text

Public Class ServizioEsportazione_RsService_Ulma

    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _fileWriter As FileSystemHelper
    Private _parametriDinamici As Object
    Private Const DefaultFileEsportRSServiceUlmaPrimoIngresso As String = "CodiceProduzione.txt"
    Private Const DefaultFileEsportRSServiceUlmaOrdine As String = "[IDAGENDA].txt"
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

    Public Function AvviaEsportPrimoIngressoConfezionatrice() As RispostaStandard
        Dim r As New RispostaStandard()
        Try

            EsportazionePrimoIngresso()
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

    Private Sub EsportazionePrimoIngresso()
        Try
            _fileWriter.CrearePercorsiEsport()
            EsportPrimoIngressoConfezionatrice()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub EsportPrimoIngressoConfezionatrice()

        Dim lavorazione As Lavorazione = Nothing
        Dim helper As New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)

        Dim parametri As ParametriEsportatore = New ParametriEsportatore With
                                                    {
                                                        .Piva = _parametriDinamici.Piva,
                                                        .SaCod = _parametriDinamici.SaCod,
                                                        .IdAgenda = _parametriDinamici.IdAgenda,
                                                        .LavCod = _parametriDinamici.LavCod,
                                                        .IdAgendaPrimoIngresso = _parametriDinamici.IdAgendaPrimoIngresso,
                                                        .controllaSePrimoIngresso = _parametriDinamici.controllaSePrimoIngresso
                                                    }

        _logger.InfoFormat("--- Inizio esportazione primo ingresso (IdAgenda={0})", parametri.IdAgendaPrimoIngresso.ToString())

        lavorazione = helper.LeggiLavorazione(parametri.Piva, parametri.SaCod, parametri.IdAgendaPrimoIngresso)

        If IsNothing(lavorazione) Then
            _logger.Error("Lavorazione non trovata con ID " & parametri.IdAgendaPrimoIngresso.ToString())
        End If

        Dim eseguiEsportazione As Boolean = False

        If parametri.controllaSePrimoIngresso Then
            Dim movScarico = lavorazione.Movimenti.Find(Function(x) x.Cau_Mov = CAU_SCARICO)
            If IsNothing(movScarico) Then
                _logger.Error("Non trovati movimenti scarico per lavorazione con ID " & parametri.IdAgendaPrimoIngresso.ToString())
            End If

            Dim pesate = lavorazione.Movimenti_Dettagli.Where(Function(md) md.Id_Mov.Equals(movScarico.Id_Mov))
            If IsNothing(pesate) OrElse Not pesate.Any Then
                _logger.Error("Non trovati movimenti scarico per lavorazione (IdAgenda=" & parametri.IdAgendaPrimoIngresso.ToString() _
                                                                          & " IdMov=" & movScarico.Id_Mov.ToString() & ")")
            End If
            If pesate.Count = 1 Then
                eseguiEsportazione = True
            End If
        Else
            eseguiEsportazione = True
        End If

        If eseguiEsportazione Then

            If parametri.IdAgenda <> parametri.IdAgendaPrimoIngresso Then
                lavorazione = Nothing
                lavorazione = helper.LeggiOrdineLavorazione(parametri.Piva, parametri.SaCod, parametri.IdAgenda, parametri.LavCod)
            End If

            Dim parAggRSService = CaricaParametriAggiuntivi(_configurazioneImportatore.ParametriAgg)

            _logger.Info("Inizio esportazione file primo ingresso lotto " & lavorazione.Lotto)
            If String.IsNullOrEmpty(lavorazione.Lotto) Then
                _logger.Error("Lotto non impostato sulla lavorazione con ID " & lavorazione.IdAgenda.ToString())
            Else
                Dim NomeFileEsportTMP = _fileWriter.OttieneNomeFileAssolutoEsportTMP(parAggRSService.NomeFilePrimoIngresso)
                ' Scrittura riga file
                _logger.Info("Scrittura file " & NomeFileEsportTMP)
                Dim errore = _fileWriter.ScriviRigaFile(NomeFileEsportTMP, lavorazione.Lotto, False, Encoding.Unicode)
                ' Spostamento finale file
                If String.IsNullOrEmpty(errore) Then
                    _fileWriter.SpostareFileDaTmpInArchivio(NomeFileEsportTMP, parAggRSService.NomeFilePrimoIngresso)
                    _logger.Info("Spostamento file in " & _fileWriter.GetPercorsoEsport())
                Else
                    _logger.Error("Errore in scrittura file: " & errore)
                End If
            End If

        Else

            ' Non devo produrre il file perchè non è la prima pesata
            _logger.InfoFormat("File relativo al primo ingresso lotto (LOTTO {0}) non prodotto perchè gia prodotto in precedenza ", lavorazione.Lotto)

        End If

        _logger.Info("--- Fine esportazione primo ingresso")

    End Sub

#Region "Classi private"

    Protected Class ParametriEsportatore

        Public Piva As String
        Public SaCod As Integer
        Public IdAgenda As Integer
        Public LavCod As String
        Public IdAgendaPrimoIngresso As Integer
        Public controllaSePrimoIngresso As Boolean

    End Class

    Private Function CaricaParametriAggiuntivi(parametriAggServizio As String) As parametriAggRSService_Ulma
        Dim parAgg = New parametriAggRSService_Ulma
        If Not String.IsNullOrEmpty(parametriAggServizio) Then
            parAgg = JsonConvert.DeserializeObject(Of parametriAggRSService_Ulma)(parametriAggServizio)
        End If
        If String.IsNullOrEmpty(parAgg.NomeFilePrimoIngresso) Then
            parAgg.NomeFilePrimoIngresso = DefaultFileEsportRSServiceUlmaPrimoIngresso
        End If
        If String.IsNullOrEmpty(parAgg.NomeFileOrdine) Then
            parAgg.NomeFileOrdine = DefaultFileEsportRSServiceUlmaOrdine
        End If
        Return parAgg
    End Function

    Private Function DeterminaFileEsportOrdine(parAgg As parametriAggRSService_Ulma,
                                                       parametri As ParametriEsportatore,
                                                       lavorazione As Lavorazione)
        Dim nomeFileOrdine = parAgg.NomeFileOrdine

        If nomeFileOrdine.Contains(placeholderIdAgenda) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderIdAgenda, Trim(parametri.IdAgenda.ToString()))
        End If

        If nomeFileOrdine.Contains(placeholderLotto) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderLotto, Trim(lavorazione.Lotto))
        End If

        Return nomeFileOrdine
    End Function

    Private Class parametriAggRSService_Ulma
        Public NomeFilePrimoIngresso As String
        Public NomeFileOrdine As String
    End Class

#End Region

End Class

