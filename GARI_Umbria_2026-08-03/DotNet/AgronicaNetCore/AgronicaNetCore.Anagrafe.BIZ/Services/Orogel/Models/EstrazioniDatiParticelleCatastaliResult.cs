namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS07-BL §Output: Wraps the paginated extraction result for FS003 (Particelle Catastali).
    /// Contains keyset pagination metadata and the list of <see cref="ParticellaCatastaleItem"/> records.
    /// Each physical cadastral parcel may appear multiple times — once per conduzione period.
    /// </summary>
    /// <param name="Metadata">Pagination metadata including <c>pageSize</c> and <c>nextKey</c> cursor.</param>
    /// <param name="Data">Ordered list of particella records for the current page.</param>
    public record EstrazioniDatiParticelleCatastaliResult(
        KeysetMetadatiPaginazione Metadata,
        IReadOnlyList<ParticellaCatastaleItem> Data
    );
}
