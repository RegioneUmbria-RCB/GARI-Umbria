using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Effluente : BaseCodeDescr
    {
        public UnitaDiMisura udm { get; set; }
        public TipoEffluente tipoEffluente { get; set; }
        public decimal carico { get; set; }
        public decimal N { get; set; }

        public Effluente(int codice) : base(codice, "")
        {

        }


        public Effluente() : base() { }

    }
}
