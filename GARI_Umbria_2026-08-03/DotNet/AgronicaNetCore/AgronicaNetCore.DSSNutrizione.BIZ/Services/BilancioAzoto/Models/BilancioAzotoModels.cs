namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.BilancioAzoto.Models
{
    // ── DS09-BL: BilancioAzotoAggregazioneEventi — Output ───────────────────────

    /// <summary>
    /// Singolo evento della timeline annuale che modifica i parametri di input
    /// per il calcolo nutrizionale (cambio BBCH, nuova analisi terreno, fertilizzazione).
    /// Riferimento: DS09-BL — Output.timeline_eventi.
    /// </summary>
    public sealed class EventoTimelineDto
    {
        /// <summary>Data dell'evento in formato ISO 8601.</summary>
        public string DataEvento { get; init; } = string.Empty;
        /// <summary>Identificativo univoco dell'evento (UUID).</summary>
        public string EventoId { get; init; } = string.Empty;
        /// <summary>Tipo evento: BBCH_CHANGE | ANALISI_TERRENO | FERTILIZZAZIONE.</summary>
        public string TipoEvento { get; init; } = string.Empty;
        /// <summary>Descrizione leggibile dell'evento.</summary>
        public string Descrizione { get; init; } = string.Empty;
    }

    /// <summary>
    /// Richiesta consiglio aggregata per una data specifica della timeline.
    /// Se nella medesima data si verificano più eventi, vengono uniti in un'unica richiesta.
    /// Riferimento: DS09-BL — Output.richieste_consigli_aggregate; Regole di Business (aggregazione per data).
    /// </summary>
    public sealed class RichiestaConsiglioAggregataDto
    {
        /// <summary>Data della richiesta in formato ISO 8601.</summary>
        public string DataRichiesta { get; init; } = string.Empty;
        /// <summary>Numero di eventi della timeline aggregati in questa richiesta.</summary>
        public int NumeroEventiAggregati { get; init; }
        /// <summary>ID del consiglio nutrizionale persistito; null se il salvataggio non è andato a buon fine.</summary>
        public int? ConsiglioId { get; init; }
    }

    /// <summary>
    /// Risultato dell'aggregazione eventi e richieste consigli per il bilancio azoto di un appezzamento.
    /// Riferimento: DS09-BL — Output.
    /// </summary>
    public sealed class BilancioAzotoResult
    {
        /// <summary>Timeline ordinata cronologicamente di tutti gli eventi che modificano i parametri nutrizionali.</summary>
        public IReadOnlyList<EventoTimelineDto> TimelineEventi { get; init; } = Array.Empty<EventoTimelineDto>();
        /// <summary>
        /// Lista delle richieste consigli aggregate per data.
        /// Ciascuna voce corrisponde a una singola chiamata all'engine nutrizionale.
        /// </summary>
        public IReadOnlyList<RichiestaConsiglioAggregataDto> RichiesteConsiglioAggregate { get; init; } = Array.Empty<RichiestaConsiglioAggregataDto>();
    }
}
