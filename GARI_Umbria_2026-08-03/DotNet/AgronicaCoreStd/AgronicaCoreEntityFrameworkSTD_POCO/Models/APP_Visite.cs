using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Visite : APP_Agronica_Entity
    {
        public int Visita_Cod { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Tipo_Visita { get; set; }
        public DateTime Data_Visita { get; set; }
        public string Operatore { get; set; }
        public string Posizione { get; set; }
        public int Lav_Cod { get; set; }
        public string Note { get; set; }
    }
}