using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Comune : BaseCodeDescrStr
    {
        public Provincia provincia { get; set; }
        public string cap { get; set; }
        public string codiceCatastale { get; set; }

        public Comune() : base() { }

        public Comune(string codice) : base(codice, "") { }

        public Comune(string codice, string codiceProvincia) : base(codice, "")
        {
            provincia = new Provincia(codiceProvincia);
        }

    }
}
