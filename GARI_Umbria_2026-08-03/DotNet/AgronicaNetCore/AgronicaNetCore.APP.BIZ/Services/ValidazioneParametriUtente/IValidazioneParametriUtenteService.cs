using AgronicaNetCore.Base.Models;
using static AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente.ValidazioneParametriUtenteDatiComuniResult;

namespace AgronicaNetCore.APP.BIZ.Services.ValidazioneParametriUtente;

/// <summary>
/// Validates the API call parameters, user permissions and visibility filters received in a
/// synchronisation request against the values stored from the last successful synchronisation.
/// Determines whether a full dataset should be returned or whether timestamp-based comparison
/// can be applied.
/// Ref: DS07-BL – Nome: ValidazioneParametriUtente.
/// </summary>
public interface IValidazioneParametriUtenteService
{
    /// <summary>
    /// Compares <paramref name="paramRichiamoApiRicevuto"/> and the current permissions /
    /// visibility filters against the values stored in <c>app_param_utente_daticomuni_web2app</c>
    /// for <c>objParametriUtenti.UtenteUsername</c>.
    /// On first access (no stored record) the method still reads and returns the current
    /// permissions and visibility filters so the caller can persist them after a successful sync.
    /// Ref: DS07-BL – Descrizione; Regole di Business.
    /// </summary>
    /// <param name="paramRichiamoApiRicevuto">
    /// JSON string of API call parameters received from the mobile client.
    /// Must be valid JSON; otherwise <see cref="Exceptions.InvalidParameterFormatException"/> is thrown.
    /// </param>
    /// <param name="objParametriUtenti">User context (provides <c>UtenteUsername</c>).</param>
    /// <param name="objParametriServer">Server context used for data-provider resolution.</param>
    /// <exception cref="Exceptions.InvalidParameterFormatException">
    /// Thrown when <paramref name="paramRichiamoApiRicevuto"/> is not valid JSON.
    /// </exception>
    /// <exception cref="Exceptions.SetupSystemUnavailableException">
    /// Thrown when current permissions or visibility filters cannot be read from the setup system.
    /// </exception>
    /// <exception cref="Exceptions.DataAccessException">
    /// Thrown when reading from <c>app_param_utente_daticomuni_web2app</c> fails.
    /// </exception>
    Task<ValidazioneParametriUtenteDatiComuniResult> ValidaParametriDatiComuniAsync(
        string paramRichiamoApiRicevuto,
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    );

    Task<ValidazioneParametriUtenteDatiAziendaResult> ValidaParametriDatiAziendaAsync(
        string paramRichiamoApiRicevuto,
        AgronicaCoreParametriUtenti objParametriUtenti,
        AgronicaCoreParametriServer objParametriServer
    );
}
