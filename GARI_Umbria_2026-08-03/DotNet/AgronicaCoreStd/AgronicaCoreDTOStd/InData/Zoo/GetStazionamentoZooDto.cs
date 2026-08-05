using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class GetStazionamentoZooDto
    {
        public string Piva { get; set; }
        public int CodCentro { get; set; }
        public int CodStalla { get; set; }
        public int CodRaggruppamento { get; set; }
        public int CodAnimale { get; set; }
        public DateTime Data { get; set; }
        public int GiorniStazionamento { get; set; }
        [DefaultValue(null)]
        public List<int> ListaCodAnimali { get; set; } = null;
    }
}
