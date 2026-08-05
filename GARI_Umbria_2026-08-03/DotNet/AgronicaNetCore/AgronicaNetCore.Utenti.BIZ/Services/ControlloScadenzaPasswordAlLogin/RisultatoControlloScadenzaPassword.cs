namespace AgronicaNetCore.Utenti.BIZ.Services.ControlloScadenzaPasswordAlLogin
{
    /// <summary>
    /// Risultato dell'orchestrazione del controllo di scadenza password al login.
    /// </summary>
    /// <param name="PasswordScaduta">True se la password e' scaduta.</param>
    /// <param name="AzioneRichiesta">
    /// Azione da eseguire dopo il controllo.
    /// Valori: <see cref="AzioneControlloScadenza.ProcediLoginOrdinario"/> oppure
    /// <see cref="AzioneControlloScadenza.RedirigWizard"/>.
    /// </param>
    public record RisultatoControlloScadenzaPassword(
        bool PasswordScaduta,
        string AzioneRichiesta);

    /// <summary>
    /// Costanti per il campo azione_richiesta del risultato del controllo di scadenza password.
    /// </summary>
    public static class AzioneControlloScadenza
    {
        /// <summary>La password e' valida; il flusso di login ordinario puo' procedere.</summary>
        public const string ProcediLoginOrdinario = "PROCEDI_LOGIN_ORDINARIO";

        /// <summary>La password e' scaduta; il controller deve reindirizzare al wizard di cambio forzato.</summary>
        public const string RedirigWizard = "REDIRIGE_WIZARD";
    }
}
