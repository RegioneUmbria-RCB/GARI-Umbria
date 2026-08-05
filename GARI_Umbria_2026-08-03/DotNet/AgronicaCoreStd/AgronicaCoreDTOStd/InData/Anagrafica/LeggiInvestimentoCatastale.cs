using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiInvestimentoCatastale
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centro { get; set; }
        public ParticelleCatastali particella { get; set; }
        public Impianto impianto { get; set; }
        public DateTime data { get; set; }

    }
}
