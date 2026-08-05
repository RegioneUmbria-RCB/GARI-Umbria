using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class EliminaLettureContatori
    {
        public int id_lettura { get; set; }
        public string piva { get; set; }
        public int id_contatore { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}
