using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Tipologie : APP_Agronica_Entity
    {
        public int ID_Tipologia { get; set; }
        public string Nome_Tipologia { get; set; }
        public int ID_Area { get; set; }
        public string Nome_Area { get; set; }

        public override string ToString()
        {
            return (!string.IsNullOrEmpty(Nome_Area) ? Nome_Area + " > " : "") + Nome_Tipologia;
        }
    }
}
