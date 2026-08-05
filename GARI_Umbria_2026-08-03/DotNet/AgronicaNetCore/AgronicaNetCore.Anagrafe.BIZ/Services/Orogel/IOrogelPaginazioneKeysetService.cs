using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS04-BL: Manages cursor-based (Keyset) Pagination for all Orogel BI APIs (FS001–FS005).
    /// Decodes incoming cursors into SQL WHERE conditions and generates outgoing cursors from
    /// last-row data. Pure in-memory operation — no database interaction.
    /// </summary>
    public interface IOrogelPaginazioneKeysetService
    {
        /// <summary>
        /// DS04-BL §Prima Pagina / §Pagine Successive: Validates <paramref name="pageSize"/> and
        /// decodes <paramref name="nextKey"/> into an SQL WHERE clause and bound parameters.
        /// For the first page (<paramref name="nextKey"/> is null or empty) default fallback values
        /// are used so the DAL always receives a consistent WHERE structure.
        /// </summary>
        /// <param name="nextKey">Cursor from the previous page, or <c>null</c>/<c>""</c> for the first page.</param>
        /// <param name="pageSize">Pre-normalized page size (must be ≤ 1000).</param>
        /// <param name="colonneOrdinamento">Ordered list of keyset columns defining sort order and types.</param>
        /// <returns>A <see cref="KeysetFiltroResult"/> with the WHERE fragment, SQL parameters, and decoded cursor.</returns>
        /// <exception cref="PaginationSizeExceededException"><paramref name="pageSize"/> exceeds 1000.</exception>
        /// <exception cref="InvalidNextKeyException">Number of cursor segments does not match column count.</exception>
        /// <exception cref="InvalidNextKeyValuesException">A cursor segment cannot be parsed as the expected type.</exception>
        KeysetFiltroResult DecodificaNextKey(
            string? nextKey,
            int pageSize,
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento
        );

        /// <summary>
        /// DS04-BL §Costruzione Metadati e nextKey: Generates pagination metadata and the next
        /// cursor from the last fetched record. Returns <c>null</c> nextKey when
        /// <paramref name="recordEstratti"/> is less than <paramref name="pageSize"/> (last page).
        /// </summary>
        /// <param name="pageSize">Page size used for the current request.</param>
        /// <param name="recordEstratti">Number of records returned by the DAL query.</param>
        /// <param name="colonneOrdinamento">Ordered list of keyset columns defining sort order.</param>
        /// <param name="ultimoRecord">
        /// Column values of the last record in the current page. Keys must match
        /// <see cref="KeysetColonnaDefinizione.NomeColonna"/>. <c>null</c> values use type fallbacks.
        /// </param>
        /// <returns>A <see cref="KeysetMetadatiPaginazione"/> with the effective page size and next cursor.</returns>
        KeysetMetadatiPaginazione GeneraMetadati(
            int pageSize,
            int recordEstratti,
            IReadOnlyList<KeysetColonnaDefinizione> colonneOrdinamento,
            IReadOnlyDictionary<string, object?> ultimoRecord
        );
    }
}
