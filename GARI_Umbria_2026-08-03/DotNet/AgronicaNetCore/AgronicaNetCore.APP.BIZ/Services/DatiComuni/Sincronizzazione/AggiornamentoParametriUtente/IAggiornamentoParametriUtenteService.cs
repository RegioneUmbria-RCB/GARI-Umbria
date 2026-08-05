using AgronicaNetCore.APP.BIZ.Exceptions;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.APP.BIZ.Services.DatiComuni.Sincronizzazione.AggiornamentoParametriUtente;

/// <summary>
/// Persists or updates the API call parameters, user permissions and visibility
/// filters for the current user in <c>app_param_utente_daticomuni_web2app</c>
/// after a successful synchronisation round-trip.
/// Ref: DS10-BL – Nome: AggiornamentoParametriUtente.
/// </summary>
public interface IAggiornamentoParametriUtenteService
{
    /// <summary>
    /// Normalises all JSON inputs and executes an UPSERT (INSERT when no record exists for
    /// <paramref name="username"/>, UPDATE otherwise) on
    /// <c>app_param_utente_daticomuni_web2app</c>.
    /// Ref: DS10-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="username">Unique user identifier (primary key of the target table).</param>
    /// <param name="paramRichiamoApi">
    /// JSON string of API call parameters to persist. Will be normalised (sorted keys, minimal
    /// whitespace) before storage. Ref: DS10-BL – Input: param_richiamo_api.
    /// </param>
    /// <param name="paramPermessiUtente">
    /// JSON string of user permissions to persist. Will be normalised before storage.
    /// Ref: DS10-BL – Input: param_permessi_utente.
    /// </param>
    /// <param name="paramVisibilitaUtente">
    /// JSON string of visibility filters to persist. Will be normalised before storage.
    /// Ref: DS10-BL – Input: param_visibilita_utente.
    /// </param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    /// <exception cref="UpsertFailedException">
    /// Thrown when the INSERT or UPDATE operation fails at the data-access layer.
    /// Ref: DS10-BL – Eccezioni: UpsertFailedException.
    /// </exception>
    /// <exception cref="InvalidParameterFormatException">
    /// Thrown when any of the JSON parameter strings is not valid JSON.
    /// </exception>
    Task<AggiornamentoParametriUtenteResult> AggiornaAsync(
        string username,
        string paramRichiamoApi,
        string paramPermessiUtente,
        string paramVisibilitaUtente,
        AgronicaCoreParametriServer objParametriServer);
}
