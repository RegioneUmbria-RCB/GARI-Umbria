using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiComuni;

/// <summary>
/// Data-access contract for <c>app_param_utente_daticomuni_web2app</c>.
/// Ref: DS07-BL – Persistenze Coinvolte; DS10-BL – Persistenze Coinvolte.
/// </summary>
public interface IParamUtenteDatiComuni
{
    /// <summary>
    /// Reads the columns <c>param_richiamo_api</c>, <c>param_permessi_utente</c> and
    /// <c>param_visibilita_utente</c> for the given <paramref name="username"/>.
    /// Returns an empty <see cref="DataTable"/> when no record is found (first access).
    /// Ref: DS07-BL – Pattern Framework: querylettura; Regole di Business.
    /// </summary>
    Task<DataTable> LeggiParametriUtenteAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Returns a single-row <see cref="DataTable"/> with column <c>cnt</c> containing
    /// the number of records for <paramref name="username"/> (0 or 1).
    /// Ref: DS10-BL – Pattern Framework: VerificaEsistenzaParametri.
    /// </summary>
    Task<DataTable> VerificaEsistenzaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Inserts a new record into <c>app_param_utente_daticomuni_web2app</c>.
    /// Ref: DS10-BL – Pattern Framework: InserisciParametriUtente.
    /// </summary>
    Task InsertAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Updates the existing record for <paramref name="username"/> in
    /// <c>app_param_utente_daticomuni_web2app</c>.
    /// Ref: DS10-BL – Pattern Framework: AggiornaParametriUtente.
    /// </summary>
    Task UpdateAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer);
}
