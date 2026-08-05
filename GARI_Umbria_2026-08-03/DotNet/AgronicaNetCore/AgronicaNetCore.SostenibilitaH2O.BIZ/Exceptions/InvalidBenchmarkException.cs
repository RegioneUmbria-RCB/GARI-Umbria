namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando il valore Green-Blue WF (m³/t) è ≤ 0.
    /// Il benchmark di consumo idrico non è calcolabile senza un WF positivo.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Eccezioni.
    /// </summary>
    public class InvalidBenchmarkException : Exception
    {
        /// <param name="message">Descrizione del valore WF non valido ricevuto.</param>
        public InvalidBenchmarkException(string message) : base(message) { }

        /// <param name="message">Descrizione del valore WF non valido ricevuto.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public InvalidBenchmarkException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
