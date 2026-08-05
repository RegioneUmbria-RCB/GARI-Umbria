namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.DecisioneModalitaSincronizzazione;

/// <summary>
/// Aggregate DS03 summary for the selected synchronisation mode.
/// Ref: DS03-BL – Output: riassunto_decisione.
/// </summary>
public sealed class RiassuntoDecisioneSincronizzazioneResult
{
    /// <summary>
    /// Number of evaluated tables.
    /// Ref: DS03-BL – Output: totale_tabelle.
    /// </summary>
    public int TotaleTabelle { get; }

    /// <summary>
    /// Number of tables selected for synchronisation.
    /// Ref: DS03-BL – Output: tabelle_sincronizzate.
    /// </summary>
    public int TabelleSincronizzate { get; }

    /// <summary>
    /// Number of tables omitted from the payload.
    /// Ref: DS03-BL – Output: tabelle_omesse.
    /// </summary>
    public int TabelleOmesse { get; }

    /// <summary>
    /// True when DS01 forced a full synchronisation regardless of DS02 results.
    /// Ref: DS03-BL – Output: forza_full_sync_attiva.
    /// </summary>
    public bool ForzaFullSyncAttiva { get; }

    public RiassuntoDecisioneSincronizzazioneResult(
        int totaleTabelle,
        int tabelleSincronizzate,
        int tabelleOmesse,
        bool forzaFullSyncAttiva)
    {
        TotaleTabelle = totaleTabelle;
        TabelleSincronizzate = tabelleSincronizzate;
        TabelleOmesse = tabelleOmesse;
        ForzaFullSyncAttiva = forzaFullSyncAttiva;
    }
}