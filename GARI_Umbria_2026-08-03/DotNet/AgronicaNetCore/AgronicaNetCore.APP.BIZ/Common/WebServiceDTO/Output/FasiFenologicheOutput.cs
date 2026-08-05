using Newtonsoft.Json;

namespace AgronicaNetCore.APP.BIZ.Common.WebServiceDTO.Output
{
    public class FasiFenologicheOutput
    {
        [JsonProperty("ListaFasiFenologiche")]
        public List<FaseFenologica> ListaFasiFenologiche { get; set; } = new();

        [JsonProperty("MessaggioErrore")]
        public string MessaggioErrore { get; set; } = "";
    }

    public class FaseFenologica
    {
        [JsonProperty("Cod_SS")]
        public int CodSs { get; set; }

        [JsonProperty("Veg_Cod")]
        public int VegCod { get; set; }

        [JsonProperty("ID_BBCH")]
        public int IdBbch { get; set; }

        [JsonProperty("Descrizione")]
        public string Descrizione { get; set; } = "";

        [JsonProperty("FF_Cod")]
        public int FfCod { get; set; }

        [JsonProperty("Visibile")]
        public int Visibile { get; set; }

        [JsonProperty("Fioritura")]
        public int Fioritura { get; set; }

        [JsonProperty("Stadio")]
        public string Stadio { get; set; } = "";

        [JsonProperty("RipresaVegetativa")]
        public int RipresaVegetativa { get; set; }

        [JsonProperty("RipresaVegetativa_GG")]
        public int RipresaVegetativaGg { get; set; }

        [JsonProperty("RipresaVegetativa_MM")]
        public int RipresaVegetativaMm { get; set; }

        [JsonProperty("pivasuperuser")]
        public string Pivasuperuser { get; set; } = "";
    }
}
