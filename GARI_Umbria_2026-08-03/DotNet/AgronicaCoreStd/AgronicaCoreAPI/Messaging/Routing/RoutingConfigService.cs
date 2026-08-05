using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace AgronicaCoreAPI.Messaging.Routing
{
    /// <summary>
    /// Risolve il percorso di sincronizzazione (direct / async) basandosi su regole
    /// per-cliente × per-tipologia caricate da <see cref="RoutingConfigOptions"/>.
    ///
    /// Le regole vengono ricaricate automaticamente ogni volta che la configurazione
    /// cambia (IOptionsMonitor), garantendo propagazione multi-istanza entro il
    /// SLA definito da <see cref="RoutingConfigOptions.RefreshIntervalSeconds"/>.
    /// La prima regola che corrisponde vince (first-match semantics).
    /// </summary>
    public sealed class RoutingConfigService : IRoutingConfigService, IDisposable
    {
        private readonly ILogger<RoutingConfigService> _logger;
        private volatile IReadOnlyList<RoutingRule> _rules;
        private readonly IDisposable _changeToken;

        public RoutingConfigService(
            IOptionsMonitor<RoutingConfigOptions> optionsMonitor,
            ILogger<RoutingConfigService> logger)
        {
            _logger = logger;
            _rules = optionsMonitor.CurrentValue.Rules ?? new List<RoutingRule>();

            _changeToken = optionsMonitor.OnChange(opts =>
            {
                _rules = opts.Rules ?? new List<RoutingRule>();
                _logger.LogInformation(
                    "SyncRouting: configurazione aggiornata — {Count} regole caricate.",
                    _rules.Count);
            });
        }

        /// <inheritdoc />
        public SyncRoute ResolveRoute(string clienteId, string tipoEntita)
        {
            var rules = _rules;

            foreach (var rule in rules)
            {
                bool clientMatch = rule.ClienteId == "*"
                    || string.Equals(rule.ClienteId, clienteId, StringComparison.OrdinalIgnoreCase);

                bool entityMatch = rule.TipoEntita == "*"
                    || string.Equals(rule.TipoEntita, tipoEntita, StringComparison.OrdinalIgnoreCase);

                if (!clientMatch || !entityMatch)
                    continue;

                var route = ParseRoute(rule.Percorso);

                _logger.LogInformation(
                    "SyncRouting: clienteId={ClienteId} tipoEntita={TipoEntita} → {Route} " +
                    "(regola: clienteId={RuleClient} tipoEntita={RuleEntity})",
                    clienteId, tipoEntita, route, rule.ClienteId, rule.TipoEntita);

                return route;
            }

            // Nessuna regola esplicita trovata: default async.
            _logger.LogInformation(
                "SyncRouting: nessuna regola per clienteId={ClienteId} tipoEntita={TipoEntita} — default async.",
                clienteId, tipoEntita);

            return SyncRoute.Async;
        }

        private static SyncRoute ParseRoute(string percorso)
        {
            if (string.Equals(percorso, "direct", StringComparison.OrdinalIgnoreCase))
                return SyncRoute.Direct;

            if (string.Equals(percorso, "async", StringComparison.OrdinalIgnoreCase))
                return SyncRoute.Async;

            throw new InvalidOperationException(
                $"Valore percorso non valido in SyncRouting: '{percorso}'. Valori ammessi: 'direct', 'async'.");
        }

        public void Dispose() => _changeToken?.Dispose();
    }
}
