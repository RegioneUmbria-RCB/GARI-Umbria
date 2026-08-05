using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class AnalisiTipo : BaseCodeDescr
    {
        public AnalisiTipo(int codice) : base(codice, "") { }
        public AnalisiTipo(int codice, string descrizione) : base(codice, descrizione) { }
        public AnalisiTipo() { }
    }
}
