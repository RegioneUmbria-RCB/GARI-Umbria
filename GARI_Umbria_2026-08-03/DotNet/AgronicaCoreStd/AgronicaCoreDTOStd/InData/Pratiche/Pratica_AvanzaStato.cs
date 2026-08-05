using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Pratiche
{
    public class Pratica_AvanzaStato
    {
        public string piva { get; set; }
        public string pratica_cod { get; set; }
        public int servizio_cod { get; set; }
        public int statoFinaleRichiesto { get; set; }
        public string note { get; set; }
    }
}