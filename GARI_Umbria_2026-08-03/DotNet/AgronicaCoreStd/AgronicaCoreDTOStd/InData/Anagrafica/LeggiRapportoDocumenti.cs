using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
   public  class LeggiRapportoDocumenti
    {
        public string piva { get; set; }
        public Boolean rapportoAttivo { get; set; }
        public DateTime? dataValidita { get; set; }
        public int tipoRapporto { get; set; }
        public Boolean filtraSoloValidi { get; set; }
        public Boolean cercaSoloValidi { get; set; }
        public Boolean includiIndirizzo { get; set; }
        public int accettazioneConGerarchia { get; set; }
        public string pivaPadreGerarchia { get; set; }
        public string testoRicerca { get; set; }
        public int codRisUm { get; set; }  
        public string codContatto { get; set; }
        public Boolean checkRaccolte { get; set; }
        public DateTime? dataFineRaccolte { get; set; }                                 
    }
}
