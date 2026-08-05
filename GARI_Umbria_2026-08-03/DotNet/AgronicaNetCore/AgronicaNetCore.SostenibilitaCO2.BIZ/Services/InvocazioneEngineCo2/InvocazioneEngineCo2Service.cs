using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions;
using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Aziendale_Payload;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Chiavi;
using AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.Lookup_Sost_CO2_Colture_Payload;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.InvocazioneEngineCo2
{
    /// <summary>
    /// Implementazione della business logic di invocazione del motore esterno M4 (DS07-API).
    /// <para>
    /// Flusso: legge URL e Bearer token da <c>Configurazione_Siti</c>, persiste un record
    /// "in attesa" nelle tabelle di lookup, invia il payload via POST con timeout 30s, poi
    /// aggiorna il record con la risposta (successo o errore).
    /// </para>
    /// Riferimento spec: DS07-API InvocazioneEngineSOstenibilitàCO2.
    /// </summary>
    public class InvocazioneEngineCo2Service : BaseServiceSostenibilitaCO2Biz, IInvocazioneEngineCo2Service
    {
        // ----------------------------------------------------------------
        // Chiavi in Configurazione_Siti
        // ----------------------------------------------------------------

        /// <summary>Chiave per l'URL base del motore M4 in <c>Configurazione_Siti</c>.</summary>
        private const string ChiaveUrlSostenibilitaCo2 = "urlEngineSostenibilitaCo2";

        /// <summary>Chiave per il Bearer token JWT del motore M4 in <c>Configurazione_Siti</c>.</summary>
        private const string ChiaveBearerTokenSostenibilitaCo2 = "apikeyEngineSostenibilitaCo2";

        /// <summary>Path dell'endpoint M4 relativo all'URL base.</summary>
        private const string PathSostenibilitaCo2 = "/v1/calculate-emissions";

        private const string ModalitaPerColture = "Colture";

        private const int TimeoutSecondi = 30;

        private readonly ISecurityLayerDAL _securityLayerDAL;
        private readonly HttpClient _httpClient;
        private readonly ILookup_Sost_CO2_Aziendale_Chiavi _lookupAziendaleChiaviDAL;
        private readonly ILookup_Sost_CO2_Aziendale_Payload _lookupAziendalePayloadDAL;
        private readonly ILookup_Sost_CO2_Colture_Chiavi _lookupColtureChiaviDAL;
        private readonly ILookup_Sost_CO2_Colture_Payload _lookupColturePayloadDAL;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public InvocazioneEngineCo2Service(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDAL = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient = provider.GetRequiredService<HttpClient>();
            _lookupAziendaleChiaviDAL = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Chiavi>();
            _lookupAziendalePayloadDAL = provider.GetRequiredService<ILookup_Sost_CO2_Aziendale_Payload>();
            _lookupColtureChiaviDAL = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Chiavi>();
            _lookupColturePayloadDAL = provider.GetRequiredService<ILookup_Sost_CO2_Colture_Payload>();
        }

        /// <inheritdoc/>
        public async Task<RispostaEngineCo2> InvokeAsync(
            InvocazioneEngineCo2Input input,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            ArgumentNullException.ThrowIfNull(input);
            ArgumentNullException.ThrowIfNull(input.PayloadValidato);

            // ────────────────────────────────────────────────────────────
            // 1. Leggi configurazione da Configurazione_Siti
            // ────────────────────────────────────────────────────────────
            //var chiavi = new List<string> { ChiaveUrlM4, ChiaveBearerTokenM4 };
            var urlSostenibilitaCo2 = await _securityLayerDAL.LeggiConfigurazioneSitiScalareAsync(ChiaveUrlSostenibilitaCo2, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlSostenibilitaCo2))
                throw new InvalidOperationException($"Configurazione mancante: chiave '{ChiaveUrlSostenibilitaCo2}' non trovata in Configurazione_Siti.");

            var dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(ChiaveBearerTokenSostenibilitaCo2, objParametriServer);

            string bearerToken = string.Empty;
            
            if (dt.Rows.Count > 0)
                bearerToken = dt.Rows[0]["Valore"]?.ToString() ?? string.Empty;
         

            if (string.IsNullOrWhiteSpace(bearerToken))
                throw new InvalidOperationException($"Configurazione mancante: chiave '{ChiaveBearerTokenSostenibilitaCo2}' non trovata in Configurazione_Siti.");

            //var configMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //foreach (DataRow row in dtConfig.Rows)
            //    configMap[row["Chiave"]?.ToString() ?? string.Empty] = row["Valore"]?.ToString() ?? string.Empty;

            //if (!configMap.TryGetValue(ChiaveUrlM4, out var urlM4) || string.IsNullOrWhiteSpace(urlM4))
            //    throw new InvalidOperationException(
            //        $"Configurazione mancante: chiave '{ChiaveUrlM4}' non trovata in Configurazione_Siti.");

            //if (!configMap.TryGetValue(ChiaveBearerTokenM4, out var bearerToken) || string.IsNullOrWhiteSpace(bearerToken))
            //    throw new InvalidOperationException(
            //        $"Configurazione mancante: chiave '{ChiaveBearerTokenM4}' non trovata in Configurazione_Siti.");

            //string baseUrl = _securityLayerDAL..LeggiConfigurazioneSitiScalareAsync("LanToWebSiteBasePath",
            //     objParametriServer!, objParametriSuperServer).GetAwaiter().GetResult();

            // ────────────────────────────────────────────────────────────
            // 2. Serializza il payload e calcola hash SHA-256 per audit
            // ────────────────────────────────────────────────────────────
            var jsonRichiesta = JsonSerializer.Serialize(input.PayloadValidato);
            var payloadHash = ComputeSha256(jsonRichiesta);

            LogInformation(
                "InvocazioneMotoreSostenibilitaCo2 avviata — Filiera: {Filiera}, Anno: {Anno}, Aziende: {Count}, PayloadHash: {Hash}",
                objParametriServer, null, input.Filiera, input.Anno, input.PayloadValidato.aziende.Count, payloadHash);

            // ────────────────────────────────────────────────────────────
            // 3. Persisti un record "in attesa" per ogni azienda (Lookup_Chiavi)
            //    e un record payload (Lookup_Payload) — Inviato = 0
            // ────────────────────────────────────────────────────────────
            var idInvocazione = await PersistiRecordInAttesa(input, jsonRichiesta, objParametriServer);

            // ────────────────────────────────────────────────────────────
            // 4. Chiamata HTTP POST al motore M4 (timeout 30s)
            // ────────────────────────────────────────────────────────────
            string jsonRisposta;
            try
            {
                jsonRisposta = await SendToMotoreAsync(urlSostenibilitaCo2, bearerToken, jsonRichiesta);
            }
            catch (EngineCo2Exception ex)
            {
                LogError(
                    "InvocazioneEngineSostenibilitaCo2 fallita — Codice: {Codice}, HTTP: {Status}, Filiera: {Filiera}, Anno: {Anno}",
                    objParametriServer, ex, ex.Codice, ex.HttpStatusCode, input.Filiera, input.Anno);

                // Aggiorna record con errore (Inviato rimane 0)
                await AggiornaPersistenzaErrore(idInvocazione, ex.Message, input.Modalita, input.Username, objParametriServer);
                throw;
            }

            // ────────────────────────────────────────────────────────────
            // 5. Deserializza risposta
            // ────────────────────────────────────────────────────────────
            RispostaEngineCo2 risposta;
            try
            {
                risposta = JsonSerializer.Deserialize<RispostaEngineCo2>(jsonRisposta, JsonOptions)
                    ?? throw new InvalidOperationException("Risposta SostenibilitaCo2 deserializzata come null.");
            }
            catch (Exception ex)
            {
                LogError(
                    "InvocazioneEngineSostenibilitaCo2: risposta non deserializzabile — IdElaborazione non disponibile.",
                    objParametriServer, ex);
                throw new EngineCo2Exception(
                    CodiceErroreEngineCo2.UnexpectedResponse, 200,
                    "Risposta SostenibilitaCo2 ricevuta ma non deserializzabile come JSON atteso.", ex);
            }

            // ────────────────────────────────────────────────────────────
            // 6. Aggiorna record lookup con risposta (Inviato = 1)
            // ────────────────────────────────────────────────────────────
            await AggiornaPersistenzaSuccesso(idInvocazione, jsonRisposta, risposta, input, objParametriServer);

            LogInformation(
                "InvocazioneEngineSostenibilitaCo2 completata — IdElaborazione: {Id}, Filiera: {Filiera}, Anno: {Anno}",
                objParametriServer, null, risposta.IdElaborazione, input.Filiera, input.Anno);

            return risposta;
        }

        // ────────────────────────────────────────────────────────────────
        // HTTP
        // ────────────────────────────────────────────────────────────────

        private async Task<string> SendToMotoreAsync(string urlBase, string bearerToken, string jsonRichiesta)
        {
            var uri = new Uri(urlBase.TrimEnd('/') + PathSostenibilitaCo2);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", bearerToken);
            requestMessage.Content = new StringContent(jsonRichiesta, Encoding.UTF8, "application/json");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSecondi));

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(requestMessage, cts.Token);
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || cts.Token.IsCancellationRequested)
            {
                throw new EngineCo2Exception(
                    CodiceErroreEngineCo2.Timeout, 0,
                    $"Connessione al motore CO2 non riuscita (timeout >{TimeoutSecondi}s).", ex);
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return responseBody;

            var messaggioSostenibilitaCo2 = EstraiMessaggioErroreSostenibilitaCo2(responseBody);

            var codice = (int)response.StatusCode switch
            {
                400 => CodiceErroreEngineCo2.ValidationError,
                401 or 403 => CodiceErroreEngineCo2.AuthenticationError,
                429 => CodiceErroreEngineCo2.RateLimitExceeded,
                503 or 504 => CodiceErroreEngineCo2.Timeout,       // gateway timeout / service unavailable
                >= 500 => CodiceErroreEngineCo2.Co2ServerError,
                _ => CodiceErroreEngineCo2.UnexpectedResponse
            };

            throw new EngineCo2Exception(codice, (int)response.StatusCode, messaggioSostenibilitaCo2);
        }

        // ────────────────────────────────────────────────────────────────
        // Persistenza lookup
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Scrive i record "in attesa" (Inviato = 0) nelle tabelle di lookup corrette
        /// in base a <see cref="InvocazioneEngineCo2Input.Modalita"/>:
        /// <list type="bullet">
        ///   <item><c>Aziendale</c> → <c>Lookup_Sost_CO2_Aziendale_Chiavi</c> / <c>_Payload</c></item>
        ///   <item><c>Colture</c> → <c>Lookup_Sost_CO2_Colture_Chiavi</c> / <c>_Payload</c></item>
        /// </list>
        /// Restituisce l'<c>id_invocazione</c> generato.
        /// </summary>
        private Task<string> PersistiRecordInAttesa(
            InvocazioneEngineCo2Input input,
            string jsonRichiesta,
            AgronicaCoreParametriServer objParametriServer)
        {
            return input.Modalita.Equals(ModalitaPerColture, StringComparison.OrdinalIgnoreCase)
                ? PersistiInAttesaColtureAsync(input, jsonRichiesta, objParametriServer)
                : PersistiInAttesaAziendaleAsync(input, jsonRichiesta, objParametriServer);
        }

        private async Task<string> PersistiInAttesaAziendaleAsync(
            InvocazioneEngineCo2Input input,
            string jsonRichiesta,
            AgronicaCoreParametriServer objParametriServer)
        {
            var idInvocazione = Guid.NewGuid().ToString("N");
                
            foreach (var azienda in input.PayloadValidato.aziende)
            {
                var chiave = new WriteLookupSostCO2AziendaleChiavi
                {
                    Id = 0,
                    Id_Invocazione = idInvocazione,
                    Data_Invocazione = DateTime.Now,
                    Anno = input.Anno,
                    Filiera = input.Filiera,
                    Azienda = azienda.id_azienda,
                    Inviato = 0,
                    Username_Creazione = input.Username,
                    Username_Modifica = input.Username
                };
                idInvocazione = await _lookupAziendaleChiaviDAL.ScriviModificaAsync(chiave, objParametriServer);
            }

            var payloadRecord = new WriteLookupSostCO2AziendalePayload
            {
                Id_Invocazione = idInvocazione,
                Json_Richiesta = jsonRichiesta,
                Json_Risposta = null,
                Inviato = 0,
                Username_Creazione = input.Username,
                Username_Modifica = input.Username
            };
            await _lookupAziendalePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);

            return idInvocazione;
        }

        private async Task<string> PersistiInAttesaColtureAsync(
            InvocazioneEngineCo2Input input,
            string jsonRichiesta,
            AgronicaCoreParametriServer objParametriServer)
        {
            var idInvocazione = Guid.NewGuid().ToString("N");

            foreach (var azienda in input.PayloadValidato.aziende)
            {
                foreach (var appezzamento in azienda.appezzamenti)
                {
                    if (!TryParseUltimoSegmento(appezzamento.id_appezzamento, out int appezzamentoId))
                    {
                        LogWarning(
                            "PersistiInAttesaPerColture: id_appezzamento '{Id}' non parsabile come intero — riga omessa.",
                            objParametriServer, null, appezzamento.id_appezzamento);
                        continue;
                    }

                    foreach (var impianto in appezzamento.impianti)
                    {
                        if (!int.TryParse(impianto.id_coltura, out int vegCod))
                        {
                            LogWarning(
                                "PersistiInAttesaPerColture: id_coltura '{IdColtura}' non parsabile come intero — riga omessa.",
                                objParametriServer, null, impianto.id_coltura);
                            continue;
                        }

                        var chiave = new WriteLookupSostCO2ColtureChiavi
                        {
                            Id = 0,
                            Id_Invocazione = idInvocazione,
                            Data_Invocazione = DateTime.Now,
                            Anno = input.Anno,
                            Piva_Filiera = input.Filiera,
                            Piva_Azienda = azienda.id_azienda,
                            Appezzamento = appezzamentoId,
                            Nazione = impianto.Stato,
                            Regione = impianto.Regione,
                            Veg_Cod = vegCod,
                            Cul_Cod = impianto.Cul_Cod,
                            Elem_Cod = impianto.Elem_Cod,
                            Mat_Cod = impianto.Mat_Cod,
                            Lotto = impianto.Lotto,
                            Progetto_Cod = BuildProgettoCodJson(impianto.id_impianto),
                            Inviato = 0,
                            Username_Creazione = input.Username,
                            Username_Modifica = input.Username
                        };

                        await _lookupColtureChiaviDAL.ScriviModificaAsync(chiave, objParametriServer);
                    }
                }
            }

            var payloadRecord = new WriteLookupSostCO2ColturePayload
            {
                Id_Invocazione = idInvocazione,
                Json_Richiesta = jsonRichiesta,
                Json_Risposta = null,
                Inviato = 0,
                Username_Creazione = input.Username,
                Username_Modifica = input.Username
            };
            await _lookupColturePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);

            return idInvocazione;
        }

        /// <summary>Aggiorna il record payload con la risposta del motore (Inviato = 1).</summary>
        private async Task AggiornaPersistenzaSuccesso(
            string idInvocazione,
            string jsonRisposta,
            RispostaEngineCo2 risposta,
            InvocazioneEngineCo2Input input,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (input.Modalita.Equals(ModalitaPerColture, StringComparison.OrdinalIgnoreCase))
            {
                var payloadRecord = new WriteLookupSostCO2ColturePayload
                {
                    Id_Invocazione = idInvocazione,
                    Json_Risposta = jsonRisposta,
                    Inviato = 1,
                    DataInvio = DateTime.Now,
                    Username_Modifica = input.Username
                };
                await _lookupColturePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);
            }
            else
            {
                var payloadRecord = new WriteLookupSostCO2AziendalePayload
                {
                    Id_Invocazione = idInvocazione,
                    Json_Risposta = jsonRisposta,
                    Inviato = 1,
                    DataInvio = DateTime.Now,
                    Username_Modifica = input.Username
                };
                await _lookupAziendalePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);
            }
        }

        /// <summary>Aggiorna il record payload con il messaggio di errore (Inviato rimane 0).</summary>
        private async Task AggiornaPersistenzaErrore(
            string idInvocazione,
            string messaggioErrore,
            string modalita,
            string username,
            AgronicaCoreParametriServer objParametriServer)
        {
            try
            {
                if (modalita.Equals(ModalitaPerColture, StringComparison.OrdinalIgnoreCase))
                {
                    var payloadRecord = new WriteLookupSostCO2ColturePayload
                    {
                        Id_Invocazione = idInvocazione,
                        Json_Risposta = messaggioErrore,
                        Inviato = 0,
                        DataInvio = DateTime.Now,
                        Username_Modifica = username
                    };
                    await _lookupColturePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);
                }
                else
                {
                    var payloadRecord = new WriteLookupSostCO2AziendalePayload
                    {
                        Id_Invocazione = idInvocazione,
                        Json_Risposta = messaggioErrore,
                        Inviato = 0,
                        DataInvio = DateTime.Now,
                        Username_Modifica = username
                    };
                    await _lookupAziendalePayloadDAL.ScriviModificaAsync(payloadRecord, objParametriServer);
                }
            }
            catch (Exception ex)
            {
                // Non propagare — il fallimento della persistenza dell'errore è meno critico
                LogWarning("Impossibile aggiornare il record lookup con l'errore per id_invocazione {Id}.",
                           objParametriServer, ex, idInvocazione);
            }
        }

        // ────────────────────────────────────────────────────────────────
        // Helper
        // ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Estrae il campo <c>message</c> dal corpo di errore JSON restituito da M4.
        /// Se il corpo non è parseable, restituisce il testo grezzo.
        /// </summary>
        private static string? EstraiMessaggioErroreSostenibilitaCo2(string responseBody)
        {
            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            try
            {
                var node = JsonNode.Parse(responseBody);
                return node?["message"]?.GetValue<string>() ?? responseBody;
            }
            catch
            {
                return responseBody;
            }
        }

        private static string ComputeSha256(string input)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>
        /// Estrae il segmento finale da un identificativo composto con separatore '|'
        /// (es. <c>PIVA|Sa_Cod|Appezza</c> → <c>Appezza</c>) e lo converte in intero.
        /// </summary>
        private static bool TryParseUltimoSegmento(string id, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(id))
                return false;
            var span = id.AsSpan().TrimEnd();
            var sep = span.LastIndexOf('|');
            var segment = sep >= 0 ? span[(sep + 1)..] : span;
            return int.TryParse(segment, out value);
        }

        /// <summary>
        /// Serializza il codice progetto (ultimo segmento di <c>id_impianto</c>) come array JSON
        /// (es. <c>["E32"]</c>) per il campo <c>Progetto_Cod</c> di <c>Lookup_Sost_CO2_Colture_Chiavi</c>.
        /// </summary>
        private static int BuildProgettoCodJson(string idImpianto)
        {
            if (string.IsNullOrWhiteSpace(idImpianto))
                return 0;
            var span = idImpianto.AsSpan().TrimEnd();
            var sep = span.LastIndexOf('|');
            var cod = sep >= 0 ? span[(sep + 1)..].ToString() : span.ToString();
            return int.TryParse(cod, out var result) ? result : 0;
        }
    }
}
