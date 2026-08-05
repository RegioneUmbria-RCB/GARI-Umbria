namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS04-BL §Output metadatiPaginazione: Pagination metadata to include in the HTTP response.
    /// </summary>
    /// <param name="PageSize">Effective page size used for this request.</param>
    /// <param name="NextKey">
    /// Cursor for the next page request, or <c>null</c> if this is the last page
    /// (record count returned was less than <paramref name="PageSize"/>).
    /// </param>
    public record KeysetMetadatiPaginazione(int PageSize, string? NextKey);
}
