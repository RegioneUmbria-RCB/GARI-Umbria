using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class ParticelleCatastaliClassamento
    {
        public string porzione { get; set; }
        public double Area { get; set; }
        public BaseCodeDescr qualita { get; set; }

        public string classe { get; set; }

        public double redditoDomiciliare { get; set; }
        public double redditoAgrario { get; set; }

        public ParticelleCatastaliClassamento()
        {

        }
    }
}
