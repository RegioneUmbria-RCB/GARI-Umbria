using InData.Zoo.DataMars;

namespace AgronicaNetCore.OperazioniZoo.BIZ.Services.PesateAutomaticheDataMars.DatamarsHttpService;

/// <summary>
/// Contratto per le chiamate HTTP verso l'API Datamars con retry esponenziale integrato.
/// <para>Riferimento spec: DS10-BL GestioneRetryEsponentialeApiDatamars.</para>
/// </summary>
public interface IDatamarsHttpService
{
    /// <summary>
    /// Autentica il client verso l'endpoint OAuth2 Datamars e restituisce il token di accesso.
    /// </summary>
    /// <param name="authEndpoint">URL completo dell'endpoint di autenticazione OAuth2.</param>
    /// <param name="credenziali">Credenziali OAuth2 (grant_type, client_id, client_secret).</param>
    /// <param name="cancellationToken">Token di cancellazione.</param>
    /// <returns>Risposta di autenticazione contenente <c>access_token</c> e <c>expires_in</c>.</returns>
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Autenticazione DataMars API.</para>
    Task<DatamarsAuthResponse> AuthenticateAsync(
        string authEndpoint,
        DatamarsCredentials credenziali,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera la lista delle sessionIntegration disponibili per un farm specifico.
    /// Implementa retry esponenziale con backoff [0s, 5s, 15s, 45s] e max 3 tentativi.
    /// </summary>
    /// <param name="farmId">Identificativo del farm su Datamars.</param>
    /// <param name="baseUrl">URL base dell'API Datamars.</param>
    /// <param name="apiKey">Bearer token per l'autenticazione.</param>
    /// <param name="cancellationToken">Token di cancellazione.</param>
    Task<List<DatamarsIntegrationSessionItem>> GetIntegrationSessionsAsync(
        string farmId,
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Recupera il dettaglio completo di una singola sessionIntegration, includendo l'array delle pesate.
    /// Implementa retry esponenziale con backoff [0s, 5s, 15s, 45s] e max 3 tentativi.
    /// </summary>
    /// <param name="sessionId">Identificativo univoco della sessione.</param>
    /// <param name="baseUrl">URL base dell'API Datamars.</param>
    /// <param name="apiKey">Bearer token per l'autenticazione.</param>
    /// <param name="cancellationToken">Token di cancellazione.</param>
    Task<DatamarsSessioneDettaglio> GetSessioneDettaglioAsync(
        string sessionId,
        string baseUrl,
        string apiKey,
        CancellationToken cancellationToken = default);
}
