Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports Newtonsoft.Json.Linq
Imports Newtonsoft.Json
Imports System.Web.UI.WebControls
Imports System.Text
Imports System.Web
Imports System.Web.UI
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreDataProvider

Public Class UmaBootstrap
    Inherits System.Web.UI.MasterPage


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

    Public Property flag_pag_GestioneMagazziniBS As Boolean
        Get
            Return agroMasterPage.flag_pag_GestioneMagazziniBS
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_GestioneMagazziniBS = value
        End Set
    End Property

    Public Property flag_MostraBtnCambiaImpresa As Boolean
        Get
            Return agroMasterPage.flag_MostraBtnCambiaImpresa
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraBtnCambiaImpresa = value
        End Set
    End Property

    Public Property flag_pag_CampionamentoConferito As Boolean
        Get
            Return agroMasterPage.flag_pag_CampionamentoConferito
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_CampionamentoConferito = value
        End Set
    End Property
    Public Property flag_pag_LavorazioniFF As Boolean
        Get
            Return agroMasterPage.flag_pag_LavorazioniFF
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_LavorazioniFF = value
        End Set
    End Property

    Public Property flag_pag_AnalisiProgetti As Boolean
        Get
            Return agroMasterPage.flag_pag_AnalisiProgetti
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_AnalisiProgetti = value
        End Set
    End Property

    Public Property flag_pag_GestioneCompletaCdG As Boolean
        Get
            Return agroMasterPage.flag_pag_GestioneCompletaCdG
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_GestioneCompletaCdG = value
        End Set
    End Property

    Public Property flag_pag_ScaricoTempi As Boolean
        Get
            Return agroMasterPage.flag_pag_ScaricoTempi
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_ScaricoTempi = value
        End Set
    End Property

    Public Property flag_pag_GestioneCosti As Boolean
        Get
            Return agroMasterPage.flag_pag_GestioneCosti
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_GestioneCosti = value
        End Set
    End Property

    Public Property flag_pag_Operazione As Boolean
        Get
            Return agroMasterPage.flag_pag_Operazione
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_Operazione = value
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

    Public Property flag_pag_MenuBS_2017 As Boolean
        Get
            Return agroMasterPage.flag_pag_MenuBS_2017
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_pag_MenuBS_2017 = value
        End Set
    End Property

    Public Property flag_base_MenuBS_2017 As Boolean
        Get
            Return agroMasterPage.flag_base_MenuBS_2017
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_base_MenuBS_2017 = value
        End Set
    End Property

    Public Property flag_link_MenuBS_2017 As Boolean
        Get
            Return agroMasterPage.flag_link_MenuBS_2017
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_link_MenuBS_2017 = value
        End Set
    End Property

    Public Property config_MenuBS_2017 As JObject
        Get
            Return agroMasterPage.config_MenuBS_2017
        End Get
        Set(value As JObject)
            agroMasterPage.config_MenuBS_2017 = value
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

    Public Property piva_MenuBS_2017 As String
        Get
            Return agroMasterPage.piva_MenuBS_2017
        End Get
        Set(value As String)
            agroMasterPage.piva_MenuBS_2017 = value
        End Set
    End Property

    Public Property flag_MostraHeader As Boolean
        Get
            Return agroMasterPage.flag_MostraHeader
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraHeader = value
        End Set
    End Property

    Public Property flag_MostraFooter As Boolean
        Get
            Return agroMasterPage.flag_MostraFooter
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraFooter = value
        End Set
    End Property

    Public Property flag_MostraBtnEsci As Boolean
        Get
            Return agroMasterPage.flag_MostraBtnEsci
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraBtnEsci = value
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

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return agroMasterPage.PATH_GIASBASE
        End Get
    End Property

    Public ReadOnly Property azione_BtnIndietro() As String
        Get

            If (flag_pag_AnalisiProgetti Or flag_pag_GestioneCompletaCdG Or flag_pag_GestioneCosti Or flag_pag_ScaricoTempi Or flag_pag_LavorazioniFF Or flag_pag_CampionamentoConferito Or flag_pag_MenuAgenda Or flag_pag_MenuAnagrafica Or flag_pag_Anagrafica Or flag_pag_Operazione Or flag_pag_Scadenzario Or flag_pag_Visite Or flag_MostraBtnIndietro) And Not flag_pag_BootstrapModal Then

                Dim azione_indietro As String = "Azione_Indietro();"

                If flag_pag_LavorazioniFF Then
                    azione_indietro = "Azione_Indietro_LavorazioniFF();"
                End If

                If flag_pag_CampionamentoConferito Then
                    azione_indietro = "Azione_Indietro_CampionamentoConferito();"
                End If

                If flag_pag_AnalisiProgetti Then
                    azione_indietro = "Azione_Indietro_AnalisiProgetti();"
                End If

                If flag_pag_GestioneCompletaCdG Then
                    azione_indietro = "Azione_Indietro_GestioneCompletaCdG();"
                End If

                If flag_pag_GestioneCosti Then
                    azione_indietro = "Azione_Indietro_GestioneCosti();"
                End If

                If flag_pag_ScaricoTempi Then
                    azione_indietro = "Azione_Indietro_ScaricoTempi ();"
                End If

                If flag_pag_Scadenzario Then
                    azione_indietro = "Azione_Indietro_Scadenzario ();"
                End If

                Return azione_indietro

            End If

            Return Nothing

        End Get
    End Property

    Public UsernameLoggato As String
    Public objParametri_server_String As String
    Public objParametri_utenti_String As String
    Public objparametri_super_server_string As String

    Public objParametri_agenda_String As String = ""

    Public pathCoreWS As String
    Public SonoLoggatoComeSuperUser As String
    Public LinguaDellUtente As String

    Public pivaAziendaSelezionataClientSide As String = ""
    Public masterAgroMeteoLatitudine As String = "0"
    Public masterAgroMeteoLongitudine As String = "0"
    Public masterAgroMeteoDescrizione As String = ""

    Public ReadOnly Property lblDescrGeneric_ClientID As String
        Get
            Return lblDescrGeneric.ClientID
        End Get
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

    Public Property LbLUltimoAccesso() As Global.System.Web.UI.WebControls.Label
        Get
            Return agroMasterPage.LbLUltimoAccesso
        End Get
        Set(value As Global.System.Web.UI.WebControls.Label)
            agroMasterPage.LbLUltimoAccesso = value
        End Set
    End Property

    Public Property agroMasterPage_UtenteUsername() As String
        Get
            Return agroMasterPage.UtenteUsername
        End Get
        Set(value As String)
            agroMasterPage.UtenteUsername = value
        End Set
    End Property

    Public Property bootstrapSelect_versione As String
        Get
            Return bS.Versione
        End Get
        Set(value As String)
            bS.Versione = value
        End Set
    End Property

    Public Property Bootstrap_Versione As String
        Get
            Return bootstrap.Versione
        End Get
        Set(value As String)
            bootstrap.Versione = value
        End Set
    End Property


    Public Property jQuery_versione As String
        Get
            Return jquery.Versione
        End Get
        Set(value As String)
            jquery.Versione = value
        End Set
    End Property

    Public debug_isattached As Boolean = False

    Public ReadOnly Property hdLinkHomePageGlobale_Value As String
        Get
            Return hdLinkHomePageGlobale.Value
        End Get
    End Property

    Private _isCustom500 As Boolean = False
    Public Property IsCustom500 As Boolean
        Get
            Return _isCustom500
        End Get
        Set(value As Boolean)
            _isCustom500 = value
        End Set
    End Property

    Public Property Master_versione As String
        Get
            Return agroVersioneMaster.VersioneMaster
        End Get
        Set(value As String)
            agroVersioneMaster.VersioneMaster = value
        End Set
    End Property

    Public ReadOnly Property CustomLoghi As Object
        Get
            Return agroMasterPage.CustomLoghi
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        If Not Page.IsPostBack Then
            hdLinkHomePageGlobale.Value = AgronicaBase.LinkHomePageGlobale
        End If

        Dim objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        objParametri_server_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objParametri_utenti_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)
        objparametri_super_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Super_Server)

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaUma

        agroVersioneMaster.VersioneMasterPlaceHolder = versioneMasterPlaceHolder
        agroVersioneMaster.SitoOspite = Enum_SiteRedirector.Sito_AgronicaUma

        jquery.SitoOspite = Enum_SiteRedirector.Sito_AgronicaUma

        agroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        agroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        agroMasterPage.FooterPlaceHolder = footerPlaceHoler
        agroMasterPage.SitoOspite = Enum_SiteRedirector.Sito_AgronicaUma

        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader
        bootstrap.SitoOspite = Enum_SiteRedirector.Sito_AgronicaUma

        'se la pagina è la custom500 allora non ho a disposizione gli oggetti di sessione poichè è scaduta.
        If _isCustom500 Then
            Exit Sub
        End If

        UsernameLoggato = objParametri_Server.UtenteUsername

        SonoLoggatoComeSuperUser = (objParametri_Server.SuperUserUsername = objParametri_Server.UtenteUsername).ToString.ToLower
        LinguaDellUtente = objParametri_Server.Lingua_Cod


        'imposto rag_soc e utente
        Dim objParametriAgenda As New ParametriAgenda
        If objParametriAgenda.Piva <> "" Then

            pivaAziendaSelezionataClientSide = objParametriAgenda.Piva

            If objParametriAgenda.RagSoc = "" Then
                'lo leggo solamente una volta
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                objParametriAgenda.RagSoc = objImprese.RagSoc_from_Piva(objParametriAgenda.Piva, objParametri_Server)
            End If
            LblRag_Soc.Text = objParametriAgenda.RagSoc
            Dim a = AgronicaCoreDataProvider.Utility.convertOBJtoString(objParametriAgenda, False)
            a = AgronicaCoreUtility.jSon.Escape2(a)
            objParametri_agenda_String = a

        ElseIf Not flag_pag_MenuBS_2017 Then

            Dim Script As New StringBuilder


            Script.Length = 0

            Script.AppendLine("$(document).ready(function () { ")
            Script.AppendLine("     Azione_Cambia_Impresa();")
            Script.AppendLine("}); ")

            ScriptManager.RegisterStartupScript(Page, Page.GetType(),
                                            String.Format("jQuery_{0}", Page.ClientID), Script.ToString, True)
        End If

        'imposto la finestra temporale
        If objParametri_Server.FinestraTemporaleInizio = "01/01/1900" And objParametri_Server.FinestraTemporaleFine = "31/12/2100" Then
            lblFinestraTemporale.Text = "Visualizzazione Illimitata"
        Else
            lblFinestraTemporale.Text = "Visualizzazione Limitatata tra il " &
                IIf(objParametri_Server.FinestraTemporaleInizio = "01/01/1900", "...", objParametri_Server.FinestraTemporaleInizio) &
                " e il " &
            IIf(objParametri_Server.FinestraTemporaleFine = "31/12/2100", "...", objParametri_Server.FinestraTemporaleFine)
        End If

        'imposto l'utente
        Dim dtUtente As DataTable = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R().Utenti_Dettagli_from_USERNAME(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        Dim Username As String = dtUtente.Rows(0).Item("Username")
        agroMasterPage_UtenteUsername = Username
        Dim Flag_Azienda_Persona As String = dtUtente.Rows(0).Item("Flag_Azienda_Persona")

        If Flag_Azienda_Persona = "2" Then
            Dim Nome As String = dtUtente.Rows(0).Item("Nome")
            Dim Cognome As String = dtUtente.Rows(0).Item("Cognome")
            LbLUtente.Text = Nome & " " & Cognome & " (" & Username & ")"
        Else
            Dim Rag_Soc As String = dtUtente.Rows(0).Item("Rag_Soc")
            LbLUtente.Text = Rag_Soc & " (" & Username & ")"
        End If

        Dim ultimoAccesso As String = Session("ASG_UltimoAccesso")

        If String.IsNullOrEmpty(ultimoAccesso) Then

            Dim xLeggiUltimoAccesso As New AgronicaCoreUtentiBIZ.AWS_Log_R
            ultimoAccesso = xLeggiUltimoAccesso.UltimoAccesso(objParametri_Utenti)

            If Not String.IsNullOrEmpty(ultimoAccesso) Then
                Session("ASG_UltimoAccesso") = ultimoAccesso
                LbLUtente.Text &= " - Ultimo Accesso: " & ultimoAccesso
            End If
        Else
            LbLUtente.Text &= " - Ultimo Accesso: " & ultimoAccesso
        End If

        ' settaggi per nuovo menu
        agroMasterPage.SetHeaderMenuBS2017("", "", "", ultimoAccesso)
        agroMasterPage.SetMenuBS2017(Session("IDSezione"), objParametriAgenda.Piva, True)

        'DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)
        'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso DTConfigSiti.Rows(0)("valore").ToString.ToLower = "true" Then
        '    flag_MenuBS_2017 = True
        'ElseIf Not String.IsNullOrEmpty(ultimoAccesso) Then
        '    LbLUtente.Text &= " - Ultimo Accesso: " & ultimoAccesso
        'End If

        'Imposto la variabile con il path dei coreWS da usare in JS

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable

        If IsNothing(pathCoreWS) OrElse pathCoreWS <> "" Then


            DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                pathCoreWS = DTConfigSiti.Rows(0).Item("Valore")
            End If

        End If

        MostraAssistenza(objParametri_Super_Server, objParametri_Server)

    End Sub

    Private Sub MostraAssistenza(objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim xAssistenza As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim sAssistenza As String = xAssistenza.Assistenza(objParametri_Super_Server, objParametri_Server)

        agroMasterPage.sAssistenzaInfo = sAssistenza
    End Sub


    ' Imposta il titolo pagina se menu nuovo
    Public Sub SetTitoloPagina(IDSezione As Integer)
        If flag_MenuBS_2017 AndAlso IDSezione <> 0 Then
            agroMasterPage.SetTitoloMenuBS2017(IDSezione)
        End If
    End Sub

    ' Imposta il titolo pagina custom
    Public Sub SetTitoloPaginaCustom(Testo As String, Optional Colore As String = "#002F5F")
        Dim sezione As New JObject From {
            {"testo", Testo},
            {"colore", Colore},
            {"classeCSS", "buttonpreferitielementi"},
            {"IDSezione", "-1"},
            {"IDSezionePadre", "0"},
            {"sitoRichiesto", ""},
            {"RedirectURL", ""},
            {"paginaRichiesta", ""},
            {"aziendaRichiesta", ""},
            {"tipoAperturaPagina", ""},
            {"configurazione", ""}
        }
        Dim oggetto As String = JsonConvert.SerializeObject(sezione)
        HttpContext.Current.Session("ASG_MenuBS_2017") = oggetto
    End Sub


    Public Function TrovaRedirectCorretto(ByVal online As Boolean, ByVal paginaRichiesta As enum_PagineGiasOnline, ByVal objParametriAgenda As ParametriAgenda, Optional ByVal usePaginaRichiestaAgenda As Boolean = False) As String

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable
        Dim TargetMenuAgenda As String = "../Menu/Menu.aspx"
        'DTConfigSiti = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)
        'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0)("Valore")) = "true" Then
        '    TargetMenuAgenda = "../menu/menubs_2017.aspx"
        'Else
        DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
            TargetMenuAgenda = "../menu/menubs_agenda_nuovo.aspx"
        End If
        'End If

        If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
            If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                Return TargetMenuAgenda
            End If

            If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                Dim objConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim valAnagraficaNG = objConfSiti.Leggi_Valore(0, "MenuAnagrafeNG", "", "", objParametri_Server)
                Dim TargetRedirect = ""
                If valAnagraficaNG.ToLower = "true" Then
                    MenuBS_2017_RedirectGestione.RedirectGenerico(objParametriAgenda.Piva,
                                                                  Enum_SiteRedirector.GiasNG,
                                                                  enum_PagineGiasNG.Pagina_Menu_Anagrafica_Impianti,
                                                                  TargetRedirect,
                                                                  objParametri_Server)
                Else
                    TargetRedirect = "../MenuAnagrafica/Menubs_anagrafica.aspx"
                End If
                Return TargetRedirect
            End If

            Dim objGiasOnline_2010 As New AgronicaCoreGestioneRichieste.ParametriGiasOnline_2010
            objGiasOnline_2010.Pagina_Richiesta = paginaRichiesta

            Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri(
                                           Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                           paginaRichiesta,
                                           enum_PagineAgenda_2010.Menu, objParametriAgenda.Piva, "", "", 0, "")

            Response.Redirect(PaginaLink)

            'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Return objWebConfig.LinkGiasOnline_2010
        End If
        If online = False Then
            If usePaginaRichiestaAgenda = False Then
                If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                    Return TargetMenuAgenda
                End If

                If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                    Return "../MenuAnagrafica/Menubs_anagrafica.aspx"
                End If
            Else
                ' Se uso l'come pagina richiesta l'enum_PagineAgenda_2010 allora redirigo alla pagina corretta
                Select Case paginaRichiesta
                    Case enum_PagineAgenda_2010.Pagina_Anagrafica_Menu
                        Return "../MenuAnagrafica/Menubs_anagrafica.aspx"
                    Case enum_PagineAgenda_2010.Menu_BS
                        Return "../menu/menubs_agenda_nuovo.aspx"
                End Select
            End If

            Return TargetMenuAgenda
        Else
            Select Case HttpContext.Current.Session("Sito_Origine")
                Case Enum_SiteRedirector.Sito_GiasOnline

                    Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
                    objGiasOnline.Cul_Cod = objParametriAgenda.Cul_Cod
                    objGiasOnline.DataSelezionata = objParametriAgenda.Data
                    objGiasOnline.Id_Agenda = objParametriAgenda.Id_Agenda
                    objGiasOnline.Lavorazione = objParametriAgenda.Lav_Cod
                    objGiasOnline.PaginaRichiesta = paginaRichiesta
                    objGiasOnline.Piva = objParametriAgenda.Piva
                    objGiasOnline.Sa_Cod = objParametriAgenda.Sa_Cod
                    Dim specie As Integer = 0
                    If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
                        specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
                    End If
                    objGiasOnline.Veg_Cod = specie

                    Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
                                   Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                                   objGiasOnline)

                    Response.Redirect(str)
                Case Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                    If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                        Return TargetMenuAgenda
                    End If

                    If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                        Return "../MenuAnagrafica/Menubs_anagrafica.aspx"
                    End If
                Case Else
                    If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                        Return TargetMenuAgenda
                    End If

                    If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                        Return "../MenuAnagrafica/Menubs_anagrafica.aspx"
                    End If
            End Select

        End If
    End Function

End Class