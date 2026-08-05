namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Result returned by <see cref="IVerificaTimestampTabelleRilevamentoModificheService.VerificaAsync"/>.
/// Ref: DS02-BL – Output.
/// </summary>
public sealed class VerificaTimestampTabelleRilevamentoModificheResult
{
    /// <summary>
    /// Per-table synchronisation decisions, keyed by logical table name.
    /// Ref: DS02-BL – Output: modifiche_rilevate.
    /// </summary>
    public IReadOnlyDictionary<string, VerificaTimestampTabellaResult> ModificheRilevate { get; }

    /// <summary>
    /// Aggregate synchronisation summary.
    /// Ref: DS02-BL – Output: riassunto_sincronizzazione.
    /// </summary>
    public RiassuntoSincronizzazioneResult RiassuntoSincronizzazione { get; }

    public VerificaTimestampTabelleRilevamentoModificheResult(
        IReadOnlyDictionary<string, VerificaTimestampTabellaResult> modificheRilevate,
        RiassuntoSincronizzazioneResult riassuntoSincronizzazione)
    {
        ModificheRilevate = modificheRilevate;
        RiassuntoSincronizzazione = riassuntoSincronizzazione;
    }
}