using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Audit
{
    public class ChecklistEUDR
    {
        public int codice { get; set; }
        public int stato { get; set; }
        public DateTime data { get; set; }
        public DateTime data_scadenza { get; set; }
        public string cod_fornitore { get; set; }
        public string fornitore { get; set; }
        public int score { get; set; }
        public int pubblica { get; set; }
        public string link { get; set; }
        public string note { get; set; }

        public ChecklistEUDR()
        {
            codice = 0;
            stato = 0;
            data = new DateTime(1900, 1, 1);
            data_scadenza = new DateTime(2100, 12, 31);
        }

    }
}
