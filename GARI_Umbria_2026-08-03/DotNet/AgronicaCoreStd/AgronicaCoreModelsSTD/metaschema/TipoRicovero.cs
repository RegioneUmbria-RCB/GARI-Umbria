using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class TipoRicovero : BaseCodeDescr
    {
        public TipoRicovero(int codice) : base(codice, "")
        {

        }

        public TipoRicovero() : base() { }
    }
}
