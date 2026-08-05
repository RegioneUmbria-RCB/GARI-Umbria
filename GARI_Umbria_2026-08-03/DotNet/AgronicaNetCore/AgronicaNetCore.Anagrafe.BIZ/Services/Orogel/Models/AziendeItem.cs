namespace AgronicaNetCore.Anagrafe.BIZ.Services.Orogel.Models
{
    /// <summary>
    /// DS05-BL §Output data[]: A single azienda record returned by the FS001 API.
    /// </summary>
    /// <param name="CodiceAzienda">PIVA of the company (<c>PIVA</c> column).</param>
    /// <param name="RagioneSociale">Company name (<c>rag_soc</c> column).</param>
    /// <param name="DataInizio">Activity start date (<c>Validita_Inizio</c> column). ISO 8601 timestamp.</param>
    /// <param name="DataFine">Activity end date (<c>Validita_Fine</c> column). ISO 8601 timestamp. Null if open-ended.</param>
    public record AziendeItem(
        string CodiceAzienda,
        string RagioneSociale,
        DateTime DataInizio,
        DateTime? DataFine
    );
}
