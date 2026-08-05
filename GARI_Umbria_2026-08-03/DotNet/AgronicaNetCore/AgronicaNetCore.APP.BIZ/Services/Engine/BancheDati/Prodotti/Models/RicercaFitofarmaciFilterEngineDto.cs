using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models
{
    /// <summary>
    /// Filtri per la ricerca avanzata FS024 dei fitofarmaci sull'engine Banche Dati.
    /// Campi obbligatori: <see cref="Data"/>, <see cref="CodiceSpecie"/>, <see cref="CodiceTipoAttivita"/>.
    /// </summary>
    public sealed class RicercaFitofarmaciFilterEngineDto
    {
        [JsonProperty("data")]
        public DateTime Data { get; init; }

        [JsonProperty("codice_specie")]
        public int? CodiceSpecie { get; init; }

        [JsonProperty("codice_tipo_attivita")]
        public int CodiceTipoAttivita { get; init; }

        [JsonProperty("codice_disciplinare")]
        public int? CodiceDisciplinare { get; init; }

        [JsonProperty("codice_visibilita_disciplinare")]
        public int? CodiceVisibilitaDisciplinare { get; init; }

        [JsonProperty("codice_raggruppamento_colturale")]
        public int? CodiceRaggruppamentoColturale { get; init; }

        [JsonProperty("codice_gruppo_avversita_infestante")]
        public int? CodiceGruppoAvversitaInfestante { get; init; }

        [JsonProperty("codice_avversita_infestante")]
        public int? CodiceAvversitaInfestante { get; init; }

        [JsonProperty("codice_epoca")]
        public int? CodiceEpoca { get; init; }

        [JsonProperty("testo_ricerca")]
        public string? TestoRicerca { get; init; }

        [JsonProperty("codice_finalita_produttiva")]
        public int? CodiceFinalitaProduttiva { get; init; }

        [JsonProperty("flag_filtro_prodotti_utilizzabili")]
        public int? FlagFiltroProdottiUtilizzabili { get; init; }

        [JsonProperty("elenco_comuni")]
        public List<string>? ElencoComuni { get; init; }

        [JsonProperty("flag_copertura")]
        public int? FlagCopertura { get; init; }

        [JsonProperty("codice_nazione")]
        public string? CodiceNazione { get; init; }

        [JsonProperty("codice_lingua")]
        public int? CodiceLingua { get; init; }

        [JsonProperty("codici_prodotti")]
        public List<int>? CodiciProdotti { get; init; }
    }
}
