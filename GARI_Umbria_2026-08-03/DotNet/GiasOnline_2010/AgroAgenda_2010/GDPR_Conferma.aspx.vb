Imports AgronicaCoreDataProvider

Public Class GDPR_Conferma
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'phInfo.Controls.Add(New LiteralControl("Confermo di aver preso visione dell'informativa ai sensi..."))


        Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        Dim GDPRCod As String = Request.QueryString("GDPRCod")
        hdVersioneGDPR.Value = GDPRCod

        Dim xleggi As New AgronicaCoreUtentiDAL.GDPR_R
        Dim dtG As DataTable =
            xleggi.Leggi(GDPRCod, "", "", objParametri_Utenti)

        Dim rval As New AgronicaCoreUtentiBIZ.GDPRmodel

        If dtG.Rows.Count > 0 Then
            rval.GDPR_Cod = dtG.Rows(0)("GDPR_Cod")
            rval.TestoHtmlBreve = dtG.Rows(0)("TestoHtmlBreve")
            rval.TestoHtmlCompleto = dtG.Rows(0)("TestoHtmlCompleto")
            rval.Validita_Inizio = dtG.Rows(0)("Validita_Inizio")
            rval.Validita_Fine = dtG.Rows(0)("Validita_Fine")
        End If

        phInfo.Controls.Add(New LiteralControl(rval.TestoHtmlBreve))



    End Sub

    Protected Sub conferma_Click(sender As Object, e As EventArgs) Handles conferma.Click


        Dim objParametri_Utenti As AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim xRvalScrittura As New AgronicaCoreUtentiDAL.Utenti_GDPR_Accettazione_W
        Dim dtLettura As DataTable = Nothing

        Dim xverifica As New AgronicaCoreUtentiBIZ.GDPR

        xverifica.OttieniGDPRAccettati(hdVersioneGDPR.Value, objParametri_Utenti, dtLettura)

        'verifica forse superflua se ci si trova in questa pagina ... 
        If dtLettura.Rows.Count = 0 Then
            xRvalScrittura.Scrivi(hdVersioneGDPR.Value, 1, Now, objParametri_Utenti)
        End If

        Dim url As String = Session("GDPR_Conferma_redir")
        Response.Redirect(url)
    End Sub



End Class