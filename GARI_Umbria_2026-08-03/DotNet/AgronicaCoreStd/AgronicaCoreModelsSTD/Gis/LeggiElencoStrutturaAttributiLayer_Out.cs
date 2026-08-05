using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Dati struttura attributi layer
    /// </summary>
    public class DatiStrutturaAttributiLayer
    {
        /// <summary>
        /// Elenco struttura attributi layer
        /// </summary>
        public List<StrutturaAttributiLayer> ListaStrutturaAttributiLayer { get; set; }
        /// <summary>
        /// Struttura dati layer in formato Kendo Grid
        /// </summary>
        public string KendoGridAttributiLayer { get; set; }
    }

    /// <summary>
    /// Struttura attributi layer
    /// </summary>
    public class StrutturaAttributiLayer
    {
        /// <summary>
        /// Progressivo attributo
        /// </summary>
        public int StructCod { get; set; }
        /// <summary>
        /// Codice etichetta attributo
        /// </summary>
        public string EtichettaCod { get; set; }
        /// <summary>
        /// Descrizione etichetta attributo
        /// </summary>
        public string EtichettaDes { get; set; }
        /// <summary>
        /// Tipo dato attributo
        /// </summary>
        public string TipoDato { get; set; }
    }
}
