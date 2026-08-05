Public Class GisControl
    Inherits System.Web.UI.UserControl



    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")


        If Not IsPostBack Then

        End If


    End Sub


End Class