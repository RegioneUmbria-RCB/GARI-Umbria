
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Operazioni : APP_Agronica_Entity
    {
        public int lav_cod { get; set; }
        public string lav_des { get; set; }
        public int gru_cod { get; set; }
        public string gru_des { get; set; }

    }
}