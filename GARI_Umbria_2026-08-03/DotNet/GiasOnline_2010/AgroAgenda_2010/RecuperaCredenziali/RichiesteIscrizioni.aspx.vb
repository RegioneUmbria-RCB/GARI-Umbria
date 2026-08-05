Imports AgroAgenda_2010.Resources
Imports AgronicaControlli_2010
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreUtentiDAL
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports System.Configuration.ConfigurationManager
Imports System.Web.Services
Imports AgronicaCoreGestioneRichieste
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json

Public Class RichiesteIscrizioni
    Inherits System.Web.UI.Page

    Public debug_isattached As Boolean = False
    Private _loginVersione As String = VERSIONE_LOGIN_DEFAULT
    Private _loginVersioniDisponibili As String() = {VERSIONE_LOGIN_DEFAULT, "2022"}
    Public Property Login_Versione As String
        Get
            Return _loginVersione
        End Get
        Set(value As String)
            SetLoginVersione(value)
        End Set
    End Property

    Public ReadOnly Property PATH_GIASBASE As String
        Get
            Return AgroMasterPage.PATH_GIASBASE
        End Get
    End Property

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

    Protected Overrides Sub InitializeCulture()
        MyBase.InitializeCulture()
        RecuperaCredenziali.InizializzaCulturaPreLogin()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If IsNothing(HttpContext.Current.Session("ASG_objParametri_Super_Server")) Then
            RecuperaCredenziali.InizializzaSitoGiasOnline()
        End If

        Dim dominioDesc As String = Request.QueryString("d_d")
        hfDescSuperServer.Value = dominioDesc
        'descDominioScelto.InnerText = Stringa_Decodifica(dominioDesc, AgroKey_EncoderDecoder)

        Dim valSuperServer As String = Request.QueryString("d_v")
        hfValSuperServer.Value = valSuperServer

        Dim decoValSuperServer As String = Stringa_Decodifica(valSuperServer, AgroKey_EncoderDecoder)
        Dim idSuperServer = CInt(decoValSuperServer.Split("|")(0))
        Dim pivaSuperServer = decoValSuperServer.Split("|")(4)

        Dim objParametri_Server As New AgronicaCoreParametri
        Dim objParametri_Utenti As New AgronicaCoreParametri
        RecuperaCredenziali.ValorizzaObjParametri(idSuperServer, objParametri_Server, objParametri_Utenti)

        Dim objUtenti As New Utenti_Read
        Dim datiSuperUser = objUtenti.Leggi_SuperUser(pivaSuperServer, objParametri_Utenti.StringaConnessione)
        Dim superUser_UserName = datiSuperUser(0)("username")

        objParametri_Server.SuperUserUsername = superUser_UserName
        objParametri_Server.PivaSuperUser = pivaSuperServer
        objParametri_Server.UtenteUsername = superUser_UserName

        objParametri_Utenti.SuperUserUsername = superUser_UserName
        objParametri_Utenti.PivaSuperUser = pivaSuperServer
        objParametri_Utenti.UtenteUsername = superUser_UserName

        Session("ASG_objParametri_Server") = objParametri_Server
        Session("ASG_objParametri_Utenti") = objParametri_Utenti
        hfObjPServer.Value = Utility.convertOBJparametritoString(objParametri_Server)
        hfObjPUtenti.Value = Utility.convertOBJparametritoString(objParametri_Utenti)

        Dim objParametri_Super_Server As New AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Super_Server.SuperUserUsername = superUser_UserName
        objParametri_Super_Server.PivaSuperUser = pivaSuperServer

        Session("ASG_objParametri_Super_Server") = objParametri_Super_Server
        hfPivaSuperServer.Value = pivaSuperServer
        hfObjPSuperServer.Value = Utility.convertOBJparametritoString(objParametri_Super_Server)

        'Imposto la variabile con il path dei coreWS da usare in JS
        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        Dim DTConfigSiti As DataTable
        DTConfigSiti = objConfigSiti.Leggi(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objParametri_Server)
        If Not IsNothing(DTConfigSiti) AndAlso DTConfigSiti.Rows.Count > 0 Then
            hfPathCoreWS.Value = DTConfigSiti.Rows(0).Item("Valore")
        End If

        'Leggo il documento contenente l'allegato che l'utente deve scaricare e compilare
        Dim objElenco As New AgronicaCoreScadenziario.Alert_Elenco_R
        Dim dtTemplateIscr As DataTable = objElenco.Leggi_Elenco(
            0,
            enum_ID_Area_Alert.Richiesta_Iscrizione_GIAS,
            enum_ID_Area_Tipologia.Doc_Template_Richiesta_Iscrizione_GIAS,
            pivaSuperServer,
            True,
            False,
            1,
            "",
            "",
            objParametri_Server,
            objParametri_Utenti)

        hfTemplateIscr.Value = JsonConvert.SerializeObject(dtTemplateIscr)

        Dim handleTipologia As New AgronicaCoreScadenziario.Alert_Tipologia_R
        Dim dtTipologiaArea = handleTipologia.Leggi_AreaETipologia(
            pivaSuperServer, 
            enum_ID_Area_Alert.Richiesta_Iscrizione_GIAS, 
            enum_ID_Area_Tipologia.Richiesta_Iscrizione_GIAS, 
            objParametri_Server)
        hfTipologiaIscr.Value = JsonConvert.SerializeObject(dtTipologiaArea)

        AgroMasterPage.CssPlaceHolder = siteCssPlaceHolder
        AgroMasterPage.FooterPlaceHolder = footerPlaceHolder
        AgroMasterPage.HeaderPlaceHolder = headerPlaceHolder
        AgroMasterPage.PaginaOspite = 0
        bootstrap.Versione = "5.3.3"
        bootstrap.BootstrapPlaceHeader = bootstrapPlaceHeader

        Dim xAssistenza As New Configurazione_Siti_R
        AgroMasterPage.sAssistenzaInfo = xAssistenza.Assistenza(objParametri_Super_Server, Nothing)

        agroKendo.KendoPlaceHeader = kendoPlaceHeader
        agroKendo.SitoOspite = Enum_SiteRedirector.Sito_AgronicaAgenda_2010

    End Sub

    <WebMethod>
    Public Shared Function LinkHomepage(ByVal valSuperServer As String) As RispostaStandard
        Return RecuperaCredenziali.LinkPaginaLogin(valSuperServer)
    End Function
    
    <WebMethod>
    Public Shared Function VerificaFirmaDigitale(ByVal fileBase64 As String) As RispostaStandard
        Dim r As New RispostaStandard
        'r.RispostaOK = True

        Dim bytesFile As Byte() = Convert.FromBase64String(fileBase64)
        Dim handleSH As New Agronica.Helpers.CA.SignHelper(Nothing)
        r.RispostaOk = handleSH.VerificaFirmaValida(bytesFile)

        Return r
    End Function

End Class