using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class GetTrattamentiZooDto
    {
        public string Piva { get; set; }
        public int CodCentro { get; set; }
        public int CodStalla { get; set; }
        public int CodRaggruppamento { get; set; }
        public int CodAnimale { get; set; }
        public string Matricola { get; set; }
        public DateTime DataInizio { get; set; }
        public DateTime DataFine { get; set; }
        [DefaultValue(null)]
        public List<int> ListaCodAnimali { get; set; } = null;
        [DefaultValue(null)]
        public List<int> FarmCatList { get; set; } = null;
        [DefaultValue(null)]
        public List<int> FarmCatSemplList { get; set; } = null;
    }
}
