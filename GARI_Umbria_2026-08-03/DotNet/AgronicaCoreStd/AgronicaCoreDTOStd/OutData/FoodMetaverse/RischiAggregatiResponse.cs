using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// API response for GET /v1/rischi/aggregati.
    /// </summary>
    public class RischiAggregatiApiResponse
    {
        /// <summary>Operation status: success or no_data.</summary>
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>Aggregated risk payload.</summary>
        [JsonProperty("data")]
        public RischiAggregatiData Data { get; set; } = new RischiAggregatiData();

        /// <summary>Optional informational message for no_data responses.</summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>Response metadata.</summary>
        [JsonProperty("meta")]
        public RischiAggregatiMeta Meta { get; set; } = new RischiAggregatiMeta();
    }

    /// <summary>
    /// Error payload for GET /v1/rischi/aggregati.
    /// </summary>
    public class RischiAggregatiErrorResponse
    {
        [JsonProperty("error")]
        public RischiAggregatiError Error { get; set; } = new RischiAggregatiError();

        [JsonProperty("retry_after", NullValueHandling = NullValueHandling.Ignore)]
        public int? RetryAfter { get; set; }

        [JsonProperty("meta")]
        public RischiAggregatiMeta Meta { get; set; } = new RischiAggregatiMeta();
    }

    /// <summary>
    /// Error details for GET /v1/rischi/aggregati.
    /// </summary>
    public class RischiAggregatiError
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Computed risk aggregates for global, nation and region scopes.
    /// </summary>
    public class RischiAggregatiData
    {
        [JsonProperty("rischio_gelo")]
        public double? RischioGelo { get; set; }

        [JsonProperty("rischio_siccita")]
        public double? RischioSiccita { get; set; }

        [JsonProperty("rischio_allagamento")]
        public double? RischioAllagamento { get; set; }

        [JsonProperty("superficie_totale_ha")]
        public double SuperficieTotaleHa { get; set; }

        [JsonProperty("nazioni")]
        public List<RischiAggregatiNazione> Nazioni { get; set; } = new List<RischiAggregatiNazione>();
    }

    /// <summary>
    /// Nation-level aggregates.
    /// </summary>
    public class RischiAggregatiNazione
    {
        [JsonProperty("cod_nazione")]
        public string CodNazione { get; set; } = string.Empty;

        [JsonProperty("rischio_gelo")]
        public double? RischioGelo { get; set; }

        [JsonProperty("rischio_siccita")]
        public double? RischioSiccita { get; set; }

        [JsonProperty("rischio_allagamento")]
        public double? RischioAllagamento { get; set; }

        [JsonProperty("superficie_totale_ha")]
        public double SuperficieTotaleHa { get; set; }

        [JsonProperty("numero_esercizi")]
        public int NumeroEsercizi { get; set; }

        [JsonProperty("regioni")]
        public List<RischiAggregatiRegione> Regioni { get; set; } = new List<RischiAggregatiRegione>();
    }

    /// <summary>
    /// Region-level aggregates within a nation.
    /// </summary>
    public class RischiAggregatiRegione
    {
        [JsonProperty("regione")]
        public string Regione { get; set; }

        [JsonProperty("rischio_gelo")]
        public double? RischioGelo { get; set; }

        [JsonProperty("rischio_siccita")]
        public double? RischioSiccita { get; set; }

        [JsonProperty("rischio_allagamento")]
        public double? RischioAllagamento { get; set; }

        [JsonProperty("superficie_totale_ha")]
        public double SuperficieTotaleHa { get; set; }

        [JsonProperty("numero_esercizi")]
        public int NumeroEsercizi { get; set; }
    }

    /// <summary>
    /// Metadata for GET /v1/rischi/aggregati responses.
    /// </summary>
    public class RischiAggregatiMeta
    {
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonProperty("request_id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonProperty("numero_righe_elaborate", NullValueHandling = NullValueHandling.Ignore)]
        public int? NumeroRigheElaborate { get; set; }

        [JsonProperty("numero_righe_valide", NullValueHandling = NullValueHandling.Ignore)]
        public int? NumeroRigheValide { get; set; }
    }
}