using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.DataExchange.AntaresTrace
{
    public class FiltroRicercaSemine
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
        public int Id_Reg { get; set; }
        public int? Progetto_Cod { get; set; } // Opzionale
        public DateTime ValiditaInizio { get; set; }
        public DateTime ValiditaFine { get; set; }
    }
}
