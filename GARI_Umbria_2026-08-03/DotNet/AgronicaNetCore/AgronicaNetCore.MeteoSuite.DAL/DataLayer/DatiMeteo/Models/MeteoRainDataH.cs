namespace AgronicaNetCore.MeteoSuite.DAL.DataLayer.DatiMeteo.Models
{
    /// <summary>
    /// Data Transfer Object representing a single meteorological data point with hourly aggregation.
    /// 
    /// Referenced in DS02-BL_ Acquisizione Dati Meteorologici Ibrida - Output section:
    /// Contains normalized meteorological parameters (temperature, precipitation, humidity, leaf wetness)
    /// all aggregated on an hourly basis from Meteo Suite sensors.
    /// </summary>
    public class MeteoRainDataH
    {
        /// <summary>
        /// The date and time of the meteorological data in ISO 8601 format (UTC).
        /// Represents the hour for which the data is aggregated (minutes and seconds are 0).
        /// </summary>
        public DateTime DataOra { get; set; }

        /// <summary>
        /// Mean hourly air temperature in degrees Celsius.
        /// Valid range: [-50, +60]°C
        /// Source: TC2M_HOURLY sensor
        /// </summary>
        public decimal? Temp { get; set; }

        /// <summary>
        /// Cumulative hourly precipitation in millimeters.
        /// Valid range: [0, ∞) mm
        /// Source: PREC_HOURLY sensor
        /// </summary>
        public decimal? Prec { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        public decimal? RelHum { get; set; }

        /// <summary>
        /// Hourly leaf wetness index (bagnatura fogliare).
        /// Valid range: [0, 1] (normalized)
        /// Source: LEAFWET_HOURLY sensor
        /// </summary>
        public decimal? Lw { get; set; }
    }
}
