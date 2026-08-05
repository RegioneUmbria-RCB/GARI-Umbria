using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Audit
{
    public class ScriviChecklistEUDR
    {
        public string cod_fornitore { get; set; }
        public DateTime data { get; set; }
        public int pubblica { get; set; }
        public string email { get; set; }
        public string note { get; set; }
    }
}
