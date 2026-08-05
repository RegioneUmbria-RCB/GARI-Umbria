using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS08-BL: Business logic contract for paginated extraction of appezzamento data (FS004).
    /// Orchestrates DS04-BL keyset pagination and the DAL query against
    /// <c>Impresa_Progetti INNER JOIN Reg_Impianti</c>.
    /// </summary>
    public interface IOrogelEstrazioniDatiAppezzamentiService
    {
        /// <summary>
        /// DS08-BL: Extracts a keyset-paginated page of appezzamenti with colture, production
        /// and certification data from the archive database identified by <paramref name="objParametriTriple"/>.
        /// </summary>
        /// <param name="codiciAzienda">Optional PIVA filter. Empty list → all companies.</param>
        /// <param name="pageSize">Number of records per page (1–1000; enforced by DS04-BL).</param>
        /// <param name="nextKey">
        /// Opaque keyset cursor (<c>LastProgettoCod</c> as string) from the previous page,
        /// or null for the first page. Decoded to an integer <c>@LastProgettoCod</c> by DS04-BL.
        /// </param>
        /// <param name="objParametriTriple">Dynamic DB connection parameters resolved by DS01-BL.</param>
        /// <returns>
        /// An <see cref="EstrazioniDatiAppezzamentiResult"/> containing pagination metadata
        /// and the current page of <see cref="AppezzamentoItem"/> records.
        /// </returns>
        Task<EstrazioniDatiAppezzamentiResult> EstraiAppezzamentiAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        );
    }
}
