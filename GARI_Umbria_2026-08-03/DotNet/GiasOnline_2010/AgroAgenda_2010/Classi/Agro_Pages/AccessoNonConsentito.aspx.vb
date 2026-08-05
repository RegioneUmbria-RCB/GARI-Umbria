Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgroAgenda_2010.My.Resources
Imports AgronicaCoreDataProvider

Public Class AccessoNonConsentito
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Private Sub AccessoNonConsentito_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        CType(Me.Master, AgendaBootstrap).flag_MostraBtnIndietro = True
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim link As String = "~/menu/menu.aspx"

        Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

        Dim sitoorigine As Enum_SiteRedirector = objParametriAgenda.SitoOrigine

        If sitoorigine = Enum_SiteRedirector.GiasNG Then
            AgronicaCoreGestioneRichieste.MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                                            Enum_SiteRedirector.GiasNG,
                                                                                            objParametriAgenda.PaginaSitoOrigine,
                                                                                            link,
                                                                                            objParametri_Server)
        Else

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
                link = "~/Menu/MenuBS_Agenda_Nuovo.aspx"
            End If

        End If



        Response.Redirect(link)
    End Sub

End Class