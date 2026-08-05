using AgronicaNetCore.Base.Models;
using OutData.FoodMetaverse;

namespace AgronicaNetCore.RischiMeteo.BIZ.Services.RischiAggregati;

/// <summary>
/// Provides geographic aggregation of weather-climate risk indicators for product and supply chain.
/// See DS09-API Endpoint Consultazione Rischi Aggregati Geografici, sezione "Specifiche Tecniche".
/// Depends on DS05-BL Aggregazione Indicatori Geografica per API FS4.08.1.
/// </summary>
public interface IRischiMeteoService
{
    /// <summary>
    /// Computes global, nation and nation+region weighted averages by cultivated area.
    /// See DS09-API Endpoint Consultazione Rischi Aggregati Geografici, sezione "Descrizione" and "Risposte".
    /// </summary>
    /// <param name="idFiliera">Supply chain identifier.</param>
    /// <param name="codProdottoFmp">FMP product code.</param>
    /// <param name="tolleranzaMaxDays">Maximum age in days for accepted calculations.</param>
    /// <param name="objP">Database access parameters.</param>
    /// <returns>
    /// A computation result containing the aggregated payload and processed row count.
    /// </returns>
    Task<RischiMeteoAggregationResult> GetRischiAggregatiAsync(
        string idFiliera,
        string codProdottoFmp,
        int tolleranzaMaxDays,
        AgronicaCoreParametriServer objP);

    /// <summary>
    /// Retrieves risk indicators for each cultivation exercise row matching filters.
    /// See DS10-API Endpoint Consultazione Rischi per Singoli Esercizi, sezione "Specifiche Tecniche".
    /// </summary>
    /// <param name="idFiliera">Supply chain identifier.</param>
    /// <param name="codProdottoFmp">FMP product code.</param>
    /// <param name="tolleranzaMaxDays">Maximum age in days for accepted calculations.</param>
    /// <param name="stato">Optional ISO 3166-1 alpha-3 nation code filter.</param>
    /// <param name="regione">Optional region filter.</param>
    /// <param name="objP">Database access parameters.</param>
    /// <returns>
    /// A computation result containing the detail payload and processed row count.
    /// </returns>
    Task<RischiMeteoDettaglioResult> GetRischiDettaglioAsync(
        string idFiliera,
        string codProdottoFmp,
        int tolleranzaMaxDays,
        string? stato,
        string? regione,
        AgronicaCoreParametriServer objP);
}

/// <summary>
/// Service computation result for GET /v1/rischi/aggregati.
/// See DS09-API Endpoint Consultazione Rischi Aggregati Geografici, sezione "200 - Successo".
/// </summary>
public class RischiMeteoAggregationResult
{
    /// <summary>True when at least one lookup row matched the filters.</summary>
    public bool HasData { get; set; }

    /// <summary>Aggregated data payload.</summary>
    public RischiAggregatiData Data { get; set; } = new RischiAggregatiData();

    /// <summary>Number of DB rows used for aggregation.</summary>
    public int NumeroRigheElaborate { get; set; }

    /// <summary>Number of rows considered valid for at least one risk indicator.</summary>
    public int NumeroRigheValide { get; set; }

    /// <summary>True when at least one dimension has partial risk availability.</summary>
    public bool IsPartialData { get; set; }
}

/// <summary>
/// Service computation result for GET /v1/rischi/dettaglio.
/// See DS10-API Endpoint Consultazione Rischi per Singoli Esercizi, sezione "Risposte".
/// </summary>
public class RischiMeteoDettaglioResult
{
    /// <summary>True when at least one lookup row matched the filters.</summary>
    public bool HasData { get; set; }

    /// <summary>Detail data payload.</summary>
    public RischiDettaglioData Data { get; set; } = new RischiDettaglioData();

    /// <summary>Number of DB rows returned for detail.</summary>
    public int NumeroRigheElaborate { get; set; }
}