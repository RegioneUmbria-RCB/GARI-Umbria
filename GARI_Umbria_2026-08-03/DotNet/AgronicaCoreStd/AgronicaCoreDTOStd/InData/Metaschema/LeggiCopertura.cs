using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiCopertura
    {
        public Specie specie { get; set; }

        public LeggiCopertura()
        {
            specie = new Specie(0);
        }
    }
}
