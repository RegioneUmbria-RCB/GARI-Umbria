using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class ManutenzioniMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<ManutenzioniMessageHandler> _logger;

        public ManutenzioniMessageHandler(ISincroService sincroService, ILogger<ManutenzioniMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var manutenzioni = DeserializePayload<List<Manutenzione>>(envelope, _logger);
            foreach (var m in manutenzioni) if (string.IsNullOrEmpty(m.guid)) m.guid = envelope.CorrelationId;
            return await _sincroService.ScriviManutenzioniAsync(manutenzioni, envelope.Auth);
        }
    }
}
