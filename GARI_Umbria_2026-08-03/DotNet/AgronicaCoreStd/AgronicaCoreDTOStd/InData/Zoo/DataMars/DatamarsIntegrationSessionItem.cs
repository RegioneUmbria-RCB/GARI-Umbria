using Newtonsoft.Json;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Rappresenta un singolo elemento nella risposta paginata di
    /// <c>GET /farms/{farmId}/integrationSessions</c>.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI â€” Step 2 Interrogazione API IntegrationSessions;
    /// Architectural diagram (sezione risposta DataMars API).</para>
    /// </summary>
    public sealed class DatamarsIntegrationSessionItem
    {
        /// <summary>Identificativo univoco della sessionIntegration.</summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Nome descrittivo della sessione.</summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Numero di record presenti nella sessione secondo l'API.</summary>
        [JsonProperty("recordCount")]
        public int RecordCount { get; set; }

        /// <summary>Timestamp di creazione della sessione (ISO 8601 UTC).</summary>
        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }
}
