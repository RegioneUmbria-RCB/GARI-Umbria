using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.VerificaStatoSemaforo;

/// <summary>
/// Verifies the current state of the concurrency semaphore to determine whether
/// an aggregation job is in progress.
/// Ref: DS11-BL – Nome: VerificaStatoSemaforo; Scopo.
/// </summary>
public interface IVerificaStatoSemaforoService
{
    /// <summary>
    /// Queries <c>app_semaforo_daticomuni_web2app</c> for the most recent record and
    /// returns the semaphore state together with timing metadata.
    /// Ref: DS11-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="objParametriServer">Server-level connection parameters.</param>
    /// <returns>A <see cref="StatoSemaforoResult"/> describing the current semaphore state.</returns>
    /// <exception cref="Exceptions.SemaphoreQueryTimeoutException">
    /// Thrown when the query exceeds 20 ms.
    /// Ref: DS11-BL – Eccezioni: SemaphoreQueryTimeoutException.
    /// </exception>
    Task<StatoSemaforoResult> VerificaStatoSemaforoAsync(AgronicaCoreParametriServer objParametriServer);
}
