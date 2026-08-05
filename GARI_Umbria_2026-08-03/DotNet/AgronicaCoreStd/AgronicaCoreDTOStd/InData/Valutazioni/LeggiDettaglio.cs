using AgronicaCoreModelsSTD.valutazioni;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Valutazioni
{

    public class LeggiDettaglio
    {
        public string piva;        
        public int idTestata;
        public int ContoCod;

        public LeggiDettaglio()
        {            
            idTestata = 0;
            ContoCod = 0;
        }
    }

}
