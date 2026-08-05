using AgronicaCoreAPI.Messaging.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.Messaging.Infrastructure
{
    /// <summary>
    /// Pubblica messaggi sull'exchange RabbitMQ "sync.requests".
    /// Registrato come singleton — la connessione e il channel sono creati on-demand (lazy initialization).
    /// Include retry Polly (3 tentativi, 1s di attesa) su PublishAsync.
    /// Se RabbitMQ non è raggiungibile, IsHealthy() ritorna false e il controller fallback al percorso diretto.
    /// </summary>
    public class RabbitMqPublisher : IMessagePublisher, IDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqPublisher> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly ResiliencePipeline _retryPipeline;
        private readonly object _initLock = new object();
        private bool _initializationAttempted = false;
        private bool _initializationSucceeded = false;

        public RabbitMqPublisher(IOptions<RabbitMqOptions> options, ILogger<RabbitMqPublisher> logger)
        {
            _options = options.Value;
            _logger = logger;

            _retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Constant,
                    OnRetry = args =>
                    {
                        _logger.LogWarning("RabbitMqPublisher retry {Attempt} dopo errore: {Error}",
                            args.AttemptNumber, args.Outcome.Exception?.Message);
                        return default;
                    }
                })
                .Build();

            _logger.LogInformation("RabbitMqPublisher istanziato. Connessione lazy: verrà tentata al primo utilizzo.");
        }

        private void EnsureInitialized()
        {
            if (_initializationSucceeded)
                return;

            lock (_initLock)
            {
                if (_initializationSucceeded)
                    return;

                if (_initializationAttempted)
                {
                    throw new InvalidOperationException(
                        "RabbitMQ non è disponibile. Il tentativo di connessione iniziale è fallito. " +
                        "Usare IsHealthy() per verificare la disponibilità prima di chiamare PublishAsync.");
                }

                _initializationAttempted = true;

                try
                {
                    var factory = new ConnectionFactory
                    {
                        HostName = _options.Hostname,
                        Port = _options.Port,
                        UserName = _options.Username,
                        Password = _options.Password,
                        VirtualHost = _options.VirtualHost,
                        AutomaticRecoveryEnabled = true,
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(10),
                        SocketReadTimeout = TimeSpan.FromSeconds(10),
                        SocketWriteTimeout = TimeSpan.FromSeconds(10)
                    };

                    _logger.LogInformation("Tentativo connessione a RabbitMQ: {Host}:{Port} VHost={VHost}",
                        _options.Hostname, _options.Port, _options.VirtualHost);

                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();

                    _channel.ExchangeDeclare(
                        exchange: _options.ExchangeRequests,
                        type: ExchangeType.Direct,
                        durable: true,
                        autoDelete: false);

                    _initializationSucceeded = true;
                    _logger.LogInformation("Connessione RabbitMQ stabilita con successo.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Impossibile connettersi a RabbitMQ. Host={Host}:{Port}. " +
                        "L'applicazione continuerà con il percorso di sincronizzazione diretto.",
                        _options.Hostname, _options.Port);
                    throw;
                }
            }
        }

        public Task PublishAsync(SyncEnvelope envelope, string routingKey, CancellationToken ct = default)
        {
            EnsureInitialized();

            _retryPipeline.Execute(() =>
            {
                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(envelope));
                var props = _channel.CreateBasicProperties();
                props.Persistent = true;
                props.MessageId = envelope.MessageId;
                props.CorrelationId = envelope.CorrelationId;

                _channel.BasicPublish(
                    exchange: _options.ExchangeRequests,
                    routingKey: routingKey,
                    mandatory: false,
                    basicProperties: props,
                    body: body);

                _logger.LogInformation(
                    "Messaggio accodato: corrId={CorrelationId} messageId={MessageId} type={Type} " +
                    "clienteId={ClienteId} byteSize={ByteSize} enqueuedAt={EnqueuedAt:O}",
                    envelope.CorrelationId,
                    envelope.MessageId,
                    envelope.Type,
                    envelope.Meta?.ClienteId,
                    body.Length,
                    envelope.Meta?.SentAt ?? DateTime.UtcNow);
            });
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool IsHealthy()
        {
            if (!_initializationAttempted)
            {
                try
                {
                    EnsureInitialized();
                }
                catch
                {
                    return false;
                }
            }

            return _initializationSucceeded && 
                   _connection?.IsOpen == true && 
                   _channel?.IsOpen == true;
        }

        public void Dispose()
        {
            try { _channel?.Close(); _channel?.Dispose(); } catch { }
            try { _connection?.Close(); _connection?.Dispose(); } catch { }
        }
    }
}
