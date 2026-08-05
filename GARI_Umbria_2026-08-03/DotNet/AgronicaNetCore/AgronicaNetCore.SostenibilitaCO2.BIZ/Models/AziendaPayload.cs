using System.Text.Json.Serialization;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta una singola azienda nell'array <c>aziende[]</c> del payload M4.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Regola 3-7, Output <c>aziende[]</c>.
    /// </summary>
    public class AziendaPayload
    {
        /// <summary>Identificativo univoco dell'azienda (CUAA).</summary>
        public string id_azienda { get; set; } = string.Empty;

        /// <summary>Partita IVA dell'azienda.</summary>
        [JsonIgnore]
        public string piva_azienda { get; set; } = string.Empty;

        /// <summary>Anno di campagna.</summary>
        public int campagna { get; set; }

        /// <summary>
        /// Centroide dell'azienda in formato WKT EPSG:4326.
        /// Esempio: <c>"POINT(12.3456 41.2345)"</c>.
        /// </summary>
        public string centroide { get; set; } = string.Empty;

        /// <summary>Codice nazione ISO 3166-1 alpha-2 dell'azienda.</summary>
        public string nazione { get; set; } = string.Empty;

        /// <summary>Consumi aziendali (carburanti ed energia).</summary>
        public ConsumiPayload consumi { get; set; } = new();

        /// <summary>
        /// Lista degli appezzamenti con impianti e operazioni.
        /// Inizializzata come array vuoto da DS03-BL; popolata da DS04-BL.
        /// </summary>
        public List<AppezzamentoPayload> appezzamenti { get; set; } = new List<AppezzamentoPayload>();
    }
}
