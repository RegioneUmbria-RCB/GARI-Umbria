using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SmartTractor.BIZ.Services.DispatcherManager;

public interface IAuthDispatcherService
{
    /// <summary>
    /// Returns a valid bearer token for the Smart Tractor SAT service,
    /// using a per-connection cache to avoid redundant token requests.
    /// </summary>
    Task<TokenResponse> GenerateNewBearerTokenAsync(
        AgronicaCoreParametriServer serverParams,
        AgronicaCoreParametriSuperServer superServerParams);
}
