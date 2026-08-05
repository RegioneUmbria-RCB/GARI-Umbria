using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_SpecieVegetaliXStadiCrescita: APP_Agronica_Entity
    {
        public int Cod_SS { get; set; }
        public int Veg_Cod { get; set; }
        public int ID_BBCH { get; set; }
        public int Cod_MS { get; set; }
        public int Progressivo { get; set; }
        public string Descrizione { get; set; }
        public int FF_Cod { get; set; }
        public int Flag_Fioritura { get; set; }
        public int Flag_Visibile { get; set; }

    }
}
