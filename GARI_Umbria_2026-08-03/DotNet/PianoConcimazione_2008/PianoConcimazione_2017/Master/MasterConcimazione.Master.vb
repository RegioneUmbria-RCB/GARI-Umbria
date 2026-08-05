Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
Imports AgronicaCoreDataProvider
Imports AgronicaControlli_2010

Public Class MasterConcimazione
    Inherits System.Web.UI.MasterPage

    'Public flag_pag_Menu As Boolean = False
    Public debug_isattached As Boolean = False
    'Public flag_pag_Operazione As Boolean = False

    Public Property lbl_DescrGeneric() As Label
        Get
            Return lblDescrGeneric
        End Get
        Set(value As Label)
            lblDescrGeneric = value
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

    Public Property flag_MostraBtnCambiaImpresa() As Boolean
        Get
            Return agroMasterPage.flag_MostraBtnCambiaImpresa
        End Get
        Set(value As Boolean)
            agroMasterPage.flag_MostraBtnCambiaImpresa = value
        End Set
    End Property

    Public Property Lbl_Titolo() As Label
        Get
            Return agroMasterPage.Lbl_Titolo
        End Get
        Set(value As Label)
            agroMasterPage.Lbl_Titolo = value
        End Set
    End Property

    Public Property ImgBtnAnnullaTutto() As ImageButton
        Get
            Return agroMasterPage.ImgBtnAnnullaTutto
        End Get
        Set(value As ImageButton)
            agroMasterPage.ImgBtnAnnullaTutto = value
        End Set
    End Property

    Public Property ImgBtnFiltro() As ImageButton
        Get
            Return agroMasterPage.ImgBtnFiltro
        End Get
        Set(value As ImageButton)
            agroMasterPage.ImgBtnFiltro = value
        End Set
    End Property

    Public Property LblRag_Soc() As Label
        Get
            Return agroMasterPage.LblRag_Soc
        End Get
        Set(value As Label)
            agroMasterPage.LblRag_Soc = value
        End Set
    End Property

    Public Property lblFinestraTemporale() As Label
        Get
            Return agroMasterPage.lblFinestraTemporale
        End Get
        Set(value As Label)
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

    Public Property HiddenMessaggioErrore() As String
        Get
            Return Hidden_Messaggio_Errore.Value
        End Get
        Set(value As String)
            Hidden_Messaggio_Errore.Value = GlobalAsax_Helper.SanitizeHTML(value)
        End Set
    End Property
    Public Property HiddenMessaggioOK() As String
        Get
            Return Hidden_Messaggio_Ok.Value
        End Get
        Set(value As String)
            Hidden_Messaggio_Ok.Value = GlobalAsax_Helper.SanitizeHTML(value)
        End Set
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

    Public Property Master_versione As String
        Get
            Return agroVersioneMaster.VersioneMaster
        End Get
        Set(value As String)
            agroVersioneMaster.VersioneMaster = value
        End Set
    End Property

    Public objParametri_server_String As String
    Public objParametri_utenti_String As String
    Public objparametri_super_server_string As String

    Public pathCoreWS As String

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return agroMasterPage.PATH_GIASBASE
        End Get
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

        Dim objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        objParametri_server_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objParametri_utenti_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)
        objparametri_super_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Super_Server)

        If Not Page.IsPostBack Then
            hdLinkHomePageGlobale.Value = AgronicaBase.LinkHomePageGlobale
        End If

        jquery.SitoOspite = Enum_SiteRedirector.Sito_PianoConcimazione_2017
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader
        bootstrap.SitoOspite = Enum_SiteRedirector.Sito_PianoConcimazione_2017

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_PianoConcimazione_2017

        agroVersioneMaster.VersioneMasterPlaceHolder = versioneMasterPlaceHolder
        agroVersioneMaster.SitoOspite = Enum_SiteRedirector.Sito_PianoConcimazione_2017

        agroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        agroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        agroMasterPage.FooterPlaceHolder = footerPlaceHoler
        agroMasterPage.SitoOspite = Enum_SiteRedirector.Sito_PianoConcimazione_2017

        'se la pagina è la custom500 allora non ho a disposizione gli oggetti di sessione poichè è scaduta.
        If _isCustom500 Then
            Exit Sub
        End If

        'imposto rag_soc e utente
        Dim objParametriConcimazione As New AgronicaCoreGestioneRichieste.ParametriConcimazione_2017
        objParametriConcimazione.Leggi()
        If objParametriConcimazione.Piva <> "" Then
            Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
            LblRag_Soc.Text = objImprese.RagSoc_from_Piva(objParametriConcimazione.Piva, objParametri_Server)
        Else
            LblRag_Soc.Text = "TUTTE LE IMPRESE"
        End If

        'imposto la finestra temporale
        If objParametri_Server.FinestraTemporaleInizio = "01/01/1900" And objParametri_Server.FinestraTemporaleFine = "31/12/2100" Then
            lblFinestraTemporale.Text = "Visualizzazione Illimitata"
        Else
            lblFinestraTemporale.Text = "Visualizzazione limitata tra il" &
                IIf(objParametri_Server.FinestraTemporaleInizio = "01/01/1900", "...", objParametri_Server.FinestraTemporaleInizio) &
                " e il " &
            IIf(objParametri_Server.FinestraTemporaleFine = "31/12/2100", "...", objParametri_Server.FinestraTemporaleFine)
        End If

        'imposto l'utente
        Dim dtUtente As DataTable = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R().Utenti_Dettagli_from_USERNAME(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
        Dim Username As String = dtUtente.Rows(0).Item("Username")
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

        ' imposta parametri per gestione nuovo menu
        agroMasterPage.SetHeaderMenuBS2017(LblRag_Soc.Text, LbLUtente.Text, lblFinestraTemporale.Text, ultimoAccesso)
        agroMasterPage.SetMenuBS2017(Session("IDSezione"), objParametriConcimazione.Piva, True)

        'Imposto la variabile con il path dei coreWS da usare in JS
        If IsNothing(pathCoreWS) OrElse pathCoreWS <> "" Then

            Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)

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

    'Public Function TrovaRedirectCorretto(ByVal online As Boolean, ByVal paginaRichiesta As enum_PagineGiasOnline, ByVal objParametriConcimazione As AgronicaCoreGestioneRichieste.ParametriConcimazione_2008) As String

    '    'Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
    '    'Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
    '    'Dim DTConfigSiti As DataTable
    '    Dim TargetMenuAgenda As String = "../Menu/Menu.aspx"
    '    'DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)
    '    'If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then
    '    '    TargetMenuAgenda = "../menu/menubs_agenda_nuovo.aspx"
    '    'End If
    '    If Not IsNothing(ConfigurationSettings.AppSettings("MenuAgendaBS")) AndAlso ConfigurationSettings.AppSettings("MenuAgendaBS") = True Then
    '        TargetMenuAgenda = "../menu/menubs_agenda_nuovo.aspx"
    '    End If

    '    If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
    '        If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
    '            Return TargetMenuAgenda
    '        End If

    '        If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
    '            Return "../Menu/Menubs_anagrafica.aspx"
    '        End If

    '        Dim objGiasOnline_2010 As New AgronicaCoreGestioneRichieste.ParametriGiasOnline_2010
    '        objGiasOnline_2010.Pagina_Richiesta = paginaRichiesta

    '        Dim PaginaLink As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline2010_PassandoDirettamenteIParametri( _
    '                                       Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
    '                                       paginaRichiesta, _
    '                                       enum_PagineAgenda_2010.Menu, objParametriConcimazione.Piva, "", "", 0, "")

    '        Response.Redirect(PaginaLink)

    '        'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '        'Return objWebConfig.LinkGiasOnline_2010
    '    End If
    '    If online = False Then
    '        If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
    '            Return TargetMenuAgenda
    '        End If

    '        If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
    '            Return "../Menu/Menubs_anagrafica.aspx"
    '        End If

    '        Return TargetMenuAgenda
    '    Else
    '        Select Case HttpContext.Current.Session("Sito_Origine")
    '            Case Enum_SiteRedirector.Sito_GiasOnline

    '                Dim objGiasOnline As New AgronicaCoreGestioneRichieste.ParametriGiasOnline
    '                objGiasOnline.Cul_Cod = objParametriConcimazione.Cul_Cod
    '                objGiasOnline.DataSelezionata = objParametriConcimazione.Data
    '                objGiasOnline.Id_Agenda = objParametriConcimazione.Id_Agenda
    '                objGiasOnline.Lavorazione = objParametriConcimazione.Lav_Cod
    '                objGiasOnline.PaginaRichiesta = paginaRichiesta
    '                objGiasOnline.Piva = objParametriConcimazione.Piva
    '                objGiasOnline.Sa_Cod = objParametriConcimazione.Sa_Cod
    '                Dim specie As Integer = 0
    '                If IsNumeric(objParametriConcimazione.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriConcimazione.Veg_Cod.Split("/")(0)) > 0 Then
    '                    specie = CInt(objParametriConcimazione.Veg_Cod.Split("/")(0))
    '                End If
    '                objGiasOnline.Veg_Cod = specie

    '                Dim str As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline( _
    '                               Enum_SiteRedirector.Sito_AgronicaAgenda_2010, _
    '                               objGiasOnline)

    '                Response.Redirect(str)

    '        End Select

    '    End If
    'End Function


End Class