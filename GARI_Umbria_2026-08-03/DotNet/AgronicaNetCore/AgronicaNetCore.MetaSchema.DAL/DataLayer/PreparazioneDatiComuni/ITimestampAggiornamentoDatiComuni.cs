using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

/// <summary>
/// Read-only DAL contract for retrieving the maximum update timestamp from
/// <c>app_preparazione_daticomuni_web2app</c>.
/// Ref: DS08-BL – Persistenze Coinvolte; Pattern Framework: querylettura.
/// </summary>
public interface ITimestampAggiornamentoDatiComuni
{
    /// <summary>
    /// Returns a single-row <see cref="DataTable"/> with column <c>max_timestamp</c>
    /// containing <c>MAX(timestamp_aggiornamento)</c> for the rows whose
    /// <c>nome_tabella</c> is in <paramref name="tabelleRichieste"/>.
    /// The column value is <see cref="DBNull"/> when no matching rows exist (first synchronisation).
    /// Ref: DS08-BL – Pattern Framework: LeggiTimestampAggiornamento.
    /// </summary>
    /// <param name="tabelleRichieste">
    /// Names of the common tables to include in the MAX aggregation (≤ 36).
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    Task<DataTable> LeggiTimestampAggiornamentoAsync(
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer);
}
