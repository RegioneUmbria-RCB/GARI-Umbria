namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS05-BL §Output: Full paged response for FS001 — wraps the aziende data array
    /// and the pagination metadata (pageSize + next cursor).
    /// </summary>
    /// <param name="Metadata">Pagination metadata for the response envelope.</param>
    /// <param name="Data">
    /// List of <see cref="AziendeItem"/> records for the current page.
    /// Empty list when no records are found — never null (HTTP 200, not an error).
    /// </param>
    public record EstrazioniDatiAziendeResult(
        KeysetMetadatiPaginazione Metadata,
        IReadOnlyList<AziendeItem> Data
    );
}
