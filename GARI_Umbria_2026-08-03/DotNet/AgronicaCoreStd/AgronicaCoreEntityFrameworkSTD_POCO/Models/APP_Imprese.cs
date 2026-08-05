using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Imprese : APP_Agronica_Entity
    {
        public string piva { get; set; }
        public string cuaa { get; set; }
        public string rag_soc { get; set; }

    }
}
