using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Riferimenti_Interventi_Cdg : APP_Agronica_Entity
    {
        // public string Piva_Superuser { get; set; }
        public string Piva { get; set; }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Id_Cdg_Generale_Rif { get; set; }
    }
}
