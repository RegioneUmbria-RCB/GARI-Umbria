namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS04-BL §Output: Result of <see cref="IOrogelPaginazioneKeysetService.DecodificaNextKey"/>.
    /// Carries the SQL WHERE fragment, bound SQL parameters, and the decoded cursor key-value map.
    /// </summary>
    /// <param name="FiltroWherePaginazione">
    /// SQL WHERE fragment for keyset filtering (e.g., <c>PIVA &gt; @LastPiva</c> or the
    /// cascading OR form for composite keys). To be injected into the DAL query.
    /// </param>
    /// <param name="ParSqlPaginazione">
    /// SQL parameter dictionary mapping parameter names to their bound values
    /// (e.g., <c>@LastPiva → "ABC123"</c>).
    /// </param>
    /// <param name="NextKeyDecodificato">
    /// Key-value pairs decoded from the incoming cursor, keyed by column name.
    /// <c>null</c> for first-page requests (no incoming cursor).
    /// </param>
    public record KeysetFiltroResult(
        string FiltroWherePaginazione,
        IReadOnlyDictionary<string, object> ParSqlPaginazione,
        IReadOnlyDictionary<string, object>? NextKeyDecodificato
    );
}
