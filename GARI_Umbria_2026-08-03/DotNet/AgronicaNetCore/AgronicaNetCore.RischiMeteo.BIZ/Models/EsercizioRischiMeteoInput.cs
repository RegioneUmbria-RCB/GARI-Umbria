using Newtonsoft.Json;

namespace AgronicaNetCore.RischiMeteo.BIZ.Models
{
    /// <summary>
    /// Identificativi di un Esercizio colturale selezionato per la costruzione del payload M2.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Input, esercizi_selezionati[].
    /// </summary>
    public class EsercizioRischiMeteoInput
    {
        /// <summary>Codice univoco azienda (CUAA).</summary>
        [JsonProperty("cuaa_azienda")]
        public string cuaa_azienda { get; set; } = string.Empty;

        /// <summary>Partita IVA dell'azienda.</summary>
        [JsonProperty("piva_azienda")]
        public string piva_azienda { get; init; } = string.Empty;

        /// <summary>Codice stabilimento (Sa_Cod) dell'appezzamento.</summary>
        [JsonProperty("sa_cod")]
        public int sa_cod { get; init; } = 0;

        /// <summary>Codice appezzamento (Appezza).</summary>
        [JsonProperty("id_appezzamento")]
        public int id_appezzamento { get; init; }

        /// <summary>Identificativo dell'Esercizio colturale (Progetto_Cod in Imprese_Progetti).</summary>
        [JsonProperty("id_esercizio")]
        public int id_esercizio { get; init; }

        /// <summary>Identificativo dell'impianto (Id_Reg in Reg_Impianti), usato come fallback DAL.</summary>
        [JsonProperty("id_impianto")]
        public int id_impianto { get; init; }

        /// <summary>Anno dell'annata agraria di riferimento.</summary>
        //[JsonPropertyName("anno_esercizio")]
        //public int AnnoEsercizio { get; init; }

        /// <summary>Data di inizio dell'esercizio colturale.</summary>
        [JsonProperty("data_inizio_esercizio")]
        public string data_inizio_esercizio { get; init; }

        /// <summary>Data di fine dell'esercizio colturale.</summary>
        [JsonProperty("data_fine_esercizio")]
        public string data_fine_esercizio { get; init; }

        [JsonProperty("cod_specie")]
        public int? cod_specie { get; init; }

        [JsonProperty("cod_varieta")]
        public int? cod_varieta { get; init; }

        [JsonProperty("nazione")]
        public string nazione { get; init; } = string.Empty;

        [JsonProperty("regione")]
        public string regione { get; init; } = string.Empty;

        [JsonProperty("istat_reg")]
        public string? istat_reg { get; set; }

        [JsonProperty("nome_varieta")]
        public string nome_varieta { get; init; } = string.Empty;
        
        [JsonProperty("nome_esercizio")]
        public string nome_esercizio { get; init; } = string.Empty; 
        
        [JsonProperty("nome_azienda")]
        public string nome_azienda { get; init; } = string.Empty;   
        
        [JsonProperty("nome_appezzamento")]
        public string nome_appezzamento { get; init; } = string.Empty;  

        [JsonProperty("superficie_ha")]
        public decimal superficie_ha { get; init; }

        [JsonProperty("nome_specie")]
        public string nome_specie { get; init; } = string.Empty;
    }
}
