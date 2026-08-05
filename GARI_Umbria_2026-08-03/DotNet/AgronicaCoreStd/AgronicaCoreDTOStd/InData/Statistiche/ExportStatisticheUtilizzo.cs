using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Statistiche
{
    public class ExportStatisticheUtilizzo
    {
        public int JobId { get; set; }
        public DateTime? JobAvvio { get; set; }
        public int FiltroPerDataCompetenzaOrDataRegistrazione { get; set; }
        public IntervalloTemporale IntervalloOperazioniDiCampagna { get; set; }
        public IntervalloTemporale IntervalloPratiche { get; set; }
        public string Username { get; set; }
        public string Piva { get; set; }
        public bool DettagliQdC {  get; set; }
    }
}
