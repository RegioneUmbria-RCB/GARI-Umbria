Imports System.Net
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports AgronicaCoreVarieDAL
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Data
Imports System.Configuration

Public Class CoreApiControllerFactory

    Private _linkCoreAPI As String = String.Empty
    Private _linkCoreWS As String = String.Empty
    Private _objParametri_Server As AgronicaCoreParametri = Nothing
    Private _objParametri_Super_Server As AgronicaCoreParametri = Nothing
    Private _hdr As WebHeaderCollection = Nothing
    Private _http_Request As AgronicaCoreUtility.Http = Nothing
    Private _jwt As String = String.Empty
    Private _versioneHeader As String = String.Empty
    Private _versioneMaster As String = String.Empty
    Private _useCoreAPI As Boolean = False
    Private _SitoOspite As TipiEnumerativi.Enum_SiteRedirector = TipiEnumerativi.Enum_SiteRedirector.GiasLan
    Private _menuGisNG As Boolean = False
    Private _passwordScaduta As Boolean = False


    Private Shared objSingleton As CoreApiControllerFactory
    Private Shared classLocker As New Object()

    Private Const AUTH_COOKIE_NAME As String = "auth_cookie"
    Private Const AUTH_COOKIE_NAME_1 As String = "auth_cookieC1"
    Private Const AUTH_COOKIE_NAME_2 As String = "auth_cookieC2"
    Private Const REFRESH_TOKEN_NAME As String = "refreshToken"

    Public ReadOnly Property Http_Request() As AgronicaCoreUtility.Http
        Get
            Return _http_Request
        End Get
    End Property

    Public ReadOnly Property Headers() As WebHeaderCollection
        Get
            Return _hdr
        End Get
    End Property

    Public ReadOnly Property LinkCoreApi() As String
        Get
            Return _linkCoreAPI
        End Get
    End Property

    Public ReadOnly Property LinkCoreWS() As String
        Get
            Return _linkCoreWS
        End Get
    End Property

    Public ReadOnly Property PasswordScaduta() As Boolean
        Get
            Return _passwordScaduta
        End Get
    End Property

    Public ReadOnly Property JWT() As String
        Get
            Return _jwt
        End Get
    End Property

    Public ReadOnly Property CanUseAPI() As Boolean
        Get
            Return _useCoreAPI
        End Get
    End Property

    Public ReadOnly Property VersioneHeader() As String
        Get
            Return _versioneHeader
        End Get
    End Property

    Public ReadOnly Property MenuGisNG() As Boolean
        Get
            Return _menuGisNG
        End Get
    End Property

    Public Property SitoOspite As TipiEnumerativi.Enum_SiteRedirector
        Get
            Return _SitoOspite
        End Get
        Set(value As TipiEnumerativi.Enum_SiteRedirector)
            _SitoOspite = value
        End Set
    End Property


    Public Sub Inizializza(ObjParametri_Super_Server As AgronicaCoreParametri, ObjParametri_Server As AgronicaCoreParametri)

        _objParametri_Server = ObjParametri_Server
        _objParametri_Super_Server = ObjParametri_Super_Server

        If Not String.IsNullOrEmpty(_linkCoreWS) AndAlso Not String.IsNullOrEmpty(_linkCoreAPI) Then
            Exit Sub
        End If

        Try

            Dim pbj_configurazione_siti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
            Dim linkCoreAPI = pbj_configurazione_siti.Leggi_Valore(0, "GiasOnline_Core_API", "", "", ObjParametri_Server)
            'linkCoreAPI = "http://localhost/AgronicaCoreAPI"

            Dim linkCoreWS = pbj_configurazione_siti.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", ObjParametri_Server)
            'linkCoreWS = "http://localhost:52548/AgronicaCoreWS/"

            If linkCoreAPI = "" Then
                linkCoreAPI = pbj_configurazione_siti.Leggi_Valore(0, "GiasOnline_Core_API", "", "", ObjParametri_Super_Server)
            End If
            AggiustaUrl(linkCoreAPI)

            If ConfigurationManager.AppSettings("CoreAPIOnLocalhost") IsNot Nothing AndAlso CStr(ConfigurationManager.AppSettings("CoreAPIOnLocalhost")) = "true" Then
                Dim coreAPIDns = "localhost"
                If ConfigurationManager.AppSettings("CoreAPIOnDns") IsNot Nothing Then
                    coreAPIDns = CStr(ConfigurationManager.AppSettings("CoreAPIOnDns"))
                End If

                Dim coreAPIPort = "80"
                If ConfigurationManager.AppSettings("CoreAPIOnPort") IsNot Nothing Then
                    coreAPIPort = CStr(ConfigurationManager.AppSettings("CoreAPIOnPort"))
                End If

                Dim coreAPIProtocol = "http://"
                If ConfigurationManager.AppSettings("CoreAPILocalhostProtocol") IsNot Nothing Then
                    coreAPIProtocol = CStr(ConfigurationManager.AppSettings("CoreAPILocalhostProtocol"))
                End If

                linkCoreAPI = AggiustaUrlLocalhost(linkCoreAPI, coreAPIDns, coreAPIPort, coreAPIProtocol)

            End If

            If linkCoreWS = "" Then
                linkCoreWS = pbj_configurazione_siti.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", ObjParametri_Super_Server)
            End If
            AggiustaUrl(linkCoreWS)

            _linkCoreAPI = linkCoreAPI
            _linkCoreWS = linkCoreWS

            _hdr = New System.Net.WebHeaderCollection
            _hdr.Add("Access-Control-Allow-Origin", linkCoreAPI)
            _hdr.Add("Access-Control-Allow-Headers", "X-Requested-With,content-type")
            _hdr.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS, PUT, PATCH, DELETE")
            _hdr.Add("Access-Control-Allow-Credentials", "true")

            _http_Request = New AgronicaCoreUtility.Http

            Set_CanUseCoreAPI()
            Set_MenuGisNG()
            Set_Header_Version(_SitoOspite)

        Catch ex As Exception

        End Try

    End Sub

    Public Sub Inizializza(ObjParametri_Super_Server As AgronicaCoreParametri,
                           ObjParametri_Server As AgronicaCoreParametri,
                           Optional SitoOspite As TipiEnumerativi.Enum_SiteRedirector = Enum_SiteRedirector.GiasLan)
        _SitoOspite = SitoOspite
        Me.Inizializza(ObjParametri_Super_Server, ObjParametri_Server)

    End Sub
    Public Sub AggiustaUrl(ByRef link)

        If link.StartsWith("/") Then
            link = HttpContext.Current.Request.Url.Scheme & "://" &
                             HttpContext.Current.Request.Url.Host & link
        End If

    End Sub

    Public Function AggiustaUrlLocalhost(ByVal baseAddr As String,
                           ByRef CoreAPIOnDns As String,
                           ByRef CoreAPIOnPort As String,
                           ByRef CoreAPILocalhostProtocol As String) As String
        Dim baseArray As String()
        Dim protocol = ""
        Dim baseString = ""
        Dim returnBaseAddr = ""
        If baseAddr.Contains("https") Then
            protocol = "https://"
            baseString = baseAddr.Replace(protocol, "")
            baseArray = baseString.Split("/")
        ElseIf baseAddr.Contains("http") Then
            protocol = "http://"
            baseString = baseAddr.Replace(protocol, "")
            baseArray = baseString.Split("/")
        Else
            Return baseAddr
        End If

        returnBaseAddr += CoreAPILocalhostProtocol + CoreAPIOnDns + ":" + CoreAPIOnPort

        For index As Integer = 1 To baseArray.Length - 1
            If Not String.IsNullOrEmpty(baseArray(index)) AndAlso index <> 0 Then
                returnBaseAddr &= "/" + baseArray(index)
            End If
        Next

        Return returnBaseAddr

    End Function

    Public Function Login(username As String, password As String, idDB As String) As List(Of HttpCookie)
        Dim objReq As New JObject()
        objReq("coreWSBaseURL") = LinkCoreWS
        objReq("password") = password
        objReq("pivaSuperUser") = _objParametri_Server.PivaSuperUser
        objReq("username") = username
        objReq("versioneAPP") = "1"
        objReq("idDB") = idDB



        Dim currentProtocol As SecurityProtocolType = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        System.Net.ServicePointManager.DefaultConnectionLimit = Integer.MaxValue
        'ByPass per lavorare in localhost ed evitare errori di certificato SSL self-signed non valido

