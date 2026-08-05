namespace AgronicaNetCore.DSSDifesa.DAL.DataLayer.Engine.DatiMeteo.Models
{
    /// <summary>
    /// Internal model representing a sensor reading from Meteo Suite API response.
    /// Maps sensor codes to their corresponding values.
    /// </summary>
    public class MeteoSensorsDataPoint
    {
        /// <summary>
        /// Dictionary of sensor codes to their values.
        /// Expected sensor codes:
        /// - TC2M_HOURLY: Mean hourly air temperature
        /// - PREC_HOURLY: Cumulative hourly precipitation
        /// - RH2M_HOURLY: Mean hourly relative humidity
        /// - LEAFWET_HOURLY: Hourly leaf wetness
        /// </summary>
        public Dictionary<string, decimal> Sensors { get; set; } = new Dictionary<string, decimal>();

        /// <summary>
        /// Timestamp of the sensor reading (ISO 8601 format).
        /// </summary>
        public DateTime PointInTime { get; set; }
    }
}
