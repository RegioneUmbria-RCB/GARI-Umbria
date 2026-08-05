using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.AggregazioneDatiComuni;

/// <summary>
/// Orchestrates the full DS01→DS05 preparation cycle for all 26+6 common tables.
/// Ref: DS14-API – Endpoint POST /v1/daticomuni/predisposizione-job.
/// </summary>
public interface IAggregazioneDatiComuniService
{
    /// <summary>
    /// Executes steps 1-5 for all common tables in sequence:
    /// (1) DS01-BL read, (2) DS02-BL serialise, (3) DS03-BL compare,
    /// (4) DS04-BL collect changed, (5) DS05-BL atomic commit.
    /// On any table-level failure the in-memory buffer is rolled back (DS04-BL) and a
    /// failure result is returned without persisting any data.
    /// Ref: DS14-API – Flusso Logico Dettagliato API, steps 4-8.
    /// </summary>
    Task<AggregazioneDatiComuniResult> EseguiAggregazioneAsync(
        AgronicaCoreParametriTriple objParametri,
        string bearerToken, 
        string coreWsUrl
    );
}
