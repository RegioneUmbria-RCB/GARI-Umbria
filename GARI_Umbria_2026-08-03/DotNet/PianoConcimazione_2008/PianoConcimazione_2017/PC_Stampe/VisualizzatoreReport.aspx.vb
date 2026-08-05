Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports CrystalDecisions.CrystalReports.Engine
Imports CrystalDecisions.Shared


Public Class VisualizzatoreReport
    Inherits System.Web.UI.Page

    Dim Qs_Anteprima As String
    Dim Qs_FilePDF As String
    Dim Qs_FlagAllegati As Boolean
    Dim rptTempSuDisco As Boolean


    '#########################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Page.Request.QueryString("tmpReportPath") Is Nothing Then
            rptTempSuDisco = False
        Else
            rptTempSuDisco = True
        End If

        If Not IsNothing(Request.QueryString("anteprima")) Then

            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString,
                                              AgroKey_EncoderDecoder,
                                              Server)
        Else
            Qs_Anteprima = "0"
        End If

        If Not IsNothing(Request.QueryString("pdf")) Then
            Qs_FilePDF = Stringa_Decodifica(Request.QueryString("pdf").ToString,
                                              AgroKey_EncoderDecoder,
                                              Server)
        Else
            Qs_FilePDF = ""
        End If

        If Not IsNothing(Request.QueryString("fall")) Then
            Qs_FlagAllegati = Stringa_Decodifica(Request.QueryString("fall").ToString,
                                              AgroKey_EncoderDecoder,
                                              Server)
        Else
            Qs_FlagAllegati = False
        End If

        CrystalReportViewer1.DisplayToolbar = True
        CrystalReportViewer1.HasExportButton = True
        CrystalReportViewer1.HasPrintButton = True

        Dim RPT As CrystalDecisions.CrystalReports.Engine.ReportDocument

        'MS Per discriminare nuova e vecchia gestione verifico se è stato passato i path del report temporaneo in QueryString
        If rptTempSuDisco Then
            'MS Nuovo giro con report temporaneo caricato da disco
            Try
                Dim tmpReportPath As String = Stringa_Decodifica(Page.Request.QueryString("tmpReportPath").ToString, AgroKey_EncoderDecoder, Server)
                RPT = New ReportDocument
                RPT.Load(tmpReportPath)
            Catch ex As Exception
                Throw (New Exception("Errore nel caricamento report temporaneo da VisualizzatoreReport: ", ex))
            End Try
        Else
            'MS Vecchio giro con riferimento a report in session che crea il problema perchè non rilascia la memoria
            RPT = Session("Report")
        End If

        'faccio il databind col visualizzatore dei reports...
        CrystalReportViewer1.ReportSource = RPT

        CrystalReportViewer1.DataBind()

        'leggo l'impsotazione solo se non sono nella gestione degli allegati
        'altrimenti rischio di sovraschivere Qs_FilePDF
        If Qs_FlagAllegati = False Then

            'Controllo se voglio mostrare direttamente il PDF
            Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
            Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
            Dim DtImpostazioni As DataTable = objUtenti.Leggi_Utente_Poi_SuperUser(enum_Impostazioni_Utenti.UTENTE_COD_MODALITA_STAMPA, 1,
                             AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                             "", "", objParametri_Utenti)

            If Not IsNothing(DtImpostazioni) AndAlso DtImpostazioni.Rows.Count > 0 AndAlso DtImpostazioni.Rows(0).Item("Impostazione_Valore_1") = enum_Modalita_Stampa.MostraPDF Then
                Qs_Anteprima = "0"
                Qs_FilePDF = ""
            End If

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

End Class