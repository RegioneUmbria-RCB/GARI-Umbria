namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando non vengono trovate operazioni di irrigazione
    /// o fertirrigazione per il perimetro e l'anno specificati.
    /// Riferimento spec: DS03-BL RecuperoConsumoIdricoEffettivo — Eccezioni.
    /// </summary>
    public class NoIrrigationDataException : Exception
    {
        /// <param name="message">Descrizione del motivo dell'assenza di dati.</param>
        public NoIrrigationDataException(string message) : base(message) { }

        /// <param name="message">Descrizione del motivo dell'assenza di dati.</param>
        /// <param name="innerException">Eccezione originale.</param>
        public NoIrrigationDataException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
