using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Centri_Aziendali : APP_Agronica_Entity
    {
        public string piva { get; set; }
        public int sa_cod { get; set; }
        public string sa_nome { get; set; }

    }
}
