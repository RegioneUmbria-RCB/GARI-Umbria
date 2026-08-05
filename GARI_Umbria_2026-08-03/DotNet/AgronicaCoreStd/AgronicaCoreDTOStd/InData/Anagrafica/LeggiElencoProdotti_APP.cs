using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class LeggiElencoProdotti_APP
    {
        public string piva { get; set; }
        public int Elem_Cod { get; set; }
        public string Data_Movimento_Str { get; set; }
        public string FiltroDescrizioneProdotto { get; set; }
        public int Veg_Cod { get; set; }
        public int Magazzino { get; set; }
    }
}