#If DEBUG Then
        ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
#End If

        'Dim Development As Boolean = _linkCoreAPI.Contains("localhost")
        'ServicePointManager.ServerCertificateValidationCallback = Function(sender, certificate, chain, errors)
        '                                                              If Development Then Return True ' For development, trust all certificates
        '                                                              Return errors = SslPolicyErrors.None ' Compliant: trust only some certificates
        '                                                          End Function

        Dim cookies As New List(Of HttpCookie)

        Try

            'Delete_Auth_Cookie_Response()

            Dim resp = _http_Request.chiamaWS_RestShapr(objReq,
                                                        "",
                                                        _linkCoreAPI & "/Login",
                                                        "application/json",
                                                        RestSharp.Method.POST,
                                                        "application/json", "",
                                                        _hdr, , , True)

            If resp.IsSuccessful AndAlso resp.ResponseStatus = RestSharp.ResponseStatus.Completed AndAlso resp.StatusCode = HttpStatusCode.OK Then

                Dim rs As RispostaStandard = JsonConvert.DeserializeObject(Of RispostaStandard)(resp.Content)

                For Each cookie In resp.Cookies
                    Dim httpcookie = New HttpCookie(cookie.HttpCookie.Name, cookie.HttpCookie.Value)
                    'httpcookie.Domain = cookie.Domain
                    'httpcookie.Domain = HttpContext.Current.Request.Url.GetComponents(UriComponents.Host, UriFormat.UriEscaped)
                    httpcookie.Path = "/"
                    httpcookie.HttpOnly = cookie.HttpOnly
                    httpcookie.Secure = True
                    httpcookie.SameSite = SameSiteMode.None
                    httpcookie.Expires = cookie.Expires
                    cookies.Add(httpcookie)
                Next
                _jwt = rs.RispostaStringa

                'Dim authorizationCookie = New HttpCookie("Authorization", "Bearer " & _jwt)
                ''authorizationCookie.Domain = HttpContext.Current.Request.Url.GetComponents(UriComponents.Host, UriFormat.UriEscaped)
                'cookies.Add(authorizationCookie)
                '_hdr.Add("Authorization", "Bearer " & _jwt)
            ElseIf resp.StatusCode = HttpStatusCode.Forbidden Then
                Try
                    Dim body = JObject.Parse(resp.Content)
                    If body("reason")?.ToString() = "PASSWORD_EXPIRED" Then
                        _passwordScaduta = True
                    End If
                Catch
                End Try
                Delete_Auth_Cookie_Response()
            ElseIf Not resp.IsSuccessful Then
                Delete_Auth_Cookie_Response()
            End If
        Catch ex As Exception

        Finally
            ServicePointManager.SecurityProtocol = currentProtocol
        End Try

        Return cookies

    End Function

    Private Shared Function AcceptAllCertifications(ByVal sender As Object, ByVal certification As System.Security.Cryptography.X509Certificates.X509Certificate, ByVal chain As System.Security.Cryptography.X509Certificates.X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean

        Return True

    End Function

    Public Function Call_Endpoint(Of T, Y)(controllerWrapperType As Y,
                                           nomeMetodo As String,
                                           ParamArray parametri() As Object) As T

        Dim currentProtocol As SecurityProtocolType = ServicePointManager.SecurityProtocol
        Dim resp As Object = Nothing
        Try
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            'ByPass per lavorare in localhost ed evitare errori di certificato SSL self-signed non valido

#If DEBUG Then
            ServicePointManager.ServerCertificateValidationCallback = AddressOf AcceptAllCertifications
#End If

            'Dim Development As Boolean = _linkCoreAPI.Contains("localhost")
            'ServicePointManager.ServerCertificateValidationCallback = Function(sender, certificate, chain, errors)
            '                                                              If Development Then Return True ' For development, trust all certificates
            '                                                              Return errors = SslPolicyErrors.None ' Compliant: trust only some certificates
            '                                                          End Function


            Dim rs = CallByName(controllerWrapperType, nomeMetodo, CallType.Method, parametri)
            If Not IsNothing(rs) Then
                resp = DirectCast(rs, T)
            End If

        Catch ex As Exception
            '' Se ricevo un 401 Unhautorized va fatta chiamata alle api per refresh cookie
        Finally
            ServicePointManager.SecurityProtocol = currentProtocol
        End Try

        Return resp


    End Function

    Private Function LeggiDaSessioneOppureDaConfigSiti(ByVal chiave As String, ByVal valoreDefault As String, ByVal TipoDB As agronicacoreparametri_tipoDB, Optional ByVal MemorizzaInSessioneDopoLettura As Boolean = True, Optional ByVal paramSessioneObjParametriValue As String = "") As String

        Dim xSessioneObjParametri As String = ""
        Dim Prefisso As String = "ASG_"

        Select Case TipoDB
            Case agronicacoreparametri_tipoDB.SuperServer
                xSessioneObjParametri = "ASG_objParametri_Super_Server"
                Prefisso &= "SS_"

            Case agronicacoreparametri_tipoDB.Server
                xSessioneObjParametri = "ASG_objParametri_Server"
                Prefisso &= "M_"
        End Select

        If paramSessioneObjParametriValue <> "" Then
            xSessioneObjParametri = paramSessioneObjParametriValue
        End If

        If IsNothing(HttpContext.Current.Session(xSessioneObjParametri)) Then
            Return ""
        End If

        Dim objParametri = New AgronicaCoreDataProvider.AgronicaCoreParametri(HttpContext.Current.Session(xSessioneObjParametri))

        Dim rval As String = HttpContext.Current.Session(Prefisso & chiave)
        'Dim rval As String = ""

        If String.IsNullOrEmpty(rval) Then

            Dim xLeggiKendoCfg As New AgronicaCoreVarieDAL.Configurazione_Siti_R

            Dim xDtLeggi As DataTable = xLeggiKendoCfg.Leggi(0, chiave, "", "", objParametri)

            If xDtLeggi.Rows.Count > 0 Then
                rval = xDtLeggi.Rows(0)("Valore")
            Else
                rval = valoreDefault
            End If

            If MemorizzaInSessioneDopoLettura Then
                HttpContext.Current.Session(Prefisso & chiave) = rval
            End If

        End If

        Return rval

    End Function





    Private Sub Set_Header_Version(Optional SitoOspite As Integer = AgronicaCoreDataProvider.TipiEnumerativi.Enum_SiteRedirector.GiasLan)
        Dim MemorizzaInSessioneDopoLettura As Boolean = True

        If _useCoreAPI Then

            'cerco versione personalizzata su db Server
            _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader" & SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)

            'cerco versione non personalizzata su db Server
            If String.IsNullOrEmpty(_versioneHeader) Then
                _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader", "", TipiEnumerativi.agronicacoreparametri_tipoDB.Server, MemorizzaInSessioneDopoLettura)
            End If

            If String.IsNullOrEmpty(_versioneHeader) Then
                'cerco versione personalizzata su Super Server
                _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader" & SitoOspite, "", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)
            End If

            If String.IsNullOrEmpty(_versioneHeader) Then

                'cerco versione comune per tutti ed in deficit uso la versione di default
                _versioneHeader = LeggiDaSessioneOppureDaConfigSiti("VersioneHeader", "UltimaVersione", TipiEnumerativi.agronicacoreparametri_tipoDB.SuperServer, MemorizzaInSessioneDopoLettura)

                If _versioneHeader.ToLower = "ultimaversione" OrElse _versioneHeader = "" Then
                    _versioneHeader = VERSIONE_HEADER_DEFAULT
                End If

            End If

        Else
            _versioneHeader = VERSIONE_HEADER_DEFAULT
        End If

        If MemorizzaInSessioneDopoLettura Then
            HttpContext.Current.Session("ASG_M_VersioneHeader") = _versioneHeader
        End If

        'Dim objConfSiti As New Configurazione_Siti_R

        ''cerco versione personalizzata su db Server
        '_versioneHeader = objConfSiti.Leggi_Valore(0, "VersioneHeader", "", "", _objParametri_Server)

        'If String.IsNullOrEmpty(_versioneHeader) Then
        '    _versioneHeader = objConfSiti.Leggi_Valore(0, "VersioneHeader", "", "", _objParametri_Super_Server)
        'End If

        'If String.IsNullOrEmpty(_versioneHeader) OrElse _versioneHeader.ToLower = "ultimaversione" Then
        '    _versioneHeader = VERSIONE_HEADER_DEFAULT
        'End If

    End Sub

    Private Sub Set_MenuGisNG()

        Dim objConfSiti As New Configurazione_Siti_R

        'cerco versione personalizzata su db Server Server
        Dim dbServerusaMenuGisNG = objConfSiti.Leggi_Valore(0, "MenuGisNG", "", "", _objParametri_Server)
        Boolean.TryParse(dbServerusaMenuGisNG, _menuGisNG)

    End Sub

    Private Sub Set_CanUseCoreAPI()

        Dim objConfSiti As New Configurazione_Siti_R

        Dim boolSuperServerAPI As Boolean = False
        Dim boolServerAPI As Boolean = False

        'cerco versione personalizzata su db Server Server
        Dim dbSuperServerusaApi = objConfSiti.Leggi_Valore(0, "UsaCoreAPI", "", "", _objParametri_Super_Server)
        Boolean.TryParse(dbSuperServerusaApi, boolSuperServerAPI)

        _useCoreAPI = boolSuperServerAPI

    End Sub

    'Private Sub Delete_Auth_Cookie()

    '    Dim auth_coockieFind = True
    '    While auth_coockieFind
    '        Dim auth_coockie = HttpContext.Current.Request.Cookies.Get(AUTH_COOKIE_NAME)
    '        If Not IsNothing(auth_coockie) Then
    '            HttpContext.Current.Request.Cookies.Remove(AUTH_COOKIE_NAME)
    '        Else
    '            auth_coockieFind = False
    '        End If
    '    End While

    '    auth_coockieFind = True
    '    While auth_coockieFind
    '        Dim auth_coockie_1 = HttpContext.Current.Request.Cookies.Get(AUTH_COOKIE_NAME_1)
    '        If Not IsNothing(auth_coockie_1) Then
    '            HttpContext.Current.Request.Cookies.Remove(AUTH_COOKIE_NAME_1)
    '        Else
    '            auth_coockieFind = False
    '        End If
    '    End While

    '    auth_coockieFind = True
    '    While auth_coockieFind
    '        Dim auth_coockie_2 = HttpContext.Current.Request.Cookies.Get(AUTH_COOKIE_NAME_2)
    '        If Not IsNothing(auth_coockie_2) Then
    '            HttpContext.Current.Request.Cookies.Remove(AUTH_COOKIE_NAME_2)
    '        Else
    '            auth_coockieFind = False
    '        End If
    '    End While

    '    auth_coockieFind = True
    '    While auth_coockieFind
    '        Dim refresh_Token = HttpContext.Current.Request.Cookies.Get(REFRESH_TOKEN_NAME)
    '        If Not IsNothing(refresh_Token) Then
    '            HttpContext.Current.Request.Cookies.Remove(REFRESH_TOKEN_NAME)
    '        Else
    '            auth_coockieFind = False
    '        End If
    '    End While

    'End Sub

    Public Sub Delete_Auth_Cookie_Response()

        Dim i As Integer = 0
        For i = 0 To HttpContext.Current.Request.Cookies.AllKeys.Length - 1
            Dim cookobj_coockie = HttpContext.Current.Request.Cookies.Get(i)
            If Not IsNothing(cookobj_coockie) AndAlso {AUTH_COOKIE_NAME, AUTH_COOKIE_NAME_1, AUTH_COOKIE_NAME_2, REFRESH_TOKEN_NAME}.Contains(cookobj_coockie.Name) Then
                cookobj_coockie.Expires = DateTime.Now.AddDays(-1)
                HttpContext.Current.Response.Cookies.Add(cookobj_coockie)

            End If
        Next

    End Sub

    Private Sub Check_AUTH_Cookie_Is_Valid()

    End Sub

    Public Function DammiIdSezioneDaQueryString() As Integer

        Dim idSezione As Integer = 0
        Dim context As HttpContext = HttpContext.Current
        If IsNothing(context) OrElse IsNothing(context.Request) Then
            Return idSezione
        End If

        If Not IsNothing(context.Request.QueryString("IDSezione")) AndAlso IsNumeric(context.Request.QueryString("IDSezione")) Then
            idSezione = CInt(context.Request.QueryString("IDSezione"))
        Else
            If Not IsNothing(context.Request.QueryString("idBC")) Then
                Dim idSezioneDecoded = Stringa_Decodifica(context.Request.QueryString("idBC"), AgroKey_EncoderDecoder)
                If Not String.IsNullOrEmpty(idSezioneDecoded) AndAlso IsNumeric(idSezioneDecoded) Then
                    idSezione = CInt(idSezioneDecoded)
                End If
            End If
        End If

        If idSezione = -1 Then
            idSezione = 0
        End If

        Return idSezione

    End Function

End Class
