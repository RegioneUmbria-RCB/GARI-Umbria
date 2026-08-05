using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiFinalita
    {
        public Specie specie { get; set; }

        public List<int> listaSpecie { get; set; }

        public bool cache { get; set; } = true;

        public AgronicaCoreModelsSTD.Gis.SementieriParametrizzazione ParametriSementieri { get; set; } = null;
    }
}
