namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando la superficie dell'appezzamento è ≤ 0.
    /// Il calcolo del bilancio idrico normalizzato per ettaro non è eseguibile.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Eccezioni.
    /// </summary>
    public class InvalidSurfaceException : Exception
    {
        /// <param name="message">Descrizione del valore non valido ricevuto.</param>
        public InvalidSurfaceException(string message) : base(message) { }

        /// <param name="message">Descrizione del valore non valido ricevuto.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public InvalidSurfaceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
