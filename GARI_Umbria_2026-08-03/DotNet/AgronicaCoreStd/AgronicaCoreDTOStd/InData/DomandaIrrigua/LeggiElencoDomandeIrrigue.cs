using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class LeggiElencoDomandeIrrigue
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<string> elencoPiva { get; set; }
    }

    public class RiepilogoDomandeIrrigue
    {
        public int id { get; set; }
        public string piva { get; set; }
        public string pivaReale { get; set; }
        public string ragionesociale { get; set; }
        public int anno { get; set; }
        public decimal superficietotale { get; set; }
    }
}
