using Newtonsoft.Json;
using System;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Represents the CloudEvent payload sent by the Engine Mappe Prescrizione when a prescription map
    /// calculation completes (success or error).
    /// See: /Gis/NuovaElaborazioneMappePrescrizione
    /// </summary>
    public class MappePrescrizioneCloudEvent
    {
        [JsonProperty("specVersion")]
        public string SpecVersion { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("time")]
        public DateTimeOffset Time { get; set; }

        [JsonProperty("dataContentType")]
        public string DataContentType { get; set; }

        [JsonProperty("executionid")]
        public string ExecutionId {get ;set;}

        [JsonProperty("requestid")]
        public string RequestId {get ;set;}

        [JsonProperty("tenant")]
        public string Tenant {get ;set;}

        [JsonProperty("data")]
        public MappePrescrizioneCloudEventData Data { get; set; }
    }

    public class MappePrescrizioneCloudEventData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public MappePrescrizioneCloudEventDataResult Result { get; set; }
    }

    public class MappePrescrizioneCloudEventDataResult
    {
        [JsonProperty("asset_location")]
        public string AssetLocation { get; set; }
    }
}
