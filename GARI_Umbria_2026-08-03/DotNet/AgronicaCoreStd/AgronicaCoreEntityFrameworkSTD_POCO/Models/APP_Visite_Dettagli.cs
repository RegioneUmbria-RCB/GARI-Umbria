using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Visite_Dettagli : APP_Agronica_Entity
    {
        public int Visita_Cod { get; set; }
        public int Visita_Dettaglio_Cod { get; set; }
        public int Id_Attivita { get; set; }
        public string Descrizione { get; set; }
        public int Dettaglio_GenCod { get; set; }
        public int Dettaglio_SpeCod { get; set; }
        public int Dettaglio_IProCod { get; set; }
        public int Dettaglio_VegCod { get; set; }
        public int Dettaglio_IdCod { get; set; }

    }
}
