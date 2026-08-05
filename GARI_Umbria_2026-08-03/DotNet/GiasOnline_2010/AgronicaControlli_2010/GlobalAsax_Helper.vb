Imports System.Configuration
Imports System.Data
Imports System.IO
Imports System.Text
Imports System.Web.Services
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreUtility
Imports AgronicaCoreVarieBIZ
Imports AgronicaCoreVarieDAL
Imports Ganss.Xss
Imports Newtonsoft.Json


Public Class GlobalAsax_Helper

    Public Shared Sub Manage_PreRequestHandlerExecute_TimeZone(ByVal sender As Object, ByVal e As EventArgs, ByVal istanza As Object)

        Dim x = DirectCast(sender, HttpApplication)
        Dim context = x.Context

        If Not IsNothing(x.Context.CurrentHandler) Then
            If TypeOf (x.Context.CurrentHandler) Is Page Then
                Dim p As Page = DirectCast(context.CurrentHandler, Page)
                If Not IsNothing(p) Then
                    Dim theScript = String.Format("var SERVER_TIME_ZONE_ID = '{0}';", TimeZoneUtility.Get_TZ_Short_Id())
                    p.ClientScript.RegisterClientScriptBlock(istanza.GetType(), Guid.NewGuid.ToString, theScript, True)

                    'Dim sb As New StringBuilder
                    'sb.AppendLine("var  TIME_ZONE_CACHE = null;")
                    'sb.AppendLine("if (String(typeof(AGRO_JS_TIMEZONE)).toLowerCase() === 'object') {")
                    'sb.AppendLine("if (String(typeof (AGRO_JS_TIMEZONE.Cache)).toLowerCase() === 'function') {")
                    'sb.AppendLine("     if ( TIME_ZONE_CACHE === null || TIME_ZONE_CACHE === undefined) { ")
                    'sb.AppendLine("         TIME_ZONE_CACHE = new  AGRO_JS_TIMEZONE.Cache(); }}}")
                    'p.ClientScript.RegisterClientScriptBlock(istanza.GetType(), Guid.NewGuid.ToString, sb.ToString, True)

                End If
            End If
        End If

    End Sub


    Public Shared Sub Manage_PostMapRequestHandler_Security(ByVal sender As Object, ByVal e As EventArgs)

        Dim application As HttpApplication = DirectCast(sender, HttpApplication)
        Dim req As System.Web.HttpRequest = application.Request

#If DEBUG Then
        Dim origin As String = req.Headers("Origin")

        If (Not IsNothing(origin)) Then 'puo essere nothing per esempio quando la richiesta proviene dalle core api
            Dim baseUrl As String = req.Url.GetLeftPart(UriPartial.Authority)
            If (baseUrl <> origin) Then 'cross origin request, può accadere in condizioni di debug con i sorgenti avviati
                Exit Sub
            End If
        End If
