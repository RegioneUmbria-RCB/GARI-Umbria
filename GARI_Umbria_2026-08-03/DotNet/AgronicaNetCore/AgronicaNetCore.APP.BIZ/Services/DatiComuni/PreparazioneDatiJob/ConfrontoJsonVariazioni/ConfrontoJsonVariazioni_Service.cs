using AgronicaNetCore.APP.BIZ.Common;
using Microsoft.Extensions.Options;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;
using System.Diagnostics;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.ConfrontoJsonVariazioni;

/// <summary>
/// Reads the previous JSON from the database then performs a string-level comparison
/// to detect data changes for a given table.
/// Ref: DS03-BL – Confronto Stringa JSON per Rilevazione Variazioni.
/// </summary>
public sealed class ConfrontoJsonVariazioniService : IConfrontoJsonVariazioniService
{
    private readonly IAggiornamentoAtomicoPreparazione _preparazioneDAL;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public ConfrontoJsonVariazioniService(
        IAggiornamentoAtomicoPreparazione preparazioneDAL,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _preparazioneDAL = preparazioneDAL;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<ConfrontoJsonVariazioniResult> ConfrontaAsync(
        string jsonNuovo,
        string nomeTabella,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Ref: DS03-BL – Regole di Business: Fase 1 – Lettura DB.
        bool exists;
        string? jsonPrecedente;

        try
        {
            (exists, jsonPrecedente) = await _preparazioneDAL.LeggiJsonContentAsync(nomeTabella, objParametriServer);
        }
        catch (Exception ex)
        {
            throw new DatabaseReadException(
                $"[{nomeTabella}] Failed to read json_content from app_preparazione_daticomuni_web2app: {ex.Message}", ex);
        }

        // Ref: DS03-BL – Regole di Business: Fase 2 – Confronto JSON.
        // Prima esecuzione o riga inesistente → sempre variato.
        if (jsonPrecedente is null)
        {
            _loggingService.LogInformation("[{NomeTabella}] json_precedente è null (prima esecuzione o riga assente): ha_variazioni = true.", objParametriServer, null, nomeTabella);
            return new ConfrontoJsonVariazioniResult(haVariazioni: true, exists: exists, comparisonTimeMs: 0);
        }

        var stopwatch = Stopwatch.StartNew();

        // Ref: DS03-BL – Regole di Business: early-exit sulla lunghezza.
        if (jsonNuovo.Length != jsonPrecedente.Length)
        {
            stopwatch.Stop();
            _loggingService.LogInformation(
                "[{NomeTabella}] Lunghezze diverse ({NewLen} vs {OldLen}): ha_variazioni = true in {ElapsedMs}ms.",
                objParametriServer,
                null,
                nomeTabella, jsonNuovo.Length, jsonPrecedente.Length, stopwatch.ElapsedMilliseconds);

            return new ConfrontoJsonVariazioniResult(haVariazioni: true, exists: exists, comparisonTimeMs: stopwatch.ElapsedMilliseconds);
        }

        // Full ordinal comparison offloaded to the thread pool so the 500 ms guard can fire.
        // string.Equals(Ordinal) is SIMD-accelerated and exits at the first mismatch internally,
        // satisfying the case-sensitive byte-per-byte early-exit requirement.
        // Ref: DS03-BL – Regole di Business: confronto case-sensitive byte-per-byte con early-exit.
        var comparisonTask = Task.Run(
            () => string.Equals(jsonNuovo, jsonPrecedente, StringComparison.Ordinal));

        // Ref: DS03-BL – Regole di Business: warning log se il confronto supera il limite configurato.
        bool completedInTime = await Task.WhenAny(comparisonTask, Task.Delay(_settings.TimeoutMs)) == comparisonTask;

        stopwatch.Stop();

        if (!completedInTime)
            _loggingService.LogWarning(
                "[{NomeTabella}] Il confronto JSON ha superato il limite di {TimeoutMs} ms. Ref: DS03-BL.",
                objParametriServer,
                null,
                nomeTabella, _settings.TimeoutMs);

        bool areEqual = await comparisonTask;
        bool haVariazioni = !areEqual;

        _loggingService.LogInformation(
            "[{NomeTabella}] Confronto completato in {ElapsedMs}ms: ha_variazioni = {HaVariazioni}.",
            objParametriServer,
            null,
            nomeTabella, stopwatch.ElapsedMilliseconds, haVariazioni);

        return new ConfrontoJsonVariazioniResult(haVariazioni, exists, stopwatch.ElapsedMilliseconds);
    }
}
