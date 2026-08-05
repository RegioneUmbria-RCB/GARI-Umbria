using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteQueryWeatherStationResponseDto
    {
        public MeteoSuiteQueryWeatherStationDto device {  get; set; } = new MeteoSuiteQueryWeatherStationDto();
        public double distance { get; set; }
    }
}
