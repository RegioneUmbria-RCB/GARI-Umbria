using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.BilancioAzoto.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.BilancioAzoto
{
    /// <summary>
    /// Contratto del servizio BIZ per l'aggregazione degli eventi della timeline del bilancio azoto
    /// e l'invio delle richieste consigli nutrizionali aggregate all'engine.
    /// Riferimento: DS09-BL BilancioAzotoAggregazioneEventi.
    /// </summary>
    public interface IBilancioAzotoService
    {
        /// <summary>
        /// Costruisce la timeline annuale degli eventi che modificano i parametri nutrizionali
        /// (cambio BBCH, nuova analisi terreno, fertilizzazione) nell'arco di validità dell'impianto,
        /// aggrega gli eventi per data e invia una singola richiesta consiglio all'engine
        /// per ogni data in cui si verifica almeno un evento.
        /// Tutte le richieste di un singolo grafico utilizzano lo stesso Raccoglitore_Cod.
        /// Riferimento: DS09-BL — Scopo, Descrizione, Regole di Business.
        /// </summary>
        /// <param name="appezzamento">
        /// Dati dell'appezzamento (output DS05-BL): include identificativi impianto,
        /// analisi terreno corrente, fase fenologica corrente e dati colturali.
        /// </param>
        /// <param name="objParametriServer">Parametri server GIAS (connessione DB, utente).</param>
        /// <param name="objParametriSuperServer">Parametri super-server GIAS per le chiavi engine.</param>
        /// <param name="idDbServer">ID del server DB corrente (per il contesto FMIS).</param>
        /// <param name="cancellationToken">Token di cancellazione della richiesta HTTP.</param>
        /// <returns>
        /// Risultato con timeline eventi ordinata cronologicamente e lista delle richieste
        /// consigli aggregate con i rispettivi ID consiglio persistiti.
        /// </returns>
        Task<BilancioAzotoResult> AggregazioneEventiEConsiglioAsync(
            AppezzamentoNutrizioneDto appezzamento,
            AgronicaCoreParametriServer objParametriServer,
            AgronicaCoreParametriSuperServer objParametriSuperServer,
            int idDbServer,
            CancellationToken cancellationToken = default);
    }
}
