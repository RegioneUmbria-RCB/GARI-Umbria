namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando uno o più campi non superano la validazione
    /// dei dati di consumo aziendale (carburanti od energia).
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Eccezioni.
    /// </summary>
    public class ConsumiValidationException : Exception
    {
        /// <summary>Elenco dei messaggi di errore rilevati durante la validazione.</summary>
        public IReadOnlyList<string> Errori { get; }

        public ConsumiValidationException(IReadOnlyList<string> errori)
            : base("Uno o più campi non superano la validazione dei consumi aziendali.")
        {
            Errori = errori;
        }

        public ConsumiValidationException(IReadOnlyList<string> errori, Exception innerException)
            : base("Uno o più campi non superano la validazione dei consumi aziendali.", innerException)
        {
            Errori = errori;
        }
    }
}
