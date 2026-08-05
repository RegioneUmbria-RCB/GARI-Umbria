using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class FiltroFinalita2
    {
        public Specie specie { get; set; }
        public GruppoFinalita finalita { get; set; }
        public RegolamentoConcimazione regolamentoConcimazione { get; set; }

    }
}
