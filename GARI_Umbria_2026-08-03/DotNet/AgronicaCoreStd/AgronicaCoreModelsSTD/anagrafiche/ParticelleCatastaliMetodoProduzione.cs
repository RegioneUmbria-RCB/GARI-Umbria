using AgronicaCoreModelsSTD.metaschema;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParticelleCatastaliMetodoProduzione
    {
        public MetodoProduzione metodoProduzione { get; set; }
        public IntervalloTemporale validita { get; set; }

        public ParticelleCatastaliMetodoProduzione()
        {

        }
    }
}
