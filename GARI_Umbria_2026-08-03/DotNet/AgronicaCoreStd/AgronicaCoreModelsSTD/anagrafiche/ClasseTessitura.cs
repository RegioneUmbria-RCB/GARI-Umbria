using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ClasseTessitura : BaseCodeDescr
    {

        public double peso_specifico { get; set; }
        public double peso20 { get; set; }
        public double peso30 { get; set; }
        public double peso50 { get; set; }

        public ClasseTessitura(int codice) : base(codice, "") { }
        public ClasseTessitura(int codice, string descrizione) : base(codice, descrizione) { }


        public ClasseTessitura() : base() { }

    }
}
