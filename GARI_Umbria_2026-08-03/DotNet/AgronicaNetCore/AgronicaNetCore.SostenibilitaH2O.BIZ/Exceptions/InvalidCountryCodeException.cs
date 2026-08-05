namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il codice paese fornito non è un codice ISO Alpha-3 valido
    /// (deve essere esattamente 3 caratteri alfabetici).
    /// <para>
    /// Questa eccezione è bloccante: il servizio non può proseguire senza un paese valido.
    /// </para>
    /// Riferimento spec: DS06-BL LookupBenchmarkWaterFootprint — Eccezioni.
    /// </summary>
    public class InvalidCountryCodeException : Exception
    {
        /// <param name="message">Descrizione del codice paese non valido.</param>
        public InvalidCountryCodeException(string message) : base(message) { }

        /// <param name="message">Descrizione del codice paese non valido.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public InvalidCountryCodeException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
