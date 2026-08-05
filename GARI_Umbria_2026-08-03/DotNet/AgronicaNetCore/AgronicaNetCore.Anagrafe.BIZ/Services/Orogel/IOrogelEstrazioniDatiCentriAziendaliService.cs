using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS06-BL: Business logic contract for paginated extraction of centro aziendale data (FS002).
    /// Orchestrates DS04-BL keyset pagination and the DAL query against <c>Centri_Aziendali</c>.
    /// </summary>
    public interface IOrogelEstrazioniDatiCentriAziendaliService
    {
        /// <summary>
        /// DS06-BL: Extracts a keyset-paginated page of centri aziendali with geographic
        /// and operational data from the archive database identified by <paramref name="objParametriTriple"/>.
        /// </summary>
        /// <param name="codiciAzienda">Optional PIVA filter. Empty list → all companies.</param>
        /// <param name="pageSize">Number of records per page (1–1000; enforced by DS04-BL).</param>
        /// <param name="nextKey">
        /// Opaque keyset cursor (<c>LastPiva§LastSaCod</c>) from the previous page,
        /// or null for the first page.
        /// </param>
        /// <param name="objParametriTriple">Dynamic DB connection parameters resolved by DS01-BL.</param>
        /// <returns>
        /// An <see cref="EstrazioniDatiCentriAziendaliResult"/> containing pagination metadata
        /// and the current page of <see cref="CentroAziendaleItem"/> records.
        /// </returns>
        Task<EstrazioniDatiCentriAziendaliResult> EstraiCentriAziendaliAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        );
    }
}
