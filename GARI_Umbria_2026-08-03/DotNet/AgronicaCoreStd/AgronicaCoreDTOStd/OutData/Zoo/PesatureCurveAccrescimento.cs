using System;
using System.Collections.Generic;

namespace OutData.Zoo
{
    /// <summary>
    /// Singola riga del dataset restituito dall'endpoint GET /api/v1/animals/weighing-curves.
    /// Vedere DS06-API, sezione "200 - Success / data[]".
    /// </summary>
    public class PesaturaCurvaAccrescimentoRow
    {
        /// <summary>Matricola (LID) dell'animale.</summary>
        public string Lid { get; set; }

        /// <summary>Data della pesata (UTC ISO 8601).</summary>
        public DateTime DataPesata { get; set; }

        /// <summary>Peso rilevato in kg (da Movimenti_dettagli.Qta).</summary>
        public double PesoKg { get; set; }

        /// <summary>
        /// Età in giorni al momento della pesata:
        /// DATEDIFF(day, Zoo_Animali.Dat_Nascita, Movimenti.Data_Movimento).
        /// </summary>
        public int EtaGiorni { get; set; }

        /// <summary>
        /// Peso teorico calcolato:
        /// Zoo_Animali.Peso + (eta_giorni × kg_per_day).
        /// </summary>
        public double PesoTeorico { get; set; }

        /// <summary>
        /// Scostamento percentuale:
        /// ((peso_kg - peso_teorico) / peso_teorico) × 100.
        /// </summary>
        public double ScostamentoPct { get; set; }

        /// <summary>
        /// True se |scostamento_pct| > alert_threshold_pct.
        /// </summary>
        public bool AlertFlag { get; set; }

        /// <summary>Chiave composita razza "{GEN_COD}_{SPE_COD}_{RAZ_COD}".</summary>
        public string RazzaKey { get; set; }

        /// <summary>Descrizione della razza (Lista_Razze_Animali.RAZ_DES).</summary>
        public string RazzaDes { get; set; }

        /// <summary>Chiave composita stalla "{piva}_{sa_cod}_{sta_num}".</summary>
        public string StallaKey { get; set; }

        /// <summary>Descrizione della stalla (Stalla.STA_DES).</summary>
        public string StaDes { get; set; }
    }

    /// <summary>
    /// Risposta completa dell'endpoint GET /api/v1/animals/weighing-curves.
    /// Vedere DS06-API, sezione "200 - Success".
    /// </summary>
    public class PesatureCurveAccrescimentoResult
    {
        /// <summary>Metadati della query eseguita.</summary>
        public PesatureCurveAccrescimentoMetadata Metadata { get; set; } = new PesatureCurveAccrescimentoMetadata();

        /// <summary>Righe di dati pesate con metriche calcolate.</summary>
        public IReadOnlyList<PesaturaCurvaAccrescimentoRow> Data { get; set; } = new List<PesaturaCurvaAccrescimentoRow>();
    }

    /// <summary>
    /// Metadati restituiti insieme ai dati dall'endpoint DS06.
    /// Vedere DS06-API, sezione "200 - Success / metadata".
    /// </summary>
    public class PesatureCurveAccrescimentoMetadata
    {
        /// <summary>Timestamp UTC della query.</summary>
        public DateTime QueryTimestamp { get; set; }

        /// <summary>Chiavi stalla usate come filtro (array vuoto se nessun filtro).</summary>
        public IReadOnlyList<string> StallaKeyFiltro { get; set; } = new List<string>();

        /// <summary>Chiavi razza usate come filtro (array vuoto se nessun filtro).</summary>
        public IReadOnlyList<string> RazzaKeyFiltro { get; set; } = new List<string>();

        /// <summary>Range di date applicato alla query.</summary>
        public PesatureDateRange DateRange { get; set; } = new PesatureDateRange();

        /// <summary>Fattore kg/giorno usato per il calcolo del peso teorico.</summary>
        public double KgPerDay { get; set; }

        /// <summary>Soglia percentuale usata per l'alert flag.</summary>
        public double AlertThresholdPct { get; set; }

        /// <summary>Numero totale di animali distinti nei risultati.</summary>
        public int TotalAnimals { get; set; }

        /// <summary>Numero totale di pesate nei risultati (prima della paginazione).</summary>
        public int TotalWeighings { get; set; }

        /// <summary>Dettagli paginazione.</summary>
        public PesaturePagination Pagination { get; set; } = new PesaturePagination();
    }

    /// <summary>Range di date applicato alla query DS06.</summary>
    public class PesatureDateRange
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    /// <summary>Informazioni di paginazione restituite dall'endpoint DS06.</summary>
    public class PesaturePagination
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
    }

    /// <summary>
    /// Riga aggregata restituita da GET /api/v1/animals/weighing-curves?view_mode=aggregated.
    /// Ogni riga rappresenta un gruppo (animale, razza o stalla) con le relative statistiche.
    /// </summary>
    public class PesaturaCurvaAccrescimentoAggregatedRow
    {
        /// <summary>Chiave del gruppo (LID per animal, razza_key per razza, stalla_key per stalla).</summary>
        public string GruppoKey { get; set; } = string.Empty;

        /// <summary>Descrizione del gruppo (es. nome razza o nome stalla).</summary>
        public string GruppoDes { get; set; } = string.Empty;

        /// <summary>Numero di animali distinti nel gruppo.</summary>
        public int NumeroAnimali { get; set; }

        /// <summary>Numero totale di pesate nel gruppo.</summary>
        public int NumeroPesate { get; set; }

        /// <summary>Scostamento percentuale medio nel gruppo, arrotondato a 2 decimali.</summary>
        public double ScostamentoMedioPct { get; set; }

        /// <summary>Scostamento percentuale massimo nel gruppo, arrotondato a 2 decimali.</summary>
        public double ScostamentoMaxPct { get; set; }

        /// <summary>Scostamento percentuale minimo nel gruppo, arrotondato a 2 decimali.</summary>
        public double ScostamentoMinPct { get; set; }

        /// <summary>Numero di pesate in alert nel gruppo.</summary>
        public int AlertCount { get; set; }
    }

    /// <summary>
    /// Risposta aggregata di GET /api/v1/animals/weighing-curves?view_mode=aggregated.
    /// </summary>
    public class PesatureCurveAccrescimentoAggregatedResult
    {
        /// <summary>Metadati della query eseguita.</summary>
        public PesatureCurveAccrescimentoMetadata Metadata { get; set; } = new PesatureCurveAccrescimentoMetadata();

        /// <summary>Righe aggregate per gruppo.</summary>
        public IReadOnlyList<PesaturaCurvaAccrescimentoAggregatedRow> Data { get; set; } =
            new List<PesaturaCurvaAccrescimentoAggregatedRow>();
    }
}
