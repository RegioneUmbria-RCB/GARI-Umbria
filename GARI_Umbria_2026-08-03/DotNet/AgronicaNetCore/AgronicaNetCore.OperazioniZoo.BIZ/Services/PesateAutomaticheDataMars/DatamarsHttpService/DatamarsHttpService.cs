using AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.Exceptions;
using InData.Zoo.DataMars;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.DatamarsHttpService;

/// <summary>
/// Implementazione del client HTTP verso l'API Datamars con logica di retry esponenziale server-side.
/// <para>
/// La logica di retry è completamente interna: Datamars non viene informata del numero di tentativi.
/// Ogni tentativo fallito è seguito da un backoff crescente [0s, 5s, 15s, 45s] per max 3 retry.
/// </para>
/// <para>
/// Criteri di retry: timeout, HTTP 5xx, HTTP 429.<br/>
/// No-retry (fail fast): HTTP 4xx (tranne 429), HTTP 422, <see cref="JsonException"/>.
/// </para>
/// <para>Riferimento spec: DS10-BL GestioneRetryEsponentialeApiDatamars.</para>
/// </summary>
public sealed class DatamarsHttpService : IDatamarsHttpService
{
    private const int MaxRetries = 3;
    private const int TimeoutSecondi = 60;

    // Backoff in secondi: indice 0 = primo tentativo (no wait), poi 5s, 15s, 45s
    private static readonly int[] BackoffDelays = new[] { 0, 5, 15, 45 };

    private static readonly HashSet<HttpStatusCode> RetryableStatusCodes = new HashSet<HttpStatusCode>
    {
        HttpStatusCode.InternalServerError,     // 500
        HttpStatusCode.BadGateway,              // 502
        HttpStatusCode.ServiceUnavailable,      // 503
        HttpStatusCode.GatewayTimeout,          // 504
        HttpStatusCode.TooManyRequests          // 429
    };

    private readonly HttpClient _httpClient;

    public DatamarsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public async Task<DatamarsAuthResponse> AuthenticateAsync(
    string authEndpoint,
    DatamarsCredentials credenziali,
    CancellationToken cancellationToken = default)
    {
        var formContent = new FormUrlEncodedContent(new[]
        {
        new KeyValuePair<string, string>("client_id",     credenziali.ClientId),
        new KeyValuePair<string, string>("client_secret", credenziali.ClientSecret),
        new KeyValuePair<string, string>("grant_type",    credenziali.GrantType)
    });

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSecondi));

        using var response = await _httpClient.PostAsync(authEndpoint, formContent, cts.Token);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Autenticazione Datamars fallita. Status: {(int)response.StatusCode}. Body: {errorBody}",
                null,
                response.StatusCode);
        }

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonConvert.DeserializeObject<DatamarsAuthResponse>(responseJson)
               ?? throw new JsonParsingException(authEndpoint, "Risposta di autenticazione deserializzata come null.");
    }

    /// <inheritdoc/>
    public async Task<List<DatamarsIntegrationSessionItem>> GetIntegrationSessionsAsync(
        string farmId,
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{baseUrl.TrimEnd('/')}/farms/{farmId}/integrationSessions";

        var responseJson = await ExecuteWithRetryAsync(endpoint, apiKey, cancellationToken);

        // Datamars restituisce un oggetto con property "content" contenente l'array
        var root = JToken.Parse(responseJson);
        var contentToken = root is JObject obj ? obj["content"] : null;
        var arrayJson = contentToken?.ToString() ?? responseJson;

        return JsonConvert.DeserializeObject<List<DatamarsIntegrationSessionItem>>(arrayJson)
                   ?? new List<DatamarsIntegrationSessionItem>();
    }

    /// <inheritdoc/>
    public async Task<DatamarsSessioneDettaglio> GetSessioneDettaglioAsync(
        string sessionId,
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken = default)
    {
        var endpoint = $"{baseUrl.TrimEnd('/')}/integrationSessions/{sessionId}";

        var responseJson = await ExecuteWithRetryAsync(endpoint, apiKey, cancellationToken);

        return JsonConvert.DeserializeObject<DatamarsSessioneDettaglio>(responseJson)
               ?? throw new JsonParsingException(sessionId, "Risposta deserializzata come null.");
    }

    /// <summary>
    /// Esegue una chiamata GET verso <paramref name="endpoint"/> con retry esponenziale.
    /// Restituisce il body della risposta come stringa JSON.
    /// </summary>
    private async Task<string> ExecuteWithRetryAsync(
        string endpoint,
        string apiKey,
        CancellationToken cancellationToken)
    {
        int attempt = 0;
        HttpStatusCode? lastStatusCode = null;
        Exception? lastException = null;

        while (attempt <= MaxRetries)
        {
            // backoff: no wait al primo tentativo
            if (attempt > 0)
            {
                var delaySec = BackoffDelays[Math.Min(attempt, BackoffDelays.Length - 1)];
                await Task.Delay(TimeSpan.FromSeconds(delaySec), cancellationToken);
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(TimeoutSecondi));

                using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                using var response = await _httpClient.SendAsync(request, cts.Token);

                lastStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync(cancellationToken);

                // HTTP 4xx (escluso 429) e 422: fail fast, no retry
                if (!RetryableStatusCodes.Contains(response.StatusCode))
                {
                    var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new HttpRequestException(
                        $"Risposta non recuperabile dall'API Datamars. Status: {(int)response.StatusCode}. Body: {errorBody}",
                        null,
                        response.StatusCode);
                }

                // Status retryable: lancia per attivare il ciclo di retry
                lastException = new HttpRequestException(
                    $"API Datamars ha risposto con status retryable: {(int)response.StatusCode}.",
                    null,
                    response.StatusCode);
            }
            catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Timeout del singolo tentativo: retryable
                lastException = ex;
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue && !RetryableStatusCodes.Contains(ex.StatusCode.Value))
            {
                // Errore non retryable già sollevato internamente: propaga subito
                throw;
            }
            catch (JsonException ex)
            {
                // Parsing JSON fallito: fail fast, no retry
                throw new JsonParsingException(endpoint, ex.Message, ex);
            }
            catch (HttpRequestException ex)
            {
                // Errore di connessione/rete: retryable
                lastException = ex;
            }

            attempt++;
        }

        throw new ApiRetryExhaustedException(MaxRetries, (int?)lastStatusCode, endpoint, lastException);
    }
}
