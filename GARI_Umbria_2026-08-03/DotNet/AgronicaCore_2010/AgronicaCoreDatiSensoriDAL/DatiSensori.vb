Imports System.Globalization
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Threading
Imports AgronicaCoreDataProvider
Imports Google.Apis.Auth.OAuth2
Imports Newtonsoft.Json.Linq

Public Class DatiSensori_R

End Class
Public Class DatiSensori_W

    ' Shared HttpClient instance reused across requests to avoid socket exhaustion
    Private Shared ReadOnly _sharedHttpClient As HttpClient = New HttpClient()

    Public Function LetturaVegIndexes(ByVal GEEInput As String,
                                      ByRef objParametri_Server As AgronicaCoreParametri) As String

        Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        Dim DT = cfgRead.Leggi(0, CostantiPersonalizzate.Google_Earth_Engine_ConfKey, "", "", objParametri_Server)

        If DT.Rows.Count <> 1 Then
            Throw New Exception("Errore nel recupero dell'endpoint Google Earth Engine.")
        End If

        Dim valoreConfigurazione = JObject.Parse(DT.Rows(0)("Valore").ToString)

        Dim credential As GoogleCredential = GoogleCredential.FromJson(valoreConfigurazione("JSONAuth").ToString)

        Dim baseUrl = valoreConfigurazione("BaseUrl_GEE").ToString 'Non deve finire con la barra
        Dim audience = String.Format("{0}/{1}", baseUrl, valoreConfigurazione("QueriesGEEVegIndexesTimeSeries").ToString) 'Non iniziare con la barra

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

        Dim responseContent = hr.Content.ReadAsStringAsync().Result
        cf_client.Dispose()
        Return responseContent
    End Function

    ''' <summary>
    ''' Recupera dati elaborati (indici spettrali come NDVI, NDWI, NDMI) da SAT per un poligono.
    ''' Implementa retry con backoff e validazione della contiguità temporale.
    ''' Riferimento specifiche: RecuperoDatiElaboratiSAT - Regole di Business.
    ''' </summary>
    Public Function RecuperaDatiElaboratiSAT(ByVal poligonoId As String,
                                             ByVal tenantId As String,
                                             ByVal indiceRichiesto As String,
                                             ByVal dataInizio As String,
                                             ByVal dataFine As String,
                                             ByRef objParametri_Server As AgronicaCoreParametri,
                                             ByRef objParametri_SuperServer As AgronicaCoreParametri) As String

        Dim cfgRead As New AgronicaCoreVarieDAL.Configurazione_Siti_R

        ' Recupera configurazioni SAT da chiavi separate
        Dim satBaseUrl = cfgRead.Leggi_Valore_ServerESuperServer(0, CostantiPersonalizzate.SAT_BaseUrl_ConfKey, "", "", objParametri_Server, objParametri_SuperServer)
        Dim satApiKey = cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_ApiKey_ConfKey , "", "", objParametri_Server)
        Dim satAvailableIndexes = cfgRead.Leggi_Valore(0, CostantiPersonalizzate.SAT_Available_Indexes_ConfKey , "", "", objParametri_Server)
        
        Dim maxRetryAttempts = 3
        Dim retryBackoffSeconds = 30
        Dim timeoutSeconds = 60

        If String.IsNullOrWhiteSpace(satBaseUrl) Then
            Throw New Exception("Errore: urlEngine_SAT non configurato in Configurazione_Siti.")
        End If

        If String.IsNullOrWhiteSpace(satApiKey) Then
            Throw New Exception("Errore: apiKeyEngine_SAT non configurato in Configurazione_Siti.")
        End If

        ' Valida indici richiesti contro indici disponibili
        Dim availableIndexes = satAvailableIndexes.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries).Select(Function(x) x.Trim()).ToList()
        Dim validIndex = availableIndexes.FirstOrDefault(Function(idx) idx = indiceRichiesto)

        If validIndex Is Nothing Then
            Throw New Exception(String.Format("Errore: Nessun indice valido richiesto. Indici disponibili: {0}", satAvailableIndexes))
        End If

        Dim dataInizioFormatted = DateTime.Parse(dataInizio.Substring(0, 10)).ToString("yyyy-MM-dd")
        Dim dataFineFormatted = DateTime.Parse(dataFine.Substring(0, 10)).ToString("yyyy-MM-dd")

        ' Recupera statistiche per ogni indice richiesto
        Dim indexStats = FetchIndexStatsFromSAT(
            satBaseUrl,
            satApiKey,
            tenantId,
            poligonoId,
            validIndex,
            dataInizioFormatted,
            dataFineFormatted,
            timeoutSeconds,
            maxRetryAttempts,
            retryBackoffSeconds)
        Dim indexStatsList = TrasformaDatiSATaGIAS(indexStats)

        Return indexStatsList.ToString()
    End Function


    ''' <summary>
    ''' Recupera statistiche per un singolo indice da SAT con retry e backoff.
    ''' </summary>
    Private Function FetchIndexStatsFromSAT(ByVal satBaseUrl As String,
                                            ByVal satApiKey As String,
                                            ByVal tenantId As String,
                                            ByVal poligonoId As String,
                                            ByVal indexCode As String,
                                            ByVal dataInizio As String,
                                            ByVal dataFine As String,
                                            ByVal timeoutSeconds As Integer,
                                            ByVal maxRetryAttempts As Integer,
                                            ByVal retryBackoffSeconds As Integer) As JObject

        Dim parsedStartDate As DateTime = DateTime.ParseExact(dataInizio, "yyyy-MM-dd", CultureInfo.InvariantCulture)
        Dim utcStartDate As DateTime = DateTime.SpecifyKind(parsedStartDate, DateTimeKind.Utc)
        Dim startDate As String = utcStartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        
        Dim parsedEndDate As DateTime = DateTime.ParseExact(dataFine, "yyyy-MM-dd", CultureInfo.InvariantCulture)
        Dim utcEndDate As DateTime = DateTime.SpecifyKind(parsedEndDate, DateTimeKind.Utc)
        Dim endDate As String = utcEndDate.ToString("yyyy-MM-ddT23:59:59.999Z") ' Force time to include this date

        Dim url = String.Format("https://{0}/indexes/{1}/public/v1/polygons/{2}/indexes/{3}/stats?from={4}&to={5}",
                               satBaseUrl, tenantId, poligonoId.ToString(), indexCode, startDate, endDate)

        Dim lastException As Exception = Nothing

        For attempt = 1 To maxRetryAttempts
            Try
                Dim request As New HttpRequestMessage(HttpMethod.Get, url)
                request.Headers.Add("Authorization", String.Format("APIKEY {0}", satApiKey))
                request.Headers.Add("Accept", "application/json")

                ' Use shared HttpClient and cancellation token for timeout
                Dim cts = New CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds))
                Dim response = _sharedHttpClient.SendAsync(request, cts.Token).Result

                If response.StatusCode = Net.HttpStatusCode.Unauthorized OrElse 
                   response.StatusCode = Net.HttpStatusCode.Forbidden Then
                    Throw New Exception(String.Format("Errore autenticazione SAT: {0}", response.StatusCode.ToString()))
                End If

                If Not response.IsSuccessStatusCode Then
                    ' Errore transitorio 5xx: retry
                    If CInt(response.StatusCode) >= 500 Then
                        If attempt < maxRetryAttempts Then
                            Threading.Thread.Sleep(TimeSpan.FromSeconds(retryBackoffSeconds))
                            Continue For
                        End If
                    End If
                    Throw New Exception(String.Format("Errore SAT: {0}", response.StatusCode.ToString()))
                End If

                Dim responseContent = response.Content.ReadAsStringAsync().Result
                Dim statsResponse = JObject.Parse(responseContent)

                ' Ordina valori cronologicamente e valida contiguità temporale
                Dim values = statsResponse("values")
                If values IsNot Nothing Then
                    Dim sortedValues = values.OrderBy(Function(v) DateTime.Parse(v("point_in_time").ToString())).ToList()
                    statsResponse("values") = JArray.FromObject(sortedValues)
                End If

                Return statsResponse

            Catch ex As Exception
                lastException = ex
                If attempt < maxRetryAttempts Then
                    Threading.Thread.Sleep(TimeSpan.FromSeconds(retryBackoffSeconds))
                End If
            End Try
        Next

        ' Se siamo qui, tutti i tentativi sono falliti
        If lastException IsNot Nothing Then
            Throw lastException
        End If

        Throw New Exception(String.Format("Errore nel recupero dati SAT per indice {0} dopo {1} tentativi", indexCode, maxRetryAttempts))
    End Function

    ''' <summary>
    ''' Trasforma i dati elaborati da SAT nel formato legacy GIAS per compatibilità con FMIS e dashboard.
    ''' Riferimento specifiche: TrasformazioneDatiSATaGIAS - Scopo e Descrizione.
    ''' </summary>
    ''' <param name="satIndexStats">Oggetto JObject contenente le statistiche indice da SAT (polygon_id, index_code, values).</param>
    ''' <returns>Oggetto JObject trasformato nel formato GIAS legacy.</returns>
    Private Function TrasformaDatiSATaGIAS(ByVal satIndexStats As JObject) As JArray
        Dim giasResponse As New JArray()

        ' Trasformazione time_series
        Dim values = satIndexStats("values")
        If values IsNot Nothing Then
            ' Raggruppa per data (yyyy-MM-dd) e tieni solo l'entry con pixel_coverage maggiore
            Dim bestPerDay = values.Cast(Of JObject)() _
                .GroupBy(Function(v) DateTime.Parse(v("point_in_time").ToString()).ToString("yyyy-MM-dd")) _
                .Select(Function(g) g.OrderByDescending(Function(v)
                                                             Dim pc = v("pixel_coverage")
                                                             Return If(pc IsNot Nothing AndAlso pc.Type <> JTokenType.Null, pc.Value(Of Double)(), 0.0)
                                                         End Function).First())

            For Each value As JObject In bestPerDay
                Dim pointInTime = DateTime.Parse(value("point_in_time").ToString())
                Dim dataRiferimento = pointInTime.ToString("yyyy-MM-dd")
                Dim dataRiferimentoInizio = pointInTime.Date.ToString("O")
                Dim dataRiferimentoFine = pointInTime.Date.AddDays(1).AddTicks(-1).ToString("O")

                Dim tsItem As New JObject()
                tsItem.Add("DataRiferimento", dataRiferimento)
                tsItem.Add("DataRiferimentoInizio", dataRiferimentoInizio)
                tsItem.Add("DataRiferimentoFine", dataRiferimentoFine)
                tsItem.Add("ProductID", CostantiPersonalizzate.Dati_Sensori_Platform)
                tsItem.Add("Name", "")  ' Placeholder per asset identifier, da definire se necessario

                ' Mapping valori
                Dim avg = value("avg")
                Dim stddev = value("stddev")
                Dim min = value("min")
                Dim max = value("max")
                Dim pixelCount = value("pixel_count")

                tsItem.Add("Valore", avg)
                tsItem.Add("Valore_mean", avg)
                tsItem.Add("Valore_std", stddev)
                tsItem.Add("Valore_min", min)
                tsItem.Add("Valore_max", max)
                tsItem.Add("pixel_count", pixelCount)

                giasResponse.Add(tsItem)
            Next
        End If

        Return giasResponse
    End Function
End Class