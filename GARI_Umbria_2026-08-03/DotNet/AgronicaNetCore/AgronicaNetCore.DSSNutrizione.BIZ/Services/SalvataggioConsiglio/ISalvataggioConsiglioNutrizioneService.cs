using AgronicaNetCore.Base.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio.Models;
using AgronicaNetCore.DSSNutrizione.BIZ.Services.WidgetNutrizione.Models;
using AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ConsiglioNutrizione.Models;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.SalvataggioConsiglio
{
    /// <summary>
    /// Contratto BIZ per il salvataggio esplicito del consiglio nutrizionale
    /// in risposta al click del pulsante "Salva consiglio" nel widget DSS Nutrizione.
    /// Garantisce unicità per (PIVA, SA_COD, APPEZZA, ID_REG, Data_Consiglio)
    /// restituendo esito DUPLICATE se il consiglio è già stato salvato.
    /// </summary>
    /// <remarks>
    /// Design Specification: DS07-BL Salvataggio Consiglio Nutrizione — Scopo.
    /// DS16-API POST /v1/dss/nutrizione/consigli/salva — Dipendenze Business Logic.
    /// </remarks>
    public interface ISalvataggioConsiglioNutrizioneService
    {
        /// <summary>
        /// Persiste il consiglio nutrizionale contenuto nella risposta aggregata
        /// nella tabella Consigli_Nutrizione_Engine.
        /// Se esiste già un record per la stessa chiave (PIVA, SA_COD, APPEZZA, ID_REG, Data_Consiglio)
        /// restituisce esito DUPLICATE senza effettuare scritture aggiuntive.
        /// L'operazione inserisce un record per ogni elemento NPK in transazione singola.
        /// </summary>
        /// <param name="aggregazione">
        /// Risposta aggregata prodotta da DS05B-API (POST /v1/nutrizione/appezzamenti/consigli/aggregato).
        /// Fornisce gli identificativi appezzamento, la data consiglio e gli elementi NPK.
        /// </param>
        /// <param name="controllaDuplicati">
        /// Se <c>true</c>, verifica la presenza di consigli già salvati per lo stesso appezzamento prima di procedere;
        /// se <c>false</c>, salta il controllo e salva direttamente.
        /// </param>
        /// <param name="username">Username dell'utente autenticato (Username_Creazione).</param>
        /// <param name="objParametriServer">Parametri di connessione al server GIAS.</param>
        /// <param name="cancellationToken">Token di cancellazione della richiesta HTTP.</param>
        /// <returns>
        /// <see cref="SalvataggioConsiglioNutrizioneResponse"/> con salvataggio_id,
        /// esito (SUCCESS | DUPLICATE), messaggio localizzato e data_salvataggio ISO 8601.
        /// </returns>
        /// <remarks>
        /// Design Specification: DS07-BL — Regole di Business; DS07-BL — Descrizione.
        /// </remarks>
        Task<SalvataggioConsiglioNutrizioneResponse> EseguiAsync(
            AggregazioneConsiglioNutrizioneResult aggregazione,
            bool controllaDuplicati,
            string username,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Persiste il consiglio nutrizionale ricevuto direttamente dall'engine nutrizionale
        /// nelle tabelle Consigli_Nutrizione_Engine e Input_Consigli_Nutrizione_Engine.
        /// Restituisce l'ID del primo record inserito, o null se l'engine non ha prodotto consigli.
        /// </summary>
        /// <param name="input">Input originale della richiesta all'engine con i dati impianto e audit.</param>
        /// <param name="esitoEngine">Risposta dell'engine nutrizionale con gli elementi NPK.</param>
        /// <param name="usernameRichiedente">Username dell'utente autenticato (Username_Creazione).</param>
        /// <param name="objParametriServer">Parametri di connessione al server GIAS.</param>
        /// <param name="cancellationToken">Token di cancellazione della richiesta HTTP.</param>
        /// <returns>ID del primo consiglio inserito, o null se nessun elemento è stato persistito.</returns>
        Task<int?> EseguiAsync(
            RichiestaConsiglioNutrizioneInput input,
            EsitoConsiglioNutrizione esitoEngine,
            string usernameRichiedente,
            AgronicaCoreParametriServer objParametriServer,
            CancellationToken cancellationToken = default);
    }
}
