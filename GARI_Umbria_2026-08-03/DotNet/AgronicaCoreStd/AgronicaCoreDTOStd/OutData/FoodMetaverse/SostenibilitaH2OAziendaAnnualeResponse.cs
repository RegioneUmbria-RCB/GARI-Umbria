using Newtonsoft.Json;
using System.Collections.Generic;

namespace OutData.FoodMetaverse
{
    /// <summary>
    /// Response DTO for GET /sostenibilita_h2o/per_azienda_annuale.
    /// See DS09-API Endpoint GET Azienda Annuale Sostenibilita H2O, sezione "Formato Risposta - HTTP 200".
    /// </summary>
    public class SostenibilitaH2OAziendaAnnualeResponse
    {
        [JsonProperty("aziende")]
        public List<SostenibilitaH2OAziendaDto> Aziende { get; set; } = new List<SostenibilitaH2OAziendaDto>();

        [JsonProperty("metadata")]
        public SostenibilitaH2OMetadataDto Metadata { get; set; } = new SostenibilitaH2OMetadataDto();
    }

    public class SostenibilitaH2OAziendaDto
    {
        [JsonProperty("id_azienda")]
        public string IdAzienda { get; set; } = string.Empty;

        [JsonProperty("des_azienda")]
        public string DesAzienda { get; set; } = string.Empty;

        [JsonProperty("anni")]
        public List<SostenibilitaH2OAnnoDto> Anni { get; set; } = new List<SostenibilitaH2OAnnoDto>();
    }

    public class SostenibilitaH2OAnnoDto
    {
        [JsonProperty("anno")]
        public int Anno { get; set; }

        [JsonProperty("superficie_coltivata_ha")]
        public decimal SuperficieColtivataHa { get; set; }

        [JsonProperty("sostenibilita_h2o")]
        public SostenibilitaH2OValoriDto SostenibilitaH2O { get; set; } = new SostenibilitaH2OValoriDto();
    }

    public class SostenibilitaH2OValoriDto
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

    public class SostenibilitaH2OMetadataDto
    {
        [JsonProperty("timestamp_risposta")]
        public string TimestampRisposta { get; set; } = string.Empty;

        [JsonProperty("versione_algoritmo")]
        public string VersioneAlgoritmo { get; set; } = string.Empty;

        [JsonProperty("fonte_meteo")]
        public string FonteMeteo { get; set; } = string.Empty;

        [JsonProperty("fonte_benchmark")]
        public string FonteBenchmark { get; set; } = string.Empty;

        [JsonProperty("numero_anni_restituiti")]
        public int NumeroAnniRestituiti { get; set; }

        [JsonProperty("numero_aziende_restituiti")]
        public int NumeroAziendeRestituiti { get; set; }
    }
}
