using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.Base.Utility;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Exceptions;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.Webhook.DAL.DataLayer.Models;
using AgronicaNetCore.Webhook.DAL.DataLayer.WebhookTestata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Implementazione del servizio di richiesta del consiglio nutrizionale per un appezzamento.
    /// Flusso: validazione → recupero configurazione engine → chiamata API →
    ///          salvataggio in WebHook_Testata → polling (DS15-BL) →
    ///          persistenza in Consigli_Nutrizione_Engine e Input_Consigli_Nutrizione_Engine.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione Appezzamenti.
    /// </summary>
    public sealed class RichiestaConsiglioNutrizioneService : BaseService, IRichiestaConsiglioNutrizioneService
    {
        private const int    TimeoutEngineSecondi = 30;
        private const string ChiaveUrlEngine      = "urlEngine_Nutrizione";
        private const string ChiaveApiKey         = "apiKeyEngine_Nutrizione";
        private const string ChiaveTenantName     = "tenantNameEngine_Nutrizione";

        private readonly ISecurityLayerDAL                          _securityLayerDal;
        private readonly HttpClient                                 _httpClient;
        private readonly ISalvataggioConsiglioNutrizioneService     _salvataggioService;
        private readonly IPollingWebHookConsiglioNutrizioneService  _polling;
        private readonly ILoggingService                            _loggingService;
        private readonly IWebhookTestataDAL                         _webhookTestataDal;
        private readonly IAgro_Sequence                            _seq;
        private readonly IConfiguration                            _config;

        public RichiestaConsiglioNutrizioneService(IServiceProvider provider) : base(provider)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient       = provider.GetRequiredService<HttpClient>();
            _salvataggioService = provider.GetRequiredService<ISalvataggioConsiglioNutrizioneService>();
            _polling          = provider.GetRequiredService<IPollingWebHookConsiglioNutrizioneService>();
            _loggingService   = provider.GetRequiredService<ILoggingService>();
            _webhookTestataDal = provider.GetRequiredService<IWebhookTestataDAL>();
            _seq = provider.GetRequiredService<IAgro_Sequence>();
            _config = provider.GetRequiredService<IConfiguration>();
        }

        /// <inheritdoc/>
        public async Task<RichiestaConsiglioNutrizioneResult> EseguiAsync(
            RichiestaConsiglioNutrizioneInput input,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int IdDbServer,
            bool salvaConsiglio = true,
            int raccoglitoreCod = 0,
            CancellationToken cancellationToken = default)
        {
            if (input is null)              throw new ArgumentNullException(nameof(input));
            if (input.DatiImpianto is null) throw new ArgumentNullException(nameof(input.DatiImpianto));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            var tipo = (short)WebhookTipo.EsitoConsiglioNutrizione;

            // Step 1: Recupero configurazione engine
            var (urlEngine, apiKey, tenantName) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine, ChiaveApiKey, ChiaveTenantName, objParametriServer, objParametriSuperServer);

            // Step 2: Costruzione payload
            var payload = CostruisciPayload(input);
            var payloadJson = JsonConvert.SerializeObject(payload);

            // ── Transaction 1: sequenza + tracking WebHook_Testata ────────────────
            int idTestata = 0;
            string requestId = "";

            bool connectionOpened = false;
            try
            {
                await OpenConnectionAsync(objParametriServer, OpenTransaction: true);
                connectionOpened = true;

                idTestata = await _seq.NuovoId_TabellaAsync("WebHook_Testata", 0, 2_000_000_000, objParametriServer);

                var fmisContextData = new FmisContextDataNutrizione(IdDbServer, idTestata, raccoglitoreCod);

                // Step 3: Chiamata all'engine (POST asincrona; restituisce requestId)
                var initialResponse = await ChiamaEngineAsync(urlEngine, apiKey, tenantName, input.ModelCode, fmisContextData, payloadJson, cancellationToken);

                requestId = initialResponse.RequestId;

                // Step 4: Salvataggio in WebHook_Testata per il tracking asincrono
                await _webhookTestataDal.InsertTestataAsync(idTestata, tipo, requestId, raccoglitoreCod, payloadJson, objParametriServer);

                // COMMIT transaction 1 and close the connection before polling
                CloseTransaction(objParametriServer, Rollback: false);
                CloseConnection(objParametriServer);
                connectionOpened = false;
            }
            catch (Exception)
            {
                if (connectionOpened)
                    CloseTransaction(objParametriServer, Rollback: true);
                throw;
            }

            // Step 5: Polling DS15-BL — attesa del risultato dalla tabella WebHook_Testata
            var pollingInput = new PollingWebHookInput
            {
                RequestId           = requestId,
                TestataId           = idTestata,
                TimeoutSecondi      = 10,
                IntervalloPollingMs = 500
            };

            var pollingResult = await _polling.EseguiPollingAsync(pollingInput, objParametriServer, cancellationToken);

            if (pollingResult.Status is "TIMEOUT" or "NOT_FOUND")
            {
                _loggingService.LogWarning(
                    $"Polling per RequestId={requestId} terminato con Status={pollingResult.Status}.",
                    objParametriServer);

                return new RichiestaConsiglioNutrizioneResult
                {
                    Status   = "ERROR",
                    Messaggi = pollingResult.Messaggi
                };
            }

            if (pollingResult.Status == "ERROR" || pollingResult.Response is null)
            {
                return new RichiestaConsiglioNutrizioneResult
                {
                    Status   = "ERROR",
                    Messaggi = pollingResult.EsitoDettagli ?? pollingResult.Messaggi
                };
            }

            // ── Transaction 2: persistenza consiglio ──────────────────────────────
            int? consiglioId;

            if (salvaConsiglio)
            {
                var aggregazione = MappaPerSalvataggio(input, pollingResult.Response);
                var salvataggioResult = await _salvataggioService.EseguiAsync(
                    aggregazione, false,usernameRichiedente, objParametriServer, cancellationToken);
                consiglioId = salvataggioResult.SalvataggioId > 0 ? salvataggioResult.SalvataggioId : (int?)null;
            }
            else
                consiglioId = null;

            // Step 7: Mappatura risultato verso il DTO BIZ
            return MappaComeResult(pollingResult.Response, consiglioId);
        }

        // ── Step 2: Costruzione payload engine ───────────────────────────────────

        private static EngineConsiglioNutrizioneRequest CostruisciPayload(RichiestaConsiglioNutrizioneInput input)
        {
            var coltura = new EngineColturaNutrizioneInfo
            {
                SpecieVegetale       = input.DatiImpianto.VegCod.ToString(),
                Varieta              = input.DatiImpianto.CulCod.ToString(),
                TipoCodifica         = null,
                NumeroPiante         = input.DatiImpianto.NumPiante,
                Portinnesto          = input.DatiImpianto.PortinnestoCod,
                StatoImpianto        = input.DatiImpianto.StatoImpianto,
                GeometriaImpiantoWkt = input.DatiImpianto.GeometriaWkt,
                GeometriaImpiantoSrid = input.DatiImpianto.GeometriaSrid
            };

            // DS04-BL: fase fenologica BBCH è facoltativa
            string? bbch = input.DatiFaseFenologica?.CodiceBbch;

            EngineAnalisiTerreno? analisiTerreno = null;
            if (input.DatiAnalisiTerreno is not null)
            {
                analisiTerreno = new EngineAnalisiTerreno
                {
                    Data = input.DatiAnalisiTerreno.Data.ToString("yyyy-MM-dd"),
                    Sabbia   = input.DatiAnalisiTerreno.Sabbia,
                    Limo     = input.DatiAnalisiTerreno.Limo,
                    Argilla  = input.DatiAnalisiTerreno.Argilla,

                    ElencoElementiRilevati = input.DatiAnalisiTerreno.ElencoElementiRilevati
                        .Select(e => new EngineElementoRilevato
                        {
                            Elemento    = e.Elemento,
                            Quantitativo = e.Quantitativo
                        })
                        .ToArray()
                };
            }

            IReadOnlyList<EngineFertilizzazionePrecedente> fertilizzazioni = Array.Empty<EngineFertilizzazionePrecedente>();
            if (input.DatiFertilizzazioniPrecedenti.Count > 0)
            {
                fertilizzazioni = input.DatiFertilizzazioniPrecedenti
                    .Select(f => new EngineFertilizzazionePrecedente
                    {
                        Elemento          = f.Elemento,
                        Quantitativo      = f.Quantitativo,
                        FaseFenologicaBbch = f.FaseFenologicaBbch,
                        Data              = f.Data.ToString("yyyy-MM-dd")
                    })
                    .ToArray();
            }

            return new EngineConsiglioNutrizioneRequest
            {
                DataConsiglio                  = input.DataConsiglio.ToString("yyyy-MM-dd"),
                DataSemina                     = input.DataSemina.ToString("yyyy-MM-dd"),
                ColturaInfo                    = coltura,
                FaseFenologicaBbch             = string.IsNullOrWhiteSpace(bbch) ? null : bbch,
                DatiAnalisiTerreno             = analisiTerreno,
                ElencoFertilizzazioniPrecedenti = fertilizzazioni
            };
        }

        // ── Step 3: Chiamata HTTP all'engine ─────────────────────────────────────

        private async Task<EngineConsiglioNutrizioneInitialResponse> ChiamaEngineAsync(
            string urlEngine,
            string apiKey,
            string tenantName,
            string modelCode,
            FmisContextDataNutrizione fmisContextData,
            string payloadJson,
            CancellationToken cancellationToken)
        {
            // DS04-BL: POST 
            var requestUrl = $"{urlEngine.TrimEnd('/')}/{tenantName}/public/api/v1/models/{HttpUtility.UrlEncode(modelCode)}/executionrequest";

            using var requestMsg = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            requestMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestMsg.Headers.Add("fmis-context", UtilityAgronica.CreateFmisContextHeader(_config, fmisContextData));

            requestMsg.Content = new StringContent(payloadJson, System.Text.Encoding.UTF8, "application/json");

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
                throw new ConsiglioNutrizioneEngineTimeoutException(TimeoutEngineSecondi);
            }

            var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);

            if (!response.IsSuccessStatusCode)
                throw new ConsiglioNutrizioneEngineHttpException(response.StatusCode, responseBody);

            try
            {
                var result = JsonConvert.DeserializeObject<EngineConsiglioNutrizioneInitialResponse>(responseBody);
                if (result is null || string.IsNullOrWhiteSpace(result.RequestId))
                    throw new ConsiglioNutrizioneEngineParseException("La risposta non contiene RequestId.");

                return result;
            }
            catch (JsonException ex)
            {
                throw new ConsiglioNutrizioneEngineParseException("Il corpo della risposta non è JSON valido.", ex);
            }
        }

        // ── Step 7: Mappatura ─────────────────────────────────────────────────────

        private static AggregazioneConsiglioNutrizioneResult MappaPerSalvataggio(
            RichiestaConsiglioNutrizioneInput input,
            EsitoConsiglioNutrizione esitoEngine)
        {
            var messaggi = esitoEngine.Results?.Outcome?.Messaggi is { Count: > 0 } msgs
                ? string.Join("; ", msgs)
                : null;

            var elementi = esitoEngine.Results?.Outcome?.Consigli
                ?.Select(e => new ElementoNutrizioneAggregatoDto
                {
                    Elemento               = e.Elemento,
                    FabbisognoMinimo       = e.FabbisognoMinimo       ?? 0,
                    FabbisognoMassimo      = e.FabbisognoMassimo      ?? 0,
                    DoseConsigliataMinima  = e.DoseConsigliataMinima  ?? 0,
                    DoseConsigliataMassima = e.DoseConsigliataMassima ?? 0,
                    QuantitativoPresente   = e.QuantitativoGiaPresente    ?? 0,
                    QuantitativoMinimoResiduo  = e.QuantitativoMinimoResiduo  ?? 0,
                    QuantitativoMassimoResiduo = e.QuantitativoMassimoResiduo ?? 0,
                    Message = messaggi
                })
                .ToArray() ?? Array.Empty<ElementoNutrizioneAggregatoDto>();

            return new AggregazioneConsiglioNutrizioneResult
            {
                Appezzamento = new AppezzamentoNutrizioneDto
                {
                    Piva    = input.DatiImpianto.Piva,
                    SaCod   = input.DatiImpianto.SaCod,
                    Appezza = input.DatiImpianto.Appezza,
                    IdReg   = input.DatiImpianto.IdReg,
                    Varieta = string.Empty
                },
                ConsiglioNutrizione = new ConsiglioNutrizioneAggregatoDto
                {
                    DataConsiglio = input.DataConsiglio.ToString("O"),
                    Elementi      = elementi
                }
            };
        }

        private static RichiestaConsiglioNutrizioneResult MappaComeResult(
            EsitoConsiglioNutrizione response,
            int? consiglioId)
        {
            var elementi = response.Results?.Outcome?.Consigli
                ?.Select(e => new ElementoConsigliatoDto
                {
                    Elemento                   = e.Elemento,
                    FabbisognoMinimo           = e.FabbisognoMinimo           ?? 0,
                    FabbisognoMassimo          = e.FabbisognoMassimo          ?? 0,
                    DoseConsigliataMiniima     = e.DoseConsigliataMinima      ?? 0,
                    DoseConsigliataMassima     = e.DoseConsigliataMassima     ?? 0,
                    QuantitativoPresente       = e.QuantitativoGiaPresente    ?? 0,
                    QuantitativoMinimoResiduo  = e.QuantitativoMinimoResiduo  ?? 0,
                    QuantitativoMassimoResiduo = e.QuantitativoMassimoResiduo ?? 0
                })
                .ToArray() ?? Array.Empty<ElementoConsigliatoDto>();

            var messaggi = response.Results?.Outcome?.Messaggi is { Count: > 0 } msgs
                ? string.Join("; ", msgs)
                : null;

            return new RichiestaConsiglioNutrizioneResult
            {
                ConsiglioId         = consiglioId,
                Appezzamento        = string.Empty,
                ElementiConsigliati = elementi,
                Status              = response.Status,
                Messaggi            = messaggi
            };
        }
    }
}
