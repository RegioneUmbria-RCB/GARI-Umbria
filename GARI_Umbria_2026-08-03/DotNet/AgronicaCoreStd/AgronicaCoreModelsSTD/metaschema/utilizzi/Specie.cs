using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.utilizzi
{
    public class Specie : BaseCodeDescr
    {
        public Specie(int codice) : base(codice, "") { }

        public Specie(int codice, string desc) : base(codice, desc) { }

        public Specie(): base() { }

    }
}
