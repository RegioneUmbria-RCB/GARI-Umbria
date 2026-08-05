namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.ResetSemaforoTimeout;

/// <summary>
/// Result returned by <see cref="IResetSemaforoTimeoutService.ResetSemaforoTimeoutAsync"/>.
/// Ref: DS12-BL – Output; DS16-API – risultato.id_record_resettato.
/// </summary>
/// <param name="JobTimeoutTrovato"><c>true</c> when a stuck job was detected and reset.</param>
/// <param name="IdRecordResettato">Database ID of the reset record, or <c>null</c> when no timeout was found.</param>
/// <param name="TimestampInizioJob">UTC start time of the reset job, or <c>null</c> when no timeout was found.</param>
/// <param name="DurataJobSecondi">Duration of the stuck job in seconds, or 0 when no timeout was found.</param>
/// <param name="EsitoReset">'semaforo_resettato' or 'nessun_timeout_rilevato'.</param>
public sealed record ResetSemaforoTimeoutResult(
    bool JobTimeoutTrovato,
    int? IdRecordResettato,
    DateTime? TimestampInizioJob,
    int DurataJobSecondi,
    string EsitoReset);
