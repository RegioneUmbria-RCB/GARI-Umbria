using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiPassword
{
    /// <summary>
    /// Interfaccia BIZ per le operazioni relative alla password utente.
    /// Riferimento DS: DS01-BL - LeggiDataUltimaModificaPassword.
    /// </summary>
    public interface IUtentiPasswordService
    {
        /// <summary>
        /// Recupera il timestamp dell'ultima modifica della password per l'utente specificato.
        /// </summary>
        Task<(DateTime? DataUltimaModificaPassword, bool UtenteTrovato)> LeggiDataUltimaModificaPasswordAsync(
            string username,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
