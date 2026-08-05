Imports System.Net.Http
Imports AgronicaControlli_2010

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando l'applicazione viene avviata
        Dim GiasVersioneCorrente As String = ""
        Try
            GiasVersioneCorrente = "versione=" + My.Computer.FileSystem.ReadAllText(Server.MapPath(".") & "\GiasVersioneCorrente.txt").Replace(" ", "").Replace(",", "_").Replace(":", "_").Replace("""", "").Replace("\", "").Replace("-", "_")
            'CreateConfigDataListJsonFile()
            'InitializeIdentityProviderList().GetAwaiter().GetResult()
        Catch ex As Exception

        End Try
        Application("GiasVersioneCorrente") = GiasVersioneCorrente

        ' inizializza stringhe connessione in application
        GlobalAsax_Helper.Inizializza_Stringhe_Connessione()

    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando la sessione viene avviata
    End Sub

    Sub Application_PreRequestHandlerExecute(ByVal sender As Object, ByVal e As EventArgs)

        GlobalAsax_Helper.Manage_PreRequestHandlerExecute_TimeZone(sender, e, Me)

    End Sub

    Sub Application_EndRequest(ByVal sender As Object, ByVal e As EventArgs)

    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato all'inizio di ogni richiesta

        GlobalAsax_Helper.Manage_BeginRequest_Security(sender, e, Me)

    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al tentativo di autenticare l'utilizzo
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato quando si verifica un errore
        'lo faccio solo se non è scaduta la sessione, altrimenti non riuscirei a memorizzare lo stack
        If TypeOf Context.Handler Is IRequiresSessionState OrElse
            TypeOf Context.Handler Is IReadOnlySessionState Then
            Session("objTrace") = Server.GetLastError
        End If

    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine della sessione
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Generato al termine dell'applicazione
    End Sub

    Private Sub Global_asax_PostMapRequestHandler(sender As Object, e As EventArgs) Handles Me.PostMapRequestHandler

        GlobalAsax_Helper.Manage_PostMapRequestHandler_Security(sender, e)

    End Sub

    Private Shared _httpClient As HttpClient
    Private Shared ReadOnly _lock As Object = New Object()
    Public Shared ReadOnly Property HttpClient As HttpClient
        Get
            SyncLock _lock
                If (_httpClient Is Nothing) Then
                    _httpClient = New HttpClient()
                End If
            End SyncLock

            Return _httpClient
        End Get

    End Property


    'Private Sub Application_PreRequestHandlerExecute(sender As Object, e As EventArgs)
    '    Dim app As HttpApplication = TryCast(sender, HttpApplication)
    '    Dim acceptEncoding As String = app.Request.Headers("Accept-Encoding")
    '    Dim prevUncompressedStream As Stream = app.Response.Filter

    '    If Not (TypeOf app.Context.CurrentHandler Is Page OrElse app.Context.CurrentHandler.[GetType]().Name = "SyncSessionlessHandler") OrElse app.Request("HTTP_X_MICROSOFTAJAX") IsNot Nothing Then
    '        Return
    '    End If

    '    If acceptEncoding Is Nothing OrElse acceptEncoding.Length = 0 Then
    '        Return
    '    End If

    '    acceptEncoding = acceptEncoding.ToLower()

    '    If acceptEncoding.Contains("deflate") OrElse acceptEncoding = "*" Then
    '        ' defalte
    '        app.Response.Filter = New DeflateStream(prevUncompressedStream, CompressionMode.Compress)
    '        app.Response.AppendHeader("Content-Encoding", "deflate")
    '    ElseIf acceptEncoding.Contains("gzip") Then
    '        ' gzip
    '        app.Response.Filter = New GZipStream(prevUncompressedStream, CompressionMode.Compress)
    '        app.Response.AppendHeader("Content-Encoding", "gzip")
    '    End If
    'End Sub

    'Private Async Function InitializeIdentityProviderList() As Task
    '    Dim idpMetadataList As List(Of IdentityProviderMetaData) = Nothing
    '    Dim idpMetadataListUrl As String = ConfigurationManager.AppSettings("IDP_METADATA_LIST_URL")

    '    If Not String.IsNullOrWhiteSpace(idpMetadataListUrl) Then
    '        idpMetadataList = Await IdentityProvidersList.GetIdpMetaDataListAsync(idpMetadataListUrl)
    '    End If

    '    Dim idpConfigDataList As List(Of IdentityProviderConfigData) = Nothing

    '    Using sr As StreamReader = New StreamReader(Server.MapPath("~/idpConfigDataList.json"))
    '        idpConfigDataList = JsonConvert.DeserializeObject(Of List(Of IdentityProviderConfigData))(sr.ReadToEnd())
    '    End Using

    '    IdentityProvidersList.IdentityProvidersListFactory(idpMetadataList, idpConfigDataList)
    'End Function

    'Private Sub CreateConfigDataListJsonFile()
    '    Dim idpConfigDataList As List(Of IdentityProviderConfigData) = New List(Of IdentityProviderConfigData) From {
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "WSO2",
    '            .OrganizationName = "Local SPID IdP (WSO2 on Docker container for testing)",
    '            .OrganizationDisplayName = "Local SPID IdP Test service",
    '            .OrganizationUrl = "https://github.com/italia/spid-testenv-docker",
    '            .SingleSignOnServiceUrl = "https://localhost:9443/samlsso",
    '            .SingleLogoutServiceUrl = "https://localhost:9443/samlsso",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://spidposte.test.poste.it",
    '            .OrganizationName = "Poste Italiane SpA IDP DI TEST",
    '            .OrganizationDisplayName = "Poste Italiane SpA IDP DI TEST",
    '            .OrganizationUrl = "https://spidposte.test.poste.it",
    '            .SingleSignOnServiceUrl = "https://spidposte.test.poste.it/jod-fs/ssoservicepost",
    '            .SingleLogoutServiceUrl = "https://spidposte.test.poste.it/jod-fs/sloservicepost",
    '            .SubjectNameIdRemoveText = "SPID-",
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://loginspid.aruba.it",
    '            .OrganizationName = "ArubaPEC S.p.A.",
    '            .OrganizationDisplayName = "ArubaPEC S.p.A.",
    '            .OrganizationUrl = "https://www.pec.it/",
    '            .SingleSignOnServiceUrl = "https://loginspid.aruba.it/ServiceLoginWelcome",
    '            .SingleLogoutServiceUrl = "https://loginspid.aruba.it/ServiceLogoutRequest",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://spid.intesa.it",
    '            .OrganizationName = "IN.TE.S.A. S.p.A.",
    '            .OrganizationDisplayName = "Intesa S.p.A.",
    '            .OrganizationUrl = "https://www.intesa.it/",
    '            .SingleSignOnServiceUrl = "https://spid.intesa.it/Time4UserServices/services/idp/AuthnRequest/",
    '            .SingleLogoutServiceUrl = "https://spid.intesa.it/Time4UserServices/services/idp/SingleLogout",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://identity.infocert.it",
    '            .OrganizationName = "InfoCert S.p.A.",
    '            .OrganizationDisplayName = "InfoCert S.p.A.",
    '            .OrganizationUrl = "https://www.infocert.it",
    '            .SingleSignOnServiceUrl = "https://identity.infocert.it/spid/samlsso",
    '            .SingleLogoutServiceUrl = "https://identity.infocert.it/spid/samlslo",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://idp.namirialtsp.com/idp",
    '            .OrganizationName = "Namirial",
    '            .OrganizationDisplayName = "Namirial S.p.a. Trust Service Provider",
    '            .OrganizationUrl = "https://www.namirialtsp.com",
    '            .SingleSignOnServiceUrl = "https://idp.namirialtsp.com/idp/profile/SAML2/POST/SSO",
    '            .SingleLogoutServiceUrl = "https://idp.namirialtsp.com/idp/profile/SAML2/POST/SLO",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://posteid.poste.it",
    '            .OrganizationName = "Poste Italiane SpA",
    '            .OrganizationDisplayName = "Poste Italiane SpA",
    '            .OrganizationUrl = "https://www.poste.it",
    '            .SingleSignOnServiceUrl = "https://posteid.poste.it/jod-fs/ssoservicepost",
    '            .SingleLogoutServiceUrl = "https://posteid.poste.it/jod-fs/sloserviceresponsepost",
    '            .SubjectNameIdRemoveText = "SPID-",
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://spid.register.it",
    '            .OrganizationName = "Register.it S.p.A.",
    '            .OrganizationDisplayName = "Register.it S.p.A.",
    '            .OrganizationUrl = "https//www.register.it",
    '            .SingleSignOnServiceUrl = "https://spid.register.it/login/sso",
    '            .SingleLogoutServiceUrl = "https://spid.register.it/login/singleLogout",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://identity.sieltecloud.it",
    '            .OrganizationName = "Sielte S.p.A.",
    '            .OrganizationDisplayName = "Sielte S.p.A.",
    '            .OrganizationUrl = "http://www.sielte.it",
    '            .SingleSignOnServiceUrl = "https://identity.sieltecloud.it/simplesaml/saml2/idp/SSO.php",
    '            .SingleLogoutServiceUrl = "https://identity.sieltecloud.it/simplesaml/saml2/idp/SLS.php",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
    '            .NowDelta = -2
    '        },
    '        New IdentityProviderConfigData() With {
    '            .EntityId = "https://login.id.tim.it/affwebservices/public/saml2sso",
    '            .OrganizationName = "TI Trust Technologies srl",
    '            .OrganizationDisplayName = "Trust Technologies srl",
    '            .OrganizationUrl = "https://www.trusttechnologies.it",
    '            .SingleSignOnServiceUrl = "https://login.id.tim.it/affwebservices/public/saml2sso",
    '            .SingleLogoutServiceUrl = "https://login.id.tim.it/affwebservices/public/saml2slo",
    '            .SubjectNameIdRemoveText = String.Empty,
    '            .DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'",
    '            .NowDelta = 0
    '        }
    '    }

    '    Using sw As StreamWriter = New StreamWriter(Server.MapPath("~/idpConfigDataList.json"))
    '        sw.Write(JsonConvert.SerializeObject(idpConfigDataList))
    '        sw.Close()
    '    End Using
    'End Sub

End Class