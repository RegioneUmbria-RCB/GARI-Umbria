using System;
using System.Collections.Generic;
using System.Text;
//per erbe infestanti
namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    //aaaa
    public class APP_InfestantiAttive : APP_Agronica_Entity
    {
        public int Av_Cod { get; set; }
        public int Av_Gru { get; set; }
        public string Av_Des_Vol { get; set; }
    }
}
