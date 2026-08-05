namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS08-BL §Output: Wraps the paginated extraction result for FS004 (Appezzamenti).
    /// Contains keyset pagination metadata and the list of <see cref="AppezzamentoItem"/> records.
    /// </summary>
    /// <param name="Metadata">Pagination metadata including <c>pageSize</c> and <c>nextKey</c> cursor.</param>
    /// <param name="Data">Ordered list of appezzamento records for the current page.</param>
    public record EstrazioniDatiAppezzamentiResult(
        KeysetMetadatiPaginazione Metadata,
        IReadOnlyList<AppezzamentoItem> Data
    );
}
