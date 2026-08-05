namespace AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationCalculator
{
    /// <summary>
    /// Risultato del calcolo dello stato di scadenza della password.
    /// Riferimento DS: DS03-BL §Output, §Regole di Business.
    /// </summary>
    /// <param name="PasswordScaduta">True se la password è scaduta, false se ancora valida.</param>
    /// <param name="GiorniDallaModifica">Numero di giorni interi (floor) trascorsi dall'ultima modifica alla data di riferimento.</param>
    /// <param name="GiorniRimanenti">Giorni rimanenti prima della scadenza. Negativo se già scaduta.</param>
    /// <param name="DataScadenzaPrevista">Data prevista di scadenza. Null se <c>DataUltimaModificaPassword</c> era null.</param>
    public record StatoScadenzaPassword(
        bool PasswordScaduta,
        int GiorniDallaModifica,
        int GiorniRimanenti,
        DateTime? DataScadenzaPrevista);
}
