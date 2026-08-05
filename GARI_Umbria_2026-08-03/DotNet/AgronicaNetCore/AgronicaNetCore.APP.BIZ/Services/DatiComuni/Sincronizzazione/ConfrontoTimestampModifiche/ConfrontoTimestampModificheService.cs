using System.Data;
using System.Diagnostics;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.APP.BIZ.Common;
using Microsoft.Extensions.Options;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

namespace AgronicaNetCore.APP.BIZ.Services.ConfrontoTimestampModifiche;

/// <summary>
/// Compares the client's last-sync timestamp against <c>MAX(timestamp_aggiornamento)</c>
/// in <c>app_preparazione_daticomuni_web2app</c> to determine whether sync data must be sent.
/// Ref: DS08-BL – Nome: ConfrontoTimestampModifiche.
/// </summary>
public sealed class ConfrontoTimestampModificheService : IConfrontoTimestampModificheService
{
    private readonly ITimestampAggiornamentoDatiComuni _timestampDal;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public ConfrontoTimestampModificheService(
        ITimestampAggiornamentoDatiComuni timestampDal,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _timestampDal = timestampDal;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<ConfrontoTimestampModificheResult> ConfrontaAsync(
        DateTime timestampClient,
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Capture server UTC now before any async work so all comparisons use a consistent reference.
        DateTime serverNow = DateTime.Now;

        // Fail-fast: reject timestamps coming from the future (clock skew / manipulation).
        // Ref: DS08-BL – Regole di Business: range check timestamp_client.
        if (timestampClient > serverNow.AddMinutes(_settings.ClockSkewToleranceMinutes))
        {
            throw new InvalidTimestampException(
                $"Il timestamp client ({timestampClient:O}) è nel futuro rispetto al server " +
                $"({serverNow:O}). Possibile manipolazione o skew orologio. Ref: DS08-BL."
            );
        }

        var sw = Stopwatch.StartNew();

        DataTable result;
        try
        {
            result = await _timestampDal.LeggiTimestampAggiornamentoAsync(
                tabelleRichieste,
                objParametriServer
            );
        }
        catch (Exception ex)
        {
            throw new DataAccessException(
                "Errore nella lettura di MAX(timestamp_aggiornamento) da app_preparazione_daticomuni_web2app.",
                ex
            );
        }

        sw.Stop();
        long elapsedMs = sw.ElapsedMilliseconds;

        // Ref: DS08-BL – Regole di Business: loggare warning se il confronto supera il limite configurato.
        int timeoutMs = _settings.TimeoutMs;
        if (elapsedMs > timeoutMs)
        {
            _loggingService.LogWarning(
                "ConfrontoTimestamp ha superato il timeout di {TimeoutMs} ms: {ElapsedMs}ms. Ref: DS08-BL.",
                objParametriServer,
                null,
                timeoutMs,
                elapsedMs
            );
        }

        // Extract MAX timestamp; null means no rows have ever been written (first sync ever).
        // Ref: DS08-BL – Regole di Business: Gestire caso MAX = NULL.
        DateTime? maxTimestampServer = result.Rows.Count > 0
            && result.Rows[0]["max_timestamp"] != DBNull.Value
            ? Convert.ToDateTime(result.Rows[0]["max_timestamp"])
            : null;

        if (maxTimestampServer == null)
        {
            // First synchronisation: no server data yet — instruct client to request full dataset.
            // Ref: DS08-BL – Regole di Business: MAX = NULL → esistono_modifiche = true, timestamp_server = NOW().
            return new ConfrontoTimestampModificheResult(
                esistonoModifiche: true,
                timestampServer: serverNow,
                comparisonTimeMs: elapsedMs
            );
        }

        bool esistonoModifiche = maxTimestampServer.Value > timestampClient;

        // Ref: DS08-BL – Regole di Business:
        //   MAX > timestamp_client  → timestamp_server = MAX(timestamp_aggiornamento)
        //   MAX <= timestamp_client → timestamp_server = NOW() server (UTC)
        DateTime timestampServer = esistonoModifiche
            ? maxTimestampServer.Value
            : serverNow;

        return new ConfrontoTimestampModificheResult(
            esistonoModifiche,
            timestampServer,
            elapsedMs
        );
    }
}
