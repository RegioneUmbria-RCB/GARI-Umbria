using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_MisuraXAvversita_Anagrafiche: APP_Agronica_Entity
    {
        public int MxAV_Cod { get; set; }
        public string Anag_des  { get; set; }
        public int Anag_valore { get; set; }
    }
}
