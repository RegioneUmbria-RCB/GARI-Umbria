using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class FaseFenologica: BaseCodeDescr
    {
        public FaseFenologica() : base() { }

        public FaseFenologica(int codice) : base(codice, "") { }

        public BaseCodeDescr stadioCrescitaBBCH { get; set; }

        public Specie specieVegetale { get; set; }

        public string stadio { get; set; }

        public bool fioritura { get; set; }
    }
}
