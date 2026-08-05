using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Epoche : APP_Agronica_Entity
    {
        public int Epoca_Cod { get; set; }
        public string Epoca_Des { get; set; }
        public int Specie_Cod { get; set; }
    }
}
