Imports System.IO
Imports System.Net
Imports System.Net.Http
Imports System.Security.Cryptography.X509Certificates
Imports System.Text
Imports System.Web
Imports Agronica.Helpers.GiasBase
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq
Imports RestSharp
Imports RestSharp.Serializers.NewtonsoftJson

Public Class Http
    Dim certificato As X509Certificate

    Public Shared Function UriUnescape(ByVal uriToUnescape As String) As String
        Return Uri.UnescapeDataString(uriToUnescape)
    End Function

    Public Shared Function CookieLeggi(ByVal NomeCookie As String) As String
        Dim cookieDaClientJS As Web.HttpCookie = HttpContext.Current.Request.Cookies(NomeCookie)

        If cookieDaClientJS Is Nothing Then
            Return ""
        Else
            Return cookieDaClientJS.Value
        End If
    End Function

    Public Shared Sub CookieImposta(ByVal NomeCookie As String, ByVal ValoreCookie As String, Optional ByVal CookiePath As String = "/")

        Dim myCookie As New Web.HttpCookie(NomeCookie)
        'Commentato per uniformità con la creazione precedente del cookie che era fatta sul javascript,
        'ed in questo modo permette di sovrascriverlo nel caso rimasto in memoria del client, evitando errori.
        'Questo in quanto il dominio risulta scritto in maniera identica,
        'mentre impostando la proprietà aggiunge un punto come prefisso per indicare che sono supportati i sub-domains.
        'La proprietà va utilizzata in caso diventi necessario utilizzare i cookie in sub-domains.
        'myCookie.Domain = HttpContext.Current.Request.Url.GetComponents(UriComponents.Host, UriFormat.UriEscaped)
        myCookie.Path = CookiePath
        myCookie.Expires = Now.AddDays(7)
        myCookie.Value = ValoreCookie
        myCookie.HttpOnly = True
        myCookie.Secure = True
        myCookie.SameSite = SameSiteMode.None
        HttpContext.Current.Response.Cookies.Add(myCookie)

    End Sub

    Public Function IndirizzoIpChiamante() As String

        Dim rval As String = ""
        Try

            Dim context As System.Web.HttpContext = System.Web.HttpContext.Current
            Dim sIPAddress As String = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(sIPAddress) Then
                rval = context.Request.ServerVariables("REMOTE_ADDR")
            Else
                Dim ipArray As String() = sIPAddress.Split(New [Char]() {","c})
                rval = ipArray(0)
            End If



        Catch ex As Exception
            rval = "Errore leggendo ip"
        End Try

        Return rval

    End Function

    Public Function RestPostBasicAuth(basePath As String, payload As String, user As String, password As String, nomeOggettoEmbed As String, Optional ByPassHttps As Boolean = False) As String

        If Not ByPassHttps AndAlso (Not (basePath.ToLower.Contains("https") OrElse basePath.Contains("http://localhost"))) Then
            Throw New Exception("Autorizzate chiamate solo in HTTPS")
        End If


        Dim customHeaders As New WebHeaderCollection
        Dim basicAut As String = "Authorization: Basic "
        Dim credentials As String = Convert.ToBase64String(Encoding.ASCII.GetBytes(user & ":" & password))

        customHeaders.Add(basicAut & credentials)
        Dim xRisp As String =
            chiamaWS(payload, Nothing, basePath, "application/json", "POST", "application/json", "", customHeaders)

        If xRisp.StartsWith("{""" & nomeOggettoEmbed & """:") Then
            xRisp = xRisp.Replace("{""" & nomeOggettoEmbed & """:", "")
            xRisp = xRisp.Remove(xRisp.Length - 1, 1)
        End If

        Return xRisp
    End Function

    Public Function ReadByteArrayFromUrlGet(url As String, Optional ByVal timeoutInSeconds As Integer = -1, Optional ByRef objParametri As AgronicaCoreParametri = Nothing) As Byte()

        Dim client As New HttpClient
        Dim response As New HttpResponseMessage
        Dim myByteArray As Byte() = Nothing
        If timeoutInSeconds > 0 Then
            client.Timeout = TimeSpan.FromSeconds(timeoutInSeconds)
        End If

        response = client.GetAsync(url).Result
        If (response.IsSuccessStatusCode) Then
            myByteArray = response.Content.ReadAsByteArrayAsync.Result
        Else
            If objParametri IsNot Nothing Then
                Dim responseBody As String
                Try
                    responseBody = response.Content.ReadAsStringAsync().Result
                Catch ex As Exception
                    responseBody = $"[Impossibile leggere il body: {ex.GetType().Name} - {ex.Message}]"
                End Try

                Dim objLog As New LogProvider
                objLog.Scrivi_LOG(objParametri, "AgronicaCoreUtility.Http.vb\ReadByteArrayFromUrlGet()", "Errore lettura bytes da array " & RedactApiKeyFromUrl(url) & " - StatusCode: " & response.StatusCode.ToString() & " - ReasonPhrase: " & response.ReasonPhrase & " - Body: " & responseBody, CustomLOGParams:=New CustomLOGParams() With {.LogDirectory = objParametri.LogDirectory, .LogDescrizioneUtente = objParametri.LogDescrizioneUtente, .LogFileName = $"ReadByteArrayFromUrlGet_{Guid.NewGuid()}.txt"})
            End If
        End If

        Return myByteArray

    End Function

    Public Shared Function RedactApiKeyFromUrl(ByVal url As String) As String
        Const keyParam As String = "key="
        Dim keyIndex As Integer = url.IndexOf(keyParam, StringComparison.OrdinalIgnoreCase)
        If keyIndex < 0 Then
            Return url
        End If
        Return url.Substring(0, keyIndex + keyParam.Length) & "[APIKEY]"
    End Function

    Public Function chiamaWS(ByVal request As String,
                              ByVal certificatoPath As String,
                              ByVal webService As String,
                              ByVal contentType As String,
                              ByVal method As String,
                              ByVal accept As String,
                              ByVal soapAction As String,
                              Optional ByVal CustomHeaders As WebHeaderCollection = Nothing,
                             Optional ByVal TimeOut As Integer = -1
                    ) As String

        'certificato = X509Certificate2.CreateFromCertFile("C:\cooperazione.sian.it.crt")



        Dim resp As String
        If String.IsNullOrEmpty(certificatoPath) Then
            resp = GetResponse(webService, request, soapAction, contentType, method, accept, False, Nothing, CustomHeaders, TimeOut)
        Else

            If certificatoPath.Contains("https://") Then
                certificato = GetCertFromUrl(certificatoPath)
            Else
                certificato = X509Certificate.CreateFromCertFile(certificatoPath)
            End If

            ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf customXertificateValidation)

            'Dim messaggio As String = xSoggSiRPV.sSoggSiRPV("", "")
            resp = GetResponse(webService, request, soapAction, contentType, method, accept, True, certificato, CustomHeaders, TimeOut)

        End If
        Return resp
    End Function

    Public Function GetCertFromUrl(ByVal certificatoPath As String) As X509Certificate

        'Do webrequest to get info on secure site
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(certificatoPath), HttpWebRequest)
        Try

            ServicePointManager.ServerCertificateValidationCallback = New Security.RemoteCertificateValidationCallback(AddressOf customXertificateValidation)
            Dim response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            response.Close()

            'retrieve the ssl cert and assign it to an X509Certificate object
            Return request.ServicePoint.Certificate
        Catch ex As Exception
            Return request.ServicePoint.Certificate
        End Try

    End Function

    Private Function customXertificateValidation(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPoicyErrors As Security.SslPolicyErrors) As Boolean
        Select Case sslPoicyErrors
            Case Security.SslPolicyErrors.RemoteCertificateChainErrors

            Case Security.SslPolicyErrors.RemoteCertificateNameMismatch

            Case Security.SslPolicyErrors.RemoteCertificateNotAvailable

        End Select
        Return True
    End Function


    Public Function GetAuthFromCurrentHeader(Optional ByVal hdr As WebHeaderCollection = Nothing) As String
        Return GetFromCurrentHeader("Authorization", hdr)
    End Function

    ''' <summary>
    ''' Ottiene dall'header corrente il valore per la chiave passata come parametro, ovvia all'errore di cast da chiamate da app xamarin
    ''' </summary>
    ''' <param name="searchKey"></param>
    ''' <returns></returns>
    Public Function GetFromCurrentHeader2(ByVal searchKey As String) As String

        Dim rval As String = ""
        Dim whCollection = Web.HttpContext.Current.Request.Headers
        For Each key In whCollection
            If key.ToLower = searchKey.ToLower Then
                rval = whCollection(key)
            End If


        Next

        Return rval
    End Function

    ''' <summary>
    ''' Ottiene dall'header corrente il valore per la chiave passata come parametro
    ''' </summary>
    ''' <param name="searchKey"></param>
    ''' <returns></returns>
    Public Function GetFromCurrentHeader(ByVal searchKey As String, Optional ByVal hdr As WebHeaderCollection = Nothing) As String


        Dim CustomHeaders As WebHeaderCollection
        If hdr IsNot Nothing Then
            CustomHeaders = hdr
        Else

            CustomHeaders = Web.HttpContext.Current.Request.Headers
        End If


        Dim rval As String = ""

        For Each key As String In CustomHeaders.AllKeys
            Dim value As String = CustomHeaders(key)
            If key.ToLower = searchKey.ToLower Then
                rval = value
            End If
        Next

        Return rval

    End Function
    Private Function GetResponse(ByVal sSoapUri As String,
                                 ByVal sSoapMessage As String,
                                 ByVal sSoapAction As String,
                                 ByVal contentType As String,
                                 ByVal method As String,
                                 ByVal accept As String,
                                 ByVal bAttachCert As Boolean,
                                 _cert As X509Certificate,
                                 ByVal CustomHeaders As WebHeaderCollection,
                                 Optional ByVal timeOut As Integer = -1) As String
        Try
            Dim oHttpReq As HttpWebRequest = DirectCast(WebRequest.CreateDefault(New Uri(sSoapUri)), HttpWebRequest)
            oHttpReq.ContentType = contentType
            oHttpReq.Method = method
            oHttpReq.Accept = accept

            If timeOut > 0 Then
                oHttpReq.Timeout = timeOut
            End If



            If CustomHeaders IsNot Nothing Then

                For Each key As String In CustomHeaders.AllKeys
                    Dim value As String = CustomHeaders(key)
                    Select Case key.ToLower
                        Case "accept"
                            oHttpReq.Accept = value
                        Case "content-type"
                            oHttpReq.ContentType = value
                        Case Else
                            oHttpReq.Headers.Add(key, value)
                    End Select

                Next

            End If


            If sSoapAction <> "" Then
                oHttpReq.Headers.Add("soapaction", sSoapAction)
            End If


            oHttpReq.ServicePoint.Expect100Continue = False  ' <-- I've tried this both on and off to no avail

            If bAttachCert Then
                oHttpReq.ClientCertificates.Add(_cert)
            End If

            If method <> "GET" Then

                Dim mencoding As New System.Text.UTF8Encoding()
                Dim bytes As Byte() = mencoding.GetBytes(sSoapMessage)

                oHttpReq.ContentLength = bytes.Length

                Dim oReqStream As Stream = oHttpReq.GetRequestStream()
                oReqStream.Write(bytes, 0, bytes.Length)  '<-- This string is in just over 4K in length
                oReqStream.Flush()
                oReqStream.Close()
            End If

            Dim oHttpResp As HttpWebResponse = TryCast(oHttpReq.GetResponse(), HttpWebResponse)
            Dim oRespStream As Stream = oHttpResp.GetResponseStream()
            oHttpReq = Nothing

            Dim responseString As String
            Using stream As Stream = oHttpResp.GetResponseStream()
                Dim reader As New StreamReader(stream, Encoding.UTF8)
                responseString = reader.ReadToEnd()
            End Using

            Return responseString

            'Dim oXmlResp As XDocument =
            'XDocument.Load(oRespStream)
            'oRespStream.Flush()
            'oRespStream.Close()
            'Return oXmlResp.ToString

        Catch ex As WebException


            'uso questa funzione per ottenere il Messaggio..:
            Dim MessaggioErrore As String
            MessaggioErrore = Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[AgronicaCoreUtility.http] : " & MessaggioErrore, ex)

        End Try
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="TimeOut">-1 = lascia invariato il timeout di default (100secondi), altrimenti indicare il timeout desiderato in millisecondi</param>
    ''' <returns></returns>
    Public Function chiamaWS_RestShapr(ByVal requestObject As Object,
                                       ByVal certificatoPath As String,
                                       ByVal webService As String,
                                       ByVal contentType As String,
                                       ByVal method As RestSharp.Method,
                                       ByVal accept As String,
                                       ByVal soapAction As String,
                                       Optional ByVal CustomHeaders As WebHeaderCollection = Nothing,
                                       Optional ByVal CustomParameters As Dictionary(Of String, String) = Nothing,
                                       Optional ByVal TimeOut As Integer = -1,
                                       Optional ByVal UseNewtonsoftJson As Boolean = False,
                                       Optional ByVal serializeSettings As Newtonsoft.Json.JsonSerializerSettings = Nothing
                                       ) As IRestResponse


        Dim request As New RestRequest(method)
        Dim client As New RestClient(webService)

        'If Not IsNothing(HttpContext.Current) Then
        '    Dim auth_coockie = HttpContext.Current.Request.Cookies.Get("Authorization")
        '    If Not IsNothing(auth_coockie) Then
        '        request.AddCookie(auth_coockie.Name, auth_coockie.Value)
        '    End If
        'End If

        If CustomHeaders IsNot Nothing Then
            For Each key In CustomHeaders.AllKeys
                request.AddHeader(key, CustomHeaders.Get(key))
            Next
        End If

        If contentType IsNot Nothing AndAlso contentType <> "" Then
            request.AddHeader("Content-Type", contentType)
        End If

        If CustomParameters IsNot Nothing Then
            For Each key In CustomParameters.Keys
                request.AddParameter(key, CustomParameters(key), ParameterType.RequestBody)
            Next
        End If

        'Dim obj = JObject.Parse(requestObject)

        'request.AddBody(requestObject)
        If Not {RestSharp.Method.GET}.Contains(method) Then

            'Forza l'uso di Newtonsoft per la serializzazione dell'oggetto
            If UseNewtonsoftJson Then

                If serializeSettings IsNot Nothing Then
                    client.UseNewtonsoftJson(serializeSettings)
                Else
                    'Se non specificato, usa i valori di settings di default
                    client.UseNewtonsoftJson()

                    'JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
                    '{
                    '    ContractResolver     = new CamelCasePropertyNamesContractResolver(),
                    '    DefaultValueHandling = DefaultValueHandling.Include,
                    '    TypeNameHandling     = TypeNameHandling.None,
                    '    NullValueHandling    = NullValueHandling.Ignore,
                    '    Formatting           = Formatting.None,
                    '    ConstructorHandling  = ConstructorHandling.AllowNonPublicDefaultConstructor
                    '}

                End If

            End If

            request.RequestFormat = DataFormat.Json
            request.AddJsonBody(requestObject)
        End If

        If TimeOut <> -1 Then
            'Timeout default = 100 secondi = 100.000 millisecondi
            '5 minuti = 300 secondi = 300.000 millisecondi
            request.Timeout = TimeOut
        End If

        Dim response = client.Execute(request)

        Return response

    End Function


    Public Function chiamaWS_RestShapr_XML(ByVal requestObject As Object,
                                           ByVal certificatoPath As String,
                                           ByVal webService As String,
                                           ByVal contentType As String,
                                           ByVal method As RestSharp.Method,
                                           ByVal accept As String,
                                           ByVal soapAction As String,
                                           Optional ByVal CustomHeaders As WebHeaderCollection = Nothing,
                                           Optional ByVal TimeOut As Integer = -1
                                           ) As IRestResponse


        Dim request As New RestRequest(method)
        Dim client As New RestClient(webService)
        If CustomHeaders IsNot Nothing Then
            For Each key In CustomHeaders.AllKeys
                request.AddHeader(key, CustomHeaders.Get(key))
            Next
        End If

        'Dim obj = JObject.Parse(requestObject)

        request.RequestFormat = DataFormat.Xml

        'request.AddBody(requestObject)
        request.AddParameter("application/xml", requestObject, ParameterType.RequestBody)

        Dim response = client.Execute(request)

        Return response

    End Function


    Public Class JsonRestResponse
        Public StatusCode As System.Net.HttpStatusCode
        Public Content As Object
        Public Header As Object
    End Class

    ''' <summary>
    ''' Effettua un post usando restharp
    ''' </summary>
    ''' <param name="url">es.: "https://gestionale.terretruria.it/api/jobs/controlla/"</param>
    ''' <param name="BearerToken">abc</param>
    ''' <param name="postData"></param>
    Public Function PostWS_RestSharp_JSON(url As String, BearerToken As String, postData As String) As JsonRestResponse

        Dim client = New RestClient(url)
        Dim request = New RestRequest(Method.POST)
        request.AddHeader("Content-Type", "application/json")
        request.AddHeader("Authorization", "Bearer " & BearerToken)
        request.AddParameter("application/json", postData, ParameterType.RequestBody)
        Dim response As IRestResponse = client.Execute(request)

        Dim r1 As New JsonRestResponse With {
            .StatusCode = response.StatusCode,
            .Content = response.Content
        }

        Return r1
    End Function

    Public Function CallWS_RestSharp_JSON(ByVal baseUrl As String, ByVal resource As String, ByVal method As String, ByVal body As String, Optional ByVal hdrs As WebHeaderCollection = Nothing, Optional ByVal Accept As String = "", Optional ByVal BodyReqFormat As String = "", Optional ByVal FilePayload As Byte() = Nothing) As JsonRestResponse

        Dim client As New RestClient(baseUrl)

        Dim RS_Method As RestSharp.Method = RestSharp.Method.GET

        Select Case method.ToUpper()
            Case "POST"
                RS_Method = RestSharp.Method.POST
            Case "PUT"
                RS_Method = RestSharp.Method.PUT
            Case "DELETE"
                RS_Method = RestSharp.Method.DELETE
        End Select

        Dim request As New RestRequest(resource, RS_Method)

        request.RequestFormat = RestSharp.DataFormat.Json

        If Accept <> "" Then
            request.AddHeader("Accept", Accept)
        Else
            request.AddHeader("Accept", "application/json")
        End If

        If hdrs IsNot Nothing Then
            For Each key In hdrs.AllKeys
                request.AddHeader(key, hdrs.Get(key))
            Next
        End If

        If BodyReqFormat <> "" And FilePayload IsNot Nothing Then
            request.AddParameter(BodyReqFormat, FilePayload, ParameterType.RequestBody)
        Else
            If BodyReqFormat <> "" Then
                request.AddParameter(BodyReqFormat, body, ParameterType.RequestBody)
            Else
                request.AddParameter("text/json", body, ParameterType.RequestBody)
            End If
        End If

        Dim resp As RestSharp.RestResponse = client.Execute(request)

        Dim resp_Content As Object = Nothing

        Try

            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer With {
                .MaxJsonLength = Integer.MaxValue
            }

            resp_Content = js.Deserialize(Of Object)(resp.Content)

        Catch ex As Exception
            Try
                resp_Content = resp.Content.ToString()
            Catch ex1 As Exception
                resp_Content = Nothing
            End Try
        End Try

        Return New JsonRestResponse With {
            .StatusCode = resp.StatusCode,
            .Content = resp_Content,
            .Header = resp.Headers
        }

    End Function

    Public Class NetCDFRestResponse
        Public StatusCode As HttpStatusCode
        Public Content As MemoryStream
        Public ErrorMessage As String
    End Class
    Public Function CallWS_RestSharp_NetCDF(ByVal baseUrl As String, Optional ByVal hdrs As WebHeaderCollection = Nothing, Optional ByVal Accept As String = "") As NetCDFRestResponse

        Dim myReq As HttpWebRequest
        Dim myRes As WebResponse = Nothing
        Dim mySourceStream As Stream = Nothing
        Dim myTempStream As MemoryStream = Nothing

        myReq = WebRequest.Create(baseUrl)
        myReq.AllowAutoRedirect = True
        myReq.KeepAlive = True
        myReq.CookieContainer = New CookieContainer()

        If hdrs IsNot Nothing Then
            For Each key In hdrs.AllKeys
                myReq.Headers.Add(key, hdrs.Get(key))
            Next
        End If

        myRes = myReq.GetResponse()

        Try
            If myRes IsNot Nothing AndAlso myRes.ContentType = "application/x-netcdf" Then

                'Source stream with requested document  
                mySourceStream = myRes.GetResponseStream()

                'SourceStream has no ReadAll, so we must read data block-by-block  
                'Temporary Buffer and block size  
                Dim buffer(4096) As Byte, blockSize As Integer
                myTempStream = New MemoryStream

                Do
                    blockSize = mySourceStream.Read(buffer, 0, 4096)
                    If blockSize > 0 Then
                        myTempStream.Write(buffer, 0, blockSize)
                    End If
                Loop While blockSize > 0

            End If

            Return New NetCDFRestResponse With {
                .StatusCode = HttpStatusCode.OK,
                .Content = myTempStream
            }
        Catch ex As Exception
            Return New NetCDFRestResponse With {
                .StatusCode = HttpStatusCode.OK,
                .ErrorMessage = ex.ToString()
            }
        End Try

    End Function


    Public Class XMLRestResponse
        Public StatusCode As System.Net.HttpStatusCode
        Public Content As XDocument
    End Class
    Public Function CallWS_RestSharp_XML(ByVal baseUrl As String, ByVal resource As String, ByVal method As String, ByVal body As String, Optional ByVal hdrs As WebHeaderCollection = Nothing) As XMLRestResponse

        Dim client As New RestClient(baseUrl)

        Dim RS_Method As RestSharp.Method = RestSharp.Method.GET

        Select Case method.ToUpper()
            Case "POST"
                RS_Method = RestSharp.Method.POST
            Case "PUT"
                RS_Method = RestSharp.Method.PUT
            Case "DELETE"
                RS_Method = RestSharp.Method.DELETE
        End Select

        Dim request As New RestRequest(resource, RS_Method)

        request.RequestFormat = DataFormat.Json

        request.AddHeader("Accept", "application/XML")
        If hdrs IsNot Nothing Then
            For Each key In hdrs.AllKeys
                request.AddHeader(key, hdrs.Get(key))
            Next
        End If

        request.AddParameter("text/xml", body, ParameterType.RequestBody)

        Dim resp As RestResponse = client.Execute(request)

        Dim resp_Content As XDocument = Nothing

        Try

            resp_Content = XDocument.Parse(resp.Content)

        Catch ex As Exception

            resp_Content = Nothing

        End Try

        Return New XMLRestResponse With {
            .StatusCode = resp.StatusCode,
            .Content = resp_Content
        }

    End Function

End Class



Public Class RestSharpHelper
    Public Class Response
        Public StatusCode As System.Net.HttpStatusCode
        Public Content As JObject
    End Class

    Private ReadOnly _client As RestClient
    Private ReadOnly _request As RestRequest

    Public Sub New(ByVal url As String, Optional ByVal method As String = "POST")

        _client = New RestClient(url)

        Dim rs_method As RestSharp.Method = RestSharp.Method.POST
        Select Case method.ToUpper()
            Case "GET"
                rs_method = RestSharp.Method.GET
            Case "PUT"
                rs_method = RestSharp.Method.PUT
            Case "DELETE"
                rs_method = RestSharp.Method.DELETE
        End Select

        _request = New RestRequest("", rs_method)
    End Sub
    Public Sub AddHttpBasicAuth(ByVal username As String, ByVal password As String)
        Dim hdrValue As String = "Basic " & Convert.ToBase64String(Encoding.ASCII.GetBytes(username & ":" & password))
        Dim hdrName As String = "Authorization"
        _request.AddHeader(hdrName, hdrValue)
    End Sub
    Public Sub AddHeader(ByVal name As String, ByVal value As String)
        _request.AddHeader(name, value)
    End Sub

    Public Sub AddBody(ByVal body As JObject)
        _request.AddHeader("Content-Type", "application/json")

        If body IsNot Nothing Then
            _request.AddParameter("application/json", body.ToString, ParameterType.RequestBody)
        End If
    End Sub

    Public Sub AddBody(ByVal body As String)
        _request.AddHeader("Content-Type", "application/json")

        If body IsNot Nothing Then
            _request.AddParameter("application/json", body, ParameterType.RequestBody)
        End If
    End Sub

    Public Sub AddOrReplaceBody(ByVal body As JObject)
        Dim removeMe As Parameter

        If _request.Parameters.Where(Function(p) (p.Type.Equals(ParameterType.HttpHeader) And p.Name.Equals("Content-Type"))).Count > 0 Then
            removeMe = _request.Parameters.Where(Function(p) (p.Type.Equals(ParameterType.HttpHeader) And p.Name.Equals("Content-Type"))).FirstOrDefault
            _request.Parameters.Remove(removeMe)
        End If

        _request.AddHeader("Content-Type", "application/json")

        If body IsNot Nothing Then
            _request.AddOrUpdateParameter("application/json", body.ToString, ParameterType.RequestBody)
        End If
    End Sub

    Public Sub AddOrReplaceBody(ByVal body As String)
        Dim removeMe As Parameter

        If _request.Parameters.Where(Function(p) (p.Type.Equals(ParameterType.HttpHeader) And p.Name.Equals("Content-Type"))).Count > 0 Then
            removeMe = _request.Parameters.Where(Function(p) (p.Type.Equals(ParameterType.HttpHeader) And p.Name.Equals("Content-Type"))).FirstOrDefault
            _request.Parameters.Remove(removeMe)
        End If

        _request.AddHeader("Content-Type", "application/json")

        If body IsNot Nothing Then
            _request.AddOrUpdateParameter("application/json", body, ParameterType.RequestBody)
        End If
    End Sub

    Public Function Execute() As Response

        Dim oSProt As SecurityProtocolType = ServicePointManager.SecurityProtocol
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

        Dim resp = _client.Execute(_request)

        Dim result As New Response With {
            .StatusCode = resp.StatusCode,
            .Content = New JObject(New JProperty("content", Nothing))
        }

        Try

            Dim jtok = JToken.Parse(resp.Content)

            If TypeOf jtok Is JObject Then

                result.Content("content") = JObject.FromObject(jtok)

            Else
                If TypeOf jtok Is JArray Then

                    result.Content("content") = JArray.FromObject(jtok)

                End If
            End If

        Catch ex As Exception

            result.Content("content") = Nothing

        End Try

        ServicePointManager.SecurityProtocol = oSProt

        Return result
    End Function
End Class
