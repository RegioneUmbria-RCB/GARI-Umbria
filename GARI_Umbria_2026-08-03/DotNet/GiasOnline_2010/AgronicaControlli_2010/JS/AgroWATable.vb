Imports AgronicaCoreVarieDAL

Public Class AgroWATable
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"


    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("AgroWATable", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.waTableHelper.js"))

    End Sub


#End Region

End Class
