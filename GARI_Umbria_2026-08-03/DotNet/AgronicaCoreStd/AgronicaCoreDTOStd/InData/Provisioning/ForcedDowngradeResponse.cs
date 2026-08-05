using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace InData.Provisioning
{
   public class ForcedDowngradeResponse
    {
        [JsonPropertyName("ForceUpgrade")]
        public bool ForceUpgrade { get; set; }

        [JsonPropertyName("ForceDowngrade")]
        public bool ForceDowngrade { get; set; }

        [JsonPropertyName("Message")]
        public string Message { get; set; }

    }
}
