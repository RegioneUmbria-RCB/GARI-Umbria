using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class CategorieMagazzino
    {
        public int Elem_Cod { get; set; }
        public string Cau_Mov { get; set; }
        public int Lav_Cod { get; set; }
        public Boolean Flag_NoSemilavorati { get; set; }
        public Boolean Flag_AltriBeni { get; set; }
        public string xFiltroAggiuntivo { get; set; }

    }
}
