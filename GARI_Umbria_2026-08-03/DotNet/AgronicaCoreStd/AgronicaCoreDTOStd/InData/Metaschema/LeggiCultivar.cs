using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiCultivar
    {
        public Specie specie { get; set; }

        public List<int> listaSpecie { get; set; }

        public bool cache { get; set; } = true;

        public LeggiCultivar()
        {
            specie = new Specie(0);
        }
    }
}
