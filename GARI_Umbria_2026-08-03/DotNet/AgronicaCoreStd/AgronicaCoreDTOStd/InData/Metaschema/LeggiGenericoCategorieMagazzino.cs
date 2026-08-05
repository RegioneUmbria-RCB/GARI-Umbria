using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Metaschema
{
    public class LeggiGenericoCategorieMagazzino
    {
        public string Filtro { get; set; }
        /// <summary>
        /// Indica se si vogliono ricevere i dati in forma semplificata (indicando solo codice e descrizione).
        /// </summary>
        public Boolean Simple { get; set; } = false;

        public LeggiGenericoCategorieMagazzino()
        {
            this.Filtro = "";
        }

        public LeggiGenericoCategorieMagazzino(string filtro)
        {
            this.Filtro = filtro;
        }
    }
}
