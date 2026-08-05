using AgronicaCoreModelsSTD.valutazioni;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Valutazioni
{

    public class LeggiTestata
    {
        public string piva;
        public int idTestata;
        public bool includiAnno;

        public LeggiTestata()
        {
            idTestata = 0;
            includiAnno = true;
        }
    }

}
