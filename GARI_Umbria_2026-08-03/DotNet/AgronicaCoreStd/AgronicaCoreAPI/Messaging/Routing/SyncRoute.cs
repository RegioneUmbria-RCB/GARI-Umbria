namespace AgronicaCoreAPI.Messaging.Routing
{
    /// <summary>
    /// Route di elaborazione per una richiesta di sincronizzazione.
    /// </summary>
    public enum SyncRoute
    {
        /// <summary>Elaborazione sincrona diretta tramite CoreWS (percorso legacy).</summary>
        Direct,

        /// <summary>Elaborazione asincrona tramite coda RabbitMQ.</summary>
        Async
    }
}
