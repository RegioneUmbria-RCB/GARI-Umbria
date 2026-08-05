using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InData.Gis
{
    // --- CLASSE PER IL DETTAGLIO ---
    /// <summary>
    /// Rappresenta una singola riga di dettaglio.
    /// Definisce una regola specifica per un intervallo di zoom.
    /// </summary>
    public class GisClusterConfigDetail_InData
    {
        /// <summary>
        /// Un ID interno per questa riga di dettaglio.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Una descrizione leggibile di cosa fa questa regola.
        /// Esempio: "Raggruppamento per province"
        /// </summary>
        [JsonProperty("Description")]
        public string Description { get; set; }

        /// <summary>
        /// I parametri tecnici per questa regola (es. distanza, dimensione griglia).
        /// Esempio: "{\"ZoomMax\":8, \"ZoomMin\":3, \"Distance\":50000}"
        /// </summary>
        [JsonProperty("Parameters")]
        public string Parameters { get; set; }
    }

    // --- CLASSE PRINCIPALE (TESTATA + DETTAGLIO) ---
    /// <summary>
    /// DTO principale per CREARE o AGGIORNARE una configurazione di clustering GIS.
    /// Contiene le informazioni generali (testata) e una lista di regole di dettaglio.
    /// </summary>
    public class GisClusterConfigSave_InData
    {
        /// <summary>
        /// L'ID del tipo di algoritmo da usare.
        /// </summary>
        [JsonProperty("LayerElementiGrafici_Config_Type")]
        public int LayerElementiGraficiConfigTypeCod { get; set; }

        /// <summary>
        /// L'ID del layer della mappa.
        /// </summary>
        [JsonProperty("LayerElementiGrafici_Cod")]
        public int LayerElementiGraficiCod { get; set; }

        /// <summary>
        /// La descrizione del layer della mappa.
        /// </summary>
        [JsonProperty("LayerElementiGrafici_Des")]
        public string LayerElementiGraficiDes { get; set; }

        /// <summary>
        /// Il nome dell'utente.
        /// </summary>
        [JsonProperty("Utente")]
        public string Utente { get; set; }

        /// <summary>
        /// Il livello di zoom oltre il quale i punti non vengono più raggruppati.
        /// </summary>
        [JsonProperty("Livello_Zoom_Massimo_Visualizzazione_Raggruppata")]
        public int LivelloZoomMassimoVisualizzazioneRaggruppata { get; set; }

        /// <summary>
        /// La lista di tutte le regole di dettaglio per questa configurazione.
        /// /// È come la tabella sul retro del modulo d'ordine.
        /// </summary>
        [JsonProperty("details")]
        public List<GisClusterConfigDetail_InData> Details { get; set; }

        /// <summary>
        /// Costruttore per assicurarsi che la lista non sia mai "nulla".
        /// </summary>
        public GisClusterConfigSave_InData()
        {
            Details = new List<GisClusterConfigDetail_InData>();
        }
    }
}
