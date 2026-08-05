using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di dettaglio CO2 a livello di appezzamento e impianto restituiti dal motore M4.
    /// Usato sia in <c>aziende[].appezzamenti[].indicators</c>
    /// sia in <c>aziende[].appezzamenti[].impianti[].indicators</c>
    /// sia in <c>aziende[].appezzamenti[].impianti[].operazioni[].indicators</c>.
    /// I valori sono stringhe numeriche o null come da API M4.
    /// </summary>
    public class IndicatoriRispostaCo2
    {
        [JsonPropertyName("CO2_fuel_scope_1")]
        public string? CO2FuelScope1 { get; set; }

        [JsonPropertyName("CO2_fuel_production_scope_3")]
        public string? CO2FuelProductionScope3 { get; set; }

        [JsonPropertyName("CO2_fuel_biogenic_carbon")]
        public string? CO2FuelBiogenicCarbon { get; set; }

        [JsonPropertyName("CO2_liming_scope_1")]
        public string? CO2LimingScope1 { get; set; }

        [JsonPropertyName("CO2_urea_scope_1")]
        public string? CO2UreaScope1 { get; set; }

        [JsonPropertyName("CO2_fertilizers_production_scope_3")]
        public string? CO2FertilizersProductionScope3 { get; set; }

        [JsonPropertyName("CO2_agrochemicals_production_scope_3")]
        public string? CO2AgrochemicalsProductionScope3 { get; set; }

        [JsonPropertyName("CO2_seeds_production_scope_3")]
        public string? CO2SeedsProductionScope3 { get; set; }

        [JsonPropertyName("N2O_soil_scope_1")]
        public string? N2OSoilScope1 { get; set; }

        [JsonPropertyName("CH4_biomass_burning_scope_1")]
        public string? CH4BiomassBurningScope1 { get; set; }

        [JsonPropertyName("N2O_biomass_burning_scope_1")]
        public string? N2OBiomassBurningScope1 { get; set; }

        [JsonPropertyName("CO2_biomass_burning_biogenic_carbon")]
        public string? CO2BiomassBurningBiogenicCarbon { get; set; }

        [JsonPropertyName("var_soc_soil_scope_1")]
        public string? VarSocSoilScope1 { get; set; }

        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public string? VarSocSoilBiogenicCarbon { get; set; }

        [JsonPropertyName("var_biomass_scope_1")]
        public string? VarBiomassScope1 { get; set; }

        [JsonPropertyName("var_biomass_biogenic_carbon")]
        public string? VarBiomassBiogenicCarbon { get; set; }

        [JsonPropertyName("CH4_rice_scope_1")]
        public string? CH4RiceScope1 { get; set; }
    }
}
