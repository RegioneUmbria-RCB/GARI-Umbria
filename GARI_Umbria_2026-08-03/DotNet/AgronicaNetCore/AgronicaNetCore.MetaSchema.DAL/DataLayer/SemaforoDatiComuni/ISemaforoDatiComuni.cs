using System.Data;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.SemaforoDatiComuni;

/// <summary>
/// Data-access contract for <c>app_semaforo_daticomuni_web2app</c>.
/// Ref: DS06-BL – Persistenze Coinvolte.
/// </summary>
public interface ISemaforoDatiComuni
{
    /// <summary>
    /// Returns <c>true</c> if a semaphore record with flag_aggiornamento_in_corso = 1 exists.
    /// Ref: DS06-BL – Regole di Business: Prima dell'inizio job.
    /// </summary>
    Task<bool> VerificaSemaforoAttivoAsync(AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Inserts a new semaphore record with flag_aggiornamento_in_corso = 1.
    /// Ref: DS06-BL – Regole di Business: Inizio job.
    /// </summary>
    Task InserisciSemaforoInizioAsync(DateTime timestampInizio, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Updates the semaphore record identified by <paramref name="timestampInizio"/> with finish data.
    /// Ref: DS06-BL – Regole di Business: Fine job.
    /// </summary>
    Task AggiornaSemaforoFineAsync(
        DateTime timestampInizio,
        DateTime timestampFine,
        int durataSecondi,
        bool successo,
        string? messaggioStato,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Reads the most recent record from <c>app_semaforo_daticomuni_web2app</c>
    /// (ORDER BY timestamp_inizio DESC TOP 1), returning <c>flag_aggiornamento_in_corso</c>
    /// and <c>timestamp_inizio</c>.
    /// Ref: DS11-BL – Pattern Framework: querylettura; Persistenze Coinvolte.
    /// </summary>
    Task<DataTable> LeggiStatoSemaforoAsync(AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Returns the most recent semaphore record whose job has been running longer than
    /// <paramref name="timeoutMinuti"/> minutes (flag_aggiornamento_in_corso = 1
    /// AND DATEDIFF(MINUTE, timestamp_inizio, GETUTCDATE()) &gt; timeoutMinuti).
    /// Returns an empty <see cref="DataTable"/> when no stuck job is found.
    /// Ref: DS12-BL – Pattern Framework: querylettura; Persistenze Coinvolte.
    /// </summary>
    Task<DataTable> CercaJobInTimeoutAsync(int timeoutMinuti, AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Resets the stuck semaphore record identified by <paramref name="idRecord"/>:
    /// sets flag_aggiornamento_in_corso = 0, flag_aggiornamento_andato_bene = 0,
    /// timestamp_fine = NOW(), durata_secondi, messaggio_stato = 'Job timeout detected',
    /// Data_Modifica = GETDATE(), Username_Modifica = 'agronica_watchdog'.
    /// Ref: DS12-BL – Pattern Framework: queryupdate; Regole di Business.
    /// </summary>
    Task ResetSemaforoTimeoutAsync(int idRecord, DateTime timestampInizio, AgronicaCoreParametriServer objParametriServer);
}
