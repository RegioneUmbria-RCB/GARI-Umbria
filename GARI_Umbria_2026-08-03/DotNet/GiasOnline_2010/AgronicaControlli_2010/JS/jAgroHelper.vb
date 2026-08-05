
Imports AgronicaCoreVarieDAL

Public Class jAgroHelper
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"


    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("jAgroHelper", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.jAgroHelper.js"))

    End Sub


#End Region

End Class