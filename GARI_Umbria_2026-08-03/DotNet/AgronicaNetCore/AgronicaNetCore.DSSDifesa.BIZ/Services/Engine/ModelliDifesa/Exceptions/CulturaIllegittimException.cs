namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.ModelliDifesa.Exceptions
{
    /// <summary>
    /// Exception thrown when the crop is not recognized by the DSS Engine.
    /// Referenced in DS03-BL_ Recupero Lista Infestanti Dinamica - Eccezioni section.
    /// </summary>
    public class CulturaIllegittimException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the CulturaIllegittimException class.
        /// </summary>
        public CulturaIllegittimException()
            : base("coltura non riconosciuta da Engine DSS")
        {
        }

        /// <summary>
        /// Initializes a new instance of the CulturaIllegittimException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CulturaIllegittimException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CulturaIllegittimException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        public CulturaIllegittimException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}