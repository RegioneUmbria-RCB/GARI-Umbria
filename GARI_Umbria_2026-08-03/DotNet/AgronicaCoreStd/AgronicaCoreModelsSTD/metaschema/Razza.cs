using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Razza : BaseCodeDescr
    {
        public Razza(int codice) : base(codice, "") { }

        public Razza(int codice, string desc) : base(codice, desc) { }

        public Razza() : base() { }
    }
}
