//using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSDifesa.BIZ.Resources;
using AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;
using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.ModelliDifesa.Models;
using InData.Engine.DSSDifesa;
using Microsoft.Extensions.Localization;
using OutData.Engine.DSSDifesa;
using System.Net.Http.Headers;
using System.Text.Json;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Acquisizione
{
    /// <summary>
    /// Service for retrieving the dynamic list of pest models for a crop from DSS Engine.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica.
    /// </summary>
    public class AcquisizioneModelliDifesaService : BaseDSSDifesaBIZService, IAcquisizioneModelliDifesaService
    {
        private const string ChiaveUrlEngine = "urlEngine_DSSDifesa";
        private const string ChiaveApiKey = "apiKeyEngine_DSSDifesa";
        private const string ChiaveTenantName = "tenantNameEngine_DSSDifesa";
        private const int TimeoutSeconds = 20;

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the AcquisizioneModelliDifesaService.
        /// </summary>
        /// <param name="provider">The service provider.</param>
        /// <param name="localizer">The localizer for resources.</param>
        /// <param name="securityLayerDal">The security layer DAL for configuration.</param>
        /// <param name="httpClient">The HTTP client for API calls.</param>
        public AcquisizioneModelliDifesaService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer,
            ISecurityLayerDAL securityLayerDal,
            HttpClient httpClient)
            : base(provider, localizer)
        {
            _securityLayerDal = securityLayerDal ?? throw new ArgumentNullException(nameof(securityLayerDal));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Retrieves the dynamic list of pest models for the specified crop.
        /// </summary>
        /// <param name="request">The request containing crop code, variety code and model codes.</param>
        /// <param name="objParametriServer">Server parameters.</param>
        /// <param name="objParametriSuperServer">Super server parameters.</param>
        /// <returns>The response containing the list of pest models, status, and fetch timestamp.</returns>
        public async Task<AcquisizioneModelliDSSDifesaResponse> RecuperaListaModelliDifesaDinamicaAsync(
            AcquisizioneModelliDSSDifesaRequest request,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            if (string.IsNullOrWhiteSpace(request.CropCode))
                throw new ArgumentException("CropCode cannot be null or whitespace.", nameof(request.CropCode));
            
            var fetchTimestamp = DateTimeOffset.UtcNow.ToString("O");

            try
            {
                // Retrieve configuration
                var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(
                    ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

                if (string.IsNullOrWhiteSpace(urlEngine))
                    throw new InvalidOperationException($"Configuration missing: key '{ChiaveUrlEngine}' not found.");

                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new InvalidOperationException($"Configuration missing: key '{ChiaveApiKey}' not found.");

                if (string.IsNullOrWhiteSpace(tenantName))
                    throw new InvalidOperationException($"Configuration missing: key '{ChiaveTenantName}' not found.");

                string apiUrl = urlEngine.TrimEnd('/') + "/dss-api/DEFENSE/" + tenantName.TrimStart('/').TrimEnd('/')+ "/public/api/v1";

                //using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSeconds));

                using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(TimeoutSeconds));
                using CancellationTokenSource linkedCts =
                    CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

                // Determine API endpoint and call
                List<ModelloDifesa> modelliDifesa;
                if (request.ModelliCodici == null || request.ModelliCodici.Count == 0)
                {
                    // Retrieve all models for the crop
                    var endpoint = $"/models?crop_code={Uri.EscapeDataString(request.CropCode)}";
                    if (!string.IsNullOrWhiteSpace(request.VarCode))
                        endpoint += $"&variety_code={Uri.EscapeDataString(request.VarCode)}";

                    modelliDifesa = await CallApiForAllModelsAsync(apiUrl, apiKey, endpoint, linkedCts);
                }
                else
                {
                    // Retrieve specific models
                    modelliDifesa = new List<ModelloDifesa>();
                    foreach (var code in request.ModelliCodici)
                    {
                        var endpoint = $"/models/{Uri.EscapeDataString(code)}";
                        var model = await CallApiForSpecificModelAsync(apiUrl, apiKey, endpoint, linkedCts);
                        if (model != null)
                            modelliDifesa.Add(model);
                    }
                }

                // Validate results
                if (modelliDifesa.Count == 0)
                    throw new CulturaIllegittimException("NESSUN_INFESTANTE_MAPPATO");

                // Deduplicate
                modelliDifesa = modelliDifesa
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    .ToList();

                return new AcquisizioneModelliDSSDifesaResponse
                {
                    Modelli = modelliDifesa,
                    Status = "OK",
                    FetchTimestamp = fetchTimestamp
                };
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                LogModelliDSSDifesaAcquisition(request, "ERRORE", $"API Timeout: {ex.Message}");
                throw new EngineApiTimeoutException($"API Engine DSS non risponde entro timeout ({TimeoutSeconds})", ex);
            }
            catch (HttpRequestException ex)
            {
                // Handle 404 as crop not supported
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    LogModelliDSSDifesaAcquisition(request, "ERRORE", $"Crop not supported: {ex.Message}");
                    throw new CulturaIllegittimException("COLTURA_NON_SUPPORTATA", ex);
                }
                LogModelliDSSDifesaAcquisition(request, "ERRORE", $"http error: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                LogModelliDSSDifesaAcquisition(request, "ERRORE", $"json error: {ex.Message}");
                throw new EngineApiInvalidResponseException("API ritorna JSON malformato", ex);
            }
        }

        private async Task<List<ModelloDifesa>> CallApiForAllModelsAsync(string apiUrl, string apiKey, string endpoint, CancellationTokenSource cts)
        {
            var fullUrl = apiUrl.TrimEnd('/') + endpoint;

            using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cts.Token);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            var models = JsonSerializer.Deserialize<List<ModelloDifesa>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (models == null)
                throw new EngineApiInvalidResponseException("API returned null response");

            // Validate each model has id
            foreach (var model in models)
            {
                if (model.Id == 0) // Assuming int default is 0, but check if it's set
                    models.Remove(model); // Exclude malformed
            }

            return models;
        }

        private async Task<ModelloDifesa?> CallApiForSpecificModelAsync(string urlEngine, string apiKey, string endpoint, CancellationTokenSource cts)
        {
            var fullUrl = urlEngine.TrimEnd('/') + endpoint;

            using var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var response = await _httpClient.SendAsync(request, cts.Token);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null; // Model not found, skip

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cts.Token);
            var model = JsonSerializer.Deserialize<ModelloDifesa>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (model == null || model.Id == 0)
                return null; // Exclude malformed

            return model;
        }

        private void LogModelliDSSDifesaAcquisition(
            AcquisizioneModelliDSSDifesaRequest request,
            string status,
            string? errorMessage)
        {
            string crop = $"{request.CropCode}";
            if (string.IsNullOrWhiteSpace(request.VarCode))
                crop += $", Variety:{request.VarCode}";

            string modelliRichiesti = "[";
            if (request.ModelliCodici is not null)
            {
                foreach (string modello in request.ModelliCodici)
                {
                    modelliRichiesti += modello;
                }
            }
            modelliRichiesti += "]";

            if (string.IsNullOrEmpty(errorMessage))
            {
                LogInformation(
                    $"DSS Difesa models acquisition successful. Crop: {crop}, Requested Models: {modelliRichiesti}, Status: {status}");
            }
            else
            {
                LogWarning(
                    $"DSS Difesa models acquisition failed. Crop: {crop}, Status: {status}, Error: {errorMessage}");
            }
        }
    }
}