using AgronicaCoreAPI.Messaging.Consumers;
using AgronicaCoreAPI.Messaging.Handlers;
using AgronicaCoreAPI.Messaging.Infrastructure;
using AgronicaCoreAPI.Messaging.Routing;
using AgronicaCoreAPI.Messaging.Validation;
using AgronicaCoreAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AgronicaCoreAPI.Messaging
{
    public static class MessagingExtensions
    {
        /// <summary>
        /// Registra il layer di messaggistica RabbitMQ nel container DI.
        /// Se RabbitMQ:Enabled = false non registra nulla e ritorna subito.
        /// </summary>
        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            // Routing per-cliente × per-tipologia — registrato sempre (indipendente da RabbitMQ).
            services.Configure<RoutingConfigOptions>(configuration.GetSection(RoutingConfigOptions.SectionName));
            services.AddSingleton<IRoutingConfigService, RoutingConfigService>();

            // Validazione payload — registrata sempre (usata sia dal percorso async che come guard).
            services.AddSingleton<ISyncPayloadValidator, SyncPayloadValidator>();

            var rabbitmqEnabled = configuration.GetValue<bool>("RabbitMQ:Enabled", false);
            if (!rabbitmqEnabled)
                return services;

            // Impedisce che un'eccezione nel BackgroundService fermi l'intero host
            services.Configure<HostOptions>(options =>
            {
                options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });

            // Opzioni RabbitMQ
            services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

            // Status Store
            var storeType = configuration.GetValue<string>("SyncStore:Type") ?? "None";
            if (storeType.Equals("Redis", System.StringComparison.OrdinalIgnoreCase))
            {
                // Configura StackExchange.Redis come IDistributedCache
                var redisConnectionString = configuration.GetValue<string>("SyncStore:Redis:ConnectionString") ?? "localhost:6379";
                var instanceName = configuration.GetValue<string>("SyncStore:Redis:InstanceName") ?? "SyncStatus:";
                services.AddStackExchangeRedisCache(opt =>
                {
                    opt.Configuration = redisConnectionString;
                    opt.InstanceName = instanceName;
                });
                services.AddSingleton<IRequestStatusStore, RedisRequestStatusStore>();
            }
            else
            {
                services.AddSingleton<IRequestStatusStore, NoOpRequestStatusStore>();
            }

            // Publisher (singleton — channel condiviso)
            services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

            // FallbackBuffer + background service drain (singleton — channel boundato in-memory)
            services.AddSingleton<ILocalFallbackBuffer, InMemoryFallbackBuffer>();
            services.AddHostedService<FallbackPublisherBackgroundService>();

            // SincroService (scoped — usa IHttpClientFactory)
            services.AddScoped<ISincroService, SincroService>();

            // Handlers (scoped — risolti per ogni messaggio)
            services.AddScoped<AttivitaMessageHandler>();
            services.AddScoped<AttivitaMisteMessageHandler>();
            services.AddScoped<RilieviMessageHandler>();
            services.AddScoped<VisiteMessageHandler>();
            services.AddScoped<DocumentiMessageHandler>();
            services.AddScoped<MovimentiMessageHandler>();
            services.AddScoped<AcquistoMessageHandler>();
            services.AddScoped<ManutenzioniMessageHandler>();

            // Consumer BackgroundService
            services.AddHostedService<SyncRequestConsumerService>();

            return services;
        }
    }
}
