using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Regione : BaseCodeDescrStr
    {
        public Regione(string codice) : base(codice, "")
        {

        }

        public Regione() : base() { }

    }
}
