using System;
using System.Collections.Generic;

namespace OutData.Engine.MeteoSuite
{
    public class DatiMeteoPioggeUmiditaTerreno
    {
        public string Sensore { get; set; }
        public float Valore { get; set; }
    }

    public class DatiMeteoPioggeOrariDto
    {
        /// <summary>
        /// The date and time of the meteorological data in ISO 8601 format (UTC).
        /// Represents the hour for which the data is aggregated (minutes and seconds are 0).
        /// </summary>
        public DateTime DataOra { get; set; }

        /// <summary>
        /// Hourly leaf wetness index (bagnatura fogliare).
        /// Valid range: [0, 1] (normalized)
        /// Source: LEAFWET_HOURLY sensor
        /// </summary>
        public float? BagnaturaFogliare { get; set; }

        /// <summary>
        /// Cumulative hourly precipitation in millimeters.
        /// Valid range: [0, ∞) mm
        /// Source: PREC_HOURLY sensor
        /// </summary>
        public float? Precipiatazioni { get; set; }

        /// <summary>
        /// Mean hourly air temperature in degrees Celsius.
        /// Valid range: [-50, +60]°C
        /// Source: TC2M_HOURLY sensor
        /// </summary>
        public float? Temperatura { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        public float? UmiditaRelativa { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        //public float? UmiditaTerreno { get; set; }
        public List<DatiMeteoPioggeUmiditaTerreno> UmiditaTerreno { get; set; }
    }

    public class DatiMeteoPioggeGiornalieriDto
    {
        /// <summary>
        /// The date and time of the meteorological data in ISO 8601 format (UTC).
        /// Represents the hour for which the data is aggregated (minutes and seconds are 0).
        /// </summary>
        public DateTime DataOra { get; set; }

        /// <summary>
        /// Hourly leaf wetness index (bagnatura fogliare).
        /// Valid range: [0, 1] (normalized)
        /// Source: LEAFWET_HOURLY sensor
        /// </summary>
        public float? BagnaturaFogliare { get; set; }

        /// <summary>
        /// Cumulative hourly precipitation in millimeters.
        /// Valid range: [0, ∞) mm
        /// Source: PREC_HOURLY sensor
        /// </summary>
        public float? PrecipiatazioniCumulate { get; set; }

        /// <summary>
        /// Cumulative hourly precipitation in millimeters.
        /// Valid range: [0, ∞) mm
        /// Source: PREC_HOURLY sensor
        /// </summary>
        public int? OrePioggia { get; set; }

        /// <summary>
        /// Mean hourly air temperature in degrees Celsius.
        /// Valid range: [-50, +60]°C
        /// Source: TC2M_HOURLY/TC2M_DAILY sensor
        /// </summary>
        public float? TemperaturaMin { get; set; }

        /// <summary>
        /// Mean hourly air temperature in degrees Celsius.
        /// Valid range: [-50, +60]°C
        /// Source: TC2M_HOURLY/TC2M_DAILY sensor
        /// </summary>
        public float? TemperaturaMax { get; set; }

        /// <summary>
        /// Mean hourly air temperature in degrees Celsius.
        /// Valid range: [-50, +60]°C
        /// Source: TC2M_HOURLY sensor
        /// </summary>
        public float? TemperaturaMedia { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        public float? UmiditaRelativaMin { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        public float? UmiditaRelativaMax { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        public float? UmiditaRelativaMedia { get; set; }

        /// <summary>
        /// Mean hourly relative humidity in percentage.
        /// Valid range: [0, 100]%
        /// Source: RH2M_HOURLY sensor
        /// </summary>
        //public float? UmiditaTerreno { get; set; }
        public List<DatiMeteoPioggeUmiditaTerreno> UmiditaTerreno { get; set; }
    }

    public class DatiMeteoPioggiaSensore
    {
        public string Sensore { get; set; }
        public string Etichetta { get; set; }
        public string TipoSensore { get; set; }
        public string UM { get; set; }
        public string FunAggreg { get; set; }
    }

    public class DatiMeteoPioggiaSensori
    {
        public DatiMeteoPioggiaSensore SensoreBagnaturaFogliare { get; set; }
        public DatiMeteoPioggiaSensore SensorePrecipiatazioni { get; set; }
        public DatiMeteoPioggiaSensore SensoreTemperatura { get; set; }
        public DatiMeteoPioggiaSensore SensoreUmiditaRelativa { get; set; }
        public List<DatiMeteoPioggiaSensore> SensoriUmiditaTerreno { get; set; }
    }

    public class AcquisizioneDatiMeteoPioggeOrariResponse
    {
        /// <summary>
        /// Collection of meteorological data points for the requested time period.
        /// Data is aggregated hourly with all available sensors.
        /// </summary>
        public List<DatiMeteoPioggeOrariDto> MeteoData { get; set; } = new List<DatiMeteoPioggeOrariDto>();
        public DatiMeteoPioggiaSensori Sensors { get; set; }
    }

    public class AcquisizioneDatiMeteoPioggeGiornalieriResponse
    {
        /// <summary>
        /// Collection of meteorological data points for the requested time period.
        /// Data is aggregated hourly with all available sensors.
        /// </summary>
        public List<DatiMeteoPioggeGiornalieriDto> MeteoData { get; set; } = new List<DatiMeteoPioggeGiornalieriDto>();
        public DatiMeteoPioggiaSensori Sensors { get; set; }
    }
}
