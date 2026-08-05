using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class RilieviMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<RilieviMessageHandler> _logger;

        public RilieviMessageHandler(ISincroService sincroService, ILogger<RilieviMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var rilievo = DeserializePayload<Attivita>(envelope, _logger);
            if (string.IsNullOrEmpty(rilievo.guid)) rilievo.guid = envelope.CorrelationId;
            return await _sincroService.ScriviRilievoAsync(rilievo, envelope.Auth);
        }
    }
}
