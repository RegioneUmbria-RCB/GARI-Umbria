Imports System.Net
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports RestSharp

Public Class Ws_Auth_OAUTH2

    ''' <summary>
    ''' password, client_credentials
    ''' </summary>
    Public Property grant_type As String
    Public Property username As String
    Public Property password As String
    Public Property client_id As String
    Public Property client_secret As String
    Public Property endpointAuth As String
    Public Property resource As String
    Public Property scope As String

    Public Sub New()
        grant_type = ""
        username = ""
        password = ""
        client_id = ""
        client_secret = ""
        endpointAuth = ""
        resource = ""
        scope = ""
    End Sub
End Class

Public Class Ws_Auth_Basic

    Public Property username As String
    Public Property password As String

    Public Sub New()
        username = ""
        password = ""
    End Sub
End Class

Public Class APICalls

    ''' <summary>
    ''' Autenticazione tramite OAUTH2 (al momento gestite grant_type = password e client_credentials
    ''' </summary>
    ''' <returns>Token in Risposta Stringa se riesce ad ottenerlo</returns>
    Public Shared Function GetToken(ByVal objOAuth2 As Ws_Auth_OAUTH2) As RispostaStandard

        Dim r As New RispostaStandard

        Try

            If String.IsNullOrEmpty(objOAuth2.endpointAuth) Then
                Throw New ArgumentException("Endpoint Authorization URL not provided")
            End If

            Dim client As New RestClient(objOAuth2.endpointAuth)
            Dim req As New RestRequest(Method.POST)
            'Dim req As New RestRequest("token", Method.POST)
            'req.AddHeader("cache-control", "no-cache")

            If String.IsNullOrEmpty(objOAuth2.grant_type) Then
                Throw New ArgumentException("grant_type not provided")
            End If

            req.AddHeader("Content-Type", "application/x-www-form-urlencoded")
            
            req.AddParameter("grant_type", objOAuth2.grant_type) ' password, client_credentials

            If Not String.IsNullOrEmpty(objOAuth2.client_id) Then
                req.AddParameter("client_id", objOAuth2.client_id)
            End If

            If Not String.IsNullOrEmpty(objOAuth2.scope) Then
                req.AddParameter("scope", objOAuth2.scope)  ' https://esempio.cloudax.dynamics.com/.default openid email profile offline_access Group.ReadWrite.All User.ReadWrite.All Financials.ReadWrite.All
            End If
            
            If Not String.IsNullOrEmpty(objOAuth2.resource) Then
                req.AddParameter("resource", objOAuth2.resource)    ' https://esempio.cloudax.dynamics.com/
            End If

            If objOAuth2.grant_type = "password" Then

                If String.IsNullOrEmpty(objOAuth2.username)Then
                    Throw New ArgumentException("grant_type is password but username not provided")
                End If

                If String.IsNullOrEmpty(objOAuth2.password) Then
                    Throw New ArgumentException("grant_type is password but password not provided")
                End If

                req.AddParameter("username", objOAuth2.username)
                req.AddParameter("password", objOAuth2.password)

            ElseIf objOAuth2.grant_type = "client_credentials" Then

                If String.IsNullOrEmpty(objOAuth2.client_secret) Then
                    Throw New ArgumentException("grant_type is client_credentials but client_secret not provided")
                End If
                
                req.AddParameter("client_secret", objOAuth2.client_secret)

            Else
                
                If Not String.IsNullOrEmpty(objOAuth2.username) Then
                    req.AddParameter("username", objOAuth2.username)
                End If
                If Not String.IsNullOrEmpty(objOAuth2.password) Then
                    req.AddParameter("password", objOAuth2.password)
                End If
                If Not String.IsNullOrEmpty(objOAuth2.client_secret) Then
                    req.AddParameter("client_secret", objOAuth2.client_secret)
                End If

            End If
            
            Dim resp = client.Execute(req)
            Dim response As JObject = JsonConvert.DeserializeObject(resp.Content)

            If resp.StatusCode = HttpStatusCode.OK Then
                r.RispostaOK = True
                r.RispostaStringa = response("access_token")
            Else
                r.RispostaOK = False
                r.Errore = response("error_description")
            End If

        Catch ex As Exception
            r.RispostaOK = False
            r.Errore = ex.Message
        End Try

        Return r

    End Function

    Public Shared Function GetBasicAuth(ByVal objBasic As Ws_Auth_Basic) As String

        Try

            If String.IsNullOrEmpty(objBasic.username) Then
                Throw New ArgumentException("Username not provided for Basic Authentication")
            End If

            If String.IsNullOrEmpty(objBasic.password) Then
                Throw New ArgumentException("Password not provided for Basic Authentication")
            End If

            Dim credentials As String = Convert.ToBase64String(Text.Encoding.UTF8.GetBytes($"{objBasic.username}:{objBasic.password}"))
            Return credentials

        Catch ex As Exception
            Throw New Exception("Error creating Basic Authentication credentials: " & ex.Message)
        End Try

    End Function

End Class
