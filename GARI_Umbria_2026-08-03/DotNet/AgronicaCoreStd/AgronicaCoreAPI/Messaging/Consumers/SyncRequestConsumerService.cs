using AgronicaCoreAPI.Messaging.Contracts;
using AgronicaCoreAPI.Messaging.Handlers;
using AgronicaCoreAPI.Messaging.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Consumers
{
    /// <summary>
    /// BackgroundService che consuma gli 8 queue di sincronizzazione APP mobile.
    /// Implementa un loop di riconnessione esterno con exponential backoff.
    /// In caso di eccezione handler → BasicNack (requeue: false) → messaggio in dead-letter.
    /// </summary>
    public class SyncRequestConsumerService : BackgroundService
    {
        private static readonly string[] RoutingKeys = new[]
        {
            "sync.requests.attivita",
            "sync.requests.attivitaMiste",
            "sync.requests.rilievi",
            "sync.requests.visite",
            "sync.requests.documenti",
            "sync.requests.movimenti",
            "sync.requests.acquisto",
            "sync.requests.manutenzioni"
        };

        private const int MaxRetryAttempts = 10;
        private const int BaseDelayMs = 2000;
        private const int HealthCheckIntervalMs = 5000;
        private const int PrefetchCount = 10;
        private const int DeliveryLimit = 10;

        private readonly IServiceProvider _sp;
        private readonly RabbitMqOptions _options;
        private readonly ILogger<SyncRequestConsumerService> _logger;

        public SyncRequestConsumerService(
            IServiceProvider sp,
            IOptions<RabbitMqOptions> options,
            ILogger<SyncRequestConsumerService> logger)
        {
            _sp = sp;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ConnectAndConsumeAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    // Arresto normale — usciamo
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Consumer loop interrotto. Nuovo tentativo tra 5s.");
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }

        private async Task ConnectAndConsumeAsync(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _options.Hostname,
                Port = _options.Port,
                UserName = _options.Username,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                DispatchConsumersAsync = true
            };

            // Exponential backoff sulla connessione
            IConnection connection = null;
            for (int attempt = 0; attempt < MaxRetryAttempts; attempt++)
            {
                if (attempt > 0)
                {
                    var delay = Math.Min((int)Math.Pow(2, attempt) * BaseDelayMs, 60_000);
                    _logger.LogWarning("Tentativo connessione RabbitMQ {Attempt}/{Max}, attesa {Delay}ms", attempt + 1, MaxRetryAttempts, delay);
                    await Task.Delay(delay, ct);
                }

                try
                {
                    connection = factory.CreateConnection();
                    _logger.LogInformation("Consumer RabbitMQ connesso a {Host}:{Port}", _options.Hostname, _options.Port);
                    break;
                }
                catch (Exception ex) when (attempt < MaxRetryAttempts - 1)
                {
                    _logger.LogWarning(ex, "Connessione fallita, tentativo {Attempt}", attempt + 1);
                }
            }

            if (connection == null)
                throw new InvalidOperationException("Impossibile connettersi a RabbitMQ dopo il numero massimo di tentativi.");

            using (connection)
            {
                using var channel = connection.CreateModel();

                // Dichiara exchange principale
                channel.ExchangeDeclare(
                    exchange: _options.ExchangeRequests,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);

                // Dichiara DLX
                channel.ExchangeDeclare(
                    exchange: _options.DlxExchange,
                    type: ExchangeType.Direct,
                    durable: true,
                    autoDelete: false);

                // Configura ogni routing key → queue + DLQ
                foreach (var rk in RoutingKeys)
                {
                    // Argomenti queue principale con DLX
                    var queueArgs = new Dictionary<string, object>
                    {
                        { "x-queue-type",              "quorum"          },
                        { "x-delivery-limit",          (long)DeliveryLimit },
                        { "x-dead-letter-exchange",    _options.DlxExchange },
                        { "x-dead-letter-routing-key", rk + ".dead"      }
                    };

                    channel.QueueDeclare(rk, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
                    channel.QueueBind(rk, _options.ExchangeRequests, rk);

                    // Dead Letter Queue corrispondente
                    var dlqArgs = new Dictionary<string, object> { { "x-queue-type", "quorum" } };
                    var dlqName = rk + ".dead";
                    channel.QueueDeclare(dlqName, durable: true, exclusive: false, autoDelete: false, arguments: dlqArgs);
                    channel.QueueBind(dlqName, _options.DlxExchange, dlqName);
                }

                // QoS globale
                channel.BasicQos(prefetchSize: 0, prefetchCount: PrefetchCount, global: false);

                // Avvia consumer per ogni routing key
                foreach (var rk in RoutingKeys)
                {
                    var consumer = new AsyncEventingBasicConsumer(channel);
                    var capturedRk = rk;
                    consumer.Received += async (sender, ea) =>
                        await ProcessMessageAsync(channel, ea, capturedRk, ct);

                    channel.BasicConsume(queue: rk, autoAck: false, consumer: consumer);
                    _logger.LogInformation("Consumer attivo su queue: {Queue}", rk);
                }

                // Health check loop
                while (!ct.IsCancellationRequested && connection.IsOpen)
                {
                    await Task.Delay(HealthCheckIntervalMs, ct);
                }

                if (!connection.IsOpen)
                    throw new InvalidOperationException("Connessione RabbitMQ chiusa inaspettatamente.");
            }
        }

        private async Task ProcessMessageAsync(IModel channel, BasicDeliverEventArgs ea, string routingKey, CancellationToken ct)
        {
            SyncEnvelope envelope = null;
            string correlationId = null;
            var sw = Stopwatch.StartNew();

            try
            {
                var body = Encoding.UTF8.GetString(ea.Body.Span);
                var byteSize = ea.Body.Length;
                envelope = JsonConvert.DeserializeObject<SyncEnvelope>(body);
                correlationId = envelope?.CorrelationId;

                await HandleMessageAsync(routingKey, envelope, byteSize, ct);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
                _logger.LogInformation(
                    "ACK messaggio routingKey={RoutingKey} corrId={CorrelationId} elapsed={ElapsedMs}ms",
                    routingKey, correlationId, sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore handler {RoutingKey} corrId={CorrelationId}. Invio NACK.", routingKey, correlationId);

                try
                {
                    // Nack senza requeue: RabbitMQ gestirà il redelivery/dead-letter via x-delivery-limit
                    channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);

                    // Aggiorna status store in modo best-effort
                    if (correlationId != null)
                    {
                        using var scope = _sp.CreateScope();
                        var store = scope.ServiceProvider.GetRequiredService<IRequestStatusStore>();
                        await store.SetAsync(new SyncStatusRecord
                        {
                            CorrelationId = correlationId,
                            Status = SyncStatus.Failed,
                            ErrorMessage = ex.Message,
                            ErrorDetails = ex.ToString()
                        }, ct);
                    }
                }
                catch (Exception nackEx)
                {
                    _logger.LogError(nackEx, "Errore durante NACK corrId={CorrelationId}", correlationId);
                }
            }
        }

        private async Task HandleMessageAsync(string routingKey, SyncEnvelope envelope, int byteSize, CancellationToken ct)
        {
            using var scope = _sp.CreateScope();
            var statusStore = scope.ServiceProvider.GetRequiredService<IRequestStatusStore>();

            await statusStore.SetAsync(new SyncStatusRecord
            {
                CorrelationId = envelope.CorrelationId,
                Status = SyncStatus.InProgress
            }, ct);

            ISyncMessageHandler handler = routingKey switch
            {
                "sync.requests.attivita"      => scope.ServiceProvider.GetRequiredService<AttivitaMessageHandler>(),
                "sync.requests.attivitaMiste" => scope.ServiceProvider.GetRequiredService<AttivitaMisteMessageHandler>(),
                "sync.requests.rilievi"       => scope.ServiceProvider.GetRequiredService<RilieviMessageHandler>(),
                "sync.requests.visite"        => scope.ServiceProvider.GetRequiredService<VisiteMessageHandler>(),
                "sync.requests.documenti"     => scope.ServiceProvider.GetRequiredService<DocumentiMessageHandler>(),
                "sync.requests.movimenti"     => scope.ServiceProvider.GetRequiredService<MovimentiMessageHandler>(),
                "sync.requests.acquisto"      => scope.ServiceProvider.GetRequiredService<AcquistoMessageHandler>(),
                "sync.requests.manutenzioni"  => scope.ServiceProvider.GetRequiredService<ManutenzioniMessageHandler>(),
                _ => null
            };

            if (handler == null)
            {
                _logger.LogWarning("Nessun handler per routingKey={RoutingKey}", routingKey);
                return;
            }

            var sw = Stopwatch.StartNew();
            var result = await handler.HandleAsync(envelope);
            sw.Stop();

            var entityType = routingKey.Contains('.') ? routingKey.Split('.')[^1] : routingKey;
            _logger.LogInformation(
                "Messaggio elaborato: corrId={CorrelationId} entityType={EntityType} status={Status} byteSize={ByteSize} elapsed={ElapsedMs}ms",
                envelope.CorrelationId, entityType,
                result.Success ? "Succeeded" : "Failed",
                byteSize, sw.ElapsedMilliseconds);

            await statusStore.SetAsync(new SyncStatusRecord
            {
                CorrelationId = envelope.CorrelationId,
                Status = result.Success ? SyncStatus.Succeeded : SyncStatus.Failed,
                ResultJson = result.Success ? Newtonsoft.Json.JsonConvert.SerializeObject(result) : null,
                ErrorMessage = result.Success ? null : result.ErrorMessage,
                CompletedAt = DateTime.UtcNow
            }, ct);

            if (!result.Success)
                throw new InvalidOperationException($"Handler fallito corrId={envelope.CorrelationId}: {result.ErrorMessage}");
        }
    }

}
