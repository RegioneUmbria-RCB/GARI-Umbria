using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_AttivitaXCentri_Aziendali : APP_Agronica_Entity
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int ID_Attivita { get; set; }
        public int Inclusa { get; set; }     
    }
}
