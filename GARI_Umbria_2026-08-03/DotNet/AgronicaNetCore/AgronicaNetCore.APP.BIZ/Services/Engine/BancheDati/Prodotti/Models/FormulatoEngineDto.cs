using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Services.Engine.BancheDati.Prodotti.Models
{
    public sealed class ClassificazioneFormulatoEngineDto
    {
        [JsonProperty("codice")]
        public int Codice { get; init; }

        [JsonProperty("descrizione")]
        public string? Descrizione { get; init; }
    }

    public sealed class SostanzaAttivaFormulatoEngineDto
    {
        [JsonProperty("codice_sostanza_attiva")]
        public int CodiceSostanzaAttiva { get; init; }

        [JsonProperty("descrizione_sostanza_attiva")]
        public string? DescrizioneSostanzaAttiva { get; init; }

        [JsonProperty("titolo")]
        public decimal Titolo { get; init; }

        [JsonProperty("peso")]
        public decimal Peso { get; init; }
    }

    public sealed class PeriodoSospensioneFormulatoEngineDto
    {
        [JsonProperty("data_inizio_sospensione")]
        public DateTime? DataInizioSospensione { get; init; }

        [JsonProperty("data_fine_sospensione")]
        public DateTime? DataFineSospensione { get; init; }
    }

    public sealed class EpocaBloccoFormulatoEngineDto
    {
        [JsonProperty("codice_epoca_blocco_da")]
        public int CodiceEpocaBloccoDA { get; init; }

        [JsonProperty("codice_epoca_blocco_a")]
        public int CodiceEpocaBloccoA { get; init; }

        [JsonProperty("progressivo")]
        public int Progressivo { get; init; }
    }

    /// <summary>
    /// DTO che rappresenta un formulato (fitofarmaco) restituito dall'endpoint
    /// POST /api/v1/prodotti/fitofarmaci dell'engine Banche Dati.
    /// </summary>
    public sealed class FormulatoEngineDto
    {
        [JsonProperty("codice_formulato")]
        public int CodiceFitofarmaco { get; init; }

        [JsonProperty("descrizione_formulato")]
        public string? DescrizioneFitofarmaco { get; init; }

        [JsonProperty("codici_classificazione")]
        public List<int>? CodiciClassificazione { get; init; }

        [JsonProperty("elenco_classificazioni")]
        public List<ClassificazioneFormulatoEngineDto>? ElencoClassificazioni { get; init; }

        [JsonProperty("codici_sostanze_attive")]
        public List<int>? CodiciSostanzeAttive { get; init; }

        [JsonProperty("sostanze_attive")]
        public List<SostanzaAttivaFormulatoEngineDto>? SostanzeAttive { get; init; }

        [JsonProperty("periodi_sospensione")]
        public List<PeriodoSospensioneFormulatoEngineDto>? PeriodISospensione { get; init; }

        [JsonProperty("elenco_epoche_blocco")]
        public List<EpocaBloccoFormulatoEngineDto>? ElencoEpocheBlocco { get; init; }

        [JsonProperty("bio")]
        public int Bio { get; init; }

        [JsonProperty("polverulento")]
        public int Polverulento { get; init; }

        [JsonProperty("in_sospensione_alla_data_richiesta")]
        public bool InSospensioneAllaDataRichiesta { get; init; }

        [JsonProperty("data_registrazione")]
        public DateTime? DataRegistrazione { get; init; }

        [JsonProperty("data_revoca")]
        public DateTime? DataRevoca { get; init; }

        [JsonProperty("data_termine")]
        public DateTime? DataTermine { get; init; }

        [JsonProperty("data_fine_commercializzazione")]
        public DateTime? DataFineCommercializzazione { get; init; }

        [JsonProperty("data_fine_uso_scorte")]
        public DateTime? DataFineUsoScorte { get; init; }

        [JsonProperty("descrizione_formulato_precedente")]
        public string? DescrizioneFormulatoPrecedente { get; init; }

        [JsonProperty("flag_copertura")]
        public int FlagCopertura { get; init; }

        [JsonProperty("codice_modalita_impiego")]
        public int CodiceModalitaImpiego { get; init; }

        [JsonProperty("codice_formulato_per_specie")]
        public int CodiceFormulatoPerSpecie { get; init; }

        [JsonProperty("tempo_carenza")]
        public int TempoCarenza { get; init; }

        [JsonProperty("carenza")]
        public int Carenza { get; init; }

        [JsonProperty("buffer_zone_min")]
        public decimal BufferZoneMin { get; init; }

        [JsonProperty("buffer_zone_max")]
        public decimal BufferZoneMax { get; init; }

        [JsonProperty("durata_feromone")]
        public int DurataFeromone { get; init; }

        [JsonProperty("data_atto_normativo")]
        public DateTime? DataAttoNormativo { get; init; }

        [JsonProperty("codice_identificativo_atto_normativo")]
        public int CodiceIdentificativoAttoNormativo { get; init; }
    }
}
