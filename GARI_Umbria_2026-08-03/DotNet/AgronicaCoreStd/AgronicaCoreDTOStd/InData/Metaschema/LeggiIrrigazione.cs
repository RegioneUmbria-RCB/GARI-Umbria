using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiIrrigazione
    {
        public Specie specie { get; set; }

        public LeggiIrrigazione()
        {
            specie = new Specie(0);
        }
    }
}
