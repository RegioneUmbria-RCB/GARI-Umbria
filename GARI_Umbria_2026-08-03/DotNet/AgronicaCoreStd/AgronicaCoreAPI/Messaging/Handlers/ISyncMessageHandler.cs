using AgronicaCoreAPI.Messaging.Contracts;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Handlers
{
    /// <summary>Handler per un singolo tipo di messaggio RabbitMQ.</summary>
    public interface ISyncMessageHandler
    {
        Task<SyncResult> HandleAsync(SyncEnvelope envelope);
    }
}
