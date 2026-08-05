Imports System
Imports System.Web
Imports System.Web.UI


Public Class javascript_helper

#Region "const"
    Private Const TEMPLATE_SCRIPT As String = "<script type=""text/javascript"" src=""{0}""></script>" & vbCrLf
#End Region


    ''' <summary>
    ''' Includes the specified embedded JavaScript file in the page.
    ''' </summary>
    ''' <param name="manager">Accessible via Page.ClientScript.</param>
    ''' <param name="resourceName">The name used to identify the embedded JavaScript file.</param>
    ''' <param name="late">Include the JavaScript at the bottom of the HTML?</param>
    Public Shared Function IncludeJavaScript(ByVal manager As ClientScriptManager, ByVal resourceName As String, ByVal late As Boolean) As String
        Dim type = GetType(AgronicaControlli_2010.javascript_helper)
        Dim url As String = manager.GetWebResourceUrl(type, resourceName)
        If Not manager.IsStartupScriptRegistered(type, resourceName) Then
            If late Then
                Dim scriptBlock As String = String.Format(TEMPLATE_SCRIPT, HttpUtility.HtmlEncode(url))
                manager.RegisterStartupScript(type, resourceName, scriptBlock)
            Else
                manager.RegisterClientScriptResource(type, resourceName)
                manager.RegisterStartupScript(type, resourceName, String.Empty)
            End If
        End If

        Return url
    End Function

End Class
