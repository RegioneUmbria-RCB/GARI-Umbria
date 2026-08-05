using AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione
{
    /// <summary>
    /// Data Transfer Object representing the response from hybrid meteorological data acquisition.
    /// 
    /// Contains aggregated meteorological data with validation status information.
    /// Status indicates the overall quality of the retrieved data (VALIDO, INCOMPLETO, ANOMALO, ERRORE).
    /// </summary>
    public class AcquisizioneDatiMeteoPioggeResponse
    {
        /// <summary>
        /// Collection of meteorological data points for the requested time period.
        /// Data is aggregated hourly with all available sensors.
        /// </summary>
        public List<MeteoRainDataH> MeteoData { get; set; } = new List<MeteoRainDataH>();

        /// <summary>
        /// Overall validation status of the meteorological data.
        /// 
        /// Valid values:
        /// - VALIDO: All data points are within acceptable ranges and complete
        /// - INCOMPLETO: One or more parameters are missing or at least 95% of data is not available in 24h window
        /// - ANOMALO: One or more data values are outside acceptable ranges
        /// - ERRORE: An error occurred during data acquisition (API unreachable, unauthorized, station not found, malformed response)
        /// 
        /// Referenced in DS02-BL_: 
        /// Validazione range: temp ∈ [-50, +60]°C, relHum ∈ [0, 100]%, prec ≥ 0 mm, lw ∈ [0, 1]
        /// Dati fuori range → status = ANOMALO; dati parziali → status = INCOMPLETO
        /// Completamento dati meteorici: almeno 95% dei dati validi su finestra 24h
        /// </summary>
        public string Status { get; set; } = "ERRORE";
    }
}
