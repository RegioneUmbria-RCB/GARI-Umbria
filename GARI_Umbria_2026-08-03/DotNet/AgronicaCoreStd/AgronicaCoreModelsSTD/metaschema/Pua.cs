using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Pua : BaseCodeDescr
    {

        public Disciplinare disciplinare { get; set; }


        public Pua(int codice) : base(codice, "")
        {

        }


        public Pua() : base() { }

    }
}
