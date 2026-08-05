namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Input per la persistenza atomica di una coppia chiavi+payload nella modalità "Per Azienda".
    /// Corrisponde ai dati prodotti da DS09-BL e da persistere nelle tabelle
    /// <c>Lookup_Sost_H20_Aziendale_Chiavi</c> e <c>Lookup_Sost_H20_Aziendale_Payload</c>.
    /// Riferimento spec: DS10-BL PersistenzaChiaviPayloadPerAzienda — Input.
    /// </summary>
    public class PersistenzaChiaviPayloadPerAziendaInput
    {
        /// <summary>
        /// Identificativo univoco dell'evento di calcolo (GUID). Collega chiavi e payload.
        /// Usato come <c>id_invocazione</c> in entrambe le tabelle.
        /// </summary>
        public Guid IdInvocazione { get; set; }

        /// <summary>
        /// Timestamp di calcolo in UTC (ISO 8601).
        /// </summary>
        public DateTime DataCalcolo { get; set; }

        /// <summary>Identificativo della filiera produttiva.</summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>Identificativo dell'azienda agricola.</summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>Anno solare di riferimento del calcolo.</summary>
        public int Anno { get; set; }

        /// <summary>
        /// Codice paese ISO Alpha-3 (es. <c>ITA</c>).
        /// Riferimento spec: DS10-BL — "nazione: string (ISO Alpha-3)".
        /// </summary>
        public string Nazione { get; set; } = string.Empty;

        /// <summary>Regione geografica. Opzionale.</summary>
        public string? Regione { get; set; }

        /// <summary>Specie colturale (es. <c>Vitis vinifera</c>). Opzionale.</summary>
        public string? Specie { get; set; }

        /// <summary>Varietà colturale (es. <c>Sangiovese</c>). Opzionale.</summary>
        public string? Varieta { get; set; }

        /// <summary>Quantità raccolta aggregata per l'esercizio in kg.</summary>
        public decimal QuantitaRaccoltaKg { get; set; }

        /// <summary>
        /// Superficie coltivata totale in ettari (ha).
        /// Riferimento spec: DS10-BL — Validazione: superficie_coltivata_ha &gt; 0.
        /// </summary>
        public decimal SuperficieColtivataHa { get; set; }

        /// <summary>
        /// Payload JSON strutturato (output di DS09-BL) serializzato come byte array UTF-8.
        /// Riferimento spec: DS10-BL — Validazione: deve essere JSON valido (parse test).
        /// </summary>
        public string PayloadJson { get; set; } = string.Empty;

        /// <summary>
        /// JSON firmato (output di DS09-BL) serializzato come byte array UTF-8.
        /// Riferimento spec: DS10-BL.
        /// </summary>
        public byte[] JsonFirmato { get; set; } = Array.Empty<byte>();
    }
}
