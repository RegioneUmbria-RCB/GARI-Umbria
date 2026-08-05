namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API does not respond within the expected timeout.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API Meteo Suite non risponde entro 2s → blocco, status = ERRORE"
    /// </summary>
    public class MeteoDevicesTimeoutException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesTimeoutException"/> class.
        /// </summary>
        public MeteoDevicesTimeoutException(int timeoutSecondi) 
            : base($"The Meteo Suite API did not respond within the expected timeout period ({timeoutSecondi} seconds).")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesTimeoutException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoDevicesTimeoutException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesTimeoutException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoDevicesTimeoutException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
