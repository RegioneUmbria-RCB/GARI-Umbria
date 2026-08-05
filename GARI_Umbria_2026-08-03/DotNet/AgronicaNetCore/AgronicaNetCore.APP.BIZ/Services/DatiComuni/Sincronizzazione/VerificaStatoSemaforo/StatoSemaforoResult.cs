namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.VerificaStatoSemaforo;

/// <summary>
/// Result returned by <see cref="IVerificaStatoSemaforoService.VerificaStatoSemaforoAsync"/>.
/// Ref: DS11-BL – Output.
/// </summary>
/// <param name="StatoSemaforo">'libero' or 'occupato'.</param>
/// <param name="TimestampInizioJob">UTC start time of the running job, or <c>null</c> when the semaphore is free.</param>
/// <param name="DurataTrascorsaSecondi">Seconds elapsed since job start, or 0 when the semaphore is free.</param>
/// <param name="QueryTimeMs">Milliseconds taken to execute the semaphore query.</param>
public sealed record StatoSemaforoResult(
    string StatoSemaforo,
    DateTime? TimestampInizioJob,
    int DurataTrascorsaSecondi,
    long QueryTimeMs);
