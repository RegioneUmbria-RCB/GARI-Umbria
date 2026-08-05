using AgronicaCoreDTOStd.Identity;
using AgronicaCoreVarieBizSTD;
using AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti
{
    /// <summary>
    /// Recupera fertilizzanti e formulati dall'engine Banche Dati tramite chiamate HTTP POST.
    /// </summary>
    public sealed class ProdottiBancheDatiEngineService : BaseService, IProdottiBancheDatiEngineService
    {
        private const string ChiaveUrlEngine = "urlEngine_ProfitosanApi";
        private const string ChiaveApiKey = "apiKeyEngine_ProfitosanApi";
        private const string ChiaveUrlEngineOverride = "UrlEngineBancheDati";
        private const string ChiaveApiKeyOverride = "ApiKeyEngineBancheDati";
        private const string ChiaveSuffissoUrlEngine = "SuffissoEngineBancheDati";
        private const string SuffissoUrlEngineDefault = "profitosan-banchedati";
        private const string EndpointFertilizzanti = "/api/v1/prodotti/fertilizzanti";
        private const string EndpointFitofarmaci = "/api/v1/prodotti/fitofarmaci";

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly ChiamaCoreWS _coreWsClient;
        private readonly ILoggingService _loggingService;
        private readonly IConfiguration _configuration;

        public ProdottiBancheDatiEngineService(IServiceProvider provider) : base(provider)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient       = provider.GetRequiredService<HttpClient>();
            _coreWsClient     = provider.GetRequiredService<ChiamaCoreWS>();
            _loggingService   = provider.GetRequiredService<ILoggingService>();
            _configuration    = provider.GetRequiredService<IConfiguration>();
        }

        /// <inheritdoc />
        public async Task<bool> IsEngineConfiguratoAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var (urlEngine, apiKey) = await ResolveEngineCredentialsAsync(objParametriServer, objParametriSuperServer);
            return !string.IsNullOrWhiteSpace(urlEngine) && !string.IsNullOrWhiteSpace(apiKey);
        }

        private async Task<(string? UrlEngine, string? ApiKey)> ResolveEngineCredentialsAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            var urlOverride    = _configuration[ChiaveUrlEngineOverride];
            var apiKeyOverride = _configuration[ChiaveApiKeyOverride];

            if (!string.IsNullOrWhiteSpace(urlOverride) && !string.IsNullOrWhiteSpace(apiKeyOverride))
                return (ApplicaSuffissoUrl(urlOverride), apiKeyOverride);

            try
            {
                var (urlEngine, apiKey) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(
                    ChiaveUrlEngine, ChiaveApiKey, objParametriServer, objParametriSuperServer);
                return (ApplicaSuffissoUrl(urlEngine), apiKey);
            }
            catch
            {
                return (null, null);
            }
        }

        private string? ApplicaSuffissoUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return url;
            var suffisso = _configuration[ChiaveSuffissoUrlEngine] ?? SuffissoUrlEngineDefault;
            if (string.IsNullOrWhiteSpace(suffisso)) return url;

            var urlNorm     = url.TrimEnd('/');
            var suffissoNorm = suffisso.Trim('/');
            return urlNorm.EndsWith($"/{suffissoNorm}", StringComparison.OrdinalIgnoreCase)
                ? url
                : $"{urlNorm}/{suffissoNorm}";
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<FertilizzanteEngineDto>> GetFertilizzantiAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            RicercaFertilizzantiFiltriEngineDto filtri,
            CancellationToken cancellationToken = default)
        {
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));
            if (filtri is null) throw new ArgumentNullException(nameof(filtri));

            var result = await EseguiChiamataAsync<FertilizzanteEngineDto>(
                EndpointFertilizzanti, objParametriServer, objParametriSuperServer, cancellationToken,
                body: filtri);

            _loggingService.LogInformation($"Recuperati {result.Count} fertilizzanti dall'engine Banche Dati.");
            return result;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<FormulatoEngineDto>> GetFormulatiAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            RicercaFitofarmaciFilterEngineDto filtri,
            CancellationToken cancellationToken = default)
        {
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));
            if (filtri is null) throw new ArgumentNullException(nameof(filtri));

            var result = await EseguiChiamataAsync<FormulatoEngineDto>(
                EndpointFitofarmaci, objParametriServer, objParametriSuperServer, cancellationToken,
                body: filtri);

            _loggingService.LogInformation($"Recuperati {result.Count} formulati dall'engine Banche Dati.");
            return result;
        }

        private async Task<IReadOnlyList<T>> EseguiChiamataAsync<T>(
            string endpointPath,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken,
            object? body = null,
            bool unwrapSingleItem = false)
        {
            var (urlEngine, apiKey) = await ResolveEngineCredentialsAsync(objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlEngine) || string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException($"Credenziali engine Banche Dati non configurate. Endpoint: {endpointPath}");

            var requestUrl = $"{urlEngine.TrimEnd('/')}{endpointPath}";

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var bodyJson = body is not null ? JsonConvert.SerializeObject(body) : "{}";
            request.Content = new StringContent(bodyJson, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                _loggingService.LogWarning($"Chiamata all'engine Banche Dati annullata ({endpointPath}).");
                throw;
            }
            catch (Exception ex)
            {
                _loggingService.LogError($"Errore durante la chiamata all'engine Banche Dati ({endpointPath}): {ex.Message}");
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
                _loggingService.LogError($"L'engine Banche Dati ha restituito HTTP {(int)response.StatusCode} per {endpointPath}. Body: {errorBody}");

                string errorMessage = errorBody;
                try
                {
                    var errorObj = JsonConvert.DeserializeObject<JObject>(errorBody);
                    errorMessage = errorObj?["message"]?.ToString() ?? errorBody;
                }
                catch { /* mantieni errorBody grezzo se non è JSON valido */ }

                throw new HttpRequestException(errorMessage, inner: null, statusCode: response.StatusCode);
            }

            var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);

            if (unwrapSingleItem)
            {
                var single = JsonConvert.DeserializeObject<T>(responseBody);
                return single is null ? Array.Empty<T>() : new[] { single };
            }

            var result = JsonConvert.DeserializeObject<List<T>>(responseBody);
            return result is null ? Array.Empty<T>() : (IReadOnlyList<T>)result;
        }

        public async Task<List<ProdottoEntity>> GetProdottiAsync(string piva, int categoria, string filtro, int specie, string codici, string stato, int lav_cod,
            ObjParametri objParametri, AgronicaCoreParametriTriple tripleParams, string bearerToken, string coreWsUrl)
        {
            var url = coreWsUrl + "/Anagrafica/Prodotti.asmx/RicercaProdottiSenzaGiacenza";

            var core_ws_prodotti = new CoreWS_Prodotti(objParametri.objP_super_server, objParametri.objP_server, objParametri.objP_utenti, piva, categoria, filtro, specie.ToString(), codici, stato, lav_cod);

            var risposta = await CallCoreWSAsync(tripleParams, bearerToken, url, core_ws_prodotti);

            var r = JsonConvert.DeserializeObject<RispostaStandard>(risposta);

            if (r?.RispostaOK != true) {
                var errorMessage = r?.Errore ?? "Errore nella lettura prodotti dal web service." + $" Categoria: {categoria}";
                _loggingService.LogError(errorMessage);
                throw new HttpRequestException(errorMessage, inner: null, statusCode: System.Net.HttpStatusCode.InternalServerError);
            }

            var prodotti = JsonConvert.DeserializeObject<List<ProdottiEntity>>(r.RispostaStringa) ?? new List<ProdottiEntity>();

            return prodotti
                // .Where(x => string.IsNullOrEmpty(piva) || x.Piva == piva)
                .Select(x => new ProdottoEntity
                {
                    //IsTrappolaFormulato = x.IsTrappolaFormulato,
                    codice           = x.Prodotto_Cod,
                    descrizione      = x.Prodotto_Des,
                    nomeComune       = x.NomeComune,
                    elemCod          = x.Elem_Cod,
                    unitaDiMisuraCod = x.Udm_Cod,
                    specieCod        = x.Veg_Cod,
                    N                = x.N,
                    P2O5             = x.P2O5,
                    K20              = x.K2O,
                    Cu               = x.Cu
                })
                .GroupBy(x => new { x.codice, x.elemCod })
                .Select(g => g.First())
                .ToList();
        }

        private async Task<string> CallCoreWSAsync(AgronicaCoreParametriTriple tripleParams, string bearerToken, string url, object input)
        {
            try
            {
                var json = await _coreWsClient.ChiamaCoreWSAsync(url, input, tripleParams, bearerToken, false);

                if (string.IsNullOrEmpty(json))
                    throw new Exception("Risposta nulla da web service");

                JObject jObj = JObject.Parse(json);
                JToken d = jObj["d"];

                return d.ToString();
            }
            catch (Exception ex)
            {
                LogError(ex.Message, tripleParams.ObjParametriServer, ex);
                throw;
            }
        }

        private class CoreWS_Prodotti
        {
            public string piva { get; set; }
            public int Elem_Cod { get; set; }
            public string FiltroDescrizioneProdotto { get; set; }
            public string Elenco_Specie { get; set; }
            public string Elenco_Codici { get; set; }
            public string Stato_Cod { get; set; }
            public int Lav_Cod { get; set; }

            public string objP_super_server { get; set; }
            public string objP_server { get; set; }
            public string objP_utenti { get; set; }

            public CoreWS_Prodotti(string objP_super_server, string objP_server, string objP_utenti, string piva, int Elem_Cod, 
                string FiltroDescrizioneProdotto, string Elenco_Specie, string Elenco_Codici, string Stato_Cod, int Lav_Cod)
            {
                this.objP_super_server = objP_super_server;
                this.objP_server = objP_server;
                this.objP_utenti = objP_utenti;

                var filtro = new JArray { new JObject { new JProperty("value", FiltroDescrizioneProdotto) } };

                this.piva = piva;
                this.Elem_Cod = Elem_Cod;
                this.FiltroDescrizioneProdotto = JsonConvert.SerializeObject(filtro);
                this.Elenco_Specie = Elenco_Specie;
                this.Elenco_Codici = Elenco_Codici;
                this.Stato_Cod = Stato_Cod;
                this.Lav_Cod = Lav_Cod;
            }
        }

        private class ProdottiEntity
        {
            public int Elem_Cod;
            public string NomeComune;
            public int Prodotto_Cod;
            public string Prodotto_Des;
            public double Prodotto_Giacenza;
            public double N;
            public double P2O5;
            public double K2O;
            public double Cu;
            public int Uso;
            public string Piva;
            public int Sa_Cod;
            public int Udm_Cod;
            public int Veg_Cod;
            public bool IsTrappolaFormulato;
        }

    }
}
