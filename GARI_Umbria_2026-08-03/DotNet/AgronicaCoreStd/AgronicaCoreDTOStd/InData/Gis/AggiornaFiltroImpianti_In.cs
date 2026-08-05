using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri impostazione filtro impianti
    /// </summary>
    public class AggiornaFiltroImpianti_In
    {
        /// <summary>
        /// Id per inserimento/lettura tabella di appoggio.
        /// Se passato = 0, inserisce impianti in tabella di appoggio e crea nuovo Id.
        /// </summary>
        public int IdTestata { get; set; }
        /// <summary>
        /// Elenco impianti da filtrare
        /// </summary>
        public List<ChiaveImpianto_In> Impianti { get; set; }
    }
}
