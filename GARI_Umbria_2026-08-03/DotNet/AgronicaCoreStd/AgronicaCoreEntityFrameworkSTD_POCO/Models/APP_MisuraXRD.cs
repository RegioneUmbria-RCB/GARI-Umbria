using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_MisuraXRD: APP_Agronica_Entity
    {
        public int Veg_Cod { get; set; }
        public int Dr_Cod { get; set; }
        public int Udm_Cod { get; set; }
        public string Dr_Des { get; set; }
        public string Udm_Des { get; set; }
        public string Udm_Sim { get; set; }   
        public int Flag_Visibile { get; set; }
    }
}
