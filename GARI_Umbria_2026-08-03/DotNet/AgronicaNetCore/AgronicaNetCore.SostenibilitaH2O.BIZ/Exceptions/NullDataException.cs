namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata (come warning non-bloccante) quando i dati di input per un esercizio
    /// sono parziali o mancanti. L'indicatore viene impostato a INDETERMINATO.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Eccezioni NullDataException.
    /// </summary>
    public class NullDataException : Exception
    {
        /// <summary>Identificativo dell'esercizio per cui i dati sono incompleti.</summary>
        public string IdEsercizio { get; }

        /// <param name="idEsercizio">Identificativo dell'esercizio con dati incompleti.</param>
        /// <param name="message">Descrizione dei campi mancanti.</param>
        public NullDataException(string idEsercizio, string message)
            : base(message)
        {
            IdEsercizio = idEsercizio;
        }

        /// <param name="idEsercizio">Identificativo dell'esercizio con dati incompleti.</param>
        /// <param name="message">Descrizione dei campi mancanti.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public NullDataException(string idEsercizio, string message, Exception innerException)
            : base(message, innerException)
        {
            IdEsercizio = idEsercizio;
        }
    }
}
