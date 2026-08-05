using System.Text.Json.Serialization;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models
{
    public class EsitoConsiglioNutrizione
    {
        [JsonPropertyName("results")]
        public RisultatiConsiglioNutrizione? Results { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    public class RisultatiConsiglioNutrizione
    {
        [JsonPropertyName("outcome")]
        public OutcomeConsiglioNutrizione? Outcome { get; set; }

        [JsonPropertyName("partial")]
        public object? Partial { get; set; }
    }

    public class OutcomeConsiglioNutrizione
    {
        [JsonPropertyName("consigli")]
        public List<ConsiglioNutrizione> Consigli { get; set; } = new();

        [JsonPropertyName("messaggi")]
        public List<string> Messaggi { get; set; } = new();
    }

    public class ConsiglioNutrizione
    {
        [JsonPropertyName("elemento")]
        public string Elemento { get; set; } = string.Empty;

        [JsonPropertyName("fabbisognoMinimo")]
        public double? FabbisognoMinimo { get; set; }

        [JsonPropertyName("fabbisognoMassimo")]
        public double? FabbisognoMassimo { get; set; }

        [JsonPropertyName("doseConsigliataMinima")]
        public double? DoseConsigliataMinima { get; set; }

        [JsonPropertyName("doseConsigliataMassima")]
        public double? DoseConsigliataMassima { get; set; }

        [JsonPropertyName("quantitativoGiaPresente")]
        public double? QuantitativoGiaPresente { get; set; }

        [JsonPropertyName("quantitativoMinimoResiduo")]
        public double? QuantitativoMinimoResiduo { get; set; }

        [JsonPropertyName("quantitativoMassimoResiduo")]
        public double? QuantitativoMassimoResiduo { get; set; }

        [JsonPropertyName("elencoDistribuzioniPrecedenti")]
        public List<DistribuzionePrecedente> ElencoDistribuzioniPrecedenti { get; set; } = new();
    }

    public class DistribuzionePrecedente
    {
        [JsonPropertyName("data")]
        public string Data { get; set; } = string.Empty;

        [JsonPropertyName("quantita")]
        public double Quantita { get; set; }

        [JsonPropertyName("faseFenologicaBBCH")]
        public string FaseFenologicaBBCH { get; set; } = string.Empty;
    }
}
