namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS06-BL §Output data[]: Represents a single centro aziendale record with geographic
    /// and operational data, as described in FS002.
    /// </summary>
    /// <param name="CodiceAzienda">PIVA of the azienda (maps to <c>ca.PIVA</c>).</param>
    /// <param name="CodiceCentro">Centro code (maps to <c>ca.sa_cod</c>).</param>
    /// <param name="DataInizio">Validity start date (<c>ca.Validita_Inizio</c>).</param>
    /// <param name="DataFine">Validity end date (<c>ca.Validita_Fine</c>); null if open-ended.</param>
    /// <param name="DestinazioneProdotto">
    /// Product destination enum value: SURGELATO, FRESCO, AZIENDA AGRICOLA.
    /// Null or empty if not set (<c>cac.val_cod</c> via LEFT JOIN).
    /// </param>
    /// <param name="Indirizzo">Street address (<c>ind.ind_des</c>).</param>
    /// <param name="Frazione">Hamlet/locality subdivision (<c>ind.frz_des</c>); null if absent.</param>
    /// <param name="Cap">Postal code (<c>ind.CAP</c>).</param>
    /// <param name="Localita">Municipality locality (<c>ist.LOCALITA</c>).</param>
    /// <param name="Provincia">Two-character province code (<c>ist.PROV</c>).</param>
    /// <param name="CodiceIstat">6-character ISTAT code (<c>CONCAT(ist.PROV, ist.COM)</c>).</param>
    public record CentroAziendaleItem(
        string CodiceAzienda,
        string CodiceCentro,
        DateTime DataInizio,
        DateTime? DataFine,
        string? DestinazioneProdotto,
        string Indirizzo,
        string? Frazione,
        string Cap,
        string? Localita,
        string Provincia,
        string CodiceIstat
    );
}
