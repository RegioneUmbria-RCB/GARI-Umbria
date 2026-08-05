using AgronicaCoreAnagrafeStdBIZ;
using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Services;
using AgronicaCoreModelloSTD;
using AgronicaCoreModelsSTD.attivita;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    public class DocumentiMessageHandler : SyncMessageHandlerBase, ISyncMessageHandler
    {
        private readonly ISincroService _sincroService;
        private readonly ILogger<DocumentiMessageHandler> _logger;

        public DocumentiMessageHandler(ISincroService sincroService, ILogger<DocumentiMessageHandler> logger)
        {
            _sincroService = sincroService;
            _logger = logger;
        }

        public async Task<SyncResult> HandleAsync(SyncEnvelope envelope)
        {
            var documento = DeserializePayload<DocumentoPerScarico>(envelope, _logger);
            if (string.IsNullOrEmpty(documento.guid)) documento.guid = envelope.CorrelationId;
            return await _sincroService.ScriviDocumentiAsync(documento, envelope.Auth);
        }
    }
}
