using System.Diagnostics;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using Microsoft.Extensions.Options;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.SemaforoDatiComuni;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.VerificaStatoSemaforo;

/// <summary>
/// Implements <see cref="IVerificaStatoSemaforoService"/> by delegating persistence to
/// <see cref="ISemaforoDatiComuni"/> and applying the semaphore state business rules.
/// Ref: DS11-BL – Pattern Framework: querylettura; Regole di Business.
/// </summary>
public sealed class VerificaStatoSemaforoService : IVerificaStatoSemaforoService
{
    private const string StatoLibero = "libero";
    private const string StatoOccupato = "occupato";

    private readonly ISemaforoDatiComuni _semaforoDAL;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public VerificaStatoSemaforoService(
        ISemaforoDatiComuni semaforoDAL,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _semaforoDAL = semaforoDAL;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<StatoSemaforoResult> VerificaStatoSemaforoAsync(AgronicaCoreParametriServer objParametriServer)
    {
        var stopwatch = Stopwatch.StartNew();

        // Ref: DS11-BL – Pattern Framework: querylettura
        var table = await _semaforoDAL.LeggiStatoSemaforoAsync(objParametriServer);

        stopwatch.Stop();
        var queryTimeMs = stopwatch.ElapsedMilliseconds;

        // Ref: DS11-BL – Regole di Business: warning log se la query supera il limite configurato.
        if (queryTimeMs > _settings.TimeoutMs)
        {
            _loggingService.LogWarning(
                "Semaphore state query exceeded maximum allowed time: {ElapsedMs} ms (max {MaxMs} ms).",
                objParametriServer,
                null,
                queryTimeMs,
                _settings.TimeoutMs);
        }

        // Ref: DS11-BL – Regola 1: nessun record trovato → libero
        if (table.Rows.Count == 0)
            return new StatoSemaforoResult(StatoLibero, null, 0, queryTimeMs);

        var row = table.Rows[0];
        int flagInCorso = Convert.ToInt32(row["flag_aggiornamento_in_corso"]);

        // Ref: DS11-BL – Regola 2: flag_aggiornamento_in_corso = 0 → libero
        if (flagInCorso == 0)
            return new StatoSemaforoResult(StatoLibero, null, 0, queryTimeMs);

        // Ref: DS11-BL – Regola 3: flag_aggiornamento_in_corso = 1 → occupato
        var timestampInizio = Convert.ToDateTime(row["timestamp_inizio"]);
        int durataSecondi = (int)(DateTime.Now - timestampInizio).TotalSeconds;

        return new StatoSemaforoResult(StatoOccupato, timestampInizio, durataSecondi, queryTimeMs);
    }
}
