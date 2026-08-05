using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class MovimentiMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<MovimentiMessageHandler> _logger;

        public MovimentiMessageHandler(ISincroService sincroService, ILogger<MovimentiMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var movimenti = DeserializePayload<List<MovimentoDiMagazzino>>(envelope, _logger);
            foreach (var m in movimenti) if (string.IsNullOrEmpty(m.guid)) m.guid = envelope.CorrelationId;
            return await _sincroService.ScriviMovimentiAsync(movimenti, envelope.Auth);
        }
    }
}
