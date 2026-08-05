using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Menu
{
    public class LeggiSezioni
    {
        public int IDTipoSezione { get; set; }
        public int IDSezionePadre { get; set; }
        public Boolean Preferiti { get; set; }

    }

    
 
    public class preferiti_in
    { 
        public List<int> preferiti { get; set; }
    }

    public class attivitaNavigazioneAziende_in
    {
        public string piva { get; set; }
    }
    

}
