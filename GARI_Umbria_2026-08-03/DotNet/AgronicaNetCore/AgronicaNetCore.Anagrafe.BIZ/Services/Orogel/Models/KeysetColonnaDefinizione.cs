namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS04-BL §Input tipiDatiColonne: Supported data types for keyset ordering columns.
    /// Determines null-fallback values and cursor value parsing behaviour.
    /// </summary>
    public enum KeysetTipoDato
    {
        /// <summary>String (VARCHAR) column — null fallback is empty string <c>""</c>.</summary>
        String,

        /// <summary>Integer (INT/BIGINT) column — null fallback is <c>-1</c>.</summary>
        Integer,
    }

    /// <summary>
    /// DS04-BL §Input ordineColonne / tipiDatiColonne: Defines a single column participating
    /// in the keyset ordering for a given API endpoint.
    /// </summary>
    /// <param name="NomeColonna">SQL column name as it appears in the ORDER BY / WHERE clause (e.g., <c>PIVA</c>, <c>sa_cod</c>).</param>
    /// <param name="TipoDato">Data type used for parsing cursor values and null fallbacks.</param>
    public record KeysetColonnaDefinizione(string NomeColonna, KeysetTipoDato TipoDato);
}
