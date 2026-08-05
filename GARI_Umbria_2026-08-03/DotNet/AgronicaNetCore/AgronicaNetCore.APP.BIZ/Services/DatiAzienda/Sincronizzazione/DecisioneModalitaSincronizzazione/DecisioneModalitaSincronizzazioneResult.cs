namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Final DS03 output combining the global mode, per-table decisions and summary counters.
/// Ref: DS03-BL – Output.
/// </summary>
public sealed class DecisioneModalitaSincronizzazioneResult
{
    /// <summary>
    /// Selected synchronisation mode.
    /// Ref: DS03-BL – Output: modalita_sincronizzazione.
    /// </summary>
    public string ModalitaSincronizzazione { get; }

    /// <summary>
    /// Final per-table payload decisions.
    /// Ref: DS03-BL – Output: tabelle_per_sincronizzazione.
    /// </summary>
    public IReadOnlyDictionary<string, DecisioneTabellaSincronizzazioneResult> TabellePerSincronizzazione { get; }

    /// <summary>
    /// Aggregate counters for the decision.
    /// Ref: DS03-BL – Output: riassunto_decisione.
    /// </summary>
    public RiassuntoDecisioneSincronizzazioneResult RiassuntoDecisione { get; }

    public DecisioneModalitaSincronizzazioneResult(
        string modalitaSincronizzazione,
        IReadOnlyDictionary<string, DecisioneTabellaSincronizzazioneResult> tabellePerSincronizzazione,
        RiassuntoDecisioneSincronizzazioneResult riassuntoDecisione)
    {
        ModalitaSincronizzazione = modalitaSincronizzazione;
        TabellePerSincronizzazione = tabellePerSincronizzazione;
        RiassuntoDecisione = riassuntoDecisione;
    }
}