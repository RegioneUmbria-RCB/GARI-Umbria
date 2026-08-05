using AgronicaCoreDTOStd.InData;
using AgronicaNetCore.Anagrafe.DAL.DataLayer.Impresa;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.DataLayer.Security;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Request;
using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Response;
using AgronicaNetCore.RischiMeteo.DAL.DataLayer.Lookup_Rischio_Meteo;
using InData.FoodMetaVerse;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.InvocazioneEngineRischiMeteo
{
    /// <summary>
    /// Implementa la business logic di invocazione del motore esterno M2 (Rischi Meteoclimatici).
    /// <para>
    /// Flusso: legge URL e Bearer token da <c>Configurazione_Siti</c>, persiste un record
    /// "in attesa" in <c>Lookup_Rischio_Meteo</c>, invia il payload via POST con timeout 30s, poi
    /// aggiorna il record con la risposta (successo) o segnala l'errore.
    /// </para>
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Flusso invocazione Engine M2.
    /// </summary>
    public class InvocazioneEngineRischiMeteoService : BaseServiceRischiMeteoBiz, IInvocazioneEngineRischiMeteoService
    {
        private const string ChiaveUrlRischiMeteo         = "urlEngineRischiMeteo";
        private const string ChiaveBearerTokenRischiMeteo = "apikeyEngineRischiMeteo";
        private const string PathRischiMeteo   = "/v1/rischi/valutazione";
        private const int TimeoutSecondi         = 60; // DS03-BL: timeout 60s

        private readonly ISecurityLayerDAL _securityLayerDAL;
        private readonly HttpClient _httpClient;
        private readonly ILookup_Rischio_Meteo _lookupRischioMeteoDAL;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public InvocazioneEngineRischiMeteoService(
            IServiceProvider provider,
            IStringLocalizer<Resources.Messages> localizer)
            : base(provider, localizer)
        {
            _securityLayerDAL     = provider.GetRequiredService<ISecurityLayerDAL>();
            _httpClient           = provider.GetRequiredService<HttpClient>();
            _lookupRischioMeteoDAL = provider.GetRequiredService<ILookup_Rischio_Meteo>();
        }

        /// <inheritdoc/>
        public async Task<IList<InvocazioneEngineRischiMeteoRisultato>> InvokeAsync(
            IList<InvocazioneEngineRischiMeteoInput> inputs,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer)
        {
            ArgumentNullException.ThrowIfNull(inputs);
            if (inputs.Count == 0)
                return Array.Empty<InvocazioneEngineRischiMeteoRisultato>();

            // ────────────────────────────────────────────────────────────
            // 1. Leggi configurazione una sola volta per tutta la lista
            // ────────────────────────────────────────────────────────────
            //var chiavi    = new List<string> { ChiaveUrlM2, ChiaveBearerTokenM2 };

            var urlRischiMeteo = await _securityLayerDAL.LeggiConfigurazioneSitiScalareAsync(ChiaveUrlRischiMeteo, objParametriServer, objParametriSuperServer);

            if (string.IsNullOrWhiteSpace(urlRischiMeteo))
                throw new InvalidOperationException($"Configurazione mancante: chiave '{ChiaveUrlRischiMeteo}' non trovata in Configurazione_Siti.");

            var dt = await _securityLayerDAL.LeggiConfigurazioneSitiAsync(ChiaveBearerTokenRischiMeteo, objParametriServer);

            string bearerToken = string.Empty;

            if (dt.Rows.Count > 0)
                bearerToken = dt.Rows[0]["Valore"]?.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(bearerToken))
                throw new InvalidOperationException($"Configurazione mancante: chiave '{ChiaveBearerTokenRischiMeteo}' non trovata in Configurazione_Siti.");

            //var configMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //foreach (DataRow row in dtConfig.Rows)
            //    configMap[row["Chiave"]?.ToString() ?? string.Empty] = row["Valore"]?.ToString() ?? string.Empty;

            //if (!configMap.TryGetValue(ChiaveUrlM2, out var urlM2) || string.IsNullOrWhiteSpace(urlM2))
            //    throw new InvalidOperationException(
            //        $"Configurazione mancante: chiave '{ChiaveUrlM2}' non trovata in Configurazione_Siti.");

            //if (!configMap.TryGetValue(ChiaveBearerTokenM2, out var bearerToken) || string.IsNullOrWhiteSpace(bearerToken))
            //    throw new InvalidOperationException(
            //        $"Configurazione mancante: chiave '{ChiaveBearerTokenM2}' non trovata in Configurazione_Siti.");

            // ────────────────────────────────────────────────────────────
            // 2. Serializza i payload e persisti record "in attesa" (Inviato = 0)
            // ────────────────────────────────────────────────────────────
            var prepItems = new List<(InvocazioneEngineRischiMeteoInput Input, string JsonRichiesta)>(inputs.Count);
            foreach (var input in inputs)
            {
                var jsonRichiesta = JsonSerializer.Serialize(input.Payload);

                LogInformation(
                    "InvocazioneEngineRischiMeteo avviata – Filiera: {Filiera}, Esercizio: {IdEsercizio}",
                    objParametriServer, null, input.IdFiliera, input.Esercizio.id_esercizio);

                var imprese = _serviceProvider.GetRequiredService<IImpresa>();
                input.Esercizio.cuaa_azienda = await imprese.CuaaFromPivaAsync(input.Esercizio.piva_azienda, objParametriServer);
                if (string.IsNullOrWhiteSpace(input.Esercizio.cuaa_azienda))
                    throw new Exceptions.DataNotFoundException($"CUAA non trovato per la PIVA azienda '{input.Esercizio.piva_azienda}'.");

                var dtoInAttesa = BuildLookupDto(input, jsonRichiesta, null, 0);
                
                dtoInAttesa.Id = await _lookupRischioMeteoDAL.RetrieveIdAsync(input.IdFiliera, input.Esercizio.cuaa_azienda, input.Esercizio.id_appezzamento, input.Esercizio.id_esercizio, objParametriServer);

                input.Id = await _lookupRischioMeteoDAL.ScriviModificaAsync(dtoInAttesa, objParametriServer);

                prepItems.Add((input, jsonRichiesta));
            }

            // ────────────────────────────────────────────────────────────
            // 3. Chiamate HTTP POST al motore M2 in batch paralleli (DS03-BL: 5 per volta)
            // ────────────────────────────────────────────────────────────
            const int batchSize = 5;
            var httpResults = new List<(InvocazioneEngineRischiMeteoInput Input, string JsonRichiesta, string? JsonRisposta, Exception? Error)>(inputs.Count);

            for (int i = 0; i < prepItems.Count; i += batchSize)
            {
                var batch = prepItems.GetRange(i, Math.Min(batchSize, prepItems.Count - i));

                var batchTasks = batch.Select(async item =>
                {
                    try
                    {
                        var jsonRisposta = await SendToMotoreAsync(urlRischiMeteo, bearerToken, item.JsonRichiesta);
                        return (item.Input, item.JsonRichiesta, JsonRisposta: jsonRisposta, Error: (Exception?)null);
                    }
                    catch (Exception ex)
                    {
                        LogError(
                            "InvocazioneEngineRischiMeteo fallita – Filiera: {Filiera}, Esercizio: {IdEsercizio}",
                            objParametriServer, ex, item.Input.IdFiliera, item.Input.Esercizio.id_esercizio);
                        return (item.Input, item.JsonRichiesta, JsonRisposta: (string?)null, Error: ex);
                    }
                });

                var batchRisultati = await Task.WhenAll(batchTasks);
                httpResults.AddRange(batchRisultati);
            }

            // ────────────────────────────────────────────────────────────
            // 4. Deserializza risposte, persisti lookup e costruisci output
            // ────────────────────────────────────────────────────────────
            var risultati = new List<InvocazioneEngineRischiMeteoRisultato>(inputs.Count);
            foreach (var (input, jsonRichiesta, jsonRisposta, error) in httpResults)
            {
                if (error is not null)
                {
                    var dtoErrore = BuildLookupDto(input, jsonRichiesta, error.Message, 0);
                    await _lookupRischioMeteoDAL.ScriviModificaAsync(dtoErrore, objParametriServer);
                    risultati.Add(new InvocazioneEngineRischiMeteoRisultato
                    {
                        Esercizio = input.Esercizio,
                        Avvisi    = input.Avvisi,
                        Successo  = false,
                        Errore    = error.Message
                    });
                    continue;
                }

                AssessRiskResponse risposta;
                try
                {
                    risposta = JsonSerializer.Deserialize<AssessRiskResponse>(jsonRisposta!, JsonOptions)
                        ?? throw new InvalidOperationException("Risposta RischiMeteo deserializzata come null.");
                }
                catch (Exception ex)
                {
                    LogError(
                        "InvocazioneEngineRischiMeteo: risposta non deserializzabile per l'Esercizio {IdEsercizio}.",
                        objParametriServer, ex, input.Esercizio.id_esercizio);
                    var dtoErrore = BuildLookupDto(input, jsonRichiesta, ex.Message, 0);
                    await _lookupRischioMeteoDAL.ScriviModificaAsync(dtoErrore, objParametriServer);
                    risultati.Add(new InvocazioneEngineRischiMeteoRisultato
                    {
                        Esercizio = input.Esercizio,
                        Avvisi    = input.Avvisi,
                        Successo  = false,
                        Errore    = ex.Message
                    });
                    continue;
                }

                var dtoSuccesso = BuildLookupDtoConRisposta(input, jsonRichiesta, jsonRisposta!, risposta);
                await _lookupRischioMeteoDAL.ScriviModificaAsync(dtoSuccesso, objParametriServer);

                LogInformation(
                    "InvocazioneEngineRischiMeteo completata – Filiera: {Filiera}, Esercizio: {IdEsercizio}",
                    objParametriServer, null, input.IdFiliera, input.Esercizio.id_esercizio);

                risultati.Add(new InvocazioneEngineRischiMeteoRisultato
                {
                    Esercizio = input.Esercizio,
                    Risposta  = risposta,
                    Avvisi    = input.Avvisi,
                    Successo  = true
                });
            }

            return risultati;
        }

        // ────────────────────────────────────────────────────────────────
        // HTTP
        // ────────────────────────────────────────────────────────────────

        private async Task<string> SendToMotoreAsync(string urlBase, string bearerToken, string jsonRichiesta)
        {
            var uri = new Uri(urlBase.TrimEnd('/') + PathRischiMeteo);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("APIKEY", bearerToken);
            requestMessage.Content = new StringContent(jsonRichiesta, Encoding.UTF8, "application/json");

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(TimeoutSecondi));

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(requestMessage, cts.Token);
            }
            catch (TaskCanceledException ex) when (cts.Token.IsCancellationRequested)
            {
                throw new InvalidOperationException(
                    $"Connessione all'Engine RischiMeteo non riuscita (timeout >{TimeoutSecondi}s).", ex);
            }
            catch (Exception ex)
            {
                // Timeout di rete, connessione rifiutata, DNS fallito, SSL error – DS03-BL: failure_network
                throw new InvalidOperationException("Connessione al RischiMeteo non riuscita.", ex);
            }

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            var responseBody = await response.Content.ReadAsStringAsync();
            var statusCode   = (int)response.StatusCode;

            // DS03-BL: mappatura HTTP → messaggi user-friendly
            throw statusCode switch
            {
                400 => new InvalidOperationException($"Payload non valido: {responseBody}"),
                401 => new UnauthorizedAccessException("Errore di autenticazione all'engine RischiMeteo."),
                403 => new UnauthorizedAccessException("Accesso negato all'engine RischiMeteo."),
                429 => new InvalidOperationException("Troppe richieste all'engine RischiMeteo. Riprova tra 1 minuto."),
                >= 500 => new InvalidOperationException($"Errore dell'engine RischiMeteo. HTTP {statusCode}: {responseBody}"),
                _ => new InvalidOperationException($"Engine RischiMeteo ha risposto con HTTP {statusCode}: {responseBody}")
            };
        }

        // ────────────────────────────────────────────────────────────────
        // Helpers costruzione DTO lookup
        // ────────────────────────────────────────────────────────────────

        private static WriteLookupRischioMeteo BuildLookupDto(
            InvocazioneEngineRischiMeteoInput input,
            string jsonRichiesta,
            string? jsonRispostaErrore,
            int inviato)
        {
            return new WriteLookupRischioMeteo
            {
                Id = input.Id, // Aggiorna il record esistente
                Cuaa_Filiera     = input.IdFiliera,
                Cuaa_Azienda     = input.Esercizio.cuaa_azienda,
                Id_Appezzamento  = input.Esercizio.id_appezzamento,
                Id_Esercizio     = input.Esercizio.id_esercizio,
                Anno_Esercizio   = DateTime.Now.Year,
                Cod_Specie       = input.Payload.coltura?.codice_specie ?? 0,
                Cod_Varieta      = input.Payload.coltura?.codice_cultivar,
                Nazione          = input.Esercizio.nazione,
                Regione          = input.Esercizio.istat_reg,
                Centroide_Wkt    = input.Payload.geo_impianto?.centroide_wkt,
                Poligono_Wkt     = input.Payload.geo_impianto?.poligono_wkt,
                Epsg             = input.Payload.geo_impianto?.epsg ?? "4326",
                Superficie_Ha    = input.Esercizio.superficie_ha,
                Json_Richiesta   = jsonRichiesta,
                Json_Risposta    = jsonRispostaErrore,
                Data_Invocazione = DateTime.Now,
                Inviato          = (short?)inviato,
                Username_Creazione = input.Username,
                Username_Modifica  = input.Username
            };
        }

        private static WriteLookupRischioMeteo BuildLookupDtoConRisposta(
            InvocazioneEngineRischiMeteoInput input,
            string jsonRichiesta,
            string jsonRisposta,
            AssessRiskResponse risposta)
        {
            return new WriteLookupRischioMeteo
            {
                Id               = input.Id, // Aggiorna il record esistente
                Cuaa_Filiera     = input.IdFiliera,
                Cuaa_Azienda     = input.Esercizio.cuaa_azienda,
                Id_Appezzamento  = input.Esercizio.id_appezzamento,
                Id_Esercizio     = input.Esercizio.id_esercizio,
                Anno_Esercizio   = DateTime.Now.Year,
                Cod_Specie       = input.Payload.coltura?.codice_specie ?? 0,
                Cod_Varieta      = input.Payload.coltura?.codice_cultivar ?? 0,
                Nazione          = input.Esercizio.nazione,
                Regione          = input.Esercizio.istat_reg,
                Centroide_Wkt    = input.Payload.geo_impianto?.centroide_wkt,
                Poligono_Wkt     = input.Payload.geo_impianto?.poligono_wkt,
                Epsg             = input.Payload.geo_impianto?.epsg ?? "4326",
                Superficie_Ha    = input.Esercizio.superficie_ha,
                Json_Richiesta   = jsonRichiesta,
                Json_Risposta    = jsonRisposta,
                Rischio_Gelo         = risposta.rischioGelo?.dannoPopolazionePctComb,
                Rischio_Siccita      = risposta.rischioSiccita?.dannoPopolazionePctComb,
                Rischio_Allagamento  = risposta.rischioAllagamento?.dannoPopolazionePctComb,
                Data_Invocazione     = DateTime.Now,
                Inviato              = (short?)1,
                DataInvio            = DateTime.Now,
                Username_Creazione   = input.Username,
                Username_Modifica    = input.Username
            };
        }
    }
}
