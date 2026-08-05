using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiCategorieMagazzinoImpostazioni
    {
        public string filter { get; set; }
        public int Elem_Cod { get; set; }

        public string Piva { get; set; } = "";
        public int Sa_Cod { get; set; } = 0;
    }
}
