using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class FiltroCalcoloNPK
    {
        public RegolamentoConcimazione regolamento { get; set; }
        public Specie specie { get; set; }
        public FinalitaPianoConcimazione finalita { get; set; }
        public FaseCicloColturale stato { get; set; }
    }
}
