namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggiornamentoAtomicoPreparazione;

/// <summary>
/// Output of <see cref="IAggiornamentoAtomicoPreparazioneService.AggiornaAsync"/>.
/// Ref: DS05-BL – Output.
/// </summary>
public sealed class AggiornamentoAtomicoResult
{
    /// <summary>
    /// Number of tables updated (row already existed).
    /// Ref: DS05-BL – Output: righe_aggiornate.
    /// </summary>
    public int RigheAggiornate { get; }

    /// <summary>
    /// Number of tables inserted (first execution, row did not exist).
    /// Ref: DS05-BL – Output: righe_inserte.
    /// </summary>
    public int RigheInserte { get; }

    /// <summary>
    /// <c>"success"</c> if the transaction committed; <c>"rollback"</c> if rolled back.
    /// Ref: DS05-BL – Output: esito.
    /// </summary>
    public string Esito { get; }

    /// <summary>
    /// Milliseconds elapsed for the entire transaction.
    /// Ref: DS05-BL – Output: transaction_time_ms.
    /// </summary>
    public long TransactionTimeMs { get; }

    public AggiornamentoAtomicoResult(int righeAggiornate, int righeInserte, string esito, long transactionTimeMs)
    {
        RigheAggiornate = righeAggiornate;
        RigheInserte = righeInserte;
        Esito = esito;
        TransactionTimeMs = transactionTimeMs;
    }

    public static AggiornamentoAtomicoResult Success(int updated, int inserted, long ms) =>
        new(updated, inserted, "success", ms);

    public static AggiornamentoAtomicoResult Rollback(long ms) => new(0, 0, "rollback", ms);
}
