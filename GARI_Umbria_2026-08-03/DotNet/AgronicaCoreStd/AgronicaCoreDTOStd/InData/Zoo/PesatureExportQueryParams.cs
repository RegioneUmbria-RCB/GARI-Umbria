using System;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    /// <summary>
    /// Parametri di input per DS07-API: GET /api/v1/animals/weighing-curves/export.
    /// Estende i filtri di DS06 con parametri specifici per il formato e la modalità di export.
    /// Vedere DS07-API, sezione "Specifiche Tecniche / Query Parameters".
    /// </summary>
    public class PesatureExportQueryParams
    {
        // ── Filtri (identici a DS06) ───────────────────────────────────────────────────────────────

        /// <summary>
        /// PIVA dell'azienda (farm) per la quale esportare le pesature.
        /// Passato esplicitamente dal client perché PivaSuperUser nel JWT può riferirsi
        /// al super-utente (es. 'inalca') e non alla singola azienda agricola.
        /// </summary>
        public string Piva { get; set; }

        /// <summary>Chiavi composite stalla "{piva}_{sa_cod}_{sta_num}", multi-select separato da virgola.</summary>
        public string StallaPKeys { get; set; }

        /// <summary>Chiavi composite razza "{GEN_COD}_{SPE_COD}_{RAZ_COD}", multi-select separato da virgola.</summary>
        public string RazzaKeys { get; set; }

        /// <summary>Data inizio range. Default: ultimi 90 giorni.</summary>
        public DateTime DateFrom { get; set; } = DateTime.Today.AddDays(-90);

        /// <summary>Data fine range. Default: oggi.</summary>
        public DateTime DateTo { get; set; } = DateTime.Today;

        /// <summary>Fattore accrescimento giornaliero kg/giorno (0.1-2.0). Default: 0.8.</summary>
        public double KgPerDay { get; set; } = 0.8;

        /// <summary>Soglia scostamento percentuale per alert flag. Default: 10.0.</summary>
        public double AlertThresholdPct { get; set; } = 10.0;

        /// <summary>Colonna ordinamento: lid | data_pesata | scostamento_pct. Default: lid.</summary>
        public string SortBy { get; set; } = "lid";

        /// <summary>Direzione ordinamento: ASC | DESC. Default: ASC.</summary>
        public string SortOrder { get; set; } = "ASC";

        // ── Parametri specifici export ─────────────────────────────────────────────────────────────

        /// <summary>Formato file: CSV | XLS. Default: CSV. PDF riservato a Fase 3.</summary>
        public string Format { get; set; } = "CSV";

        /// <summary>Modalità export: detailed | aggregated. Default: detailed.</summary>
        public string ViewMode { get; set; } = "detailed";

        /// <summary>
        /// Livello aggregazione (obbligatorio se ViewMode=aggregated):
        /// animal | razza_key | stalla_key.
        /// </summary>
        public string AggregationLevel { get; set; }

        /// <summary>Includi footer con metadata export. Default: true.</summary>
        public bool IncludeMetadata { get; set; } = true;
    }
}
