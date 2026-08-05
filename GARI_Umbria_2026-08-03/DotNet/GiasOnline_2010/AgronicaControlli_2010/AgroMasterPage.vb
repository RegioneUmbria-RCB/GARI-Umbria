Imports System.Configuration
Imports System.Data
Imports System.Text
Imports System.Xml.XPath
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModello
Imports AgronicaCoreDataProvider.Sicurezza
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreModelsSTD.utente
Imports AgronicaCoreModelsSTD.Matomo
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AngleSharp.Io

Public Class AgroMasterPage
    Inherits AgroControlliCommons
    Implements iAgronicaControlliCommons

    Private _utenteUsername As String
    Private _Licenza_Inizializzata As Boolean = False

    Private _CssPlaceHolder As PlaceHolder
    Private _HeaderPlaceHolder As PlaceHolder
    Private _FooterPlaceHolder As PlaceHolder
    Private _VersioneHeaderPlaceHolder As PlaceHolder
    Private _sAssistenzaInfo As String

    Private LblTitolo As Label

    Private _Lbl_ScadenzaPermessi As Label

    Private _ImgBtn_AnnullaTutto As ImageButton
    Private _ImgBtn_Filtro As ImageButton
    Private _ImgBtn_Menu2017 As ImageButton

    Private Lbl_Rag_Soc As Label
    Private lbl_FinestraTemporale As Label
    Private LbL_Utente As Label
    Private LbL_UltimoAccesso As Label
    Private lblDescrGeneric As Label

    'Private _flag_MenuBS_2017 As Boolean = False
    'Private _flag_titoloSessione_MenuBS_2017 As Boolean = False

    Public flag_pag_MenuPrincipale As Boolean = False
    Public flag_pag_Anagrafica As Boolean = False
    Public flag_pag_MenuAnagrafica As Boolean = False
    Public flag_pag_MenuAgenda As Boolean = False
    Public flag_pag_CampionamentoConferito As Boolean = False
    Public flag_pag_LavorazioniFF As Boolean = False
    Public flag_pag_AnalisiProgetti As Boolean = False
    Public flag_pag_GestioneCompletaCdG As Boolean = False
    Public flag_pag_ScaricoTempi As Boolean = False
    Public flag_pag_GestioneCosti As Boolean = False
    Public flag_pag_Operazione As Boolean = False
    Public flag_pag_BootstrapModal As Boolean = False
    Public flag_pag_Scadenzario As Boolean = False
    Public flag_pag_Visite As Boolean = False
    Public flag_pag_DocContabile As Boolean = False

    Public flag_pag_MenuBS_2017 As Boolean = False
    Public flag_base_MenuBS_2017 As Boolean = False
    Public flag_link_MenuBS_2017 As Boolean = True
    Public flag_MenuBS_2017 As Boolean = False
    Public piva_MenuBS_2017 As String = ""
    Public url_MenuBS_2017 As String = ""
    Public ws_MenuBS_2017 As Boolean = False
    Public config_MenuBS_2017 As JObject = Nothing

    Public flag_titoloSessione_MenuBS_2017 As Boolean = False
    Public testoTitoloSessione_MenuBS_2017 As String = ""
    Public testoTitoloSessione_MenuBS_2022 As String = ""
    Public idSezione_MenuBS_2022 As Integer = 0
    Public coloreTitoloSessione_MenuBS_2017 As String = ""
    Public menuTitoloSessione_MenuBS_2017 As String = ""
    Public azioniTitoloSessione_MenuBS_2017 As String = ""
    Public configurazioneTitoloSessione_MenuBS_2017 As String = ""

    Public flag_pag_GestioneMagazziniBS As Boolean = False

    Public flag_MostraHeader As Boolean = True
    Public flag_MostraFooter As Boolean = True

    Public flag_MostraBtnEsci As Boolean = False
    Public flag_MostraBtnIndietro As Boolean = False
    Public flag_MostraBtnCambiaImpresa As Boolean = False

    Public flag_Matomo_Enabled As Boolean = False

    Public UsaFileMinified As Boolean = False
    Private _modalitaMin As String = ""

    'Public personalizzazioniRegioneUmbria = ""

    Public CustomLoghi As Object = Nothing

    Public azione_indietro As String = ""
    Public id_Azione_indietro As String = ""
    Public MemorizzaInSessioneDopoLettura As Boolean = True

    Private paginePreLogin As String() = {"ASP.index_aspx", "ASP.recuperacredenziali_cambiapassword_aspx", "ASP.recuperacredenziali_troublelogin_aspx"}

    Private _versioneHeader As String = ""
    Private _headerVersionDisponibili As String() = {VERSIONE_HEADER_DEFAULT, "2022"}
    Private apiController As CoreApiControllerFactory = New CoreApiControllerFactory
    Private _SitoOspite As TipiEnumerativi.Enum_SiteRedirector =
        AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.Sito_AgronicaAgenda_2010

    Public ReadOnly Property PathLogoGiasOnlineGrande As String
        Get
            Return _LinkGiasBase & "agronica/AB_Immagini/logo/Logo_GiasOnline_Grande.jpg"
        End Get
    End Property

    Public ReadOnly Property PathLogoGiasLogin As String
        Get
            Dim PuntoInterrogativo As String = If(GiasVersioneCorrente.StartsWith("?"), "", "?")
            Return _LinkGiasBase & "agronica/AB_Immagini/logo/Logo_Gias_Login.png" & PuntoInterrogativo & _GiasVersioneCorrente
        End Get
    End Property

    Public ReadOnly Property PathLogoAgronica As String
        Get
            Return _LinkGiasBase & "agronica/AB_Immagini/logo/logo_small.png"
        End Get
    End Property


    Private _listaLoghiPersonalizzati As String = ""
    Public ReadOnly Property PathLoghiPersonalizzati As List(Of String)
        Get
            Dim rval As New List(Of String)

            If _listaLoghiPersonalizzati.Contains("{") Then
                Dim o1 As JObject = JObject.Parse(_listaLoghiPersonalizzati)
                rval.Add(_BasePath & CStr(o1("path")))
            Else
                rval.Add(_BasePath & _listaLoghiPersonalizzati)
            End If


            Return rval
        End Get
    End Property

    Public ReadOnly Property lblDescrGeneric_ClientID As String
        Get
            Return lblDescrGeneric.ClientID
        End Get
    End Property

    Public Property Lbl_Titolo() As Global.System.Web.UI.WebControls.Label
        Get
            Return LblTitolo
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            LblTitolo = value
        End Set
    End Property

    Public Property ImgBtnAnnullaTutto() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return _ImgBtn_AnnullaTutto
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            _ImgBtn_AnnullaTutto = value
        End Set
    End Property

    Public Property ImgBtnFiltro() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return _ImgBtn_Filtro
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            _ImgBtn_Filtro = value
        End Set
    End Property

    Public Property ImgBtn_Menu2017() As Global.System.Web.UI.WebControls.ImageButton
        Get
            Return _ImgBtn_Menu2017
        End Get
        Set(value As Global.System.Web.UI.WebControls.ImageButton)
            _ImgBtn_Menu2017 = value
        End Set
    End Property


    Public Property LblRag_Soc() As Global.System.Web.UI.WebControls.Label
        Get
            Return Lbl_Rag_Soc
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            Lbl_Rag_Soc = value
        End Set
    End Property

    Public Property lblFinestraTemporale() As Global.System.Web.UI.WebControls.Label
        Get
            Return lbl_FinestraTemporale
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            lbl_FinestraTemporale = value
        End Set
    End Property

    Public Property LbLUtente() As Global.System.Web.UI.WebControls.Label
        Get
            Return LbL_Utente
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            LbL_Utente = value
        End Set
    End Property

    Public Property LbLUltimoAccesso() As Global.System.Web.UI.WebControls.Label
        Get
            Return LbL_UltimoAccesso
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            LbL_UltimoAccesso = value
        End Set
    End Property

    Public Property UtenteUsername() As String
        Get
            Return _utenteUsername
        End Get
        Set(value As String)
            _utenteUsername = value
        End Set
    End Property

    Public Property CssPlaceHolder As PlaceHolder
        Get
            Return _CssPlaceHolder
        End Get
        Set(value As PlaceHolder)
            _CssPlaceHolder = value
        End Set
    End Property

    Public Property HeaderPlaceHolder As PlaceHolder
        Get
            Return _HeaderPlaceHolder
        End Get
        Set(value As PlaceHolder)
            _HeaderPlaceHolder = value
        End Set
    End Property

    Public Property FooterPlaceHolder As PlaceHolder
        Get
            Return _FooterPlaceHolder
        End Get
        Set(value As PlaceHolder)
            _FooterPlaceHolder = value
        End Set
    End Property

    Public Property sAssistenzaInfo As String
        Get
            Return _sAssistenzaInfo
        End Get
        Set(value As String)
            _sAssistenzaInfo = value
        End Set
    End Property

    Public Property Lbl_ScadenzaPermessi As Label
        Get
            Return _Lbl_ScadenzaPermessi
        End Get
        Set(value As Label)
            _Lbl_ScadenzaPermessi = value
        End Set
    End Property

    Public Property HeaderVersione As String
        Get
            Return _versioneHeader
        End Get
        Set(value As String)
            SetHeaderVersion(value)
        End Set
    End Property

    Public Property SitoOspite As TipiEnumerativi.Enum_SiteRedirector
        Get
            Return _SitoOspite
        End Get
        Set(value As TipiEnumerativi.Enum_SiteRedirector)
            _SitoOspite = value
        End Set
    End Property

    Protected Overrides Sub Inizializza()

        MyBase.inizializza()

        If _CssPlaceHolder Is Nothing Then
            Throw New Exception("AgronicaControlli_2010: Agro Master Page: placeholder per fogli di stile non inizializzato ")
        End If

        If _FooterPlaceHolder Is Nothing Then
            Throw New Exception("AgronicaControlli_2010: Agro Master Page: placeholder per il footer non inizializzato ")
        End If

        Leggi_Versione_Header()

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)

        MyBase.inizializza()

        InizializzaFields()

        'la lettura qui può avvenire solo nelle pagine master, non nella pagina di login, poiché non è inizializzata apposita variabile di sessione.
        If Not HttpContext.Current.Session("ASG_objParametri_Super_Server") Is Nothing Then
            _listaLoghiPersonalizzati =
                LeggiDaSessioneOppureDaConfigSiti("ListaLoghiPersonalizzati", "", agronicacoreparametri_tipoDB.SuperServer)
        End If

        If Me.Page.ToString = "ASP.index_aspx" Then
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        ElseIf Me.Page.ToString = "ASP.login_login_aspx" Then
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        Else
            _LinkGiasBase = MyBase.PATH_GIASBASE
        End If


        ' Imposta il flag se è attivo il nuovo menu
        If Not HttpContext.Current.Session("ASG_objParametri_Server") Is Nothing Then

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
            Dim objParametriSuperServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
            Dim objParametriUtenti As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

            Dim DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametriServer)
            flag_MenuBS_2017 = Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso DTConfigSiti.Rows(0)("valore").ToString.ToLower = "true"

            If flag_MenuBS_2017 Then

                DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017_Config", "", "", objParametriServer)
                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                    config_MenuBS_2017 = JsonConvert.DeserializeObject(DTConfigSiti.Rows(0)("valore").ToString)
                    flag_base_MenuBS_2017 = GetConfigMenuBS2017("base", False)
                    flag_link_MenuBS_2017 = GetConfigMenuBS2017("link", True)
                End If

                ' gestione filtro cambia aziende
                flag_MostraBtnCambiaImpresa = True
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim dtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametriServer)
                If Not dtImpreseVisibili Is Nothing AndAlso dtImpreseVisibili.Rows.Count = 1 Then
                    piva_MenuBS_2017 = dtImpreseVisibili.Rows(0).Item("Piva")
                    flag_MostraBtnCambiaImpresa = False
                Else
                    Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                    Dim dtImprese = objImprese.LeggiPivaTopN(2, objParametriServer) 'datatable non ordinato, non necessario visto che usiamo solo la prima riga 
                    If Not dtImprese Is Nothing AndAlso dtImprese.Rows.Count = 1 Then
                        piva_MenuBS_2017 = dtImprese.Rows(0).Item("Piva")
                        flag_MostraBtnCambiaImpresa = False
                    End If
                End If

            End If

            'Integrazione Matomo -- verifichiamo che esista la configurazione sulla Configurazione_Siti e che non l'utente loggato no nsia superi

            Dim isSuperUser As Boolean = (objParametriUtenti.SuperUserUsername = objParametriUtenti.UtenteUsername)

            If Not isSuperUser Then
                DTConfigSiti = objConfigSiti.Leggi(0, "ConfigMatomo", "", "", objParametriServer)
                flag_Matomo_Enabled = Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0

                Dim result As ConfigMatomo

                'se esiste quantomeno la configurazione
                If flag_Matomo_Enabled Then
                    'verifico che la configurazione registrata abbia i campi corretti
                    result = JsonConvert.DeserializeObject(Of ConfigMatomo)(DTConfigSiti.Rows(0)("valore").ToString)
                    If result IsNot Nothing AndAlso Not String.IsNullOrEmpty(result.Domain) AndAlso Not String.IsNullOrEmpty(result.Container) Then
                        If Not Page.IsPostBack Then
                            Dim script As String = "<script>
                                            var _mtm = window._mtm = window._mtm || [];
                                            var _paq = window._paq = window._paq || [];
                                            _paq.push(['setUserId', '" & objParametriUtenti.UtenteUsername & "']);
                                            _mtm.push({'mtm.startTime': (new Date().getTime()), 'event': 'mtm.Start'});
                                            var d = document, g = d.createElement('script'), s = d.getElementsByTagName('script')[0];
                                            g.async = true; g.src = 'https://" & result.Domain & "/js/container_" & result.Container & ".js';
                                            s.parentNode.insertBefore(g, s);
                                        </script>"

                            Page.ClientScript.RegisterStartupScript(Me.GetType(), "MatomoScript", script, False)
                        End If
                        flag_Matomo_Enabled = True
                    Else
                        flag_Matomo_Enabled = False
                    End If
                End If
            End If

        End If
        'LeggiPersonalizzazioniRegioneUmbria()

        Me.Controls.Add(_ImgBtn_AnnullaTutto)
        Me.Controls.Add(_ImgBtn_Filtro)
        Me.Controls.Add(_ImgBtn_Menu2017)

        GetTipoCompilazioneMin()

        MyBase.OnInit(e)
    End Sub

    Private Sub LeggiCustomLoghi()

        Dim objParametriServer As AgronicaCoreParametri = Nothing
        Dim objParametriSuperServer As AgronicaCoreParametri = Nothing

        'andiamo a leggere la configurazione dei loghi, dando precedenza al DB Server, ed eventualmente, se non esiste, sul Super Server
        If paginePreLogin.Contains(Me.Page.ToString) Then
            If HttpContext.Current.Session("ASG_objParametri_Server_IndexAspx") IsNot Nothing Then
                objParametriServer = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server_IndexAspx"))
            End If
        ElseIf HttpContext.Current.Session("ASG_objParametri_Server") IsNot Nothing Then
            objParametriServer = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        End If

        If HttpContext.Current.Session("ASG_objParametri_Super_Server") IsNot Nothing Then
            objParametriSuperServer = New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
        End If

        If objParametriServer IsNot Nothing OrElse objParametriSuperServer IsNot Nothing Then
            CustomLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametriServer, objParametriSuperServer)
        End If
    End Sub


    Private Sub InizializzaFields()

        Dim prefisso As String = ""


        LblTitolo = New Label With {.ID = prefisso & "LblTitolo"}
        LblTitolo.Attributes.Add("style", "text-transform: uppercase; font-size: 14px;")

        lblDescrGeneric = New Label With {.ID = prefisso & "lblDescrGeneric"}

        ImgBtnAnnullaTutto = New ImageButton With {.ID = prefisso & "ImgBtnAnnullaTutto"}
        ControlDisplayNone(ImgBtnAnnullaTutto)

        ImgBtnFiltro = New ImageButton With {.ID = prefisso & "ImgBtnFiltro"}
        ControlDisplayNone(ImgBtnFiltro)

        _ImgBtn_Menu2017 = New ImageButton With {.ID = prefisso & "ImgBtn_Menu2017"}
        ControlDisplayNone(_ImgBtn_Menu2017)

        Lbl_Rag_Soc = New Label With {.ID = prefisso & "Lbl_Rag_Soc"}
        LblRag_Soc.Attributes.Add("style", "font-weight: bold")

        LbL_Utente = New Label With {.ID = prefisso & "LbL_Utente"}
        LbL_Utente.Attributes.Add("style", "font-weight: bold")

        LbL_UltimoAccesso = New Label With {.ID = prefisso & "LbL_UltimoAccesso"}
        LbL_UltimoAccesso.Attributes.Add("style", "font-weight: bold")

        Lbl_ScadenzaPermessi = New Label With {.ID = prefisso & "Lbl_ScadenzaPermessi"}
        Lbl_ScadenzaPermessi.Attributes.Add("style", "font-weight: bold; color: red")

        lbl_FinestraTemporale = New Label With {.ID = prefisso & "lbl_FinestraTemporale"}
        lbl_FinestraTemporale.Attributes.Add("style", "font-weight: bold")
    End Sub

    Private Sub ControlDisplayNone(ByRef control As WebControl)

        If control.Attributes.Item("style") <> Nothing Then
            control.Attributes("style") &= "; display: none"
        Else
            control.Attributes.Add("Style", "display: none")
        End If

    End Sub



    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        Inizializza()


        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        Dim str As New StringBuilder

        str.Append("<div></div>")


        If Not _HeaderPlaceHolder Is Nothing AndAlso flag_MostraHeader = True Then
            Testata()
            Select Case _versioneHeader
                Case "2022"
                    Dim idSezione As Integer = -1


                    If IsNothing(HttpContext.Current.Request.QueryString.Get("idBC")) Then
                        idSezione = -1
                    Else
                        Try
                            Dim idSezioneSTR As String = Stringa_Decodifica(HttpContext.Current.Request.QueryString.Get("idBC"), AgroKey_EncoderDecoder)
                            If Not String.IsNullOrEmpty(idSezioneSTR) Then
                                idSezione = IIf(idSezioneSTR Is Nothing, -1, CType(idSezioneSTR, Integer))
                            End If
                        Catch ex As Exception
                            idSezione = -1
                        End Try
                    End If

                    If idSezione = -1 AndAlso Me.LblTitolo.Text IsNot Nothing AndAlso Me.LblTitolo.Text <> "" Then
                        If testoTitoloSessione_MenuBS_2017 <> "" Then
                            testoTitoloSessione_MenuBS_2022 = testoTitoloSessione_MenuBS_2022 & " (" & Me.LblTitolo.Text & ")"
                        Else
                            testoTitoloSessione_MenuBS_2022 = "~" & Me.LblTitolo.Text.Replace("<br>", " ")
                        End If
                    End If

                    Dim pathLogo = _LinkGiasBase & "agronica/Styles/images/dashboard/navbar/gias_icon_256x256.png"
                    'If personalizzazioniRegioneUmbria <> "" Then
                    '    pathLogo = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_gari_" & personalizzazioniRegioneUmbria & ".png"
                    'End If

                    If CustomLoghi IsNot Nothing AndAlso Not String.IsNullOrEmpty(CustomLoghi.Logo_Navbar_PATH) Then
                        pathLogo = _LinkGiasBase & CustomLoghi.Logo_Navbar_PATH
                    End If

                    Dim parametriHeader As New ParametriHeader2022 With {
                        .idSezione = idSezione,
                        .idSezioneSessione = idSezione_MenuBS_2022,
                        .funzioneIndietro = azione_indietro,
                        .idIndietro = id_Azione_indietro,
                        .testoBreadcrum = testoTitoloSessione_MenuBS_2022,
                        .objParametri_Super_Server = HttpContext.Current.Session("ASG_objParametri_Super_Server"),
                        .objParametri_Server = HttpContext.Current.Session("ASG_objParametri_Server"),
                        .objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti"),
                        .sitoOspite = _SitoOspite,
                        .PATHPATH_GIASBASE = Me.PATH_GIASBASE,
                        .pathLogo = pathLogo
                    }
                    str.Append(Load_And_Render_Header_2022(parametriHeader))
            End Select
        End If
        writer.Write(str.ToString)

        If flag_MostraFooter Then
            Select Case _versioneHeader
                Case "2022"
                Case Else
                    Footer()
            End Select
        End If

        MyBase.Render(writer)

    End Sub


    Private Sub Footer()

        Dim panelFooter As New Panel With {.ID = "footer"}

        Dim AFooter As String = "AgronicaFooter"
        If _paginaOspite = 0 Then
            AFooter = ""
        End If
        panelFooter.CssClass = AFooter

        Dim panelFooterContainer As New Panel With {.CssClass = "container " & AFooter}

        Dim panelFooterRow As New Panel With {.CssClass = "row"}

        Dim panelFooterLogo As New Panel
        Dim imgLogo As New Image

        Dim panelFooterAssistenza As New Panel
        panelFooterAssistenza.Attributes.Add("style", "text-align:right")

        ' aggiunto parametro nel web.config per visualizzare il nuovo footer anche in fase di login
        Dim footer_MenuBS = IsNothing(ConfigurationManager.AppSettings("footerMenuBS")) OrElse ConfigurationManager.AppSettings("footerMenuBS") = True

        If footer_MenuBS Or flag_MenuBS_2017 Then

            Dim marginTop = IIf(flag_MenuBS_2017, 38, 36)
            panelFooter.Attributes.Add("style", "padding: 0 !important;")
            panelFooterContainer.Attributes.Add("style", "margin-top:" & marginTop & "px;padding: 2px !important;")
            panelFooterLogo.CssClass = "col-xs-6 col-6"

            'If personalizzazioniRegioneUmbria <> "" Then
            '    imgLogo.ImageUrl = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_umbria_small_" & personalizzazioniRegioneUmbria & ".png"
            'Else
            '    imgLogo.ImageUrl = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_agronica_small.png"
            'End If

            If CustomLoghi IsNot Nothing Then
                If CustomLoghi.NascondiFooter IsNot Nothing AndAlso CustomLoghi.NascondiFooter Then
                    'Il logo non deve essere visibile, quindi non faccio nulla
                Else
                    If Not String.IsNullOrEmpty(CustomLoghi.Logo_Footer_PATH) Then
                        imgLogo.ImageUrl = _LinkGiasBase & CustomLoghi.Logo_Footer_PATH
                    Else
                        imgLogo.ImageUrl = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_agronica_small.png"
                    End If
                End If
            Else
                imgLogo.ImageUrl = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_agronica_small.png"
            End If

            imgLogo.Attributes.Add("style", "margin:5px;")
            panelFooterAssistenza.CssClass = "col-xs-6 col-6"

            ' gestione icone assistenza
            If Not String.IsNullOrEmpty(sAssistenzaInfo) Then

                Dim telAssistenza = ""
                Dim emailAssistenza = ""
                If sAssistenzaInfo.IndexOf("callto:") <> -1 Then
                    telAssistenza = sAssistenzaInfo.Substring(sAssistenzaInfo.IndexOf("callto:") + 7)
                    telAssistenza = telAssistenza.Substring(0, telAssistenza.IndexOf(""""))
                End If
                If sAssistenzaInfo.IndexOf("mailto:") <> -1 Then
                    emailAssistenza = sAssistenzaInfo.Substring(sAssistenzaInfo.IndexOf("mailto:") + 7)
                    emailAssistenza = emailAssistenza.Substring(0, emailAssistenza.IndexOf(""""))
                End If
                ' verificare se usare come default per agronica
                'If String.IsNullOrEmpty(telAssistenza) AndAlso emailAssistenza.IndexOf("@agronica.it") > 0 Then
                '    Then telAssistenza = "0547 632933"
                'End If
                Dim iconaTelUrl = _LinkGiasBase & "agronica/AB_Immagini/Icone32/tel_icon_32.png"
                Dim iconaEmailUrl = _LinkGiasBase & "agronica/AB_Immagini/Icone32/email_icon_32.png"
                Dim imageTel = "<a href='callto:" & telAssistenza & "'><img title='" & telAssistenza & "' src='" & iconaTelUrl & "' style='margin:5px;'></a>"
                '11/01/2024 Giulia: si è deciso di eliminare l'icona del telefono per tutti (sia con grafica nuova che vecchia)
                imageTel = ""

                Dim imageEmail = "<a href='mailto:" & emailAssistenza & "'><img title='" & emailAssistenza & "' src='" & iconaEmailUrl & "' style='margin:5px;cursor:pointer;'></a>"
                panelFooterAssistenza.Controls.Add(New LiteralControl("<h5>" & My.Resources.AgronicaControlli_2010.Assistenza & " " & imageTel & imageEmail & "</h5>"))

            End If

        Else

            panelFooter.Attributes.Add("style", "height: 56px !important;")
            panelFooterLogo.CssClass = "col-lg-9 col-md-9 col-sm-8 nopadding"
            imgLogo.ImageUrl = _LinkGiasBase & "agronica/AB_Immagini/logo/logo_100.png"
            imgLogo.Attributes.Add("style", "height: 35px")
            imgLogo.CssClass = "footer_logo"
            panelFooterAssistenza.CssClass = "col-lg-3 col-md-3 col-sm-4 nopadding"
            panelFooterAssistenza.Controls.Add(New LiteralControl("<h5>" & My.Resources.AgronicaControlli_2010.Assistenza & " <i class=""fa fa-info-circle""></i></h5>"))
            panelFooterAssistenza.Controls.Add(New LiteralControl(sAssistenzaInfo))

        End If

        'Grilli 15/05/2019 Aggiungo dati nascosti con nome frontend per distinguere in caso di bilanciatore di carico (es Coldiretti)
        Dim panelDatiServer As New Panel()
        panelDatiServer.Controls.Add(New LiteralControl("<input type=""hidden"" id=""hdDatiServer"" value=""Nome Frontend:" & Environment.MachineName & """>"))


        panelFooterLogo.Controls.Add(imgLogo)

        panelFooterRow.Controls.Add(panelFooterLogo)
        panelFooterRow.Controls.Add(panelFooterAssistenza)
        panelFooterRow.Controls.Add(panelDatiServer)

        panelFooterContainer.Controls.Add(panelFooterRow)
        panelFooter.Controls.Add(panelFooterContainer)


        _FooterPlaceHolder.Controls.Add(panelFooter)


    End Sub

    Private Sub TestataMessaggioScadenzaPermessi()

        Dim objParametri_Utenti As AgronicaCoreParametri = Nothing
        Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente

        If IsNothing(HttpContext.Current) OrElse IsNothing(HttpContext.Current.Session) Then
            Exit Sub
        End If

        'chiaramente deve esistere l'oggetto per leggere i parametri utente...
        If HttpContext.Current.Session("ASG_objParametri_Utenti") Is Nothing Then
            Exit Sub
        End If

        objParametri_Utenti = HttpContext.Current.Session("ASG_objParametri_Utenti")
        Dim utentiPermessi As Messaggio_Utente_Permessi = auK.Verifica_Validita_Permessi_E_Chiave_Licenza(objParametri_Utenti)

        If IsNothing(utentiPermessi) Then
            Exit Sub
        End If

        Me.Lbl_ScadenzaPermessi.Visible = False
        Me.Lbl_ScadenzaPermessi.Text = String.Empty
        If Not String.IsNullOrEmpty(utentiPermessi.Messaggio) Then
            Me.Lbl_ScadenzaPermessi.Visible = True
            Me.Lbl_ScadenzaPermessi.Text = utentiPermessi.Messaggio
        End If

    End Sub

    Private Sub Testata()


        Dim paramSessioneObjParametriValue As String = ""

        If paginePreLogin.Contains(Me.Page.ToString) Then
            paramSessioneObjParametriValue = "ASG_objParametri_Server_IndexAspx"
        End If

        'rileggo in fase di render del controllo, poiché nella pagina di login non è disponibile apposita variabile di sessione.
        _listaLoghiPersonalizzati =
            LeggiDaSessioneOppureDaConfigSiti("ListaLoghiPersonalizzati", "", agronicacoreparametri_tipoDB.Server, paramSessioneObjParametriValue:=paramSessioneObjParametriValue)

        If _listaLoghiPersonalizzati = "" Then
            _listaLoghiPersonalizzati = LeggiDaSessioneOppureDaConfigSiti("ListaLoghiPersonalizzati", "", agronicacoreparametri_tipoDB.SuperServer)
        End If

        Select Case _paginaOspite

            Case 0 'index

                'per compatibilità..
                _HeaderPlaceHolder.Controls.Add(_ImgBtn_Filtro)
                _HeaderPlaceHolder.Controls.Add(_ImgBtn_AnnullaTutto)
                _HeaderPlaceHolder.Controls.Add(_ImgBtn_Menu2017)



                Dim stileAgronica As String
                Dim stilePersonalizzati As String
                Dim logoPersonale As String = ""
                Dim NascondiLogoAgronica As Boolean = False

                If _listaLoghiPersonalizzati = "" Then
                    stileAgronica = "margin-right: 40px; margin-left: 40px; "
                    stilePersonalizzati = "width: 79px;"
                Else

                    Dim pathLogoPers As String


                    If _listaLoghiPersonalizzati.Contains("{") Then
                        Dim o1 As JObject = JObject.Parse(_listaLoghiPersonalizzati)
                        pathLogoPers = CStr(o1("path"))
                        NascondiLogoAgronica = CBool(o1("NascondiLogoAgronica"))
                        stilePersonalizzati = CStr(o1("StilePersonalizzati"))
                    Else
                        pathLogoPers = PathLoghiPersonalizzati.FirstOrDefault
                        stilePersonalizzati = "width: 79px;"
                    End If


                    stileAgronica = "width: 225px; margin-left: 25px"
                    logoPersonale = " <img alt = """" src=""" & pathLogoPers & """ style=""" & stilePersonalizzati & """>"

                End If

                Dim a As String = ""
                If NascondiLogoAgronica Then
                    a = "<h1 Class=""text-center login-title"">" &
                        logoPersonale &
                        " </h1>"
                Else

                    'a = "<h1 Class=""text-center login-title"">" &
                    '    " <img alt = """" src=""" & PathLogoGiasOnlineGrande & """ style=""" & stileAgronica & """>" &
                    '    logoPersonale &
                    '    " </h1>"

                    a = "<h1 Class=""text-center login-title"" style=""margin-top:40px; margin-bottom:40px;"">" &
                        " <img alt = """" src=""" & PathLogoGiasLogin & """ style=""max-width: 100%;"">" &
                        logoPersonale &
                        " </h1>"
                End If

                _HeaderPlaceHolder.Controls.Add(New LiteralControl(a))



            Case 1 'master


                'la lettura qui può avvenire solo nelle pagine master, non nella pagina di login, poiché non è inizializzata apposita variabile di sessione.
                If Not HttpContext.Current.Session("ASG_objParametri_Utenti") Is Nothing Then
                    TestataMessaggioScadenzaPermessi()
                End If


                'RIGA
                Dim panelRigaTitolo As New Panel
                panelRigaTitolo.CssClass = "navbar bg-primary agroTestataPersonalizzata"
                panelRigaTitolo.Attributes.Add("role", "navigation")

                If flag_MenuBS_2017 Then
                    panelRigaTitolo.Attributes.Add("style", "border-radius: 0px; border: 0px; margin-bottom: 0px; padding: 0px;")
                Else
                    panelRigaTitolo.Attributes.Add("style", "margin-bottom: 11px")
                End If

                Dim logoAgronica = ""
                If CustomLoghi IsNot Nothing AndAlso Not String.IsNullOrEmpty(CustomLoghi.Logo_Small_PATH) Then
                    logoAgronica = _LinkGiasBase & CustomLoghi.Logo_Small_PATH
                Else
                    logoAgronica = _LinkGiasBase & "agronica/AB_Immagini/logo/" & IIf(flag_MenuBS_2017, "logo_gias_small.png", "logo_small.png")
                End If

                Dim panelLogoAgronica As Panel = PanelLogoGenera("LogoAgronica", logoAgronica, "margin: 5px;", "col-xs-pull-5")

                'da gestire più di un logo
                Dim colXsPull As String = "col-xs-pull-3 "
                Dim panelLogoPersonalizzati As Panel = Nothing
                If _listaLoghiPersonalizzati <> "" Then
                    If _listaLoghiPersonalizzati.Contains("{") Then
                        Dim o1 As JObject = JObject.Parse(_listaLoghiPersonalizzati)
                        Dim pathLogoPers As String = CStr(o1("path"))
                        Dim MostraNellaTestata As Boolean = CBool(o1("MostraNellaTestata"))
                        If MostraNellaTestata Then
                            panelLogoPersonalizzati = PanelLogoGenera("ListaLoghiPersonalizzati", _BasePath & pathLogoPers, "margin: 5px;", "col-xs-pull-4")
                            colXsPull = "" '"col-xs-pull-1 "
                        End If
                    Else
                        panelLogoPersonalizzati = PanelLogoGenera("ListaLoghiPersonalizzati", _BasePath & _listaLoghiPersonalizzati, "margin: 5px;", "col-xs-pull-4")
                        colXsPull = "" '"col-xs-pull-1 "
                    End If
                End If

                'TITOLO
                Dim panelTitolo As New Panel
                panelTitolo.ID = "panelTitolo"
                panelTitolo.CssClass = "col-lg-3 col-md-3 col-sm-3 col-xs-6 col-6" & colXsPull & "col-sm-pull-2 col-md-pull-2 col-lg-pull-2 box-logo nopadding"

                'Azienda ed altro..
                Dim xPullAzieda As String = "col-lg-pull-2 col-sm-pull-2"
                If panelLogoPersonalizzati Is Nothing Then
                    xPullAzieda = "col-lg-pull-2 col-sm-pull-2"
                End If
                Dim panelAziendaEtAl As New Panel
                panelAziendaEtAl.ID = "panelAziendaEtAl"
                panelAziendaEtAl.CssClass = "col-lg-4 col-md-4 col-sm-4 col-xs-12 " & xPullAzieda & " nopadding"

                If flag_MenuBS_2017 Then

                    ' Loghi
                    panelLogoAgronica.Attributes.Add("style", "min-width:100px;")
                    If panelLogoPersonalizzati IsNot Nothing Then
                        panelLogoAgronica.CssClass = "col-xs-1 nopadding"
                        panelLogoPersonalizzati.CssClass = "col-xs-1 nopadding"
                    Else
                        panelLogoAgronica.CssClass = "col-xs-2 nopadding"
                    End If

                    ' Utente
                    panelTitolo.CssClass = "col-sm-4 col-xs-6 col-6"
                    panelTitolo.Attributes.Add("style", "padding:5px;")
                    AziendaEtAl_Utente(panelTitolo)
                    AziendaEtAl_ScadenzaPermessi(panelTitolo)
                    AziendaEtAl_Visibilità(panelTitolo)
                    AziendaEtAl_UltimoAccesso(panelTitolo)

                    ' Azienda
                    panelAziendaEtAl.CssClass = "col-sm-4 col-xs-12"
                    panelAziendaEtAl.Attributes.Add("style", "padding:5px;")
                    AziendaEtAl_Azienda(panelAziendaEtAl)

                Else
                    panelTitolo.Controls.Add(LblTitolo)
                    AziendaEtAl_Utente(panelAziendaEtAl)
                    AziendaEtAl_ScadenzaPermessi(panelAziendaEtAl)
                    AziendaEtAl_Visibilità(panelAziendaEtAl)
                    AziendaEtAl_Azienda(panelAziendaEtAl)
                End If

                'Pulsante Menu bs
                Dim panelPulsanteBS2017 As New Panel
                panelPulsanteBS2017.ID = "panelPulsanteBS2017"
                panelPulsanteBS2017.CssClass = "col-lg-1 col-md-12 col-sm-12 col-xs-12 nopadding pull-right"

                'Azioni(aggiunte in testa)
                Dim div_master_azioni As New Panel
                div_master_azioni.ID = "div_master_azioni"
                div_master_azioni.CssClass = "col-lg-2 col-md-2 col-sm-2 col-xs-5 col-sm-push-9 col-xs-push-7 col-md-push-10 col-lg-push-10 text-right nopadding"

                div_master_azioni.Controls.Add(_ImgBtn_AnnullaTutto)
                div_master_azioni.Controls.Add(ImgBtnFiltro)
                div_master_azioni.Controls.Add(_ImgBtn_Menu2017)



                If (flag_pag_AnalisiProgetti Or flag_pag_GestioneCompletaCdG Or flag_pag_ScaricoTempi Or flag_pag_GestioneCosti Or flag_pag_LavorazioniFF Or flag_pag_CampionamentoConferito Or flag_pag_MenuAgenda Or flag_pag_MenuAnagrafica Or flag_pag_Anagrafica Or flag_pag_Operazione Or flag_pag_Scadenzario Or flag_pag_Visite Or flag_pag_DocContabile Or flag_MostraBtnIndietro) And Not flag_pag_BootstrapModal Then

                    azione_indietro = "Azione_Indietro();"
                    id_Azione_indietro = "azioni_anagrafica"

                    If flag_pag_LavorazioniFF Then
                        azione_indietro = "Azione_Indietro_LavorazioniFF();"
                        id_Azione_indietro = "azioni_LavorazioniFF"
                    End If

                    If flag_pag_CampionamentoConferito Then
                        azione_indietro = "Azione_Indietro_CampionamentoConferito();"
                        id_Azione_indietro = "azioni_CampionamentoConferito"
                    End If

                    If flag_pag_AnalisiProgetti Then
                        azione_indietro = "Azione_Indietro_AnalisiProgetti();"
                        id_Azione_indietro = "azioni_AnalisiProgetti"
                    End If

                    If flag_pag_GestioneCompletaCdG Then
                        azione_indietro = "Azione_Indietro_GestioneCompletaCdG();"
                        id_Azione_indietro = "azioni_GestioneCompletaCdG"
                    End If

                    If flag_pag_ScaricoTempi Then
                        azione_indietro = "Azione_Indietro_ScaricoTempi();"
                        id_Azione_indietro = "azioni_ScaricoTempi"
                    End If

                    If flag_pag_GestioneCosti Then
                        azione_indietro = "Azione_Indietro_GestioneCosti();"
                        id_Azione_indietro = "azioni_GestioneCosti"
                    End If

                    If flag_pag_Scadenzario Then
                        azione_indietro = "Azione_Indietro_Scadenzario();"
                        id_Azione_indietro = "azioni_Scadenzario"
                    End If

                    If flag_pag_DocContabile Then
                        azione_indietro = "Azione_Indietro_DocContabile();"
                        id_Azione_indietro = "azioni_DocContabile"
                    End If

                End If

                If flag_MenuBS_2017 Then

                    Dim ctrlMenuBS2017 As String = "<div id=""header_azioni_menu"" style=""text-align:right;margin-right:5px;"">"

                    If flag_pag_MenuBS_2017 Then
                        'ctrlMenuBS2017 &= "<a href='#' onclick='GoToMenuPrecedente();'><i title='Menu Precedente' id='azione_indietro_icon' class='fa fa-undo' style='color:white;margin:5px;font-size:30px;'></i></a>"
                        ctrlMenuBS2017 &= "<a href='#' onclick='Azione_Esci_Da_Gias();'><i title='" & My.Resources.AgronicaControlli_2010.Esci & "' class='fa fa-sign-out' style='color:white;margin:5px;font-size:36px;'></i></a>"
                    Else
                        If azione_indietro <> "" Then
                            ctrlMenuBS2017 &= "<a href='#' onclick='" & azione_indietro & "'><i title='" & My.Resources.AgronicaControlli_2010.Indietro & "' id='azione_indietro_icon' class='fa fa-reply' style='color:white;margin:5px;font-size:24px;'></i></a>"
                        End If
                        If Not String.IsNullOrEmpty(url_MenuBS_2017) Then
                            Dim url_Home = url_MenuBS_2017 & IIf(ws_MenuBS_2017, "&PivaSelezionata=" & piva_MenuBS_2017, "")
                            ctrlMenuBS2017 &= "<a href=""" & url_Home & """><i title=""Home"" class=""fa fa-home"" style=""color:white;margin:5px;font-size:36px;""></i></a>"
                        End If
                    End If

                    ctrlMenuBS2017 &= "</div>"

                    div_master_azioni.CssClass = "pull-right text-right nopadding"

                    div_master_azioni.Controls.Add(New LiteralControl(ctrlMenuBS2017))

                ElseIf azione_indietro <> "" Then

                    Dim strCtrlIndietro As String = ""

                    strCtrlIndietro &= "<div id = """ & id_Azione_indietro & """ style=""margin-right: 10px; color: white; " & """>"

                    If flag_MostraBtnEsci Then
                        strCtrlIndietro &= " <h5 onclick='Azione_Esci_Da_Gias();' style='cursor: pointer;'><i class='fa fa-sign-out'></i><span class='navi_item'>" & My.Resources.AgronicaControlli_2010.Esci & "</span></h5>"
                    End If

                    strCtrlIndietro &= " <h4 onclick = '" & azione_indietro & "' style='cursor: pointer; color: white'><i class='fa fa-reply'></i><span class='navi_item'>" & My.Resources.AgronicaControlli_2010.Indietro & "</span></h4>"

                    If flag_pag_MenuAgenda OrElse flag_pag_GestioneMagazziniBS OrElse flag_MostraBtnCambiaImpresa Then
                        strCtrlIndietro &= " <h5 onclick=""Azione_Cambia_Impresa();"" style=""cursor: pointer;""><i class=""fa fa-search""></i><span class=""navi_item"">Cambia Impresa</span></h5>"
                    End If

                    strCtrlIndietro &= "</div>"

                    Dim ctrlIndietro As New LiteralControl(strCtrlIndietro)

                    div_master_azioni.Controls.Add(ctrlIndietro)

                End If

                'Aggiunta di tutto...
                panelRigaTitolo.Controls.Add(div_master_azioni)
                panelRigaTitolo.Controls.Add(panelLogoAgronica)

                If Not panelLogoPersonalizzati Is Nothing Then
                    panelRigaTitolo.Controls.Add(panelLogoPersonalizzati)
                End If

                panelRigaTitolo.Controls.Add(panelTitolo)
                panelRigaTitolo.Controls.Add(panelAziendaEtAl)
                'panelRigaTitolo.Controls.Add(panelPulsanteBS2017)

                Dim panelContainer As New Panel
                panelContainer.ID = "panelContainer"
                panelContainer.CssClass = "container"

                'panelContainer.Controls.Add(panelRigaTitolo)
                Dim objParametriSuperServer = HttpContext.Current.Session("ASG_objParametri_Super_Server")
                Dim objParametriServer = HttpContext.Current.Session("ASG_objParametri_Server")


                _versioneHeader = apiController.VersioneHeader
                'Aggiunta della riga..
                Select Case _versioneHeader
                    Case "2022"
                        panelRigaTitolo.CssClass = "hidePanelRigaTitolo"
                        _HeaderPlaceHolder.Controls.Add(panelRigaTitolo)
                    Case Else
                        _HeaderPlaceHolder.Controls.Add(panelRigaTitolo)
                End Select


                'Aggiunta riga titolo
                If flag_MenuBS_2017 Then

                    ' Imposta il titolo partendo dalla variabile di sessione
                    If HttpContext.Current.Session.Item("ASG_MenuBS_2017") IsNot Nothing Then
                        Dim s As String = CType(HttpContext.Current.Session("ASG_MenuBS_2017"), String)
                        Dim variabileSessioneMenuTitolo As JObject = JObject.Parse(s)
                        Dim testo = variabileSessioneMenuTitolo("testo").ToString
                        testoTitoloSessione_MenuBS_2022 = testo
                        Dim colore = variabileSessioneMenuTitolo("colore").ToString
                        Dim classeCSS = variabileSessioneMenuTitolo("classeCSS").ToString
                        Dim IDSezione = variabileSessioneMenuTitolo("IDSezione").ToString
                        idSezione_MenuBS_2022 = If(Not IsNothing(IDSezione) And IDSezione <> "", CInt(IDSezione), 0)
                        Dim IDSezionePadre = variabileSessioneMenuTitolo("IDSezionePadre").ToString
                        Dim aziendaRichiesta = variabileSessioneMenuTitolo("aziendaRichiesta").ToString
                        Dim configurazione = variabileSessioneMenuTitolo("configurazione").ToString
                        SetTitoloMenuBS2017(testo, colore, classeCSS, IDSezione, IDSezionePadre, aziendaRichiesta, configurazione)
                    End If

                    Dim PuntoInterrogativo As String = IIf(GiasVersioneCorrente.StartsWith("?"), "", "?")

                    Dim cssMenuBS2017 = New StringBuilder
                    If flag_base_MenuBS_2017 Then
                        cssMenuBS2017.Append("<link href='" & _LinkGiasBase & "agronica/Scripts/AgronicaControlli_2010/AgroMasterPage/MenuBS_2017.css" & PuntoInterrogativo & _GiasVersioneCorrente & "' type='text/css' rel='stylesheet' />")
                    Else
                        Dim style = GetConfigMenuBS2017("style", "border: solid 2px rgba(0,0,0,0.21) !important;")
                        cssMenuBS2017.Append("<style type='text/css'>")
                        cssMenuBS2017.Append(".buttonpreferiti { color: black; background-color: #aeaeae; text-transform: none; text-align: center !important; line-height: 38px !important; border-radius: 10px !important; " & style & " width: 100% !important; height: 40px !important; max-height: 40px !important; min-height: 40px !important; font-weight: bold !important; font-size: 16px; margin-bottom: 0px; padding: 0px !important;} ")
                        cssMenuBS2017.Append(".buttonpreferiti:hover { color: white;} ")
                        cssMenuBS2017.Append(".buttonpreferitielementi { color: black; text-transform: none; text-align: center !important; line-height: 38px !important; border-radius: 10px !important; " & style & " width: 100%; height: 40px !important; min-height: 40px !important; font-weight: bold !important; font-size: 16px !important; padding: 0px !important; margin-top: 5px; margin-bottom: 0px !important;} ")
                        cssMenuBS2017.Append(".buttonpreferitielementi:hover { color: white;} ")
                        cssMenuBS2017.Append(".titolo_sezione { color: white; text-align: center !important; line-height: 40px !important; vertical-align: central !important; border-bottom: solid 1px !important; border-top: solid 1px !important; height: 40px !important; max-height: 40px !important; min-height: 40px !important; font-weight: bold !important; font-size: 16px !important; display: none;} ")
                        cssMenuBS2017.Append(".ricercavocemenu { background-color: white; border-radius: 10px; " & style & " width: 100%; height: 40px; color: black; text-align: center; margin-top: 5px; font-size: 16px; font-weight: normal !important;} ")
                        cssMenuBS2017.Append(".buttonmacrocategoria { text-transform: none; white-space: normal; color: black; text-align: center; " & style & " width: 100%; height: 50px; font-weight: bold !important; margin-top: 5px; margin-bottom: 0px; font-size: 16px; min-height: 60px;} ")
                        cssMenuBS2017.Append(".buttonmacrocategoria:hover { color: white;} ")
                        cssMenuBS2017.Append(".buttonmenu { color: black; white-space: normal; text-transform: none; text-align: center; border-radius: 10px; " & style & " width: 100%; height: 100px; font-weight: bold !important; margin-bottom: 0px;  margin-top: 5px; font-size: 18px; min-height: 40px !important;} ")
                        cssMenuBS2017.Append(".buttonmenu:hover { color: white;} ")
                        cssMenuBS2017.Append(".widgetallarmi { color: black; text-align: center; border-radius: 10px; " & style & " font-size: 24px; font-weight: bold !important; width: 100px; height: 80px; margin-right: 5px; margin-bottom: 0px; margin-top: 10px; min-height: 40px;} ")
                        cssMenuBS2017.Append(".widgetallarmi:hover { color: white;} ")
                        cssMenuBS2017.Append(".widgetmeteo { background-color: white; width: 100px; height: 100px; border-radius: 10px; " & style & "} ")
                        cssMenuBS2017.Append(".widgetemergenza { background-color: transparent; height: 100px; border-radius: 10px; " & style & "} ")
                        cssMenuBS2017.Append(".iconmenusezioni { color: white;} ")
                        cssMenuBS2017.Append(".iconmenusezioni:hover { color: #cccccc !important;} ")
                        cssMenuBS2017.Append(".iconprofitosan { content:url('../AB_Immagini/icone24/profitosan2.png'); } ")
                        cssMenuBS2017.Append("#weather-data { " & style & " border-right: solid 0px !important;} ")
                        cssMenuBS2017.Append("#forecast_scroll { " & style & " border-left: solid 0px !important;} ")
                        cssMenuBS2017.Append("</style>")
                    End If
                    _HeaderPlaceHolder.Controls.Add(New LiteralControl(cssMenuBS2017.ToString))

                    If Not flag_pag_MenuBS_2017 Then
                        Dim scriptMenuBS2017 = New StringBuilder
                        scriptMenuBS2017.Append("<script>")
                        scriptMenuBS2017.Append("var menu_corews = " & IIf(ws_MenuBS_2017, "true", "false") & ";")
                        scriptMenuBS2017.Append("var menu_sezioni = false;")
                        scriptMenuBS2017.Append("var menu_preferiti = false;")
                        scriptMenuBS2017.Append("var menu_url = '" & url_MenuBS_2017.Replace(vbCrLf, "") & "';")
                        scriptMenuBS2017.Append("var menu_piva = '" & piva_MenuBS_2017 & "';")
                        scriptMenuBS2017.Append("var rootUrl = menu_corews ? '" & _BasePath & "/AgronicaAgenda/' : '" & ResolveClientUrl("~") & "';")
                        scriptMenuBS2017.Append("var pathMenuWS = menu_corews ? pathCoreWS + 'AgronicaCoreUtility/Gestione_Menu.asmx' : rootUrl + 'Menu/MenuBS_2017.aspx';")
                        If Not flag_base_MenuBS_2017 Then
                            scriptMenuBS2017.Append("function alertImprese(lenFiltro) { $(""#cerca_aziende"").empty(); var html = ""<div class='alert alert-info' style='font-size:12px; line-height:0px; height:30px; border-radius:10px; border: solid 2px rgba(0,0,0,0.21);'>Inserire almeno "" + lenFiltro + "" caratteri</div>""; $(""#cerca_aziende"").append('<li style=""margin-top: 5px;"">' + html + '</li>'); }")
                            scriptMenuBS2017.Append("function leggiImprese(ricerca, idSezione) { if (ricerca === """") { $(""#cerca_aziende"").empty(); } else { var parametri = { ricerca: ricerca, idSezione: idSezione }; if (menu_corews) { parametri.objP_server = objP_server; parametri.objP_utenti = objP_utenti; } ajaxAgronica(pathMenuWS + ""/LeggiImprese"", JSON.stringify(parametri), function (risposta) { $(""#cerca_aziende"").empty(); for (var i = 0; i < risposta.RispostaStringa.length; i++) { var html = risposta.RispostaStringa[i].Contenutohtml; $(""#cerca_aziende"").append('<li>' + html + '</li>'); } }, null); } }")
                            scriptMenuBS2017.Append("function leggiSezioni(idSezione, idSezionePadre) { var parametri = { IDTipoSezione: 2, IDSezionePadre: idSezionePadre, Ricerca: """", Preferiti: idSezionePadre == 0 ? ""true"" : ""false"" }; if (menu_corews) { parametri.objP_server = objP_server; parametri.objP_utenti = objP_utenti; } ajaxAgronica(pathMenuWS + ""/LeggiSezioni"", JSON.stringify(parametri), function (risposta) { $(""#menu_sezioni"").empty(); for (var i = 0; i < risposta.RispostaStringa.length; i++) { var html = risposta.RispostaStringa[i].Contenutohtml; if (html.includes(""aria-idSezione='"" + idSezione + ""'"")) { html = html.replace(""style='"", ""style='color:white;""); } $(""#menu_sezioni"").append('<li>' + html + '</li>'); } }, null); }")
                            scriptMenuBS2017.Append("function menuSezioni(event, idSezione, idSezionePadre) { if (!menu_sezioni || !$(""#menu_sezioni"").is("":visible"")) { menu_preferiti = false; menu_sezioni = true; $(""#menu_sezioni"").show(); leggiSezioni(idSezione, idSezionePadre); } else { menu_sezioni = false; $(""#menu_sezioni"").hide(); } event.stopPropagation(); }")
                            scriptMenuBS2017.Append("function menuPreferiti(event, idSezione) { if (!menu_preferiti || !$(""#menu_sezioni"").is("":visible"")) { menu_sezioni = false; menu_preferiti = true; $(""#menu_sezioni"").show(); leggiSezioni(idSezione, 0); } else { menu_preferiti = false; $(""#menu_sezioni"").hide(); } event.stopPropagation();  }")
                            scriptMenuBS2017.Append("function cercaAziende() { if (!$(""#cerca_aziende"").is("":visible"")) { $(""#cerca_aziende"").show(); $(""#cerca_aziende_text"").show(); } else { $(""#cerca_aziende"").hide(); $(""#cerca_aziende_text"").hide(); } }")
                            scriptMenuBS2017.Append("function gestioneMacrosezioneSezione(obj) { var idSezione = $(obj).attr(""aria-idSezione""); var RedirectURL = $(obj).attr(""aria-RedirectURL""); var sitoRichiesto = $(obj).attr(""aria-sitoRichiesto""); var paginaRichiesta = $(obj).attr(""aria-paginaRichiesta""); var aziendaRichiesta = $(obj).attr(""aria-RichiedeAziendaSelezionata""); if (RedirectURL !== """" || (sitoRichiesto !== """" && sitoRichiesto !== ""0"")) { if (aziendaRichiesta !== ""true"" || menu_piva !== """") { if (menu_corews) { window.location = menu_url + ""&IDSezione="" + idSezione; } else { ajaxAgronica(pathMenuWS + ""/salvaTitoloSezioneConGestioneRedirect"", JSON.stringify({ IDSezione: idSezione }), gestioneRedirect, null); } } else { GoToFiltrino(idSezione); } } } ")
                            scriptMenuBS2017.Append("function gestioneCambioImpresa(obj) { var idSezione = $(obj).attr(""aria-idSezione""); var PivaSelezionata = $(obj).attr(""aria-PivaSelezionata""); var AziendaSelezionata = $(obj).attr(""aria-AziendaSelezionata""); if (menu_corews) { window.location = menu_url + ""&IDSezione="" + idSezione + ""&PivaSelezionata="" + PivaSelezionata; } else { ajaxAgronica(pathMenuWS + ""/cambiaImpresaConGestioneRedirect"", JSON.stringify({ Piva: PivaSelezionata, Azienda: AziendaSelezionata, IDSezione: idSezione }), gestioneRedirect, null); } }")
                            scriptMenuBS2017.Append("function gestioneRedirect(risposta) { if (risposta.RispostaOK) { var targetUrl = null; if (risposta.Tipo === ""1"") { targetUrl = risposta.RispostaStringa; if (targetUrl.indexOf(""<script"") !== -1) { $('#menu_script').append(risposta.RispostaStringa); } else { window.open(targetUrl.replace(""../"", rootUrl)); } } else { targetUrl = risposta.RispostaStringa; window.location = targetUrl.replace(""../"", rootUrl); } } }")
                            scriptMenuBS2017.Append("function GoToFiltrino(idSezione) { if (menu_corews) { window.location = menu_url + ""&IDSezione="" + idSezione + ""&FiltroAziende=""; } else { $.ajax({ type: 'POST', url: pathMenuWS + '/GetFiltroAziende', data: '{ ""IDSezione"": ' + idSezione + ' }', contentType: 'application/json; charset=utf-8', cache: false, dataType: 'json', async: true, success: function (r) { var targetUrl = r.d; window.location = targetUrl.replace(""../"", rootUrl); } }); } }")
                            scriptMenuBS2017.Append("function LeggiWorkflow(workflow_cod, servizio_cod) { var parametri = { WWorkflow_Cod: workflow_cod, Servizio_Cod: servizio_cod }; if (menu_corews) { parametri.Piva = menu_piva; parametri.objP_server = objP_server; parametri.objP_utenti = objP_utenti; } ajaxAgronica(pathMenuWS + ""/LeggiWorkflow"", JSON.stringify(parametri), function (risposta) { risp = JSON.parse(risposta.RispostaStringa); var json = JSON.parse(risposta.RispostaStringa); var html_json = ''; if (json.length == 0 ) html_json = 'Nessuna pratica trovata'; else for(var i = 0; i < json.length; i++) { var obj = json[i]; html_json=obj.Descrizione; break; } $(""#titolo_sezione_configurazione"").html(html_json); }, null, null, false); }")
                            scriptMenuBS2017.Append("$(""html"").click(function (e) { if (menu_sezioni && $(""#menu_sezioni"").is("":visible"")) { menu_sezioni = false; $(""#menu_sezioni"").hide(); } if (menu_preferiti && $(""#menu_sezioni"").is("":visible"")) { menu_preferiti = false; $(""#menu_sezioni"").hide(); } });")
                        End If
                        If Not String.IsNullOrEmpty(configurazioneTitoloSessione_MenuBS_2017) Then
                            scriptMenuBS2017.Append(configurazioneTitoloSessione_MenuBS_2017)
                        End If
                        scriptMenuBS2017.Append("</script>")
                        If flag_base_MenuBS_2017 Then
                            scriptMenuBS2017.Append("<script src='" & _LinkGiasBase & "agronica/Scripts/AgronicaControlli_2010/AgroMasterPage/MenuBS_2017.js" & PuntoInterrogativo & _GiasVersioneCorrente & "' type='text/javascript'></script>")
                        End If
                        _FooterPlaceHolder.Controls.Add(New LiteralControl(scriptMenuBS2017.ToString))
                    End If

                    Dim titoloMenuBS2017 = New StringBuilder
                    titoloMenuBS2017.Append("<div id='menu_script'></div><div id='IntestazioneMenuBS2017'>")
                    If flag_titoloSessione_MenuBS_2017 Then
                        titoloMenuBS2017.Append("<div id='titolo_sezione_preferiti' class='titolo_sezione' style='" & coloreTitoloSessione_MenuBS_2017 & "'>")
                        titoloMenuBS2017.Append(menuTitoloSessione_MenuBS_2017 & azioniTitoloSessione_MenuBS_2017 & testoTitoloSessione_MenuBS_2017 & "</div>")
                        If Not String.IsNullOrEmpty(configurazioneTitoloSessione_MenuBS_2017) Then
                            Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))
                            Dim utente = HttpContext.Current.Session("ASG_Utente_Username")
                            Dim servizio = HttpContext.Current.Session("ASG_IdServizio")
                            Dim idSezione = 77

                            Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
                            Dim permessoWorkFlowNew = objPermessi.Controlla_Permessi_Utente(
                                       utente,
                                        servizio,
                                        enum_Security_Attivita.Gestione_Servizi_NEW,
                                        enum_Security_Operazione.Lettura,
                                        Date.Now,
                                        "",
                                        objParametri_Utenti)

                            If permessoWorkFlowNew Then
                                idSezione = 338
                            End If

                            titoloMenuBS2017.AppendFormat("<div class='row' style='background-color:white; text-align:center;'><div class='pull-left' id='titolo_sezione_configurazione'></div><div class='pull-right btn btn-default' onclick='window.location = menu_url + (menu_corews?""&"":""?"") + ""IDSezione={0}"";'>Vai a Gestione Pratiche</div></div>", idSezione)
                        End If
                    ElseIf Not String.IsNullOrEmpty(Lbl_Titolo.Text) Then
                        titoloMenuBS2017.Append("<div id='titolo_sezione_preferiti' class='titolo_sezione' style='background-color:#002F5F;display: block;'>" & Lbl_Titolo.Text.Replace("<br>", " ") & "</div>")
                    Else
                        titoloMenuBS2017.Append("<div id='titolo_sezione_preferiti' class='titolo_sezione'></div>")
                    End If
                    titoloMenuBS2017.Append("</div><br/>")
                    Select Case _versioneHeader
                        Case "2022"
                        Case Else
                            _HeaderPlaceHolder.Controls.Add(New LiteralControl(titoloMenuBS2017.ToString))
                    End Select


                End If

            Case Else

        End Select
    End Sub

    Private Function PanelLogoGenera(ByVal id As String, PercorsoLogo As String, ByVal styleAdd As String, ByVal ColXsPull As String) As Panel
        'LOGO Agronica
        Dim panelLogoAgronica As New Panel
        panelLogoAgronica.ID = "panel" & id
        panelLogoAgronica.CssClass = panelLogoAgronicaCssClass(ColXsPull)

        '<asp:Image ID="Logo" Height="40px" ImageUrl="~/AB_Immagini/logo_small.png" runat="server" meta:resourcekey="LogoResource1"/>  
        Dim imgLogoAgronica As New Image
        imgLogoAgronica.ID = id
        imgLogoAgronica.ImageUrl = ResolveClientUrl(PercorsoLogo)
        imgLogoAgronica.Attributes.Add("style", "height:40px;border-width:0px;" & styleAdd)
        imgLogoAgronica.Attributes.Add("height", "40px")

        panelLogoAgronica.Controls.Add(imgLogoAgronica)
        Return panelLogoAgronica
    End Function

    Private Function panelLogoAgronicaCssClass(ByVal ColXsPull As String) As String

        Dim xCSS = "col-lg-1 col-md-1 col-sm-1 col-xs-1 " & ColXsPull & " col-sm-pull-2 col-md-pull-2 col-lg-pull-2 box-logo nopadding"
        Return xCSS

    End Function

    Private Sub AziendaEtAl_Azienda(ByRef panelAziendaEtAl As Panel)
        Dim lblAziendaEtAl_Azienda As New Label
        lblAziendaEtAl_Azienda.ID = "lblAziendaEtAl_Azienda"
        lblAziendaEtAl_Azienda.Text = My.Resources.AgronicaControlli_2010.Azienda & ": "

        Dim panelAziendaEtAl_Azienda As New Panel
        panelAziendaEtAl_Azienda.ID = "panelAziendaEtAl_Azienda"
        panelAziendaEtAl_Azienda.CssClass = "m_p_1"
        panelAziendaEtAl_Azienda.Controls.Add(lblAziendaEtAl_Azienda)
        panelAziendaEtAl_Azienda.Controls.Add(LblRag_Soc)

        panelAziendaEtAl.Controls.Add(panelAziendaEtAl_Azienda)

    End Sub

    Private Sub AziendaEtAl_Utente(ByRef panelAziendaEtAl As Panel)
        Dim lblAziendaEtAl_Utente As New Label
        lblAziendaEtAl_Utente.ID = "lblAziendaEtAl_Utente"
        lblAziendaEtAl_Utente.Text = My.Resources.AgronicaControlli_2010.Utente & ": "

        Dim panelAziendaEtAl_Utente As New Panel
        panelAziendaEtAl_Utente.ID = "panelAziendaEtAl_Utente"
        panelAziendaEtAl_Utente.CssClass = "m_p_0"
        panelAziendaEtAl_Utente.Controls.Add(lblAziendaEtAl_Utente)
        panelAziendaEtAl_Utente.Controls.Add(LbLUtente)

        panelAziendaEtAl.Controls.Add(panelAziendaEtAl_Utente)
    End Sub


    Private Sub AziendaEtAl_ScadenzaPermessi(ByRef panelAziendaEtAl As Panel)

        If Lbl_ScadenzaPermessi.Text = "" Then
            Exit Sub
        End If

        Dim lblAziendaEtAl_ScadenzaPermessi As New Label
        lblAziendaEtAl_ScadenzaPermessi.ID = "lblAziendaEtAl_ScadenzaPermessi"
        lblAziendaEtAl_ScadenzaPermessi.Text = "Scadenza Permessi: "

        Dim panelAziendaEtAl_ScadenzaPermessi As New Panel
        panelAziendaEtAl_ScadenzaPermessi.ID = "panelAziendaEtAl_ScadenzaPermessi"
        panelAziendaEtAl_ScadenzaPermessi.CssClass = "m_p_0"
        panelAziendaEtAl_ScadenzaPermessi.Controls.Add(lblAziendaEtAl_ScadenzaPermessi)
        panelAziendaEtAl_ScadenzaPermessi.Controls.Add(Lbl_ScadenzaPermessi)

        panelAziendaEtAl.Controls.Add(panelAziendaEtAl_ScadenzaPermessi)
    End Sub

    Private Sub AziendaEtAl_Visibilità(ByRef panelAziendaEtAl As Panel)
        Dim lblAziendaEtAl_Visibilità As New Label
        lblAziendaEtAl_Visibilità.ID = "lblAziendaEtAl_Vsibilità"
        lblAziendaEtAl_Visibilità.Text = My.Resources.AgronicaControlli_2010.Visibilità & ": "

        Dim panelAziendaEtAl_Visibilità As New Panel
        panelAziendaEtAl_Visibilità.ID = "panelAziendaEtAl_Visibilità"
        panelAziendaEtAl_Visibilità.CssClass = "m_p_0"
        panelAziendaEtAl_Visibilità.Controls.Add(lblAziendaEtAl_Visibilità)
        panelAziendaEtAl_Visibilità.Controls.Add(lblFinestraTemporale)

        panelAziendaEtAl.Controls.Add(panelAziendaEtAl_Visibilità)
    End Sub

    Private Sub AziendaEtAl_UltimoAccesso(ByRef panelAziendaEtAl As Panel)
        If Not String.IsNullOrEmpty(LbLUltimoAccesso.Text) Then
            Dim lblAziendaEtAl_UltimoAccesso As New Label
            lblAziendaEtAl_UltimoAccesso.ID = "lblAziendaEtAl_UltimoAccesso"
            lblAziendaEtAl_UltimoAccesso.Text = My.Resources.AgronicaControlli_2010.UltimoAccesso & ": "

            Dim panelAziendaEtAl_UltimoAccesso As New Panel
            panelAziendaEtAl_UltimoAccesso.ID = "panelAziendaEtAl_UltimoAccesso"
            panelAziendaEtAl_UltimoAccesso.CssClass = "m_p_0"
            panelAziendaEtAl_UltimoAccesso.Controls.Add(lblAziendaEtAl_UltimoAccesso)
            panelAziendaEtAl_UltimoAccesso.Controls.Add(LbLUltimoAccesso)
            panelAziendaEtAl.Controls.Add(panelAziendaEtAl_UltimoAccesso)
        End If
    End Sub

    Public Function GetConfigMenuBS2017(ByVal Chiave As String, ByVal DefVal As Object) As Object
        If Not IsNothing(config_MenuBS_2017) Then
            If Not IsNothing(config_MenuBS_2017.Property(Chiave)) Then
                Return config_MenuBS_2017.GetValue(Chiave)
            End If
        End If
        Return DefVal
    End Function

    Public Sub SetHeaderMenuBS2017(ByVal azienda As String, ByVal utente As String, ByVal visibilita As String, ByVal ultimoAccesso As String)
        If flag_MenuBS_2017 Then
            If Not String.IsNullOrEmpty(azienda) Then
                LblRag_Soc.Text = azienda
            End If
            If Not String.IsNullOrEmpty(utente) Then
                LbLUtente.Text = utente
            End If
            If Not String.IsNullOrEmpty(visibilita) Then
                lblFinestraTemporale.Text = visibilita
            End If
            If Not String.IsNullOrEmpty(ultimoAccesso) Then
                LbLUltimoAccesso.Text = ultimoAccesso
                LbLUtente.Text = LbLUtente.Text.Replace(" - Ultimo Accesso: " & ultimoAccesso, "")
            End If
        End If
    End Sub

    Public Sub SetMenuBS2017(ByVal IDSezione As Integer, ByVal Piva As String, ByVal CoreWS As Boolean)
        If flag_MenuBS_2017 Then
            If IDSezione <> 0 Then
                SetTitoloMenuBS2017(IDSezione)
            End If
            If CoreWS Then
                SetURLMenuBS2017(Piva)
            Else
                Dim objWebConfig As New AgroWebConfig
                Dim linkAgenda = objWebConfig.LinkAgronicaAgenda2010
                piva_MenuBS_2017 = Piva
                url_MenuBS_2017 = Replace(linkAgenda, "GestioneRichieste.aspx", "Menu/MenuBS_2017.aspx")
            End If
            ws_MenuBS_2017 = CoreWS
        End If
    End Sub

    Public Sub SetTitoloMenuBS2017(ByVal IDSezione As Integer)

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Utenti"))

        Dim lettureDB As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
        Dim dtLettura As DataTable

        'lo utilizzo quando faccio redirect
        Dim IDSezionePadre As Integer
        Dim testo As String
        Dim colore As String
        Dim classeCSS As String
        Dim sitoRichiesto As Integer
        Dim RedirectURL As String = ""
        Dim paginaRichiesta As String
        Dim aziendaRichiesta As Integer
        Dim tipoAperturaPagina As Integer
        Dim configurazione As String = ""

        dtLettura = lettureDB.LeggiSezione(IDSezione, 2, objParametri_Server, objParametri_Utenti)

        If Not IsNothing(dtLettura) AndAlso dtLettura.Rows.Count > 0 Then

            Dim riga = dtLettura.Rows(0)
            testo = riga.Item("Testo")
            colore = riga.Item("Colore")
            classeCSS = riga.Item("ClasseCSS")
            sitoRichiesto = riga.Item("Enum_SiteRedirector")
            paginaRichiesta = riga.Item("PaginaRichiesta")
            aziendaRichiesta = riga.Item("RichiedeAziendaSelezionata")
            If Not IsDBNull(riga.Item("RedirectURL")) Then
                RedirectURL = riga.Item("RedirectURL")
            End If
            If Not IsDBNull(riga.Item("Enum_TipoAperturaPagina")) Then
                tipoAperturaPagina = riga.Item("Enum_TipoAperturaPagina")
            End If
            If Not IsDBNull(riga.Item("Configurazione")) Then
                configurazione = riga.Item("Configurazione")
            End If

            IDSezione = riga.Item("IDSezione")
            IDSezionePadre = riga.Item("IDSezionePadre")

            If Not IsNothing(IDSezionePadre) Then
                dtLettura = lettureDB.LeggiSezione(IDSezionePadre, 1, objParametri_Server, objParametri_Utenti)
                If Not IsNothing(dtLettura) AndAlso dtLettura.Rows.Count > 0 Then
                    If testo <> dtLettura.Rows(0).Item("Testo") Then
                        testo = dtLettura.Rows(0).Item("Testo") & "_" & testo
                    End If
                End If
            End If

            Dim sezione As New JObject()
            sezione.Add("testo", testo)
            sezione.Add("colore", colore)
            sezione.Add("classeCSS", classeCSS)
            sezione.Add("IDSezione", IDSezione)
            sezione.Add("IDSezionePadre", IDSezionePadre)
            sezione.Add("sitoRichiesto", sitoRichiesto)
            sezione.Add("RedirectURL", RedirectURL)
            sezione.Add("paginaRichiesta", paginaRichiesta)
            sezione.Add("aziendaRichiesta", aziendaRichiesta)
            sezione.Add("tipoAperturaPagina", tipoAperturaPagina)
            sezione.Add("configurazione", configurazione)
            Dim oggetto As String = JsonConvert.SerializeObject(sezione)
            HttpContext.Current.Session("ASG_MenuBS_2017") = oggetto

        End If

    End Sub

    Public Sub SetCustomLoghi()
        LeggiCustomLoghi()
    End Sub

    Private Sub SetTitoloMenuBS2017(ByVal testo As String, ByVal colore As String, ByVal classeCSS As String, ByVal IDSezione As String, ByVal IDSezionePadre As String, ByVal aziendaRichiesta As String, ByVal configurazione As String)

        flag_titoloSessione_MenuBS_2017 = True
        If testo.IndexOf("_") <> -1 Then
            testoTitoloSessione_MenuBS_2017 = "<span class='hidden-xs'>" & testo.Replace("_", " <i class='fa fa-arrow-right'></i></span> ")
        Else
            testoTitoloSessione_MenuBS_2017 = testo
        End If
        coloreTitoloSessione_MenuBS_2017 = "background-color: " + colore + "; display: block;"

        If Not String.IsNullOrEmpty(Lbl_Titolo.Text) AndAlso classeCSS.Contains("lbltitolo") Then
            testoTitoloSessione_MenuBS_2017 &= " (" & Lbl_Titolo.Text.Replace("<br>", " ") & ")"
        End If

        menuTitoloSessione_MenuBS_2017 = ""
        azioniTitoloSessione_MenuBS_2017 = ""
        configurazioneTitoloSessione_MenuBS_2017 = ""

        If Not String.IsNullOrEmpty(IDSezione) AndAlso IDSezione <> "0" Then

            ' menu sezioni / preferiti
            Dim menuSezioniHTML = New StringBuilder
            menuSezioniHTML.Append("<div class='pull-right' style='margin-top:4px;margin-right:10px;'>")
            menuSezioniHTML.Append("<a href='#' onclick='menuPreferiti(event, " & IDSezione & ");'><i title='Preferiti' id='menu_preferiti_icon' class='iconmenusezioni fa fa-star-o' style='cursor:pointer; font-size:30px;'></i></a>")
            menuSezioniHTML.Append("<a href='#' onclick='menuSezioni(event, " & IDSezione & "," & IDSezionePadre & ");'><i title='Menu' id='menu_sezioni_icon' class='iconmenusezioni fa fa-bars' style='cursor:pointer; font-size:30px;'></i></a>")
            menuSezioniHTML.Append("</div><div class='pull-right' style='width:0px;'><nav style='position:relative;top:40px;right:320px;width:400px;height:0px;z-index:99999;'><ul id='menu_sezioni' class='nav' style='display:none;overflow: hidden; line-height:40px !important;'>")
            menuSezioniHTML.Append("</ul></nav></div>")
            menuTitoloSessione_MenuBS_2017 = menuSezioniHTML.ToString

            ' cerca aziende / filtro imprese
            If flag_MostraBtnCambiaImpresa Then
                Dim azioniHTML = New StringBuilder
                azioniHTML.Append("<div class='pull-left' style='margin-top:3px;margin-right:10px;'>")
                Dim azioniRight As Integer = 80
                Dim lenFiltroAziende = GetConfigMenuBS2017("filtroAziende", "3")
                If aziendaRichiesta <> "0" Then
                    azioniHTML.Append("<a href='#' onclick='cercaAziende();'><i title='Cerca Azienda' id='cerca_aziende_icon' class='iconmenusezioni fa fa-search' style='cursor:pointer; font-size:30px;'></i></a>")
                    azioniHTML.Append("<a href='#' onclick='GoToFiltrino(" & IDSezione & ");'><i title='Filtro Aziende' id='cambia_aziende_icon' class='iconmenusezioni fa fa-search-plus' style='cursor:pointer; font-size:30px;'></i></a>")
                End If
                azioniHTML.Append("</div><div class='pull-left' style='width:0px;'><nav style='position:relative;top:40px;right:" & azioniRight & "px;width:400px;height:0px;z-index:99999;'>")
                azioniHTML.Append("<input autocomplete='off' onkeydown='if (event.keyCode == 13) return false;' onkeyup='if (event.keyCode == 13 || this.value.length >= " & lenFiltroAziende & ") leggiImprese(this.value," & IDSezione & "); else alertImprese(" & lenFiltroAziende & ");' type='text' placeholder='Cerca azienda' class='ricercavocemenu' id='cerca_aziende_text' style='display:none;'>")
                azioniHTML.Append("<ul id='cerca_aziende' class='nav' style='display:none;overflow: hidden; line-height:40px !important;'></ul></nav></div>")
                azioniTitoloSessione_MenuBS_2017 = azioniHTML.ToString
            End If

            If Not String.IsNullOrEmpty(configurazione) Then

                Dim jsonConfig = Newtonsoft.Json.JsonConvert.DeserializeObject(configurazione)
                Dim filtro As JArray = jsonConfig.GetValue("FiltrinoListaServiziStati")
                If filtro.Count > 0 Then
                    Dim servizio As JObject = filtro(0)
                    Dim cod_servizio = servizio.GetValue("servizio_cod").ToString
                    Dim cod_workflow = servizio.GetValue("WWorkFlow_Cod").ToString
                    'configurazioneTitoloSessione_MenuBS_2017 = "creaKendoDropDownList('id_workflow', { read: function() {LeggiWorkflow(" & cod_workflow & "," & cod_servizio & ");} }, 'Descrizione', 'Codice');"
                    configurazioneTitoloSessione_MenuBS_2017 = "LeggiWorkflow(" & cod_workflow & "," & cod_servizio & ");"
                End If

            End If

        End If

    End Sub

    Private Sub SetURLMenuBS2017(ByVal Piva As String)
        Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu
        objAgenda.Piva = Piva
        piva_MenuBS_2017 = Piva
        url_MenuBS_2017 = RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)
        'url_MenuBS_2017 = Replace(url_MenuBS_2017, ":52551", "")
    End Sub

    Private Sub AgroMasterPage_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        Inizializza()

        Dim paramSessioneObjParametriValue As String = ""
        If Me.Page.ToString = "ASP.index_aspx" Then
            paramSessioneObjParametriValue = "ASG_objParametri_Server_IndexAspx"
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        ElseIf Me.Page.ToString = "ASP.login_login_aspx" Then
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase)
        Else
            _LinkGiasBase = MyBase.PATH_GIASBASE
        End If

        Dim PuntoInterrogativo As String = "?"

        If GiasVersioneCorrente.StartsWith("?") Then
            PuntoInterrogativo = ""
        End If

        AppendCssToHeader("", PuntoInterrogativo, _LinkGiasBase & "agronica/Styles/site.css", _CssPlaceHolder)

        Dim ListaStiliPersonalizzati As String =
            LeggiDaSessioneOppureDaConfigSiti("ListaStiliPersonalizzati", "", agronicacoreparametri_tipoDB.Server, paramSessioneObjParametriValue:=paramSessioneObjParametriValue)

        If ListaStiliPersonalizzati = "" Then
            ListaStiliPersonalizzati = LeggiDaSessioneOppureDaConfigSiti("ListaStiliPersonalizzati", "", agronicacoreparametri_tipoDB.SuperServer)
        End If

        If ListaStiliPersonalizzati <> "" Then
            For Each curStile As String In ListaStiliPersonalizzati.Split(";")
                If curStile <> "" Then
                    AppendCssToHeader("", PuntoInterrogativo, _LinkGiasBase & "agronica/Styles/" & curStile, _CssPlaceHolder)

                End If
            Next
        End If

        LeggiCustomLoghi()
    End Sub

    Private Sub GetTipoCompilazioneMin()

        _modalitaMin = HttpContext.Current.Session("ASG_M_ModalitaMinify")

        Try
            If String.IsNullOrEmpty(_modalitaMin) OrElse _modalitaMin = "ND" Then


                Dim nomeFile As String = "/Min_Config_pers.xml"

                Dim sito As String = HttpContext.Current.Server.MapPath("~/Classi/Minify")

                Dim fileCompleto As String = sito & nomeFile

                If IO.File.Exists(fileCompleto) Then

                    Dim xpathDoc As New XPathDocument(fileCompleto)

                    If Not xpathDoc Is Nothing Then
                        Dim xmlNav As XPathNavigator = xpathDoc.CreateNavigator()

                        Dim xmlNI = xmlNav.SelectSingleNode("/MinConfig/Target")
                        If Not xmlNI Is Nothing Then
                            _modalitaMin = xmlNI.Value
                        End If
                    End If

                End If

            End If

        Catch ex As Exception
            'Lasciato vuoto apposta perché non sia bloccante
        Finally

            If _modalitaMin = "Release" Then
                UsaFileMinified = True
            ElseIf _modalitaMin = "Debug" Then
                UsaFileMinified = False
            Else
                UsaFileMinified = False 'Fallback per sicurezza in caso di errori
            End If

            HttpContext.Current.Session("ASG_M_ModalitaMinify") = _modalitaMin
            HttpContext.Current.Session("UsaFileMinify") = UsaFileMinified
        End Try

    End Sub

    Private Sub Leggi_Versione_Header()

        If _versioneHeader <> "" Then
            Exit Sub
        End If
        '_versioneHeader = apiController.VersioneHeader
        If IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
            _versioneHeader = VERSIONE_HEADER_DEFAULT
            Exit Sub
        End If

        If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            _versioneHeader = VERSIONE_HEADER_DEFAULT
            Exit Sub
        End If

        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        Dim objParametriSuperServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Super_Server"))
        apiController.Inizializza(objParametriSuperServer, objParametriServer, _SitoOspite)
        _versioneHeader = apiController.VersioneHeader

        If _CssPlaceHolder Is Nothing Then
            Throw New Exception("AgronicaControlli_2010: Agro Master Page: placeholder per fogli di stile non inizializzato ")
        End If

        If _FooterPlaceHolder Is Nothing Then
            Throw New Exception("AgronicaControlli_2010: Agro Master Page: placeholder per il footer non inizializzato ")
        End If

    End Sub

    Private Sub SetHeaderVersion(versione As String)
        If Not _headerVersionDisponibili.Contains(versione) Then
            Throw New Exception("Versione Header " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _headerVersionDisponibili))
        End If
        _versioneHeader = versione
    End Sub

    Private Function Load_And_Render_Header_2022(parametriHeader As ParametriHeader2022) As String

        Dim helper As New headerHelper(parametriHeader)
        Return helper.RenderUserControl()

    End Function

End Class
