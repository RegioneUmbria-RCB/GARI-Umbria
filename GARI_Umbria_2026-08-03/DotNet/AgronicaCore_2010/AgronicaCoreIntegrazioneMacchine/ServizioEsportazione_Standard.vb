Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Xml.Serialization
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class ServizioEsportazione_Standard

    Private _objParametri As ObjParametri
    Private _configurazioneServizio As Configurazione_Servizio
    Private _configurazioneImportatore As ConfigurazioneImportatore
    Private _logger As Logger
    Private _fileWriter As FileSystemHelper
    Private _parametriDinamici As Object
    Private Const DefaultFileEsportStandardPrimoIngresso As String = "InvioPrimoIngresso.xml"
    Private Const DefaultFileEsportStandardOrdine As String = "InvioOrdineLavorazione_[IDAGENDA].xml"

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

    Public Function AvviaEsportPrimoIngressoMacchinaStandard() As RispostaStandard
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
            EsportPrimoIngressoMacchinaStandard()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub EsportPrimoIngressoMacchinaStandard()

        Dim parametri = CaricaParametriDinamiciPrimoIngresso()

        _logger.InfoFormat("--- Inizio esportazione primo ingresso (IdAgenda={0})",
                           parametri.IdAgendaPrimoIngresso.ToString())

        Dim helper As New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)

        Dim lavorazione = LeggiLavorazione(helper, parametri)

        Dim eseguiEsportazione = SeControllaPrimoIngresso(parametri, lavorazione)

        If eseguiEsportazione Then

            _logger.Info("Inizio esportazione file primo ingresso lotto " & lavorazione.Lotto)

            'Gestione invio a lavorazione successiva

            If parametri.IdAgenda <> parametri.IdAgendaPrimoIngresso Then
                lavorazione = LeggiLavorazioneSuccessiva(helper, parametri)
            End If

            'Preparazione dati per invio XML

            If String.IsNullOrEmpty(lavorazione.Lotto) Then

                _logger.ErrorFormat("Lotto non impostato su lavorazione (IdAgenda={0})", parametri.IdAgenda.ToString())

            Else

                Dim parAggStandard = CaricaParametriAggiuntiviStandard(_configurazioneImportatore.ParametriAgg)

                Dim NomeFileEsport = parAggStandard.NomeFilePrimoIngresso

                Dim NomeFileAssEsportTMP = _fileWriter.OttieneNomeFileAssolutoEsportTMP(NomeFileEsport)

                Dim invioXML = CaricaInvioXML(lavorazione)

                ScriviFileXML(invioXML.GetType(), invioXML, NomeFileAssEsportTMP, NomeFileEsport)

            End If

        Else

            _logger.InfoFormat("File primo ingresso (Lotto: {0}) non inviato perchè gia fatto in precedenza", lavorazione.Lotto)

        End If

        _logger.Info("--- Fine esportazione primo ingresso")

    End Sub

    Private Function CaricaParametriDinamiciPrimoIngresso() As ParametriEsportatore

        Dim parametri As ParametriEsportatore = New ParametriEsportatore With
            {
            .Piva = _parametriDinamici.Piva,
            .SaCod = _parametriDinamici.SaCod,
            .IdAgenda = _parametriDinamici.IdAgenda,
            .LavCod = _parametriDinamici.LavCod,
            .IdAgendaPrimoIngresso = _parametriDinamici.IdAgendaPrimoIngresso,
            .controllaSePrimoIngresso = _parametriDinamici.controllaSePrimoIngresso
            }

        Return parametri

    End Function

    Private Function LeggiLavorazione(helper As LavorazioniHelper,
                                      parametri As ParametriEsportatore
                                      ) As Lavorazione

        Dim lavorazione As Lavorazione = Nothing

        lavorazione = helper.LeggiLavorazione(parametri.Piva, parametri.SaCod, parametri.IdAgendaPrimoIngresso)

        If IsNothing(lavorazione) Then
            _logger.ErrorFormat("Lavorazione non trovata (IdAgenda={0})",
                                parametri.IdAgendaPrimoIngresso.ToString())
        End If

        Return lavorazione

    End Function

    Private Function LeggiLavorazioneSuccessiva(helper As LavorazioniHelper,
                                                parametri As ParametriEsportatore
                                                ) As Lavorazione

        _logger.InfoFormat("Lettura lavorazione successiva (IdAgenda={0} LavCod={1})",
                           parametri.IdAgenda,
                           parametri.LavCod)

        Dim lavorazione As Lavorazione = Nothing

        lavorazione = helper.LeggiOrdineLavorazione(parametri.Piva, parametri.SaCod, parametri.IdAgenda, parametri.LavCod)

        Return lavorazione

    End Function

    Private Function SeControllaPrimoIngresso(parametri As ParametriEsportatore,
                                              lavorazione As Lavorazione
                                              ) As Boolean

        Dim eseguiEsportazione As Boolean = False

        If parametri.controllaSePrimoIngresso Then

            Dim movScarico = lavorazione.Movimenti.Find(Function(x) x.Cau_Mov = CAU_SCARICO)

            If IsNothing(movScarico) Then
                _logger.ErrorFormat("Non trovati movimenti scarico per lavorazione (IdAgenda={0})",
                                    parametri.IdAgenda.ToString())
            End If

            Dim pesate = lavorazione.Movimenti_Dettagli.Where(Function(md) md.Id_Mov.Equals(movScarico.Id_Mov))

            If IsNothing(pesate) OrElse Not pesate.Any Then
                _logger.ErrorFormat("Non trovati movimenti scarico per lavorazione (IdAgenda={0} IdMov={1})",
                                    parametri.IdAgenda.ToString(),
                                    movScarico.Id_Mov.ToString())
            End If

            If pesate.Count = 1 Then
                eseguiEsportazione = True
            End If

        Else

            eseguiEsportazione = True

        End If

        Return eseguiEsportazione

    End Function

    Private Function CaricaInvioXML(lavorazione As Lavorazione) As InvioPrimoIngresso

        Dim invioXML As New InvioPrimoIngresso

        invioXML.IdLavorazione = lavorazione.Lotto

        Return invioXML

    End Function

    Private Sub ScriviFileXML(tipoObj As Type,
                              objInvioXML As Object,
                              nomeFileAssolutoEsportTMP As String,
                              nomeFileEsportDefinitivo As String
                              )

        Try

            Dim serializer As New XmlSerializer(tipoObj)

            Using sw As New StreamWriter(nomeFileAssolutoEsportTMP)

                _logger.Info("Scrittura file XML: " & nomeFileAssolutoEsportTMP)

                serializer.Serialize(sw, objInvioXML)

            End Using

        Catch ex As Exception

            _logger.Error("Errore in scrittura file XML: " & ex.Message)

        End Try

        _fileWriter.SpostareFileDaTmpInArchivio(nomeFileAssolutoEsportTMP, nomeFileEsportDefinitivo)

        _logger.Info("Spostamento file in " & _fileWriter.GetPercorsoEsport())

    End Sub

    Public Function AvviaEsportOrdineMacchinaStandard() As RispostaStandard
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
            EsportOrdineMacchinaStandard()
        Catch ex As Exception
            _logger.Error(ex.Message)
        End Try
    End Sub

    Private Sub EsportOrdineMacchinaStandard()

        Dim parametri = CaricaParametriDinamiciOrdineLavorazione()

        _logger.InfoFormat("--- Inizio esportazione ordine lavorazione (IdAgenda={0})", parametri.IdAgenda.ToString())

        Dim lavorazione = LeggiOrdineLavorazione(parametri)

        _logger.Info("Inizio esportazione file ordine lotto " & lavorazione.Lotto)

        If String.IsNullOrEmpty(lavorazione.Lotto) Then

            _logger.ErrorFormat("Lotto non impostato su ordine (IdAgenda={0})", parametri.IdAgenda.ToString())

        Else

            Dim parAggStandard = CaricaParametriAggiuntiviStandard(_configurazioneImportatore.ParametriAgg)

            Dim NomeFileEsport = DeterminaFileEsportOrdine(parAggStandard, parametri, lavorazione)

            Dim NomeFileAssEsportTMP = _fileWriter.OttieneNomeFileAssolutoEsportTMP(NomeFileEsport)

            Dim invioOrdineXML = CaricaInvioOrdineXML(lavorazione)

            ScriviFileXML(invioOrdineXML.GetType(), invioOrdineXML, NomeFileAssEsportTMP, NomeFileEsport)

        End If

        _logger.Info("--- Fine esportazione ordine lavorazione")

    End Sub

    Private Function CaricaParametriDinamiciOrdineLavorazione()

        Dim parametri As ParametriEsportatore = New ParametriEsportatore With
            {
            .Piva = _parametriDinamici.Piva,
            .IdAgenda = _parametriDinamici.IdAgenda,
            .SaCod = _parametriDinamici.SaCod
            }

        Return parametri

    End Function

    Private Function LeggiOrdineLavorazione(parametri As ParametriEsportatore) As Lavorazione

        Dim helper As New LavorazioniHelper(_objParametri.Server, _objParametri.Utenti)

        Dim lavorazione As Lavorazione = Nothing

        lavorazione = helper.LeggiOrdine(parametri.Piva, parametri.SaCod, parametri.IdAgenda)

        If IsNothing(lavorazione) Then
            _logger.ErrorFormat("Ordine lavorazione non trovato (IdAgenda={0})", parametri.IdAgenda.ToString())
        End If

        Return lavorazione

    End Function

    Private Function CaricaInvioOrdineXML(lavorazione As Lavorazione) As InvioOrdineLavorazione

        Dim invioOrdineXML As New InvioOrdineLavorazione

        'Lotto
        invioOrdineXML.IdLavorazione = lavorazione.Lotto

        'Data
        Dim dataMov As Date = lavorazione.DataMovimento
        invioOrdineXML.Data = dataMov.ToString("yyyyMMdd")

        'Descrizione prodotto
        invioOrdineXML.DescrizioneProdotto = lavorazione.MatDes

        'Descrizione fornitore
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
        invioOrdineXML.DescrizioneFornitore = ragsocFornitore

        'Totale quantità prevista in ingresso
        Dim totQtaScarico As Decimal = (From movDett In lavorazione.Movimenti_Dettagli
                                        Where movDett.Cau_Mov = enum_Agenda_Causali.SCARICO And movDett.Udm_Cod = enum_UnitaMisura.KG
                                        Select movDett.Qta).Sum
        invioOrdineXML.TotaleQtaIngresso = Math.Round(totQtaScarico, 1).ToString()

        Return invioOrdineXML

    End Function

