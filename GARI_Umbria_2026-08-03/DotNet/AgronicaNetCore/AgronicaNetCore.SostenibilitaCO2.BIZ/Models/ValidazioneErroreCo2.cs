namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo errore di validazione rilevato nel payload M4.
    /// Identifica il livello gerarchico, il tipo di violazione, il percorso JSONPath al
    /// campo errato e il valore attuale non valido.
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4 — Output, <c>errori_validazione[]</c>.
    /// </summary>
    public class ValidazioneErroreCo2
    {
        /// <summary>
        /// Livello gerarchico del payload in cui si è verificato l'errore.
        /// Valori ammessi: <c>filiera</c> | <c>azienda</c> | <c>appezzamento</c> |
        /// <c>impianto</c> | <c>operazione</c>.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public string Livello { get; set; } = string.Empty;

        /// <summary>
        /// Tipo di violazione rilevata.
        /// Valori ammessi: <c>FIELD_MISSING</c> | <c>FIELD_NULL</c> | <c>INVALID_TYPE</c> |
        /// <c>OUT_OF_RANGE</c> | <c>INVALID_FORMAT</c> | <c>CARDINALITY_VIOLATION</c>.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public string TipoErrore { get; set; } = string.Empty;

        /// <summary>
        /// JSONPath al campo che ha generato l'errore.
        /// Esempio: <c>$.aziende[0].centroide</c>.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public string PathJson { get; set; } = string.Empty;

        /// <summary>Messaggio descrittivo dell'errore.</summary>
        public string Messaggio { get; set; } = string.Empty;

        /// <summary>
        /// Valore attuale del campo errato, serializzato come stringa.
        /// <c>null</c> se il campo è assente o non valorizzato.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public string? ValoreAttuale { get; set; }
    }
}
