namespace AgronicaCoreAPI.Messaging.Routing
{
    /// <summary>
    /// Risolve il percorso di instradamento (direct o async) per una coppia
    /// clienteId × tipoEntita, basandosi sulla configurazione server-side.
    /// </summary>
    public interface IRoutingConfigService
    {
        /// <summary>
        /// Restituisce il <see cref="SyncRoute"/> appropriato per il cliente e il tipo di entità indicati.
        /// </summary>
        /// <param name="clienteId">
        /// Identificativo del cliente (pivaSuperUser estratto dal token JWT).
        /// </param>
        /// <param name="tipoEntita">
        /// Tipo di entità da sincronizzare (es. "attivita", "rilievi", "visite").
        /// </param>
        SyncRoute ResolveRoute(string clienteId, string tipoEntita);
    }
}
