using AgronicaCoreVisibilitaStd;
using AgronicaNetCore.Base.Models;
using System.Data;

namespace AgronicaNetCore.Utenti.DAL.DataLayer.UtentiProfili
{
    public interface IUtentiProfili
    {
        Task<DataTable?> ReadAsync(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0);

        /// <summary>
        /// Controlla in base ai flags su Utenti_Profili se l'utente ha visibilità totale.
        /// Questa funzione rimane approssimativa, ritornando true solo dove è la visibilità totale è ovvia.
        /// Nel caso in cui sia necessario eseguire il calcolo della visibilità per sapere se essa è totale o meno,
        /// questa funzione ritorna false, anche se l'utente potrebbe avere visibilità totale.
        /// Tale logica è stata scelta per evitare di eseguire query complesse ogni volta serva l'informazione.
        /// </summary>
        Task<bool> VisibilitaTotale(string username, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0);

        /// <summary>
        /// Controlla in base ai flags su Utenti_Profili se l'utente ha visibilità nulla.
        /// Questa funzione rimane approssimativa, ritornando true solo dove è la visibilità nulla è ovvia.
        /// Nel caso in cui sia necessario eseguire il calcolo della visibilità per sapere se essa è nulla o meno,
        /// questa funzione ritorna false, anche se l'utente potrebbe avere visibilità nulla.
        /// Tale logica è stata scelta per conformarsi al check di visibilità totale.
        /// </summary>        Task<bool> VisibilitaNulla(string username, AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer, int idServizio = 0);
        Task<bool> VisibilitaTotaleGiasOnline(AgronicaCoreParametriUtenti objParametriUtenti, AgronicaCoreParametriServer objParametriServer);

        /// <summary>
        /// Aggiorna FiltroPraticheAttivo e OperatoreFiltri sul record utenti_profili dell'utente target.
        /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ Persistenze – UPDATE utenti_profili).
        /// </summary>
        Task<bool> AggiornaFiltroPraticheAsync(string idUtente, bool filtroPraticheAttivo, PraticheFilterOperator operatoreFiltri, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Verifica che il record utente esista in utenti_profili per il SuperUser corrente.
        /// DS02-BL – Regola 1 (UserNotFoundException).
        /// </summary>
        Task<bool> EsisteUtenteAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti);
        Task<bool> EsisteUtenteAsync(IEnumerable<string> idUtente, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Legge FiltroPraticheAttivo e OperatoreFiltri per l'utente target.
        /// DS02-BL – GestioneProfiloUtenteFiltriPratiche (§ GET – lettura profilo filtri).
        /// </summary>
        Task<DataTable?> LeggiProfiloFiltriAsync(string idUtente, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Copia la configurazione di visibilità e filtri pratiche da un utente template ad una lista di utenti target.
        /// </summary>
        /// <param name="template">Username dell'utente template da cui copiare la visibilità</param>
        /// <param name="targets">
        ///     Lista di username degli utenti target su cui verrà copiata la visibilità dell'utente template</param>
        /// <param name="copyHierarchy">
        ///     Se impostato a true verranno copiati i valori di descrizione_1 e descrizione_2, 
        ///     altrimenti tali colonne rimarranno invariare sugli utenti target</param>
        /// <param name="copyPratiche">
        ///     Se impostato a true i verranno copiati i valori di filtro_pratiche_attivo e operatore_filtri,
        ///     altrimenti tali colonne rimarranno invariare sugli utenti target</param>
        Task<bool> CopyAsync(string template, IEnumerable<string> targets, bool copyHierarchy, bool copyPratiche, AgronicaCoreParametriUtenti objParametriUtenti);
    }
}
