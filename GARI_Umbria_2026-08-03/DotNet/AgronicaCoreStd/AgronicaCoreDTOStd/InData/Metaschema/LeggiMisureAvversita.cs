using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiMisureAvversita
    {
        public string piva { get; set; }
        public int vegCod { get; set; }
        public Disciplinare dpi { get; set; }
        public int udmCod { get; set; }
        public int avvCod { get; set; }

    }
}
