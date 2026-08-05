namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando la costruzione o la serializzazione del payload JSON per la modalità
    /// "Per Azienda" fallisce a causa di un errore interno (es. JsonException).
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda — PayloadGenerationException.
    /// </summary>
    public class PayloadGenerationException : Exception
    {
        /// <param name="message">Descrizione del fallimento nella generazione del payload.</param>
        public PayloadGenerationException(string message) : base(message) { }

        /// <param name="message">Descrizione del fallimento nella generazione del payload.</param>
        /// <param name="innerException">Eccezione originale (es. <see cref="System.Text.Json.JsonException"/>).</param>
        public PayloadGenerationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
