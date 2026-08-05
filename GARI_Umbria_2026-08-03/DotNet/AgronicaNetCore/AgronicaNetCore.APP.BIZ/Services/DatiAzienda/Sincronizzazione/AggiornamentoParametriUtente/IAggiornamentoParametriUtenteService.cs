using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiAzienda.Sincronizzazione.AggiornamentoParametriUtente;

/// <summary>
/// Persists or updates the API call parameters, user permissions, visibility filters and
/// enabled-company list for the current user in <c>app_param_utente_datiazienda_web2app</c>
/// after a successful DatiAzienda synchronisation round-trip.
/// Ref: DS06-BL – Nome: AggiornamentoParametriUtente.
/// </summary>
public interface IAggiornamentoParametriUtenteService
{
    /// <summary>
    /// Normalises all JSON inputs and executes an UPSERT (INSERT when no record exists for
    /// <paramref name="username"/>, UPDATE otherwise) on
    /// <c>app_param_utente_datiazienda_web2app</c>.
    /// Ref: DS06-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="username">Unique user identifier (primary key of the target table).</param>
    /// <param name="paramRichiamoApi">
    /// JSON string of API call parameters. Will be normalised (sorted keys, minimal whitespace)
    /// before storage. Ref: DS06-BL – Input: param_richiamo_api.
    /// </param>
    /// <param name="paramPermessiUtente">
    /// JSON string of user permissions. Will be normalised before storage.
    /// Ref: DS06-BL – Input: param_permessi_utente.
    /// </param>
    /// <param name="paramVisibilitaUtente">
    /// JSON string of visibility filters. Will be normalised before storage.
    /// Ref: DS06-BL – Input: param_visibilita_utente.
    /// </param>
    /// <param name="paramVisibilitaAziendeUtente">
    /// JSON array of enabled company P.IVA values. Will be normalised before storage.
    /// Ref: DS06-BL – Input: param_visibilita_azienda_utente.
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    /// <returns>
    /// An <see cref="AggiornamentoParametriUtenteResult"/> containing the operation
    /// type, the persisted UTC timestamp and the <c>forza_full_sync</c> flag.
    /// </returns>
    /// <exception cref="UpsertFailedException">
    /// Thrown when the INSERT or UPDATE operation fails at the data-access layer.
    /// Ref: DS06-BL – Eccezioni: UpsertFailedException.
    /// </exception>
    /// <exception cref="InvalidParameterFormatException">
    /// Thrown when any JSON parameter string is not valid JSON.
    /// </exception>
    /// <exception cref="ParameterSizeExceededException">
    /// Thrown when any JSON parameter string exceeds the configured character-length limit.
    /// Ref: DS06-BL – Eccezioni: ParameterSizeExceededException.
    /// </exception>
    Task<AggiornamentoParametriUtenteResult> AggiornaAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        string paramVisibilitaAziendeUtente,
        AgronicaCoreParametriServer objParametriServer);
}
