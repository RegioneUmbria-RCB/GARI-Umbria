using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.IoT
{
    public class DispositiviXSorgente
    {
        public string PIVA { get; set; }
        public int TipoSorgente { get; set; }
        public DistanzaDa DistanzaDa { get; set; }
        public List<ElencoDispositivi> ElencoDispositivi { get; set; }
    }

    public class DistanzaDa
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }

    public class ElencoDispositivi
    {
        public int TipoSorgente { get; set; }
        public List<Dispositivo> elenco_dispositivi { get; set; }
    }

    public class Dispositivo
    {
        public int dispositivo_cod { get; set; }
    }
}
