Imports System.Configuration
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.attivita
Imports AgronicaCoreModelsSTD.provisioning
Imports AgronicaCoreVarieDAL
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq


#If DEBUG Then
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
#End If

Public Class CallNetCore

    Public bearerToken As String = ""
    Public configurazioniSitiRead As New Configurazione_Siti_R

    Private Sub GetBackgroundAuth(accessToken As String, objPar As AgronicaCoreParametri)
        Try
            Dim authModel As BackgroundAuthenticationModel = New BackgroundAuthenticationModel()
            Dim client As New System.Net.Http.HttpClient()

            authModel.access_token = accessToken
            authModel.corewsbaseurl = configurazioniSitiRead.Leggi_Valore(0, "GiasOnline_WS_Core_AgroWS_Core", "", "", objPar)
            AggiustaUrl(authModel.corewsbaseurl, objPar)
            Dim correctKey = Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(authModel)))

            Dim key As KeyAuth = New KeyAuth()
            key.key = correctKey

            Dim content As New System.Net.Http.StringContent(JsonConvert.SerializeObject(key), Text.Encoding.UTF8, "application/json")
            Dim url As String = configurazioniSitiRead.Leggi_Valore(0, "GiasOnline_Core_API", "", "", objPar) & "/Login/BackgroundAuthentication"
            AggiustaUrl(url, objPar)

#If DEBUG Then
            ' ATTENZIONE RIMUOVERE SOLO TEMPORANEO PER TEST TROVARE SOLUZIONE OLE OLE OLE
            ServicePointManager.ServerCertificateValidationCallback =
    Function(sender, cert, chain, errors)
        Dim req = TryCast(sender, Net.HttpWebRequest)
        If req IsNot Nothing AndAlso
           req.RequestUri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) Then
            Return True
        End If
        Return errors = SslPolicyErrors.None
    End Function
#End If

            Dim hr As System.Net.Http.HttpResponseMessage = client.PostAsync(url, content).Result
            hr.EnsureSuccessStatusCode()
            Dim response = hr.Content.ReadAsStringAsync().Result

            Dim jObj As JObject = JsonConvert.DeserializeObject(Of JObject)(response)
            bearerToken = jObj("RispostaStringa").ToString()

        Catch ex As Exception
            Throw New Exception("Errore durante l'autenticazione di background: " & ex.Message.ToString())
        End Try
    End Sub

    Public Function CallNetCoreGet(urlComplete As String, accessToken As String, objParamServer As AgronicaCoreParametri) As Object
        If bearerToken = "" Then
            GetBackgroundAuth(accessToken, objParamServer)
            bearerToken = "Bearer " + bearerToken
        End If

        Dim client As New System.Net.Http.HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", bearerToken)

        Dim hr As System.Net.Http.HttpResponseMessage = client.GetAsync(urlComplete).Result
        hr.EnsureSuccessStatusCode()
        Dim response = hr.Content.ReadAsStringAsync().Result

        Return response
    End Function

    Public Function CallNetCorePost(urlComplete As String, accessToken As String, payLoad As String, objParamServer As AgronicaCoreParametri) As Object
        If bearerToken = "" Then
            GetBackgroundAuth(accessToken, objParamServer)
            bearerToken = "Bearer " + bearerToken
        End If

        Dim client As New System.Net.Http.HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", bearerToken)

        Dim content As New System.Net.Http.StringContent(payLoad, Text.Encoding.UTF8, "application/json")

        Dim hr As System.Net.Http.HttpResponseMessage = client.PostAsync(urlComplete, content).Result
        hr.EnsureSuccessStatusCode()
        Dim response = hr.Content.ReadAsStringAsync().Result

        Return response
    End Function

    Public Function CallNetCoreDelete(urlComplete As String, accessToken As String, objParamServer As AgronicaCoreParametri) As Object
        If bearerToken = "" Then
            GetBackgroundAuth(accessToken, objParamServer)
            bearerToken = "Bearer " + bearerToken
        End If

        Dim client As New System.Net.Http.HttpClient()
        client.DefaultRequestHeaders.Add("Authorization", bearerToken)

        Dim hr As System.Net.Http.HttpResponseMessage = client.DeleteAsync(urlComplete).Result
        hr.EnsureSuccessStatusCode()
        Dim response = hr.Content.ReadAsStringAsync().Result

        Return response
    End Function

    Public Sub AggiustaUrl(ByRef link, ByRef objPar)
        If link.StartsWith("/") Then
            Dim baseUrl = configurazioniSitiRead.Leggi_Valore(0, "LanToWebSiteBasePath", "", "", objPar)
            link = baseUrl + link
        End If
    End Sub

    Public Class KeyAuth
        Public key As String
    End Class

End Class
