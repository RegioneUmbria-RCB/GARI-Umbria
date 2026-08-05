using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Data;
using System.Diagnostics;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggiornamentoAtomicoPreparazione;

/// <summary>
/// Orchestrates the SERIALIZABLE transaction that persists all collected JSON entries into
/// <c>app_preparazione_daticomuni_web2app</c>, delegating each UPDATE to the DAL.
/// Handles deadlock retry (max 1 attempt, 5 s delay) and full rollback on failure.
/// Ref: DS05-BL – Aggiornamento Atomico Tabella Preparazione Dati.
/// </summary>
public sealed class AggiornamentoAtomicoPreparazioneService : BaseServiceAppBIZ, IAggiornamentoAtomicoPreparazioneService
{
    private readonly IAggiornamentoAtomicoPreparazione _aggiornamentoDAL;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public AggiornamentoAtomicoPreparazioneService(
        IServiceProvider provider,
        IAggiornamentoAtomicoPreparazione aggiornamentoDAL,
        IStringLocalizer<Resources.Messages> localizer,
        IOptions<SincroWeb2AppSettings> settings)
        : base(provider, localizer)
    {
        _loggingService = _serviceProvider.GetRequiredService<ILoggingService>();
        _aggiornamentoDAL = aggiornamentoDAL;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<AggiornamentoAtomicoResult> AggiornaAsync(
        IReadOnlyCollection<PreparazioneDatiComuniEntry> jsonRaccolti,
        AgronicaCoreParametriServer objParametriServer)
    {
        for (int attempt = 0; attempt <= _settings.MaxDeadlockRetry; attempt++)
        {
            var stopwatch = Stopwatch.StartNew();
            bool connectionOpened = false;

            try
            {
                // Ref: DS05-BL – Regole di Business: apertura transazione SERIALIZABLE.
                await OpenConnectionAsync(objParametriServer, OpenTransaction: true, level: IsolationLevel.Serializable);
                connectionOpened = true;

                // Ref: DS05-BL – Regole di Business: durata massima transazione 30 secondi.
                using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.TransactionTimeoutMs));

                int righeAggiornate = 0;
                int righeInserte = 0;

                foreach (var entry in jsonRaccolti)
                {
                    cts.Token.ThrowIfCancellationRequested();

                    // Ref: DS03-BL – Nota Tecnica: use entry.Exists to choose INSERT vs UPDATE.
                    if (entry.Exists)
                    {
                        await _aggiornamentoDAL.UpdatePreparazioneAsync(
                            entry.NomeTabella, entry.JsonNuovo, entry.TimestampGenerazione, objParametriServer);
                        righeAggiornate++;
                    }
                    else
                    {
                        await _aggiornamentoDAL.InsertPreparazioneAsync(
                            entry.NomeTabella, entry.JsonNuovo, entry.TimestampGenerazione, objParametriServer);
                        righeInserte++;
                    }
                }

                // Ref: DS05-BL – Regole di Business: COMMIT in caso di successo completo.
                CloseTransaction(objParametriServer, Rollback: false);
                stopwatch.Stop();

                _loggingService.LogInformation(
                    "Aggiornamento atomico completato: {Updated} aggiornate, {Inserted} inserte in {Ms}ms. Ref: DS05-BL.",
                    objParametriServer,
                    null,
                    righeAggiornate, righeInserte, stopwatch.ElapsedMilliseconds);

                return AggiornamentoAtomicoResult.Success(righeAggiornate, righeInserte, stopwatch.ElapsedMilliseconds);
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                if (connectionOpened) CloseTransaction(objParametriServer, Rollback: true);
                // Ref: DS05-BL – Eccezioni: TransactionTimeoutException.
                throw new TransactionTimeoutException(
                    $"[DS05-BL] Transazione superata il limite di {_settings.TransactionTimeoutMs}ms ({stopwatch.ElapsedMilliseconds}ms).");
            }
            catch (Exception ex) when (IsDeadlock(ex) && attempt < _settings.MaxDeadlockRetry)
            {
                // Ref: DS05-BL – Regole di Business: retry max 1 volta su deadlock con delay 5s.
                stopwatch.Stop();
                if (connectionOpened) CloseTransaction(objParametriServer, Rollback: true);
                _loggingService.LogWarning(
                    "[DS05-BL] Deadlock rilevato (tentativo {Attempt}/{Max}): retry tra {Delay}s.",
                    objParametriServer,
                    null,
                    attempt + 1, _settings.MaxDeadlockRetry + 1, _settings.DeadlockRetryDelaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(_settings.DeadlockRetryDelaySeconds));
            }
            catch (Exception ex) when (IsDeadlock(ex))
            {
                // Ref: DS05-BL – Eccezioni: DeadlockException dopo retry esaurito.
                stopwatch.Stop();
                if (connectionOpened) CloseTransaction(objParametriServer, Rollback: true);
                throw new DeadlockException(
                    $"[DS05-BL] Deadlock SQL persistente dopo {_settings.MaxDeadlockRetry + 1} tentativi.", ex);
            }
            catch (Exception ex)
            {
                // Ref: DS05-BL – Regole di Business: ROLLBACK + eccezione su errore persistente.
                stopwatch.Stop();
                if (connectionOpened) CloseTransaction(objParametriServer, Rollback: true);
                throw new AtomicUpdateFailedException(
                    "[DS05-BL] Aggiornamento atomico fallito dopo rollback.", ex);
            }
            finally
            {
                CloseConnection(objParametriServer);
            }
        }

        // All retry attempts exhausted without returning.
        throw new AtomicUpdateFailedException("[DS05-BL] Aggiornamento atomico fallito in modo inatteso.");
    }

    /// <summary>
    /// Returns <c>true</c> if the exception chain contains a deadlock message.
    /// Avoids a direct SqlClient assembly dependency in the BIZ layer.
    /// </summary>
    private static bool IsDeadlock(Exception ex)
    {
        var current = ex;
        while (current is not null)
        {
            if (current.Message.Contains("deadlock", StringComparison.OrdinalIgnoreCase))
                return true;
            current = current.InnerException;
        }
        return false;
    }
}
