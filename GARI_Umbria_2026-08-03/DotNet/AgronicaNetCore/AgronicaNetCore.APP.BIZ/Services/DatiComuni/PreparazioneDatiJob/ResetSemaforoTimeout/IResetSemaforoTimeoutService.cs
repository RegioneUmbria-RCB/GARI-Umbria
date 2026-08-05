using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.ResetSemaforoTimeout;

/// <summary>
/// Watchdog service that detects stuck semaphore jobs (running beyond the configured timeout)
/// and resets them automatically to prevent indefinite deadlocks.
/// Ref: DS12-BL – Nome: ResetSemaforoTimeout; Scopo.
/// </summary>
public interface IResetSemaforoTimeoutService
{
    /// <summary>
    /// Searches for a semaphore record running longer than <paramref name="timeoutMinuti"/> minutes
    /// and, if found, resets flag_aggiornamento_in_corso = 0.
    /// Ref: DS12-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="timeoutMinuti">Configurable timeout threshold in minutes (default 5).</param>
    /// <param name="objParametriServer">Server-level connection parameters.</param>
    /// <returns>A <see cref="ResetSemaforoTimeoutResult"/> describing what was found and done.</returns>
    /// <exception cref="Exceptions.ResetFailedException">
    /// Thrown when the UPDATE to reset the stuck semaphore record fails.
    /// Ref: DS12-BL – Eccezioni: ResetFailedException.
    /// </exception>
    Task<ResetSemaforoTimeoutResult> ResetSemaforoTimeoutAsync(
        int timeoutMinuti,
        AgronicaCoreParametriServer objParametriServer);
}
