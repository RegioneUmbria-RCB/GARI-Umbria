using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_CDG_Generale : APP_Agronica_Entity
    {
        // public string Piva_Superuser { get; set; }
        public string Piva { get; set; }
        public int Id_Cdg_Generale { get; set; }
        public DateTime Data_Inserimento { get; set; }
        public int Bozza { get; set; }
        public string Note { get; set; }
    }
}
