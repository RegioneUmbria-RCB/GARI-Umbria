using System.Net.Http.Headers;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Resources;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ModelloNutrizione.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.VerificaModelliNutrizione
{
    /// <summary>
    /// Servizio BIZ per la verifica della disponibilità di modelli di calcolo nutrizionale.
    /// Aggrega le coppie (specie, varietà) degli appezzamenti attivi dell'azienda,
    /// interroga l'engine di nutrizione in parallelo per ogni coppia univoca e
    /// restituisce il risultato aggregato.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione.
    /// DS13-API POST /v1/dss/nutrizione/modelli/verifica.
    /// </summary>
    public sealed class VerificaModelliNutrizioneService : BaseDSSNutrizioneBIZService, IVerificaModelliNutrizioneService
    {
        private const int    TimeoutEngineSecondi = 30;
        private const string ChiaveUrlEngine      = "urlEngine_Nutrizione";
        private const string ChiaveApiKey         = "apiKeyEngine_Nutrizione";
        private const string ChiaveTenantName = "tenantNameEngine_Nutrizione";

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient        _httpClient;
        private readonly ILoggingService   _loggingService;

        public VerificaModelliNutrizioneService(
            IServiceProvider provider,
            IStringLocalizer<Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _loggingService   = provider.GetRequiredService<ILoggingService>();
        }

        /// <inheritdoc/>
        public async Task<VerificaModelliNutrizioneResult> VerificaAsync(
            IReadOnlyList<CoppiaSpecieVarieta> coppie,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (coppie is null)
                throw new ArgumentNullException(nameof(coppie));

            var timestampVerifica = DateTimeOffset.UtcNow;

            // Step 1: Recupero configurazione engine (DS03-BL: chiavi apiKeyEngine_Nutrizione, urlEngine_Nutrizione, tenantNameEngine_Nutrizione).
            var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

            if (coppie.Count == 0)
            {
                return new VerificaModelliNutrizioneResult
                {
                    ModelliPerSpecieVarieta = Array.Empty<ModelliPerSpecieVarietaDto>(),
                    TimestampVerifica       = timestampVerifica.ToString("O")
                };
            }

            // Step 2: Chiamata engine in parallelo per ogni coppia univoca.
            // DS03-BL: una richiesta per ogni coppia (specie, varietà) univoca;
            // il fallimento su una coppia è isolato e non blocca le altre.
            var tasks = coppie
                .Select(coppia => InterrogaEngineSingoloAsync(
                    coppia, urlEngine, apiKey, tenantName, objParametriServer, cancellationToken))
                .ToArray();

            var risultati = await Task.WhenAll(tasks);

            return new VerificaModelliNutrizioneResult
            {
                ModelliPerSpecieVarieta = risultati,
                TimestampVerifica       = timestampVerifica.ToString("O")
            };
        }

        // ── Chiamata engine: fallimento isolato per coppia ───────────────────

        /// <summary>
        /// Chiama l'engine per una singola coppia (specie, varietà).
        /// DS03-BL: se l'engine non risponde, lo status è ERROR e non blocca le altre coppie.
        /// </summary>
        private async Task<ModelliPerSpecieVarietaDto> InterrogaEngineSingoloAsync(
            CoppiaSpecieVarieta coppia,
            string urlEngine,
            string apiKey,
            string tenantName,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken)
        {
            try
            {
                var modelli = await ChiamaEngineAsync(coppia, urlEngine, apiKey, tenantName, cancellationToken);

                return new ModelliPerSpecieVarietaDto
                {
                    Veg_Cod     = coppia.Veg_Cod,
                    Cul_Cod = coppia.Cul_Cod,
                    NumModelli = modelli.Count,
                    Modelli    = modelli.Select(m => new ModelloNutrizioneDto
                    {
                        Id       = m.Id,
                        DetailId = m.DetailId,
                        Code     = m.Code
                    }).ToArray(),
                    Status = modelli.Count > 0
                        ? StatoModelloNutrizione.Available
                        : StatoModelloNutrizione.NotAvailable
                };
            }
            catch (Exception ex)
            {
                _loggingService.LogWarning(
                    $"Errore chiamata engine Nutrizione per Veg_Cod={coppia.Veg_Cod} Cul_Cod={coppia.Cul_Cod}: {ex.Message}",
                    objParametriServer,
                    ex);

                return new ModelliPerSpecieVarietaDto
                {
                    Veg_Cod = coppia.Veg_Cod,
                    Cul_Cod = coppia.Cul_Cod,
                    NumModelli = 0,
                    Modelli    = Array.Empty<ModelloNutrizioneDto>(),
                    Status     = StatoModelloNutrizione.Error
                };
            }
        }

        private async Task<IReadOnlyList<ModelloNutrizioneItem>> ChiamaEngineAsync(
            CoppiaSpecieVarieta coppia,
            string urlEngine,
            string apiKey,
            string tenantName,
            CancellationToken cancellationToken)
        {
            // GET {urlEngine}?crop_code={vegCod}
            // Riferimento: GET /dss-api/NUTRITION/{tenant}/public/api/v1/models?crop_code={crop_code}
            // variety_code non è ancora gestito dall'engine; aggiungere &variety_code={Cul_Cod} quando supportato.
            string requestUrl = $"{urlEngine.TrimEnd('/')}/{tenantName}/public/api/v1/models?crop_code={coppia.Veg_Cod}";

            using var requestMsg = new HttpRequestMessage(HttpMethod.Get, requestUrl);
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutEngineSecondi));
            using var linkedCts  = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(
                    requestMsg, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                throw new ModelloNutrizioneEngineTimeoutException(TimeoutEngineSecondi);
            }

            string responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);

            if (!response.IsSuccessStatusCode)
                throw new ModelloNutrizioneEngineHttpException(response.StatusCode, responseBody);

            return ParseModelli(responseBody);
        }

        private static IReadOnlyList<ModelloNutrizioneItem> ParseModelli(string responseBody)
        {
            JArray modelsArray;
            try
            {
                modelsArray = JArray.Parse(responseBody);
            }
            catch (JsonException ex)
            {
                throw new ModelloNutrizioneEngineParseException(
                    "Il corpo della risposta non è un JSON valido.", ex);
            }

            var result = new List<ModelloNutrizioneItem>(modelsArray.Count);
            foreach (JToken item in modelsArray)
            {
                result.Add(new ModelloNutrizioneItem
                {
                    Id       = item["id"]?.Value<string>()       ?? string.Empty,
                    DetailId = item["detailId"]?.Value<string>() ?? string.Empty,
                    Code     = item["code"]?.Value<string>()     ?? string.Empty
                });
            }
            return result;
        }

    }
}
