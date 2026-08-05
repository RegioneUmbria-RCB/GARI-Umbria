namespace AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Represents the validation result of a single meteorological data point.
    /// </summary>
    public class ValidazioneDatiMeteoResult
    {
        /// <summary>
        /// Indicates if the data point is valid (all values within acceptable ranges).
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Indicates if the data point is complete (all expected parameters are present).
        /// </summary>
        public bool IsComplete { get; set; }

        /// <summary>
        /// List of validation messages explaining any issues with the data.
        /// </summary>
        public List<string> ValidationMessages { get; set; } = new List<string>();
    }
}
