using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Parco_Macchine : APP_Agronica_Entity
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Mac_Cod { get; set; }
        public string Mac_Des { get; set; }
        public string Classe_Desc { get; set; }
        public string Modello { get; set; }
        public string Ditta_Des { get; set; }
        public string Codice { get; set; }

        public override string ToString()
        {
            return Mac_Des;
        }
    }
}