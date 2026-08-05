using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.FiliereAziende
{
    /// <summary>
    /// Provides the list of filiere visible to the current user to populate the
    /// selection dropdown in the CO2 sustainability calculation page.
    /// </summary>
    /// <remarks>DS-15 — <c>GET v1/sostenibilita/filiere-aziende</c>.</remarks>
    public interface IFiliereAziendeDropdownService
    {
        /// <summary>
        /// Returns all filiere (hierarchy level 2) visible to the requesting user.
        /// </summary>
        /// <param name="objParametriServer">Server-level request context (includes username for visibility filter).</param>
        /// <param name="objParametriUtenti">User-level request context (used to read user profile).</param>
        Task<FiliereAziendeDropdownResult> GetFiliereAsync(
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
