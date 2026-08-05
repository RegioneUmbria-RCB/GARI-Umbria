using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models
{
    public sealed class TipologiaFertilizzanteEngineDto
    {
        [JsonProperty("codice_tipologia")]
        public int CodiceTipologia { get; init; }

        [JsonProperty("descrizione_tipologia")]
        public string? DescrizioneTipologia { get; init; }
    }

    public sealed class DittaFertilizzanteEngineDto
    {
        [JsonProperty("codice")]
        public int Codice { get; init; }

        [JsonProperty("descrizione")]
        public string? Descrizione { get; init; }
    }

    /// <summary>
    /// DTO che rappresenta un fertilizzante (FertilizzanteNonPaginatoDto) restituito dall'engine Banche Dati.
    /// </summary>
    public sealed class FertilizzanteEngineDto
    {
        [JsonProperty("codice_fertilizzante")]
        public int CodiceFertilizzante { get; init; }

        [JsonProperty("descrizione_fertilizzante")]
        public string? DescrizioneFertilizzante { get; init; }

        [JsonProperty("N")]
        public double N { get; init; }

        [JsonProperty("P2O5")]
        public double P2O5 { get; init; }

        [JsonProperty("K2O")]
        public double K2O { get; init; }

        [JsonProperty("MgO")]
        public double MgO { get; init; }

        [JsonProperty("Cu")]
        public double Cu { get; init; }

        [JsonProperty("codice_unita_misura")]
        public int CodiceUnitaMisura { get; init; }

        [JsonProperty("bio")]
        public int Bio { get; init; }

        [JsonProperty("elenco_tipologie")]
        public List<TipologiaFertilizzanteEngineDto>? ElencoTipologie { get; init; }

        [JsonProperty("elenco_ditte")]
        public List<DittaFertilizzanteEngineDto>? ElencoDitte { get; init; }
    }

}
