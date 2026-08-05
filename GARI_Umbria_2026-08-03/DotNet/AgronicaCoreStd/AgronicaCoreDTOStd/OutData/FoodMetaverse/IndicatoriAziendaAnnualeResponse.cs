using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Response DTO for <c>GET /v1/api/sostenibilita/indicatori-azienda-annuale</c>.
    /// See DS08-API: FS2.08.1 Sost. CO2| API| Indicatori per Azienda Annuale (Design Specification 01KHXAFCB1651AWBC2Y884DNTR).
    /// </summary>
    public class IndicatoriAziendaAnnualeResponse
    {
        /// <summary>PIVA dell'azienda.</summary>
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        /// <summary>Indicatori per ogni anno disponibile, ordinati per anno decrescente.</summary>
        [JsonProperty("anni")]
        public List<IndicatoriAnnoDto> Anni { get; set; } = new List<IndicatoriAnnoDto>();
    }

    /// <summary>Indicatori di sostenibilità CO2 per un singolo anno di campagna.</summary>
    public class IndicatoriAnnoDto
    {
        /// <summary>Anno di riferimento della campagna (YYYY).</summary>
        [JsonProperty("anno")]
        public int Anno { get; set; }

        /// <summary>Emissioni GHG Scope 1 (kg CO2 equivalente).</summary>
        [JsonProperty("ghg_scope1_kg_co2_equiv")]
        public decimal? GhgScope1KgCo2Equiv { get; set; }

        /// <summary>Emissioni GHG Scope 2 - Location Based (kg CO2 equivalente).</summary>
        [JsonProperty("ghg_scope2_kg_co2_equiv")]
        public decimal? GhgScope2KgCo2Equiv { get; set; }

        /// <summary>Emissioni GHG Scope 3 (kg CO2 equivalente).</summary>
        [JsonProperty("ghg_scope3_kg_co2_equiv")]
        public decimal? GhgScope3KgCo2Equiv { get; set; }

        /// <summary>Carbonio biogenico totale (kg CO2 equivalente).</summary>
        [JsonProperty("ghg_biogenic_carbon_kg_co2_equiv")]
        public decimal? GhgBiogenicCarbonKgCo2Equiv { get; set; }

        /// <summary>Variazione SOC biogenico del suolo (kg CO2 equivalente).</summary>
        [JsonProperty("var_soc_soil_biogenic_carbon_kg_co2_equiv")]
        public decimal? VarSocSoilBiogenicCarbonKgCo2Equiv { get; set; }

        /// <summary>Variazione biomassa biogenica (kg CO2 equivalente).</summary>
        [JsonProperty("var_biomass_biogenic_carbon_kg_co2_equiv")]
        public decimal? VarBiomassBiogenicCarbonKgCo2Equiv { get; set; }

        /// <summary>Superficie totale coltivata (ettari), somma delle aree degli appezzamenti.</summary>
        [JsonProperty("superficie_coltivata_ha")]
        public decimal? SuperficieColtivataHa { get; set; }
    }
}
