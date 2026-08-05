using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Audit
{
    public class LeggiChecklistEUDR
    {
        public string cod_fornitore { get; set; }

        public LeggiChecklistEUDR(string cod_fornitore)
        {
            this.cod_fornitore = cod_fornitore;
        }
    }
}
