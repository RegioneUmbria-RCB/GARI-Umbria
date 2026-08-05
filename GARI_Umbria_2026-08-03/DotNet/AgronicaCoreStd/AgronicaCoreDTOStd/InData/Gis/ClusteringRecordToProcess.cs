using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InData.Gis
{
    /// <summary>
    /// Rappresenta un singolo record della tabella GIS_ElementiGrafici_Clustering
    /// </summary>
    public class ClusteringRecordToProcess
    {
        /// <summary>
        /// Identificativo PivaSuperUser dalla chiave primaria composita.
        /// </summary>
        public string PivaSuperUser { get; set; }

        /// <summary>
        /// Identificativo LayerElementoGrafico_Cod dalla chiave primaria composita.
        /// </summary>
        public int LayerElementoGrafico_Cod { get; set; }

        /// <summary>
        /// Identificativo ElementoGrafico_Cod dalla chiave primaria composita.
        /// </summary>
        public int ElementoGrafico_Cod { get; set; }

        /// <summary>
        /// Costruttore di default.
        /// per inizializzare la stringa a string.Empty per evitare
        /// NullReferenceException se l'oggetto viene creato senza popolare le proprietà.
        /// </summary>
        public ClusteringRecordToProcess()
        {
            PivaSuperUser = string.Empty;
        }
    }
}
