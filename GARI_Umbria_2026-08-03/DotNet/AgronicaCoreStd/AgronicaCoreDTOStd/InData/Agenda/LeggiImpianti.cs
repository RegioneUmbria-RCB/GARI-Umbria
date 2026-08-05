using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class LeggiImpianti
    {
        public string piva { get; set; }
        public int Sa_Cod { get; set; }
        public string Veg_Cod { get; set; }
        public DateTime Data_Inizio { get; set; }
        public DateTime Data_Fine { get; set; }
    }
}
