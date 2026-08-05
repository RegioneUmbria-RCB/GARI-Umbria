Imports AgronicaCoreVarieDAL

Public Class jquery_cookie
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"


    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("jquery.cookie", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jquery.cookie.js"))

    End Sub


#End Region

End Class