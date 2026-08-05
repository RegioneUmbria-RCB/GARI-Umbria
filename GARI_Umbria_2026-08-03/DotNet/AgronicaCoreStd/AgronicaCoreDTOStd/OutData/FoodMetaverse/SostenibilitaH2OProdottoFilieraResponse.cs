using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Response DTO for GET /sostenibilita_h2o/per_prodotto_di_filiera.
    /// See DS10-API Endpoint GET Prodotto Filiera Sostenibilita H2O, section "Formato Risposta - HTTP 200".
    /// </summary>
    public class SostenibilitaH2OProdottoFilieraResponse
    {
        [JsonProperty("aziende")]
        public List<SostenibilitaH2OProdottoAziendaDto> Aziende { get; set; } = new List<SostenibilitaH2OProdottoAziendaDto>();

        [JsonProperty("metadata")]
        public SostenibilitaH2OProdottoMetadataDto Metadata { get; set; } = new SostenibilitaH2OProdottoMetadataDto();
    }

    public class SostenibilitaH2OProdottoAziendaDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("des_azienda")]
        public string DesAzienda { get; set; } = string.Empty;

        [JsonProperty("anni")]
        public List<SostenibilitaH2OProdottoAnnoDto> Anni { get; set; } = new List<SostenibilitaH2OProdottoAnnoDto>();
    }

    public class SostenibilitaH2OProdottoAnnoDto
    {
        [JsonProperty("anno")]
        public int Anno { get; set; }

        [JsonProperty("superficie_coltivata_ha")]
        public decimal SuperficieColtivataHa { get; set; }

        [JsonProperty("sostenibilita_h2o")]
        public SostenibilitaH2OProdottoValoriDto SostenibilitaH2O { get; set; } = new SostenibilitaH2OProdottoValoriDto();
    }

    public class SostenibilitaH2OProdottoValoriDto
    {
        [JsonProperty("fabbisogno_m3_per_ha")]
        public decimal FabbisognoM3PerHa { get; set; }

        [JsonProperty("consumi_m3_per_ha")]
        public decimal ConsumiM3PerHa { get; set; }

        [JsonProperty("da_meteo_m3_per_ha")]
        public decimal DaMeteoM3PerHa { get; set; }

        [JsonProperty("delta_m3_per_ha")]
        public decimal DeltaM3PerHa { get; set; }
    }

    public class SostenibilitaH2OProdottoMetadataDto
    {
        [JsonProperty("timestamp_risposta")]
        public string TimestampRisposta { get; set; } = string.Empty;

        [JsonProperty("cod_prodotto")]
        public string CodProdottoFmp { get; set; } = string.Empty;

        [JsonProperty("numero_aziende_produttrici")]
        public int NumeroAziendeProduttrici { get; set; }

        [JsonProperty("numero_anni_totali")]
        public int NumeroAnniTotali { get; set; }
    }
}