using System.Data;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.Webhook.DAL.DataLayer.Models;
using AgronicaNetCore.Webhook.DAL.DataLayer.WebhookTestata;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione
{
    /// <summary>
    /// Implementazione del servizio di polling sul WebHook_Testata per il consiglio nutrizionale.
    /// Interroga periodicamente la tabella fino a raggiungimento di uno stato terminale o timeout.
    /// Riferimento: DS15-BL Polling WebHook Consiglio Nutrizione.
    /// </summary>
    public sealed class PollingWebHookConsiglioNutrizioneService : BaseService, IPollingWebHookConsiglioNutrizioneService
    {
        private const int TimeoutMinimoSecondi      = 1;
        private const int TimeoutMassimoSecondi     = 60;
        private const int IntervalloMinimoMs        = 100;
        private const int IntervalloMassimoMs       = 2000;

        private readonly IWebhookTestataDAL _dal;
        private readonly ILoggingService          _loggingService;

        public PollingWebHookConsiglioNutrizioneService(IServiceProvider provider) : base(provider)
        {
            _dal            = provider.GetRequiredService<IWebhookTestataDAL>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
        }

        /// <inheritdoc/>
        public async Task<PollingWebHookResult> EseguiPollingAsync(
            PollingWebHookInput input,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default)
        {
            // DS15-BL Phase 1: Validazione input
            if (string.IsNullOrWhiteSpace(input?.RequestId))
                throw new ArgumentException("RequestId obbligatorio.", nameof(input));

            if (input.TestataId <= 0)
                throw new ArgumentException("TestataId obbligatorio.", nameof(input.TestataId));

            if (input.TimeoutSecondi < TimeoutMinimoSecondi || input.TimeoutSecondi > TimeoutMassimoSecondi)
                throw new ArgumentOutOfRangeException(nameof(input.TimeoutSecondi),
                    $"Timeout deve essere tra {TimeoutMinimoSecondi} e {TimeoutMassimoSecondi} secondi.");

            if (input.IntervalloPollingMs < IntervalloMinimoMs || input.IntervalloPollingMs > IntervalloMassimoMs)
                throw new ArgumentOutOfRangeException(nameof(input.IntervalloPollingMs),
                    $"Intervallo polling deve essere tra {IntervalloMinimoMs} e {IntervalloMassimoMs}ms.");

            // DS15-BL Phase 2: Inizializzazione loop
            var endTime  = DateTime.UtcNow.AddSeconds(input.TimeoutSecondi);
            bool recordMaiTrovato = true;

            // DS15-BL Phase 3: Polling loop
            while (DateTime.UtcNow < endTime)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var dt = await _dal.GetWebHookTestataByRequestIdAsync(input.RequestId, input.TestataId, objParametriServer);

                if (dt is null)
                {
                    // Record non ancora creato: attendi e riprova
                    await Task.Delay(input.IntervalloPollingMs, CancellationToken.None);
                    continue;
                }

                recordMaiTrovato = false;

                var row      = dt.Rows[0];
                var status   = row["Status"]?.ToString()   ?? string.Empty;
                var requestId = row["Request_Id"]?.ToString() ?? string.Empty;
                var responseRaw = row["Response"] == DBNull.Value ? null : row["Response"]?.ToString();

                if (status == "DONE")
                {
                    var parsedResponse = DeserializzaResponse(responseRaw, objParametriServer);
                    return new PollingWebHookResult
                    {
                        Status      = "DONE",
                        ConsiglioId = requestId,
                        TestataId   = input.TestataId,
                        Response    = parsedResponse,
                        Esito       = "SUCCESS"
                    };
                }

                if (status == "ERROR")
                {
                    _loggingService.LogWarning(
                        $"Polling completato con Status=ERROR per RequestId={input.RequestId}.",
                        objParametriServer);

                    var parsedResponse = DeserializzaResponse(responseRaw, objParametriServer);
                    return new PollingWebHookResult
                    {
                        Status        = "ERROR",
                        ConsiglioId   = requestId,
                        TestataId     = input.TestataId,
                        Response      = parsedResponse,
                        Esito         = "ERROR",
                        EsitoDettagli = parsedResponse?.Results?.Outcome?.Messaggi is { Count: > 0 } msgs
                            ? string.Join("; ", msgs)
                            : null
                    };
                }

                if (status != "TODO")
                {
                    // DS15-BL Edge Case: stato sconosciuto
                    _loggingService.LogWarning(
                        $"Stato sconosciuto '{status}' per RequestId={input.RequestId}.",
                        objParametriServer);

                    return new PollingWebHookResult
                    {
                        Status        = "ERROR",
                        ConsiglioId   = requestId,
                        TestataId     = input.TestataId,
                        Response      = null,
                        Esito         = "ERROR",
                        EsitoDettagli = $"Stato sconosciuto: {status}"
                    };
                }

                // Status == TODO: ancora in elaborazione, attendi
                await Task.Delay(input.IntervalloPollingMs, CancellationToken.None);
            }

            // Timeout scaduto
            if (recordMaiTrovato)
            {
                _loggingService.LogWarning(
                    $"RequestId={input.RequestId} non trovato in WebHook_Testata entro il timeout.",
                    objParametriServer);

                return new PollingWebHookResult
                {
                    Status    = "NOT_FOUND",
                    TestataId = input.TestataId,
                    Esito     = "NOT_FOUND",
                    Messaggi  = "RequestId e/o TestataId non trovato nel sistema"
                };
            }

            _loggingService.LogWarning(
                $"Timeout polling ({input.TimeoutSecondi}s) per RequestId={input.RequestId}.",
                objParametriServer);

            return new PollingWebHookResult
            {
                Status    = "TIMEOUT",
                TestataId = input.TestataId,
                Esito     = "TIMEOUT",
                Messaggi  = $"Elaborazione non completata entro {input.TimeoutSecondi} secondi"
            };
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private EsitoConsiglioNutrizione? DeserializzaResponse(
            string? responseJson,
            AgronicaCoreParametriServer objParametriServer)
        {
            if (string.IsNullOrWhiteSpace(responseJson))
                return null;

            try
            {
                return JsonConvert.DeserializeObject<EsitoConsiglioNutrizione>(responseJson);
            }
            catch (JsonException ex)
            {
                // DS15-BL Edge Case: Response JSON invalido → restituisce null senza bloccare il flusso.
                _loggingService.LogWarning(
                    $"Impossibile deserializzare Response da WebHook_Testata: {ex.Message}",
                    objParametriServer, ex);
                return null;
            }
        }
    }
}
