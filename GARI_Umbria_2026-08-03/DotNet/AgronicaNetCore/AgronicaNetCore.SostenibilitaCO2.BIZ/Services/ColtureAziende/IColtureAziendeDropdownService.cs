using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ColtureAziende
{
    /// <summary>
    /// Provides the list of crop species linked to one or more filiere, used to
    /// populate the colture selection dropdown on the CO2 sustainability page.
    /// </summary>
    /// <remarks>DS-16 — <c>GET v1/sostenibilita/colture-aziende</c>.</remarks>
    public interface IColtureAziendeDropdownService
    {
        /// <summary>
        /// Returns all distinct crop species present in the impianti of the companies
        /// belonging to the supplied filiere.
        /// </summary>
        /// <param name="piveFiliere">P.IVA list of the selected filiere (from DS-15 result).</param>
        /// <param name="objParametriServer">Server-level request context.</param>
        Task<ColtureAziendeDropdownResult> GetColtureAsync(
            IEnumerable<string> piveFiliere,
            AgronicaCoreParametriServer objParametriServer);
    }
}
