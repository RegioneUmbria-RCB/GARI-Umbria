using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class BloccaSbloccaAppezzamenti
    {
        public List<Appezzamento> appezzamenti { get; set; }
        public bool blocca { get; set; }
    }
}
