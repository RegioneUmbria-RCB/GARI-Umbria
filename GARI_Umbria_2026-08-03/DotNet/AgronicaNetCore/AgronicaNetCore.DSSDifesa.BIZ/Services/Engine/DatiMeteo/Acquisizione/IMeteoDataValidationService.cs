using AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models;

namespace AgronicaNetCore.DSSDifesa.BIZ.Services.Engine.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Interface for meteorological data validation service.
    /// Validates meteorological data values against acceptable ranges.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida:
    /// "Validazione range: temp ∈ [-50, +60]°C, relHum ∈ [0, 100]%, prec ≥ 0 mm, lw ∈ [0, 1]
    /// Dati fuori range → status = ANOMALO; dati parziali (uno/più parametri mancanti) → status = INCOMPLETO"
    /// </summary>
    public interface IMeteoDataValidationService
    {
        /// <summary>
        /// Validates a single meteorological data point.
        /// Checks all parameters (temperature, humidity, precipitation, leaf wetness) against acceptable ranges.
        /// </summary>
        /// <param name="meteoData">The meteorological data point to validate</param>
        /// <returns>Validation result containing isValid, isComplete, and any validation messages</returns>
        MeteoDataValidationResult ValidateSingleDataPoint(MeteoDataH meteoData);

        /// <summary>
        /// Determines the overall status of meteorological data collection.
        /// </summary>
        /// <param name="dataPoints">The collection of meteorological data points</param>
        /// <param name="validationResults">The validation results for each data point</param>
        /// <returns>
        /// Status string:
        /// - VALIDO: All data points are valid and complete
        /// - INCOMPLETO: One or more parameters are missing in any data point, or &lt; 95% data availability in any 24h window
        /// - ANOMALO: One or more data values are outside acceptable ranges
        /// - ERRORE: Should be set by calling service on exception
        /// </returns>
        string DetermineOverallStatus(List<MeteoDataH> dataPoints, List<MeteoDataValidationResult> validationResults);
    }
}
