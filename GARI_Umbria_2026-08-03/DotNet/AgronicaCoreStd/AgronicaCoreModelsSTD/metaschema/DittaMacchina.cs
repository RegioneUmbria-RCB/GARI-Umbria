using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class DittaMacchina : Ditta
    {
        public DittaMacchina(int codice) : base(codice)
        {
        }

        public DittaMacchina() { }
    }
}
