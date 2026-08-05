Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp

Public Class DatiReteAcqua
    Inherits System.Web.UI.Page

    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private objParametriAgenda As ParametriAgenda
    Public srv_gm As String

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Lingua.Gias_InizializzaCultura_DaSession()

    End Sub

    Public sUtenteAbilitatoLettura As String
    Public sUtenteAbilitatoScrittura As String
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'errore autenticazione o sessione scaduta
        If IsNothing(Session("ASG_objParametri_Server")) OrElse IsNothing(Session("ASG_objParametri_Utenti")) Then
            Response.Redirect("~/Custom500.aspx")
        End If

        objParametriAgenda = New ParametriAgenda
        Session("objParametriAgenda") = objParametriAgenda

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

        hdPiva.Value = objParametriAgenda.Piva

        'Controllo se l'utente ha i permessi per accedere
        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        Dim UtenteAbilitatoLettura As Boolean = False
        Dim UtenteAbilitatoScrittura As Boolean = False


        UtenteAbilitatoLettura = objPermessi.Controlla_Permessi_Utente(
                                            Session("ASG_Utente_Username"),
                                            Session("ASG_IdServizio"),
                                            enum_Security_Attivita.ReteAcqua_AnalisiDati,
                                            enum_Security_Operazione.Lettura,
                                            Date.Now,
                                            "",
                                            objParametri_Utenti)

        UtenteAbilitatoScrittura = objPermessi.Controlla_Permessi_Utente(
                                           Session("ASG_Utente_Username"),
                                           Session("ASG_IdServizio"),
                                           enum_Security_Attivita.ReteAcqua_AnalisiDati,
                                           enum_Security_Operazione.Modifica,
                                           Date.Now,
                                           "",
                                           objParametri_Utenti)


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

        Dim AperturaDaPopup = Request.QueryString.AllKeys.Contains("Ifr")

        If AperturaDaPopup Then
            Master.flag_MostraHeader = False
            Master.flag_MostraFooter = False
        End If

        If Not UtenteAbilitatoLettura Then
            AnnullaTutto()
        End If
    End Sub

    Private Sub DatiReteAcqua_Init(sender As Object, e As EventArgs) Handles Me.Init
        AddHandler CType(Me.Master, AgendaBootstrap).ImgBtn_AnnullaTutto.Click, AddressOf Me.AnnullaTutto
    End Sub

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

    End Sub


End Class