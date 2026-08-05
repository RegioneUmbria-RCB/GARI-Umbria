using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// API response for GET /v1/rischi/dettaglio.
    /// </summary>
    public class RischiDettaglioApiResponse
    {
        /// <summary>Operation status: success or no_data.</summary>
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>Detail payload.</summary>
        [JsonProperty("data")]
        public RischiDettaglioData Data { get; set; } = new RischiDettaglioData();

        /// <summary>Optional informational message for no_data responses.</summary>
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>Response metadata.</summary>
        [JsonProperty("meta")]
        public RischiDettaglioMeta Meta { get; set; } = new RischiDettaglioMeta();
    }

    /// <summary>
    /// Error payload for GET /v1/rischi/dettaglio.
    /// </summary>
    public class RischiDettaglioErrorResponse
    {
        [JsonProperty("error")]
        public RischiDettaglioError Error { get; set; } = new RischiDettaglioError();

        [JsonProperty("retry_after", NullValueHandling = NullValueHandling.Ignore)]
        public int? RetryAfter { get; set; }

        [JsonProperty("meta")]
        public RischiDettaglioMeta Meta { get; set; } = new RischiDettaglioMeta();
    }

    /// <summary>
    /// Error details for GET /v1/rischi/dettaglio.
    /// </summary>
    public class RischiDettaglioError
    {
        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// Detail payload with one row per cultivation exercise.
    /// </summary>
    public class RischiDettaglioData
    {
        [JsonProperty("numero_esercizi")]
        public int NumeroEsercizi { get; set; }

        [JsonProperty("esercizi")]
        public List<RischiDettaglioEsercizio> Esercizi { get; set; } = new List<RischiDettaglioEsercizio>();
    }

    /// <summary>
    /// Risk detail row for a single exercise.
    /// </summary>
    public class RischiDettaglioEsercizio
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("stato")]
        public string Stato { get; set; } = string.Empty;

        [JsonProperty("regione")]
        public string Regione { get; set; }

        [JsonProperty("centroide")]
        public RischiDettaglioCentroide Centroide { get; set; } = new RischiDettaglioCentroide();

        [JsonProperty("superficie_ha")]
        public double SuperficieHa { get; set; }

        [JsonProperty("rischio_gelo")]
        public double? RischioGelo { get; set; }

        [JsonProperty("rischio_siccita")]
        public double? RischioSiccita { get; set; }

        [JsonProperty("rischio_allagamento")]
        public double? RischioAllagamento { get; set; }
    }

    /// <summary>
    /// Geometry centroid wrapper for a detail row.
    /// </summary>
    public class RischiDettaglioCentroide
    {
        [JsonProperty("coordinate")]
        public string Coordinate { get; set; } = string.Empty;

        [JsonProperty("epsg")]
        public string Epsg { get; set; } = string.Empty;
    }

    /// <summary>
    /// Metadata for GET /v1/rischi/dettaglio responses.
    /// </summary>
    public class RischiDettaglioMeta
    {
        [JsonProperty("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [JsonProperty("request_id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonProperty("numero_righe_elaborate", NullValueHandling = NullValueHandling.Ignore)]
        public int? NumeroRigheElaborate { get; set; }
    }
}
