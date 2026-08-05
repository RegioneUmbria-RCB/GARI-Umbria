using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Provincia : BaseCodeDescrStr
    {
        public CodiciNazioniISO3166 stato { get; set; }
        public Regione regione { get; set; }
        public string sigla { get; set; }
        public string comuneDefault { get; set; }

        public Provincia(string codice) : base(codice, "")
        {

        }

        public Provincia() : base() { }

    }
}
