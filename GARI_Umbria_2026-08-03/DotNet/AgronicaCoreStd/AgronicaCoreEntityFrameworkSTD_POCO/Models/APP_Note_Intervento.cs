using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Note_Intervento : APP_Agronica_Entity
    {
        public int Nota_Cod { get; set; }
        public int NotaGruppo_Cod { get; set; }
        public string Nota_Des { get; set; }
        public string NotaGruppo_Des { get; set; }
    }
}
