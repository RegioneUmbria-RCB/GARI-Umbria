using System;

namespace AgronicaCoreDTOStd.InData.Zoo
{
    /// <summary>
    /// Parametri di query per l'endpoint GET /api/v1/animals/weighing-curves.
    /// Vedere DS06-API: Endpoint Query Metriche Curva Accrescimento, sezione "Query Parameters".
    /// </summary>
    public class PesatureCurveAccrescimentoQueryParams
    {
        /// <summary>
        /// PIVA dell'azienda (farm) per la quale recuperare le pesature.
        /// Passato esplicitamente dal client perché PivaSuperUser nel JWT può riferirsi
        /// al super-utente (es. 'inalca') e non alla singola azienda agricola.
        /// </summary>
        public string Piva { get; set; }

        /// <summary>
        /// Chiavi composite stalla "{piva}_{sa_cod}_{sta_num}", separati da virgola per multi-select.
        /// Il segmento piva viene validato server-side contro il piva estratto dal JWT.
        /// </summary>
        public string StallaPKeys { get; set; }

        /// <summary>
        /// Chiavi composite razza "{GEN_COD}_{SPE_COD}_{RAZ_COD}", separati da virgola per multi-select.
        /// </summary>
        public string RazzaKeys { get; set; }

        /// <summary>Data inizio range (default: ultimi 90 giorni).</summary>
        public DateTime DateFrom { get; set; }

        /// <summary>Data fine range (default: oggi).</summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Fattore accrescimento giornaliero in kg/giorno (range 0.1-2.0).
        /// Sovrascrive Zoo_Animali.Incremento_Teorico. Default 0.8.
        /// </summary>
        public double KgPerDay { get; set; } = 0.8;

        /// <summary>
        /// Soglia scostamento percentuale per attivare l'alert flag. Default 10.0.
        /// </summary>
        public double AlertThresholdPct { get; set; } = 10.0;

        /// <summary>Numero pagina (1-based). Default 1.</summary>
        public int Page { get; set; } = 1;

        /// <summary>Numero di record per pagina (max 10000). Default 100.</summary>
        public int Limit { get; set; } = 100;

        /// <summary>Colonna di ordinamento: lid, data_pesata, scostamento_pct. Default "lid".</summary>
        public string SortBy { get; set; } = "lid";

        /// <summary>Direzione di ordinamento: ASC o DESC. Default "ASC".</summary>
        public string SortOrder { get; set; } = "ASC";

        /// <summary>Modalità risposta: detailed (una riga per pesatura) | aggregated (aggregazione per gruppo). Default "detailed".</summary>
        public string ViewMode { get; set; } = "detailed";

        /// <summary>Livello aggregazione quando view_mode=aggregated: animal | razza_key | stalla_key. Default "animal".</summary>
        public string AggregationLevel { get; set; } = "animal";
    }
}
