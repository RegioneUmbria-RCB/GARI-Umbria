using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.ControlloScadenzaPasswordAlLogin
{
    /// <summary>
    /// Interfaccia BIZ per l'orchestrazione del controllo di scadenza password durante il flusso di login.
    /// </summary>
    public interface IControlloScadenzaPasswordAlLoginService
    {
        /// <summary>
        /// Controlla lo stato di scadenza password dopo l'autenticazione credenziali.
        /// </summary>
        /// <param name="username">Username gia' autenticato.</param>
        /// <param name="pivaSuperUser">P.IVA del super-user.</param>
        /// <param name="objParametriUtenti">Parametri di connessione al database Utenti.</param>
        /// <param name="objParametriServer">Parametri di connessione al server database per la configurazione.</param>
        /// <returns>Stato password e azione richiesta al controller.</returns>
        Task<RisultatoControlloScadenzaPassword> ControllaScadenzaAsync(
            string username,
            string pivaSuperUser,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriServer objParametriServer);
    }
}
