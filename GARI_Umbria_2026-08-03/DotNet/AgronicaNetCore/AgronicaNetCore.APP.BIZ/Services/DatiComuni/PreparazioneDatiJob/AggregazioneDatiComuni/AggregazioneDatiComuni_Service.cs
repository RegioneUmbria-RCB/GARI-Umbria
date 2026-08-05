using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDati.SerializzazioneJsonDeterministico;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.ConfrontoJsonVariazioni;
using AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.RaccoltaJsonMemoria;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune;
using AgronicaNetCore.APP.BIZ.Services.LetturaDatiTabellaComune.Enums;
using AgronicaNetCore.Base.Base;
using AgronicaNetCore.Base.Models;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;
using AgronicaNetCore.MetaSchema.DAL.DataLayer.TabelleComuni_APP;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggregazioneDatiComuni;

/// <summary>
/// Orchestrates the full DS01→DS05 aggregation cycle for all common tables.
/// Ref: DS14-API – Flusso Logico Dettagliato API, steps 4-8.
/// </summary>
public sealed class AggregazioneDatiComuniService : IAggregazioneDatiComuniService
{
    private readonly ILetturaDatiTabellaComuneAPPService _letturaService;
    private readonly ISerializzazioneJsonDeterministicoService _serializzazioneService;
    private readonly IConfrontoJsonVariazioniService _confrontoService;
    private readonly IRaccoltaJsonMemoriaService _raccoltaService;
    private readonly ILoggingService _loggingService;

    public AggregazioneDatiComuniService(
        ILetturaDatiTabellaComuneAPPService letturaService,
        ISerializzazioneJsonDeterministicoService serializzazioneService,
        IConfrontoJsonVariazioniService confrontoService,
        IRaccoltaJsonMemoriaService raccoltaService,
        ILoggingService loggingService)
    {
        _letturaService = letturaService;
        _serializzazioneService = serializzazioneService;
        _confrontoService = confrontoService;
        _raccoltaService = raccoltaService;
        _loggingService = loggingService;
    }

    /// <inheritdoc/>
    public async Task<AggregazioneDatiComuniResult> EseguiAggregazioneAsync(AgronicaCoreParametriTriple objParams, string bearerToken, string coreWsUrl)
    {
        var parameters = new LetturaTabellaComuneAppParameters(objParams, bearerToken, coreWsUrl);
        var allTables = Enum.GetValues<TabellaComune>();

        int tabelleElaborate = 0;
        int tabelleConVariazioni = 0;
        long dimensioneTotale = 0;

        foreach (TabellaComune tabella in allTables)
        {
            string nomeTabella = tabella.ToString();

            try
            {
                // DS01-BL: read (includes retry logic).
                object entityObj = await _letturaService.LeggiAsync(tabella, parameters);

                // Convert the returned collection to IReadOnlyList<object> for serialisation.
                // Newtonsoft.Json's serializer uses the runtime type of each element, so
                // AlphabeticalContractResolver will sort the concrete entity properties correctly.
                IReadOnlyList<object> entityList = CoerceToReadOnlyList(entityObj);

                // DS02-BL: deterministic serialisation.
                var serialResult = await _serializzazioneService.SerializzaAsync(entityList, nomeTabella);

                // DS03-BL: read previous JSON from DB and compare (per-table).
                // Ref: DS03-BL – Regole di Business: Fase 1 lettura DB + Fase 2 confronto.
                var confrontoResult = await _confrontoService.ConfrontaAsync(
                    serialResult.JsonContent, nomeTabella, objParams.ObjParametriServer);

                tabelleElaborate++;
                dimensioneTotale += serialResult.JsonSizeBytes;

                // DS04-BL: accumulate in-memory buffer only when changed.
                if (confrontoResult.HaVariazioni)
                {
                    await _raccoltaService.AggiungiAsync(nomeTabella, serialResult.JsonContent, DateTime.Now, confrontoResult.Exists);
                    tabelleConVariazioni++;
                }
            }
            catch (MaxRetryExceededException ex)
            {
                // DS01-BL: read failed after max retries – rollback everything and abort.
                _raccoltaService.Rollback();
                _loggingService.LogError(
                    "Aggregazione fallita: lettura {NomeTabella} non riuscita dopo 2 tentativi. Rollback eseguito. Ref: DS01-BL.",
                    objParams?.ObjParametriServer,
                    ex,
                    nomeTabella);
                return AggregazioneDatiComuniResult.Failure(tabelleElaborate, nomeTabella, ex.Message);
            }
            catch (Exception ex)
            {
                // Any unexpected error during the per-table pipeline triggers full rollback.
                _raccoltaService.Rollback();
                _loggingService.LogError(
                    "Errore imprevisto durante elaborazione tabella {NomeTabella}. Rollback eseguito. Ref: DS14-API.",
                    objParams?.ObjParametriServer,
                    ex,
                    nomeTabella);
                return AggregazioneDatiComuniResult.Failure(tabelleElaborate, nomeTabella, ex.Message);
            }
        }

        // DS05-BL (via DS04-BL CommitAsync): atomic SERIALIZABLE transaction.
        try
        {
            await _raccoltaService.CommitAsync(objParams.ObjParametriServer);
        }
        catch (Exception ex)
        {
            _raccoltaService.Rollback();
            _loggingService.LogError(
                "Commit atomico fallito (DS05-BL). Rollback eseguito. Ref: DS14-API.",
                objParams?.ObjParametriServer,
                ex);
            return AggregazioneDatiComuniResult.Failure(tabelleElaborate, "CommitAtomico", ex.Message);
        }

        _loggingService.LogInformation(
            "Aggregazione completata: {TabelleElaborate} elaborate, {TabelleConVariazioni} con variazioni, {DimensioneTotale} bytes totali. Ref: DS14-API.",
            objParams?.ObjParametriServer,
            null,
            tabelleElaborate, tabelleConVariazioni, dimensioneTotale);

        return AggregazioneDatiComuniResult.Success(tabelleElaborate, tabelleConVariazioni, dimensioneTotale);
    }

    /// <summary>
    /// Converts the object returned by <see cref="ILetturaDatiTabellaComuneAPPService.LeggiAsync"/>
    /// (a typed <c>List&lt;TEntity&gt;</c> at runtime) into a covariant-safe
    /// <see cref="IReadOnlyList{T}"/> of <c>object</c> without allocating a copy when possible.
    /// </summary>
    private static IReadOnlyList<object> CoerceToReadOnlyList(object entityObj)
    {
        if (entityObj is IReadOnlyList<object> already)
            return already;

        if (entityObj is System.Collections.IEnumerable enumerable)
            return enumerable.Cast<object>().ToList().AsReadOnly();

        return new List<object> { entityObj }.AsReadOnly();
    }
}
