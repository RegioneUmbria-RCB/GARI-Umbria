using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.GestioneSemaforoJob;

/// <summary>
/// Manages the concurrency semaphore lifecycle for the aggregation job
/// against table <c>app_semaforo_daticomuni_web2app</c>.
/// Ref: DS06-BL – Nome: GestioneSemaforoJob.
/// </summary>
public interface IGestioneSemaforoJobService
{
    /// <summary>
    /// Verifies no active semaphore exists, then inserts a new semaphore record with
    /// flag_aggiornamento_in_corso = 1 and timestamp_inizio = NOW(UTC).
    /// Ref: DS06-BL – Regole di Business: Prima dell'inizio job, Inizio job.
    /// </summary>
    /// <exception cref="Exceptions.JobAlreadyRunningException">
    /// Thrown when a record with flag_aggiornamento_in_corso = 1 already exists.
    /// </exception>
    Task<ImpostaSemaforoResult> ImpostaSemaforoInizioAsync(AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Updates the semaphore record identified by <paramref name="timestampInizio"/> with
    /// end timestamp, duration, success flag and optional status message.
    /// Ref: DS06-BL – Regole di Business: Fine job.
    /// </summary>
    /// <exception cref="Exceptions.SemaphoreUpdateFailedException">
    /// Thrown when the UPDATE on the semaphore record fails.
    /// </exception>
    Task<AggiornaSemaforoFineResult> AggiornaSemaforoFineAsync(
        DateTime timestampInizio,
        bool successo,
        string? messaggioStato,
        AgronicaCoreParametriServer objParametriServer);
}
