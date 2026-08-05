using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.metaschema
{
    public class Farmaco: BaseCodeDescr
    {
        public string AIC { get; set; }
        public string confezione { get; set; }
        public BaseCodeDescr modalitaPrescrizione { get; set; }

        public IntervalloTemporale validita { get; set; }

        public IntervalloTemporale validitaCommercializzazione { get; set; }

        public Farmaco() : base() { }
    }
}
