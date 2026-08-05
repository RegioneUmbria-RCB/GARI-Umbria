using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Macchine : BaseCodeDescrStr
    {
        //public string codice { get; set; }
        //public string descrizione { get; set; }

        public Macchine(string codice) : base(codice, "")
        {

        }

        public Macchine() : base("", "")
        {
        }
    }
}
