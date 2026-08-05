
namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteQueryWeatherSensorDataPointDto
    {
        public string Name { get; set; } = "";
        public float Value { get; set; } = 0;
    }

    public class MeteoSuiteQueryWeatherDataPointDto
    {
        public DateTime PointInTime { get; set; } = new DateTime();
        public List<MeteoSuiteQueryWeatherSensorDataPointDto> Sensors { get; set; } = new List<MeteoSuiteQueryWeatherSensorDataPointDto>();
    }
}
