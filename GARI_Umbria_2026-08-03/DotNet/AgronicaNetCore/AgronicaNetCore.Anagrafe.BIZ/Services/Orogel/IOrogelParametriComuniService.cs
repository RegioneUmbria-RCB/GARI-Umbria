using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS03-BL: Validates and normalizes the common parameters shared by all five Orogel BI APIs
    /// (FS001–FS005): <c>pageSize</c>, <c>codiciAzienda</c>, and <c>anno</c>.
    /// This is a pure in-memory operation with no database interaction.
    /// </summary>
    public interface IOrogelParametriComuniService
    {
        /// <summary>
        /// DS03-BL §Descrizione: Validates and normalizes <paramref name="anno"/>,
        /// <paramref name="pageSize"/>, and <paramref name="codiciAzienda"/>.
        /// </summary>
        /// <param name="anno">Required year in YYYY format (2000 ≤ anno ≤ current year).</param>
        /// <param name="pageSize">
        /// Optional page size. Null → default 100. &lt; 1 → <see cref="InvalidPageSizeException"/>.
        /// &gt; 1000 → silently clamped to 1000.
        /// </param>
        /// <param name="codiciAzienda">
        /// Optional comma-separated string or pre-split collection of company codes.
        /// Null/empty → no filter applied (empty result list).
        /// Invalid format → <see cref="InvalidCodiciAziendaFormatException"/>.
        /// </param>
        /// <returns>A <see cref="ParametriComuniNormalizzatiResult"/> with all normalized values.</returns>
        /// <exception cref="InvalidPageSizeException">pageSize &lt; 1.</exception>
        /// <exception cref="InvalidCodiciAziendaFormatException">A codice fails format rules.</exception>
        /// <exception cref="InvalidAnnoCohesionException">anno fails cohesion re-check.</exception>
        ParametriComuniNormalizzatiResult NormalizzaParametriComuni(
            int anno,
            int? pageSize,
            IEnumerable<string>? codiciAzienda
        );
    }
}
