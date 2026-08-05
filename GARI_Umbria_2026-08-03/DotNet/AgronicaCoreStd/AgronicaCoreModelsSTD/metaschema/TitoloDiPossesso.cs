using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TitoloDiPossesso : BaseCodeDescr
    {
        public TitoloDiPossesso(int codice) : base(codice,"")
        {
        }

        public TitoloDiPossesso() : base(-1, "")
        {
        }

    }
}
