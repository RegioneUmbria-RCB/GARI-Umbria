using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TipoTarga : BaseCodeDescr
    {
        public TipoTarga(int codice) : base(codice, "")
        {
        }

        public TipoTarga() : base(-1, "")
        {
        }
    }
}
