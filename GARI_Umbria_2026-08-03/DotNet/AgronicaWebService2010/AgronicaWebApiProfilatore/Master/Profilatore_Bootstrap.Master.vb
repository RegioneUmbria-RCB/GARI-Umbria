Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaControlli_2010
Imports Agronica.Helpers.GiasBase

Public Class Profilatore_Bootstrap
    Inherits System.Web.UI.MasterPage

    Public objParametri_server_String As String
    Public objParametri_utenti_String As String

    Public pathCoreWS As String

    Public debug_isattached As Boolean = False

    Public ReadOnly Property agroMasterPageControl As AgronicaControlli_2010.AgroMasterPage
        Get
            Return agroMasterPage
        End Get
    End Property

    Public Property ImgBtn_AnnullaTutto As ImageButton
        Get
            Return agroMasterPage.ImgBtnAnnullaTutto
        End Get
        Set(value As ImageButton)
            agroMasterPage.ImgBtnAnnullaTutto = value
        End Set
    End Property

    Public Property ImgBtn_Menu2017 As ImageButton
        Get
            Return agroMasterPage.ImgBtn_Menu2017
        End Get
        Set(value As ImageButton)
            agroMasterPage.ImgBtn_Menu2017 = value
        End Set
    End Property

    Public Property ImgBtn_Filtro As ImageButton
        Get
            Return agroMasterPage.ImgBtnFiltro
        End Get
        Set(value As ImageButton)
            agroMasterPage.ImgBtnFiltro = value
        End Set
    End Property

    Public Property flag_pag_MenuPrincipale As Boolean
        Get
            Return agroMasterPage.flag_pag_MenuPrincipale
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_MenuPrincipale = value
        End Set
    End Property

    Public Property flag_pag_Anagrafica As Boolean
        Get
            Return agroMasterPage.flag_pag_Anagrafica
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_Anagrafica = value
        End Set
    End Property

    Public Property flag_pag_MenuAnagrafica As Boolean
        Get
            Return agroMasterPage.flag_pag_MenuAnagrafica
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_MenuAnagrafica = value
        End Set
    End Property

    Public Property flag_pag_MenuAgenda As Boolean
        Get
            Return agroMasterPage.flag_pag_MenuAgenda
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_MenuAgenda = value
        End Set
    End Property

    Public Property flag_pag_BootstrapModal As Boolean
        Get
            Return agroMasterPage.flag_pag_BootstrapModal
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_BootstrapModal = value
        End Set
    End Property


    Public Property flag_pag_Scadenzario As Boolean
        Get
            Return agroMasterPage.flag_pag_Scadenzario
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_Scadenzario = value
        End Set
    End Property


    Public Property flag_pag_Visite As Boolean
        Get
            Return agroMasterPage.flag_pag_Visite
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_Visite = value
        End Set
    End Property


    Public Property flag_MenuBS_2017 As Boolean
        Get
            Return agroMasterPage.flag_MenuBS_2017
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MenuBS_2017 = value
        End Set
    End Property

    Public Property flag_titoloSessione_MenuBS_2017 As Boolean
        Get
            Return agroMasterPage.flag_titoloSessione_MenuBS_2017
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_titoloSessione_MenuBS_2017 = value
        End Set
    End Property

    Public Property Lbl_Titolo() As Global.System.Web.UI.WebControls.Label
        Get
            Return agroMasterPage.Lbl_Titolo
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            agroMasterPage.Lbl_Titolo = value
        End Set
    End Property

    Public Property ImgBtnAnnullaTutto() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return agroMasterPage.ImgBtnAnnullaTutto
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            agroMasterPage.ImgBtnAnnullaTutto = value
        End Set
    End Property

    Public Property ImgBtnFiltro() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return agroMasterPage.ImgBtnFiltro
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            agroMasterPage.ImgBtnFiltro = value
        End Set
    End Property

    Public Property LblRag_Soc() As Global.System.Web.UI.WebControls.Label
        Get
            Return agroMasterPage.LblRag_Soc
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            agroMasterPage.LblRag_Soc = value
        End Set
    End Property

    Public Property lblFinestraTemporale() As Global.System.Web.UI.WebControls.Label
        Get
            Return agroMasterPage.lblFinestraTemporale
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            agroMasterPage.lblFinestraTemporale = value
        End Set
    End Property

    Public Property LbLUtente() As Global.System.Web.UI.WebControls.Label
        Get
            Return agroMasterPage.LbLUtente
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            agroMasterPage.LbLUtente = value
        End Set
    End Property

    Public Property flag_MostraBtnIndietro() As Boolean
        Get
            Return agroMasterPage.flag_MostraBtnIndietro
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraBtnIndietro = value
        End Set
    End Property

    Private _LinkGiasBase As String = String.Empty
    Public ReadOnly Property PATH_GIASBASE As String
        Get
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
            Return _LinkGiasBase
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        Dim objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri

        objParametri_Super_Server.PivaSuperUser = ""
        objParametri_Super_Server.UsernameOperazione = ""
        objParametri_Super_Server.UtenteUsername = ""
        objParametri_Super_Server.UtenteCodFiscale = ""
        objParametri_Super_Server.SuperUserUsername = ""
        objParametri_Super_Server.FinestraTemporaleInizio = New Date(1900, 1, 1)
        objParametri_Super_Server.FinestraTemporaleFine = New Date(2100, 12, 31)
        'objParametri_Server.FlagVisibilita = ""
        'objParametri_Server.FlagCancellazioneLogica = ""
        objParametri_Super_Server.objConnessione = Nothing
        objParametri_Super_Server.objTransazione = Nothing
        objParametri_Super_Server.StringaConnessione = ConfigurationManager.AppSettings("StringaConnessione_SuperServer")
        objParametri_Super_Server.LogDirectory = ""
        objParametri_Super_Server.LogFileName = ""
        objParametri_Super_Server.LogDescrizioneUtente = ""
        'objParametri_Super_Server.LinguaobjParametri_Server.Cod = ""

        Session("ASG_objParametri_Super_Server") = objParametri_Super_Server


        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri

        objParametri_Server.PivaSuperUser = ""
        objParametri_Server.UsernameOperazione = ""
        objParametri_Server.UtenteUsername = ""
        objParametri_Server.UtenteCodFiscale = ""
        objParametri_Server.SuperUserUsername = ""
        objParametri_Server.FinestraTemporaleInizio = New Date(1900, 1, 1)
        objParametri_Server.FinestraTemporaleFine = New Date(2100, 12, 31)
        'objParametri_Server.FlagVisibilita = ""
        'objParametri_Server.FlagCancellazioneLogica = ""
        objParametri_Server.objConnessione = Nothing
        objParametri_Server.objTransazione = Nothing
        objParametri_Server.StringaConnessione = ConfigurationManager.AppSettings("StringaConnessione_Web_Server")
        objParametri_Server.LogDirectory = ""
        objParametri_Server.LogFileName = ""
        objParametri_Server.LogDescrizioneUtente = ""
        'objParametri_Server.LinguaobjParametri_Server.Cod = ""

        Session("ASG_objParametri_Server") = objParametri_Server

        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri

        objParametri_Utenti.PivaSuperUser = ""
        objParametri_Utenti.UsernameOperazione = ""
        objParametri_Utenti.UtenteUsername = ""
        objParametri_Utenti.UtenteCodFiscale = ""
        objParametri_Utenti.SuperUserUsername = ""
        objParametri_Utenti.FinestraTemporaleInizio = New Date(1900, 1, 1)
        objParametri_Utenti.FinestraTemporaleFine = New Date(2100, 12, 31)
        'objParametri_Utenti.FlagVisibilita = ""
        'objParametri_Utenti.FlagCancellazioneLogica = ""
        objParametri_Utenti.objConnessione = Nothing
        objParametri_Utenti.objTransazione = Nothing
        objParametri_Utenti.StringaConnessione = ConfigurationManager.AppSettings("StringaConnessione_Web_Utenti")
        objParametri_Utenti.LogDirectory = ""
        objParametri_Utenti.LogFileName = ""
        objParametri_Utenti.LogDescrizioneUtente = ""
        'objParametri_Server.LinguaobjParametri_Server.Cod = ""

        Session("ASG_objParametri_Utenti") = objParametri_Utenti


        jquery.SitoOspite = Enum_SiteRedirector.Sito_AgronicaWebApiProfilatore
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader
        bootstrap.SitoOspite = Enum_SiteRedirector.Sito_AgronicaWebApiProfilatore

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaWebApiProfilatore

        agroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        agroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        agroMasterPage.FooterPlaceHolder = footerPlaceHoler

    End Sub


End Class