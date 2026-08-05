using AgronicaCoreDTOStd.InData.Anagrafica;
using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Anagrafe.BIZ.Services.AlberoGerarchiaImprese
{
    public interface IAlberoGerarchiaImpreseFASTService
    {
        Task<KendoHierarchicalDataSource> GetNodesGearchiaObjAsync(AgronicaCoreParametriServer objParametriServer, AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera il cono di visibilità organizzativo di un Utente specifico interrogando il database GIAS.
        /// Determina se l'Utente ha visibilità totale o filtrata, esegue la query con il timeout specificato
        /// e restituisce la struttura gerarchica filiere/aziende accessibili.
        /// </summary>
        /// <param name="idUtente">
        /// Identificativo univoco dell'Utente (Username) di cui recuperare il cono di visibilità.
        /// Massimo 256 caratteri, non nullo e non vuoto.
        /// </param>
        /// <param name="timeoutMs">
        /// Timeout in millisecondi per l'esecuzione della query SQL (default: 500 ms).
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo (GerarchiaImprese).</param>
        /// <param name="objParametriUtenti">Parametri di connessione al DB SuperServer (Utenti_Profili).</param>
        /// <returns>
        /// Un <see cref="ConoVisibilitaResponseDto"/> con la lista delle Filiere visibili e le Aziende
        /// accessibili per ciascuna. Restituisce <c>Filiere</c> vuota se l'Utente non ha visibilità assegnata.
        /// </returns>
        /// <exception cref="Exceptions.InvalidUserException">Se <paramref name="idUtente"/> è nullo, vuoto o supera 256 caratteri.</exception>
        /// <exception cref="Exceptions.UserNotFoundException">Se l'Utente non esiste in <c>Utenti_Profili</c>.</exception>
        /// <exception cref="Exceptions.QueryTimeoutException">Se la query SQL supera il <paramref name="timeoutMs"/>.</exception>
        /// <exception cref="Exceptions.DataMappingException">Se il mapping del result set SQL fallisce.</exception>
        /// <remarks>
        /// Design Specification DS01-BL: RecuperoConoVisibilitaUtente - Descrizione.
        /// </remarks>
        Task<ConoVisibilitaResponseDto> GetConoVisibilitaUtenteAsync(
            string idUtente,
            int timeoutMs,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Recupera la struttura organizzativa completa di tutte le Filiere e Aziende presenti
        /// in piattaforma, inclusi i dati di geolocalizzazione e l'Admin di Filiera assegnato.
        /// Non applica filtri di autorizzazione: restituisce l'intera struttura.
        /// </summary>
        /// <param name="timeoutMs">
        /// Timeout in millisecondi per le query al database GIAS (default: 1000 ms).
        /// </param>
        /// <param name="objParametriServer">Parametri di connessione al DB Server operativo (GerarchiaImprese).</param>
        /// <param name="objParametriSuperServer">Parametri di connessione al DB SuperServer (Utenti_Tipologie).</param>
        /// <returns>
        /// Un <see cref="StrutturaFiliereResponseDto"/> con tutte le Filiere ordinate per
        /// <c>IdFiliera</c> ASC; per ciascuna l'admin (o <c>null</c>) e le aziende ordinate
        /// per livello gerarchico ASC poi per <c>IdAzienda</c> ASC.
        /// </returns>
        /// <exception cref="Exceptions.QueryTimeoutException">Se una delle query SQL supera il <paramref name="timeoutMs"/>.</exception>
        /// <exception cref="Exceptions.HierarchyValidationException">Se la gerarchia aziendale contiene cicli.</exception>
        /// <exception cref="Exceptions.DataMappingException">Se il mapping del result set SQL fallisce.</exception>
        /// <exception cref="Exceptions.AdminAssignmentException">Se si verifica un errore nel mapping dell'admin di filiera.</exception>
        /// <remarks>
        /// Design Specification DS02-BL: RecuperoStrutturaFiliereTotale - Descrizione.
        /// FS1.02: API Anagrafica | Query Filiere e Aziende.
        /// </remarks>
        Task<StrutturaFiliereResponseDto> GetStrutturaFiliereAsync(
            int timeoutMs,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti,
            AgronicaCoreParametriSuperServer objParametriSuperServer);
    }
}
