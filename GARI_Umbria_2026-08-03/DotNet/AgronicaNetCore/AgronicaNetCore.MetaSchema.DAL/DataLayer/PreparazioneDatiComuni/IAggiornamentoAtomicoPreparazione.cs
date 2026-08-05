using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.PreparazioneDatiComuni;

/// <summary>
/// DAL contract for read and UPDATE operations on <c>app_preparazione_daticomuni_web2app</c>.
/// Ref: DS03-BL – Persistenze Coinvolte; DS05-BL – Persistenze Coinvolte.
/// </summary>
public interface IAggiornamentoAtomicoPreparazione
{
    /// <summary>
    /// Reads <c>json_content</c> for the specified <paramref name="nomeTabella"/>.
    /// Returns <c>(true, jsonContent)</c> when the row exists, <c>(false, null)</c> otherwise.
    /// Ref: DS03-BL – Regole di Business: Fase 1 – Lettura DB.
    /// </summary>
    Task<(bool Exists, string? JsonContent)> LeggiJsonContentAsync(
        string nomeTabella,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Executes an INSERT for a new row in <c>app_preparazione_daticomuni_web2app</c>.
    /// Called when <c>exists = false</c> (first execution for this table).
    /// Ref: DS03-BL – Nota Tecnica; DS05-BL – INSERT vs UPDATE based on exists flag.
    /// </summary>
    Task InsertPreparazioneAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Executes a single UPDATE for one table's JSON snapshot within the active transaction
    /// already open on <paramref name="objParametriServer"/>.
    /// Ref: DS05-BL – Pattern Framework: queryupdate con StringBuilder + ExpandoObject.
    /// </summary>
    Task UpdatePreparazioneAsync(
        string nomeTabella,
        string jsonNuovo,
        DateTime timestampGenerazione,
        AgronicaCoreParametriServer objParametriServer);
}
