Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class SchedaRilievi
    Inherits System.Web.UI.Page


    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Public hideTabSelection As Boolean

    Private Sub SchedaRilievi_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        'Per gestire il pulsante "Indietro"
        AddHandler Master.ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '----- Verifico che l'utente sia autenticato
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        Master.Lbl_Titolo.Text = "Analisi Dati Schede Rilievi"

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

        Dim link As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
                                       enum_PagineGiasOnline_2010.Menu, _
                                       enum_PagineAgenda_2010.Menu, "", "", "", 0, "")

        Response.Redirect(link)

    End Sub

End Class