using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class AttivitaMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<AttivitaMessageHandler> _logger;

        public AttivitaMessageHandler(ISincroService sincroService, ILogger<AttivitaMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var attivita = DeserializePayload<Attivita>(envelope, _logger);
            if (string.IsNullOrEmpty(attivita.guid)) attivita.guid = envelope.CorrelationId;
            return await _sincroService.ScriviAttivitaAsync(attivita, envelope.Auth);
        }
    }
}
