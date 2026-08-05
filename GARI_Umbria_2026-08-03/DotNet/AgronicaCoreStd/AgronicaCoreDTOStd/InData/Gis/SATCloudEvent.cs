using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Represents the payload of a CloudEvent sent by SAT / Grandi Layer when a polygon calculation is completed.
    /// See: /Gis/NuovaElaborazionePiattaformaSAT
    /// Dictionary<string -> (type, string) // type è il tipo del CloudEvent vero e proprio (SatCloudEvent), string è invece l'url interno da contatare
    /// </summary>
    public class SatCloudEvent
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

        [JsonProperty("data")]
        public SatCloudEventData Data { get; set; }
    }

    public class SatCloudEventData
    {
        [JsonProperty("polygonId")]
        public string PolygonId { get; set; }

        [JsonProperty("indexCode")]
        public string IndexCode { get; set; }

        [JsonProperty("lastDate")]
        public DateTime LastDate { get; set; }
    }
}
