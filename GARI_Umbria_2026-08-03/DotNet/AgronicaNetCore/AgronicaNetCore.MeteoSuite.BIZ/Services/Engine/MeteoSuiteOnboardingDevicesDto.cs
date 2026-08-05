using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.Engine
{
    public class MeteoSuiteOnboardingDevicesDto
    {
        public int totalCount { get; set; } = 0;
        public int offset { get; set; } = 0;
        public int limit { get; set; } = 0;
        public List<MeteoSuiteOnboardingDeviceDto> devices = new List<MeteoSuiteOnboardingDeviceDto>();
    }
}
