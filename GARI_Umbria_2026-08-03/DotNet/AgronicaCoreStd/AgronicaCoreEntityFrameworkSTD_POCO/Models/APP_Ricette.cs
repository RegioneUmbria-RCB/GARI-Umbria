using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Ricette : APP_Agronica_Entity
    {
        public int ricetta_cod { get; set; }
        public string ricetta_des { get; set; }
        public string note { get; set; }
        public int Tipo_Ricetta { get; set; }
        public int Veg_Cod { get; set; }
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public string Ricetta_Numero { get; set; }
    }
}
