using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Istat
    {

        public string reg { get; set; } = "000";
        public string prov { get; set; }
        public string com { get; set; }
        public string localita { get; set; }
        public string comuni_prov { get; set; }
        public string cap { get; set; }        

        public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// è il codice del comune come appare sulle penultime 4 posizioni del codice fiscale (prima del carattere di controllo), es: D704 = Forlì
        /// </summary>
        public string codiceBelfiore { get; set; }

        public Istat()
        {

        }
    }
}
