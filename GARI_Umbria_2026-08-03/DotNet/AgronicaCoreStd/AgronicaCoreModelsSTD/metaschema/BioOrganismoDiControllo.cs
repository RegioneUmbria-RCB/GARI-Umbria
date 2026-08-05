using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class BioOrganismoDiControllo : BaseCodeDescrStr
    {
        //public string codice { get; set; }
        //public string descrizione { get; set; }

        public BioOrganismoDiControllo(string codice) : base(codice,"")
        {

        }

        public BioOrganismoDiControllo(): base() { }
    }
}
