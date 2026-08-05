using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class FinalitaMacchina : BaseCodeDescr
    {
        public FinalitaMacchina(int codice) : base(codice, "")
        {
        }

        public FinalitaMacchina() : base(-1, "")
        {
        }
    }
}
