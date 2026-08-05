Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza


Public Class VisualizzatoreReport
    Inherits System.Web.UI.Page

    Dim Qs_Anteprima As String
    Dim Qs_FilePDF As String

    Private Sub Page_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        If Not IsNothing(Request.QueryString("anteprima")) Then

            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString, _
                                              AgroKey_EncoderDecoder, _
                                              Server)
        Else
            Qs_Anteprima = "0"
        End If

        If Not IsNothing(Request.QueryString("pdf")) Then
            Qs_FilePDF = Stringa_Decodifica(Request.QueryString("pdf").ToString, _
                                              AgroKey_EncoderDecoder, _
                                              Server)
        Else
            Qs_FilePDF = ""
        End If

        ' nascondo l'albero dei gruppi
        CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None

        CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.HasExportButton = True
        CrystalReportViewer1.HasPrintButton = True
        CrystalReportViewer1.HasSearchButton = True


        Dim RPT As CrystalDecisions.CrystalReports.Engine.ReportClass
        RPT = Session("Report")

        'faccio il databind col visualizzatore dei reports...
        CrystalReportViewer1.ReportSource = RPT

        CrystalReportViewer1.DataBind()

        'Controllo se voglio mostrare direttamente il PDF
        Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim DtImpostazioni As DataTable = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_MODALITA_STAMPA, 1,
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "", Session("ASG_objParametri_Utenti"))

        If Not IsNothing(DtImpostazioni) AndAlso DtImpostazioni.Rows.Count > 0 AndAlso DtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = enum_Modalita_Stampa.MostraPDF Then
            Qs_Anteprima = "0"
        End If

        If Qs_Anteprima = "0" And Qs_FilePDF <> "" Then

            ' Con il seguente codice il file pdf viene scritto 
            ' nel browser del client.
            Response.ClearContent()
            Response.ClearHeaders()
            Response.ContentType = "application/pdf"
            Response.WriteFile(Qs_FilePDF)
            Response.Flush()
            Response.Close()

        ElseIf Qs_Anteprima = "0" And Qs_FilePDF = "" Then

            RPT.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Me.Response, False, "")

        End If
    End Sub



    '#########################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        'If Not IsNothing(Request.QueryString("anteprima")) Then

        '    Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString, _
        '                                      AgroKey_EncoderDecoder, _
        '                                      Server)
        'Else
        '    Qs_Anteprima = "0"
        'End If

        'If Not IsNothing(Request.QueryString("pdf")) Then
        '    Qs_FilePDF = Stringa_Decodifica(Request.QueryString("pdf").ToString, _
        '                                      AgroKey_EncoderDecoder, _
        '                                      Server)
        'Else
        '    Qs_FilePDF = ""
        'End If

        '' nascondo l'albero dei gruppi
        'CrystalReportViewer1.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None

        'CrystalReportViewer1.DisplayToolbar = True
        'CrystalReportViewer1.HasExportButton = True
        'CrystalReportViewer1.HasPrintButton = True
        'CrystalReportViewer1.HasSearchButton = True


        'Dim RPT As CrystalDecisions.CrystalReports.Engine.ReportClass
        'RPT = Session("Report")

        ''faccio il databind col visualizzatore dei reports...
        'CrystalReportViewer1.ReportSource = RPT

        'CrystalReportViewer1.DataBind()

        'If Qs_Anteprima = "0" And Qs_FilePDF <> "" Then

        '    ' Con il seguente codice il file pdf viene scritto 
        '    ' nel browser del client.
        '    Response.ClearContent()
        '    Response.ClearHeaders()
        '    Response.ContentType = "application/pdf"
        '    Response.WriteFile(Qs_FilePDF)
        '    Response.Flush()
        '    Response.Close()

        'ElseIf Qs_Anteprima = "0" And Qs_FilePDF = "" Then

        '    RPT.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Me.Response, False, "")

        'End If


    End Sub

End Class