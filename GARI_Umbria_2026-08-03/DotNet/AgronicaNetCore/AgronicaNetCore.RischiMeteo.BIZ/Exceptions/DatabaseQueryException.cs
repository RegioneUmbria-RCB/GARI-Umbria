namespace AgronicaNetCore.RischiMeteo.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando una query GIAS fallisce (timeout, connessione interrotta).
    /// L'Esercizio viene incluso in <c>errori_dettaglio</c> e gli altri Esercizi continuano.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Eccezioni, DatabaseQueryException.
    /// </summary>
    public class DatabaseQueryException : Exception
    {
        /// <summary>Identificativo dell'Esercizio per cui la query è fallita.</summary>
        public string IdEsercizio { get; }

        public DatabaseQueryException(string idEsercizio, string dettaglio, Exception innerException)
            : base($"Query GIAS fallita per l'Esercizio '{idEsercizio}': {dettaglio}", innerException)
        {
            IdEsercizio = idEsercizio;
        }
    }
}
