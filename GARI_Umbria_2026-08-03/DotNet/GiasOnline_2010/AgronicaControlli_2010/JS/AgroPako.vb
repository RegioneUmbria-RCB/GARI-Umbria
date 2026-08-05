Imports AgronicaCoreVarieDAL

Public Class AgroPako
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"


    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("pako", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.pako.min.js"))
    End Sub


#End Region

End Class
