using AgronicaCoreAPI.Messaging.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>Astrazione per la pubblicazione di messaggi su RabbitMQ.</summary>
    public interface IMessagePublisher
    {
        Task PublishAsync(SyncEnvelope envelope, string routingKey, CancellationToken ct = default);

        /// <summary>
        /// Restituisce true se la connessione RabbitMQ sottostante è aperta e operativa.
        /// Usato dal routing per decidere il fallback automatico a percorso diretto.
        /// </summary>
        bool IsHealthy();
    }
}
