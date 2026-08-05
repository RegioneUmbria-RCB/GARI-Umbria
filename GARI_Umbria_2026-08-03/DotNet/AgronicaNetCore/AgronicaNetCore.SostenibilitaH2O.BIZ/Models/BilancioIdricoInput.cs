namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Pre-aggregated input data for a single exercise used by the water balance calculation engine.
    /// All nullable decimal fields represent optional data; when null, the indicator will be INDETERMINATO.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Input.
    /// </summary>
    public class BilancioIdricoInput
    {
        /// <summary>Identificativo dell'esercizio (riportato nell'output).</summary>
        public string IdEsercizio { get; set; } = string.Empty;

        /// <summary>
        /// Volume totale di acqua irrigua effettivamente utilizzato (m³).
        /// Null se nessuna operazione di irrigazione trovata (restituisce INDETERMINATO).
        /// Riferimento DS03-BL RecuperoConsumoIdricoEffettivo.
        /// </summary>
        public decimal? ConsumoIdricoEffettivoM3 { get; set; }

        /// <summary>
        /// Resa produttiva in tonnellate.
        /// Null se nessuna raccolta trovata (restituisce INDETERMINATO).
        /// Riferimento DS04-BL RecuperoResaProduttiva.
        /// </summary>
        public decimal? ResaTonnellate { get; set; }

        /// <summary>
        /// Precipitazioni totali nel periodo dell'esercizio (mm).
        /// Null se nessuna stazione meteo disponibile (restituisce INDETERMINATO).
        /// Riferimento DS05-BL RecuperoPrecipitazioniM6.
        /// </summary>
        public decimal? PrecipitazioniMm { get; set; }

        /// <summary>
        /// Superficie dell'appezzamento in ettari (ha). Deve essere > 0.
        /// Riferimento spec: DS07-BL — Validazione obbligatoria: superficie > 0.
        /// </summary>
        public decimal SuperficieAppezzamentoHa { get; set; }

        /// <summary>
        /// Valore Green-Blue Water Footprint (m³/t) ottenuto dalla tabella waterfootprint. Deve essere > 0.
        /// Riferimento DS06-BL LookupBenchmarkWaterFootprint.
        /// </summary>
        public decimal GreenBlueWfM3PerT { get; set; }

        /// <summary>
        /// Modalità di calcolo del bilancio idrico.
        /// Valori ammessi: <c>PER_COLTURE</c> | <c>PER_AZIENDA</c>.
        /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Input modalita_calcolo.
        /// </summary>
        public string ModalitaCalcolo { get; set; } = string.Empty;
    }
}
