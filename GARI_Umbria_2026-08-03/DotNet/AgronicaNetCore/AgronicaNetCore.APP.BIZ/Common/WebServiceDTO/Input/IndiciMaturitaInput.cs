using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Input
{
    public class IndiciMaturitaInput
    {
        [JsonProperty("ind_mat_cod")]
        public int IndMatCod { get; set; }

        [JsonProperty("udm_cod")]
        public int UdmCod { get; set; }

        [JsonProperty("veg_cod")]
        public int VegCod { get; set; }

        /// <summary>INDICI_MATURITA = 0 - INDICI_RESE_RACCOLTA = 1</summary>
        [JsonProperty("tipoTestata")]
        public int TipoTestata { get; set; }

        [JsonProperty("lav_cod")]
        public int LavCod { get; set; }

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

        [JsonProperty("Lingua_Cod")]
        public int LinguaCod { get; set; }

        [JsonProperty("FiltraSpecie")]
        public bool FiltraSpecie { get; set; } = true;

        [JsonProperty("joinPersonalizzate")]
        public bool JoinPersonalizzate { get; set; } = true;
        [JsonProperty("EstraiPersonalizzatePerAPP")]
        public bool EstraiPersonalizzatePerAPP { get; set; } = false;
    }
}
