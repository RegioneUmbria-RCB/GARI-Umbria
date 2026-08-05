using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class IndirizzoProduttivo : BaseCodeDescr
    {
        public IndirizzoProduttivo(int codice) : base(codice, "") { }

        public IndirizzoProduttivo(int codice, string desc) : base(codice, desc) { }

        public IndirizzoProduttivo() : base() { }
    }
}
