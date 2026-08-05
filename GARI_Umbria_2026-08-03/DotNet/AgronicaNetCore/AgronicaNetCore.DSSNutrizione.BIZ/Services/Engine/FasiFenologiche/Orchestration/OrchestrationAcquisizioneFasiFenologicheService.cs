using AgronicaCoreModelsSTD.Engine;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Persistenza;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models;
using InData.Engine.FasiFenologiche;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OutData.Engine;
using OutData.Engine.FasiFenologiche;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.FasiFenologiche.Orchestration
{
    /// <summary>
    /// Implementazione dell'orchestratore per l'acquisizione fasi fenologiche.
    /// Esegue sequenzialmente: validazione → configurazione → chiamata engine → persistenza.
    /// </summary>
    /// <remarks>
    /// Design Specification: OrchestrationAcquisizioneFasiFenologiche - Descrizione.
    /// </remarks>
    public sealed class OrchestrationAcquisizioneFasiFenologicheService: BaseService, IOrchestrationAcquisizioneFasiFenologicheService
    {
        private const int TimeoutGlobaleSecondi = 60;
        private const int TimeoutEngineSecondi = 30;
        private const string ChiaveUrlEngine = "urlEngine_FasiFenologiche";
        private const string ChiaveApiKey = "apiKeyEngine_FasiFenologiche";

        private static readonly HashSet<string> TipiRichiestaAmmessi =
            new(StringComparer.OrdinalIgnoreCase) { "attuale", "breve", "lungo_termine" };

        private readonly ISecurityLayerDAL _securityLayerDal;
        private readonly HttpClient _httpClient;
        private readonly IPersistenzaFasiFenologicheService _persistenza;
        private readonly ILoggingService _loggingService;

        public OrchestrationAcquisizioneFasiFenologicheService(IServiceProvider provider) : base(provider)
        {
            _securityLayerDal = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient       = provider.GetRequiredService<HttpClient>();
            _persistenza      = provider.GetRequiredService<IPersistenzaFasiFenologicheService>();
            _loggingService   = provider.GetRequiredService<ILoggingService>();
        }

        /// <inheritdoc />
        public async Task<AcquisizioneFasiFenologicheResponse> EseguiAsync(
            AcquisizioneFasiFenologicheRequest request,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (objParametriServer is null) throw new ArgumentNullException(nameof(objParametriServer));
            if (objParametriSuperServer is null) throw new ArgumentNullException(nameof(objParametriSuperServer));

            // Regola di Business 1: generazione Correlation ID univoco
            var correlationId = Guid.NewGuid().ToString();
            var timestampInizio = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            // Regola di Business 6: timeout globale 60 secondi
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutGlobaleSecondi));
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                // Step 1: Validazione parametri (DS01-BL)
                var erroriValidazione = ValidaParametri(request);
                if (erroriValidazione.Count > 0)
                {
                    var erroriDettaglio = string.Join("; ", erroriValidazione.Select(e => $"{e.Campo}: {e.Messaggio}"));
                    _loggingService.LogWarning(
                        $"Validazione parametri fallita. CorrelationId={correlationId} Errori={erroriDettaglio}.",
                        objParametriServer);

                    return EsitoNegativo(correlationId, "VALIDATION_ERROR", "Parametri non validi.", erroriDettaglio, stopwatch, timestampInizio);
                }

                // Step 2: Recupero configurazione engine (DS02-BL)
                var (urlEngine, apiKey) = await _securityLayerDal.RecuperaConfigurazioneEngineAsync(ChiaveUrlEngine,ChiaveApiKey,objParametriServer,objParametriSuperServer);

                // Step 3: Chiamata engine esterno (DS03-BL)
                var esitoEngine = await EseguiChiamataAsync(
                    request,
                    urlEngine,
                    apiKey,
                    TimeoutEngineSecondi,
                    linkedCts.Token);

                // Step 4: Persistenza fasi ricevute (DS04-BL)
                var persistenzaInput = new PersistenzaFasiFenologicheInput
                {
                    Impianto = request.Impianto,
                    FasiFenologiche = esitoEngine.FasiFenologiche,
                    MetadataAcquisizione = new MetadataAcquisizione
                    {
                        UsernameCreazione = usernameRichiedente,
                        TimestampAcquisizione = timestampInizio
                    }
                };

                var esitoPersistenza = await _persistenza.SalvaAsync(persistenzaInput, objParametriServer);

                stopwatch.Stop();

                return new AcquisizioneFasiFenologicheResponse
                {
                    Esito = true,
                    CorrelationId = correlationId,
                    FasiFenologiche = persistenzaInput.FasiFenologiche,
                    DurataTotaleMs = (int)stopwatch.ElapsedMilliseconds,
                    TimestampAcquisizione = timestampInizio
                };
            }
            catch (InvalidParameterException ex)
            {
                _loggingService.LogWarning($"Errore parametri. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "VALIDATION_ERROR", ex.Message, ex.ToString(), stopwatch, timestampInizio);
            }
            catch (ConfigurationNotFoundException ex)
            {
                _loggingService.LogError($"Configurazione non trovata. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "CONFIGURATION_ERROR", "Errore di configurazione interna.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (InvalidConfigurationException ex)
            {
                _loggingService.LogError($"Configurazione non valida. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "CONFIGURATION_ERROR", "Errore di configurazione interna.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (EngineTimeoutException ex)
            {
                _loggingService.LogWarning($"Timeout engine. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "ENGINE_TIMEOUT", "L'engine esterno non ha risposto in tempo. Riprovare.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (EngineHttpException ex)
            {
                _loggingService.LogWarning($"Errore HTTP engine. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "ENGINE_HTTP_ERROR", "L'engine esterno ha restituito un errore. Riprovare.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (BulkInsertException ex)
            {
                _loggingService.LogError($"Errore bulk insert persistenza. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "PERSISTENCE_ERROR", "Errore durante il salvataggio delle fasi.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (TransactionRollbackException ex)
            {
                _loggingService.LogError($"Rollback transazione persistenza. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "PERSISTENCE_ERROR", "Errore durante il salvataggio delle fasi.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (OperationCanceledException ex)
            {
                _loggingService.LogError($"Timeout globale orchestrazione. CorrelationId={correlationId}.", objParametriServer, ex);
                return EsitoNegativo(correlationId, "TIMEOUT", "L'acquisizione ha superato il timeout globale.", ex.ToString(), stopwatch, timestampInizio);
            }
            catch (Exception ex)
            {
                throw new OrchestrationException(correlationId, ex.Message, ex);
            }
        }

        private static AcquisizioneFasiFenologicheResponse EsitoNegativo(
            string correlationId,
            string codice,
            string messaggio,
            string dettaglioTecnico,
            Stopwatch stopwatch,
            DateTimeOffset timestampInizio)
        {
            stopwatch.Stop();
            return new AcquisizioneFasiFenologicheResponse
            {
                Esito = false,
                CorrelationId = correlationId,
                FasiFenologiche = Array.Empty<FaseFenologicaEngine>(),
                DurataTotaleMs = (int)stopwatch.ElapsedMilliseconds,
                TimestampAcquisizione = timestampInizio,
                Errore = new OrchestrationErrore
                {
                    Codice = codice,
                    Messaggio = messaggio,
                    DettaglioTecnico = dettaglioTecnico
                }
            };
        }

        /// <summary>
        /// Valida i parametri di input della richiesta fasi fenologiche.
        /// </summary>
        /// <returns>Lista di errori di validazione; vuota se la validazione è superata.</returns>
        /// <remarks>
        /// Design Specification: ValidazioneParametriRichiestaFasiFenologiche - Regole di Business.
        /// </remarks>
        private static IReadOnlyList<FasiFenologicheErroreValidazione> ValidaParametri(AcquisizioneFasiFenologicheRequest parameters)
        {
            var errori = new List<FasiFenologicheErroreValidazione>();

            // Regola 1: tipo_richiesta obbligatorio; valori ammessi: "attuale", "breve", "lungo_termine"
            if (string.IsNullOrWhiteSpace(parameters.TipoRichiesta))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "tipo_richiesta", Messaggio = "Il campo è obbligatorio." });
            else if (!TipiRichiestaAmmessi.Contains(parameters.TipoRichiesta))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "tipo_richiesta", Messaggio = "Valore non ammesso. Valori ammessi: 'attuale', 'breve', 'lungo_termine'." });

            // Regola 2: tipo_codice_coltura default=1; ammessi: 1, 2, 3
            var tipoCodiceColtura = parameters.TipoCodiceColtura ?? 1;
            if (tipoCodiceColtura is not (1 or 2 or 3))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "tipo_codice_coltura", Messaggio = "Valore non ammesso. Valori ammessi: 1 (Profitosan/GIAS), 2 (AGEA), 3 (EPPO)." });

            // Regola 3: colturaId obbligatorio (DS07: nessun limite di lunghezza specificato)
            if (string.IsNullOrWhiteSpace(parameters.ColturaId))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "colturaId", Messaggio = "Il campo è obbligatorio." });

            // Regola 3b: varietaId obbligatorio (DS07: nessun limite di lunghezza specificato)
            if (string.IsNullOrWhiteSpace(parameters.VarietaId))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "varietaId", Messaggio = "Il campo è obbligatorio." });

            // Regola 4: latitudine obbligatoria, range -90.0 a 90.0
            if (parameters.Latitudine < -90.0m || parameters.Latitudine > 90.0m)
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "latitudine", Messaggio = "Il valore deve essere compreso tra -90.0 e 90.0." });

            // Regola 4: longitudine obbligatoria, range -180.0 a 180.0
            if (parameters.Longitudine < -180.0m || parameters.Longitudine > 180.0m)
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "longitudine", Messaggio = "Il valore deve essere compreso tra -180.0 e 180.0." });

            // Regola 6: data_richiesta obbligatoria, formato ISO 8601 valido, non nel futuro
            DateTimeOffset? dataRichiesta = null;
            if (string.IsNullOrWhiteSpace(parameters.DataRichiesta))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "data_richiesta", Messaggio = "Il campo è obbligatorio." });
            else if (!DateTimeOffset.TryParse(parameters.DataRichiesta, null, DateTimeStyles.RoundtripKind, out var drParsed))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "data_richiesta", Messaggio = "Formato non valido. Utilizzare il formato ISO 8601." });
            else if (drParsed > DateTimeOffset.UtcNow)
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "data_richiesta", Messaggio = "La data non può essere nel futuro." });
            else
                dataRichiesta = drParsed;

            // Regola 5: data_semina opzionale; se presente, deve essere precedente o uguale a data_richiesta
            if (!string.IsNullOrWhiteSpace(parameters.DataSemina))
            {
                if (!DateTimeOffset.TryParse(parameters.DataSemina, null, DateTimeStyles.RoundtripKind, out var dsParsed))
                    errori.Add(new FasiFenologicheErroreValidazione { Campo = "data_semina", Messaggio = "Formato non valido. Utilizzare il formato ISO 8601." });
                else if (dataRichiesta.HasValue && dsParsed > dataRichiesta.Value)
                    errori.Add(new FasiFenologicheErroreValidazione { Campo = "data_semina", Messaggio = "La data di semina deve essere precedente o uguale alla data di richiesta." });
            }

            // Regola 7: codice_lingua obbligatorio, codice ISO 639-1 valido
            if (string.IsNullOrWhiteSpace(parameters.CodiceLingua))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "codice_lingua", Messaggio = "Il campo è obbligatorio." });
            else if (!IsValidIso6391LanguageCode(parameters.CodiceLingua))
                errori.Add(new FasiFenologicheErroreValidazione { Campo = "codice_lingua", Messaggio = "Il codice lingua non corrisponde a un codice ISO 639-1 valido." });

            return errori;
        }

        private static bool IsValidIso6391LanguageCode(string codeLingua)
        {
            if (codeLingua.Length != 2) return false;
            try
            {
                var culture = CultureInfo.GetCultureInfo(codeLingua);
                return culture.IsNeutralCulture;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }

        // ── Chiamata engine HTTP ──────────────────────────────────────────────

        /// <summary>
        /// Esegue la chiamata HTTP POST all'engine esterno Fasi Fenologiche.
        /// </summary>
        /// <remarks>
        /// Design Specification: ChiamataEngineFasiFenologiche - Descrizione e Regole di Business.
        /// </remarks>
        private async Task<EngineRisposta> EseguiChiamataAsync(
            AcquisizioneFasiFenologicheRequest parametri,
            string urlEngine,
            string apiKey,
            int timeoutSecondi = 30,
            CancellationToken cancellationToken = default)
        {
            if (parametri is null) throw new ArgumentNullException(nameof(parametri));
            if (string.IsNullOrWhiteSpace(urlEngine)) throw new ArgumentNullException(nameof(urlEngine));
            if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentNullException(nameof(apiKey));

            var requestUrl = $"{urlEngine.TrimEnd('/')}/api/v1/scenario/calcola";

            DateTimeOffset timestampChiamata = DateTimeOffset.UtcNow;
            Stopwatch stopwatch = Stopwatch.StartNew();

            string requestJson = BuildRequestPayload(parametri);

            using HttpRequestMessage request = new(HttpMethod.Post, requestUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            using CancellationTokenSource timeoutCts = new(TimeSpan.FromSeconds(timeoutSecondi));
            using CancellationTokenSource linkedCts =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, linkedCts.Token);
            }
            catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
            {
                stopwatch.Stop();
                _loggingService.LogError(
                    $"Timeout ({timeoutSecondi}s) raggiunto durante la chiamata all'engine Fasi Fenologiche: {urlEngine}");
                throw new EngineTimeoutException(timeoutSecondi);
            }

            stopwatch.Stop();
            string responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
            int httpStatusCode = (int)response.StatusCode;

            EngineRispostaMetadata metadata = new()
            {
                TimestampChiamata = timestampChiamata,
                DurataMs = stopwatch.ElapsedMilliseconds,
                HttpStatusCode = httpStatusCode
            };

            if (!response.IsSuccessStatusCode)
            {
                _loggingService.LogError(
                    $"L'engine Fasi Fenologiche ha restituito HTTP {httpStatusCode}. Body: {responseBody}");
                throw new EngineHttpException(response.StatusCode, responseBody);
            }

            // HTTP 200 — parse JSON response
            IReadOnlyList<FaseFenologicaEngine> fasi = ParseFasi(responseBody);

            if (fasi.Count == 0)
            {
                _loggingService.LogWarning(
                    $"L'engine ha restituito 0 fasi fenologiche per colturaId='{parametri.ColturaId}' varietaId='{parametri.VarietaId}'.");
                throw new EngineEmptyResponseException(parametri.ColturaId ?? string.Empty, parametri.VarietaId ?? string.Empty);
            }

            _loggingService.LogInformation(
                $"Chiamata engine Fasi Fenologiche completata: {fasi.Count} fasi in {metadata.DurataMs}ms.");

            return new EngineRisposta
            {
                EsitoChiamata = true,
                FasiFenologiche = fasi,
                MetadataRisposta = metadata
            };
        }

        private static string BuildRequestPayload(AcquisizioneFasiFenologicheRequest p)
        {
            object localizzazione = new { lat = p.Latitudine, lon = p.Longitudine };

            DateTimeOffset.TryParse(p.DataRichiesta, null, DateTimeStyles.RoundtripKind, out var dataRichiesta);

            DateTimeOffset? dataSemina = null;
            if (!string.IsNullOrWhiteSpace(p.DataSemina)
                && DateTimeOffset.TryParse(p.DataSemina, null, DateTimeStyles.RoundtripKind, out var ds))
                dataSemina = ds;

            var payload = new
            {
                tipoRichiesta = p.TipoRichiesta,
                colturaId = p.ColturaId,
                varietaId = p.VarietaId,
                localizzazione,
                @override = (object?)null,
                dataSemina = dataSemina?.ToString("yyyy-MM-dd"),
                dataRichiesta = dataRichiesta.ToString("yyyy-MM-dd"),
            };

            return JsonConvert.SerializeObject(payload, Formatting.None);
        }

        private static IReadOnlyList<FaseFenologicaEngine> ParseFasi(string responseBody)
        {
            JObject root;
            try
            {
                root = JObject.Parse(responseBody);
            }
            catch (JsonException ex)
            {
                throw new EngineResponseParseException("Il corpo della risposta non e' un JSON valido.", ex);
            }

            JArray? fasiArray = root["fasiBbch"] as JArray;
            if (fasiArray is null)
            {
                throw new EngineResponseParseException("Campo 'fasi_fenologiche' mancante nella risposta dell'engine.");
            }

            List<FaseFenologicaEngine> result = new(fasiArray.Count);
            foreach (JToken item in fasiArray)
            {
                string? bbchCode = item["bbchCode"]?.Value<string>();
                string? faseDescrizione = item["faseDescrizione"]?.Value<string>();
                string? dataRaggiungimento = item["dataRaggiungimento"]?.Value<string>();
                bool? isForecast = item["isForecast"]?.Value<bool>();
                bool? isOverride = item["isOverride"]?.Value<bool>();

                if (string.IsNullOrEmpty(bbchCode) || string.IsNullOrEmpty(faseDescrizione)
                    || string.IsNullOrEmpty(dataRaggiungimento) || isForecast is null || isOverride is null)
                {
                    throw new EngineResponseParseException(
                        "Una fase fenologica nella risposta manca di campi obbligatori (codice_bbch, descrizione_fase, data_stimata_raggiungimento, is_forecast).");
                }

                if (!DateTime.TryParse(dataRaggiungimento, out DateTime dataStimata))
                {
                    throw new EngineResponseParseException(
                        $"Il campo 'data_stimata_raggiungimento' contiene un valore non valido: '{dataRaggiungimento}'.");
                }

                result.Add(new FaseFenologicaEngine
                {
                    CodiceBbch = bbchCode,
                    DescrizioneFase = faseDescrizione,
                    DataStimataRaggiungimento = dataStimata,
                    IsForecast = isForecast.Value,
                    IsOverride = isOverride.Value
                });
            }

            return result;
        }
    }
}
