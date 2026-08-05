using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class RapportoContabile : BaseCodeDescr
    {

        public bool cliente { get; set; }
        public bool fornitore { get; set; }
        public bool dipendente { get; set; }
        public bool terzista { get; set; }
        public bool legale { get; set; }
        public bool agente { get; set; }
        public bool consulente { get; set; }
        public bool flag_cancellazione { get; set; }

        public RapportoContabile(int codice) : base(codice, "")
        {
            this.flag_cancellazione = false;
        }

        public RapportoContabile()
        {
            this.flag_cancellazione = false;
        }

    }
}
