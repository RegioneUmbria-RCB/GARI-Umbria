using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiPassword
{
    /// <summary>
    /// Interfaccia DAL per la lettura dei dati password utente necessari al controllo scadenza.
    /// </summary>
    public interface IUtentiPassword
    {
        /// <summary>
        /// Recupera il timestamp dell'ultima modifica della password per l'utente specificato.
        /// </summary>
        Task<(DateTime? DataUltimaModificaPassword, bool UtenteTrovato)> LeggiDataUltimaModificaPasswordAsync(
            string username,
            AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
