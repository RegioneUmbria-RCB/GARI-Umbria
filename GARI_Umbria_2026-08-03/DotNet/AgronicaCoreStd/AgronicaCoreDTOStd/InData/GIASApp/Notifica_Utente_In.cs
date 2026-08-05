namespace AgronicaCoreDTOStd.InData.GiasApp
{
    /// <summary>
    /// Parametri per l'inserimento e la cancellazione della sottoscrizione alle notifiche push
    /// </summary>
    public class Notifica_Utente_In
    {

        /// <summary>
        /// ID del device di sottoscrizione
        /// </summary>
        /// <example>abc123</example>
        public string SubscriberID { get; set; }

        /// <summary>
        /// ID del servizio sottoscritto
        /// </summary>
        /// <example>1</example>
        public int IDServizio { get; set; }

        /// <summary>
        /// Piattaforma del servizio sottoscritto
        /// </summary>
        /// <example></example>
        public string Piattaforma { get; set; }
    }
}
