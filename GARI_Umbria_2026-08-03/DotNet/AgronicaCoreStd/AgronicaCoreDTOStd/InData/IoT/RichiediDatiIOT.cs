using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.IoT
{
    public class RichiediDatiIOT
    {
        public int TipoSorgente { get; set; }
        public int Sorgente { get; set; }
        public DateTime dataInizio { get; set; }
        public DateTime dataFine { get; set; }
        public string frequenzaDati { get; set; }
        public string LinguaCodiceISO { get; set; }
    }
}
