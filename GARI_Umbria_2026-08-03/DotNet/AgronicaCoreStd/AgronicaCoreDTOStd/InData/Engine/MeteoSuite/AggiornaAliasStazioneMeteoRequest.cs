using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Engine.MeteoSuite
{
    public class AggiornaAliasStazioneMeteoRequest
    {
        public string PIVA { get; set; }
        public int IdStazione { get; set; }
        public string NomeAlias { get; set; }
    }
}
