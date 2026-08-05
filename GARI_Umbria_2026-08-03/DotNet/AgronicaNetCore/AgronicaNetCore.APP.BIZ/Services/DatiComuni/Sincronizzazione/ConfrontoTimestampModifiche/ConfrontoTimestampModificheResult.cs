namespace AgronicaNetCore.APP.BIZ.Services.ConfrontoTimestampModifiche;

/// <summary>
/// Result returned by <see cref="IConfrontoTimestampModificheService.ConfrontaAsync"/>.
/// Ref: DS08-BL – Output.
/// </summary>
public sealed class ConfrontoTimestampModificheResult
{
    /// <summary>
    /// <c>true</c> when server data are more recent than the client timestamp or no server
    /// data exist yet (first synchronisation); <c>false</c> when the client is already up-to-date.
    /// Ref: DS08-BL – Output: esistono_modifiche.
    /// </summary>
    public bool EsistonoModifiche { get; }

    /// <summary>
    /// UTC timestamp to be stored by the client after a successful synchronisation.
    /// Equals <c>MAX(timestamp_aggiornamento)</c> when <see cref="EsistonoModifiche"/> is
    /// <c>true</c> and a non-null MAX was found; equals server <c>NOW()</c> otherwise.
    /// Ref: DS08-BL – Output: timestamp_server.
    /// </summary>
    public DateTime TimestampServer { get; }

    /// <summary>
    /// Milliseconds elapsed during the whole comparison operation (DB query + comparison).
    /// Ref: DS08-BL – Output: comparison_time_ms.
    /// </summary>
    public long ComparisonTimeMs { get; }

    public ConfrontoTimestampModificheResult(
        bool esistonoModifiche,
        DateTime timestampServer,
        long comparisonTimeMs)
    {
        EsistonoModifiche = esistonoModifiche;
        TimestampServer = timestampServer;
        ComparisonTimeMs = comparisonTimeMs;
    }
}
