using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.PreparazioneDatiJob.ConfrontoJsonVariazioni;

/// <summary>
/// Reads the previous JSON snapshot from the database and compares it against the newly
/// generated JSON to determine whether the data has changed.
/// Ref: DS03-BL – Confronto Stringa JSON per Rilevazione Variazioni.
/// </summary>
public interface IConfrontoJsonVariazioniService
{
    /// <summary>
    /// Phase 1: reads <c>json_content</c> from <c>app_preparazione_daticomuni_web2app</c>
    /// for <paramref name="nomeTabella"/>, setting <c>exists</c> accordingly.
    /// Phase 2: performs a string-level comparison (length early-exit + ordinal byte-per-byte);
    /// aborts if the comparison exceeds 500 ms.
    /// Ref: DS03-BL – Regole di Business.
    /// </summary>
    /// <param name="jsonNuovo">JSON string just produced by serialisation (DS02).</param>
    /// <param name="nomeTabella">Table name used to query <c>app_preparazione_daticomuni_web2app</c>.</param>
    /// <param name="objParametriServer">DB connection parameters.</param>
    /// <exception cref="Exceptions.ComparisonTimeoutException">
    /// Comparison exceeded the 500 ms maximum.
    /// </exception>
    /// <exception cref="Exceptions.DatabaseReadException">
    /// Error reading from <c>app_preparazione_daticomuni_web2app</c>.
    /// </exception>
    Task<ConfrontoJsonVariazioniResult> ConfrontaAsync(
        string jsonNuovo,
        string nomeTabella,
        AgronicaCoreParametriServer objParametriServer);
}
