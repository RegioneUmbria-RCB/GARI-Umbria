using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class AcquistoMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<AcquistoMessageHandler> _logger;

        public AcquistoMessageHandler(ISincroService sincroService, ILogger<AcquistoMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var acquisto = DeserializePayload<Acquisto>(envelope, _logger);
            if (string.IsNullOrEmpty(acquisto.guid)) acquisto.guid = envelope.CorrelationId;
            return await _sincroService.ScriviAcquistoAsync(acquisto, envelope.Auth);
        }
    }
}
