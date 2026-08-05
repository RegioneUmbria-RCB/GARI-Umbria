namespace AgronicaCoreAPI.Messaging.Validation
{
    /// <summary>
    /// Valida il payload di una richiesta di sincronizzazione prima dell'accodamento su RabbitMQ,
    /// garantendo le stesse regole strutturali applicate dal percorso legacy.
    /// </summary>
    public interface ISyncPayloadValidator
    {
        /// <summary>
        /// Esegue la validazione strutturale del payload.
        /// </summary>
        /// <param name="tipoEntita">
        /// Tipo di entità (es. "attivita", "rilievi"). Usato per scegliere regole specifiche.
        /// </param>
        /// <param name="payload">Oggetto payload da validare. Non deve essere null.</param>
        /// <param name="guid">GUID dell'entità estratto dal payload o assegnato dal chiamante.</param>
        ValidationResult Validate(string tipoEntita, object payload, string guid);
    }
}
