using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output
{
    public class MisuraXAvversitaOutput
    {
        public List<MisuraXAvv> ListaMisureXAvversita { get; set; }
        public string MessaggioErrore { get; set; }

        public MisuraXAvversitaOutput()
        {
            ListaMisureXAvversita = new List<MisuraXAvv>();
            MessaggioErrore = "";
        }
    }

    public class MisuraXAvv
    {
        [JsonProperty("Cod")] public int Cod { get; set; }
        [JsonProperty("Veg_Cod")] public int VegCod { get; set; }
        [JsonProperty("Av_Cod")] public int AvCod { get; set; }
        [JsonProperty("Av_Gru")] public int AvGru { get; set; }

        [JsonProperty("Udm_Cod")] public int UdmCod { get; set; }
        [JsonProperty("TipoControllo_Cod")] public int TipoControlloCod { get; set; }

        [JsonProperty("Av_Des_Vol")] public string AvDesVol { get; set; }
        [JsonProperty("Av_Des_Lat")] public string AvDesLat { get; set; }
        [JsonProperty("Av_Abbreviazione")] public string AvAbbreviazione { get; set; }
        [JsonProperty("Av_Gru_Des")] public string AvGruDes { get; set; }
        [JsonProperty("Av_Gru_Des_Lat")] public string AvGruDesLat { get; set; }

        [JsonProperty("Udm_Des")] public string UdmDes { get; set; }
        [JsonProperty("Udm_Sim")] public string UdmSim { get; set; }

        /// <summary>fondamentale</summary>
        [JsonProperty("Visibile")]
        public int Visibile { get; set; }

        [JsonProperty("FF_Cod")] public int FfCod { get; set; }
        [JsonProperty("Ordine")] public int Ordine { get; set; }

        [JsonProperty("Soglia")] public int Soglia { get; set; }

        [JsonProperty("pivasuperuser")] public string Pivasuperuser { get; set; } = "";

    }
}
