using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Imputazioni_Fasi : APP_Agronica_Entity
    {
        public int Id_Attivita { get; set; }
        public int Imputazione_Cod { get; set; }
        public string Imputazione_Nome { get; set; }
        public string Piva { get; set; }
    }
}
