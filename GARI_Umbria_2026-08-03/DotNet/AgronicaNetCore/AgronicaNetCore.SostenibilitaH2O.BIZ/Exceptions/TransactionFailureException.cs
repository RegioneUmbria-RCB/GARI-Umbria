namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando l'intera transazione di inserimento batch fallisce (disk full, permission, etc.).
    /// Causa il rollback atomico dell'intera operazione.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture — TransactionFailureException.
    /// </summary>
    public class TransactionFailureException : Exception
    {
        /// <param name="message">Descrizione del motivo del fallimento della transazione.</param>
        public TransactionFailureException(string message) : base(message) { }

        /// <param name="message">Descrizione del motivo del fallimento della transazione.</param>
        /// <param name="innerException">Eccezione originale che ha causato il rollback.</param>
        public TransactionFailureException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
