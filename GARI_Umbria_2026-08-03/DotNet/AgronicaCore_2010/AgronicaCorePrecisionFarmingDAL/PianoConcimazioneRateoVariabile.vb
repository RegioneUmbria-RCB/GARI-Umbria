Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports Google.Apis.Auth.OAuth2
Imports Newtonsoft.Json.Linq

Public Class PianoConcimazioneRateoVariabile_R

End Class
Public Class PianoConcimazioneRateoVariabile_W
    Public Function ElaboraRateoConGEE(ByVal GEEInput As String,
                                       ByRef objParametri_Server As AgronicaCoreParametri) As Boolean

        Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DT = cfgRead.Leggi(0, CostantiPersonalizzate.Google_Earth_Engine_ConfKey, "", "", objParametri_Server)

        If DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nel recupero dell'endpoint Google Earth Engine.")
        End If

        Dim valoreConfigurazione = JObject.Parse(DT.Rows(0)("Valore").ToString)

        Dim credential As GoogleCredential = GoogleCredential.FromJson(valoreConfigurazione("JSONAuth").ToString)

        Dim baseUrl = valoreConfigurazione("BaseUrl_GEE").ToString 'Non deve finire con la barra
        Dim audience = String.Format("{0}/{1}", baseUrl, valoreConfigurazione("PrescriptionsMapsOnVegIndexes").ToString) 'Non iniziare con la barra

        Dim token = credential.GetOidcTokenAsync(OidcTokenOptions.FromTargetAudience(audience), CancellationToken.None).Result
        Dim bearer As String = token.GetAccessTokenAsync(CancellationToken.None).Result

        Dim cf_client As New HttpClient()

        Dim content As New StringContent(GEEInput, Text.Encoding.UTF8, "application/json")

        cf_client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", bearer)

        Dim hr As HttpResponseMessage = cf_client.PostAsync(audience, content).Result

        If Not hr.IsSuccessStatusCode Then
            cf_client.Dispose()
            Throw New Exception("Richiesta esecuzione su piattaforma Google Earth Engine fallita.")
        End If

        'Dim responseContent = hr.Content.ReadAsStringAsync().Result

        cf_client.Dispose()

        Return True
    End Function
End Class
