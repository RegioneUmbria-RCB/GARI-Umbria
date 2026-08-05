using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input
{
    public class FasiFenologicheInput
    {
        [JsonProperty("Veg_Cod")]
        public int VegCod { get; set; }

        [JsonProperty("FF_Cod")]
        public int FfCod { get; set; }

        [JsonProperty("strFiltro")]
        public string StrFiltro { get; set; } = "";

        [JsonProperty("strOrdinamento")]
        public string StrOrdinamento { get; set; } = "";

        [JsonProperty("SoloFioritura")]
        public bool SoloFioritura { get; set; }

        [JsonProperty("SoloVisibili")]
        public bool SoloVisibili { get; set; } = true;

        [JsonProperty("SoloRipresaVegetativa")]
        public bool SoloRipresaVegetativa { get; set; }

        [JsonProperty("Personalizzate")]
        public bool Personalizzate { get; set; }

        [JsonProperty("Piva_Superuser")]
        public string PivaSuperuser { get; set; } = "";

        [JsonProperty("Lingua_Cod")]
        public int LinguaCod { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; } = "";
        [JsonProperty("EstraiPersonalizzatePerAPP")]
        public bool EstraiPersonalizzatePerAPP { get; set; } = false;
    }
}
