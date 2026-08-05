using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema.utilizzi
{
    public class GruppoFinalita : BaseCodeDescr
    {
        public int specieCod { get; set; }

        public GruppoFinalita(int Codice) : base(Codice,"") {}
        public GruppoFinalita(int Codice, string Descrizione) : base(Codice, Descrizione) {}

        public GruppoFinalita() : base() { }

    }
}
