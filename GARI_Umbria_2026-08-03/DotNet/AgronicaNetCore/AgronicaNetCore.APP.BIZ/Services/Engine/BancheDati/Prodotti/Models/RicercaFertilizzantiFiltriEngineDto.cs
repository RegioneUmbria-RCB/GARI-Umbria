using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models
{
    /// <summary>
    /// Filtri per la ricerca dei fertilizzanti sull'engine Banche Dati (RicercaFertilizzantiFiltriDto).
    /// </summary>
    public sealed class RicercaFertilizzantiFiltriEngineDto
    {
        [JsonProperty("data")]
        public DateTime Data { get; init; }

        [JsonProperty("codice_tipo_attivita")]
        public int CodiceTipoAttivita { get; init; }

        [JsonProperty("codice_fertilizzante")]
        public int? CodiceFertilizzante { get; init; }

        [JsonProperty("descrizione_fertilizzante")]
        public string? DescrizioneFertilizzante { get; init; }

        [JsonProperty("codice_disciplinare")]
        public int? CodiceDisciplinare { get; init; }

        [JsonProperty("codice_regolamento")]
        public int? CodiceRegolamento { get; init; }

        [JsonProperty("includi_apporti")]
        public bool IncludiApporti { get; init; }

        [JsonProperty("includi_tipologie")]
        public bool IncludiTipologie { get; init; }

        [JsonProperty("includi_ditte")]
        public bool IncludiDitte { get; init; }

        [JsonProperty("codice_nazione")]
        public string? CodiceNazione { get; init; }

        [JsonProperty("codici_prodotti")]
        public List<int>? CodiciProdotti { get; init; }
    }
}
