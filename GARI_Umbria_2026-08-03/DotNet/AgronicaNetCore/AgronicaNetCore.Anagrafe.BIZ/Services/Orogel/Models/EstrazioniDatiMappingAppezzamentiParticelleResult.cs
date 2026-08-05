namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS09-BL §Output: Wraps the paginated extraction result for FS005 (Mapping Appezzamenti-Particelle).
    /// Contains keyset pagination metadata and the list of <see cref="MappingAppezzamentiParticelleItem"/> records.
    /// </summary>
    /// <param name="Metadata">
    /// Pagination metadata including <c>pageSize</c> and 3-key <c>nextKey</c> cursor
    /// (<c>LastProgettoCod§LastPartCod§LastValiditaInizio</c>), or <c>null</c> if the dataset is exhausted.
    /// </param>
    /// <param name="Data">Ordered list of mapping relation records for the current page.</param>
    public record EstrazioniDatiMappingAppezzamentiParticelleResult(
        KeysetMetadatiPaginazione Metadata,
        IReadOnlyList<MappingAppezzamentiParticelleItem> Data
    );
}
