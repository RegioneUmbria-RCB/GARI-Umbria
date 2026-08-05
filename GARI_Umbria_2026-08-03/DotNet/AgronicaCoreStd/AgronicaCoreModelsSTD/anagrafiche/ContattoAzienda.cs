using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ContattoAzienda
    {
        public List<RisorseUmane> risorseUmane { get; set; }
        public bool contattoPubblico { get; set; }
        public int proprietarioContattoAzienda { get; set; } // Enum_ProprietarioContattoAzienda
        public string aziendaCorrentePIVA { get; set; } // Partita IVA dell'azienda corrente durante la creazione della nuova azienda

    }
}
