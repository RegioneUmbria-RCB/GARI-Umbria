using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class VisiteMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<VisiteMessageHandler> _logger;

        public VisiteMessageHandler(ISincroService sincroService, ILogger<VisiteMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var visita = DeserializePayload<Attivita>(envelope, _logger);
            if (string.IsNullOrEmpty(visita.guid)) visita.guid = envelope.CorrelationId;
            return await _sincroService.ScriviVisiteAsync(visita, envelope.Auth);
        }
    }
}
