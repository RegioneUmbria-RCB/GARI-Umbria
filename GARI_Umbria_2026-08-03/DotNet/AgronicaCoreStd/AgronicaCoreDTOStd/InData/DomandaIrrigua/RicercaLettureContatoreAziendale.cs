using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class RicercaLettureContatoreAziendale
    {
        public string piva { get; set; }
        public int id_contatore { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
