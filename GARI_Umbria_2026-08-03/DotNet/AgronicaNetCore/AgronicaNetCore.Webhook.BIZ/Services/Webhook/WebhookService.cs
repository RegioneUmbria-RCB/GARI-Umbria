using System.Text.Json;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.UtilityDB.DAL.DataLayer.Agro_Sequenze;
using AgronicaNetCore.Webhook.DAL.DataLayer.Models;
using AgronicaNetCore.Webhook.DAL.DataLayer.WebhookTestata;
using CloudNative.CloudEvents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AgronicaNetCore.Webhook.BIZ.Services.Webhook
{
    public class WebhookService : BaseService, IWebhookService
    {

        private readonly IWebhookTestataDAL _dal;
        private readonly ILoggingService _loggingService;
        private readonly IAgro_Sequence _seq;

        public WebhookService(IServiceProvider provider) : base(provider)
        {
            _dal = provider.GetRequiredService<IWebhookTestataDAL>();
            _loggingService = provider.GetRequiredService<ILoggingService>();
            _seq = provider.GetRequiredService<IAgro_Sequence>();
        }

        public async Task<bool> ProcessWebhookAsync(int idTestata, CloudEvent cloudEvent, AgronicaCoreParametriServer objParametriServer)
        {
            var cloudEventType = cloudEvent.Type;

            if (string.IsNullOrWhiteSpace(cloudEventType))
            {
                _loggingService.LogWarning("CloudEvent ricevuto senza type",objParametriServer);
                throw new ArgumentException("CloudEvent type mancante");
            }

            if (!WebhookRouting.TypeMapping.TryGetValue(cloudEventType, out var webhookTipo))
            {
                _loggingService.LogWarning($"CloudEvent type non mappato: {cloudEventType}", objParametriServer);
                throw new ArgumentException($"CloudEvent type '{cloudEventType}' non riconosciuto");
            }

            if (WebhookRouting.DataTypeMapping.TryGetValue(webhookTipo, out var expectedType)
                && cloudEvent.Data is JsonElement jsonElement)
            {
                try
                {
                    JsonSerializer.Deserialize(jsonElement, expectedType);
                }
                catch (JsonException ex)
                {
                    _loggingService.LogWarning($"CloudEvent data non deserializzabile come {expectedType.Name}",objParametriServer,ex);
                    throw new ArgumentException($"CloudEvent data non valido per il tipo '{expectedType.Name}'");
                }
            }

            var handlerName = WebhookRouting.HandlerMapping.GetValueOrDefault(webhookTipo, WebhookRouting.GenericSaveHandler);

            return handlerName switch
            {
                WebhookRouting.GenericSaveHandler => await GenericSaveAsync(idTestata, cloudEvent, objParametriServer),
                _ => throw new NotImplementedException($"Handler '{handlerName}' non implementato")
            };
        }

        private async Task<bool> GenericSaveAsync(int idTestata, CloudEvent cloudEvent, AgronicaCoreParametriServer objParametriServer)
        {
            string status  = string.Empty;

            string responseJson = string.Empty;

            if (cloudEvent.Data is JsonElement jsonData)
            {
                responseJson = JsonSerializer.Serialize(jsonData);

                if (jsonData.TryGetProperty("status", out var statusProperty))
                {
                    status = statusProperty.GetString() ?? string.Empty;
                }
            }

            var rowsUpdated = await _dal.UpdateTestataAsync(idTestata, status, responseJson, objParametriServer);
            if (!rowsUpdated)
            {
                _loggingService.LogWarning($"Nessuna riga trovata su Webhook_Testata per Id_Testata: {idTestata}", objParametriServer);
            }

            return rowsUpdated;
        }

        /// <summary>
        /// Metodo di test per creare una riga su Webhook_Testata. Da rimuovere dopo i test iniziali.
        /// </summary>
        public async Task<bool> CreateTestRowAsync(short tipo, string requestId, AgronicaCoreParametriServer objParametriServer)
        {
            var idTestata = await _seq.NuovoId_TabellaAsync("WebHook_Testata", 0, 2_000_000_000, objParametriServer);
            return await _dal.InsertTestataAsync(idTestata,tipo, requestId,0, string.Empty, objParametriServer);
        }
    }
}
