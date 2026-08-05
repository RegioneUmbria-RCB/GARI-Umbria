using System.Collections.Generic;

namespace AgronicaCoreAPI.Messaging.Routing
{
    /// <summary>
    /// Opzioni di configurazione per l'instradamento dinamico delle richieste di sincronizzazione.
    /// Viene letta dalla sezione "SyncRouting" di appsettings.json (o Azure AppConfig).
    /// </summary>
    public class RoutingConfigOptions
    {
        public const string SectionName = "SyncRouting";

        /// <summary>
        /// Intervallo in secondi entro cui le modifiche alla configurazione si propagano
        /// a tutte le istanze (ricarica periodica). Default: 30 secondi.
        /// </summary>
        public int RefreshIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Lista ordinata di regole di routing. La prima corrispondenza vince.
        /// Se nessuna regola corrisponde, il default è "async".
        /// </summary>
        public List<RoutingRule> Rules { get; set; } = new();
    }
}
