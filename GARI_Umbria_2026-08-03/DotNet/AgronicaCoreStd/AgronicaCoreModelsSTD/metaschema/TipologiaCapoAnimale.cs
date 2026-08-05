using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TipologiaCapoAnimale : BaseCodeDescr
    {
        public TipologiaCapoAnimale(int codice) : base(codice, "") { }

        public TipologiaCapoAnimale(int codice, string desc) : base(codice, desc) { }

        public TipologiaCapoAnimale() : base() { }
    }
}
