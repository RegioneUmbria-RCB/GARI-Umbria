using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input
{
    public class MisuraXAvversitaInput
    {
        [JsonProperty("Veg_Cod")]
        public int VegCod { get; set; }

        [JsonProperty("Av_Cod")]
        public int AvCod { get; set; }

        [JsonProperty("Av_Gru")]
        public int AvGru { get; set; }

        [JsonProperty("Dpi_Cod")]
        public int DpiCod { get; set; }

        [JsonProperty("Id_Rcdpi")]
        public int IdRcdpi { get; set; }

        [JsonProperty("Dpi_Pubblico_Privato")]
        public int DpiPubblicoPrivato { get; set; }

        [JsonProperty("TipoTestata")]
        public int TipoTestata { get; set; }

        [JsonProperty("SoloVisibili")]
        public bool SoloVisibili { get; set; } = true;

        [JsonProperty("strFiltro")]
        public string StrFiltro { get; set; } = string.Empty;

        [JsonProperty("strOrdinamento")]
        public string StrOrdinamento { get; set; } = string.Empty;

        [JsonProperty("Personalizzate")]
        public bool Personalizzate { get; set; }

        [JsonProperty("Piva_Superuser")]
        public string PivaSuperuser { get; set; } = string.Empty;

        [JsonProperty("Lingua_Cod")]
        public int LinguaCod { get; set; }

        [JsonProperty("Url")]
        public string Url { get; set; } = string.Empty;
        [JsonProperty("EstraiPersonalizzatePerAPP")]
        public bool EstraiPersonalizzatePerAPP { get; set; } = false;

    }
}
