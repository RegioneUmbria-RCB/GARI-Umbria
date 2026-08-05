
using System.Text.Json.Serialization;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class ApiValidationRequest
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
