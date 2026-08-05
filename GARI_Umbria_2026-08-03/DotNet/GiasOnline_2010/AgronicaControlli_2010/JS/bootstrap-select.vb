Public Class bootstrap_select
    Inherits System.Web.UI.WebControls.Label

    Public Property Versione As String = ""

    Protected Overrides Sub OnPreRender(e As EventArgs)

        If Versione = "1.12.4" Then
            Page.ClientScript.RegisterClientScriptInclude("bootstrap-select", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap-select_1.12.4.js"))
        Else
            Page.ClientScript.RegisterClientScriptInclude("bootstrap-select", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.bootstrap-select.js"))
        End If

    End Sub
End Class
