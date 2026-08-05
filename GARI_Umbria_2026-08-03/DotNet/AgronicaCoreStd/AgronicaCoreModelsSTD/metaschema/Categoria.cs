using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Categoria : BaseCodeDescr
    {
        public Categoria(int codice) : base(codice, "")
        {

        }

        public Categoria() : base() { }
    }
}
