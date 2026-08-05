Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreAcciseDAL
Imports AgronicaCoreAcciseCommon

Public Class DAA_GaranzieCircolazione
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument
    Private _logErrori As String = ""
    Private _catCod As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _nomeFileReport As String = "Rpt_DAA_GaranzieCircolanti.rpt"
    Private _pathRpt As String = "~/GestioneStampe/RegistriPreparazioni/DAA/Report"
    Private _nomeDocumento As String

    Private _piva As String
    Private _dataDal As String
    Private _dataAl As String
    Private _dataRiferimento As String
    Private _numProtocollo As String
    Private _ufficioDogane As String

#Region " DAA "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()
        'istanzio l'oggetto report


    End Sub

#End Region


    '##################################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        _dataDal = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)
        _dataAl = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)
        _dataRiferimento = Stringa_Decodifica(CStr(Request.QueryString("ds")), AgroKey_EncoderDecoder, Server)
        _numProtocollo = Stringa_Decodifica(CStr(Request.QueryString("np")), AgroKey_EncoderDecoder, Server)
        _ufficioDogane = Stringa_Decodifica(CStr(Request.QueryString("ud")), AgroKey_EncoderDecoder, Server)

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        _nomeDocumento = "DAARiepilogoGaranzieCircolazione"

        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################
        Dim crHlp As New CrystalHelper

        'lettura da file system
        If _nomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath(_pathRpt)
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & _nomeFileReport) Then
                _rptStampa = crHlp.getReportDaFile(pFile, _nomeFileReport)
            End If

        End If


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not IsPostBack Then

            Dim dsDAA_GaCir As New DS_DAA_GaCir
            Dim identificazioneDocumento As String = _nomeDocumento + "_" & DateTime.Now().ToString("yyyy_MM_dd_hhmmssff")
            Dim dataManager = New DataManager(_objParametriServer, Nothing, Nothing, False)


            Try

                ' data load
                Dim drDettaglioNew As DS_DAA_GaCir.DT_DettagliRow
                Dim drIntestazioneNew As DS_DAA_GaCir.DT_IntestazioneRow

                Dim dataDa As DateTime = If(String.IsNullOrEmpty(_dataDal), AGRODATAINIZIO, Convert.ToDateTime(_dataDal))
                Dim dataA As DateTime = If(String.IsNullOrEmpty(_dataAl), AGRODATAFINE, Convert.ToDateTime(_dataAl))

                Dim garanzie = dataManager.Stampe_LeggiGaranzieCircolazione(_piva, dataDa, dataA)
                Dim intestazione = dataManager.Stampe_Intestazione(_piva)

                If Not intestazione Is Nothing Then
                    drIntestazioneNew = dsDAA_GaCir.DT_Intestazione.NewDT_IntestazioneRow()
                    drIntestazioneNew.PIVA = intestazione.PIVA
                    drIntestazioneNew.RagioneSociale = intestazione.RagioneSociale
                    drIntestazioneNew.Indirizzo = intestazione.Indirizzo
                    drIntestazioneNew.Frazione = intestazione.Frazione
                    drIntestazioneNew.CAP = intestazione.CAP
                    drIntestazioneNew.Comune = intestazione.Comune
                    drIntestazioneNew.Provincia = intestazione.Provincia
                    drIntestazioneNew.Stato = intestazione.Stato
                    drIntestazioneNew.CodiceAccisa = intestazione.CodiceAccisa

                    dsDAA_GaCir.DT_Intestazione.Rows.Add(drIntestazioneNew)

                End If

                For Each g As GaranziaCircolazione In garanzie
                    drDettaglioNew = dsDAA_GaCir.DT_Dettagli.NewDT_DettagliRow()
                    drDettaglioNew.Importo_Impegnato = g.Importo_Impegnato
                    drDettaglioNew.Importo_Svincolato = g.Importo_Svincolato
                    drDettaglioNew.Tipo = g.Tipo
                    drDettaglioNew.RiportoFinePeriodo = g.RiportoFinePeriodo
                    drDettaglioNew.RiportoPeriodoPrecedente = g.RiportoPeriodoPrecedente
                    drDettaglioNew.SaldoProgressivo = g.SaldoProgressivo
                    drDettaglioNew.data_emis_documento_AAAAMMGG = g.data_emis_documento_AAAAMMGG
                    drDettaglioNew.ImportoIntegrato = g.ImportoIntegrato
                    drDettaglioNew.ImportoScaduto = g.ImportoScaduto

                    dsDAA_GaCir.DT_Dettagli.Rows.Add(drDettaglioNew)

                Next

                ' impostazione datasource
                _rptStampa.SetDataSource(dsDAA_GaCir)

                'impostazione parametri
                _rptStampa.SetParameterValue("DataStampa", If(String.IsNullOrEmpty(_dataRiferimento), AGRODATAINIZIO, Convert.ToDateTime(_dataRiferimento)))
                _rptStampa.SetParameterValue("Protocollo", _numProtocollo)
                _rptStampa.SetParameterValue("UfficioDogane", _ufficioDogane)

                'MS La nuova gestione salva il report con i dati su disco con un nome univoco per ogni esecuzione di stampa e passa a
                'VisualizzatoreReport il nome del pathname da riaprire.
                'Questi files temporanei sono cancellati periodicamente in entrata su GestioneRichieste e non in VisualizzatoreReport perché
                'possono essere eseguite una serie di PostBack es. export su pdf e il report deve rimanere disponibile.
                Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
                Try
                    _rptStampa.SaveAs(reportTemporaneo, True)
                Catch ex As Exception
                    ' todo
                End Try

                Try
                    'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
                    'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.
                    dsDAA_GaCir.Dispose()
                    dsDAA_GaCir = Nothing

                    _rptStampa.Close()
                    _rptStampa.Dispose()
                    _rptStampa = Nothing

                    GC.Collect()

                    'MS Il controllo passa a VisualizzatoreReport passando in QueryString il pathname del report temporaneo su disco in modo che 
                    'rimanga disponibile anche nelle PostBack per esportazione ecc.. 
                    'Non si può passare sulla session altrimenti l'esecuzione di 2 o più report dalla stessa postazione andrebbe in conflitto
                    'Per discriminare fra vecchio e nuovo giro il visualizzatore testa se in QueryString viene passato tmpReportPath

                    Dim urlRedirect = "..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                      "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server)

                    Response.Redirect(urlRedirect)

                Catch ex As Exception

                End Try


            Catch ex As Exception

            End Try
        End If


    End Sub

    Private Sub SalvaLogErrori(logErrori As String, nomeDocumento As String, identificazioneDocumento As String)
        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim nomeFile As String

        If logErrori <> "" Then

            logErrori = nomeDocumento & vbCrLf & vbCrLf & logErrori

            nomeFile = "Log_Errori_" & identificazioneDocumento & CStr(Session("ASG_Utente_Username"))

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(_objParametriServer, _
                                             "Stampe_FreshAndFood", _
                                             nomeFile & ".txt", _
                                             Session("ASG_Utente_Username"), _
                                             "RiepilogoLiquidazioneSoci.aspx", _
                                             logErrori)
        End If

    End Sub

End Class
