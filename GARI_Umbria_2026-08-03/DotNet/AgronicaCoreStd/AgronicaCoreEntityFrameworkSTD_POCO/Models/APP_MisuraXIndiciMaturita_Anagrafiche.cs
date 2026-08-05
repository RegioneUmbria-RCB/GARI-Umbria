using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_MisuraXIndiciMaturita_Anagrafiche: APP_Agronica_Entity
    {
        public int MxIn_Cod { get; set; }
        public int UDM_COD { get; set; }
        public string Anag_des { get; set; }
        public int Anag_valore { get; set; }
    }
}
