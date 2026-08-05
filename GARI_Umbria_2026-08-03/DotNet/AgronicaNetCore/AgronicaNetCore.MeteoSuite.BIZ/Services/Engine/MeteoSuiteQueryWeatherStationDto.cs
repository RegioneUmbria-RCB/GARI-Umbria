using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class GeoPosition
    {
        public decimal x {  get; set; }
        public decimal y { get; set; }
    }

    public class MeteoSuiteQueryWeatherSensorDto
    {
        public string code { get; set; } = "";
        public string dataType { get; set; } = "";
        public string uom { get; set; } = "";
        public string description { get; set; } = "";
    }

    public class MeteoSuiteQueryWeatherStationDto
    {
        public string type { get; set; } = "";
        public string code { get; set; } = "";
        public string description { get; set; } = "";
        public GeoPosition position { get; set; } = new GeoPosition();
        public string coverage { get; set; } = "";
        public List<MeteoSuiteQueryWeatherSensorDto> sensors { get; set; } = new List<MeteoSuiteQueryWeatherSensorDto>();
    }
}
