using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiSpecieVegetali_IN
    {
        public int Veg_Cod { get; set; } = 0;

        public int Gru_Cod { get; set; } = 0;

        public string LetteraIniziale { get; set; } = string.Empty;

        public string StringaCerca { get; set; } = string.Empty;
    }
}
