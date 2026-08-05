Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Visite_Anagrafiche
    Inherits System.Web.UI.Page

    Public objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private Sub Visite_Anagrafiche_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Page.Master.Master, AgendaBootstrap).ImgBtnAnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Response.Expires = 0

        'Controllo se la sessione è ancora su
        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametri_Utenti = New AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R

        'Scadenzario/Lettura controllato nella master

        Dim UtenteAbilitatoScrittura As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Visite_Anagrafiche,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now, "", objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoScrittura.Value = UtenteAbilitatoScrittura

    End Sub

    Private Sub AnnullaTutto(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda()

        Dim link As String = ""
        Try
            Dim sitoorigine As Enum_SiteRedirector = HttpContext.Current.Session("Sito_Origine")
            Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

            If sitoorigine = Enum_SiteRedirector.Sito_GiasOnline_2010 AndAlso paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

                link = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                       enum_PagineGiasOnline_2010.Menu,
                                       enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_GiasOnline AndAlso paginaOnLineRitorno = enum_PagineGiasOnline.MenuCartellaAziendale Then

                objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Scadenzario_Lista
                link = AgronicaCoreModello.Utility_Operazioni.Link_GiasOnline_STR(enum_PagineGiasOnline.MenuCartellaAziendale, objParametriAgenda)

            ElseIf sitoorigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 AndAlso paginaOnLineRitorno = enum_PagineAgenda_2010.Menu_BS Then
                link = "../Menu/MenuBS_Agenda_Nuovo.aspx"
            Else
                link = "../Menu/MenuBS_2017.aspx"
            End If

        Catch ex As Exception
            link = "../Menu/MenuBS_2017.aspx"
        End Try

        Response.Redirect(link)
    End Sub


End Class