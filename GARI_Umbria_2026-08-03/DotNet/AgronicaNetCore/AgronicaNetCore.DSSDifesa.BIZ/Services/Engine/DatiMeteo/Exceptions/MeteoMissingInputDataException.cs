namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Exceptions
{
    /// <summary>
    /// Exception thrown when meteorological data is missing for calculation.
    /// Occurs when neither station code nor valid coordinates are provided.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "Dati meteo mancanti per il calcolo: sia codice stazione che coordinate geografiche non fornite 
    /// (o coordinate fornite con latitudine o longitudine mancanti) → blocco, status = ERRORE"
    /// </summary>
    public class MeteoMissingInputDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class.
        /// </summary>
        public MeteoMissingInputDataException() 
            : base("Meteorological data is missing. Both station code and coordinates are absent or invalid.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class with a custom message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public MeteoMissingInputDataException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteoMissingInputDataException"/> class with a custom message and inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public MeteoMissingInputDataException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
