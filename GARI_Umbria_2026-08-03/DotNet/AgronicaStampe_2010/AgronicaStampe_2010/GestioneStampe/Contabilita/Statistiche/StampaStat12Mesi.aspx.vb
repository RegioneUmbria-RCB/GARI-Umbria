Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports CrystalDecisions.CrystalReports
Imports AgronicaCoreContabDAL
Imports AgronicaStampe_2010.DS_Stat_12_Mesi
Imports System.Collections.Generic
Imports System.Globalization
Imports CrystalDecisions.CrystalReports.Engine

Public Class StampaStat12Mesi
    Inherits System.Web.UI.Page

    Private _rptStampa As Engine.ReportDocument
    Private _logErrori As String = ""
    Private _catCod As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtente As New AgronicaCoreDataProvider.AgronicaCoreParametri

   
    Private _pathRpt As String = "~/GestioneStampe/Contabilita/Statistiche/Report"

    Private _nomeDocumento As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        Dim objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim objParametriUtente As New AgronicaCoreDataProvider.AgronicaCoreParametri

        Dim piva As String = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)
        Dim numSin As String = Stringa_Decodifica(CStr(Request.QueryString("numSin")), AgroKey_EncoderDecoder, Server)
        Dim num As String = Stringa_Decodifica(CStr(Request.QueryString("num")), AgroKey_EncoderDecoder, Server)
        Dim numDes As String = Stringa_Decodifica(CStr(Request.QueryString("numoDes")), AgroKey_EncoderDecoder, Server)
        Dim nrRiga As String = Stringa_Decodifica(CStr(Request.QueryString("nrRiga")), AgroKey_EncoderDecoder, Server)
        Dim dataDal As String = Stringa_Decodifica(CStr(Request.QueryString("dataDal")), AgroKey_EncoderDecoder, Server)
        Dim dataAl As String = Stringa_Decodifica(CStr(Request.QueryString("dataAl")), AgroKey_EncoderDecoder, Server)

        Dim clienti As String = Stringa_Decodifica(CStr(Request.QueryString("cli")), AgroKey_EncoderDecoder, Server)
        Dim agenti As String = Stringa_Decodifica(CStr(Request.QueryString("age")), AgroKey_EncoderDecoder, Server)
        Dim causali As String = Stringa_Decodifica(CStr(Request.QueryString("causali")), AgroKey_EncoderDecoder, Server)
        Dim specie As String = Stringa_Decodifica(CStr(Request.QueryString("spe")), AgroKey_EncoderDecoder, Server)
        Dim varieta As String = Stringa_Decodifica(CStr(Request.QueryString("var")), AgroKey_EncoderDecoder, Server)
        Dim prodotti As String = Stringa_Decodifica(CStr(Request.QueryString("prod")), AgroKey_EncoderDecoder, Server)
        Dim categorie As String = Stringa_Decodifica(CStr(Request.QueryString("categ")), AgroKey_EncoderDecoder, Server)
        Dim categorieCommerciali As String = Stringa_Decodifica(CStr(Request.QueryString("categcommerciali")), AgroKey_EncoderDecoder, Server)
        Dim cauTrasp As String = Stringa_Decodifica(CStr(Request.QueryString("cau_trasp")), AgroKey_EncoderDecoder, Server)
        Dim tipoReport As String = Stringa_Decodifica(CStr(Request.QueryString("tipo_report")), AgroKey_EncoderDecoder, Server)
        Dim titolo As String = Stringa_Decodifica(CStr(Request.QueryString("titolo")), AgroKey_EncoderDecoder, Server)
        Dim livelli As String = Stringa_Decodifica(CStr(Request.QueryString("liv")), AgroKey_EncoderDecoder, Server)
        Dim daMese As String = Stringa_Decodifica(CStr(Request.QueryString("da_mese")), AgroKey_EncoderDecoder, Server)
        Dim daAnno As String = Stringa_Decodifica(CStr(Request.QueryString("da_anno")), AgroKey_EncoderDecoder, Server)
        Dim aMese As String = Stringa_Decodifica(CStr(Request.QueryString("a_mese")), AgroKey_EncoderDecoder, Server)
        Dim aAnno As String = Stringa_Decodifica(CStr(Request.QueryString("a_anno")), AgroKey_EncoderDecoder, Server)
        Dim confrAnno As String = Stringa_Decodifica(CStr(Request.QueryString("confr_anno")), AgroKey_EncoderDecoder, Server)
        Dim tipoValore As String = Stringa_Decodifica(CStr(Request.QueryString("tipo_val")), AgroKey_EncoderDecoder, Server)
        Dim scostamento As String = Stringa_Decodifica(CStr(Request.QueryString("scost_totale")), AgroKey_EncoderDecoder, Server)
        Dim saltoPagina1Liv As String = Stringa_Decodifica(CStr(Request.QueryString("saltopag_1liv")), AgroKey_EncoderDecoder, Server)
        Dim meseScost As String = Stringa_Decodifica(CStr(Request.QueryString("mese_prev_scost")), AgroKey_EncoderDecoder, Server)
        Dim report As String = Stringa_Decodifica(CStr(Request.QueryString("analisi")), AgroKey_EncoderDecoder, Server)
        Dim ordinaXValore = Stringa_Decodifica(CStr(Request.QueryString("cb_ordin_x_valore")), AgroKey_EncoderDecoder, Server)
        Dim rapportiContabili = Stringa_Decodifica(CStr(Request.QueryString("rappCont")), AgroKey_EncoderDecoder, Server)
        Dim decimali_qta = Stringa_Decodifica(CStr(Request.QueryString("decimali_qta")), AgroKey_EncoderDecoder, Server)
        Dim corrispettivi As String = Stringa_Decodifica(CStr(Request.QueryString("includi_corr")), AgroKey_EncoderDecoder, Server)

        Dim includi_Corrispettivi As Boolean = True
        If Not String.IsNullOrEmpty(corrispettivi) Then
            Boolean.TryParse(corrispettivi, includi_Corrispettivi)
        End If

        Dim nazioniFatturazione = String.Empty
        If Not Session("FiltroStatistiche_Nazioni_Fatturazione") Is Nothing Then
            nazioniFatturazione = Stringa_Decodifica(CStr(Session("FiltroStatistiche_Nazioni_Fatturazione")), AgroKey_EncoderDecoder, Server)
            Session("FiltroStatistiche_Nazioni_Fatturazione") = Nothing
        End If

        Dim tipoValore2 As String = Stringa_Decodifica(CStr(Request.QueryString("tipo_val_2")), AgroKey_EncoderDecoder, Server)
        Dim boolTester As Boolean

        objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametriUtente = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        _nomeDocumento = "TODO"

        
        Dim nomeFileReport As String = String.Empty
        Dim crHlp As New CrystalHelper
        Dim stampa_controller = StatisticaControllerBase.ControllerFactory(piva, numSin, num, numDes,
                                                    nrRiga, dataDal, dataAl, clienti,
                                                    agenti, causali, specie, varieta, prodotti, categorie,
                                                    categorieCommerciali, cauTrasp, tipoReport, titolo, livelli, daMese,
                                                    daAnno, aMese, aAnno, confrAnno, tipoValore, scostamento,
                                                    saltoPagina1Liv, meseScost, report, ordinaXValore,
                                                    rapportiContabili, nazioniFatturazione, tipoValore2, decimali_qta, includi_Corrispettivi,
                                                    objParametriServer, objParametriUtente)

        '#################################################################################
        '#####  Carico il report da File (no Risorsa incorporata, ma Contenuto)
        '#################################################################################


        nomeFileReport = stampa_controller.Dammi_Nome_File_Report()

        'lettura da file system
        If nomeFileReport <> "" Then

            Dim pFile As String = HttpContext.Current.Server.MapPath(_pathRpt)
            pFile = AgronicaCoreUtility.FileSystemHelper.AggiungiSlashSeNonEsiste(pFile)

            If My.Computer.FileSystem.FileExists(pFile & nomeFileReport) Then
                _rptStampa = crHlp.getReportDaFile(pFile, nomeFileReport)
            End If

        End If


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        If Not IsPostBack Then


            Dim dsStat = stampa_controller.RiempiDataSourceStampa()

            '' impostazione datasource
            _rptStampa.SetDataSource(dsStat)

            'Dim FieldDef As FieldDefinition
            'FieldDef = _rptStampa.Database.Tables.Item("DT_Dati").Fields.Item("Totale")
            '_rptStampa.DataDefinition.Groups.Item(0).ConditionField = FieldDef

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
                dsStat.Dispose()
                dsStat = Nothing

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
                If Not ex Is Nothing Then
                    'SalvaLogErrori(ex.Message, _nomeDocumento, identificazioneDocumento)
                End If
            End Try


        End If

    End Sub

 
End Class