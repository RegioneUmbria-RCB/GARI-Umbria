namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API does not respond within the expected timeout.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API Meteo Suite non risponde entro 2s → blocco, status = ERRORE"
    /// </summary>
    public class MeteoDeviceNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDeviceNotFoundException"/> class.
        /// </summary>
        public MeteoDeviceNotFoundException(int deviceId) 
            : base($"Meteo Suite device not found for given ID ({deviceId.ToString()}).")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDeviceNotFoundException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoDeviceNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDeviceNotFoundException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoDeviceNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
