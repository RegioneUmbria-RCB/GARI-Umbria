using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class GruppoMerceDto
    { 
        public string Piva_SuperUser { get; set; }
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public Nullable<int> Id_Gruppo_Merce { get; set; }
        public string Codice { get; set; }
        public string Descrizione { get; set; }
        public Nullable<short> inviato { get; set; }
        public Nullable<DateTime> datainvio { get; set; }
        public Nullable<DateTime> Data_Creazione { get; set; }
        public Nullable<DateTime> Data_Modifica { get; set; }
        public string Username_Creazione { get; set; }
        public string Username_Modifica { get; set; }
        public Nullable<DateTime> Validita_Inizio { get; set; }
        public Nullable<DateTime> Validita_Fine { get; set; }
    }
}
