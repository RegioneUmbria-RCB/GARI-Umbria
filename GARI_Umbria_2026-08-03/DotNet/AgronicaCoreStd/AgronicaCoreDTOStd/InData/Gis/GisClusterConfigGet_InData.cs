using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InData.Gis
{
    /// <summary>
    /// DTO per le richieste di LETTURA o CANCELLAZIONE di una specifica 
    /// configurazione di clustering GIS. Contiene solo i parametri identificativi.
    /// </summary>
    public class GisClusterConfigGet_InData
    {
        /// <summary>
        /// L'ID del tipo di algoritmo di clustering che stiamo cercando.
        /// Esempio: 1 = "Raggruppamento per Distanza"
        /// </summary>
        [JsonProperty("LayerElementiGrafici_Config_Type")]
        public int LayerElementiGraficiConfigTypeCod { get; set; }

        /// <summary>
        /// L'ID del layer della mappa a cui si riferisce la configurazione.
        /// Esempio: 19 = "Mappa dei Campi di Grano"
        /// </summary>
        [JsonProperty("LayerElementiGrafici_Cod")]
        public int LayerElementiGraficiCod { get; set; }

        /// <summary>
        /// Il nome dell'utente di cui vogliamo leggere o cancellare la configurazione.
        /// Esempio: "pippo"
        /// </summary>
        [JsonProperty("Utente")]
        public string Utente { get; set; }
    }
}
