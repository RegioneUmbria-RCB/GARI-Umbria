using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace InData.Engine.Nutrizione
{
    /// <summary>
    /// DTO di input per l'endpoint di caricamento dati widget DSS Nutrizione.
    /// Mappa il corpo JSON della richiesta definito nelle specifiche API.
    /// </summary>
    /// <remarks>
    /// DS05-BL: CaricamentoDatiWidgetNutrizione — Formato Richiesta.
    /// </remarks>
    public class DatiWidgetNutrizioneRequest
    {
        /// <summary>
        /// Partita IVA dell'azienda per cui caricare i widget. Obbligatoria.
        /// </summary>
        [Required]
        [JsonProperty("piva")]
        public string Piva { get; set; }

        /// <summary>
        /// Codice centro aziendale. Se 0, vengono considerati tutti i centri
        /// autorizzati per l'utente tramite Utenti_Visibilita_Appoggio.
        /// </summary>
        [JsonProperty("sa_cod")]
        public int SaCod { get; set; }

        /// <summary>
        /// Anno solare di riferimento per il filtro degli appezzamenti attivi.
        /// Se null, viene usato l'anno corrente.
        /// </summary>
        [JsonProperty("anno_solare")]
        public int? AnnoSolare { get; set; }
    }
}
