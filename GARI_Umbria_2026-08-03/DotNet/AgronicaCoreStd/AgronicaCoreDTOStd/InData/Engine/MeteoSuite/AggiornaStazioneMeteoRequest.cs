using System.Collections.Generic;

namespace InData.Engine.MeteoSuite
{
    public class AggiornaStazioneMeteoRequest
    {
        public class Sensore
        {
            public int Id { get; set; }
            public string Descrizione  { get; set; }
        }

        public string PIVASuperUser { get; set; }
        public string PIVA { get; set; }
        public int IdStazione { get; set; }
        public string Descrizione { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public List<Sensore> Sensori {  get; set; }
    }
}
