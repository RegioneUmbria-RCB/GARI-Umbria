namespace AgronicaCoreDTOStd.InData.Provisioning
{
    /// <summary>
    /// Parametri per l'inserimento e la cancellazione della sottoscrizione alle notifiche push
    /// </summary>
    public class DatiServerRequest
    {

        /// <summary>
        /// ID del device di sottoscrizione
        /// </summary>
        /// <example>abc123</example>
        public enum_Scelta_Server SceltaServer { get; set; }

        /// <summary>
        /// Enumeratore per la scelta del server
        /// </summary>
        /// <example>abc123</example>
        public enum enum_Scelta_Server
        {
            Server = 1,
            Utenti = 2
        }
    }
}
