using System;
using System.Collections.Generic;
using System.Text;

namespace InData.OperazioniZoo
{
    public class OperazioniAgendaInput
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Sta_Num { get; set; }
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }
    }
}
