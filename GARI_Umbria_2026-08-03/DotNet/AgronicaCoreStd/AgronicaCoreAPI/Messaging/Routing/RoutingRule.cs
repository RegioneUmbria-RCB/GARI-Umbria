namespace AgronicaCoreAPI.Messaging.Routing
{
    /// <summary>
    /// Singola regola di routing per-cliente × per-tipologia di entità.
    /// Le regole sono valutate in ordine; la prima corrispondenza vince.
    /// </summary>
    public class RoutingRule
    {
        /// <summary>
        /// Identificativo cliente (pivaSuperUser). Usare "*" come wildcard.
        /// </summary>
        public string ClienteId { get; set; } = "*";

        /// <summary>
        /// Tipo entità (es. "attivita", "rilievi", "visite"). Usare "*" come wildcard.
        /// </summary>
        public string TipoEntita { get; set; } = "*";

        /// <summary>
        /// Percorso di instradamento: "direct" (legacy sincrono) oppure "async" (RabbitMQ).
        /// </summary>
        public string Percorso { get; set; } = "direct";
    }
}
