using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_LogEventi : APP_Agronica_Entity
    {
        public string Piva_Superuser { get; set; }
        public DateTime DataOraRilevata { get; set; }
        public string Evento { get; set; }
        public string NrBadge { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Identif_Dispositivo { get; set; }

    }
}
