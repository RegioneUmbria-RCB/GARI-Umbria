using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.NotificaScadenzaPassword
{
    public interface IUtentiNotificaScadenzaPasswordService
    {
        /// <summary>
        /// Recupera la lista degli utenti la cui password scadrà entro il numero di giorni
        /// configurato in <c>GIORNI_PREAVVISO_SCADENZA_PASSWORD</c> (Configurazione_Siti, default 7).
        /// Restituisce lista vuota se la feature è disabilitata (PASSWORD_EXPIRATION_DAYS assente).
        /// </summary>
        Task<List<UtenteInScadenzaPasswordDto>> GetUtentiInScadenzaAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera la lista degli utenti in scadenza password e l'URL precomposto per il cambio password.
        /// L'URL viene composto leggendo <c>LinkAgronicaAgenda2010</c> (e facoltativamente
        /// <c>LanToWebSiteBasePath</c>) da <c>Configurazione_Siti</c>.
        /// Restituisce lista vuota e URL null se la feature è disabilitata.
        /// </summary>
        Task<UtentiInScadenzaConUrlResult> GetUtentiInScadenzaConUrlAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera gli utenti in scadenza password, compone l'URL di cambio password e invia le
        /// email di notifica. Gestisce la lingua di ogni utente tramite <c>ICultureService</c>.
        /// Restituisce il conteggio di email inviate/fallite e i messaggi di log.
        /// </summary>
        Task<NotificheScadenzaPasswordResult> InviaNotificheScadenzaPasswordAsync(
            string pivaSuperUser,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
