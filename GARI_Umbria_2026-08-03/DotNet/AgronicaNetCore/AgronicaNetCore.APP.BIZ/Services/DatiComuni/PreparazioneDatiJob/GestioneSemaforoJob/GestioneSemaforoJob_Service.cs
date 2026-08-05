using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SemaforoDatiComuni;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.GestioneSemaforoJob;

/// <summary>
/// Coordinates the semaphore lifecycle for the job execution.
/// Delegates all persistence to <see cref="ISemaforoDatiComuni"/>.
/// Ref: DS06-BL – Pattern Framework: queryinsert e queryupdate del TECH_FRAMEWORK.
/// </summary>
public sealed class GestioneSemaforoJobService : IGestioneSemaforoJobService
{
    private readonly ISemaforoDatiComuni _semaforoDAL;
    private readonly ILoggingService _loggingService;

    public GestioneSemaforoJobService(
        ISemaforoDatiComuni semaforoDAL,
        ILoggingService loggingService)
    {
        _semaforoDAL = semaforoDAL;
        _loggingService = loggingService;
    }

    /// <inheritdoc/>
    public async Task<ImpostaSemaforoResult> ImpostaSemaforoInizioAsync(AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS06-BL – Prima dell'inizio job: verifica flag_aggiornamento_in_corso = 1
        bool semaforoAttivo = await _semaforoDAL.VerificaSemaforoAttivoAsync(objParametriServer);
        if (semaforoAttivo)
            throw new JobAlreadyRunningException(
                "Job already running: active semaphore (flag_aggiornamento_in_corso = 1) found in app_semaforo_daticomuni_web2app.");

        // Ref: DS06-BL – Inizio job: INSERT record semaforo con timestamp_inizio = NOW(UTC)
        var timestampInizio = DateTime.Now;

        await _semaforoDAL.InserisciSemaforoInizioAsync(timestampInizio, objParametriServer);

         return new ImpostaSemaforoResult(timestampInizio, "semaforo_impostato");
    }

    /// <inheritdoc/>
    public async Task<AggiornaSemaforoFineResult> AggiornaSemaforoFineAsync(
        DateTime timestampInizio,
        bool successo,
        string? messaggioStato,
        AgronicaCoreParametriServer objParametriServer)
    {
        var timestampFine = DateTime.Now;
        var durataSecondi = (int)(timestampFine - timestampInizio).TotalSeconds;

        try
        {
            // Ref: DS06-BL – Fine job: UPDATE semaforo con timestamp_fine, durata, flag, messaggio
            await _semaforoDAL.AggiornaSemaforoFineAsync(
                timestampInizio,
                timestampFine,
                durataSecondi,
                successo,
                messaggioStato,
                objParametriServer);
        }
        catch (Exception ex)
        {
            _loggingService.LogError("Failed to update semaphore for timestampInizio={TimestampInizio}.", objParametriServer, ex, timestampInizio);
            throw new SemaphoreUpdateFailedException($"Failed to update semaphore record: {ex.Message}", ex);
        }

        return new AggiornaSemaforoFineResult(durataSecondi, "semaforo_aggiornato");
    }
}
