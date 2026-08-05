using AgronicaCoreDTOStd.InData.FiltroRicerca;
using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.DomandaIrrigua
{
    public class LettureContatori_IN
    {
        public string Piva { get; set; }
        public IntervalloTemporale PeriodoLettura { get; set; }
    }    
    
    public class ScriviLetturaContatore
    {
        public int Id_Lettura { get; set; }
        public string Piva { get; set; }
        public int Id_Contatore { get; set; }
        public decimal Valore_Lettura { get; set; }
        public DateTime Data_Lettura { get; set; }
        public bool Flag_Cancellazione { get; set; }
    }  
    
    public class LeggiContatori_IN
    {
        public string Piva { get; set; }
        public DateTime DataLettura { get; set; }
    }


}
