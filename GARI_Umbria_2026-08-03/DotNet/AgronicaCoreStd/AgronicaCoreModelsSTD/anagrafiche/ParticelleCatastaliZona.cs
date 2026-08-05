using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParticelleCatastaliZona
    {

        public BaseCodeDescr zona { get; set; }
        public double Area { get; set; }
        public IntervalloTemporale validita { get; set; }

        public ParticelleCatastaliZona()
        {

        }
    }
}
