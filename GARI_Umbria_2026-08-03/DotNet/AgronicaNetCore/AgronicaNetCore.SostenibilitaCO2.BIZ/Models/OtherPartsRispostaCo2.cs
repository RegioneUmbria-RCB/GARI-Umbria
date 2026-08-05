using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Emissioni da fonti non agricole (energia, carburante aziendale, ecc.) restituite dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].other_parts[]</c>.
    /// I valori numerici sono in formato stringa come da API M4.
    /// </summary>
    public class OtherPartsRispostaCo2
    {
        [JsonPropertyName("scope_1")]
        public string? Scope1 { get; set; }

        [JsonPropertyName("scope_2_location_based")]
        public string? Scope2LocationBased { get; set; }

        [JsonPropertyName("scope_2_market_based")]
        public string? Scope2MarketBased { get; set; }

        [JsonPropertyName("scope_3")]
        public string? Scope3 { get; set; }

        [JsonPropertyName("biogenic_carbon")]
        public string? BiogenicCarbon { get; set; }

        [JsonPropertyName("ghg_location_based")]
        public string? GhgLocationBased { get; set; }

        [JsonPropertyName("ghg_market_based")]
        public string? GhgMarketBased { get; set; }

        [JsonPropertyName("indicators")]
        public IndicatoriOtherPartsRispostaCo2? Indicators { get; set; }
    }
}
