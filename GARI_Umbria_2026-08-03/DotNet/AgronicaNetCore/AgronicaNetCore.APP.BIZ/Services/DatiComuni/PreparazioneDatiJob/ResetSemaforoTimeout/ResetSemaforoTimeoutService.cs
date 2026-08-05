using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SemaforoDatiComuni;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.ResetSemaforoTimeout;

/// <summary>
/// Implements the watchdog logic for detecting and resetting stuck semaphore jobs.
/// Delegates all persistence to <see cref="ISemaforoDatiComuni"/>.
/// Ref: DS12-BL – Pattern Framework: querylettura + queryupdate.
/// </summary>
public sealed class ResetSemaforoTimeoutService : IResetSemaforoTimeoutService
{
    private const string EsitoResettato     = "semaforo_resettato";
    private const string EsitoNessunTimeout = "nessun_timeout_rilevato";

    private readonly ISemaforoDatiComuni _semaforoDAL;
    private readonly ILoggingService _loggingService;

    public ResetSemaforoTimeoutService(
        ISemaforoDatiComuni semaforoDAL,
        ILoggingService loggingService)
    {
        _semaforoDAL = semaforoDAL;
        _loggingService = loggingService;
    }

    /// <inheritdoc/>
    public async Task<ResetSemaforoTimeoutResult> ResetSemaforoTimeoutAsync(
        int timeoutMinuti,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS12-BL – Regola 1: cerca job con flag=1 oltre soglia timeout
        var table = await _semaforoDAL.CercaJobInTimeoutAsync(timeoutMinuti, objParametriServer);

        // Ref: DS12-BL – Regola 2: nessun record trovato → nessun_timeout_rilevato
        if (table.Rows.Count == 0)
            return new ResetSemaforoTimeoutResult(false, null, null, 0, EsitoNessunTimeout);

        var row             = table.Rows[0];
        int idRecord        = Convert.ToInt32(row["id"]);
        var timestampInizio = Convert.ToDateTime(row["timestamp_inizio"]);
        var durataSecondi   = (int)(DateTime.Now - timestampInizio).TotalSeconds;

        // Ref: DS12-BL – Regola 3: registrare evento WARN per troubleshooting
        _loggingService.LogWarning(
            "Stuck semaphore job detected: id={Id}, timestampInizio={TimestampInizio}, durataSecondi={DurataSecondi}. Starting automatic reset.",
            objParametriServer,
            null,
            idRecord, timestampInizio, durataSecondi);

        try
        {
            // Ref: DS12-BL – Regola 3: UPDATE flag=0, flag_bene=0, timestamp_fine, durata, messaggio, colonne GIAS
            await _semaforoDAL.ResetSemaforoTimeoutAsync(idRecord, timestampInizio, objParametriServer);
        }
        catch (Exception ex)
        {
            throw new ResetFailedException(
                $"Failed to reset stuck semaphore record id={idRecord}: {ex.Message}", ex);
        }

        return new ResetSemaforoTimeoutResult(true, idRecord, timestampInizio, durataSecondi, EsitoResettato);
    }
}
