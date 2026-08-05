using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Parametri restituzione filtro impianti
    /// </summary>
    public class AggiornaFiltroImpianti_Out
    {
        /// <summary>
        /// Id testata tabella aggiornato
        /// </summary>
        public int IdTestata { get; set; }
        /// <summary>
        /// Indica se occorre impostare la visibilità totale
        /// </summary>
        public bool ImpostaVisibilitaTotale { get; set; }
        /// <summary>
        /// Indica il totale elementi grafici di tipo impianto
        /// </summary>
        public int TotaleElementiGraficiImpianto { get; set; }
    }
}
