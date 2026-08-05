using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class LeggiRapportoSpecifico
    {
        public string piva { get; set; }
        public Boolean rapportoAttivo { get; set; }
        public Boolean cliente { get; set; }
        public Boolean fornitore { get; set; }
        public Boolean dipendente { get; set; }
        public Boolean terzista { get; set; }
        public Boolean legale { get; set; }
        public Boolean agente { get; set; }
        public Boolean consulente { get; set; }
        public string Cod_Contatto { get; set; }
     
    }
}
