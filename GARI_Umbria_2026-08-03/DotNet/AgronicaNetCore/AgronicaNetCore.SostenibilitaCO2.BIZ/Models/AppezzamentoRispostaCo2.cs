using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di sostenibilità CO2 a livello di appezzamento restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].appezzamenti[]</c>.
    /// </summary>
    public class AppezzamentoRispostaCo2
    {
        /// <summary>Identificativo appezzamento (PIVA|Sa_Cod|Appezza).</summary>
        [JsonPropertyName("id_appezzamento")]
        public string IdAppezzamento { get; set; } = string.Empty;

        /// <summary>Superficie in ettari dell'appezzamento.</summary>
        [JsonPropertyName("area_ha")]
        public string? AreaHa { get; set; }

        /// <summary>Emissioni Scope 1 (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("scope_1")]
        public string? Scope1 { get; set; }

        /// <summary>Emissioni Scope 3 (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("scope_3")]
        public string? Scope3 { get; set; }

        /// <summary>Carbonio biogenico (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("biogenic_carbon")]
        public string? BiogenicCarbon { get; set; }

        /// <summary>Variazione carbonio biogenico SOC (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public string? VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>Variazione carbonio biogenico biomassa (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("var_biomass_biogenic_carbon")]
        public string? VarBiomassBiogenicCarbon { get; set; }

        /// <summary>Emissioni GHG totali dell'appezzamento (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("ghg")]
        public string? Ghg { get; set; }

        /// <summary>Rimozioni totali dell'appezzamento (Kg CO2 equiv, stringa come da API M4).</summary>
        [JsonPropertyName("removals")]
        public string? Removals { get; set; }

        /// <summary>Indicatori di dettaglio a livello di appezzamento.</summary>
        [JsonPropertyName("indicators")]
        public IndicatoriRispostaCo2? Indicators { get; set; }

        /// <summary>Dettaglio indicatori per impianto colturale.</summary>
        [JsonPropertyName("impianti")]
        public List<ImpiantoRispostaCo2> Impianti { get; set; } = new();
    }
}
