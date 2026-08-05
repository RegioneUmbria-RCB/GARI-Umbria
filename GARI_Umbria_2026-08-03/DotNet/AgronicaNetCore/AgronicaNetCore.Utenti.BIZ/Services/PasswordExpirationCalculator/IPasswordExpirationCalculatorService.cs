namespace AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationCalculator
{
    /// <summary>
    /// Interfaccia BIZ per il calcolo in-memory dello stato di scadenza della password.
    /// Riferimento DS: DS03-BL — CalcoloStatoScadenzaPassword.
    /// </summary>
    public interface IPasswordExpirationCalculatorService
    {
        /// <summary>
        /// Calcola lo stato di scadenza della password confrontando il timestamp di ultima modifica
        /// con il parametro di validità, alla data di riferimento specificata.
        /// Il calcolo è sincrono e non accede al database.
        /// Riferimento DS: DS03-BL §Scopo, §Input, §Output, §Regole di Business.
        /// </summary>
        /// <param name="dataUltimaModificaPassword">
        /// Timestamp dell'ultima modifica della password. Se null, la password è considerata SCADUTA
        /// (security-first edge case).
        /// </param>
        /// <param name="giorniValidita">Numero di giorni di validità della password (intero positivo).</param>
        /// <param name="dataRiferimento">Data di riferimento per il calcolo (tipicamente la data/ora corrente).</param>
        /// <returns><see cref="StatoScadenzaPassword"/> con lo stato calcolato e i valori derivati.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Sollevata se <paramref name="giorniValidita"/> è zero o negativo.</exception>
        StatoScadenzaPassword CalcolaStatoScadenza(
            DateTime? dataUltimaModificaPassword,
            int giorniValidita,
            DateTime dataRiferimento);
    }
}
