using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using InData.Engine.FasiFenologiche;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione
{
    /// <summary>
    /// Interfaccia del servizio BIZ per il caricamento dei dati widget DSS Nutrizione.
    /// Riferimento: DS05-BL Caricamento Dati Widget Nutrizione; DS11-API GET /v1/dss/nutrizione/appezzamenti.
    /// </summary>
    public interface IWidgetNutrizioneService
    {
        /// <summary>
        /// Carica i dati aggregati di tutti gli appezzamenti attivi dell'azienda per il widget DSS Nutrizione,
        /// con supporto alla paginazione.
        /// Include dati colturali, analisi terreno, fase fenologica corrente e ultimo consiglio nutrizionale.
        /// Riferimento: DS05-BL - CaricamentoDatiWidgetNutrizione.
        /// </summary>
        /// <param name="piva">Partita IVA dell'azienda (obbligatoria).</param>
        /// <param name="saCod">Codice centro aziendale; se null, vengono usati tutti i centri autorizzati.</param>
        /// <param name="annoSolare">Anno solare per il filtro di validità. Default: anno corrente.</param>
        /// <param name="skip">Numero di record da saltare per la paginazione. Default: 0.</param>
        /// <param name="take">Numero di record da restituire per la paginazione. Default: 50.</param>
        /// <param name="objParametriServer">Parametri server (connessione, utente, ecc.).</param>
        /// <param name="objParametriUtenti">Parametri utente per la verifica dei permessi di visibilità.</param>
        Task<DatiWidgetNutrizioneResult> CaricaDatiWidgetNutrizioneAsync(
            string piva,
            int saCod,
            int annoSolare,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriUtenti objParametriUtenti);

        /// <summary>
        /// Coordina in parallelo i tre engine nutrizionali (fenologia, modelli, nutrizione)
        /// per un singolo appezzamento e aggrega le risposte in un unico risultato strutturato.
        /// Implementa il timeout per engine e la gestione degli errori parziali:
        /// un engine fallito non blocca gli altri.
        /// </summary>
        /// <param name="appezzamento">Dati dell'appezzamento (output di DS05-BL) usati come contesto di orchestrazione.</param>
        /// <param name="objParametriServer">Parametri server GIAS.</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per accesso alle chiavi engine.</param>
        /// <param name="cancellationToken">Token di cancellazione della richiesta HTTP.</param>
        /// <returns>
        /// Risultato aggregato con consiglio nutrizionale, risposte dei tre engine,
        /// stato aggregazione, errori parziali e metriche temporali.
        /// </returns>
        /// <remarks>
        /// Design Specification: DS05B-API POST /v1/nutrizione/appezzamenti/consigli/aggregato.
        /// </remarks>
        Task<AggregazioneConsiglioNutrizioneResult> AggregaConsiglioNutrizioneAsync(
            AppezzamentoNutrizioneDto appezzamento,
            bool salvaConsiglioNutrizione,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int idDbServer,
            CancellationToken cancellationToken = default);
    }
}

