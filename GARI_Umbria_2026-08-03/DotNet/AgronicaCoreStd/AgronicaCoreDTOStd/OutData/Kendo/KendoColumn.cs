using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.Kendo
{
    public class KendoColumn
    {
        public string Field { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// tipo di dato da cui estrarre il filtro da applicare alla colonna 
        /// </summary>
        public string DataType { get; set; } = "string";

        /// <summary>
        /// se title continene un valore tradotto, oppure una stringa da tradurre
        /// </summary>
        public bool TranslateTitle { get; set; } = true;

        /// <summary>
        /// Colonna pre-selezionata
        /// </summary>
        public bool Hidden { get; set; } = false;
        /// <summary>
        /// Colonna nascosta, mai visibile dall'utente
        /// </summary>
        public bool Display { get; set; } = true;

        /// <summary>
        /// ordinamento colonne mandate al client
        /// </summary>
        public int ColumnOrder { get; set; }
    }
}
