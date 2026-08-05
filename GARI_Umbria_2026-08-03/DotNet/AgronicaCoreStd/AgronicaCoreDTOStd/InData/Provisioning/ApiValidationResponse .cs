using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class ApiValidationResponse
    {
        [JsonPropertyName("Found")]
        public bool Found { get; set; }

        [JsonPropertyName("ForceUpgrade")]
        public bool ForceUpgrade { get; set; }

        [JsonPropertyName("ForceDowngrade")]
        public bool ForceDowngrade { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Query")]
        public ApiValidationRequest Query { get; set; }

       
    }
}
