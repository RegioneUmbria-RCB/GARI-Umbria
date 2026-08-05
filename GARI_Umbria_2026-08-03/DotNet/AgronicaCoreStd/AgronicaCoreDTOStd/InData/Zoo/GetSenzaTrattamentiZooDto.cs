using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class GetSenzaTrattamentiZooDto
    {
        public string Piva { get; set; }
        public int CodCentro { get; set; }
        public int CodStalla { get; set; }
        public int CodRaggruppamento { get; set; }
        public int CodAnimale { get; set; }
        public DateTime Data { get; set; }
        public int GiorniSenzaTrattamento { get; set; }
        [DefaultValue(null)]
        public List<int> FarmCatList { get; set; } = null;
        [DefaultValue(null)]
        public List<int> FarmCatSemplList { get; set; } = null;
        public bool MostraGGInizioTotali { get; set; } = true;
        public bool MostraAnomalie { get; set; } = false;
    }
}
