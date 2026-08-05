using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class ClientValidationResponse
    {
        [JsonPropertyName("Found")]
        public bool Found { get; set; }

        [JsonPropertyName("ForceUpgrade")]
        public bool ForceUpgrade { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

        [JsonPropertyName("Query")]
        public ClientValidationRequest Query { get; set; }
    }
}
