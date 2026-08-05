using Newtonsoft.Json;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.PerimetroRaccolti
{
    /// <summary>
    /// Singola riga del perimetro: combinazione univoca Azienda + Appezzamento + Impianto + Esercizio.
    /// Riferimento spec: DS01-BL CaricamentoPerimetroFiltrato — Modello di risposta.
    /// </summary>
    public class PerimetroRigaItem
    {
        [JsonProperty("piva_azienda")]
        public string PivaAzienda { get; set; } = string.Empty;

        [JsonProperty("nome_azienda")]
        public string NomeAzienda { get; set; } = string.Empty;

        [JsonProperty("id_appezzamento")]
        public int IdAppezzamento { get; set; }

        [JsonProperty("nome_appezzamento")]
        public string NomeAppezzamento { get; set; } = string.Empty;

        [JsonProperty("id_esercizio")]
        public int IdEsercizio { get; set; }

        [JsonProperty("id_impianto")]
        public int IdImpianto { get; set; }

        /// <summary>Nazione in formato ISO 3166-1 alpha-3 (es. "ITA").</summary>
        [JsonProperty("nazione")]
        public string Nazione { get; set; } = string.Empty;

        [JsonProperty("regione")]
        public string Regione { get; set; } = string.Empty;

        [JsonProperty("istat_reg")]
        public string IstatReg { get; set; } = string.Empty;

        [JsonProperty("superficie_ha")]
        public decimal SuperficieHa { get; set; }

        [JsonProperty("cod_specie")]
        public int? CodSpecie { get; set; }

        [JsonProperty("nome_specie")]
        public string NomeSpecie { get; set; } = string.Empty;

        [JsonProperty("cod_varieta")]
        public int? CodVarieta { get; set; }

        [JsonProperty("nome_varieta")]
        public string NomeVarieta { get; set; } = string.Empty;

        [JsonProperty("data_inizio_esercizio")]
        public DateTime? DataInizioEsercizio { get; set; }

        [JsonProperty("data_fine_esercizio")]
        public DateTime? DataFineEsercizio { get; set; }

        [JsonProperty("nome_esercizio")]
        public string NomeEsercizio { get; set; } = string.Empty;

        [JsonProperty("sa_cod")]
        public int sa_cod { get; set; }
    }
}
