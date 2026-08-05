using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class BioTipoAttivita : BaseCodeDescrStr
    {
        //public string codice { get; set; }
        //public string descrizione { get; set; }

        public BioTipoAttivita(string codice) : base(codice,"")
        {

        }

        public BioTipoAttivita() { }
    }
}
