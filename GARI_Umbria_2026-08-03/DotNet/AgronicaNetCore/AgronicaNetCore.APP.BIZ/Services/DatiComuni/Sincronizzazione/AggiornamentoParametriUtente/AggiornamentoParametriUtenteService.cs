using System.Data;
using AgronicaNetCore.APP.BIZ.Common;
using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiComuni;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggiornamentoParametriUtente;

/// <summary>
/// Persists or updates user synchronisation parameters in
/// <c>app_param_utente_daticomuni_web2app</c> after a successful sync.
/// Ref: DS10-BL – Nome: AggiornamentoParametriUtente.
/// </summary>
public sealed class AggiornamentoParametriUtenteService : IAggiornamentoParametriUtenteService
{
    // Ref: DS10-BL – Regole di Business: dimensione > ParametroUtenteWarningLengthChars caratteri → warning log.

    // Thread-safe, shared serializer for deterministic JSON normalisation.
    // Ref: DS10-BL – Regole di Business: Normalizzare tutti i JSON in input (sorted keys, minimal whitespace).
    private static readonly JsonSerializerSettings _normalizedJsonSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
        ContractResolver = new AlphabeticalContractResolver(),
    };

    private readonly IParamUtenteDatiComuni _paramUtenteDal;
    private readonly ILoggingService _loggingService;
    private readonly SincroWeb2AppSettings _settings;

    public AggiornamentoParametriUtenteService(
        IParamUtenteDatiComuni paramUtenteDal,
        ILoggingService loggingService,
        IOptions<SincroWeb2AppSettings> settings)
    {
        _paramUtenteDal = paramUtenteDal;
        _loggingService = loggingService;
        _settings = settings.Value;
    }

    /// <inheritdoc/>
    public async Task<AggiornamentoParametriUtenteResult> AggiornaAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        AgronicaCoreParametriServer objParametriServer)
    {
        // Fail-fast: normalise and validate all JSON inputs before touching the database.
        // Ref: DS10-BL – Regole di Business: Normalizzare tutti i JSON in input.
        string richiamoNorm = NormalizzaJson(paramRichiamoApi, nameof(paramRichiamoApi));
        string permessiNorm = NormalizzaJson(paramPermessiUtente, nameof(paramPermessiUtente));
        string visibilitaNorm = NormalizzaJson(paramVisibilitaUtente, nameof(paramVisibilitaUtente));

        // Warn on anomalously large parameter strings.
        // Ref: DS10-BL – Regole di Business: dimensione > 10.000 caratteri → warning log.
        WarnIfOversized(richiamoNorm, nameof(paramRichiamoApi), objParametriServer);
        WarnIfOversized(permessiNorm, nameof(paramPermessiUtente), objParametriServer);
        WarnIfOversized(visibilitaNorm, nameof(paramVisibilitaUtente), objParametriServer);

        // Determine whether to INSERT or UPDATE.
        // Ref: DS10-BL – Regole di Business: Verificare esistenza record per username tramite SELECT COUNT(*).
        bool recordEsiste;
        try
        {
            DataTable existsDt = await _paramUtenteDal.VerificaEsistenzaAsync(username, objParametriServer);
            int cnt = existsDt.Rows.Count > 0
                ? Convert.ToInt32(existsDt.Rows[0]["cnt"])
                : 0;
            recordEsiste = cnt > 0;
        }
        catch (Exception ex)
        {
            throw new UpsertFailedException(
                $"Errore durante verifica esistenza record per utente '{username}'.", ex);
        }

        DateTime timestampUtcNow = DateTime.Now;
        string operazione;

        if (!recordEsiste)
        {
            // Ref: DS10-BL – Regole di Business: Se record non esiste → INSERT.
            try
            {
                await _paramUtenteDal.InsertAsync(
                    username,
                    richiamoNorm,
                    permessiNorm,
                    visibilitaNorm,
                    timestampUtcNow,
                    objParametriServer
                );
            }
            catch (Exception ex)
            {
                throw new UpsertFailedException(
                    $"Errore durante INSERT parametri utente per '{username}'.", ex);
            }

            operazione = "insert";
        }
        else
        {
            // Ref: DS10-BL – Regole di Business: Se record esiste → UPDATE.
            try
            {
                await _paramUtenteDal.UpdateAsync(
                    username,
                    richiamoNorm,
                    permessiNorm,
                    visibilitaNorm,
                    timestampUtcNow,
                    objParametriServer
                );
            }
            catch (Exception ex)
            {
                throw new UpsertFailedException(
                    $"Errore durante UPDATE parametri utente per '{username}'.", ex);
            }

            operazione = "update";
        }

        return new AggiornamentoParametriUtenteResult(operazione, timestampUtcNow);
    }

    /// <summary>
    /// Parses <paramref name="jsonInput"/> and re-serialises it with alphabetically sorted
    /// keys and minimal whitespace to produce a deterministic string for storage.
    /// Ref: DS10-BL – Regole di Business: Normalizzare tutti i JSON in input.
    /// </summary>
    /// <exception cref="InvalidParameterFormatException">
    /// Thrown when <paramref name="jsonInput"/> is not valid JSON.
    /// </exception>
    private static string NormalizzaJson(string jsonInput, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(jsonInput))
            return "{}";

        try
        {
            object? parsed = JsonConvert.DeserializeObject(jsonInput);
            return JsonConvert.SerializeObject(parsed, _normalizedJsonSettings);
        }
        catch (JsonException ex)
        {
            throw new InvalidParameterFormatException(
                $"Il parametro '{parameterName}' non è un JSON valido: {ex.Message}",
                ex
            );
        }
    }

    /// <summary>
    /// Logs a warning when the parameter string exceeds <see cref="WarningParameterLengthChars"/>.
    /// Ref: DS10-BL – Regole di Business: dimensione > 10.000 caratteri → warning log.
    /// </summary>
    private void WarnIfOversized(string value, string parameterName, AgronicaCoreParametriServer objParametriServer)
    {
        if (value.Length > _settings.ParametroUtenteWarningLengthChars)
        {
            _loggingService.LogWarning(
                "Il parametro '{ParameterName}' ha dimensione anomala: {Length} caratteri (soglia: {Threshold}). " +
                "Possibile payload anomalo. Ref: DS10-BL.",
                objParametriServer,
                null,
                parameterName,
                value.Length,
                _settings.ParametroUtenteWarningLengthChars
            );
        }
    }

    /// <summary>
    /// Orders all JSON object properties alphabetically (ordinal) to guarantee determinism
    /// across serialisations of the same object.
    /// Ref: DS10-BL – Regole di Business: sorted keys.
    /// </summary>
    private sealed class AlphabeticalContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(
            Type type,
            MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);
            return properties.OrderBy(p => p.PropertyName, StringComparer.Ordinal).ToList();
        }
    }
}
