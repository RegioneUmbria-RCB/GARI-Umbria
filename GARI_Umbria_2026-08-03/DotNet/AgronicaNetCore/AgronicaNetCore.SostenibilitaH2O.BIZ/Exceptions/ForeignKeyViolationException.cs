namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando il database risponde con una violazione di vincolo di chiave esterna
    /// (SqlException error number 547) durante l'inserimento in
    /// <c>Lookup_Sost_H20_Aziendale_Chiavi</c> o <c>Lookup_Sost_H20_Aziendale_Payload</c>.
    /// Indica che <c>id_invocazione</c> non è referenziabile nella relazione tra le due tabelle.
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda — ForeignKeyViolationException.
    /// </summary>
    public class ForeignKeyViolationException : Exception
    {
        /// <summary>Numero di errore SQL Server per violazione FK (error 547).</summary>
        public const int SqlForeignKeyErrorNumber = 547;

        /// <param name="message">Descrizione della violazione della chiave esterna.</param>
        public ForeignKeyViolationException(string message) : base(message) { }

        /// <param name="message">Descrizione della violazione della chiave esterna.</param>
        /// <param name="innerException">SqlException originale con error number 547.</param>
        public ForeignKeyViolationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
