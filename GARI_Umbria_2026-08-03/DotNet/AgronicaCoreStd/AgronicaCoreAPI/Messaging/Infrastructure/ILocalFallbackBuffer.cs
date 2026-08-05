using AgronicaCoreAPI.Messaging.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>
    /// Buffer in-memory boundato usato come fallback quando RabbitMQ non è raggiungibile.
    /// I messaggi scritti qui vengono ripresi da <see cref="FallbackPublisherBackgroundService"/>.
    /// </summary>
    public interface ILocalFallbackBuffer
    {
        /// <summary>
        /// Tenta di scrivere un envelope nel buffer.
        /// Restituisce false se il buffer è pieno (dropped con log WARN nel chiamante).
        /// </summary>
        bool TryWrite(SyncEnvelope envelope);

        /// <summary>
        /// Legge il prossimo envelope in modo asincrono. Attende se il buffer è vuoto.
        /// </summary>
        ValueTask<SyncEnvelope> ReadAsync(CancellationToken ct);
    }
}
