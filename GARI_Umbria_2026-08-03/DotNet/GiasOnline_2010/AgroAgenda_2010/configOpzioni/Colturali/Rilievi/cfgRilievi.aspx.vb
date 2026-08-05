Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class cfgRilievi
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public hideTabSelection As Boolean

    Private PaginaRitorno As String = "../../../Menu/Menu.aspx"

    Private Sub SchedaRilievi_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'Per gestire il pulsante "Indietro"
        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Master.flag_pag_Operazione = True

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Master.Lbl_Titolo.Text = "Configurazione Rilievi Avversità"

        hideTabSelection = True

        '---
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        '---

    End Sub

    ''' <summary>
    ''' Bottone di annullamento
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)


        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            PaginaRitorno = "../../../Menu/MenuBS_Agenda_Nuovo.aspx"
        End If

        Response.Redirect(PaginaRitorno)

    End Sub
End Class