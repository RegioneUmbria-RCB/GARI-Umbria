using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaNetCore.Base.Models;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfiliPratiche
{
    /// <summary>
    /// Interfaccia DAL per la tabella Utenti_Profili_Pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Persistenze – Utenti_Profili_Pratiche).
    /// </summary>
    public interface IUtentiProfiliPratiche
    {
        /// <summary>Legge le pratiche filtrate associate a un utente (da database_utenti).</summary>
        Task<DataTable> LeggiAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Elimina tutte le righe Utenti_Profili_Pratiche per l'utente specificato (da database_utenti).
        /// DS02-BL – Regola 6 (DELETE vecchie righe).
        /// </summary>
        Task<bool> EliminaTutteAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Inserisce una riga per ogni pratica selezionata (da database_utenti).
        /// DS02-BL – Regola 5 (INSERT nuove righe).
        /// </summary>
        Task<bool> InserisciAsync(string idUtente, List<PraticaFiltrata_IN> pratiche, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Verifica che ogni Servizio_Cod sia presente in Servizi con Inviato = 1 (da database_server).
        /// DS02-BL – Regola 8 (InvalidServiceCodeException pre-transazione).
        /// </summary>
        Task<bool> ServiziSonoValidiAsync(List<int> serviziCod, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Legge descrizione, validità e IsValida per i codici servizio specificati dalla vista Servizi_Pratiche (db utenti).
        /// Usato per arricchire la response GET con i dati del catalogo.
        /// </summary>
        Task<DataTable> LeggiServiziDettaglioAsync(List<int> serviziCod, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Copia le pratiche in visibilita di un utente template a una lista di utenti target.
        /// </summary>
        /// <param name="template">Username dell'utente template da cui copiare le pratiche</param>
        /// <param name="targets">Lista di username degli utenti target su cui copiare le pratiche</param>
        Task<bool> CopiaAsync(string template, IEnumerable<string> targets, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
