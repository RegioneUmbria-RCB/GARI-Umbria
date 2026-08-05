using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class AttivitaMisteMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<AttivitaMisteMessageHandler> _logger;

        public AttivitaMisteMessageHandler(ISincroService sincroService, ILogger<AttivitaMisteMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var attivita = DeserializePayload<List<Attivita>>(envelope, _logger);
            foreach (var a in attivita) if (string.IsNullOrEmpty(a.guid)) a.guid = envelope.CorrelationId;
            return await _sincroService.ScriviAttivitaMisteAsync(attivita, envelope.Auth);
        }
    }
}
