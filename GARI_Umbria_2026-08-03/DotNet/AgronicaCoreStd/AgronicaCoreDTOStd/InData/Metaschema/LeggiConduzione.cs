using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiConduzione
    {
        public Specie specie { get; set; }

        public LeggiConduzione()
        {
            specie = new Specie(0);
        }
    }
}