#Region "Classi private"

    Protected Class ParametriEsportatore

        Public Piva As String
        Public SaCod As Integer
        Public IdAgenda As Integer
        Public LavCod As String
        Public IdAgendaPrimoIngresso As Integer
        Public controllaSePrimoIngresso As Boolean = False

    End Class

    Private Function CaricaParametriAggiuntiviStandard(parametriAggServizio As String) As parametriAggStandard
        Dim parAggStandard = New parametriAggStandard
        If Not String.IsNullOrEmpty(parametriAggServizio) Then
            parAggStandard = JsonConvert.DeserializeObject(Of parametriAggStandard)(parametriAggServizio)
        End If
        If String.IsNullOrEmpty(parAggStandard.NomeFilePrimoIngresso) Then
            parAggStandard.NomeFilePrimoIngresso = DefaultFileEsportStandardPrimoIngresso
        End If
        If String.IsNullOrEmpty(parAggStandard.NomeFileOrdine) Then
            parAggStandard.NomeFileOrdine = DefaultFileEsportStandardOrdine
        End If
        Return parAggStandard
    End Function

    Private Function DeterminaFileEsportOrdine(parAggStandard As parametriAggStandard,
                                               parametri As ParametriEsportatore,
                                               lavorazione As Lavorazione) As String

        Dim nomeFileOrdine = parAggStandard.NomeFileOrdine

        If nomeFileOrdine.Contains(placeholderIdAgenda) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderIdAgenda, Trim(parametri.IdAgenda.ToString()))
        End If

        If nomeFileOrdine.Contains(placeholderLotto) Then
            nomeFileOrdine = nomeFileOrdine.Replace(placeholderLotto, Trim(lavorazione.Lotto))
        End If

        Return nomeFileOrdine

    End Function

    Private Class parametriAggStandard
        Public NomeFilePrimoIngresso As String
        Public NomeFileOrdine As String
    End Class

#End Region

End Class

