Imports System.Globalization
Imports System.IO
Imports AgronicaCoreVarieDAL


Public Class sidebarHelper
    Inherits System.Web.UI.WebControls.Label

    Private Function LoadUserControl() As String

        Dim pageHolder As Page = New Page()
        Dim header As New SidebarUC
        header.AssemblyName = "AgronicaControlli_2010"
        header.ControlClassName = "SidebarUC"
        header.ControlNamespace = "AgronicaControlli_2010"
        pageHolder.Controls.Add(header)
        Using output As New StringWriter()
            HttpContext.Current.Server.Execute(pageHolder, output, False)
            Return output.ToString()
        End Using

    End Function


    Public Function RenderUserControl() As String

        Dim componentePrincipale As String = LoadUserControl()

        Dim basePath As String = ""
        If Debugger.IsAttached Then
            basePath = "http://localhost"
        End If
        componentePrincipale = componentePrincipale.Replace("__basePath__", basePath)




        Dim ci As CultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture
        Dim runTimeResourceSet As Object = My.Resources.SidebarUC_ascx.ResourceManager.GetResourceSet(ci, True, True)
        For Each dictEntry As DictionaryEntry In runTimeResourceSet
            componentePrincipale = componentePrincipale.Replace(dictEntry.Key, dictEntry.Value)
        Next




        Dim app = Utilita_Compressione.RemoveWhitespaceFromHtml(componentePrincipale)
        Return app



    End Function

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        'Dim idDiv As String = "sideBar2022" & Me.ClientID

        Dim componentePrincipale As String = RenderUserControl()

        writer.Write(componentePrincipale)
        MyBase.Render(writer)

    End Sub

End Class
