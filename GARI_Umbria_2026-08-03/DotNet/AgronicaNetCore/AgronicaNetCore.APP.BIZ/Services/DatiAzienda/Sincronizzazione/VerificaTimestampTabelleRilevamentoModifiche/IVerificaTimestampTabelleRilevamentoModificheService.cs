using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.VerificaTimestampTabelleRilevamentoModifiche;

/// <summary>
/// Verifies, for each requested company-data table, whether log entries exist after the last
/// successful app synchronisation, applying visibility joins only when the user is actually
/// restricted.
/// Ref: DS02-BL – Nome: VerificaTimestampTabelleRilevamentoModifiche.
/// </summary>
public interface IVerificaTimestampTabelleRilevamentoModificheService
{
    /// <summary>
    /// Evaluates the requested tables and returns which ones must be synchronised.
    /// Ref: DS02-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="username">User performing the synchronisation. Ref: DS02-BL – Input: username.</param>
    /// <param name="timestampUltimaSincro">
    /// ISO 8601 timestamp of the last successful app sync.
    /// Ref: DS02-BL – Input: last_sync_timestamp.
    /// </param>
    /// <param name="timestampServerAttuale">
    /// ISO 8601 authoritative server timestamp captured upstream.
    /// Ref: DS02-BL – Input: timestamp_server_attuale.
    /// </param>
    /// <param name="listaTabelle">Requested tables to evaluate. Ref: DS02-BL – Input: lista_tabelle.</param>
    /// <param name="objParametriServer">Server context used for DAL resolution.</param>
    /// <exception cref="Exceptions.InvalidTimestampException">
    /// Thrown when <paramref name="timestampUltimaSincro"/> is not a valid ISO 8601 timestamp.
    /// </exception>
    /// <exception cref="Exceptions.TimestampConversionException">
    /// Thrown when a timestamp cannot be converted to database-compatible precision.
    /// </exception>
    /// <exception cref="Exceptions.LogAccessException">
    /// Thrown when a required log source cannot be accessed.
    /// </exception>
    /// <exception cref="Exceptions.DatabaseQueryException">
    /// Thrown when the unified log verification fails or exceeds the aggregated SLA.
    /// </exception>
    Task<VerificaTimestampTabelleRilevamentoModificheResult> VerificaAsync(
        string username,
        DateTime? timestampUltimaSincro,
        DateTime timestampServerAttuale,
        IReadOnlyCollection<VerificaTimestampTabellaInput> listaTabelle,
        bool modalitaDemetra,
        string piva,
        AgronicaCoreParametriServer objParametriServer); 
}