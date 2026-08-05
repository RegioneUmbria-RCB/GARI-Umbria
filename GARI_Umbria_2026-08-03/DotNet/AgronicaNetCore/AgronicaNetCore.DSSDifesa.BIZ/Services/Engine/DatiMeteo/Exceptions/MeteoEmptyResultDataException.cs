namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API returns empty response meteo data.
    /// 
    /// </summary>
    public class MeteoEmptyResultDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class.
        /// </summary>
        public MeteoEmptyResultDataException()
            : base("Returned no metereological data usable for DSS Difesa.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoEmptyResultDataException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoEmptyResultDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
