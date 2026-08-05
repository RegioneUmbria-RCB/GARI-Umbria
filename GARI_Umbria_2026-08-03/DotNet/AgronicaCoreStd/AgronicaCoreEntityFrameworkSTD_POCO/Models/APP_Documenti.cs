using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Documenti : APP_Agronica_Entity
    {
        public string Piva_Superuser { get; set; }
        public string Piva { get; set; }
        public int Documento_Cod { get; set; }
        public int ID_Tipologia { get; set; }
        public DateTime Data_Scadenza { get; set; }
        public string Descrizione { get; set; }
        public string Allegati { get; set; }
        public string Note { get; set; }
        public int Visita_Cod { get; set; }

    }
}
