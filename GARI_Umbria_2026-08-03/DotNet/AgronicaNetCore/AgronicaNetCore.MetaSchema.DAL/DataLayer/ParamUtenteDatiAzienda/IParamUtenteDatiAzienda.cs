using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.MetaSchema.DAL.DataLayer.ParamUtenteDatiAzienda;

/// <summary>
/// Data-access contract for <c>app_param_utente_datiazienda_web2app</c>.
/// Ref: DS01-BL – Persistenze Coinvolte.
/// </summary>
public interface IParamUtenteDatiAzienda
{
    /// <summary>
    /// Reads the columns <c>param_richiamo_api</c>, <c>param_permessi_utente</c> and
    /// <c>param_visibilita_utente</c> and <c>param_visibilita_azienda_utente</c> for the given <paramref name="username"/>.
    /// Returns an empty <see cref="DataTable"/> when no record is found (first access).
    /// Ref: DS01-BL – Descrizione; Regole di Business.
    /// </summary>
    Task<DataTable> LeggiParametriUtenteDatiAziendaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Returns a single-row <see cref="DataTable"/> with column <c>cnt</c> containing
    /// the number of records for <paramref name="username"/> (0 or 1).
    /// Ref: DS01-BL – Regole di Business: verifica esistenza record per username.
    /// </summary>
    Task<DataTable> VerificaEsistenzaAsync(
        string username,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Inserts a new record into <c>app_param_utente_datiazienda_web2app</c>.
    /// Ref: DS01-BL – Persistenze Coinvolte.
    /// </summary>
    Task InsertAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        string paramVisibilitaAziende,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer);

    /// <summary>
    /// Updates the existing record for <paramref name="username"/> in
    /// <c>app_param_utente_datiazienda_web2app</c>.
    /// Ref: DS01-BL – Persistenze Coinvolte.
    /// </summary>
    Task UpdateAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        string paramVisibilitaAziende,
        DateTime timestampAggiornamento,
        AgronicaCoreParametriServer objParametriServer);
}
