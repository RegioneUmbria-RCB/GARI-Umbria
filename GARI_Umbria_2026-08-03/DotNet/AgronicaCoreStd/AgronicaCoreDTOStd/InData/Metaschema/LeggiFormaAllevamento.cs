using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiFormaAllevamento
    {
        public Specie specie { get; set; }

        public LeggiFormaAllevamento()
        {
            specie = new Specie(0);
        }
    }
}
