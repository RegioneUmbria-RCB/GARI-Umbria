using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.contabilita
{
    public class RegistrazioneContabile : BaseCodeDescr
    {
        // movimento.doc_numero-->codice
        // movimento.mov_desc-->descrizione

        // movimento.doc_numero_sin
        public string rifDocumento { get; set; }

        // movimento.doc_numero_des
        public string anno { get; set; }

        // movimento.colli
        public int colli { get; set; }

        public RisorseUmane contraente { get; set; }

        public RegistrazioneContabile(int codice) : base(codice,"")
        {
        }

        public RegistrazioneContabile(int codice, string descrizione) : base(codice, descrizione)
        {
        }

        public RegistrazioneContabile() :base() { }

    }
}
