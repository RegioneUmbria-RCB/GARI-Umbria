using AgronicaCoreAPI.Messaging.Contracts;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>
    /// Implementazione in-memory bounded (capacità 1000) del buffer di fallback.
    /// Registrato come singleton.
    /// </summary>
    public sealed class InMemoryFallbackBuffer : ILocalFallbackBuffer
    {
        private const int Capacity = 1000;

        private readonly Channel<SyncEnvelope> _channel =
            Channel.CreateBounded<SyncEnvelope>(new BoundedChannelOptions(Capacity)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            });

        private readonly ILogger<InMemoryFallbackBuffer> _logger;

        public InMemoryFallbackBuffer(ILogger<InMemoryFallbackBuffer> logger) => _logger = logger;

        public bool TryWrite(SyncEnvelope envelope)
        {
            if (_channel.Writer.TryWrite(envelope))
                return true;

            _logger.LogWarning(
                "FallbackBuffer pieno (cap={Capacity}): envelope corrId={CorrelationId} scartato.",
                Capacity, envelope.CorrelationId);
            return false;
        }

        public ValueTask<SyncEnvelope> ReadAsync(CancellationToken ct) =>
            _channel.Reader.ReadAsync(ct);
    }
}
