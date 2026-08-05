using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Attivita : APP_Agronica_Entity
    {
        public int Id_Attivita { get; set; }
        public string Desc { get; set; }
        public int Attivita_Extra_Campagna { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
    }
}