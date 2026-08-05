using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_RicettexNote : APP_Agronica_Entity
    {
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Nota_Cod { get; set; }

    }
}
