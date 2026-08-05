namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS03-BL §Output: Immutable result carrying all normalized common parameters
    /// after successful validation by <see cref="IOrogelParametriComuniService"/>.
    /// </summary>
    /// <param name="AnnoConfermato">The anno value confirmed by cohesion re-validation.</param>
    /// <param name="PageSizeNormalizzato">Page size clamped to [1, 1000]; default 100 when not supplied.</param>
    /// <param name="CodiciAziendaNormalizzati">
    /// Normalized (uppercase, trimmed, de-duplicated) codici azienda list.
    /// Empty list when the caller supplied no filter — meaning no filter is applied (all records returned).
    /// </param>
    public record ParametriComuniNormalizzatiResult(
        int AnnoConfermato,
        int PageSizeNormalizzato,
        IReadOnlyList<string> CodiciAziendaNormalizzati
    );
}
