using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class ClientValidationRequest
    {

        [JsonPropertyName("x-app-name")]
        public string xAppName { get; set; }

        [JsonPropertyName("x-app-version")]
        public string xAppVersion { get; set; }

        [JsonPropertyName("x-platform")]
        public string xPlatform { get; set; }

        [JsonPropertyName("x-environment")]
        public string xEnvironment { get; set; }


    }
}
