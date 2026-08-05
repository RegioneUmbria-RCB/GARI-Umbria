using Newtonsoft.Json;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo impianto/esercizio (ciclo colturale) nell'array <c>impianti[]</c> del payload M4.
    /// Il campo <c>Operazioni</c> viene popolato da DS05-BL nella fase successiva.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Regole 4-10, Output <c>impianti[]</c>.
    /// </summary>
    public class ImpiantoPayload
    {
        /// <summary>
        /// Identificativo univoco impianto: <c>PIVA|Sa_Cod|Appezza|ID_Reg|Progetto_Cod</c>.
        /// Riferimento spec: DS04-BL Regola 4.
        /// </summary>
        public string id_impianto { get; set; } = string.Empty;

        /// <summary>
        /// Codice coltura (<c>Veg_Cod</c> ricavato da <c>Reg_Impianti.Cul_Cod</c> → join <c>Cultivar</c>).
        /// Riferimento spec: DS04-BL mapping <c>id_coltura</c> — DS04.2-BL Mappature.
        /// </summary>
        public string id_coltura { get; set; } = string.Empty;

        /// <summary>
        /// Tipo identificatore coltura. Sempre <c>"Profitosan"</c> (valore statico).
        /// Riferimento spec: DS04-BL Regola 5.
        /// </summary>
        public string tipo_id_coltura { get; set; } = "Profitosan";

        /// <summary>
        /// Superficie coltivata in ettari (<c>Reg_Impianti.sup_imp</c>).
        /// Riferimento spec: DS04-BL Regola 6.
        /// </summary>
        public decimal area_ha { get; set; }

        /// <summary>
        /// Resa prevista in kg/ha (<c>Imprese_Progetti.produzione_prevista</c>).
        /// Campo opzionale: <c>null</c> se non disponibile → omesso dal payload serializzato.
        /// Riferimento spec: DS04-BL Regola 7.
        /// </summary>
        public decimal? resa_prevista_kg_ha { get; set; }

        /// <summary>
        /// Data di inizio ciclo colturale (<c>Imprese_Progetti.validita_inizio</c>), formato ISO 8601.
        /// Riferimento spec: DS04-BL Regola 8.
        /// </summary>
        public DateOnly data_inizio_ciclo { get; set; }

        /// <summary>
        /// Data di fine ciclo colturale (<c>Imprese_Progetti.validita_fine</c>), formato ISO 8601.
        /// Riferimento spec: DS04-BL Regola 8.
        /// </summary>
        public DateOnly data_fine_ciclo { get; set; }

        /// <summary>
        /// Anno di impianto ricavato da <c>Reg_Impianti_Codici.val_cod</c> con <c>id_cod = 1362</c>.
        /// Campo opzionale: <c>null</c> se non disponibile → omesso dal payload serializzato.
        /// Riferimento spec: DS04-BL Regola 5.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? anno_impianto { get; set; }

        /// <summary>
        /// Identificativo finalita produttiva (<c>Reg_Impianti.GRFI_COD</c>) quando <c>CUL_COD != 0</c>.
        /// Campo opzionale e mutuamente esclusivo con <c>id_destinazione_uso</c>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? id_finalita { get; set; }

        /// <summary>
        /// Identificativo destinazione uso (<c>Reg_Impianti_Codici.id_cod</c> nel range 3000-4000)
        /// quando <c>CUL_COD = 0</c>. Campo opzionale e mutuamente esclusivo con <c>id_finalita</c>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? id_destinazione_uso { get; set; }

        /// <summary>
        /// Lista delle operazioni colturali associate all'impianto.
        /// Popolato da DS05-BL nella fase successiva.
        /// Riferimento spec: DS04-BL Regola 10.
        /// </summary>
        public List<OperazionePayload> operazioni { get; set; } = new List<OperazionePayload>();


        // ── Campi audit per Lookup_Sost_CO2_Colture_Chiavi — non serializzati nel payload M4 ──

        /// <summary>
        /// Codice varietà (<c>Reg_Impianti.CUL_COD</c>). Usato per la persistenza nel lookup colture.
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public int? Cul_Cod { get; set; }

        /// <summary>
        /// Codice elemento prodotto raccolto (<c>Movimenti_dettagli.Elem_Cod</c>).
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public int? Elem_Cod { get; set; }

        /// <summary>
        /// Codice materia prima raccolta (<c>Movimenti_dettagli.Mat_Cod</c>).
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public int? Mat_Cod { get; set; }

        /// <summary>
        /// Lotto del prodotto raccolto (<c>Movimenti_dettagli.Lotto</c>).
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public string? Lotto { get; set; }

        /// <summary>
        /// Codice nazione ISO dell'esercizio (<c>Esercizio.nazione</c>).
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public string? Stato { get; set; }

        /// <summary>
        /// Regione geografica dell'esercizio (<c>Esercizio.regione</c>).
        /// Non incluso nel payload JSON inviato al motore.
        /// </summary>
        [JsonIgnore]
        public string? Regione { get; set; }
    }
}
