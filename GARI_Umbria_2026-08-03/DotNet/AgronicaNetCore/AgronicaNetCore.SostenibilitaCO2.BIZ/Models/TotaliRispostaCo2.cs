using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Totali CO2eq per scope a livello aziendale restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].totali[]</c>.
    /// I valori sono stringhe numeriche come restituiti dall'API M4.
    /// </summary>
    public class TotaliRispostaCo2
    {
        [JsonPropertyName("CO2eq_tot_scope_1")]
        public string? CO2eqTotScope1 { get; set; }

        [JsonPropertyName("CO2eq_tot_scope_3")]
        public string? CO2eqTotScope3 { get; set; }

        [JsonPropertyName("CO2eq_tot_scope_2_location_based")]
        public string? CO2eqTotScope2LocationBased { get; set; }

        [JsonPropertyName("CO2eq_tot_scope_2_market_based")]
        public string? CO2eqTotScope2MarketBased { get; set; }

        [JsonPropertyName("CO2eq_tot_biogenic")]
        public string? CO2eqTotBiogenic { get; set; }

        [JsonPropertyName("CO2eq_tot_var_SOC")]
        public string? CO2eqTotVarSOC { get; set; }

        [JsonPropertyName("CO2eq_tot_var_biomass")]
        public string? CO2eqTotVarBiomass { get; set; }
    }
}
