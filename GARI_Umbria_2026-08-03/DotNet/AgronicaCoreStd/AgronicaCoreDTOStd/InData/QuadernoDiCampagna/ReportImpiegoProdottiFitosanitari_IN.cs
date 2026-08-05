using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InData.QuadernoDiCampagna
{
    public class ReportImpiegoProdottiFitosanitari_IN
    {
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        public List<string> Province { get; set; } = new List<string>();
        public bool FiltraPerProvince => Province.Any();
        public List<ComuneDatiIn> Comuni { get; set; } = new List<ComuneDatiIn>();
        public bool FiltraPerComuni => Comuni.Any();
        public bool FiltriTerritoriali => FiltraPerComuni || FiltraPerProvince;
        public int Pa_Cod { get; set; } = 0;
        public bool FiltraPerSostanzaAttiva => Pa_Cod != 0;
        public bool ZVN { get; set; }
        public bool IncludiFertilizzanti { get; set; }
        public bool VisualizzaProdotti { get; set; }
        public List<string> Imprese { get; set; } = new List<string>();
        public bool FiltraPerImprese => Imprese.Any();
        public bool InitialLoading { get; set; } = false;
    }

    public class ComuneDatiIn
    {
        public string PROV { get; set; }
        public string COM { get; set; }
    }
}
