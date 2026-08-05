namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Input per la creazione del payload aggregato di sostenibilità idrica in modalità "Per Azienda".
    /// Contiene i metadati dell'evento di calcolo e la lista degli indicatori calcolati per ciascun
    /// esercizio che compone il perimetro aziendale.
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda — Input.
    /// </summary>
    public class AggregazionePayloadPerAziendaInput
    {
        /// <summary>
        /// Identificativo univoco dell'evento di calcolo (≡ id_invocazione).
        /// Creato dal chiamante per correlare l'aggregazione alla sessione di calcolo DS07-BL.
        /// </summary>
        public Guid IdInvocazione { get; set; }

        /// <summary>
        /// Timestamp del calcolo in UTC (ISO 8601).
        /// Riferimento spec: DS09-BL — "data_calcolo: string (ISO 8601)".
        /// </summary>
        public DateTime DataCalcolo { get; set; }

        /// <summary>
        /// Identificativo della filiera produttiva.
        /// </summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>
        /// Identificativo dell'azienda agricola. Popola il campo <c>id_azienda</c> nel payload JSON.
        /// </summary>
        public string Azienda { get; set; } = string.Empty;

        /// <summary>
        /// Anno solare di riferimento del calcolo.
        /// </summary>
        public int Anno { get; set; }

        /// <summary>
        /// Indicatori calcolati per ciascun esercizio che compone il perimetro aziendale-anno.
        /// Prodotti da DS07-BL per ogni combinazione (appezzamento, varietà, esercizio).
        /// Riferimento spec: DS09-BL — Fase 1 Aggregazione: "somma tutte le righe di calcolo
        /// precedenti (da DS07-BL) per (filiera, azienda, anno)".
        /// </summary>
        public List<AggregazioneEsercizioInput> IndicatoriEsercizi { get; set; } = new();
    }
}
