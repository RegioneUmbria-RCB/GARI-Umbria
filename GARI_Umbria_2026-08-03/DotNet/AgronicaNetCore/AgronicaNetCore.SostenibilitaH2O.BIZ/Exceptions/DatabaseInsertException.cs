namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando si verifica un errore durante l'inserimento batch nella tabella lookup_sost_h2o_lotto.
    /// È bloccante: provoca il rollback dell'intera transazione.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiPerColture — Eccezioni.
    /// </summary>
    public class DatabaseInsertException : Exception
    {
        /// <param name="message">Messaggio di errore dettagliato sull'operazione di inserimento fallita.</param>
        public DatabaseInsertException(string message) : base(message) { }

        /// <param name="message">Messaggio di errore dettagliato.</param>
        /// <param name="innerException">Eccezione di database originale.</param>
        public DatabaseInsertException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
