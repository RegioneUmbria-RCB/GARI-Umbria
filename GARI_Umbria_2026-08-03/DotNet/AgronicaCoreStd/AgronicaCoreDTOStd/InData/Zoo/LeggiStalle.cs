using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    public class LeggiStalle
    {
        public Impresa impresa { get; set; }
        public CentroAziendale centro { get; set; }
        public Fabbricato stalla { get; set; }
        public bool getRaggruppamenti { get; set; }
        public bool getIndirizzo { get; set; }
    }
}
