namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Exceptions
{
    /// <summary>
    /// Sollevata quando il <c>payload_json</c> fornito in input non supera la validazione
    /// sintattica JSON (parse test) oppure è vuoto.
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda — InvalidJsonException.
    /// </summary>
    public class InvalidJsonException : Exception
    {
        /// <param name="message">Descrizione del problema di validazione JSON.</param>
        public InvalidJsonException(string message) : base(message) { }

        /// <param name="message">Descrizione del problema di validazione JSON.</param>
        /// <param name="innerException">Eccezione originale (es. <see cref="System.Text.Json.JsonException"/>).</param>
        public InvalidJsonException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
