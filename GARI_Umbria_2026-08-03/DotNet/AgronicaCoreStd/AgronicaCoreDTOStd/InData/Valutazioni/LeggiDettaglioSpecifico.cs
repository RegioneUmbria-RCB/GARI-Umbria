using AgronicaCoreModelsSTD.valutazioni;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Valutazioni
{

    public class LeggiDettaglioSpecifico
    {
        public string piva;
        public int idTestata;
        public int ContoCod;
        public int Anno;
        public string DettaglioKey;

        public LeggiDettaglioSpecifico()
        {
            idTestata = 0;
            ContoCod = 0;
            Anno = 0;            
            DettaglioKey = "";
        }
    }



    public class LeggiDettaglioSpecificoArete
    {
        public string piva;
        public int idTestata;
        public int ContoCod;
        public int Anno;
        public string DettaglioKey;
        public string jSonArete;

        public LeggiDettaglioSpecificoArete()
        {
            idTestata = 0;
            ContoCod = 0;
            Anno = 0;
            DettaglioKey = "";
            jSonArete = "";
        }
    }

}
