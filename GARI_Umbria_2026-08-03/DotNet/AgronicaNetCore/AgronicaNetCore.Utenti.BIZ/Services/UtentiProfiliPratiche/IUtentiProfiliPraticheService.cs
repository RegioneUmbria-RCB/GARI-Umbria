using AgronicaCoreDTOStd.InData.Pratiche;
using AgronicaCoreDTOStd.OutData.Pratiche;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Utenti.BIZ.Services.UtentiProfiliPratiche
{
    /// <summary>
    /// Interfaccia BIZ per la gestione del profilo utente con filtri pratiche.
    /// DS02-BL – GestioneProfiloUtenteFiltriPratiche.
    /// </summary>
    public interface IUtentiProfiliPraticheService
    {
        /// <summary>
        /// Restituisce le pratiche filtrate attualmente associate a un utente.
        /// DS02-BL – step 2 dell'operazione tipica.
        /// </summary>
        Task<SalvaProfiloUtenteFiltriPratiche_OUT> LeggiAsync(string idUtente, AgronicaCoreParametriDouble objParametriDouble);

        /// <summary>
        /// Salva in transazione atomica la configurazione filtri pratiche per l'utente target.
        /// DS02-BL – step 5-7 dell'operazione tipica.
        /// </summary>
        Task<SalvaProfiloUtenteFiltriPratiche_OUT> SalvaAsync(SalvaProfiloUtenteFiltriPratiche_IN input, AgronicaCoreParametriDouble objParametriDouble);

        /// <summary>
        /// Restituisce una preview della visibilità risultante dalla configurazione fornita, senza persistere.
        /// DS06-API – POST preview.
        /// </summary>
        Task<PreviewProfiloUtenteFiltriPratiche_OUT> PreviewAsync(string idUtente, SalvaProfiloUtenteFiltriPratiche_IN input, AgronicaCoreParametriDouble objParametriDouble);

        /// <summary>
        /// Restituisce la configurazione pratiche corrente dell'utente per l'endpoint
        /// GET Profilazione/ConfigurazionePraticheUtente.
        /// Legge OperatoreFiltri e FiltroPraticheAttivo da Utenti_Profili e le pratiche
        /// associate da Utenti_Profili_Pratiche arricchite via vista Servizi_Pratiche (db_utenti).
        /// </summary>
        Task<ConfigurazionePraticheUtente> LeggiConfigurazioneAsync(string username, AgronicaCoreParametriDouble objParametriDouble);

        /// <summary>
        /// Aggiorna la configurazione pratiche dell'utente specificato.
        /// PUT /v1/profilo-utente/ConfigurazionePraticheUtente.
        /// </summary>
        Task<SalvaProfiloUtenteFiltriPratiche_OUT> SalvaConfigurazioneAsync(ConfigurazionePraticheUtente input, AgronicaCoreParametriDouble objParametriDouble);

        /// <summary>
        /// Copia la configurazione di visibilità (gerarchia e/o pratiche) da un utente sorgente
        /// a uno o più utenti target in modo atomico e suddiviso in batch da 10.000 utenti.
        /// DS05-BL / DS09-API – POST /v1/admin/utenti/copia-visibilita.
        /// </summary>
        /// <exception cref="ArgumentException">Se nessuna delle due flag di copia è true.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Se il template o almeno un target non esiste.</exception>
        Task CopiaVisibilita(string template, IEnumerable<string> targets, bool copyHierarchy, bool copyProcedures, AgronicaCoreParametriDouble objParametriDouble);
    }
}
