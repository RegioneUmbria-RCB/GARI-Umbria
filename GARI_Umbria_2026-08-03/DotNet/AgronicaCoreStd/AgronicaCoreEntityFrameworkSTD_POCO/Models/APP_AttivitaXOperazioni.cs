using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_AttivitaXOperazioni : APP_Agronica_Entity
    {
        public int Id_Attivita { get; set; }
        public int Lav_Cod { get; set; }
        public string Descrizione { get; set; }

    }
}
