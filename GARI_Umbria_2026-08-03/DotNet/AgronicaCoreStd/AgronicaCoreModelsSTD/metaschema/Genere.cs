using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Genere : BaseCodeDescr
    {
        public Genere(int codice) : base(codice, "") { }

        public Genere(int codice, string desc) : base(codice, desc) { }

        public Genere() : base() { }
    }
}
