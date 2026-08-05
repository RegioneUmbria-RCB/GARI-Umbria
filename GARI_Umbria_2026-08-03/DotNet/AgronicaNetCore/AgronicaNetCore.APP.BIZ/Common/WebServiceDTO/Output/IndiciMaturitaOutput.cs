using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output
{
    public class IndiciMaturitaOutput
    {
        [JsonProperty("ListaIndiciMaturita")]
        public List<IndiciMaturita> ListaIndiciMaturita { get; set; } = new();

        [JsonProperty("MessaggioErrore")]
        public string MessaggioErrore { get; set; } = "";
    }

    public class IndiciMaturita
    {
        [JsonProperty("ind_mat_cod")]
        public int IndMatCod { get; set; }

        [JsonProperty("ind_mat_des")]
        public string IndMatDes { get; set; } = "";

        [JsonProperty("veg_cod")]
        public int VegCod { get; set; }

        [JsonProperty("veg_des")]
        public string VegDes { get; set; } = "";

        [JsonProperty("gru_cod")]
        public int GruCod { get; set; }

        [JsonProperty("grsp_cod")]
        public int GrspCod { get; set; }

        [JsonProperty("udm_cod")]
        public int UdmCod { get; set; }

        [JsonProperty("udm_des")]
        public string UdmDes { get; set; } = "";

        [JsonProperty("udm_sim")]
        public string UdmSim { get; set; } = "";

        [JsonProperty("reg_cod")]
        public int RegCod { get; set; }

        [JsonProperty("classe")]
        public string Classe { get; set; } = "";

        [JsonProperty("flag_raccolta")]
        public int FlagRaccolta { get; set; }

        [JsonProperty("lav_cod")]
        public int LavCod { get; set; }

        [JsonProperty("pivasuperuser")]
        public string Pivasuperuser { get; set; } = "";
    }
}
