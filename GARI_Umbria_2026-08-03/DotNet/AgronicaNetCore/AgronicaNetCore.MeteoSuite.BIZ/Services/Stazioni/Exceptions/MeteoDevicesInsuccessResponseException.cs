using System.Net;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Stazioni.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API returns an unexpected HTTP error status code other than 401 or 404.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API Meteo Suite ritorna con altro status cod diverso da 200 → blocco, status = ERRORE"
    /// </summary>
    public class MeteoDevicesInsuccessResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesInsuccessResponseException"/> class.
        /// </summary>
        public MeteoDevicesInsuccessResponseException(HttpStatusCode statusCode, string responseBody) 
            : base($"The Meteo Suite API returned an unexpected error status code: {(int)statusCode}: {responseBody}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesInsuccessResponseException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoDevicesInsuccessResponseException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoDevicesInsuccessResponseException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoDevicesInsuccessResponseException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
