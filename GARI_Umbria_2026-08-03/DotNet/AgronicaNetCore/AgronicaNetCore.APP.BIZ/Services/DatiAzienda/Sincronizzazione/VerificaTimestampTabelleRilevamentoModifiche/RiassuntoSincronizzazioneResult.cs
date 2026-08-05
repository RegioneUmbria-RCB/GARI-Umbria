namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Aggregate summary of the DS02 table-verification outcome.
/// Ref: DS02-BL – Output: riassunto_sincronizzazione.
/// </summary>
public sealed class RiassuntoSincronizzazioneResult
{
    /// <summary>Total number of evaluated tables. Ref: DS02-BL – Output: totale_tabelle.</summary>
    public int TotaleTabelle { get; }

    /// <summary>
    /// Number of tables marked for synchronisation.
    /// Ref: DS02-BL – Output: tabelle_da_sincronizzare.
    /// </summary>
    public int TabelleDaSincronizzare { get; }

    /// <summary>Number of omitted tables. Ref: DS02-BL – Output: tabelle_omesse.</summary>
    public int TabelleOmesse { get; }

    /// <summary>
    /// True when at least one table requires synchronisation.
    /// Ref: DS02-BL – Output: sincronizzazione_completa_richiesta.
    /// </summary>
    public bool SincronizzazioneCompletaRichiesta { get; }

    public RiassuntoSincronizzazioneResult(
        int totaleTabelle,
        int tabelleDaSincronizzare,
        int tabelleOmesse,
        bool sincronizzazioneCompletaRichiesta)
    {
        TotaleTabelle = totaleTabelle;
        TabelleDaSincronizzare = tabelleDaSincronizzare;
        TabelleOmesse = tabelleOmesse;
        SincronizzazioneCompletaRichiesta = sincronizzazioneCompletaRichiesta;
    }
}