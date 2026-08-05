using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output
{
    public class MisuraXDanniRaccoltaOutput
    {
        [JsonProperty("ListaMisureXDanniRaccolta")]
        public List<MisuraXDR> ListaMisureXDanniRaccolta { get; set; } = new();

        [JsonProperty("MessaggioErrore")]
        public string MessaggioErrore { get; set; } = "";
    }

    public class MisuraXDR
    {
        [JsonProperty("Veg_Cod")]
        public int VegCod { get; set; }

        [JsonProperty("Dr_Cod")]
        public int DrCod { get; set; }

        [JsonProperty("Udm_Cod")]
        public int UdmCod { get; set; }

        [JsonProperty("Dr_Des")]
        public string DrDes { get; set; } = "";

        [JsonProperty("Udm_Des")]
        public string UdmDes { get; set; } = "";

        [JsonProperty("Udm_Sim")]
        public string UdmSim { get; set; } = "";

        /// <summary>fondamentale</summary>
       
        [JsonProperty("Visibile")]
        public int Visibile { get; set; }

        [JsonProperty("pivasuperuser")]
        public string Pivasuperuser { get; set; } = "";

    }
}
