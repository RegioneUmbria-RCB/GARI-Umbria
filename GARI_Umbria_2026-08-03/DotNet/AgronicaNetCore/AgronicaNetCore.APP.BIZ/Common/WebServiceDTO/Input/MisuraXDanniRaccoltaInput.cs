using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input
{
    public class MisuraXDanniRaccoltaInput
    {
        [JsonProperty("Veg_Cod")]
        public int VegCod { get; set; }

        [JsonProperty("Dr_Cod")]
        public int DrCod { get; set; }

        [JsonProperty("SoloVisibili")]
        public bool SoloVisibili { get; set; } = true;

        [JsonProperty("strFiltro")]
        public string StrFiltro { get; set; } = "";

        [JsonProperty("strOrdinamento")]
        public string StrOrdinamento { get; set; } = "";

        [JsonProperty("Personalizzate")]
        public bool Personalizzate { get; set; }

        [JsonProperty("Piva_Superuser")]
        public string PivaSuperuser { get; set; } = "";

        [JsonProperty("Url")]
        public string Url { get; set; } = "";

        [JsonProperty("FiltraSpecie")]
        public bool FiltraSpecie { get; set; } = true;

        [JsonProperty("joinPersonalizzate")]
        public bool JoinPersonalizzate { get; set; } = true;

        [JsonProperty("EstraiPersonalizzatePerAPP")]
        public bool EstraiPersonalizzatePerAPP { get; set; } = false;

    }
}
