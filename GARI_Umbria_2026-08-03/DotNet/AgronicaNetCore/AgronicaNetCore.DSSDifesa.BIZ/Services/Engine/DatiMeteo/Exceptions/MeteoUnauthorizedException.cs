namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions
{
    /// <summary>
    /// Exception thrown when the Meteo Suite API returns HTTP 401 Unauthorized status.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "API Meteo Suite ritorna con status cod 401 (non autorizzato) → blocco, status = ERRORE"
    /// </summary>
    public class MeteoUnauthorizedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoUnauthorizedException"/> class.
        /// </summary>
        public MeteoUnauthorizedException() 
            : base("The Meteo Suite API returned an unauthorized (401) response. API key may be invalid or expired.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoUnauthorizedException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoUnauthorizedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoUnauthorizedException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoUnauthorizedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
