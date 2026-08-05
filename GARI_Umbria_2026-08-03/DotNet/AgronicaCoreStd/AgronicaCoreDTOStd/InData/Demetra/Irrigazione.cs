using AgronicaCoreModelsSTD.attivita.dettagli;
using AgronicaCoreModelsSTD.attivita.risorse;
using InData.Demetra;
using System;

namespace AgronicaCoreDTOStd.InData.Demetra
{
    public class Irrigazione : IWithUDM
    {
        public Impianto impianto { get; set; }

        public decimal quantita { get; set; }
        
        public string udm { get; set; }
        
        public DateTime inizio { get; set; }
        
        public DateTime fine { get; set; }
        
        public int frequenza { get; set; }

        public int tipo { get; set; }
    }

}
