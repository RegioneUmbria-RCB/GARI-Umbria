using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di sostenibilità CO2 a livello di operazione colturale restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].appezzamenti[].impianti[].operazioni[]</c>.
    /// I valori numerici sono in formato stringa come da API M4.
    /// </summary>
    public class OperazioneRispostaCo2
    {
        [JsonPropertyName("id_operazione")]
        public string IdOperazione { get; set; } = string.Empty;

        [JsonPropertyName("data_operazione")]
        public string? DataOperazione { get; set; }

        [JsonPropertyName("tipo_operazione")]
        public string? TipoOperazione { get; set; }

        [JsonPropertyName("area_ha")]
        public string? AreaHa { get; set; }

        [JsonPropertyName("scope_1")]
        public string? Scope1 { get; set; }

        [JsonPropertyName("scope_3")]
        public string? Scope3 { get; set; }

        [JsonPropertyName("biogenic_carbon")]
        public string? BiogenicCarbon { get; set; }

        [JsonPropertyName("indicators")]
        public IndicatoriRispostaCo2? Indicators { get; set; }
    }
}
