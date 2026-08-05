using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Response DTO for POST /sostenibilita_h2o/sintesi_per_lotti_raccolti.
    /// See DS11-API Endpoint POST Sintesi Lotti Raccolti Sostenibilita H2O, section "Formato Risposta - HTTP 200".
    /// </summary>
    public class SostenibilitaH2OLottiRaccoltiResponse
    {
        [JsonProperty("lotti_elaborati")]
        public List<SostenibilitaH2OLottoElaboratoDto> LottiElaborati { get; set; } = new List<SostenibilitaH2OLottoElaboratoDto>();

        [JsonProperty("lotti_non_trovati")]
        public List<SostenibilitaH2OLottoNonTrovatoDto> LottiNonTrovati { get; set; } = new List<SostenibilitaH2OLottoNonTrovatoDto>();

        [JsonProperty("metadata")]
        public SostenibilitaH2OLottiRaccoltiMetadataDto Metadata { get; set; } = new SostenibilitaH2OLottiRaccoltiMetadataDto();
    }

    public class SostenibilitaH2OLottoElaboratoDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("cod_lotto")]
        public string CodLottoFmp { get; set; } = string.Empty;

        [JsonProperty("sostenibilita_h2o")]
        public SostenibilitaH2OLottoValoriDto SostenibilitaH2O { get; set; } = new SostenibilitaH2OLottoValoriDto();

        [JsonProperty("superficie_coltivata_ha")]
        public decimal SuperficieColtivataHa { get; set; }

        [JsonProperty("numero_esercizi_contribuenti")]
        public int NumeroEserciziContribuenti { get; set; }

        [JsonProperty("dettaglio_esercizi")]
        public List<SostenibilitaH2ODettaglioEsercizioDto> DettaglioEsercizi { get; set; } = new List<SostenibilitaH2ODettaglioEsercizioDto>();
    }

    public class SostenibilitaH2OLottoValoriDto
    {
        [JsonProperty("fabbisogno_m3")]
        public decimal FabbisognoM3 { get; set; }

        [JsonProperty("consumi_m3")]
        public decimal ConsumiM3 { get; set; }

        [JsonProperty("da_meteo_m3")]
        public decimal DaMeteoM3 { get; set; }

        [JsonProperty("delta_m3")]
        public decimal DeltaM3 { get; set; }
    }

    public class SostenibilitaH2ODettaglioEsercizioDto
    {
        [JsonProperty("esercizio_id")]
        public string EsercizioId { get; set; } = string.Empty;

        [JsonProperty("azienda")]
        public string Azienda { get; set; } = string.Empty;

        [JsonProperty("fabbisogno_m3_per_ha")]
        public decimal FabbisognoM3PerHa { get; set; }

        [JsonProperty("consumi_m3_per_ha")]
        public decimal ConsumiM3PerHa { get; set; }

        [JsonProperty("da_meteo_m3_per_ha")]
        public decimal DaMeteoM3PerHa { get; set; }

        [JsonProperty("delta_m3_per_ha")]
        public decimal DeltaM3PerHa { get; set; }
    }

    public class SostenibilitaH2OLottoNonTrovatoDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("cod_lotto")]
        public string CodLottoFmp { get; set; } = string.Empty;

        [JsonProperty("motivo")]
        public string Motivo { get; set; } = string.Empty;
    }

    public class SostenibilitaH2OLottiRaccoltiMetadataDto
    {
        [JsonProperty("timestamp_risposta")]
        public string TimestampRisposta { get; set; } = string.Empty;

        [JsonProperty("numero_lotti_richiesti")]
        public int NumeroLottiRichiesti { get; set; }

        [JsonProperty("numero_lotti_trovati")]
        public int NumeroLottiTrovati { get; set; }

        [JsonProperty("numero_lotti_non_trovati")]
        public int NumeroLottiNonTrovati { get; set; }
    }
}
