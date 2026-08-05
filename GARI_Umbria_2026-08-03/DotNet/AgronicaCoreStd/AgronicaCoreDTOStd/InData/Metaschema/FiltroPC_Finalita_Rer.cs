using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class FiltroPC_Finalita_Rer
    {
        public Regolamenti regolamento { get; set; }

        public Specie specie { get; set; }

        public GruppoFinalita finalita { get; set; }
    }
}
