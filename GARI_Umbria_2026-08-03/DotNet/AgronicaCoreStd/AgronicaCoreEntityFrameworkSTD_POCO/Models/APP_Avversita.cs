using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Avversita : APP_Agronica_Entity
    {
        public int Av_Cod { get; set; }
        public string Av_Des_Vol { get; set; }
        public int Av_Gru { get; set; }
        public string Av_Gru_Des { get; set; }

    }
}
