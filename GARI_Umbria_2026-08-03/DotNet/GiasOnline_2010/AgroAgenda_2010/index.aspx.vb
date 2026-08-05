Imports System.Configuration.ConfigurationManager
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Web.Services
Imports AgroAgenda_2010.Resources
Imports Agronica.Helpers.SAML
Imports Agronica.Helpers.SAML.Italia.Spid.Authentication.Saml
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreAnagrafeDAL
Imports System.Windows.Interop
Imports NUglify.Helpers
Imports AgronicaCoreModelsSTD.PersonalizzazioniLoghi
Imports AgronicaCoreModelsSTD.exceptions

Public Class index
    Inherits System.Web.UI.Page

    Dim objParametri_Super_Server As AgronicaCoreParametri
    Private const_MaxNumeroTentativiAccesso As Integer = -1
    Private apiController As CoreApiControllerFactory = New CoreApiControllerFactory
    Public debug_isattached As Boolean = False
    Private _loginVersione As String = VERSIONE_LOGIN_DEFAULT
    Private _showCredentialsInputs As Boolean = True
    Private _loginVersioniDisponibili As String() = {VERSIONE_LOGIN_DEFAULT, "2022"}
    Private _LinkGiasBase_Super_Server As String = GIAS_BASE_DEFAULT_LINK
    Private Const debugSPID = False

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            GiasBaseHelper.Setta_Link_GiasBase("ASG_objParametri_Super_Server", _LinkGiasBase_Super_Server)
            Return _LinkGiasBase_Super_Server
        End Get
    End Property

    Public Property Login_Versione As String
        Get
            Return _loginVersione
        End Get
        Set(value As String)
            SetLoginVersione(value)
        End Set
    End Property

    Public ReadOnly Property Show_Login_Fields As Boolean
        Get
            Return _showCredentialsInputs
        End Get
    End Property

    'Public ReadOnly Property personalizzazioniRegioneUmbria As String
    '    Get
    '        Return AgroMasterPage.personalizzazioniRegioneUmbria
    '    End Get
    'End Property

    Public ReadOnly Property CustomLoghi As Object
        Get
            Return AgroMasterPage.CustomLoghi
        End Get
    End Property

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        Dim listLingue As String() = HttpContext.Current.Request.UserLanguages
        If Not IsNothing(listLingue) Then
            Dim linguaBrowserUtente = listLingue(0).Split("-")(0)
            Threading.Thread.CurrentThread.CurrentUICulture = New Globalization.CultureInfo(linguaBrowserUtente)
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        If Debugger.IsAttached Then
            debug_isattached = True
        End If

        AgroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        AgroMasterPage.FooterPlaceHolder = footerPlaceHoler
        AgroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        AgroMasterPage.PaginaOspite = 0

        jquery.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
        jquery.MemorizzaInSessioneDopoLettura = False

        bootstrap.Versione = "5.3.3"
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader
        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

        'Imposto il pulsante di default quando si preme INVIO
        Form1.DefaultButton = button1.ID

        If debugSPID Then
            button1.Text = "ACCEDI DEBUG SPID: CF IN USERNAME"
        End If

        Dim sessioneScaduta = Request.QueryString("Session")
        If Not String.IsNullOrEmpty(sessioneScaduta) Then
            lblSessioneScaduta.Text = AgronicaAgenda_2010.SessioneScadutaRipetereLogin
            lblSessioneScaduta.ForeColor = Drawing.Color.Red
        End If

        If Not IsPostBack Then
            apiController.Delete_Auth_Cookie_Response()
            Session.RemoveAll()
            If Not IsNothing(AppSettings("Super_Server_NascondiDettagliDB")) AndAlso
                CStr(AppSettings("Super_Server_NascondiDettagliDB")).ToLower = "true" Then
                Label1.Visible = False
                Label2.Visible = False
                Label3.Visible = False
                Label4.Visible = False
            End If
        End If

        'mostro nascondo login .. ?
        If Not IsNothing(AppSettings("Super_Server_NascondiLoginNoQueryString")) AndAlso
           AppSettings("Super_Server_NascondiLoginNoQueryString").ToLower = "true" Then
            If (IsNothing(Request.QueryString("PivaSuperUser")) OrElse
                String.IsNullOrEmpty(Request.QueryString("PivaSuperUser").ToString.Trim)) Then
                'If Request.QueryString.AllKeys.Length = 0 Then
                pnlLogin.Visible = False
                lbl_Messaggio.Text = AgronicaAgenda_2010.RientrareAttraversoPaginaFornita
                lbl_Messaggio.Style.Add("display", "block")
                lbl_Messaggio.Style.Add("text-align", "center")
                Exit Sub
                'End If
            End If
        End If

        '--------------------------------------------------------------
        '-----------------superserver 2014-----------------------
        '--------------------------------------------------------------
        If Not IsPostBack Then
            'crea connessioni e objparametrisuperserver o server a seconda della chiave nel webconfig
            Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
            inizializza.InizializzaSito_GiasOnLine(Session)
        Else
            'se la pagina è in postback ma è scaduta la sessione la ricarico
            If Session.IsNewSession Then
                'Response.Redirect("default.aspx")
                Dim scriptText = "    document.location='./index.Aspx';"
                ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), String.Format("jQuery_{0}", Page.ClientID), scriptText, True)
            End If
        End If


        'Gestione superserver o gias_server in base alla chiave del webconfig Super_Server
        If AppSettings("Super_Server") = "true" Then

            '##############################################################################################
            '####################     OBJPARAMETRI SUPER SERVER ###########################################
            '##############################################################################################

            'carico objParametri_Super_Server dalla sessione
            objParametri_Super_Server = New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

            If Not IsPostBack Then

                If IsNothing(Session("ASG_objParametri_Super_Server")) Then
                    Throw New Exception("La chiave nel webconfig Super_Server è true ma non è stato creato l'objparametri_super_server")
                End If

                'leggo dalla sessione se ho i fitri impostati
                'utile ad esempio per utuenti esterni come su neodimio che si suole che 
                'non vedano altri server anche se fanno un logout
                Dim filtriSito As AgronicaCoreDataProvider.AgronicaCoreParametriFiltroIngressoSitoOnline
                If Not IsNothing(Session("Filtri_Siti_Per_PaginaDefault")) Then
                    filtriSito = Session("Filtri_Siti_Per_PaginaDefault")
                Else
                    filtriSito.ID_DB = 0 'usato in querystring/viewstate
                    filtriSito.TipoDB = enum_Tipo_DB.GIAS_SERVER
                    filtriSito.Server = "" 'usato in querystring/viewstate
                    filtriSito.DB = "" 'usato in querystring/viewstate
                    filtriSito.Provider = ""
                    filtriSito.UserId = ""
                    filtriSito.Password = ""
                    filtriSito.PivaSuperUser = "" 'usato in querystring/viewstate
                    filtriSito.Note = ""
                    filtriSito.Progressivo = 0 'usato in querystring/viewstate
                    filtriSito.Descrizione = ""
                    filtriSito.Filtro_Temp_Inizio = Date.Now
                    filtriSito.Filtro_Temp_Fine = Date.Now
                End If

                'se apro la pagina la prima volta leggo parametri filtro da querystring e li metto nel filtriSito,
                'quindi i parametri viewstate hanno sempre la precedenza
                If Not IsNothing(Request.QueryString("ID_DB")) Then
                    filtriSito.ID_DB = Request.QueryString("ID_DB")
                End If
                If Not IsNothing(Request.QueryString("Server")) Then
                    filtriSito.Server = Request.QueryString("Server")
                End If
                If Not IsNothing(Request.QueryString("DB")) Then
                    filtriSito.DB = Request.QueryString("DB")
                End If
                If Not IsNothing(Request.QueryString("PivaSuperUser")) Then
                    filtriSito.PivaSuperUser = Request.QueryString("PivaSuperUser")
                End If
                If Not IsNothing(Request.QueryString("Progressivo")) Then
                    filtriSito.Progressivo = Request.QueryString("Progressivo")
                End If

                If IsDate(AppSettings("Super_Server_FiltroTempInizio")) Then
                    filtriSito.Filtro_Temp_Inizio = AppSettings("Super_Server_FiltroTempInizio")
                End If
                If IsDate(AppSettings("Super_Server_FiltroTempFine")) Then
                    filtriSito.Filtro_Temp_Fine = AppSettings("Super_Server_FiltroTempFine")
                End If

                Session("Filtri_Siti_Per_PaginaDefault") = filtriSito

                carica_Combo_Servers(filtriSito)

                'AgroMasterPage.SetPersonalizzazioniRegioneUmbria()

            End If

        Else

            '##############################################################################################
            '####################    GIAS SERVER STANDARD#################################################
            '##############################################################################################

            'se non è stato creato Session("ASG_objParametri_Super_Server") allora sono nel caso senza superserver
            'ma con un unico db gas_server standard, quindi gestisco la creazione dei parametri dal connessioni.ini

            ' se esiste comunque ASG_objParametri_Super_Server eccezione
            If Not IsNothing(Session("ASG_objParametri_Super_Server")) Then
                Throw New Exception("E'stato creato l'objparametri per il super_server ma la chiave nel webconfig Super_Server è false, questo non è ammissibile ")
            End If

        End If

        '--------------------------------------------------------------
        '-----------------superserver 2014- END----------------------
        '--------------------------------------------------------------

        HttpContext.Current.Session("Collegamento_DPI") = True
        HttpContext.Current.Session("Collegamento_Fito") = True


        Dim str As String
        str = "$(document).ready(function () {"
        str &= "   $('body').trigger('create'); "
        'str &= "   $('#" & btn_accedi.ClientID & "').closest('.ui-btn').hide();"
        'str &= "   $('#cAccedi').click( function() { $('#" & Button1.ClientID & "').click(); } );"

        'str &= "   show(); "

        str &= "});"

        ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), String.Format("jQuery_{0}", Page.ClientID), str, True)

        cookieManagerAgronicaSpecialNavigation()

        Dim xToken As String = Request.QueryString("token")
        If Not Page.IsPostBack AndAlso Not String.IsNullOrEmpty(xToken) Then
            apriOnlineConDBSelezionato()
        End If

        If Not Page.IsPostBack AndAlso String.IsNullOrEmpty(xToken) Then
            Dim linkBuilder As New UriBuilder
            linkBuilder.Scheme = HttpContext.Current.Request.Url.Scheme
            linkBuilder.Host = HttpContext.Current.Request.Url.Host
            If Not HttpContext.Current.Request.Url.IsDefaultPort Then
                linkBuilder.Port = HttpContext.Current.Request.Url.Port
            End If
            linkBuilder.Path = VirtualPathUtility.ToAbsolute("~/index.aspx")
            AgronicaCoreUtility.Http.CookieImposta("LinkHomePageGlobale", linkBuilder.ToString())
        End If

        MostraAssistenza(objParametri_Super_Server, Nothing)

        MostraPannelloPersonalizzato(objParametri_Super_Server, Nothing)

        ImpostaVersioneLogin(objParametri_Super_Server)

        divSPID.Visible = False
        MostraLoginSPID(objParametri_Super_Server)

        GestioneUtenteSPID(objParametri_Super_Server)

        'Controllo ed eventualmente mostro la presenza di messaggi di manutenzione programmata (o simili)
        Dim handleMessaggiAgronica As New Messaggi_da_Agronica_R
        Dim listMessaggiDaMostrare = handleMessaggiAgronica.LeggiMessaggiAttivi(objParametri_Super_Server)
        messaggiDaAgronica.InnerHtml = String.Join("<br><br>**********************************************<br><br>", listMessaggiDaMostrare)
        'Messaggi.AgroMsgBuonFine(String.Join("<br/><br/>", listMessaggiDaMostrare), Page, UpdatePanel:=updateScript)
    End Sub

    Private Sub cookieManagerAgronicaSpecialNavigation()


        Dim xToken As String = Request.QueryString("token")
        If Not Page.IsPostBack AndAlso Not String.IsNullOrEmpty(xToken) Then

            Dim AgronicaSpecialNavigationCookieValue As String = "1"

            If Not objParametri_Super_Server Is Nothing Then

                Dim LeggiConfString As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                Dim confString As String =
                    LeggiConfString.Leggi_Valore(0, "AgronicaSpecialNavigationCFG", "", "", objParametri_Super_Server)

                If Not String.IsNullOrEmpty(confString) Then
                    Dim confObj = JObject.Parse(confString)
                    AgronicaSpecialNavigationCookieValue = confObj("AgronicaSpecialNavigationTokenValue").ToString
                End If

            End If

            SetCookie("AgronicaSpecialNavigation", AgronicaSpecialNavigationCookieValue)
        Else
            RemoveCookie("AgronicaSpecialNavigation")
        End If

    End Sub



    Private Sub SetCookie(CookieToBeSet As String, value As String)

        Dim c As New HttpCookie(CookieToBeSet, value)

        Response.Cookies.Add(c)

    End Sub

    Private Sub RemoveCookie(CookieToBeRemoved As String)

        Response.Cookies.Get(CookieToBeRemoved).Expires = DateTime.Now.AddDays(-1)

    End Sub


    <WebMethod(EnableSession:=True)>
    Public Shared Function OttieneNumDiAccountAssegnati() As RispostaStandard
        Dim r As New RispostaStandard()



        Return r
    End Function


    Public Sub GestioneUtenteSPID(objParametri_Super_Server As AgronicaCoreParametri)

        Dim SAMLResponse As String = Request.Params("SAMLResponse")

        If SAMLResponse = Nothing Then
            SAMLResponse = ViewState("SAMLResponse")
        ElseIf ViewState("SAMLResponse") Is Nothing Then
            ViewState("SAMLResponse") = SAMLResponse
        End If

        If (SAMLResponse IsNot Nothing AndAlso SAMLResponse <> "") Then

            Dim SPID_CERTIFICATE_NAME As String = ""
            Dim SPID_CERTIFICATE_NAME_CALLBACK As String = ""
            Dim SPID_DOMAIN_VALUE As String = ""
            Dim SPID_ENVIROMENT As String = ""
            Dim SPID_CLAIMS_CFG As String = ""
            Dim SPID_WindowsFindBy As String = ""
            Dim SPID_WindowsFindBy_Callback As String = ""
            Dim SPID_AgroSamlConfig As String = ""

            Dim idleSPID_AssertionConsumerServiceURL As String = ""
            Dim idleSPID_LoginURL As String = ""
            Dim idleSPID_IDP As String = ""


            'TODO: decidere sulla lettura da DB Server
            LeggiParametriSamlWebConfig(
                objParametri_Super_Server, Nothing,
                SPID_CERTIFICATE_NAME,
                SPID_CERTIFICATE_NAME_CALLBACK,
                SPID_DOMAIN_VALUE,
                SPID_ENVIROMENT,
                SPID_CLAIMS_CFG,
                idleSPID_AssertionConsumerServiceURL,
                idleSPID_LoginURL,
                idleSPID_IDP,
                SPID_WindowsFindBy,
                SPID_WindowsFindBy_Callback,
                SPID_AgroSamlConfig
            )

            Dim xFindByWin As X509FindType = X509FindType.FindBySubjectName

            If Not String.IsNullOrEmpty(SPID_WindowsFindBy_Callback) Then
                xFindByWin = SPID_WindowsFindBy_Callback
            End If

            Dim SControl As New SPID_Controller(
                SPID_CERTIFICATE_NAME_CALLBACK,
                SPID_DOMAIN_VALUE,
                SPID_ENVIROMENT,
                xFindByWin
            )


            Dim AgroConfig As AgroSamlConfig
            If Not String.IsNullOrEmpty(SPID_AgroSamlConfig) Then
                AgroConfig = JsonConvert.DeserializeObject(Of AgroSamlConfig)(SPID_AgroSamlConfig)
            Else
                AgroConfig = SamlCfgDefault()
            End If

            Dim SPID_LOG_DIR As String = ""
            LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, Nothing, "SPID_LOG_DIR", SPID_LOG_DIR)

            'loggo tutto il file xml
            If Not String.IsNullOrEmpty(SPID_LOG_DIR) Then
                My.Computer.FileSystem.WriteAllText(FileSystemHelper.AggiungiSlashSeNonEsiste(SPID_LOG_DIR) & FileSystemHelper.NomeFileUnivoco("xml"), SAMLResponse, True)
            End If

            Dim resp = SControl.GetAuthnResponse(SAMLResponse, AgroConfig)

            Dim spidUserInfo = resp.SpidUserInfo



            If Not String.IsNullOrEmpty(SPID_LOG_DIR) Then

                Dim LogDirPrecedente As String = objParametri_Super_Server.LogDirectory
                Dim LogFileNamePrecedente As String = objParametri_Super_Server.LogFileName

                objParametri_Super_Server.LogDirectory = SPID_LOG_DIR
                objParametri_Super_Server.LogFileName = "SAML2.txt"

                Dim ClaimsReceived As String = "SamlLogin: " & Now.ToLongTimeString()
                For Each c In spidUserInfo
                    ClaimsReceived &= c.Key & " - " & c.Value & vbCrLf
                Next

                Dim dp As New DataProvider

                dp.Scrivi_LOG(objParametri_Super_Server, "SAML2Login", ClaimsReceived)

                objParametri_Super_Server.LogDirectory = LogDirPrecedente
                objParametri_Super_Server.LogFileName = LogFileNamePrecedente

            End If

            Try
                Dim cfgConfig = GetSpidConfig(objParametri_Super_Server)
                Dim claimsCfg = GetSMLClaimsCfg(SPID_CLAIMS_CFG, spidUserInfo, cfgConfig)

                apriOnlineConDBSelezionatoSAML(claimsCfg)
                ViewState.Remove("SAMLResponse")
            Catch ex As RecoverableSPIDMultipleAccountLoginException
                ddl_account.Items.Clear()
                For Each item In ex.Risposta.Accounts.Rows
                    ddl_account.Items.Add(New ListItem(item.Item("UserName"), item.Item("UserName")))
                Next
                hdf_ShowModal.Value = ddl_account.Items.Count
                ddl_account.Visible = True
                div_ddl_account.Visible = True
                ClientScript.RegisterClientScriptBlock(Me.GetType(), "Popup", "$(document).ready(function() { $('#dialog_selezionaUtente').modal('show'); })", True)
            Catch ex As Exception

                Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf &
                            ex.Message, Page, UpdatePanel:=updateScript)

            End Try

        End If
    End Sub

    ''' <summary>
    ''' Checks if the dictionary contains all the specified claims and that their value isn't an empty string.
    ''' Throws an Exception otherwise.
    ''' </summary>
    ''' <param name="spidUserInfo">Dictionary containing all the claims in the SAML request</param>
    ''' <param name="ks">Claims that must be present</param>
    ''' <exception cref="Exception">If one or more claims are not present in the dictionary</exception>
    Private Shared Sub CheckSamlClaims(spidUserInfo As Dictionary(Of String, String), ks As IEnumerable(Of String))
        Dim getClaimOrDefault = Function(k As String) If(spidUserInfo.ContainsKey(k), spidUserInfo(k), String.Empty)
        If Not ks.All(Function(k) Not String.IsNullOrEmpty(getClaimOrDefault(k))) Then
            Throw New Exception(AgronicaAgenda_2010.AutenticazioneFallita & ".")
        End If
    End Sub

    Private Shared Function GetClaimObj(claimGias As String, claimSAML As String, spidUserInfo As Dictionary(Of String, String)) As SAMLClaimsRiconoscimento
        Return New SAMLClaimsRiconoscimento With {
            .GiasKey = claimGias,
            .Valore = If(spidUserInfo.ContainsKey(claimSAML), spidUserInfo(claimSAML), String.Empty)
        }
    End Function

    Private Shared Sub AddRequiredClaims(ByRef claimsCfg As SMLClaimsCfg, spidUserInfo As Dictionary(Of String, String))
        claimsCfg.ListaClaimsPerRiconoscimento = New List(Of SAMLClaimsRiconoscimento)
        Select Case claimsCfg.TipoDiSAMLClaimPerRiconoscereUtente
            Case enum_SAML_RiconoscimentoUtente.CodiceFiscale
                'CheckSamlClaims(spidUserInfo, {"CodiceFiscale"}.ToList)
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("CodiceFiscale", "CodiceFiscale", spidUserInfo))

            Case enum_SAML_RiconoscimentoUtente.email
                'CheckSamlClaims(spidUserInfo, {"email"}.ToList)
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("email", GetSamlKeyFromGiasKey("email", claimsCfg), spidUserInfo))

            Case enum_SAML_RiconoscimentoUtente.email_nome_cognome
                'CheckSamlClaims(spidUserInfo, {"email", "nome", "cognome"}.ToList)
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("email", GetSamlKeyFromGiasKey("email", claimsCfg), spidUserInfo))
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("nome", GetSamlKeyFromGiasKey("nome", claimsCfg), spidUserInfo))
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("cognome", GetSamlKeyFromGiasKey("cognome", claimsCfg), spidUserInfo))

            Case enum_SAML_RiconoscimentoUtente.userID
                'CheckSamlClaims(spidUserInfo, {"userID", "nome", "cognome"}.ToList)
                'claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("userID", spidUserInfo))
                'claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("email", spidUserInfo))
                'claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("nome", spidUserInfo))
                'claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("cognome", spidUserInfo))

                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("userID", GetSamlKeyFromGiasKey("userID", claimsCfg), spidUserInfo))
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("email", GetSamlKeyFromGiasKey("email", claimsCfg), spidUserInfo))
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("nome", GetSamlKeyFromGiasKey("nome", claimsCfg), spidUserInfo))
                claimsCfg.ListaClaimsPerRiconoscimento.Add(GetClaimObj("cognome", GetSamlKeyFromGiasKey("cognome", claimsCfg), spidUserInfo))

        End Select
    End Sub

    ''' <summary>
    ''' Ottiene la chiave SAML corrispondente alla chiave GIAS dalla configurazione claims
    ''' </summary>
    ''' <param name="giasKey">La chiave GIAS da mappare</param>
    ''' <param name="claimsCfg">La configurazione dei claims contenente i mapping</param>
    ''' <returns>La chiave SAML corrispondente, o Nothing se non trovata</returns>
    Private Shared Function GetSamlKeyFromGiasKey(giasKey As String, claimsCfg As SMLClaimsCfg) As String
        Dim valore = claimsCfg.ListaClaims.FirstOrDefault(Function(c) String.Equals(c.GiasKey, giasKey, StringComparison.OrdinalIgnoreCase))?.SAMLKey
        Return valore
    End Function

    Private Shared Function GetSMLClaimsCfg(
        spidClaimsCfg As String,
        spidUserInfo As Dictionary(Of String, String),
        cfgConfig As MostraLoginSPIDCfg
    ) As SMLClaimsCfg
        Dim claimsCfg As SMLClaimsCfg
        If String.IsNullOrEmpty(spidClaimsCfg) Then
            claimsCfg = New SMLClaimsCfg With {
                .TipoDiSAMLClaimPerRiconoscereUtente = enum_SAML_RiconoscimentoUtente.CodiceFiscale
            }
        Else
            claimsCfg = JsonConvert.DeserializeObject(Of SMLClaimsCfg)(spidClaimsCfg)
        End If
        AddRequiredClaims(claimsCfg, spidUserInfo)

        'Checks the presence of additional required attributes and that their value is in the accepted domain
        If cfgConfig IsNot Nothing AndAlso cfgConfig.claimValidi.Any AndAlso (
               Not cfgConfig.claimValidi.All(Function(c) spidUserInfo.ContainsKey(c.attributeName)) OrElse
               Not cfgConfig.claimValidi.
                   All(Function(c) Not c.attributeValues.Any OrElse c.attributeValues.Contains(spidUserInfo(c.attributeName)))
        ) Then
            Throw New Exception(AgronicaAgenda_2010.AutenticazioneFallita & ". ")
        End If
        Return claimsCfg
    End Function

    Private Shared Sub LeggiParametriSamlWebConfigServerSuperServer(
        ByRef objParametri_Super_Server As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByVal chiave As String,
        ByRef valore As String)

        valore = ""

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        If objParametri_Server IsNot Nothing Then
            valore = confSiti.Leggi_Valore(0, chiave, "", "", objParametri_Server)
        End If

        If String.IsNullOrEmpty(valore) Then
            valore = confSiti.Leggi_Valore(0, chiave, "", "", objParametri_Super_Server)
        End If

    End Sub


    Private Shared Sub LeggiParametriSamlWebConfig(
        ByRef objParametri_Super_Server As AgronicaCoreParametri,
        ByRef objParametri_Server As AgronicaCoreParametri,
        ByRef SPID_CERTIFICATE_NAME As String,
        ByRef SPID_CERTIFICATE_NAME_CALLBACK As String,
        ByRef SPID_DOMAIN_VALUE As String,
        ByRef SPID_ENVIROMENT As String,
        ByRef SPID_CLAIMS_ARRAY As String,
        ByRef SPID_AssertionConsumerServiceURL As String,
        ByRef SPID_LoginURL As String,
        ByRef SPID_IDP As String,
        ByRef SPID_WindowsFindBy As String,
        ByRef SPID_WindowsFindBy_Callback As String,
        ByRef SPID_AgroSamlConfig As String
    )

        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_CERTIFICATE_NAME", SPID_CERTIFICATE_NAME)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_CERTIFICATE_NAME_CALLBACK", SPID_CERTIFICATE_NAME_CALLBACK)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_DOMAIN_VALUE", SPID_DOMAIN_VALUE)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_ENVIROMENT", SPID_ENVIROMENT)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_CLAIMS_ARRAY", SPID_CLAIMS_ARRAY)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_WindowsFindBy", SPID_WindowsFindBy)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_WindowsFindBy_Callback", SPID_WindowsFindBy_Callback)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_AssertionConsumerServiceURL", SPID_AssertionConsumerServiceURL)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_LoginURL", SPID_LoginURL)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_IDP", SPID_IDP)
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "SPID_AgroSamlConfig", SPID_AgroSamlConfig)

    End Sub

    Private Sub ImpostaVersioneLogin(objParametri_Super_Server As AgronicaCoreParametri)

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        _loginVersione = confSiti.Leggi_Valore(0, "VersioneLogin", "", "", objParametri_Super_Server)
        If String.IsNullOrEmpty(_loginVersione) OrElse String.IsNullOrWhiteSpace(_loginVersione) Then
            _loginVersione = VERSIONE_LOGIN_DEFAULT
        End If

    End Sub

    Private Sub SetLoginVersione(versione As String)
        If Not _loginVersioniDisponibili.Contains(versione) Then
            Throw New Exception("Versione Login " & versione & " sconosciuta, valori ammessi: " & String.Join(",", _loginVersioniDisponibili))
        End If
        _loginVersione = versione
    End Sub


    Private Sub MostraPannelloPersonalizzato(objParametri_Super_Server As AgronicaCoreParametri, p As Object)

        Dim paramSessioneObjParametriValue As String = ""
        Dim sPnlPersonalizzato As String = ""

        If Me.Page.ToString = "ASP.index_aspx" Then
            paramSessioneObjParametriValue = "ASG_objParametri_Server_IndexAspx"
        End If

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim objParametri_Server1 As AgronicaCoreParametri = Session(paramSessioneObjParametriValue)

        If Not IsNothing(objParametri_Server1) Then
            sPnlPersonalizzato = confSiti.Leggi_Valore(0, "paginaIndex_PnlPersonalizzatoBasso", "", "", objParametri_Server1)

        End If
        'cerca su db server

        If sPnlPersonalizzato = "" Then
            sPnlPersonalizzato = confSiti.Leggi_Valore(0, "paginaIndex_PnlPersonalizzatoBasso", "", "", objParametri_Super_Server)
        End If


        If sPnlPersonalizzato <> "" Then
            pnlBassoPersonalizzato.Controls.Add(New LiteralControl(sPnlPersonalizzato))
        End If
    End Sub

    Friend Class SpidAttribute
        Public Property attributeName As String
        Public Property attributeValues As IEnumerable(Of String) = New List(Of String)
    End Class

    Friend Class MostraLoginSPIDCfg
        Public Property PathLogo As String
        Public Property Testo As String
        ''' <summary>
        ''' If set to True, only the SSO login button is shown while the credentials' inputs are hidden.
        ''' </summary>
        Public Property onlySpid As Boolean = False
        ''' <summary>
        ''' Indicates the parameters with which the credentials' inputs are shown (even if <tt>onlySpid</tt> is True).
        ''' </summary>
        Public Property queryStringShowUsername As IEnumerable(Of KeyValuePair(Of String, String)) = New List(Of KeyValuePair(Of String, String))
        ''' <summary>
        ''' Indicates the SAML request attributes that must be valorized and their acceptable values
        ''' </summary>
        Public Property claimValidi As IEnumerable(Of SpidAttribute) = New List(Of SpidAttribute)
    End Class

    Private Shared Function GetSpidConfig(objParametri_Super_Server As AgronicaCoreParametri) As MostraLoginSPIDCfg
        Dim sPnlcfgSPID As String = ""
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, Nothing, "paginaIndex_LoginSPIDCFG", sPnlcfgSPID)
        If String.IsNullOrEmpty(sPnlcfgSPID) Then
            Return New MostraLoginSPIDCfg
        End If
        Return JsonConvert.DeserializeObject(Of MostraLoginSPIDCfg)(sPnlcfgSPID)
    End Function

    Private Sub MostraLoginSPID(objParametri_Super_Server As AgronicaCoreParametri)
        Dim sPnlSPID As String = ""
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, Nothing, "paginaIndex_LoginSPID", sPnlSPID)

        If sPnlSPID = "true" Then
            divSPID.Visible = True
            Dim cfg = GetSpidConfig(objParametri_Super_Server)
            If cfg IsNot Nothing Then
                SetSpidLoginLogo(cfg)
                HideLoginCredentialsDiv(cfg)
            End If
            'SPFI CFG
        End If
        'SPID = true
    End Sub

    Private Sub SetSpidLoginLogo(cfg As MostraLoginSPIDCfg)
        If Not String.IsNullOrEmpty(cfg.PathLogo) Then

            Dim spidImageURL = cfg.PathLogo

            If Debugger.IsAttached AndAlso Not spidImageURL.ToLowerInvariant.Contains("http") Then
                spidImageURL = "http://localhost" & spidImageURL
            End If

            btnSPIDImage.ImageUrl = spidImageURL
            divSPIDImage.ImageUrl = spidImageURL
        End If
        If Not String.IsNullOrEmpty(cfg.Testo) Then
            btnSPID.Text = cfg.Testo
            divSPIDImage.AlternateText = cfg.Testo
        End If
    End Sub

    ''' <summary>
    ''' Hides the login credentials' div if the key <tt>onlySpid</tt> is set to True and
    ''' the page is NOT called with the <tt>queryStringShowUsername</tt> parameters as QueryString.
    ''' </summary>
    Private Sub HideLoginCredentialsDiv(cfg As MostraLoginSPIDCfg)
        _showCredentialsInputs = Not cfg.onlySpid

        If cfg.onlySpid AndAlso cfg.queryStringShowUsername.Any AndAlso
            Request.QueryString.Count = cfg.queryStringShowUsername.Count Then

            _showCredentialsInputs = cfg.queryStringShowUsername.
                    All(Function(p)
                            Dim qryStrVal = Request.QueryString.Item(p.Key)
                            Return qryStrVal IsNot Nothing AndAlso qryStrVal = p.Value
                        End Function)
        End If
    End Sub

    Private Sub MostraAssistenza(objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri, objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim xAssistenza As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim sAssistenza As String = xAssistenza.Assistenza(objParametri_Super_Server, objParametri_Server)

        AgroMasterPage.sAssistenzaInfo = sAssistenza

    End Sub



    '--------------------------------------------------------------
    '-----------------superserver 2014-----------------------
    '--------------------------------------------------------------
    Sub carica_Combo_Servers(ByVal filtriSito As AgronicaCoreDataProvider.AgronicaCoreParametriFiltroIngressoSitoOnline)
        pan_superserver.Visible = True
        Cmb_Server.Visible = True
        'Btn_Server.Visible = True

        Dim conn As New AgronicaCoreUtility.CaricaListControl
        conn.Connessioni(Cmb_Server, False, "", "",
                            filtriSito.ID_DB,
                            filtriSito.TipoDB,
                            filtriSito.Server,
                            filtriSito.DB,
                            filtriSito.Provider,
                            filtriSito.UserId,
                            filtriSito.Password,
                            filtriSito.PivaSuperUser,
                            filtriSito.Note,
                            filtriSito.Progressivo,
                            filtriSito.Descrizione,
                            filtriSito.Filtro_Temp_Inizio,
                            filtriSito.Filtro_Temp_Fine,
                            "", " Descrizione, PivaSuperUser, Server, Progressivo desc, DB ", objParametri_Super_Server)


        cambiatoServerSelezionato()

        If Not Page.IsPostBack AndAlso Cmb_Server.Items.Count = 1 Then


            Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore

            Dim indexObjParametri_Server As AgronicaCoreParametri

            Dim stringaConnessione_Server As String

            AgronicaCoreGestioneRichieste.Inizializzatore.Ricavo_Parametri_Server_Utenti_Da_Id_Db_Server(
                objParametri_Super_Server,
                CInt(Cmb_Server.SelectedValue.Split("|")(0)),
                "", "", "", "", stringaConnessione_Server, "", "", "", "", 0, 0)


            Dim objParametriHLP As New AgronicaCoreParametri_Helper

            indexObjParametri_Server = objParametriHLP.Crea_ObjParametri(
                New DateTime(&H851055320574000),
                New DateTime(&H9325D82E8380000),
                enumCancellazioneLogica.CancellazioneFisica,
                enumVisibilita.Visibilita_Tutti,
                "",
                "",
                "",
                "",
                "",
                "",
                stringaConnessione_Server)


            Session("ASG_objParametri_Server_IndexAspx") = indexObjParametri_Server
        End If

    End Sub


    '--------------------------------------------------------------
    '-----------------superserver 2014-----------------------
    '--------------------------------------------------------------
    Private Sub cambiatoServerSelezionato()

        'pulisco la sessione
        Session("ASG_objParametri_Server") = Nothing
        Session("ASG_objParametri_Utenti") = Nothing


        'se ho un solo server nascondo la combo e faccio vedere solo una label
        If Cmb_Server.Items.Count = 0 Then
            pan_superserver.Visible = True
            Cmb_Server.Visible = True
            lbl_cmb_server.Visible = True
            lbl_cmb_server.Text = "Non sono stati trovati server"
            btnCambiaPassword.Visible = False
            btnTroubleLogin.Visible = False
            btnRichiestaIscrizione.Visible = False
            Exit Sub
        End If

        Dim dbselezionato As String = Cmb_Server.SelectedValue
        If dbselezionato.Split("|").Length <> 6 Then
            Throw New Exception("Formato non corretto nel valore della combo")
        End If

        Dim ID_DB_Sel As String = dbselezionato.Split("|")(0)
        Dim TipoDB_Sel As String = dbselezionato.Split("|")(1)
        Dim Server_Sel As String = dbselezionato.Split("|")(2)
        Dim DB_Sel As String = dbselezionato.Split("|")(3)
        Dim PivaSuperUser_Sel As String = dbselezionato.Split("|")(4)
        Dim Progressivo_Sel As String = dbselezionato.Split("|")(5)


        Dim obj As New AgronicaCoreDataProvider.Connessioni
        Dim dt As DataTable = obj.Leggi(ID_DB_Sel,
                                        TipoDB_Sel,
                                        Server_Sel,
                                        DB_Sel,
                                        "",
                                        "",
                                        "",
                                        PivaSuperUser_Sel,
                                        "",
                                        Progressivo_Sel,
                                        "",
                                        AGRODATAINIZIO,
                                        AGRODATAFINE,
                                        "",
                                        "",
                                        objParametri_Super_Server)

        If dt.Rows.Count = 0 Then
            Throw New Exception("Non è stato trovato il record del server gias selezionato")
        End If
        If dt.Rows.Count > 1 Then
            Throw New Exception("Sono stati caricati più server gias, non è consentito")
        End If

        'StringaConnessione_Server = "Provider=" & dt.Rows(0).Item("Provider") & ";Server=" & dt.Rows(0).Item("Server") & ";Initial Catalog=" & dt.Rows(0).Item("DB") & ";User Id=" & dt.Rows(0).Item("UserId") & ";Password=" & dt.Rows(0).Item("Password") & ";"

        Dim Ritorno_Descrizione As String = dt.Rows(0).Item("Descrizione")
        Dim Ritorno_Note As String = dt.Rows(0).Item("Note")

        Label1.Text = Ritorno_Descrizione
        Label2.Text = Server_Sel
        Label3.Text = DB_Sel
        Label4.Text = Ritorno_Note


        'se ho un solo server nascondo la combo e faccio vedere solo una label
        If Cmb_Server.Items.Count = 1 Then
            pan_superserver.Visible = True
            pan_superserver.Style.Add("display", "block")
            pan_superserver.Style.Add("Text-align", "center")
            pan_superserver.Style.Add("margin-bottom", "25px")
            Cmb_Server.Visible = False
            lbl_dominio.Visible = False
            lbl_cmb_server.Visible = True
            lbl_cmb_server.Text = Ritorno_Descrizione
        ElseIf Cmb_Server.Items.Count = 0 Then
            pan_superserver.Visible = True
            Cmb_Server.Visible = True
            lbl_cmb_server.Visible = True
            lbl_cmb_server.Text = "Non sono stati trovati server"
        Else
            pan_superserver.Visible = True
            Cmb_Server.Visible = True
            lbl_cmb_server.Visible = False
            lbl_cmb_server.Text = ""
        End If

        'Sposto il focus una volta che si decide l'archivio
        Txt_username.Focus()

        'Leggo da configurazione_siti l'impostazione per scegliere se mostrare la funzionalità di richiesta iscrizione a Gias.
        'Per farlo creo un objParametri_Server temporaneo
        'La chiave può valere "0" => Funzionalità disabilitata, "1" => Funzionalità abilitata. Se la chiave non viene trovata considero il valore a zero.
        Dim strRichiestaIscrizioneGias As String
        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(CInt(Cmb_Server.SelectedValue.Split("|")(0)), objParametri_Server, objParametri_Utenti)

        'La funzione permette di cercare prima su db_server e poi, in caso la chiave non sia trovata, su super_server
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, objParametri_Server, "RichiestaIscrizioneGias", strRichiestaIscrizioneGias)

        Dim richiestaIscrizioneGias As Boolean = False
        If Not String.IsNullOrWhiteSpace(strRichiestaIscrizioneGias) Then
            richiestaIscrizioneGias = strRichiestaIscrizioneGias
        End If
        If richiestaIscrizioneGias Then
            btnRichiestaIscrizione.Visible = True
        Else
            btnRichiestaIscrizione.Visible = False
        End If

    End Sub


    '--------------------------------------------------------------
    '-----------------superserver 2014-----------------------
    '--------------------------------------------------------------
    'Private Sub Btn_Server_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Btn_Server.Click

    '    apriOnlineConDBSelezionato()

    'End Sub


    '--------------------------------------------------------------
    '-----------------superserver 2014-----------------------
    '--------------------------------------------------------------
    Private Sub apriOnlineConDBSelezionato()

        '----------------------------------------------------------------------------------------------
        ' Gestisco il caso con superserer o standard come nell'onload
        '----------------------------------------------------------------------------------------------

        Dim Ritorno_objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim Ritorno_objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

        If Not IsNothing(Session("ASG_objParametri_Super_Server")) Then

            '----------------------------------------------------------------------------------------------
            ' OBJPARAMETRI SUPER SERVER
            '----------------------------------------------------------------------------------------------

            'se è stato creato Session("ASG_objParametri_Super_Server") allora sono nel caso son super_server controllo comunque se la chiave nel webconfig è true ok
            If AppSettings("Super_Server") <> "true" Then
                Throw New Exception("E' stato creato l'objparametri per il super_server ma la chiave nel webconfig Super_Server non è true, questo non è ammissibile ")
            End If

            Dim dbselezionato As String = Cmb_Server.SelectedValue

            If dbselezionato.Split("|").Length <> 6 Then
                Throw New Exception("Formato non corretto nel valore della combo")
            End If

            Dim ID_DB_Sel As String = dbselezionato.Split("|")(0)

            Dim Ritorno_Descrizione As String = ""
            Dim Ritorno_Note As String = ""

            Dim Ritorno_Descrizione_Utenti As String = ""
            Dim Ritorno_Note_Utenti As String = ""

            Try

                Dim username As String = ""
                Dim password As String = ""

                Dim xToken As String = Request.QueryString("token")
                If Not String.IsNullOrEmpty(Request.QueryString("tt")) Then
                    Utility_Sicurezza.Leggi_Token(xToken, objParametri_Super_Server)
                End If

                If Not String.IsNullOrEmpty(xToken) Then

                    If Not String.IsNullOrEmpty(Request.QueryString("username")) Then
                        username = Request.QueryString("username")
                    End If

                    Dim aU As New AgronicaCoreUtentiDAL.AutenticaUtente
                    objParametri_Super_Server = Session("ASG_objParametri_Super_Server")
                    Dim AccessBlocked As Boolean = False
                    Try
                        Dim CUAACheck As String = Request.QueryString("CUAA")
                        aU.ASG_Autenticazione_Utente_viaToken(xToken, username, password, ID_DB_Sel, objParametri_Super_Server, IIf(CUAACheck = "", False, True))
                    Catch ex As AgroEccezioni_LoginFallito_Exception
                        Dim redirectUrl As String = AppSettings("ColdirettiDemetra_LoginFailureRedirect")
                        If redirectUrl <> "" Then
                            Response.Redirect(redirectUrl)
                            Exit Sub
                        End If
                    End Try
                Else

                    username = Txt_username.Text
                    password = Txt_Password.Text

                End If

                'se l'autenticazione via token fallisce, nome utente e password sono vuoti, quindi scatta l'eccezione
                If username = "" Then
                    Throw New Exception(AgronicaAgenda_2010.SpecificareUnaUsername)
                End If
                If password = "" Then
                    Throw New Exception(AgronicaAgenda_2010.SpecificareUnaPassword)
                End If

                Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore

                NumeroMassimoTentativiAccessoImposta()

                Dim xFiltroVerificaLock As String = ""
                If const_MaxNumeroTentativiAccesso > 0 Then
                    xFiltroVerificaLock = " utenti.flag = 0 "
                End If

                Try

                    'li creo, ma tanto la funzione chiamata li salva in sessione assieme alle variabili utili per la retrocompatibilità
                    inizializza.Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine(
                    username,
                    password,
                    CInt(ID_DB_Sel),
                    Ritorno_objParametri_Server,
                    Ritorno_objParametri_Utenti,
                    Ritorno_Descrizione,
                    Ritorno_Note,
                    Ritorno_Descrizione_Utenti,
                    Ritorno_Note_Utenti,
                    objParametri_Super_Server,
                    Session,
                    xFiltroVerificaLock)

                    '----------------------------------------------------------------------------------------------
                    ' VERIFICA CHIAVE LICENZA
                    '----------------------------------------------------------------------------------------------

                    If Not VerificaChiaveNew(Ritorno_objParametri_Server, Ritorno_objParametri_Utenti) Then

                        'Throw New Exception("La chiave licenza è scaduta. Contattare l'amministratore.")
                        'Exit Sub

                    End If

                    If const_MaxNumeroTentativiAccesso > 0 Then
                        'login ok, reinizializzo contatore
                        Dim xReinizializza As New AgronicaCoreUtentiBIZ.Utenti
                        xReinizializza.NumeroTentativiAccessoScrivi(username, 0, Ritorno_objParametri_Utenti)

                    End If

                Catch ex As AgroEccezioni_LoginFallito_Exception

                    'nessuna verifica necessaria...
                    If const_MaxNumeroTentativiAccesso > 0 Then
                        Dim bShouldExit As Boolean = VerificaNumeroTentativiAccesso(username, Ritorno_objParametri_Utenti, objParametri_Super_Server)
                        If bShouldExit Then

                            Dim msg As String = AgronicaAgenda_2010.UtenteBloccatoReimpostareLaPassword

                            Throw New Exception(ex.Message & "<br/>" & msg)
                        Else
                            Throw ex
                        End If

                    Else

                        Throw ex

                    End If

                End Try

                'CHIAMATA API

                Session("ID_DB_Sel") = CInt(ID_DB_Sel)

                '--------------------------------------------------------
                ' Carico l'elenco delle Icone Specie Vegetali
                '--------------------------------------------------------

                Dim Elenco_Icone_SpecieVegetali As String

                'Call Recupera_Elenco_Icone_SpecieVegetali(Server, Session, Page, Elenco_Icone_SpecieVegetali)
                AgronicaCoreUtility.Icone.Recupera_Elenco_Icone_SpecieVegetali(Me.Server, "AB_Immagini/IconeVegetali", Elenco_Icone_SpecieVegetali)

                Session("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali

                'Response.Redirect("./Menu/MenuBS_Agenda_Nuovo.aspx")
                'Dim scriptText = ""
                'scriptText &= " $(document).ready(function () { "
                'scriptText &= "    document.location='./Menu/MenuBS_Agenda_Nuovo.aspx';"
                'scriptText &= " }); "
                'ScriptManager.RegisterStartupScript(Page, Page.GetType(), String.Format("jQuery_{0}", Page.ClientID), scriptText, True)

                Dim AccessoSoloSPID As Boolean = False

                Try

                    Dim leggiUtenti As New Utenti_Read
                    Dim dtUtenti As DataTable = leggiUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, " Username = N'" & username & "'", "", Ritorno_objParametri_Utenti)

                    If String.IsNullOrEmpty(xToken) Then
                        AccessoSoloSPID = ControllaSeAccessoSoloSPID(Ritorno_objParametri_Server, password, dtUtenti)
                    End If

                    Dim leggiLingua As New Lingue_Read
                    Dim lLingua_cod_Utente As Integer = dtUtenti.Rows(0)("Lingua_COD")
                    Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(lLingua_cod_Utente, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", Ritorno_objParametri_Utenti)

                    Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
                    ImpostaCultura(New Lingua With {.CodiceISO = linguaCodiceISO, .Lingua_cod = lLingua_cod_Utente})

                    Ritorno_objParametri_Server.Lingua_Cod = dtUtenti.Rows(0)("Lingua_COD")

                Catch ex As Exception
                    ImpostaCultura(New Lingua With {.CodiceISO = "it", .Lingua_cod = 1})
                End Try

                If AccessoSoloSPID Then
                    Throw New Exception("Accesso consentito solo tramite SPID/CIE")
                End If

                '-----------------------------------------------------------------------------------------------
                '(17/11/2014) gestione tabella Utenti_Visibilita_Appoggio e ricarico i dati che può visualizzare
                '-----------------------------------------------------------------------------------------------
                InitVisbility(Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)

                LoginCoreAPICompleto(username, password, ID_DB_Sel, Ritorno_objParametri_Server)

                SetCookie("UserLng", Ritorno_objParametri_Utenti.Lingua_Cod)

                FinalizzaLogin_EseguiRedirect(Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)
            Catch ex As Exception

                Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf & ex.Message, Page, UpdatePanel:=updateScript)

            End Try

            '----------------------------------------------------------------------------------------------
            ' FINE OBJPARAMETRI SUPER SERVER
            '----------------------------------------------------------------------------------------------

        Else

            '----------------------------------------------------------------------------------------------
            ' GIAS SERVER STANDARD
            '----------------------------------------------------------------------------------------------

            'se non è stato creato Session("ASG_objParametri_Super_Server") allora sono nel caso senza superserver
            'ma con un unico db gias_server standard, quindi gestisco la creazione dei parametri dal connessioni.ini

            'se la chiave nel webconfig è false è ok
            If AppSettings("Super_Server") = "true" Then
                Throw New Exception("Non è stato creato l'objparametri per il super_server ma la chiave nel webconfig Super_Server è true, questo non è ammissibile ")
            End If

            If Session("ASG_StringaConnessione_Server") = "" Or Session("ASG_StringaConnessione_Utenti") = "" Then
                Throw New Exception("La stringa di connessione per il server gias o utenti è vuota ma la chiave nel webconfig Super_Server è false, questo non è ammissibile ")
            End If

            Me.Autentica(Me.Txt_username.Text, Me.Txt_Password.Text, False)

            '----------------------------------------------------------------------------------------------
            ' FINE GIAS SERVER STANDARD
            '----------------------------------------------------------------------------------------------

        End If
    End Sub


    ''' <remarks><list type="table">
    ''' <item> (17/11/2014) gestione tabella Utenti_Visibilita_Appoggio e ricarico i dati che può visualizzare. </item>
    ''' </list></remarks>
    Private Sub InitVisbility(Ritorno_objParametri_Server As AgronicaCoreParametri, Ritorno_objParametri_Utenti As AgronicaCoreParametri)
        Dim xVisibAppoggio As New AgronicaCoreUtentiBIZ.Utenti
        xVisibAppoggio.InizializzaTabellaUtentiVisibilitaAppoggio(Session("ASG_Utente_Username").ToString,
                                                                  CInt(Session("ASG_IdServizio")),
                                                                  Ritorno_objParametri_Server,
                                                                  Ritorno_objParametri_Utenti)
        'pulisco la tabella con il filtro temporaneo per gli impianti
        xVisibAppoggio.InizializzaTabella__tmp_FiltroImpianti(Ritorno_objParametri_Server)
    End Sub

    Private Shared Function ControllaSeAccessoSoloSPID(ByRef Ritorno_objParametri_Server As AgronicaCoreParametri,
                                                       ByRef password As String,
                                                       dtUtenti As DataTable) As Boolean

        Dim AccessoSoloSPID As Boolean = False

        Try

            If Not IsNothing(dtUtenti) AndAlso dtUtenti.Rows.Count > 0 Then
                Dim FlagAccessoSPID = dtUtenti.Rows(0)("Flag_Accesso_SPID")
                AccessoSoloSPID = If(IsDBNull(FlagAccessoSPID), 0, FlagAccessoSPID) = 1
                If AccessoSoloSPID Then
                    'Se PPT posso accede senza SPID
                    Dim autenticaDal As New AgronicaCoreUtentiDAL.AutenticaUtente
                    If autenticaDal.ASG_Verifica_PPT(password, Ritorno_objParametri_Server) Then
                        AccessoSoloSPID = False
                    End If
                End If
            End If

        Catch ex As Exception
        End Try

        Return AccessoSoloSPID

    End Function

    Private Sub LoginCoreAPICompleto(username As String, password As String, ID_DB As Integer, objParametri_Server As AgronicaCoreParametri)
        apiController.Inizializza(objParametri_Super_Server, objParametri_Server)
        If apiController.CanUseAPI Then
            Dim coreApiCookies = apiController.Login(username, password, ID_DB)
            If apiController.PasswordScaduta Then
                apriPagCambiaPassword()
                Return
            End If
            'If coreApiCookies.Count > 0 And Not String.IsNullOrEmpty(Request.QueryString("token")) Then
            '    Response.Cookies.Set()
            'End If
            For Each cookie In coreApiCookies
                If Not Response.Cookies.Item(cookie.Name) Is Nothing AndAlso
                    Not Request.Cookies.Item(cookie.Name) Is Nothing Then
                    'Il cookie c'è già sia nella response, che nella request
                    Response.Cookies.Set(cookie)
                ElseIf Response.Cookies.Item(cookie.Name) Is Nothing AndAlso
                    Not Request.Cookies.Item(cookie.Name) Is Nothing Then
                    'il cookie c'è nella request ma non nella response
                    Response.Cookies.Add(cookie)
                ElseIf Not Response.Cookies.Item(cookie.Name) Is Nothing AndAlso
                     Request.Cookies.Item(cookie.Name) Is Nothing Then
                    'il cookie c'è nella response ma non nella request
                    Response.Cookies.Set(cookie)
                Else
                    'il cookie non c'è né nella request né nella response
                    Response.Cookies.Add(cookie)
                End If
            Next
        End If
    End Sub

    Private Sub apriOnlineConDBSelezionatoSAML(cfgRiconoscimentoUtente As SMLClaimsCfg)
        '##############################################################################################
        'gestisco il caso con superserer o standard come nell'onload
        '##############################################################################################

        Dim Ritorno_objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
        Dim Ritorno_objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

        If Not IsNothing(Session("ASG_objParametri_Super_Server")) Then

            '##############################################################################################
            '####################     OBJPARAMETRI SUPER SERVER ###########################################
            '##############################################################################################

            'se è stato creato Session("ASG_objParametri_Super_Server") allora sono nel caso son super_server
            ' controllo comunque 
            'se la chiave nel webconfig è true ok
            If AppSettings("Super_Server") <> "true" Then
                Throw New Exception("E' stato creato l'objparametri per il super_server ma la chiave nel webconfig Super_Server non è true, questo non è ammissibile ")
            End If

            Dim dbselezionato As String = Cmb_Server.SelectedValue

            If dbselezionato.Split("|").Length <> 6 Then
                Throw New Exception("Formato non corretto nel valore della combo")
            End If

            Dim ID_DB_Sel As String = dbselezionato.Split("|")(0)

            Dim Ritorno_Descrizione As String = ""
            Dim Ritorno_Note As String = ""

            Dim Ritorno_Descrizione_Utenti As String = ""
            Dim Ritorno_Note_Utenti As String = ""

            Try

                Dim username As String = ""
                Dim password As String = ""

                Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore

                NumeroMassimoTentativiAccessoImposta()

                Dim xFiltroVerificaLock As String = ""
                If const_MaxNumeroTentativiAccesso > 0 Then
                    xFiltroVerificaLock = " utenti.flag = 0 "
                End If

                Try

                    'li creo, ma tanto la funzione chiamata li salva in sessione assieme 
                    'alle variabili utili per la retrocompatibilità
                    inizializza.Inizializza_Sito_Specifico_Con_Superserver_GiasOnLine_SAML(
                    cfgRiconoscimentoUtente,
                    username,
                    password,
                    CInt(ID_DB_Sel),
                    Ritorno_objParametri_Server,
                    Ritorno_objParametri_Utenti,
                    Ritorno_Descrizione,
                    Ritorno_Note,
                    Ritorno_Descrizione_Utenti,
                    Ritorno_Note_Utenti,
                    objParametri_Super_Server,
                    Session,
                    xFiltroVerificaLock,
                    Request("btn_accountSelezionato"),
                    If(Request("btn_accountSelezionato") IsNot Nothing, ddl_account.SelectedValue, ""))

                    If const_MaxNumeroTentativiAccesso > 0 Then
                        'login ok, reinizializzo contatore
                        Dim xReinizializza As New AgronicaCoreUtentiBIZ.Utenti
                        xReinizializza.NumeroTentativiAccessoScrivi(username, 0, Ritorno_objParametri_Utenti)

                    End If

                Catch ex As AgroEccezioni_LoginFallito_Exception

                    'nessuna verifica necessaria...
                    If const_MaxNumeroTentativiAccesso > 0 Then
                        Dim bShouldExit As Boolean = VerificaNumeroTentativiAccesso(username, Ritorno_objParametri_Utenti, objParametri_Super_Server)
                        If bShouldExit Then

                            Dim msg As String = AgronicaAgenda_2010.UtenteBloccatoReimpostareLaPassword

                            Throw New Exception(ex.Message & " " & msg)
                        Else
                            Throw ex
                        End If

                    Else

                        Throw ex

                    End If

                End Try

                Session("ID_DB_Sel") = CInt(ID_DB_Sel)

                '###############################################################################################################
                '##################################### VERIFICA CHIAVE LICENZA #################################################
                '###############################################################################################################

                If Not VerificaChiaveNew(Ritorno_objParametri_Server, Ritorno_objParametri_Utenti) Then

                    'Throw New Exception("La chiave licenza è scaduta. Contattare l'amministratore.")
                    'Exit Sub


                End If

                '--------------------------------------------------------
                '----- Carico l'elenco delle Icone Specie Vegetali ------
                '--------------------------------------------------------

                Dim Elenco_Icone_SpecieVegetali As String

                'Call Recupera_Elenco_Icone_SpecieVegetali(Server, Session, Page, Elenco_Icone_SpecieVegetali)
                AgronicaCoreUtility.Icone.Recupera_Elenco_Icone_SpecieVegetali(Me.Server, "AB_Immagini/IconeVegetali", Elenco_Icone_SpecieVegetali)

                Session("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali

                'Response.Redirect("./Menu/MenuBS_Agenda_Nuovo.aspx")
                'Dim scriptText = ""
                'scriptText &= " $(document).ready(function () { "
                'scriptText &= "    document.location='./Menu/MenuBS_Agenda_Nuovo.aspx';"
                'scriptText &= " }); "
                'ScriptManager.RegisterStartupScript(Page, Page.GetType(), String.Format("jQuery_{0}", Page.ClientID), scriptText, True)

                Try

                    Dim leggiUtenti As New Utenti_Read
                    Dim dtUtenti As DataTable = leggiUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, " Username = N'" & username & "'", "", Ritorno_objParametri_Utenti)

                    Dim leggiLingua As New Lingue_Read
                    Dim lLingua_cod_Utente As Integer = dtUtenti.Rows(0)("Lingua_COD")
                    Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(lLingua_cod_Utente, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", Ritorno_objParametri_Utenti)

                    Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
                    ImpostaCultura(New Lingua With {.CodiceISO = linguaCodiceISO, .Lingua_cod = lLingua_cod_Utente})

                    Ritorno_objParametri_Server.Lingua_Cod = dtUtenti.Rows(0)("Lingua_COD")

                Catch ex As Exception
                    ImpostaCultura(New Lingua With {.CodiceISO = "it", .Lingua_cod = 1})
                End Try

                '-----------------------------------------------------------------------------------------------
                '(17/11/2014) gestione tabella Utenti_Visibilita_Appoggio e ricarico i dati che può visualizzare
                '-----------------------------------------------------------------------------------------------
                Dim xVisibAppoggio As New AgronicaCoreUtentiBIZ.Utenti
                xVisibAppoggio.InizializzaTabellaUtentiVisibilitaAppoggio(
                    Session("ASG_Utente_Username").ToString,
                    CInt(Session("ASG_IdServizio")),
                    Ritorno_objParametri_Server,
                    Ritorno_objParametri_Utenti
                )

                'pulisco la tabella con il filtro temporaneo per gli impianti
                xVisibAppoggio.InizializzaTabella__tmp_FiltroImpianti(Ritorno_objParametri_Server)

                LoginCoreAPICompleto(username, password, ID_DB_Sel, Ritorno_objParametri_Server)

                'apiController.Inizializza(objParametri_Super_Server, Ritorno_objParametri_Server)
                'If apiController.CanUseAPI Then
                '    Dim coreApiCookies = apiController.Login(username, password, ID_DB_Sel)
                '    For Each cookie In coreApiCookies
                '        Response.Cookies.Add(cookie)
                '    Next
                'End If

                LoginCoreAPICompleto(username, password, ID_DB_Sel, Ritorno_objParametri_Server)

                FinalizzaLogin_EseguiRedirect(Ritorno_objParametri_Server, Ritorno_objParametri_Utenti)

            Catch ex2 As RecoverableSPIDMultipleAccountLoginException
                Throw ex2

            Catch ex As Exception

                Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf &
                            ex.Message, Page, UpdatePanel:=updateScript)

            End Try

            '##############################################################################################
            '####################### FINE OBJPARAMETRI SUPER SERVER #######################################
            '##############################################################################################

        Else

            '##############################################################################################
            '####################### GIAS SERVER STANDARD##################################################
            '##############################################################################################

            'se non è stato creato Session("ASG_objParametri_Super_Server") allora sono nel caso senza superserver
            'ma con un unico db gas_server standard, quindi gestisco la creazione dei parametri dal connessioni.ini

            'se la chiave nel webconfig è false è ok
            If AppSettings("Super_Server") = "true" Then
                Throw New Exception("Non è stato creato l'objparametri per il super_server ma la chiave nel webconfig Super_Server è true, questo non è ammissibile ")
            End If

            If Session("ASG_StringaConnessione_Server") = "" Or Session("ASG_StringaConnessione_Utenti") = "" Then
                Throw New Exception("La stringa di connessione per il server gias o utenti è vuota ma la chiave nel webconfig Super_Server è false, questo non è ammissibile ")
            End If

            Me.Autentica(Me.Txt_username.Text, Me.Txt_Password.Text, True)

            '##############################################################################################
            '####################### FINE GIAS SERVER STANDARD#############################################
            '##############################################################################################

        End If

    End Sub

    Private Function VerificaChiaveNew(objParametri_Server As AgronicaCoreParametri, objParametri_Utenti As AgronicaCoreParametri) As Boolean


        '###############################################################################################################
        '##################################### VERIFICA CHIAVE LICENZA #################################################
        '###############################################################################################################


        Dim Data_Scadenza As Date = "01/01/1900"
        Dim auK As New AgronicaCoreUtentiDAL.AutenticaUtente
        Dim urlWS As String
        Dim bVerificaChiaveWS As Boolean = False
        Dim bVerificaChiave As Boolean = False
        Dim TentativiVerifica As Integer = 0
        Dim Chiave_Locale As String = ""
        Dim Chiave_WS As String = ""
        Dim bChiave_Aggiornata As Boolean = False
        Dim GiorniFranchigia_Rimanenti As Integer
        Dim bBlocco_Precedente As Boolean = False

        Dim sms As String = ""
        Dim objUtenti_Permessi_W As New AgronicaCoreUtentiDAL.Utenti_Permessi_W

        'Nota: in questa fase di test la verifica si attiva solo in presenza del record dei giorni di franchigia
        Dim ObjUtenti_Codici As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read
        Dim dtCodice As DataTable = ObjUtenti_Codici.Leggi(enum_Impostazioni_Utenti.SUPERUSER_Licenza_Giorni_Franchigia, 2, 1, "", "", objParametri_Utenti)

        If dtCodice.Rows.Count > 0 Then

            If IsNumeric(dtCodice(0).Item("Impostazione_Valore_2")) Then

                bBlocco_Precedente = CBool(dtCodice(0).Item("Impostazione_Valore_2"))

            End If

            'Ricavo l'URL
            Dim xLeggiConfSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim dtws As DataTable = xLeggiConfSiti.Leggi(0, "GiasOnline_WS_GiasWSAggiorna_AgroWS_GiasWSAggiorna", "", "", objParametri_Server)

            If dtws.Rows.Count > 0 Then
                urlWS = dtws.Rows(0)("valore")
            Else
                urlWS = "https://ws.netagronica.it/AgronicaWebService/Gias_service.asmx"
            End If

            'Controllo Chiave Online ed Eventuale Update
            bVerificaChiaveWS = auK.ASG_GestioneChiave_PPT(Data_Scadenza, Chiave_Locale, Chiave_WS, bChiave_Aggiornata, urlWS, objParametri_Utenti)

            'Al momento nessun messaggio di errore relativo al mandato accesso chiave ws
            Select Case bVerificaChiaveWS

                Case True

                    'Lettura chiave remoto ok ed eventuale allineamento in locale

                Case False

                    'Chiamata remota fallita e verifica chiave locale

            End Select

            'Verifica Validità Chiave Locale
            Select Case auK.ASG_VerificaChiave(Chiave_Locale, bChiave_Aggiornata, bBlocco_Precedente, GiorniFranchigia_Rimanenti, sms, objParametri_Utenti)

                Case True

                    bVerificaChiave = True

                Case False

                    bVerificaChiave = True 'Al momento entra lo stesso

            End Select

        End If

        HttpContext.Current.Session("Chiave_Licenza_Verificata") = "True"

        Return bVerificaChiave

    End Function

    Private Function GDPR_VerificaStatoRichiediAccettazione(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri) As Integer

        Dim xVerifica As New AgronicaCoreUtentiBIZ.GDPR
        Dim xRval As Integer = xVerifica.VerificaStatoAccettazione(objParametri_Server, objParametri_Utenti)

        Return xRval

    End Function


    Private Function GDPR_isAttivo() As Boolean
        Dim xVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtVerifica As DataTable

        dtVerifica = xVerifica.Leggi(0, "GDPR_Attivo", "", "", objParametri_Super_Server)

        Dim xRval As Boolean = True

        If dtVerifica.Rows.Count > 0 Then
            Try
                xRval = CType(dtVerifica(0)("Valore"), Boolean)
            Catch ex As Exception
                xRval = True
            End Try
        Else
            xRval = True
        End If

        Return xRval
    End Function

    Private Sub NumeroMassimoTentativiAccessoImposta()
        Dim xVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtVerifica As DataTable

        dtVerifica = xVerifica.Leggi(0, "login_MaxNumeroTentativiAccesso", "", "", objParametri_Super_Server)

        If dtVerifica.Rows.Count > 0 Then
            const_MaxNumeroTentativiAccesso = dtVerifica(0)("Valore")
        Else
            const_MaxNumeroTentativiAccesso = -1
        End If

    End Sub


    Private Function LeggiAziendaPredefinitaDaCuaa(ByVal cuaa As String,
                                             ByRef piva As String,
                                             ByVal objParametri_Server As AgronicaCoreParametri
                                            ) As Boolean

        piva = String.Empty
        Dim errore As String = String.Empty
        Dim imprese_R As New Imprese_Read()
        Dim dtImprese As DataTable = Nothing
        Dim retVal As Boolean = True

        Try
            dtImprese = imprese_R.RecuperaDatiImpresa_From_Cuaa(cuaa, errore, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            If Not IsNothing(dtImprese) AndAlso dtImprese.Rows.Count > 0 AndAlso String.IsNullOrEmpty(errore) Then
                If dtImprese.Rows.Count <> 1 Then
                    Throw New Exception("Ci sono più imprese con lo stesso cuaa")
                End If
                piva = dtImprese.Rows(0).Item("Piva")
            End If

        Catch ex As Exception
            retVal = False
        End Try

        Return retVal

    End Function


    Private Function LeggiAziendaPredefinita(ByVal objParametri_Server As AgronicaCoreParametri,
                                             ByVal objParametri_Utenti As AgronicaCoreParametri,
                                             ByVal dt_Imprese As DataTable
                                            ) As String

        Dim aziendaPredefinita As String = String.Empty

        ' Leggo c'è una piva associata da impostazione utente
        Dim utImp_R As New Utenti_Impostazioni_Read()
        Dim aziendaDefault = utImp_R.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_AZIENDA_PREDEFINITA_ALL_AVVIO, objParametri_Utenti)

        Try

            If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then


                ' 1) --> provo a guardare nella tabella delle ultime ziende utilizzate
                Dim op_DB_R As New AgronicaCoreVarieDAL.MenuBS_2017_Operazioni_DB_R
                Dim dt As DataTable = op_DB_R.LeggiUltimeAziendeSelezionate(objParametri_Server, objParametri_Utenti)
                If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                    Dim ultimeSelezionate = dt.ToExpandoObject
                    aziendaPredefinita = ultimeSelezionate.FirstOrDefault()("piva")
                Else
                    ' 2) --> guardo tra quelle in visibilità dell'utente
                    If Not IsNothing(dt_Imprese) AndAlso dt_Imprese.Rows.Count > 0 Then
                        Dim impreseUtente = dt_Imprese.ToExpandoObject().OrderBy(Function(impresa) impresa("rag_soc"))
                        aziendaPredefinita = impreseUtente.FirstOrDefault()("PIVA")
                    End If
                End If
            End If


            If String.IsNullOrEmpty(aziendaPredefinita) Then
                aziendaPredefinita = aziendaDefault
            End If

        Catch ex As Exception
            aziendaPredefinita = aziendaDefault
        End Try

        Return aziendaPredefinita

    End Function

    Private Sub FinalizzaLogin_EseguiRedirect(ByVal objParametri_Server As AgronicaCoreParametri, ByVal objParametri_Utenti As AgronicaCoreParametri)

        Dim objAgenda As New AgronicaCoreGestioneRichieste.ParametriAgenda_2010
        objAgenda.Id_Agenda = 0
        objAgenda.Piva = ""
        objAgenda.Veg_Cod = 0

        'Verifico se c'è una piva da impostare all'avvio da impostazione utente
        'Dim utImp_R As New Utenti_Impostazioni_Read()
        'Dim pivaDiAvvio As String = utImp_R.ImpostazioneValore1_from_ImpostazioneCod(enum_Impostazioni_Utenti.UTENTE_COD_AZIENDA_PREDEFINITA_ALL_AVVIO, objParametri_Utenti)

        Dim dtImprese As DataTable = Nothing
        Dim ddlAziendaDiAvvio As New DropDownList()
        AgronicaCoreUtility.CaricaListControl.ImpreseConFiltroUtente(ddlAziendaDiAvvio, True, "", "", "", "", objParametri_Server, objParametri_Utenti, True, dtImprese)

        Dim pivaDiAvvio As String = ""

        Dim CUAA As String = Request.QueryString("cuaa")
        Dim piva As String = Request.QueryString("piva")
        Dim bOk As Boolean = False

        Do While Not bOk

            If Not String.IsNullOrEmpty(CUAA) Then

                ' ////////// CUAA in QueryString -> ha la precedenza sulla eventuale Piva passata in QueryString

                LeggiAziendaPredefinitaDaCuaa(CUAA, pivaDiAvvio, objParametri_Server)

                ' se ho trovato l'impresa attraverso il cuaa controllo se l'utente ce l'ha in visibilità altrimenti cerco la prima in visibilità
                If Not String.IsNullOrEmpty(pivaDiAvvio) Then
                    Dim index As Integer = ddlAziendaDiAvvio.Items.IndexOf(ddlAziendaDiAvvio.Items.FindByValue(pivaDiAvvio))
                    If index < 0 Then
                        'lavez - 16/10/2023 - come concordato con Belmonts\Scatto\Vanni -> se trovo 0 o più pive per singolo CUAA redirect su accesso negato
                        Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
                        Exit Sub
                        'pivaDiAvvio = LeggiAziendaPredefinita(objParametri_Server, objParametri_Utenti, dtImprese)
                    Else
                        bOk = True
                    End If
                Else
                    'lavez - 16/10/2023 - come concordato con Belmonts\Scatto\Vanni -> se trovo 0 o più pive per singolo CUAA redirect su accesso negato
                    Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
                    Exit Sub

                    ' impresa non trovata con il cuaa passato, procedo a cercare tra quelle in visibilità
                    'pivaDiAvvio = LeggiAziendaPredefinita(objParametri_Server, objParametri_Utenti, dtImprese)
                End If

            Else

                ' ///////// Piva in QueryString

                If Not String.IsNullOrEmpty(piva) Then
                    pivaDiAvvio = piva
                Else
                    pivaDiAvvio = LeggiAziendaPredefinita(objParametri_Server, objParametri_Utenti, dtImprese)
                End If

            End If

            ' alla fina se sono riuscito ad ottenere una Piva controllo sempre che sia in visibilità utente
            If Not bOk And pivaDiAvvio <> "" Then
                'Controllo se la piva impostata è tra quelle visibili dall'utente (nel caso venga assegnata erroneamente da un profilo)
                Dim index As Integer = ddlAziendaDiAvvio.Items.IndexOf(ddlAziendaDiAvvio.Items.FindByValue(pivaDiAvvio))
                If index < 0 Then

                    'Controllo che non sia stato passato il CUAA
                    If String.IsNullOrEmpty(CUAA) Then

                        'Nuovo Tentativo Impostando Il CUAA = piva
                        CUAA = pivaDiAvvio
                    Else

                        'pivaDiAvvio = ""
                        'lavez - 16/10/2023 - come concordato con Belmonts\Scatto\Vanni -> se trovo 0 o più pive per singolo CUAA redirect su accesso negato
                        Response.Redirect("~/Classi/Agro_Pages/AccessoNonConsentito.aspx")
                        Exit Sub
                    End If

                Else

                    bOk = True

                End If


            ElseIf pivaDiAvvio = "" Then

                bOk = True

            End If

        Loop

        objAgenda.Piva = pivaDiAvvio

        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuBS_2017", "", "", objParametri_Server)

        Dim paginaRichiestaViaQueryString As String =
            Request.QueryString("rDir")
        Dim sitoRichiestoViaQueryString As String =
            Request.QueryString("rSito")
        Dim goToNgFromAPP As Boolean = False
        If Request.QueryString("ng") = "true" Then
            goToNgFromAPP = True
        End If

        Dim SitoOrigine = Enum_SiteRedirector.GiasNG
        Dim SitoDestinazione = 0
        If Not String.IsNullOrEmpty(paginaRichiestaViaQueryString) Then

            objAgenda.PaginaRichiesta = Sicurezza.Stringa_Decodifica_LANCompatibile(paginaRichiestaViaQueryString, AgroKey_EncoderDecoder).Split("_")(1)

            If Not String.IsNullOrEmpty(sitoRichiestoViaQueryString) Then
                SitoDestinazione = Sicurezza.Stringa_Decodifica_LANCompatibile(sitoRichiestoViaQueryString, AgroKey_EncoderDecoder).Split("_")(1)
            End If

        Else
            If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then

                objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu
                objAgenda.PaginaProvenienza = enum_PagineAgenda_2010.Pagina_index
            Else

                DTConfigSiti = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametri_Server)

                If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 AndAlso LCase(DTConfigSiti.Rows(0).Item("Valore")) = "true" Then

                    If objAgenda.Piva <> "" Then
                        objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu_BS
                        objAgenda.PaginaProvenienza = enum_PagineAgenda_2010.Pagina_index
                    Else
                        objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Pagina_FiltrinoImprese
                        objAgenda.PaginaDestinazioneFiltrino = enum_PagineAgenda_2010.Menu_BS
                        objAgenda.SitoDestinazioneFiltrino = Enum_SiteRedirector.Sito_AgronicaAgenda_2010
                        objAgenda.PaginaProvenienza = enum_PagineAgenda_2010.Pagina_index
                    End If

                Else
                    objAgenda.PaginaRichiesta = enum_PagineAgenda_2010.Menu
                    objAgenda.PaginaProvenienza = enum_PagineAgenda_2010.Menu
                End If
            End If

        End If



        'Loggo il login.
        Dim objAWS_Log As New AgronicaCoreUtentiBIZ.AWS_Log_W
        objAWS_Log.AccessoLog(0, 0, 0, 0, True, objParametri_Server, objParametri_Utenti)

        Dim targetUrl2 As String = "GestioneRichieste.aspx?"


        Dim goToNg As Boolean = True
        Dim listAgendaPages As Integer() = {
            enum_PagineAgenda_2010.Pagina_ConsiglioFertirriguo_App,
            enum_PagineAgenda_2010.Pagina_DSS_Irrigazione_App,
            enum_PagineAgenda_2010.Pagina_DSS_Difesa,
            enum_PagineAgenda_2010.AppWeatherSummary,
            enum_PagineAgenda_2010.DSSWidget,
            enum_PagineAgenda_2010.DSSWidget_ConRdir
        }

        Dim listNgPages As Integer() = {
            enum_PagineGiasNG.Pagina_Trattamento_Zoo
        }
        Dim Parametri_Aggiuntivi As JObject = Nothing
        If listAgendaPages.Contains(objAgenda.PaginaRichiesta) AndAlso
            Not goToNgFromAPP Then
            goToNg = False
        End If

        If listNgPages.Contains(objAgenda.PaginaRichiesta) AndAlso
            goToNgFromAPP Then
            goToNg = True
            SitoOrigine = Enum_SiteRedirector.GiasAPP
            Parametri_Aggiuntivi = New JObject
            Parametri_Aggiuntivi.Item("QSF") = "?seFrame=1"

        End If


        If SitoDestinazione <> 0 AndAlso objAgenda.PaginaRichiesta <> 0 Then
            MenuBS_2017_RedirectGestione.RedirectGenerico(objAgenda.Piva, SitoDestinazione, objAgenda.PaginaRichiesta, targetUrl2, objParametri_Server,
                                                          SitoOrigine:=SitoOrigine, Parametri_Aggiuntivi:=Parametri_Aggiuntivi)


        ElseIf apiController.VersioneHeader = "2022" AndAlso
            apiController.CanUseAPI AndAlso
            Not String.IsNullOrEmpty(paginaRichiestaViaQueryString) AndAlso
            goToNg Then

            'SOLO PER IL NUOVO QDC ANGULAR E SE REDIRECT DA QUERYSTRING
            MenuBS_2017_RedirectGestione.RedirectGenerico(objAgenda.Piva, Enum_SiteRedirector.GiasNG, objAgenda.PaginaRichiesta, targetUrl2, objParametri_Server,
                                                          SitoOrigine:=SitoOrigine, Parametri_Aggiuntivi:=Parametri_Aggiuntivi)

        Else

            ' salta direttamente alla pagina della checklist dopo essersi autenticato con il token
            Dim token As String = Request.QueryString("token")
            If Not String.IsNullOrEmpty(token) AndAlso Request.QueryString("tt") = "audit" Then
                Dim audit = Utility_Sicurezza.Leggi_Token(token, objParametri_Super_Server)
                Dim AuditURL As String = RedirectGestione.IndirizzoCompleto_Sito_AgronicaAudit_PassandoDirettamente_Parametri(
                    Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_AuditPuaTipo.Audit_Fornitori_EUDR_INALCA,
                    objParametri_Server.UtenteCodFiscale, objAgenda.Piva, 0, 360)
                AuditURL &= "&audit=" & HttpUtility.UrlEncode(Utility_Sicurezza.EncryptString(audit, objParametri_Super_Server))
                If Not String.IsNullOrEmpty(Request.QueryString("ln")) Then
                    AuditURL &= "&ln=" & Request.QueryString("ln")
                End If
                Response.Redirect(AuditURL)
            End If

            Dim TargetURL As String = AgronicaCoreGestioneRichieste.RedirectGestione.IndirizzoCompleto_SitoAgenda_PassandoDirettamente_ParametriAgenda_2010(
                                 Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objAgenda)


            targetUrl2 &= TargetURL.Split("?")(1)

            Dim ff As String = Request.QueryString("FF")
            If Not String.IsNullOrEmpty("FF") Then
                targetUrl2 &= "&FF=" & ff
            End If
        End If



        'Apro nella stessa pagina        
        ' VAnni: 27/1/2017: lingua e cultura impostati..
        Dim url As String = targetUrl2 & "&ln=" & CType(Session("LinguaCorrente"), Lingua).CodiceISO & "|" & objParametri_Server.Lingua_Cod

        '(24/07/2018) fede verifico se sono valorizzati i pesi delle sa nelle operazioni
        Dim EsisteChiave As Boolean = False
        If Aggiorna_PesiSA_Operazioni(objParametri_Server, EsisteChiave) = True Then

            Dim objMovDett As New AgronicaCoreContabBIZ.Movimenti_Dettagli_W
            objMovDett.Aggiorna_PrincipiAttiviPesi(EsisteChiave, objParametri_Server, objParametri_Utenti)

        End If


        'If apiController.CanUseAPI AndAlso apiController.VersioneHeader = "2022" Then
        '    MenuBS_2017_RedirectGestione.RedirectGenerico(objAgenda.Piva, Enum_SiteRedirector.GiasNG, enum_PagineGiasNG.Pagina_Dashboard, url, objParametri_Server)
        'End If

        'Se il path e'relativo gl'aggancio host
        Dim CoreApiInstance As New CoreApiControllerFactory
        CoreApiInstance.AggiustaUrl(url)

        If GDPR_isAttivo() Then

            Dim v As Integer = GDPR_VerificaStatoRichiediAccettazione(objParametri_Server, objParametri_Utenti)
            If v <> -1 Then
                Session("GDPR_Conferma_redir") = url
                Response.Redirect("GDPR_Conferma.aspx?GDPRCod=" & v & "&sidebar=off")
            Else
                Response.Redirect(url)
            End If

        Else
            Response.Redirect(url)

        End If

    End Sub


    Private Sub Autentica(ByVal Username As String, ByVal Password As String, ByVal AccessoEffettuatoTramiteSPID As Boolean)

        Dim objAgronicaCore As New DataProvider

        Dim StringaConnessione_Server As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(AppSettings("StarGate_PathFileINI"),
                                                                                                       AppSettings("Connessione_ONLINE_Server"))

        Dim StringaConnessione_Utenti As String = objAgronicaCore.FindConnessione_Su_Ini_O_Superserver(AppSettings("StarGate_PathFileINI"),
                                                                                                       AppSettings("Connessione_ONLINE_Utenti"))

        HttpContext.Current.Session.Item("ASG_StringaConnessione_Server") = StringaConnessione_Server
        HttpContext.Current.Session.Item("ASG_StringaConnessione_Utenti") = StringaConnessione_Utenti

        Dim objParametriHLP As New AgronicaCoreParametri_Helper
        Dim objParametri_Server As AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000), enumCancellazioneLogica.CancellazioneFisica, enumVisibilita.Visibilita_Tutti, "", "", Username, "", Username, "", StringaConnessione_Server)
        Dim objParametri_Utenti As AgronicaCoreParametri = objParametriHLP.Crea_ObjParametri(New DateTime(&H851055320574000), New DateTime(&H9325D82E8380000), enumCancellazioneLogica.CancellazioneFisica, enumVisibilita.Visibilita_Tutti, "", "", Username, "", Username, "", StringaConnessione_Utenti)

        Dim risp As String = ""

        Try

            NumeroMassimoTentativiAccessoImposta()

            Dim handleConfigSiti As New Configurazione_Siti_R
            Dim dtConfigSiti As DataTable = handleConfigSiti.Leggi(0, "AbilitaHashPassword", "", "", objParametri_Server)
            Dim gestioneHashAbilitata As Boolean = False
            If dtConfigSiti.Rows.Count > 0 Then
                gestioneHashAbilitata = dtConfigSiti.Rows(0)("Valore")
            End If

            Dim au As New AutenticaUtente()
            au.ASG_Autenticazione_Utente_verificaPPT(Username, Password, gestioneHashAbilitata, objParametri_Server, objParametri_Utenti)

            Dim xFiltroVerificaLock As String = ""
            If const_MaxNumeroTentativiAccesso > 0 Then
                xFiltroVerificaLock = " Utenti.flag = 0 "
            End If

            risp = au.ASG_Autenticazione_Utente(Username, Password, 1, 0, False, gestioneHashAbilitata, (objParametri_Server), (objParametri_Utenti), xFiltroVerificaLock)

            'DRUDI 22-03-2022
            If risp = "" Then
                AutorizzaCoreAPI(Username, Password, objParametri_Server)
            End If
            'DRUDI 22-03-2022

        Catch ex As AgroEccezioni_LoginFallito_Exception

            Dim bShouldExit As Boolean = VerificaNumeroTentativiAccesso(Username, objParametri_Utenti, objParametri_Server)

            If bShouldExit Then
                Dim msg As String = AgronicaAgenda_2010.UtenteBloccatoReimpostareLaPassword
                Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf &
                            msg, Page, UpdatePanel:=updateScript)

                Exit Sub
            End If

        End Try

        Dim DevoFinalizzare As Boolean = False

        If (risp <> "") Then

            Me.lbl_Messaggio.Text = risp
            Dim scriptText As String = "$(document).ready(function () { "
            scriptText = scriptText & " deleteCookie('username');" &
                                    "  deleteCookie('password');" &
                                    "  deleteCookie('ddlsito');" &
                                    "});"

            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(),
                                       String.Format("jQuery_{0}", Page.ClientID), scriptText, True)
            lbl_Messaggio.Text = risp

        Else

            Dim Elenco_Icone_SpecieVegetali As String = ""
            AgronicaCoreUtility.Icone.Recupera_Elenco_Icone_SpecieVegetali(Me.Server, "AB_Immagini/IconeVegetali", Elenco_Icone_SpecieVegetali)
            HttpContext.Current.Session.Item("Elenco_Icone_SpecieVegetali") = Elenco_Icone_SpecieVegetali

            'USARE INIZIALIZATORE ANCHE SENZA SUPERSERVER
            'Me.Crea_ObjParametri_Server_Utenti((HttpContext.Current.Session), (objParametri_Server), (objParametri_Utenti))

            'COPIATA DAL DEFAULT ONLINE
            'NON dovrebbe occorrere niente perchè nel load è chiamata InizializzaSito_GiasOnLine come per la pagina default online

            'Dim scriptText As String = "$(document).ready(function () { "
            'scriptText = (scriptText & "    document.location='./Menu/Menu.Aspx';" & "});")

            ''USARE INIZIALIZATORE ANCHE SENZA SUPERSERVER
            '' Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)

            'ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), String.Format("jQuery_{0}", Page.ClientID), scriptText, True)

            DevoFinalizzare = True

        End If

        Dim AccessoSoloSPID As Boolean = False

        Try

            Dim leggiUtenti As New Utenti_Read
            Dim dtUtenti As DataTable = leggiUtenti.Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta, " Username = N'" & Username & "'", "", objParametri_Utenti)

            If Not AccessoEffettuatoTramiteSPID Then
                AccessoSoloSPID = ControllaSeAccessoSoloSPID(objParametri_Server, Password, dtUtenti)
            End If

            Dim leggiLingua As New Lingue_Read
            Dim lLingua_cod_Utente As Integer = dtUtenti.Rows(0)("Lingua_COD")
            Dim dtLingua As DataTable = leggiLingua.Leggi_datoCODGIAS(lLingua_cod_Utente, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Utenti)

            Dim linguaCodiceISO As String = dtLingua.Rows(0)("CodiceISO")
            ImpostaCultura(New Lingua With {.CodiceISO = linguaCodiceISO, .Lingua_cod = lLingua_cod_Utente})

            objParametri_Server.Lingua_Cod = dtUtenti.Rows(0)("Lingua_COD")

        Catch ex As Exception
            ImpostaCultura(New Lingua With {.CodiceISO = "it", .Lingua_cod = 1})
        End Try

        If AccessoSoloSPID Then
            Dim msg As String = "Accesso consentito solo tramite SPID/CIE"
            Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf & msg, Page, UpdatePanel:=updateScript)
            Exit Sub
        End If

        HttpContext.Current.Session("ASG_objParametri_Server") = objParametri_Server
        HttpContext.Current.Session("ASG_objParametri_Utenti") = objParametri_Utenti

        If DevoFinalizzare Then
            FinalizzaLogin_EseguiRedirect(objParametri_Server, objParametri_Utenti)
        End If

        'elementi che mancano ingestione richieste per agenda
        'Me.Collegamento_Dpi = HttpContext.Current.Session("Collegamento_DPI")
        'Me.Collegamento_Fito = HttpContext.Current.Session("Collegamento_Fito")

    End Sub

    Public Function AutorizzaCoreAPI(username As String, password As String, objParametri_Server As AgronicaCoreParametri)
        Dim strContent = getContent(username, password, objParametri_Server.PivaSuperUser, "http://localhost/AgronicaCoreWS")
        Dim linkLoginCoreAPI = "https://localhost/AgronicaCoreAPI/Login"

        Try

            Dim pSecur As SecurityProtocolType = ServicePointManager.SecurityProtocol
            ServicePointManager.SecurityProtocol = CType(3072, SecurityProtocolType)

            Dim hdr = New System.Net.WebHeaderCollection
            'hdr.Add("Access-Control-Allow-Origin", "https://localhost/AgronicaCoreAPI")
            hdr.Add("Access-Control-Allow-Origin", "*")
            hdr.Add("Access-Control-Allow-Headers", "X-Requested-With,content-type")
            hdr.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, PATCH, DELETE")
            hdr.Add("Access-Control-Allow-Credentials", "true")

            Dim hlpHttp As New AgronicaCoreUtility.Http
            Dim rval = hlpHttp.chiamaWS(strContent, Nothing, linkLoginCoreAPI, "application/json", "POST", "application/json", "", hdr, Integer.MaxValue)

            ServicePointManager.SecurityProtocol = pSecur

        Catch ex As Exception
            Dim i = 0
            i += 1
        End Try

    End Function

    Public Function getContent(username As String, password As String, pivaSuperUser As String, linkCoreWS As String) As String
        Dim objContent As New JObject

        objContent("username") = username
        objContent("password") = password
        objContent("pivaSuperUser") = pivaSuperUser
        objContent("versioneAPP") = "1"
        objContent("coreWSBaseURL") = linkCoreWS

        Return objContent.ToString
    End Function

    Private Function VerificaNumeroTentativiAccesso(username As String, objParametri_Utenti As AgronicaCoreParametri, ByVal objParametri_SuperServer As AgronicaCoreParametri) As Boolean


        '----- Verifico quanti tentativi sono stati fatti ... se sono troppi ==> fuori

        Dim NumeroTentativiAccesso As Integer = NumeroTentativiAccessoLeggi(username, objParametri_Utenti)

        If NumeroTentativiAccesso >= const_MaxNumeroTentativiAccesso Then

            Dim xLock As New AgronicaCoreUtentiDAL.AutenticaUtente_W
            xLock.ImpostaLockUtente(username, objParametri_Utenti)
            Return True
        Else
            NumeroTentativiAccesso += 1
            Dim rvalTentativiScrivi As Boolean =
                NumeroTentativiAccessoScrivi(username, NumeroTentativiAccesso, objParametri_Utenti)

            If Not rvalTentativiScrivi Then
                Throw New Exception("Errore in fase memorizzazione tentativi accesso")
            End If
        End If

        Return False
    End Function


    Private Function NumeroTentativiAccessoLeggi(utente As String, objParametri_Utenti As AgronicaCoreParametri) As Integer

        Dim NumeroTentativiAccesso As Integer = 0
        Dim xLeggiNumero As New AgronicaCoreUtentiBIZ.Utenti

        NumeroTentativiAccessoLeggi = xLeggiNumero.NumeroTentativiAccessoLeggi(utente, objParametri_Utenti)

        Return NumeroTentativiAccessoLeggi

    End Function

    Private Function NumeroTentativiAccessoScrivi(utente As String, NumeroTentativiAccesso As Integer, objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Dim xLeggiNumero As New AgronicaCoreUtentiBIZ.Utenti

        Dim rval As Boolean =
            xLeggiNumero.NumeroTentativiAccessoScrivi(utente, NumeroTentativiAccesso, objParametri_Utenti)

        Return rval

    End Function
    Friend Sub ImpostaCultura(ByRef lingua As Lingua)
        If lingua IsNot Nothing Then
            System.Threading.Thread.CurrentThread.CurrentUICulture = New System.Globalization.CultureInfo(lingua.CodiceISO)
            '' questa istruzione da errore
            'System.Threading.Thread.CurrentThread.CurrentCulture = New System.Globalization.CultureInfo("en")
            '_LinguaCorrente = lingua
            Session("LinguaCorrente") = lingua
        End If
    End Sub

    'Protected Sub DDL_Account_SelectedIndexChanged(sender As Object, e As EventArgs) Handles btn_accountSelezionato.Click
    '    SPIDPostAssertionAndRedirect()
    'End Sub

    Protected Sub Cmb_Server_SelectedIndexChanged(sender As Object, e As EventArgs) Handles Cmb_Server.SelectedIndexChanged
        cambiatoServerSelezionato()
    End Sub


    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles button1.Click

        If debugSPID Then

            ddl_account.Items.Clear()
            debugAccessoSPID()

        Else

            apriOnlineConDBSelezionato()

        End If

    End Sub

    Private Sub debugAccessoSPID()
        Dim cfgRiconoscimentoUtente As New SMLClaimsCfg

        Dim samlRiconoscimento As New SAMLClaimsRiconoscimento
        samlRiconoscimento.GiasKey = "CodiceFiscale"
        samlRiconoscimento.Valore = Txt_username.Text
        cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento = New List(Of SAMLClaimsRiconoscimento)
        cfgRiconoscimentoUtente.ListaClaimsPerRiconoscimento.Add(samlRiconoscimento)

        cfgRiconoscimentoUtente.TipoDiSAMLClaimPerRiconoscereUtente = enum_SAML_RiconoscimentoUtente.CodiceFiscale

        Try

            apriOnlineConDBSelezionatoSAML(cfgRiconoscimentoUtente)

        Catch ex As RecoverableSPIDMultipleAccountLoginException
            ddl_account.Items.Clear()
            For Each item In ex.Risposta.Accounts.Rows
                ddl_account.Items.Add(New ListItem(item.Item("UserName"), item.Item("UserName")))
            Next
            hdf_ShowModal.Value = ddl_account.Items.Count
            ddl_account.Visible = True
            div_ddl_account.Visible = True
            ClientScript.RegisterClientScriptBlock(Me.GetType(), "Popup", "$(document).ready(function() { $('#dialog_selezionaUtente').modal('show'); })", True)
        Catch ex As Exception
            Messaggi.AgroMsgBox(AgronicaAgenda_2010.ProblemaDuranteAutenticazione & " " & vbCrLf & vbCrLf & ex.Message, Page, UpdatePanel:=updateScript)
        End Try
    End Sub
    Private Function Aggiorna_PesiSA_Operazioni(ByRef objParametri_Server As AgronicaCoreParametri, ByRef EsisteChiave As Boolean) As Boolean
        Dim xVerifica As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim dtVerifica As DataTable

        dtVerifica = xVerifica.Leggi(0, "Aggiorna_PesiSA_Operazioni", "", "", objParametri_Server)

        Dim xRval As Boolean = True

        If dtVerifica.Rows.Count > 0 Then
            EsisteChiave = True
            Try
                xRval = CType(dtVerifica(0)("Valore"), Boolean)
            Catch ex As Exception
                xRval = True
            End Try
        Else
            xRval = True
        End If

        Return xRval
    End Function

    Private Sub btnSPID_Click(sender As Object, e As EventArgs) Handles btnSPID.Click

        SPIDPostAssertionAndRedirect()

    End Sub

    Private Sub SPIDPostAssertionAndRedirect()

        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim SPID_CERTIFICATE_NAME As String = ""
        Dim SPID_CERTIFICATE_NAME_CALLBACK As String = ""
        Dim SPID_DOMAIN_VALUE As String = ""
        Dim SPID_ENVIROMENT As String = ""
        Dim SPID_IDP As String = ""
        Dim SPID_CLAIMS_ARRAY As String = ""
        Dim SPID_AssertionConsumerServiceURL As String = ""
        Dim SPID_LoginURL As String = ""
        Dim SPID_WindowsFindBy As String = ""
        Dim SPID_WindowsFindBy_Callback As String = ""
        Dim SPID_AgroSamlConfig As String = ""

        'TODO: decidere sulla lettura da DB Server
        LeggiParametriSamlWebConfig(
            objParametri_Super_Server, Nothing,
            SPID_CERTIFICATE_NAME,
            SPID_CERTIFICATE_NAME_CALLBACK,
            SPID_DOMAIN_VALUE,
            SPID_ENVIROMENT,
            SPID_CLAIMS_ARRAY,
            SPID_AssertionConsumerServiceURL,
            SPID_LoginURL,
            SPID_IDP,
            SPID_WindowsFindBy,
            SPID_WindowsFindBy_Callback,
            SPID_AgroSamlConfig
        )

        Dim xFindByWin As X509FindType = X509FindType.FindBySubjectName

        If Not String.IsNullOrEmpty(SPID_WindowsFindBy) Then
            xFindByWin = SPID_WindowsFindBy
        End If

        Dim xSamlCfg As AgroSamlConfig
        If Not String.IsNullOrEmpty(SPID_AgroSamlConfig) Then
            xSamlCfg = JsonConvert.DeserializeObject(Of AgroSamlConfig)(SPID_AgroSamlConfig)
        Else
            xSamlCfg = SamlCfgDefault()
        End If

        Dim SControl As New SPID_Controller(
            SPID_CERTIFICATE_NAME,
            SPID_DOMAIN_VALUE,
            SPID_ENVIROMENT,
            xFindByWin
        )

        Dim Data = SControl.SpidRequest(SPID_IDP, SPID_AssertionConsumerServiceURL, xSamlCfg)


        Dim SPID_LOG_DIR As String = ""
        LeggiParametriSamlWebConfigServerSuperServer(objParametri_Super_Server, Nothing, "SPID_LOG_DIR", SPID_LOG_DIR)

        'loggo tutto il file xml
        If Not String.IsNullOrEmpty(SPID_LOG_DIR) Then
            My.Computer.FileSystem.WriteAllText(FileSystemHelper.AggiungiSlashSeNonEsiste(SPID_LOG_DIR) & FileSystemHelper.NomeFileUnivoco("xml"), Data, True)
        End If


        Dim response = HttpContext.Current.Response
        response.Clear()

        Dim sb = New System.Text.StringBuilder()
        sb.Append("<html>")
        sb.AppendFormat("<body onload='document.forms[0].submit()'>")
        sb.AppendFormat("<form action='{0}' method='post'>", SPID_LoginURL)
        sb.AppendFormat("<input type='hidden' name='SAMLRequest' value='{0}'>", Data)
        sb.AppendFormat("<input type='hidden' name='RelayState' value='token'>")
        sb.AppendFormat("<input type='hidden' name='language' value='it'>")
        sb.AppendFormat("<input type='hidden' name='target' value='http://localhost/AgronicaAgenda/index.aspx'>") 'sarà ignorato, vale quanto indicato nel XML SAML
        sb.Append("</form>")
        sb.Append("</body>")
        sb.Append("</html>")
        response.Write(sb.ToString())
        response.End()
    End Sub

    Private Shared Function SamlCfgDefault() As AgroSamlConfig
        Return New AgroSamlConfig With {
                            .ComparisonType = AuthnContextComparisonType.minimum,
                            .SignAssertion = True,
                            .VerifyResponse = True
                        }
    End Function

    Private Sub btnSPID_Logout_Click(sender As Object, e As EventArgs) Handles btnSPID_Logout.Click



        Dim confSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim SPID_CERTIFICATE_NAME As String = ""
        Dim SPID_CERTIFICATE_NAME_CALLBACK As String = ""
        Dim SPID_DOMAIN_VALUE As String = ""
        Dim SPID_ENVIROMENT As String = ""
        Dim SPID_IDP As String = ""
        Dim SPID_AssertionConsumerServiceURL As String = ""
        Dim SPID_LogoutURL As String = ""
        Dim SPID_WindowsFindBy As String = ""
        Dim SPID_WindowsFindBy_Callback As String = ""
        Dim SPID_CLAIMS_ARRAY As String = ""
        Dim SPID_LoginURL As String = ""
        Dim SPID_AgroSamlConfig As String = ""

        'TODO: decidere sulla lettura da DB Server
        LeggiParametriSamlWebConfig(
            objParametri_Super_Server, Nothing,
            SPID_CERTIFICATE_NAME,
            SPID_CERTIFICATE_NAME_CALLBACK,
            SPID_DOMAIN_VALUE,
            SPID_ENVIROMENT,
            SPID_CLAIMS_ARRAY,
            SPID_AssertionConsumerServiceURL,
            SPID_LoginURL,
            SPID_IDP,
            SPID_WindowsFindBy,
            SPID_WindowsFindBy_Callback,
            SPID_AgroSamlConfig
        )

        Dim AgroConfig As AgroSamlConfig
        If Not String.IsNullOrEmpty(SPID_AgroSamlConfig) Then
            AgroConfig = JsonConvert.DeserializeObject(Of AgroSamlConfig)(SPID_AgroSamlConfig)
        Else
            AgroConfig = SamlCfgDefault()
        End If

        Dim xFindByWin As X509FindType = X509FindType.FindBySubjectName

        If Not String.IsNullOrEmpty(SPID_WindowsFindBy) Then
            xFindByWin = SPID_WindowsFindBy
        End If

        Dim SControl As New SPID_Controller(
            SPID_CERTIFICATE_NAME,
            SPID_DOMAIN_VALUE,
            SPID_ENVIROMENT,
            xFindByWin
        )


        Dim Data = SControl.LogoutRequest(SPID_LogoutURL, AgroConfig)


        Dim response = HttpContext.Current.Response
        response.Clear()



        Dim sb = New System.Text.StringBuilder()
        sb.Append("<html>")
        sb.AppendFormat("<body onload='document.forms[0].submit()'>")
        sb.AppendFormat("<form action='{0}' method='post'>", SPID_LogoutURL)
        sb.AppendFormat("<input type='hidden' name='SAMLRequest' value='{0}'>", Data)
        sb.AppendFormat("<input type='hidden' name='RelayState' value='token'>")
        sb.AppendFormat("<input type='hidden' name='language' value='it'>")
        sb.Append("</form>")
        sb.Append("</body>")
        sb.Append("</html>")
        response.Write(sb.ToString())
        response.End()


    End Sub

    Private Sub btnTroubleLogin_Click(sender As Object, e As EventArgs) Handles btnTroubleLogin.Click
        apriPagRecuperaCredenziali()
    End Sub

    Private Sub apriPagRecuperaCredenziali()
        'La Cmb_Server anche se non viene renderizzata sul client (e ciò avviene nel caso ci sia solo un dominio disponibile),
        'è comunque valorizzata su vb
        Dim dbSelezionato As String = Cmb_Server.SelectedValue
        Dim dbDesc As String = Label1.Text
        Dim targetUrl As String =
            "RecuperaCredenziali/TroubleLogin.aspx?d_v=" &
            Stringa_Codifica(dbSelezionato, AgroKey_EncoderDecoder) &
            "&d_d=" &
            Stringa_Codifica(dbDesc, AgroKey_EncoderDecoder)

        Response.Redirect(targetUrl)
    End Sub

    Private Sub btnCambiaPassword_Click(sender As Object, e As EventArgs) Handles btnCambiaPassword.Click
        apriPagCambiaPassword()
    End Sub

    Private Sub apriPagCambiaPassword()
        Dim dbSelezionato As String = Cmb_Server.SelectedValue
        Dim dbDesc As String = Label1.Text
        Dim targetUrl As String =
            "RecuperaCredenziali/CambiaPassword.aspx?d_v=" &
            Stringa_Codifica(dbSelezionato, AgroKey_EncoderDecoder) &
            "&d_d=" &
            Stringa_Codifica(dbDesc, AgroKey_EncoderDecoder)

        Response.Redirect(targetUrl)
    End Sub

    Private Sub btnRichiestaIscrizione_Click(sender As Object, e As EventArgs) Handles btnRichiestaIscrizione.Click
        apriPagRichiestaIscrizione()
    End Sub

    Private Sub apriPagRichiestaIscrizione()
        Dim dbSelezionato As String = Cmb_Server.SelectedValue
        Dim dbDesc As String = Label1.Text
        Dim targetUrl As String =
            "RecuperaCredenziali/RichiesteIscrizioni.aspx?d_v=" &
            Stringa_Codifica(dbSelezionato, AgroKey_EncoderDecoder) &
            "&d_d=" &
            Stringa_Codifica(dbDesc, AgroKey_EncoderDecoder)

        Response.Redirect(targetUrl)
    End Sub

    'Private Sub btn_accountSelezionato_Click(sender As Object, e As EventArgs) Handles btn_accountSelezionato.Click
    '    If debugSPID Then
    '        debugAccessoSPID()
    '    End If
    'End Sub

End Class