#End If

        Dim controlloConAuthCookie As Boolean = ControlloAutenticazioneConAuthCookie()
        If Not controlloConAuthCookie Then
            Exit Sub
        End If

        Dim pathsWhereCheckMustNotBePerformed As String() =
            {
                "Metaschema/ImpostazioniUtente.asmx/LeggiStampePreferite",
                "IsAlive/IsAlive.asmx/ReachableDB",
                "IsAlive/IsAlive.asmx/ReachableSites",
                "Provisioning/Provisioning.asmx/RefreshTokenJWT",
                "AgronicaCoreUtentiBIZ/Utenti_R.asmx/getMinutiValiditaLoginMemorizzato",
                "AgronicaCoreUtentiBIZ/Utenti_R.asmx/LeggiParametriConnessioneDaAccessToken",
                "Metaschema/ImpostazioniUtente.asmx/CaricaComboStampePreferite",
                "Provisioning/Provisioning.asmx/NuovoUtenteRetail",
                "Provisioning/Provisioning.asmx/RiportaNuovoUtente",
                "Provisioning/Provisioning.asmx/RinnovaUtente",
                "Provisioning/Provisioning.asmx/CreaTokenJWT",
                "AgronicaCoreUtentiBIZ/Utenti_R.asmx/ListaConnessioni",
                "WS_Autenticazione.asmx/InizializzazioneProfilazione",
                "AgronicaCoreUtentiBIZ/Utenti_R.asmx/Autenticazione",
                "RecuperaCredenziali/TroubleLogin.aspx/wsReimpostaPasswordDaUtente",
                "RecuperaCredenziali/TroubleLogin.aspx/wsReimpostaPasswordDaEmail",
                "RecuperaCredenziali/TroubleLogin.aspx/wsRecuperaUsername",
                "RecuperaCredenziali/ReimpostaPassword.aspx/wsReimpostaPassword",
                "RecuperaCredenziali/CambiaPassword.aspx/wsCambiaPassword",
                "Cache/CacheManager.asmx/PulisciCache"
            }

        Dim path As String = req.Path
        Dim contentType As String = req.ContentType
        Dim pathParts As String() = path.Split("/"c)
        Dim skipCheck = contentType = "application/x-www-form-urlencoded" OrElse path.ToLower().EndsWith(".aspx") OrElse path.ToLower().EndsWith(".axd")

        If skipCheck Then
            Exit Sub
        End If

        For Each pathWhereCheckMustNotBePerformed As String In pathsWhereCheckMustNotBePerformed
            If path.Contains(pathWhereCheckMustNotBePerformed) Then
                Exit Sub
            End If
        Next

        Dim className As String = pathParts(pathParts.Length - 2)
        Dim classType As Type
        Dim isWebMethod As Boolean = False

        If (className.ToLower().EndsWith(".asmx")) Then
            isWebMethod = True
        Else
            Dim handler = HttpContext.Current.Handler
            If IsNothing(handler) Then
                Exit Sub
            End If
            classType = handler.GetType().BaseType
            Dim methodName As String = pathParts.Last()

            If Not IsNothing(classType) Then

                Dim methodInfos = classType.GetMethods().Where(Function(t) t.Name = methodName).ToList()

                If methodInfos IsNot Nothing AndAlso methodInfos.Any() Then
                    isWebMethod = methodInfos.Any(Function(mi) mi.GetCustomAttributes(GetType(WebMethodAttribute), False).Length > 0)
                End If

            End If
        End If

        If Not isWebMethod Then
            Exit Sub
        End If

        Dim cookies As HttpCookieCollection = req.Cookies

        'controllo per la presenza di authcookie nei cookies (caso dell'agenda)
        Dim authCookieString As String = GetAuthCookieStringFromCookies(cookies)

        'se l'authcookie non si trova nei cookies (per esempio chiamate dalle core api alle core ws) provo nei headers della richiesta (le chiamate dalle core api a core ws inseriscono auth_cookie nei headers)
        If String.IsNullOrEmpty(authCookieString) Then

            Dim authCookieArray As String() = req.Headers.GetValues("auth_cookie")

            If Not IsNothing(authCookieArray) AndAlso authCookieArray.Count > 0 Then
                authCookieString = authCookieArray(0)
            End If

        End If

        Dim bearerToken As String = Nothing

        'se l'authcookie non si trova neanche nei headers provo a prendere l'authorization token (per l'app)
        If String.IsNullOrEmpty(authCookieString) Then

            Dim authorization As String() = req.Headers.GetValues("Authorization")

            If Not IsNothing(authorization) AndAlso authorization.Count > 0 Then
                bearerToken = authorization.Last().Substring("Bearer ".Length).Trim()
            End If

        End If

        If String.IsNullOrEmpty(authCookieString) AndAlso String.IsNullOrEmpty(bearerToken) Then
            ErrorResponse("Unauthorized", 401)
            Exit Sub
        End If

        Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
        Dim objParametriSuperServer As AgronicaCoreParametri = inizializza.InizializzaObjParametriSuperServer()

        Dim utentiTokenRead As New AgronicaCoreUtentiDAL.Utenti_TokenJWT_R
        Dim DT As DataTable

        If Not String.IsNullOrEmpty(authCookieString) Then
            DT = utentiTokenRead.LeggiConAuthCookie(authCookieString, objParametriSuperServer)
        Else 'se authcookiestring è null bearerToken non sarà null (altrimenti sarebbe uscito dalla sub nella if prima)
            DT = utentiTokenRead.LeggiConBearerToken(bearerToken, objParametriSuperServer)
        End If

        If DT IsNot Nothing AndAlso DT.Rows.Count = 1 Then
            Dim firstRow = DT.Rows(0)
            Dim dataScadenza = CDate(firstRow("Validita_Fine_AccessToken"))
            Dim checkDataScadenza = req.Headers("X-RabbitMQ") <> "true"
            If checkDataScadenza AndAlso dataScadenza <= Date.UtcNow Then
                ErrorResponse("Unauthorized", 401)
            End If
        Else
            ErrorResponse("Unauthorized", 401)
        End If

    End Sub

    Private Shared Function GetAuthCookieStringFromCookies(ByVal cookies As HttpCookieCollection) As String

        Dim authCookieString As String = String.Empty

        If cookies Is Nothing OrElse cookies.Count = 0 Then
            Return authCookieString
        End If

        Dim auth_cookie = cookies.Item("auth_cookie")
        If auth_cookie Is Nothing Then
            Return authCookieString
        End If

        If auth_cookie.Value.Contains("chunks") Then

            Dim chunksNr As Integer = CInt(auth_cookie.Value.Split("-")(1))

            If chunksNr > 10 Then 'limito il numero di chunk per evitare attacchi DoS
                Throw New Exception("SECURITY - Numero di chunk auth_cookie non valido.")
            End If

            For i = 1 To chunksNr 'concateno i vari chunks per ottenere il valore

                Dim auth_cookieChunk = cookies.Item($"auth_cookieC{i}")

                If auth_cookieChunk IsNot Nothing Then

                    authCookieString = String.Format("{0}{1}", authCookieString, auth_cookieChunk.Value)

                End If

            Next
        Else
            authCookieString = auth_cookie.Value
        End If

        Return authCookieString

    End Function


    Private Shared Function ControlloAutenticazioneConAuthCookie() As Boolean

        Dim controlloConAuthCookie As Boolean = False
        If Not IsNothing(ConfigurationManager.AppSettings("ControlloAutenticazioneConAuthCookie")) Then
            controlloConAuthCookie = CBool(ConfigurationManager.AppSettings("ControlloAutenticazioneConAuthCookie"))
        End If

        Return controlloConAuthCookie
    End Function

    Public Shared Sub Manage_BeginRequest_Security(ByVal sender As Object, ByVal e As EventArgs, ByVal istanza As Object)

        Dim x As HttpApplication = DirectCast(sender, HttpApplication)
        Dim req As System.Web.HttpRequest = x.Request

        Dim contentType As String = (If(req.ContentType, "")).ToLower()
        Dim path As String = req.Path
        Dim httpMethod As String = (If(req.HttpMethod, "")).ToUpper()
        Dim contentLength As Integer = req.ContentLength
        Dim applicationPath As String = req.ApplicationPath
        Dim appRelativeCurrentExecutionFilePath As String = req.AppRelativeCurrentExecutionFilePath
        Dim rawUrl As String = req.RawUrl
        Dim isAuthenticated As Boolean = req.IsAuthenticated
        Dim isLocal As Boolean = req.IsLocal
        Dim bodyText As String = ""

        ' controllo per impedire l'apertura della pagina all'interno di un frame
        Manage_XFrameOptions(path, ConfigurationManager.AppSettings("XFRAME_Options"))

        ' se impostato attiva il log delle chiamate
        Dim scriviLog As Boolean = False
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DEBUG_LogBeginRequest")) Then
            scriviLog = CBool(ConfigurationManager.AppSettings("DEBUG_LogBeginRequest"))
        End If


        'Giulia 20/01/2025: il default è ora Logga e Blocca per tutti i siti
        Dim detectXSS As Integer = CInt(enum_XSS_Detection_Config.Blocca_Logga_Chiamate_Malevole)

        ' se impostato attiva controllo di sicurezza XSS
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("DetectScriptInjection")) Then
            detectXSS = CInt(ConfigurationManager.AppSettings("DetectScriptInjection"))
        End If

        If scriviLog OrElse detectXSS > 0 Then

            'Devo controllare solo le chiamate che hanno un payload
            '(Escludendo i post back della pagina aspx stessa?!?)
            If Not String.IsNullOrEmpty(contentType) AndAlso
                Not path.ToLower().Contains(".svc") AndAlso Not contentType.Contains("text/xml") AndAlso Not contentType.Contains("multipart/form-data") AndAlso
                Not (httpMethod = "POST" AndAlso contentType = "application/x-www-form-urlencoded" AndAlso path.ToLower().EndsWith(".aspx")) Then

                Dim referrer As Uri = req.UrlReferrer
                Dim refAbsolutePath As String = ""
                Dim refLocalPath As String = ""
                Dim refAbsoluteURI As String = ""
                Dim refOriginalString As String = ""

                If referrer IsNot Nothing Then
                    refAbsolutePath = referrer.AbsolutePath
                    refLocalPath = referrer.LocalPath
                    refAbsoluteURI = referrer.AbsoluteUri
                    refOriginalString = referrer.OriginalString
                End If

                Dim cookies As HttpCookieCollection = req.Cookies
                Dim numCookie As Integer = 0
                Dim cookSession As String = ""
                Dim cookAuthCookie As String = ""
                If cookies IsNot Nothing AndAlso cookies.Count > 0 Then
                    numCookie = cookies.Count
                    If cookies.Item("ASP.NET_SessionId") IsNot Nothing Then
                        cookSession = cookies.Item("ASP.NET_SessionId").Value
                    End If
                    If cookies.Item("auth_cookie") IsNot Nothing Then
                        cookAuthCookie = cookies.Item("auth_cookie").Value
                    End If
                End If

                Dim headers = req.Headers
                Dim headOrigin As String = ""
                Dim headReferer As String = ""
                Dim headAuthCookie As String = ""
                If headers IsNot Nothing AndAlso headers.Count > 0 Then
                    If headers.Item("Origin") IsNot Nothing Then
                        headOrigin = headers.Item("Origin")
                    End If
                    If headers.Item("Referer") IsNot Nothing Then
                        headReferer = headers.Item("Referer")
                    End If
                    If headers.Item("auth_cookie") IsNot Nothing Then
                        headAuthCookie = headers.Item("auth_cookie")
                    End If

                End If

                'Il body devo cercare di leggerlo solo se ho qualcosa
                If contentLength > 0 Then

                    Dim readModePre As ReadEntityBodyMode = req.ReadEntityBodyMode

                    'Necessario questo tipo di estrazione dello stream per non "bruciarselo" visto che è "read-once"
                    'e dopo questa estrazione deve cmq essere passato all'endpoint vero e proprio
                    Dim inputStream = req.GetBufferedInputStream()

                    Dim arrByteBody As Byte() = AgronicaCoreUtility.AgroZip.RetrieveBytesFromStream(inputStream, contentLength)
                    bodyText = Encoding.UTF8.GetString(arrByteBody)

                    Dim readModePost As ReadEntityBodyMode = req.ReadEntityBodyMode
                End If

                If scriviLog Then
                    LogBeginRequest(path, contentType, contentLength, httpMethod, bodyText,
                                applicationPath, appRelativeCurrentExecutionFilePath,
                                rawUrl, isAuthenticated, isLocal,
                                refAbsolutePath, refLocalPath, refAbsoluteURI, refOriginalString,
                                numCookie, cookSession, cookAuthCookie,
                                headOrigin, headReferer, headAuthCookie)
                End If

                ' controllo di sicurezza per evitare attacchi cross-site scripting (XSS)
                If detectXSS > 0 AndAlso Not String.IsNullOrEmpty(bodyText) AndAlso Not WhiteList(path) Then

                    Dim sanitizer = New HtmlSanitizer()
                    Dim logFileName As String = "ScriptInjectionLog"
                    Dim errorMessage As String = "Bad Request"
                    Dim errorCode As Integer = 400

                    AddHandler sanitizer.RemovingTag,
                    Sub(ByVal s As Object, ByVal ea As RemovingTagEventArgs)

                        'Dim scriptInjection As Boolean = ea.Tag.NodeName.Equals("SCRIPT", StringComparison.OrdinalIgnoreCase)
                        errorMessage &= " detected tag """ & ea.Tag.NodeName & """"

                        ' logga chiamata
                        If detectXSS = 1 OrElse detectXSS = 3 Then
                            LogBeginRequest(path, contentType, contentLength, httpMethod, bodyText,
                                applicationPath, appRelativeCurrentExecutionFilePath, rawUrl, isAuthenticated, isLocal,
                                refAbsolutePath, refLocalPath, refAbsoluteURI, refOriginalString,
                                numCookie, cookSession, cookAuthCookie, headOrigin, headReferer, headAuthCookie,
                                logFileName, errorCode & " " & errorMessage)
                        End If

                        ' restituisce errore
                        If detectXSS = CInt(enum_XSS_Detection_Config.Blocca_Chiamate_Malevole) OrElse
                           detectXSS = CInt(enum_XSS_Detection_Config.Blocca_Logga_Chiamate_Malevole) Then
                            ErrorResponse(errorMessage, errorCode)
                        End If

                    End Sub

                    AddHandler sanitizer.RemovingAttribute,
                    Sub(ByVal s As Object, ByVal ea As RemovingAttributeEventArgs)

                        'Dim scriptInjection As Boolean = ea.Attribute.Name.Equals("HREF", StringComparison.OrdinalIgnoreCase)
                        errorMessage &= " detected attribute """ & ea.Attribute.Name & """"

                        ' logga chiamata
                        If detectXSS = CInt(enum_XSS_Detection_Config.Logga_Chiamate_Malevole) OrElse
                           detectXSS = CInt(enum_XSS_Detection_Config.Blocca_Logga_Chiamate_Malevole) Then
                            LogBeginRequest(path, contentType, contentLength, httpMethod, bodyText,
                                applicationPath, appRelativeCurrentExecutionFilePath, rawUrl, isAuthenticated, isLocal,
                                refAbsolutePath, refLocalPath, refAbsoluteURI, refOriginalString,
                                numCookie, cookSession, cookAuthCookie, headOrigin, headReferer, headAuthCookie,
                                logFileName, errorCode & " " & errorMessage)
                        End If

                        ' restituisce errore
                        If detectXSS = CInt(enum_XSS_Detection_Config.Blocca_Chiamate_Malevole) OrElse
                           detectXSS = CInt(enum_XSS_Detection_Config.Blocca_Logga_Chiamate_Malevole) Then
                            ErrorResponse(errorMessage, errorCode)
                        End If

                    End Sub

                    ' se true i nodi figli degli elementi rimossi vengono mantenuti
                    sanitizer.KeepChildNodes = False

                    ' se true consente gli attributi HTML che iniziano con "data-"
                    sanitizer.AllowDataAttributes = False

                    ' consente l'uso di class negli attributi HTML
                    sanitizer.AllowedAttributes.Add("class")

                    Dim sanitized As String = sanitizer.Sanitize(bodyText)

                End If

            End If

        End If

    End Sub

    Private Shared Sub LogBeginRequest(ByVal path As String, ByVal contentType As String, ByVal contentLength As Integer,
                                       ByVal httpMethod As String, ByVal bodyText As String,
                                       ByVal applicationPath As String, ByVal appRelativeCurrentExecutionFilePath As String,
                                       ByVal rawUrl As String, ByVal isAuthenticated As Boolean, ByVal isLocal As Boolean,
                                       ByVal refAbsolutePath As String, ByVal refLocalPath As String,
                                       ByVal refAbsoluteURI As String, ByVal refOriginalString As String,
                                       ByVal numCookie As Integer, ByVal cookSession As String, ByVal cookAuthCookie As String,
                                       ByVal headOrigin As String, ByVal headReferer As String,
                                       ByVal headAuthCookie As String,
                                       Optional fileName As String = "BeginRequestLog",
                                       Optional errorMessage As String = "")

        Dim sb As New StringBuilder()
        sb.AppendLine()
        sb.AppendLine()
        sb.AppendLine("******************** Application_BeginRequest [" & applicationPath & "] *********************")
        sb.AppendLine("Time : " & DateTime.Now)
        sb.AppendLine("Path : " & path)
        sb.AppendLine("RawUrl : " & rawUrl)
        sb.AppendLine("ApplicationPath : " & applicationPath)
        sb.AppendLine("AppRelativeCurrentExecutionFilePath : " & appRelativeCurrentExecutionFilePath)
        sb.AppendLine("Http-Method : " & httpMethod)
        sb.AppendLine("Content-Type : " & contentType)
        sb.AppendLine("Content-Length : " & contentLength)

        If Not String.IsNullOrEmpty(errorMessage) Then
            sb.AppendLine("Error : " & errorMessage)
        End If

        sb.AppendLine("Body : " & bodyText & "")

        sb.AppendLine("IsAuthenticated : [" & isAuthenticated.ToString() & "]")
        sb.AppendLine("IsLocal : [" & isLocal.ToString() & "]")

        sb.AppendLine("UrlReferrer - AbsolutePath : " & refAbsolutePath)
        sb.AppendLine("UrlReferrer - LocalPath : " & refLocalPath)
        sb.AppendLine("UrlReferrer - AbsoluteURI : " & refAbsoluteURI)
        sb.AppendLine("UrlReferrer - OriginalString : " & refOriginalString)

        sb.AppendLine("Cookies - Num Cookie : " & CStr(numCookie))
        sb.AppendLine("Cookies - ASP.NET_SessionId : " & cookSession)
        sb.AppendLine("Cookies - auth_cookie : " & cookAuthCookie)

        sb.AppendLine("Headers - Origin : " & headOrigin)
        sb.AppendLine("Headers - Referer : " & headReferer)
        sb.AppendLine("Headers - auth_cookie : " & headAuthCookie)

        Dim filePath As String = HttpContext.Current.Server.MapPath("~/" & fileName & ".txt")


        Using fstr As IO.FileStream = IO.File.Open(filePath, IO.FileMode.Append, IO.FileAccess.Write, IO.FileShare.ReadWrite)
            Dim sw As New IO.StreamWriter(fstr)
            sw.Write(sb.ToString())
            sw.Flush()
            sw.Dispose()
        End Using

    End Sub

    Private Shared Sub ErrorResponse(ByVal message As String,
                                     Optional ByVal code As Integer = 400,
                                     Optional ByVal response As Boolean = True,
                                     Optional ByVal json As Boolean = True)
        If response Then
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.StatusCode = code
            HttpContext.Current.Response.StatusDescription = message
            If json Then
                Dim r As New RispostaStandard
                r.RispostaOK = False
                r.Errore = message
                r.RispostaStringa = "{ }"
                Dim stringJSON As String = "{ ""d"":" & JsonConvert.SerializeObject(r) & " }"
                HttpContext.Current.Response.ContentType = "application/json"
                HttpContext.Current.Response.AddHeader("content-length", stringJSON.Length.ToString())
                HttpContext.Current.Response.Write(stringJSON)
            End If
            HttpContext.Current.Response.Flush()
            HttpContext.Current.Response.End()
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Else
            Throw New HttpException(code, message)
            'Throw New Exception(message)
        End If
    End Sub

    ' aggiunge l'opzione nell'header per impedire di aprire la pagina aspx in un frame 
    Private Shared Sub Manage_XFrameOptions(ByVal path As String, ByVal options As String)
        If path.ToLower().EndsWith(".aspx") AndAlso Not String.IsNullOrEmpty(options) Then
            For Each o In options.Split("|")
                Dim xFrameOptions = o.Split("=")
                If xFrameOptions.Length > 1 Then
                    For Each p In xFrameOptions(1).Split(",")
                        If path.EndsWith(p & ".aspx", StringComparison.OrdinalIgnoreCase) Then
                            HttpContext.Current.Response.AddHeader("x-frame-options", xFrameOptions(0))
                            Exit Sub
                        End If
                    Next
                Else
                    HttpContext.Current.Response.AddHeader("x-frame-options", xFrameOptions(0))
                    Exit Sub
                End If
            Next
        End If
    End Sub

    ' esclude chiamate a web method dal controllo di sicurezza XSS
    Private Shared Function WhiteList(ByVal path As String)

        ' se impostato esclude chiamate dal controllo di sicurezza XSS
        ' Esempio: "Filtrone_Nuovo.aspx/SalvaFiltri|..."
        Dim list As String = ""
        If Not String.IsNullOrEmpty(ConfigurationManager.AppSettings("WhiteListXSSDetection")) Then
            list = CStr(ConfigurationManager.AppSettings("WhiteListXSSDetection"))
        End If

        If Not String.IsNullOrEmpty(list) Then
            For Each x In list.Split("|")
                If path.EndsWith(x, StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If
            Next
        End If

        Return False

    End Function

    Public Shared Function SanitizeHTML(ByVal value As String)
        Dim sanitizer = New HtmlSanitizer()
        Return sanitizer.Sanitize(value)
    End Function

    Public Shared Function Inizializza_Stringhe_Connessione()

        Dim inizializza As New AgronicaCoreGestioneRichieste.Inizializzatore
        Dim objParametriSuperServer As AgronicaCoreParametri = inizializza.InizializzaObjParametriSuperServer()

        If Not IsNothing(objParametriSuperServer) AndAlso Utility_Sicurezza.IsEncryptedConnection(objParametriSuperServer) Then

            ' recupero le stringhe di connessione decriptate dal super server
            Dim StringheConnessione = Utility_Sicurezza.Leggi_Stringhe_Connessione(objParametriSuperServer)

            ' aggiungo la stringa di connessione al super server
            StringheConnessione.Add(Sicurezza.ID_DB_Super_Server, objParametriSuperServer.StringaConnessione)

            HttpContext.Current.Application("GiasStringheConnessione") = StringheConnessione

            Return True

        End If

        Return False

    End Function

End Class
