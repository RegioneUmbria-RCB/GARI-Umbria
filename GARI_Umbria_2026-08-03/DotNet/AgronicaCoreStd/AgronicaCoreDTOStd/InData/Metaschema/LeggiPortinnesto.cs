using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiPortinnesto
    {
        public Specie specie { get; set; }

        public LeggiPortinnesto()
        {
            specie = new Specie(0);
        }
    }
}
