namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API returns malformed or invalid response data.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API ritorna dati malformati → blocco, status = ERRORE"
    /// </summary>
    public class MeteoInvalidResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoInvalidResponseException"/> class.
        /// </summary>
        public MeteoInvalidResponseException() 
            : base("The Meteo Suite API returned malformed or invalid response data.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoInvalidResponseException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoInvalidResponseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoInvalidResponseException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoInvalidResponseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
