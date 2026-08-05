Imports System.Text
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AgronicaMessaggi
    Inherits AgroControlliCommons

#Region "Methods & Event Handlers"



    Protected Overrides Sub OnPreRender(e As EventArgs)

        Dim ObjParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri =
            HttpContext.Current.Session("ASG_objParametri_Server")

        If Not ObjParametriServer Is Nothing Then
        End If
        Page.ClientScript.RegisterClientScriptInclude("AgronicaMessaggi", Page.ClientScript.GetWebResourceUrl(Me.[GetType](), "AgronicaControlli_2010.AgronicaMessaggi.js"))


    End Sub

    Private Sub AgronicaMessaggi_Init(sender As Object, e As EventArgs) Handles Me.Init

        Dim ObjParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri =
           HttpContext.Current.Session("ASG_objParametri_Super_Server")

        If Not ObjParametriServer Is Nothing Then
        End If

    End Sub


#End Region

End Class
