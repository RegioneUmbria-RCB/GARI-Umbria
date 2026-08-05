using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Rappresenta la struttura JSON di risposta del motore M4 (campo <c>json_risposta</c>
    /// della tabella <c>Lookup_Sost_CO2_Aziendale_Payload</c>), passata come input
    /// al servizio di creazione token Blockchain.
    /// Riferimento spec: DS10-BL CreaTokenBlockchain — Input, campo <c>payload_response_m4</c>.
    /// </summary>
    public class SostenibilitaCO2LookupAziendaleResponse
    {
        /// <summary>Data inizio periodo di calcolo CO2 (ISO 8601 date, da risposta M4).</summary>
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        /// <summary>Data fine periodo di calcolo CO2 (ISO 8601 date, da risposta M4).</summary>
        [JsonPropertyName("end_date")]
        public string EndDate { get; set; } = string.Empty;

        /// <summary>Elenco degli indicatori per azienda restituiti dal motore M4.</summary>
        [JsonPropertyName("aziende")]
        public List<SostenibilitaCO2LookupAziendaleAziendaResponse> Aziende { get; set; } = new List<SostenibilitaCO2LookupAziendaleAziendaResponse>();
    }

    /// <summary>Indicatori di sostenibilità CO2 a livello aziendale, estratti dalla risposta M4.</summary>
    public class SostenibilitaCO2LookupAziendaleAziendaResponse
    {
        /// <summary>Partita IVA o CUAA dell'azienda.</summary>
        [JsonPropertyName("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        /// <summary>
        /// Variazione di carbonio biogenico nel suolo SOC (Kg CO2 equiv).
        /// Stringa come da risposta M4.
        /// </summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public string VarSocSoilBiogenicCarbon { get; set; }

        /// <summary>Elenco appezzamenti con relativi indicatori.</summary>
        [JsonPropertyName("appezzamenti")]
        public List<SostenibilitaCO2LookupAziendaleAppezzamentoResponse> Appezzamenti { get; set; } = new List<SostenibilitaCO2LookupAziendaleAppezzamentoResponse>();
    }

    /// <summary>Indicatori di sostenibilità CO2 a livello di appezzamento, estratti dalla risposta M4.</summary>
    public class SostenibilitaCO2LookupAziendaleAppezzamentoResponse
    {
        /// <summary>Identificativo appezzamento (eco dalla risposta M4).</summary>
        [JsonPropertyName("id_appezzamento")]
        public string IdAppezzamento { get; set; } = string.Empty;

        /// <summary>Superficie in ettari dell'appezzamento (stringa come da API M4).</summary>
        [JsonPropertyName("area_ha")]
        public string AreaHa { get; set; }

        /// <summary>Variazione carbonio biogenico SOC per l'appezzamento (stringa come da API M4).</summary>
        [JsonPropertyName("var_soc_soil_biogenic_carbon")]
        public string VarSocSoilBiogenicCarbon { get; set; }
    }
}
