using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Sequenza_Tabelle: APP_Agronica_Entity
    {
        public string Nome_Tabella { get; set; }
        public int Ultimo_Valore { get; set; }
        public int Base { get; set; }
        public int End { get; set; }

    }
}
