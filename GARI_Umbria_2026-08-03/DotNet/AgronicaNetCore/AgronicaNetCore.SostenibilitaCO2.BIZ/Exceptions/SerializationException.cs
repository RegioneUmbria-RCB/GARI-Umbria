namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il payload M4 non è serializzabile a JSON valido.
    /// Si tratta di un errore tecnico che richiede debug della fase di assembly (DS03-DS05).
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4 — Eccezioni.
    /// </summary>
    public class SerializationException : Exception
    {
        /// <param name="message">Messaggio descrittivo del problema di serializzazione.</param>
        public SerializationException(string message)
            : base(message)
        {
        }

        /// <param name="message">Messaggio descrittivo del problema di serializzazione.</param>
        /// <param name="innerException">Eccezione originale di serializzazione.</param>
        public SerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
