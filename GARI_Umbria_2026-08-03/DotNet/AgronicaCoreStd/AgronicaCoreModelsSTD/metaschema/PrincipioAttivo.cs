using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class PrincipioAttivo: BaseCodeDescr
    {
        public decimal titolo { get; set; }
        public decimal peso { get; set; }

        public decimal percentualeSuperficieTrattabile { get; set; }

        public PrincipioAttivo(int codice) : base(codice, "")
        {
        }
        public PrincipioAttivo() : base() { }
    }
}


