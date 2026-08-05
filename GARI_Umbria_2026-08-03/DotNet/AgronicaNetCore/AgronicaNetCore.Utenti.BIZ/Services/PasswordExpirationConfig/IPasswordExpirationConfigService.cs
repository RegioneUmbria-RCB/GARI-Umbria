using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.PasswordExpirationConfig
{
    /// <summary>
    /// Interfaccia BIZ per la lettura del parametro di scadenza password dalla configurazione.
    /// Riferimento DS: DS02-BL — LeggiParametroScadenzaPassword.
    /// </summary>
    public interface IPasswordExpirationConfigService
    {
        /// <summary>
        /// Recupera il numero di giorni di validità della password configurato per l'applicazione specificata.
        /// Restituisce <c>null</c> se il parametro non è presente in <c>Configurazione_Siti</c>: in tal caso
        /// il controllo di scadenza va saltato (feature non abilitata per questo sito).
        /// Se il parametro è presente ma non è un intero valido, restituisce il valore di fallback (90 giorni).
        /// Riferimento DS: DS02-BL §Scopo, §Regole di Business, §Output.
        /// </summary>
        /// <param name="objParametriServer">Parametri di connessione al server database (obbligatori).</param>
        /// <returns>
        /// Numero di giorni di validità della password se configurato; <c>null</c> se il parametro è assente
        /// (il chiamante deve saltare il controllo di scadenza).
        /// </returns>
        Task<int?> LeggiParametroScadenzaPasswordAsync(
            AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Recupera il numero di giorni di preavviso per la notifica di scadenza password
        /// dal parametro <c>GIORNI_PREAVVISO_SCADENZA_PASSWORD</c> in <c>Configurazione_Siti</c>.
        /// Restituisce <c>null</c> se il parametro non è presente: in tal caso il chiamante non deve
        /// inviare notifiche. Se presente ma non valido, restituisce il valore di fallback (7 giorni).
        /// </summary>
        Task<int?> LeggiParametroGiorniPreavvisoAsync(
            AgronicaCoreParametriServer objParametriServer);
    }
}
