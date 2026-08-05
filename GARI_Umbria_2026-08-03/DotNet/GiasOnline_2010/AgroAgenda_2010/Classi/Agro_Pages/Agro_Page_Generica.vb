Imports System.Web
Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports AgronicaCoreDataProvider

Namespace Agro_Pages_NS

    Public Class Agro_Page_Generica
        Inherits System.Web.UI.Page

        Protected objParametri_Server As AgronicaCoreParametri
        Protected objParametri_Utenti As AgronicaCoreParametri


        Private Sub Page_PreInit(sender As Object, e As System.EventArgs) Handles Me.PreInit

            inizializzoObjParametri()

            If Not IsPostBack Then

                verificoCredenzialiDiAccesso()

            End If

        End Sub

        Private Sub inizializzoObjParametri()
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        End Sub

        Private Sub verificoCredenzialiDiAccesso()
            '----- Verifico che l'utente sia autenticato
            If Session("ASG_Utente_Username") = "" Then
                Response.Redirect("~/Custom500.aspx")
            End If
        End Sub

    End Class

End Namespace
