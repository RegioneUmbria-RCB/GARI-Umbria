using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Indicatori di sostenibilità CO2 a livello di impianto colturale restituiti dal motore M4.
    /// Riferimento spec: DS07-API — Risposta 200, array <c>aziende[].appezzamenti[].impianti[]</c>.
    /// I valori numerici sono in formato stringa come da API M4.
    /// </summary>
    public class ImpiantoRispostaCo2
    {
        [JsonPropertyName("id_impianto")]
        public string IdImpianto { get; set; } = string.Empty;

        [JsonPropertyName("id_coltura")]
        public string? IdColtura { get; set; }

        [JsonPropertyName("id_finalita")]
        public string? IdFinalita { get; set; }

        [JsonPropertyName("id_destinazione_uso")]
        public string? IdDestinazioneUso { get; set; }

        [JsonPropertyName("area_ha")]
        public string? AreaHa { get; set; }

        [JsonPropertyName("scope_1")]
        public string? Scope1 { get; set; }

        [JsonPropertyName("scope_3")]
        public string? Scope3 { get; set; }

        [JsonPropertyName("biogenic_carbon")]
        public string? BiogenicCarbon { get; set; }

        [JsonPropertyName("var_biomass_biogenic_carbon")]
        public string? VarBiomassBiogenicCarbon { get; set; }

        [JsonPropertyName("indicators")]
        public IndicatoriRispostaCo2? Indicators { get; set; }

        [JsonPropertyName("operazioni")]
        public List<OperazioneRispostaCo2> Operazioni { get; set; } = new();
    }
}
