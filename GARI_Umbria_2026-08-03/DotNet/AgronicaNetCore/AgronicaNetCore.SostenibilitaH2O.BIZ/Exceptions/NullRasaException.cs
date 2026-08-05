namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando la resa produttiva è zero e il benchmark idrico è richiesto.
    /// Il calcolo del fabbisogno idrico (wf * resa) non è significativo con resa = 0.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Eccezioni.
    /// </summary>
    public class NullRasaException : Exception
    {
        /// <param name="message">Descrizione del contesto in cui la resa risulta zero.</param>
        public NullRasaException(string message) : base(message) { }

        /// <param name="message">Descrizione del contesto in cui la resa risulta zero.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public NullRasaException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
