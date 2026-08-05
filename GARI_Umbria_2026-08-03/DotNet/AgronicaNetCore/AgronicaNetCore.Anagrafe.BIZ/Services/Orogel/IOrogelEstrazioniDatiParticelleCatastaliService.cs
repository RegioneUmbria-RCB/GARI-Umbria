using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS07-BL: Business logic contract for paginated extraction of particella catastale data (FS003).
    /// Orchestrates DS04-BL keyset pagination and the DAL query against
    /// <c>ImpreseXParticelle INNER JOIN ParticelleCatastali</c>.
    /// </summary>
    public interface IOrogelEstrazioniDatiParticelleCatastaliService
    {
        /// <summary>
        /// DS07-BL: Extracts a keyset-paginated page of particelle catastali with conduzione data
        /// from the archive database identified by <paramref name="objParametriTriple"/>.
        /// Each physical cadastral parcel is replicated for every distinct conduzione period.
        /// </summary>
        /// <param name="codiciAzienda">Optional PIVA filter. Empty list → all companies.</param>
        /// <param name="pageSize">Number of records per page (1–1000; enforced by DS04-BL).</param>
        /// <param name="nextKey">
        /// Opaque keyset cursor (<c>LastId</c> as string) from the previous page,
        /// or null for the first page. Decoded to an integer <c>@LastId</c> by DS04-BL.
        /// </param>
        /// <param name="objParametriTriple">Dynamic DB connection parameters resolved by DS01-BL.</param>
        /// <returns>
        /// An <see cref="EstrazioniDatiParticelleCatastaliResult"/> containing pagination metadata
        /// and the current page of <see cref="ParticellaCatastaleItem"/> records.
        /// </returns>
        Task<EstrazioniDatiParticelleCatastaliResult> EstraiParticelleCatastaliAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        );
    }
}
