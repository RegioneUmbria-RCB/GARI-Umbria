namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata in caso di errore durante l'esecuzione di una query
    /// sul database GIAS nel contesto del recupero del consumo idrico effettivo.
    /// Riferimento spec: DS03-BL RecuperoConsumoIdricoEffettivo — Eccezioni.
    /// </summary>
    public class DatabaseQueryException : Exception
    {
        /// <param name="message">Descrizione dell'errore di query.</param>
        public DatabaseQueryException(string message) : base(message) { }

        /// <param name="message">Descrizione dell'errore di query.</param>
        /// <param name="innerException">Eccezione SQL originale.</param>
        public DatabaseQueryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
