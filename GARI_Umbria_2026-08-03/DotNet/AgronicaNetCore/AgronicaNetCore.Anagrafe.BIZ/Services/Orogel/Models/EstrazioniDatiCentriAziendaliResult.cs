namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS06-BL §Output: Wraps the paginated extraction result for FS002 (Centri Aziendali).
    /// Contains keyset pagination metadata and the list of <see cref="CentroAziendaleItem"/> records.
    /// </summary>
    /// <param name="Metadata">Pagination metadata including <c>pageSize</c> and <c>nextKey</c> cursor.</param>
    /// <param name="Data">Ordered list of centro aziendale records for the current page.</param>
    public record EstrazioniDatiCentriAziendaliResult(
        KeysetMetadatiPaginazione Metadata,
        IReadOnlyList<CentroAziendaleItem> Data
    );
}
