Imports AgronicaCoreVarieDAL

Public Class AgroTimezone
    Inherits System.Web.UI.WebControls.Label

#Region "Methods & Event Handlers"


    Protected Overrides Sub OnPreRender(e As EventArgs)
        Page.ClientScript.RegisterClientScriptInclude("agro-timezone", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.AgroTimezone.js"))
        Page.ClientScript.RegisterClientScriptInclude("moment-with-locales", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.moment-with-locales.js"))
        Page.ClientScript.RegisterClientScriptInclude("moment-timezone", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.moment-timezone-with-data.js"))
    End Sub


#End Region

End Class
