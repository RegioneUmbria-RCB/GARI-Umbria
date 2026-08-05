
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp



Public Class Meteo
    Inherits System.Web.UI.Page

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public srv_gm As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Dim qs_t As Integer = Request.QueryString("t")
        'If qs_t = 3 Then
        '    'Response.Redirect("DSS_Difesa.aspx?" & Request.QueryString.ToString)
        '    Response.Redirect("../AnalisiRilievi/AnalisiRilievi.aspx?" & Request.QueryString.ToString)
        'End If

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

        'If Request.QueryString("gis") <> "" Then
        '    Me.Master.flag_MostraHeader = False
        '    Me.Master.flag_MostraFooter = False
        'Else
        '    Me.Master.flag_pag_Operazione = True
        'End If

        Me.Master.flag_pag_Operazione = True

        Dim objParametri_Server As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Server")
        Dim objParametri_Utenti As AgronicaCoreParametri = HttpContext.Current.Session("ASG_objParametri_Utenti")

        Dim AperturaDaPopupOld As Boolean = False
        If Request.QueryString.Get("ViewModal") IsNot Nothing Then
            AperturaDaPopupOld = CType(Request.QueryString.Get("ViewModal"), Boolean)
        End If

        If AperturaDaPopupOld Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        Else
            Master.flag_MostraHeader = True
            Master.flag_MostraFooter = True
        End If

        If Not (HttpContext.Current.Session.IsNewSession OrElse IsNothing(objParametri_Server)) Then

            'Recupero da webconfig la connessione alternativa da usare
            Dim AgroWebC As New AgronicaCoreGestioneRichieste.AgroWebConfig()

            srv_gm = "https://maps.googleapis.com/maps/api/js?key="

            If AgroWebC.GoogleMaps <> "" Then
                srv_gm = AgroWebC.GoogleMaps
            End If

            If Debugger.IsAttached Then
                srv_gm = "https://maps.googleapis.com/maps/api/js?v=3.exp&client=gme-addictive&sensor=false&libraries=drawing,geometry&callback=Function.prototype"
            End If

        End If

        hdPiva.Value = objParametriAgenda.Piva

        Dim AperturaDaPopup = Request.QueryString.AllKeys.Contains("Ifr")

        If AperturaDaPopup Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

    End Sub

    Private Sub Meteo_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

    Private objParametriAgenda As ParametriAgenda

    Private Sub AnnullaTutto()

        Dim objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server")

        Dim configurazioneSitiLettura As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim flagMenu = configurazioneSitiLettura.Leggi_Valore(0, "MenuBS_2017", "", "", objParametri_Server)

        If flagMenu.ToLower = "true" Then

            Response.Redirect("../../menu/menubs_2017.aspx")
        Else

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))
        End If


        objParametriAgenda = Session("objParametriAgenda")


        Dim paginaOnLineRitorno As Integer = objParametriAgenda.PaginaSitoOrigine

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline And
            paginaOnLineRitorno = enum_PagineGiasOnline.MenuMagazzini Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(True, paginaOnLineRitorno, objParametriAgenda))

        End If

        If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 And
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu Then

            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))

        End If

        If paginaOnLineRitorno = 0 Then
            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
                paginaOnLineRitorno = enum_PagineGiasOnline_2010.Menu
            Else
                paginaOnLineRitorno = enum_PagineAgenda_2010.Menu
            End If
            Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, paginaOnLineRitorno, objParametriAgenda))
        End If

        Response.Redirect(CType(Master, AgendaBootstrap).TrovaRedirectCorretto(False, enum_PagineAgenda_2010.Menu, objParametriAgenda))

    End Sub

End Class