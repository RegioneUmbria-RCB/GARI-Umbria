using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class OrientamentoTecnicoEconomico : BaseCodeDescrStr
    {
        //public string codice { get; set; }
        //public string descrizione { get; set; }

        public OrientamentoTecnicoEconomico(string codice) : base(codice,"")
        {

        }

        public OrientamentoTecnicoEconomico()
        {

        }
    }
}
