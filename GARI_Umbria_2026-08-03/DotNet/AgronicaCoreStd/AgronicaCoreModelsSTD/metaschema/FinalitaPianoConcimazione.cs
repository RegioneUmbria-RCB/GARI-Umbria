using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class FinalitaPianoConcimazione : BaseCodeDescr
    {
        public FinalitaPianoConcimazione(int codice) : base(codice,"")
        {
        }
        public FinalitaPianoConcimazione() : base(0, "")
        {
        }
    }
}
