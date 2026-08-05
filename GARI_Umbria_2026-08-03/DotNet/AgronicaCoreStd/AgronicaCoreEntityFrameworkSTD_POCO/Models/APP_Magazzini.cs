using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Magazzini : APP_Agronica_Entity
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Id_Destinazione { get; set; }
        public int Tipo_Destinazione { get; set; }
        public string Ubic_Des { get; set; }
        public int Visibile_da_App { get; set; }
    }
}
