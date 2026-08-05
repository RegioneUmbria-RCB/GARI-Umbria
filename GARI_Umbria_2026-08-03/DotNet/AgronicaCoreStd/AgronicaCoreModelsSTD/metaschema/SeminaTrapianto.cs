using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class SeminaTrapianto : BaseCodeDescrStr
    {

        //public string codice { get; set; }
        //public string descrizione { get; set; }

        public enum Tipo
        {
            nonSpecificato = -1,
            semina = 0,
            trapianto = 1
        }

        public SeminaTrapianto(string codice) : base(codice,"")
        {
        }

        public SeminaTrapianto() : base()
        {

        }
    }
}
