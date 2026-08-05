using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Pratiche
{
    public class GeneraPratica
    {
        public string piva { get; set; }
        public int servizio_cod { get; set; }
        public DateTime Data_Inizio { get; set; }
        public DateTime Data_Fine { get; set; }
        public DateTime Data_Inizio_Pratica { get; set; }
        public string Numero { get; set; }
    }
}
