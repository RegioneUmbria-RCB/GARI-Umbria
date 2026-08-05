using AgronicaCoreModelsSTD.attivita;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiIAF
    {

        public Disciplinare disciplinare { get; set; }

        public Specie specie { get; set; }

        public DateTime data { get; set; }

        public bool privato { get; set; }

    }
}
