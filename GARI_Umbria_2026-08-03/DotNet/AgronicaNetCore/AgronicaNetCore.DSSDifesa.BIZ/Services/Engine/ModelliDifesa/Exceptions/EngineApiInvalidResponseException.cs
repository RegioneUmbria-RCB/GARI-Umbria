namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception thrown when the DSS Engine API returns a malformed JSON response.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica - Eccezioni section.
    /// </summary>
    public class EngineApiInvalidResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the EngineApiInvalidResponseException class.
        /// </summary>
        public EngineApiInvalidResponseException()
            : base("API ritorna JSON malformato")
        {
        }

        /// <summary>
        /// Initializes a new instance of the EngineApiInvalidResponseException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EngineApiInvalidResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the EngineApiInvalidResponseException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public EngineApiInvalidResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}