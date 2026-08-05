using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiGruppoVarietale
    {
        public Specie specie { get; set; }
        public AgronicaCoreModelsSTD.Gis.SementieriParametrizzazione ParametriSementieri { get; set; } = null;
    }
}

