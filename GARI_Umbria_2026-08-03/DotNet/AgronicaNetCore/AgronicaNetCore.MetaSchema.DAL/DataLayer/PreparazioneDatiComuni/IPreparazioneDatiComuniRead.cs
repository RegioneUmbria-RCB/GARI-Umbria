using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

/// <summary>
/// Read-only DAL contract for <c>app_preparazione_daticomuni_web2app</c>.
/// Used by DS03-BL comparison to load the previous JSON snapshot for each table.
/// </summary>
public interface IPreparazioneDatiComuniRead
{
    /// <summary>
    /// Returns all rows from <c>app_preparazione_daticomuni_web2app</c> as a dictionary keyed by
    /// <c>nome_tabella</c>. The value is <c>null</c> when <c>json_content</c> has never been set.
    /// </summary>
    Task<Dictionary<string, string?>> LeggiJsonPrecedentiAsync(AgronicaCoreParametriServer objParametriServer);
}
