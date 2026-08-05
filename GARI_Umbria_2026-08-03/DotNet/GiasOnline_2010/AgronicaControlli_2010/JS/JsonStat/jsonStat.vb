Imports AgronicaCoreVarieDAL


Public Class jsonStat
    Inherits System.Web.UI.WebControls.Label


    Public Property VersionJsonStat As String

    Protected Overrides Sub OnPreRender(e As EventArgs)



        Page.ClientScript.RegisterClientScriptInclude("json-stat" & VersionJsonStat, _
                    Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.json-stat" & _VersionJsonStat & ".js"))



    End Sub

End Class
