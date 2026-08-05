using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

/// <summary>
/// Read-only DAL contract for loading consolidated JSON payloads from
/// <c>app_preparazione_daticomuni_web2app</c> in preparation for the API response.
/// Ref: DS09-BL – Persistenze Coinvolte; Pattern Framework: querylettura.
/// </summary>
public interface IAggregazioneJSONDatiComuni
{
    /// <summary>
    /// Returns a <see cref="DataTable"/> with columns <c>nome_tabella</c>,
    /// <c>json_content</c> and <c>timestamp_aggiornamento</c> for every row whose
    /// <c>nome_tabella</c> is in <paramref name="tabelleRichieste"/>.
    /// Ref: DS09-BL – Pattern Framework: LeggiJSONTabelleComuni.
    /// </summary>
    /// <param name="tabelleRichieste">
    /// Names of the common tables whose JSON must be included in the response (≤ 36).
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    Task<DataTable> LeggiJSONTabelleomuniAsync(
        List<string> tabelleRichieste,
        AgronicaCoreParametriServer objParametriServer);
}
