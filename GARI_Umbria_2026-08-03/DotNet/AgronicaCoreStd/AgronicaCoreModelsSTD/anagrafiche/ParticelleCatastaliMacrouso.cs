using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParticelleCatastaliMacrouso
    {
        public Macrouso macrouso { get; set; }
        public IntervalloTemporale validita { get; set; }
        public double Area { get; set; }

        public string Piva { get; set; }
        public string NumeroFascicolo { get; set; }
        public DateTime DataValidazioneFascicolo { get; set; }


        public ParticelleCatastaliMacrouso()
        {

        }
    }
}
