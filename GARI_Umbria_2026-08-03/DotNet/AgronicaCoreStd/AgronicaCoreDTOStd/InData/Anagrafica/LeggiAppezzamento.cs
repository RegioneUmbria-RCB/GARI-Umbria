using AgronicaCoreModelsSTD.anagrafiche;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica { 
    public class LeggiAppezzamento
    {
        public Appezzamento appezzamento { get; set; }
        public DateTime data { get; set; }
        public Boolean filtroData { get; set; }
        public Boolean leggiIndirizzi { get; set; }
        public Boolean leggiCatasto { get; set; }
        public Boolean leggiImpianti { get; set; }
        public Boolean leggiDistinte { get; set; }
        public Boolean leggiCartografia { get; set; }

    }
}
