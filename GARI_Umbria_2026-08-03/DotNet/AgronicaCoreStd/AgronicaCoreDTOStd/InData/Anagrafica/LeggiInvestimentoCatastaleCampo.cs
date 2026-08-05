using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiInvestimentoCatastaleCampo
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centro { get; set; }
        public ParticelleCatastali particella { get; set; }
        public Campo campo { get; set; }
        public DateTime data { get; set; }

    }
}
