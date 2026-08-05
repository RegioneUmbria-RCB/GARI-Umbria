namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API returns HTTP 404 Not Found status.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API Meteo Suite ritorna con altro status cod 404 (stazione meteo non trovata) → blocco, status = ERRORE"
    /// </summary>
    public class MeteoStationNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoStationNotFoundException"/> class.
        /// </summary>
        public MeteoStationNotFoundException() 
            : base("The requested meteorological station was not found in the Meteo Suite API.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoStationNotFoundException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoStationNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoStationNotFoundException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoStationNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
