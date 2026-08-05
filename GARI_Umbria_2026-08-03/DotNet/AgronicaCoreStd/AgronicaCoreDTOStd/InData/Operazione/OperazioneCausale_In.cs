using System;
using System.Collections.Generic;
using System.Text;

namespace InData.Operazione
{
    public class OperazioneCausale_In
    {
        public int Id { get; set; }
        public string Causale { get; set; }
        public int LavCod { get; set; }
        public int inviato { get; set; }
        public DateTime validitaInizio { get; set; }
        public DateTime validitaFine { get; set; }
        public bool isNew { get; set; }
    }
}
