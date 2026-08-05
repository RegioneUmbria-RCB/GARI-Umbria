
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModello.ParametriAgenda_Temp


Public Class StampeBootstrap
    Inherits System.Web.UI.MasterPage

    Public objParametri_server_String As String
    Public objParametri_utenti_String As String
    Public objparametri_super_server_string As String
    Public pathCoreWS As String

    Public flag_pag_MenuPrincipale As Boolean = False
    Public flag_pag_Anagrafica As Boolean = False
    Public flag_pag_MenuAgenda As Boolean = False

    Public debug_isattached As Boolean = False


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

    Public Property Header_versione As String
        Get
            Return agroMasterPage.HeaderVersione
        End Get
        Set(value As String)
            agroMasterPage.HeaderVersione = value
        End Set
    End Property

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

        Dim objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        Dim objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        Dim objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        objParametri_server_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Server)
        objParametri_utenti_String = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Utenti)
        objparametri_super_server_string = AgronicaCoreDataProvider.Utility.convertOBJparametritoString(objParametri_Super_Server)

        agroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        agroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        agroMasterPage.FooterPlaceHolder = footerPlaceHoler
        agroMasterPage.SitoOspite = Enum_SiteRedirector.Sito_AgronicaStampe_2010

        jquery.SitoOspite = Enum_SiteRedirector.Sito_AgronicaStampe_2010
        bootstrap.SitoOspite = Enum_SiteRedirector.Sito_AgronicaStampe_2010
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader

        'Imposto la variabile con il path dei coreWS da usare in JS
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable

        If IsNothing(pathCoreWS) OrElse pathCoreWS <> "" Then
            DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
                pathCoreWS = DTConfigSiti.Rows(0).Item("Valore")
            End If
        End If

        'imposto rag_soc e utente
        Dim objParametriAgenda As New ParametriAgenda
        If objParametriAgenda.Piva <> "" Then
            If objParametriAgenda.RagSoc = "" Then
                'lo leggo solamente una volta
                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                objParametriAgenda.RagSoc = objImprese.RagSoc_from_Piva(objParametriAgenda.Piva, objParametri_Server)
            End If
            LblRag_Soc.Text = objParametriAgenda.RagSoc
        End If

        'imposto la finestra temporale
        If objParametri_Server.FinestraTemporaleInizio = "01/01/1900" And objParametri_Server.FinestraTemporaleFine = "31/12/2100" Then
            lblFinestraTemporale.Text = Resources.AgronicaStampe_2010.VisualizzazioneIllimitata
        Else
            lblFinestraTemporale.Text = Resources.AgronicaStampe_2010.VisualizzazioneLimitataTraIl &
                                        If(objParametri_Server.FinestraTemporaleInizio = "01/01/1900", "...", objParametri_Server.FinestraTemporaleInizio) &
                                        " e il " &
                                        If(objParametri_Server.FinestraTemporaleFine = "31/12/2100", "...", objParametri_Server.FinestraTemporaleFine)
        End If

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaStampe_2010

        agroVersioneMaster.VersioneMasterPlaceHolder = versioneMasterPlaceHolder
        agroVersioneMaster.SitoOspite = Enum_SiteRedirector.Sito_AgronicaStampe_2010

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
        agroMasterPage.SetMenuBS2017(0, objParametriAgenda.Piva, True)

    End Sub



    Public Function TrovaRedirectCorretto(ByVal online As Boolean, ByVal paginaRichiesta As enum_PagineGiasOnline, ByVal objParametriAgenda As ParametriAgenda) As String

        If HttpContext.Current.Session("Sito_Origine") = Enum_SiteRedirector.Sito_GiasOnline_2010 Then
            If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                Return "../Menu/Menu.aspx"
            End If

            If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                Return "../Menu/Menubs_anagrafica.aspx"
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
            If paginaRichiesta = enum_PagineGiasOnline.MenuAgenda Then
                Return "../Menu/Menu.aspx"
            End If

            If paginaRichiesta = enum_PagineGiasOnline.MenuAnagrafica Then
                Return "../Menu/Menubs_anagrafica.aspx"
            End If

            Return "../Menu/Menu.aspx"
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

            End Select

        End If
    End Function

    Public Sub SetTitoloPagina(IDSezione As Integer)
        If agroMasterPage.flag_MenuBS_2017 AndAlso IDSezione <> 0 Then
            agroMasterPage.SetTitoloMenuBS2017(IDSezione)
        End If
    End Sub


End Class