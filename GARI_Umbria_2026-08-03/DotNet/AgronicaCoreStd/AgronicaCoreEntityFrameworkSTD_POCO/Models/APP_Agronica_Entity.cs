using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Agronica_Entity
    {
        public int Id { get; set; }
        public int inviato { get; set; }
        public DateTime? datainvio { get; set; }
        public DateTime Data_Creazione { get; set; }
        public DateTime Data_Modifica { get; set; }
        public string Username_Creazione { get; set; }
        public string Username_Modifica { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
    }
}
