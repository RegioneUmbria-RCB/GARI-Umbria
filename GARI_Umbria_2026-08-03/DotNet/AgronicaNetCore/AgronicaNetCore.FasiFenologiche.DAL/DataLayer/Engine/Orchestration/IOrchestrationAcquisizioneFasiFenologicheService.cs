using AgronicaNetCore.Base.Models;
using AgronicaNetCore.FasiFenologiche.DAL.DataLayer.Engine.Models;
using InData.Engine.FasiFenologiche;
using OutData.Engine.FasiFenologiche;

namespace AgronicaNetCore.FasiFenologiche.DAL.DataLayer.Engine.Orchestration
{
    /// <summary>
    /// Contratto del servizio di orchestrazione per l'acquisizione fasi fenologiche.
    /// Definito nel layer DAL per consentire il consumo cross-BIZ senza violare
    /// il vincolo architetturale "un progetto BIZ non può includere un altro BIZ".
    /// </summary>
    /// <remarks>
    /// Design Specification: DS01-BL Acquisizione Fasi Fenologiche da Engine Nutrizione — Scopo.
    /// DS12-API: POST /v1/dss/nutrizione/fasi-fenologiche/acquisisci — Phase 1.
    /// </remarks>
    public interface IOrchestrationAcquisizioneFasiFenologicheService
    {
        /// <summary>
        /// Esegue il flusso end-to-end di acquisizione fasi fenologiche:
        /// validazione → configurazione engine → chiamata engine → persistenza QDCA.
        /// </summary>
        /// <param name="request">Dati dell'impianto e parametri della richiesta all'engine.</param>
        /// <param name="usernameRichiedente">Username dell'utente autenticato che ha originato la richiesta.</param>
        /// <param name="objParametriServer">Parametri server GIAS per accesso a configurazione e persistenza.</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per accesso alla configurazione chiavi engine.</param>
        /// <param name="cancellationToken">Token di cancellazione.</param>
        /// <returns>
        /// Risposta strutturata con esito booleano, elenco fasi acquisite, durata e dettaglio errore opzionale.
        /// </returns>
        /// <remarks>
        /// Design Specification: DS01-BL — Descrizione; DS12-API — Phase 1.1.
        /// </remarks>
        Task<AcquisizioneFasiFenologicheResponse> EseguiAsync(
            AcquisizioneFasiFenologicheRequest request,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            CancellationToken cancellationToken = default);
    }
}
