using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParcoMacchineCaratteristiche
    {
        public int codice { get; set; }
        public CaratteristicaMacchina caratteristica { get; set; }

        public string valore { get; set; }
        public DateTime validita_inizio { get; set; }
        public DateTime validita_fine { get; set; }
    }
}
