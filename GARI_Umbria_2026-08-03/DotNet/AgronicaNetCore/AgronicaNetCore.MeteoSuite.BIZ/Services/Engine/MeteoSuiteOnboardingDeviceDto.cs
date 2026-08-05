using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteOnboardingDeviceSensorDto
    {
        public int id { get; set; } = 0;
        public string description { get; set; } = "";
        public string providerReference { get; set; } = "";
        public int sensorTypeId { get; set; } = 0;
        public string sensorType { get; set; } = "";
        public string dataType { get; set; } = "";
        public string uom { get; set; } = "";
        public string aggregationFunc { get; set; } = "";
    }

    public class MeteoSuiteOnboardingDeviceDto
    {
        public int id { get; set; } = 0;
        public string description { get; set; } = "";
        public decimal latitude { get; set; } = 0;
        public decimal longitude { get; set; } = 0;
        public int providerId { get; set; } = 0;
        public string provider { get; set; } = "";
        public bool flIsReal { get; set; }
        public string originalTimeZone { get; set; } = "";
        public string category { get; set; } = "";
        public string providerReference { get; set; } = "";
        public string deviceType { get; set; } = "";
        public int totalSensors { get; set; } = 0;
        public bool isOwner { get; set; }
        public List<MeteoSuiteOnboardingDeviceSensorDto> sensors { get; set; } = new List<MeteoSuiteOnboardingDeviceSensorDto>();
    }
}
