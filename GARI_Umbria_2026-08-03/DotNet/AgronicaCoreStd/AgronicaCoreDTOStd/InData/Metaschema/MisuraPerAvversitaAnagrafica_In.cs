using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class MisuraPerAvversitaAnagrafica_In
    {
        public List<MisuraPerAvversitaAnagrafica> MisureInsert { get; set; }
        public List<MisuraPerAvversitaAnagrafica> MisureUpdate { get; set; }
        public List<MisuraPerAvversitaAnagrafica> MisureDelete { get; set; }
    }
}
