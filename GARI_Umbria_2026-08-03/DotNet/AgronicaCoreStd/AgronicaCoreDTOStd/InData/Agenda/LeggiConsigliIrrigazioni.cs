using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiConsigliIrrigazioni
    {
        public DateTime data { get; set; }
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public int appezza { get; set; }
        public int id_reg { get; set; }
        public int progetto_cod { get; set; }
    }
}
