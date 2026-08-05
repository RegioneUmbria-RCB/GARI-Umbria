using AgronicaNetCore.Anagrafe.BIZ.Exceptions.Orogel;
using AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel
{
    /// <summary>
    /// DS05-BL: Orchestrates the extraction of paginated azienda anagrafica data from the
    /// <c>Imprese</c> table for the FS001 API endpoint.
    /// </summary>
    public interface IOrogelEstrazioniDatiAziendeService
    {
        /// <summary>
        /// DS05-BL §Descrizione: Applies temporal validity filters, optional company code filter
        /// and keyset pagination (via DS04-BL), executes the <c>Imprese</c> query via DAL,
        /// maps results to <see cref="AziendeItem"/> records, and generates the pagination
        /// metadata (next cursor or null on last page).
        /// </summary>
        /// <param name="codiciAzienda">
        /// Pre-normalized PIVA list from DS03-BL. Empty list → no company filter.
        /// </param>
        /// <param name="pageSize">Pre-normalized page size from DS03-BL (1–1000).</param>
        /// <param name="nextKey">Raw cursor from the HTTP request, or <c>null</c>/<c>""</c> for first page.</param>
        /// <param name="objParametriTriple">Connection parameters resolved by DS01-BL for the target archive.</param>
        /// <returns>
        /// An <see cref="EstrazioniDatiAziendeResult"/> with the data array and pagination metadata.
        /// Returns an empty data list (not an error) when no records match.
        /// </returns>
        /// <exception cref="DatabaseConnectionException">Cannot connect to the archive database.</exception>
        /// <exception cref="QueryExecutionException">SQL query execution failed.</exception>
        Task<EstrazioniDatiAziendeResult> EstraiAziendeAsync(
            IReadOnlyList<string> codiciAzienda,
            int pageSize,
            string? nextKey,
            AgronicaCoreParametriTriple objParametriTriple
        );
    }
}
