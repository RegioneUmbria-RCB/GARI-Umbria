using AgronicaCoreAPI.Messaging.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Consumers
{
    /// <summary>
    /// BackgroundService che draina il <see cref="ILocalFallbackBuffer"/> e riprova
    /// la pubblicazione su RabbitMQ con backoff esponenziale (1s → 5s → 30s).
    /// Garantisce che i messaggi accettati con 202 durante un'interruzione temporanea
    /// di RabbitMQ vengano consegnati appena il broker torna disponibile.
    /// </summary>
    public sealed class FallbackPublisherBackgroundService : BackgroundService
    {
        private static readonly TimeSpan[] BackoffSteps =
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(30)
        };

        private readonly ILocalFallbackBuffer _buffer;
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<FallbackPublisherBackgroundService> _logger;

        public FallbackPublisherBackgroundService(
            ILocalFallbackBuffer buffer,
            IMessagePublisher publisher,
            ILogger<FallbackPublisherBackgroundService> logger)
        {
            _buffer = buffer;
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("FallbackPublisherBackgroundService avviato.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var envelope = await _buffer.ReadAsync(stoppingToken);

                await PublishWithRetryAsync(envelope, stoppingToken);
            }
        }

        private async Task PublishWithRetryAsync(
            Contracts.SyncEnvelope envelope,
            CancellationToken ct)
        {
            for (int attempt = 0; attempt <= BackoffSteps.Length; attempt++)
            {
                if (ct.IsCancellationRequested)
                    return;

                try
                {
                    await _publisher.PublishAsync(envelope, envelope.Type, ct);

                    _logger.LogInformation(
                        "FallbackPublisher: envelope consegnato dopo {Attempts} tentativo/i. " +
                        "corrId={CorrelationId} type={Type}",
                        attempt + 1, envelope.CorrelationId, envelope.Type);
                    return;
                }
                catch (Exception ex) when (attempt < BackoffSteps.Length)
                {
                    var delay = BackoffSteps[attempt];
                    _logger.LogWarning(ex,
                        "FallbackPublisher: tentativo {Attempt}/{Max} fallito per corrId={CorrelationId}. " +
                        "Prossimo retry tra {Delay}s.",
                        attempt + 1, BackoffSteps.Length + 1, envelope.CorrelationId, delay.TotalSeconds);

                    await Task.Delay(delay, ct);
                }
                catch (Exception ex)
                {
                    // Tentativi esauriti: log errore e abbandona questo messaggio.
                    _logger.LogError(ex,
                        "FallbackPublisher: tentativi esauriti per corrId={CorrelationId} type={Type}. " +
                        "Messaggio perso definitivamente.",
                        envelope.CorrelationId, envelope.Type);
                }
            }
        }
    }
}
