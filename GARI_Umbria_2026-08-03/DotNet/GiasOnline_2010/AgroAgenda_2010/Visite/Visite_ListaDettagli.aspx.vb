Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Visite_ListaDettagli
    Inherits System.Web.UI.Page

    Dim objParametri_Server, objParametri_Utenti As AgronicaCoreParametri

    Private Sub MenuBS_Agenda_Nuovo_Init(sender As Object, e As System.EventArgs) Handles Me.Init
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

        'Visita/ Lettura controllato nella master

        Dim UtenteAbilitatoCreazioneVisita As Boolean = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.Visite_Lista,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)

        'Imposto le variabili di ponte con il client
        hf_UtenteAbilitatoCreazioneVisita.Value = UtenteAbilitatoCreazioneVisita

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
                objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Visite_Lista
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

    <Script.Services.ScriptMethod()>
    <Services.WebMethod(EnableSession:=True)>
    Public Shared Function nuova_operazione() As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda

        objParametriAgenda.Lav_Cod = CostantiPersonalizzate.LAVCOD_VISITA
        objParametriAgenda.Id_Agenda = 0
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Scrittura
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Visite_Lista

        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni 'Utility_NS.Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(objParametriAgenda.Lav_Cod, objParametriAgenda, PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Pagina_Visite_Lista, fromBootstrapToBootstrap:=True)

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl

        Return r

    End Function

    <Script.Services.ScriptMethod()>
    <Services.WebMethod(EnableSession:=True)>
    Public Shared Function infomodifica_operazione_singola(ByVal type As Integer, ByVal id_agenda As String, ByVal piva As String, ByVal lav_cod As String) As AgronicaCoreVarieBIZ.RispostaStandard

        Dim r As New AgronicaCoreVarieBIZ.RispostaStandard
        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")

        If HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server) Then
            r.Sessione = False
            Return r
        End If

        Dim objParametriAgenda As New AgronicaCoreModello.ParametriAgenda_Temp.ParametriAgenda
        objParametriAgenda.Piva = piva
        'objParametriAgenda.Data = data
        objParametriAgenda.Id_Agenda = id_agenda
        objParametriAgenda.Sa_Cod = 0
        objParametriAgenda.Lav_Cod = lav_cod
        objParametriAgenda.TipoOperazioneAgenda = enum_Tipo_Operazione_Agenda.QuadernoDiCampagna
        objParametriAgenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale
        objParametriAgenda.Programmazione_Cod = 0
        objParametriAgenda.Tipo_Operazione = type
        objParametriAgenda.PaginaSitoOrigine = enum_PagineAgenda_2010.Pagina_Visite_ListaDettagli

        Dim OpUtil As New AgronicaCoreModello.Utility_Operazioni
        Dim TargetUrl As String = OpUtil.LinkPagina_from_LavCod_NEW(lav_cod, objParametriAgenda, PaginaSitoAgendaOrigine:=enum_PagineAgenda_2010.Pagina_Visite_ListaDettagli, fromBootstrapToBootstrap:=True)

        r.RispostaOK = True
        r.RispostaStringa = TargetUrl

        Return r

    End Function


End Class