using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Risposta HTTP 200 del motore esterno M4 (Engine Sostenibilità CO2).
    /// Contiene metadati di elaborazione e gli indicatori CO2 per ogni azienda.
    /// Riferimento spec: DS07-API — Risposta 200.
    /// </summary>
    public class RispostaEngineCo2
    {
        /// <summary>Identificativo univoco elaborazione (UUID assegnato da M4).</summary>
        [JsonPropertyName("id_elaborazione")]
        public string IdElaborazione { get; set; } = string.Empty;

        /// <summary>Timestamp di elaborazione in formato ISO 8601.</summary>
        [JsonPropertyName("timestamp_elaborazione")]
        public string TimestampElaborazione { get; set; } = string.Empty;

        /// <summary>Indicatori CO2 per ogni azienda inclusa nel payload inviato.</summary>
        [JsonPropertyName("aziende")]
        public List<AziendaRispostaCo2> Aziende { get; set; } = new();

        /// <summary>Warning non bloccanti restituiti dal motore M4.</summary>
        [JsonPropertyName("warnings")]
        public List<string> Warnings { get; set; } = new();

        /// <summary>Errori restituiti dal motore M4.</summary>
        [JsonPropertyName("errors")]
        public List<string> Errors { get; set; } = new();
    }
}
