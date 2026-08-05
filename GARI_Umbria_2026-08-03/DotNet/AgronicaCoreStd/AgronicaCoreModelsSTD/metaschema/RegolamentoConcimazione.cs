using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class RegolamentoConcimazione : BaseCodeDescr
    {
        public int tipo { get; set; }

        public RegolamentoConcimazione(int codice) : base(codice, "")
        {

        }

        public RegolamentoConcimazione() : base(0, "")
        {

        }

    }
}
