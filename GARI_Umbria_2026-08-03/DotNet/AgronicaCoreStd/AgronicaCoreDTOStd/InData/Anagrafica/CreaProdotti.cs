using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class CreaProdotti
    {
        public Varieta varieta { get; set; }

        public Regolamenti regolamenti { get; set; }

        public Boolean creaSemente { get; set; }

        public Boolean creaTrasformatoVegetale { get; set; }
    }
}
