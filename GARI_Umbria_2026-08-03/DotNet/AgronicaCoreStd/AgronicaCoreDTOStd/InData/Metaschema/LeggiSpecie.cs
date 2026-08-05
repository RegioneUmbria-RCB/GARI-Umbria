using System.Collections.Generic;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiSpecie
    {
        public bool cache { get; set; } = true;

        public AgronicaCoreModelsSTD.Gis.SementieriParametrizzazione ParametriSementieri { get; set; } = null;
        public IEnumerable<int> gruppiVegetali { get; set; } = new List<int>();
    }
}
