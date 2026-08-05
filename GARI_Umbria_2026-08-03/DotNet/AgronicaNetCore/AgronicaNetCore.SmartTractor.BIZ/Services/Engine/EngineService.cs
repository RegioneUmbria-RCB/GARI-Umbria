using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SmartTractor.BIZ.Exceptions;
using AgronicaNetCore.SmartTractor.BIZ.Models;
using AgronicaNetCore.SmartTractor.BIZ.Resources;
using AgronicaNetCore.SmartTractor.BIZ.Services.DispatcherManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.Engine
{
    /// <summary>
    /// Handles outbound HTTP communication with the Smart Tractor engine API.
    /// Engine URL and API key are read from the DB on every call via <see cref="ISecurityLayerDAL"/>.
    /// </summary>
    public class EngineService : BaseServiceSmartTractorBIZ, IEngineService
    {
        private const int TimeoutGlobaleSecondi = 60;
        private const int TimeoutEngineSecondi = 300;
        private const string ChiaveUrlEngine = "urlEngine_SmartTractor_BaseUrl";
        private const string ChiaveTenantEngine = "tenantEngine_SmartTractor";
        private const string ChiaveApiKey = "apiKeyEngine_SmartTractor";

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _httpClient;
        private readonly ILoggingService _loggingService;
        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly IAuthDispatcherService _authDispatcherService;

        public EngineService(IServiceProvider provider, IStringLocalizer<Messages> localizer) : base(provider, localizer)
        {
            _httpClient = provider.GetRequiredService<HttpClient>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _authDispatcherService = provider.GetRequiredService<IAuthDispatcherService>();
        }

        /// <inheritdoc/>
        public async Task<SendPrescriptionResponse> SendPrescriptionAsync(
            SmartTractorPayload payload,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            CancellationToken cancellationToken = default)
        {
            if (payload is null) throw new ArgumentNullException(nameof(payload));

            // Read engine URL and API key from DB (mirrors OrchestrationAcquisizioneFasiFenologicheService pattern)
            var (urlEngine, apiKey) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, serverParams, superServerParams);
            //var urlEngine = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveUrlEngine, serverParams, superServerParams);
            var tenant = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveTenantEngine, serverParams, superServerParams);
            var fmisId = serverParams.UsernameOperazione;

            if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            if (!urlEngine.StartsWith("http"))
            {
                var ex = new Exception("Missing protocol on Smart Tractor engine base url");
                _loggingService.LogError(
                    $"Smart Tractor engine base url {urlEngine}: missing protocol",
                    serverParams, ex);
                throw ex;
            }

            var requestUrl = $"{urlEngine.TrimEnd('/')}/{tenant}/public/v1/organizations/{fmisId}/providers/{payload.providerCode}/prescriptions";

            payload.tenant = tenant;
            var payloadJson = JsonSerializer.Serialize(payload, _jsonOptions);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutEngineSecondi));
            using var linkedCts  = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            _loggingService.LogInformation(
                $"Sending prescription to Smart Tractor. Url={requestUrl}, Tenant={payload.tenant}, ProviderCode={payload.providerCode}.",
                serverParams);

            //var bearerToken = await _authDispatcherService.GenerateNewBearerTokenAsync(serverParams, superServerParams);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            httpRequest.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await _httpClient.SendAsync(httpRequest, linkedCts.Token);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _loggingService.LogError(
                    $"Smart Tractor engine timeout after {TimeoutEngineSecondi}s. Tenant={payload.tenant}.",
                    serverParams, ex);
                throw new SmartTractorSendException(
                    0,
                    $"Engine timeout after {TimeoutEngineSecondi} seconds.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                _loggingService.LogError(
                    $"HTTP connection error sending prescription to Smart Tractor. Tenant={payload.tenant}.",
                    serverParams, ex);
                throw new SmartTractorSendException(
                    0,
                    $"HTTP connection error: {ex.Message}",
                    ex);
            }

            using (httpResponse)
            {
                var responseBody = await httpResponse.Content.ReadAsStringAsync(linkedCts.Token);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _loggingService.LogError(
                        $"Smart Tractor engine returned HTTP {(int)httpResponse.StatusCode}. Tenant={payload.tenant}, Body={responseBody}.",
                        serverParams);
                    var resp = JsonSerializer.Deserialize<HttpSendPrescriptionErrorResponse>(responseBody);
                    return new SendPrescriptionResponse() { ActivityId = 0, error = resp.error, status = SmartTractorRequestStatus.Failed };
                }

                _loggingService.LogInformation(
                    $"Prescription sent successfully to Smart Tractor. Tenant={payload.tenant}.",
                    serverParams);

                return ParseResponse(responseBody);
            }
        }

        /// <inheritdoc/>
        public async Task<UploadRasterMapEngineResponse> UploadRasterMapAsync(
            int mapId,
            string providerId,
            byte[] tiffBytes,
            AgronicaCoreParametriServer serverParams,
            AgronicaCoreParametriSuperServer superServerParams,
            CancellationToken cancellationToken = default)
        {
            if (tiffBytes is null || tiffBytes.Length == 0)
                throw new RasterUploadException(mapId, "Raster file bytes are empty.");

            var (urlEngine, apiKey) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, serverParams, superServerParams);
            //var urlEngine = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveUrlEngine, serverParams, superServerParams);
            var tenant = await _securityLayerDal.LeggiConfigurazioneSitiScalareAsync(ChiaveTenantEngine, serverParams, superServerParams);
            var fmisId = serverParams.UsernameOperazione;

            if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            if (!urlEngine.StartsWith("http"))
            {
                var ex = new Exception("Missing protocol on Smart Tractor engine base url");
                _loggingService.LogError(
                    $"Smart Tractor engine base url {urlEngine}: missing protocol",
                    serverParams, ex);
                throw ex;
            }

            var requestUrl = $"{urlEngine.TrimEnd('/')}/{tenant}/public/v1/organizations/{fmisId}/providers/{providerId}/prescriptions/attachment";

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutEngineSecondi));
            using var linkedCts  = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            _loggingService.LogInformation(
                $"Uploading raster map {mapId} to Smart Tractor. Url={requestUrl}, Size={tiffBytes.Length} bytes.",
                serverParams);

            //var bearerToken = await _authDispatcherService.GenerateNewBearerTokenAsync(serverParams, superServerParams);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            //httpRequest.Headers.Add("Authorization", $"Bearer {bearerToken.auth_token}");
            httpRequest.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            httpRequest.Content = new ByteArrayContent(tiffBytes);
            httpRequest.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            HttpResponseMessage httpResponse;
            try
            {
                httpResponse = await _httpClient.SendAsync(httpRequest, linkedCts.Token);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                _loggingService.LogError(
                    $"Smart Tractor raster upload timeout after {TimeoutEngineSecondi}s. MapId={mapId}.",
                    serverParams, ex);
                throw new RasterUploadException(mapId, $"Upload timeout after {TimeoutEngineSecondi} seconds.");
            }
            catch (HttpRequestException ex)
            {
                _loggingService.LogError(
                    $"HTTP connection error uploading raster map. MapId={mapId}.",
                    serverParams, ex);
                throw new RasterUploadException(mapId, $"HTTP connection error: {ex.Message}");
            }

            using (httpResponse)
            {
                var responseBody = await httpResponse.Content.ReadAsStringAsync(linkedCts.Token);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _loggingService.LogError(
                        $"Smart Tractor raster upload returned HTTP {(int)httpResponse.StatusCode}. MapId={mapId}, Body={responseBody}.",
                        serverParams);

                    throw new RasterUploadException(mapId, $"HTTP {(int)httpResponse.StatusCode}: {responseBody}");
                }

                _loggingService.LogInformation(
                    $"Raster map {mapId} uploaded successfully to Smart Tractor.",
                    serverParams);

                return ParseUploadResponse(responseBody);
            }
        }

        // ─── Private helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Extracts the provider-assigned <c>activityId</c> from the engine response body.
        /// Returns an empty <see cref="SendPrescriptionResponse"/> when the body is absent or
        /// cannot be parsed.
        /// </summary>
        private static SendPrescriptionResponse ParseResponse(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return new SendPrescriptionResponse();

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var root = doc.RootElement;

                var response = JsonSerializer.Deserialize<DispatchPrescriptionEngineResponse>(responseBody);

                return new SendPrescriptionResponse() { ActivityId = response.id, error = string.Empty, status = SmartTractorRequestStatus.Sent };
            }
            catch (JsonException)
            {
                return new SendPrescriptionResponse();
            }
        }

        /// <summary>
        /// Extracts the provider-assigned <c>attachmentId</c> from the raster upload response body.
        /// Returns an empty <see cref="UploadRasterMapEngineResponse"/> when the body is absent or
        /// cannot be parsed.
        /// </summary>
        private static UploadRasterMapEngineResponse ParseUploadResponse(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return new UploadRasterMapEngineResponse();

            try
            {
                return JsonSerializer.Deserialize<UploadRasterMapEngineResponse>(responseBody) ?? new UploadRasterMapEngineResponse();
            }
            catch (JsonException)
            {
                return new UploadRasterMapEngineResponse();
            }
        }

        internal class HttpSendPrescriptionErrorResponse
        {
            public string error { get; set; }
        }
    }
}
