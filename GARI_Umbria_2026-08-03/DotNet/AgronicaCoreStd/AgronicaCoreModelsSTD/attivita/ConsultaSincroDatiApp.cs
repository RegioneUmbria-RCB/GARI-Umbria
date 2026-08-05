using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class ConsultaSincroDatiApp
    {
        public DateTime dataFiltro_inizio { get; set; }
        public DateTime dataFiltro_fine { get; set; }
        public List<string> tipiDato { get; set; }
        public bool datiAggiuntivi { get; set; }
        public int filtroImportati { get; set; }
        public string pivaCUAA { get; set; }
    }
}
