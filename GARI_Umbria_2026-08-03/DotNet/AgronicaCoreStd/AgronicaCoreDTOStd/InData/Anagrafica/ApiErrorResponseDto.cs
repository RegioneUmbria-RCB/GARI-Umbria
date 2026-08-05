using Newtonsoft.Json;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    /// <summary>
    /// Structured error body returned by the Food Metaverse Platform API endpoints
    /// for all 4xx and 5xx HTTP responses.
    /// </summary>
    /// <remarks>
    /// Design Specification DS03-API: Endpoint Query Cono di Visibilità Utente — Risposte (400, 403, 404, 500).
    /// All datetime values in <see cref="Timestamp"/> use ISO 8601 UTC format (yyyy-MM-ddTHH:mm:ssZ).
    /// </remarks>
    public class ApiErrorResponseDto
    {
        [JsonProperty("error")]
        public string Error { get; set; } = string.Empty;

        [JsonProperty("error_code")]
        public string ErrorCode { get; set; } = string.Empty;

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("details")]
        public object Details { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; } = string.Empty;
    }
}
