using System.Diagnostics;
using System.Globalization;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using Microsoft.Extensions.Options;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Constants;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.VerificaTimestampTabelleRilevamentoModifiche;
using AgronicaNetCore.Utenti.DAL.DataLayer.UtentiVisibilitaAppoggio;
using System.Data;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Implements DS02 by checking, through unified log queries, which company-data tables have
/// changes after the last app synchronisation, applying visibility filters only when needed.
/// Ref: DS02-BL – Nome: VerificaTimestampTabelleRilevamentoModifiche.
/// </summary>
public sealed class VerificaTimestampTabelleRilevamentoModificheService
    : IVerificaTimestampTabelleRilevamentoModificheService
{
    private const string MotivoModificheRilevate = "modifiche_rilevate";
    private const string MotivoPrimaSincronizzazione = "prima_sincronizzazione";
    private const string MotivoNessunaModifica = "nessuna_modifica";
    private const string MotivoSincronizzazioneSempre = "sincronizzazione_sempre";

    private static readonly DateTime MinDatabaseDateUtc = new(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly HashSet<string> TabelleSempreSincronizzate = new(StringComparer.OrdinalIgnoreCase)
    {
        "prodotti",
        "progetti",
        "centriAziendaliAttivitaCDG",
        "impreseImpostazioni",
        "attivitaCDG",
        "lavorazioniAttivitaCDG",
        "areeTipologie",
        "tipologie",
        "codificaProdotti",
        "misureAvversitaAnagrafiche",
        "misureIndiciMaturitaAnagrafiche",
        "operazioneCausale",
    };

    private readonly ILogSincronizzazioneDatiAzienda _logDal;
    private readonly IUtentiVisibilitaAppoggio _utentiVisibilitaAppoggio;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public VerificaTimestampTabelleRilevamentoModificheService(
        ILogSincronizzazioneDatiAzienda logDal,
        IUtentiVisibilitaAppoggio utentiVisibilitaAppoggio,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _logDal = logDal;
        _utentiVisibilitaAppoggio = utentiVisibilitaAppoggio;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<VerificaTimestampTabelleRilevamentoModificheResult> VerificaAsync(
        string username,
        DateTime? timestampUltimaSincro,
        DateTime timestampServerAttuale,
        IReadOnlyCollection<VerificaTimestampTabellaInput> listaTabelle,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("username non può essere vuoto.", nameof(username));
        }

        ArgumentNullException.ThrowIfNull(listaTabelle);

        var stopwatch = Stopwatch.StartNew();

        var results = new Dictionary<string, VerificaTimestampTabellaResult>(StringComparer.OrdinalIgnoreCase);
        var groupedItems = new Dictionary<(LogSourceKind Source, VisibilityMode Visibility), List<VerificaTimestampTabellaInput>>();
        bool? companyRestricted = await HasRestrictionsAsync(
                    (int)TipiEnumerativi.Enum_TipoEntita.Impresa,
                    username,
                    objParametriServer);
        // In modalità Demetra la visibilità per centro non si applica.
        bool? centerRestricted = modalitaDemetra
            ? false
            : await HasRestrictionsAsync(
                    (int)TipiEnumerativi.Enum_TipoEntita.Centro,
                    username,
                    objParametriServer);


        foreach (VerificaTimestampTabellaInput item in listaTabelle)
        {
            if (string.IsNullOrWhiteSpace(item.NomeTabella))
            {
                throw new DatabaseQueryException("Ogni elemento di lista_tabelle deve valorizzare nome_tabella. Ref: DS02-BL.");
            }

            if (TabelleSempreSincronizzate.Contains(item.NomeTabella) || string.IsNullOrWhiteSpace(item.LogSource))
            {
                results[item.NomeTabella] = new VerificaTimestampTabellaResult(true, MotivoSincronizzazioneSempre);
                continue;
            }

            if (timestampUltimaSincro is null)
            {
                results[item.NomeTabella] = new VerificaTimestampTabellaResult(true, MotivoPrimaSincronizzazione);
                continue;
            }

            VisibilityMode visibilityMode = VisibilityMode.None;
            if (item.VisibilityCompanyCenter)
                if (centerRestricted.Value)
                    visibilityMode = VisibilityMode.Center;

            if (visibilityMode == VisibilityMode.None && item.VisibilityCompany)
                if (companyRestricted.Value)
                    visibilityMode = VisibilityMode.Company;


            LogSourceKind logSource = ParseLogSource(item.LogSource);
            var key = (logSource, visibilityMode);
            if (!groupedItems.TryGetValue(key, out List<VerificaTimestampTabellaInput>? list))
            {
                list = new List<VerificaTimestampTabellaInput>();
                groupedItems[key] = list;
            }

            list.Add(item);
        }

        if (timestampUltimaSincro != null)
            foreach ((LogSourceKind Source, VisibilityMode Visibility) key in groupedItems.Keys)
            {
                WarnIfTimeout(stopwatch, objParametriServer);
                List<VerificaTimestampTabellaInput> items = groupedItems[key];
                IReadOnlyCollection<VerificaTimestampLogQueryItem> queryItems = items
                    .Select(item => new VerificaTimestampLogQueryItem(item.NomeTabella, item.LogTipoFilter))
                    .ToList();

                HashSet<string> modifiedTables = await ReadModifiedTablesAsync(
                    key.Source,
                    key.Visibility,
                    queryItems,
                    timestampUltimaSincro.Value,
                    username,
                    modalitaDemetra,
                    piva,
                    objParametriServer);

                foreach (VerificaTimestampTabellaInput item in items)
                {
                    bool sincronizzare = modifiedTables.Contains(item.NomeTabella);
                    string motivo = sincronizzare ? MotivoModificheRilevate : MotivoNessunaModifica;
                    results[item.NomeTabella] = new VerificaTimestampTabellaResult(sincronizzare, motivo);
                }
            }

        RiassuntoSincronizzazioneResult summary = BuildSummary(results);
        return new VerificaTimestampTabelleRilevamentoModificheResult(results, summary);
    }

    private async Task<HashSet<string>> ReadModifiedTablesAsync(
        LogSourceKind source,
        VisibilityMode visibility,
        IReadOnlyCollection<VerificaTimestampLogQueryItem> queryItems,
        DateTime timestampUltimaSincro,
        string username,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer)
    {
        try
        {
            var result = source switch
            {
                LogSourceKind.Anagrafe => await _logDal.LeggiTabelleModificateLogAnagrafeAsync(
                    queryItems,
                    timestampUltimaSincro,
                    username,
                    visibility == VisibilityMode.Company,
                    visibility == VisibilityMode.Center,
                    modalitaDemetra,
                    piva,
                    objParametriServer),
                LogSourceKind.Contatti => await _logDal.LeggiTabelleModificateLogContattiAsync(
                    queryItems,
                    timestampUltimaSincro,
                    username,
                    visibility == VisibilityMode.Company,
                    visibility == VisibilityMode.Center,
                    modalitaDemetra,
                    piva,
                    objParametriServer),
                _ => throw new DatabaseQueryException($"Fonte log non supportata: {source}.")
            };

            return result.Rows
                .Cast<System.Data.DataRow>()
                .Select(row => row["nome_tabella"]?.ToString())
                .Where(nome => !string.IsNullOrWhiteSpace(nome))
                .Select(nome => nome!)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        catch (LogAccessException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new LogAccessException(
                $"Errore durante l'accesso alla sorgente log '{source}'. Ref: DS02-BL.",
                ex);
        }
    }

    private async Task<bool> HasRestrictionsAsync(
        int entity,
        string username,
        AgronicaCoreParametriServer objParametriServer)
    {
        DataTable? visibilityRows = await _utentiVisibilitaAppoggio.ReadRowExistsForUsernameAsync(
            entity,
            username,
            objParametriServer);

        if (visibilityRows == null)
        {
            throw new DatabaseQueryException(
                $"Impossibile determinare le restrizioni di visibilità per l'entità {entity}. Ref: DS02-BL.");
        }

        return visibilityRows.Rows[0].Field<int>("RowExists") == 1;
    }

    private void WarnIfTimeout(Stopwatch stopwatch, AgronicaCoreParametriServer objParametriServer)
    {
        if (stopwatch.ElapsedMilliseconds <= _settings.TimeoutMs)
            return;

        _loggingService.LogWarning(
            "DS02 ha superato il timeout aggregato di {TimeoutMs} ms: {ElapsedMs} ms.",
            objParametriServer,
            null,
            _settings.TimeoutMs,
            stopwatch.ElapsedMilliseconds);
    }

    private static RiassuntoSincronizzazioneResult BuildSummary(
        IReadOnlyDictionary<string, VerificaTimestampTabellaResult> results)
    {
        int totale = results.Count;
        int daSincronizzare = results.Count(item => item.Value.Sincronizzare);
        int omesse = totale - daSincronizzare;

        return new RiassuntoSincronizzazioneResult(
            totale,
            daSincronizzare,
            omesse,
            daSincronizzare > 0);
    }

    private static LogSourceKind ParseLogSource(string? logSource)
    {
        return logSource?.Trim().ToLowerInvariant() switch
        {
            "agronica_log_anagrafe" => LogSourceKind.Anagrafe,
            "agronica_log_contatti" => LogSourceKind.Contatti,
            _ => throw new DatabaseQueryException($"Log source non supportato: '{logSource}'. Ref: DS02-BL."),
        };
    }

    private static DateTime ParseIsoTimestamp(string value, bool isClientTimestamp, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (isClientTimestamp)
            {
                throw new InvalidTimestampException("last_sync_timestamp non può essere vuoto. Ref: DS02-BL.");
            }

            throw new TimestampConversionException($"Il parametro '{parameterName}' non può essere vuoto. Ref: DS02-BL.");
        }

        if (!DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind | DateTimeStyles.AllowWhiteSpaces,
                out DateTimeOffset parsed))
        {
            if (isClientTimestamp)
            {
                throw new InvalidTimestampException(
                    $"Il timestamp '{value}' non è un ISO 8601 valido. Ref: DS02-BL.");
            }

            throw new TimestampConversionException(
                $"Il parametro '{parameterName}' non è convertibile da ISO 8601. Ref: DS02-BL.");
        }

        DateTime utc = parsed.UtcDateTime;
        long truncatedTicks = utc.Ticks - utc.Ticks % TimeSpan.TicksPerMillisecond;
        DateTime millisecondPrecisionUtc = new(truncatedTicks, DateTimeKind.Utc);

        if (millisecondPrecisionUtc < MinDatabaseDateUtc)
        {
            throw new TimestampConversionException(
                $"Il parametro '{parameterName}' è fuori range per il database: {millisecondPrecisionUtc:O}. Ref: DS02-BL.");
        }

        return millisecondPrecisionUtc;
    }

    private enum LogSourceKind
    {
        Anagrafe,
        Contatti,
    }

    private enum VisibilityMode
    {
        None,
        Company,
        Center,
    }
}
