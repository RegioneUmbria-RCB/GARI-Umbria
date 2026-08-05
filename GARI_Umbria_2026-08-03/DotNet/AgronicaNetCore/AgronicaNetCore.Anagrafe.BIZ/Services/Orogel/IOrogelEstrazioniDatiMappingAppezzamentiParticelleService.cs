using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS09-BL: Business logic contract for paginated extraction of the
    /// appezzamento–particella mapping (FS005).
    /// Uses a custom 3-key keyset cursor (<c>Progetto_Cod§PART_COD§Validita_inizio</c>)
    /// decoded and encoded directly by the service, because <see cref="KeysetTipoDato"/>
    /// does not support datetime keys.
    /// </summary>
    public interface IOrogelEstrazioniDatiMappingAppezzamentiParticelleService
    {
        /// <summary>
        /// DS09-BL: Extracts a 3-key keyset-paginated page of appezzamento–particella
        /// mapping relations from the archive database identified by
        /// <paramref name="objParametriTriple"/>.
        /// </summary>
        /// <param name="codiciAzienda">Optional PIVA filter. Empty list → all companies.</param>
        /// <param name="pageSize">
        /// Number of records per page (1–1000). Validated against the DS09-BL hardcoded
        /// maximum of 1000; throws <see cref="PaginationSizeExceededException"/> if exceeded.
        /// </param>
        /// <param name="nextKey">
        /// Opaque 3-key cursor (<c>LastProgettoCod§LastPartCod§LastValiditaInizio</c>)
        /// from the previous page, or <c>null</c> for the first page.
        /// Decoded internally; throws <see cref="InvalidNextKeyException"/> if the format
        /// is invalid, <see cref="InvalidNextKeyValuesException"/> if a part cannot be parsed.
        /// </param>
        /// <param name="objParametriTriple">Dynamic DB connection parameters resolved by DS01-BL.</param>
        /// <returns>
        /// An <see cref="EstrazioniDatiMappingAppezzamentiParticelleResult"/> containing
        /// pagination metadata and the current page of <see cref="MappingAppezzamentiParticelleItem"/> records.
        /// </returns>
        Task<EstrazioniDatiMappingAppezzamentiParticelleResult> EstraiMappingAppezzamentiParticelleAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        );
    }
}
