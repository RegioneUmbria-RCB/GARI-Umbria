using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Configurazione_Siti
    {
        public int Id { get; set; }
        public string PivaSuperUser { get; set; }
        public int Sito_Cod { get; set; }
        public string Chiave { get; set; }
        public string Valore { get; set; }

    }
}
