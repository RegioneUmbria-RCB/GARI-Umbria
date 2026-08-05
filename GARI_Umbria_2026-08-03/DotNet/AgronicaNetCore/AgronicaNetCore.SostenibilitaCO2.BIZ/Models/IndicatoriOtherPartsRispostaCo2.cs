using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di dettaglio per le emissioni da fonti non agricole (other_parts).
    /// Riferimento spec: DS07-API — Risposta 200, <c>aziende[].other_parts[].indicators</c>.
    /// </summary>
    public class IndicatoriOtherPartsRispostaCo2
    {
        [JsonPropertyName("CO2_fuel_scope_1")]
        public string? CO2FuelScope1 { get; set; }

        [JsonPropertyName("CO2_fuel_production_scope_3")]
        public string? CO2FuelProductionScope3 { get; set; }

        [JsonPropertyName("CO2_fuel_biogenic_carbon")]
        public string? CO2FuelBiogenicCarbon { get; set; }

        [JsonPropertyName("CO2_electricity_market_based_scope_2")]
        public string? CO2ElectricityMarketBasedScope2 { get; set; }

        [JsonPropertyName("CO2_electricity_location_based_scope_2")]
        public string? CO2ElectricityLocationBasedScope2 { get; set; }
    }
}
