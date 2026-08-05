using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Corpo della richiesta per l'endpoint di acquisizione e trasformazione pesate Datamars.
    /// <para>Riferimento spec: DS11-API Endpoint Acquisizione e Trasformazione Pesate Unificato — Formato Richiesta.</para>
    /// </summary>
    public class AcquisizioneTrasformazionePesateRequest
    {
        /// <summary>
        /// Credenziali OAuth2 per l'autenticazione verso l'API Datamars.
        /// Il token viene ottenuto e rinnovato internamente dal servizio di acquisizione.
        /// </summary>
        [Required]
        [JsonProperty("credenziali")]
        public DatamarsCredentials Credenziali { get; set; }

        /// <summary>Parametri per la fase di acquisizione (DS01-BL).</summary>
        [JsonProperty("acquisizione")]
        public AcquisizioneOptions Acquisizione { get; set; } = new AcquisizioneOptions();

        /// <summary>Parametri per la fase di trasformazione (DS02-BL).</summary>
        [JsonProperty("trasformazione")]
        public TrasformazioneOptions Trasformazione { get; set; } = new TrasformazioneOptions();

        /// <summary>Opzioni globali di orchestrazione delle due fasi.</summary>
        [JsonProperty("options")]
        public GlobalOptions Options { get; set; } = new GlobalOptions();
    }

    /// <summary>
    /// Credenziali OAuth2 per l'autenticazione verso l'API Datamars.
    /// <para>Riferimento spec: DS01-BL AcquisizionePesateDatamarsAPI — Autenticazione DataMars API.</para>
    /// </summary>
    public sealed class DatamarsCredentials
    {
        /// <summary>Tipo di grant OAuth2 (es. <c>client_credentials</c>). Obbligatorio.</summary>
        [Required]
        [JsonProperty("grant_type")]
        public string GrantType { get; set; } = string.Empty;

        /// <summary>Client ID dell'applicazione registrata su Datamars. Obbligatorio.</summary>
        [Required]
        [JsonProperty("client_id")]
        public string ClientId { get; set; } = string.Empty;

        /// <summary>Client Secret dell'applicazione registrata su Datamars. Obbligatorio.</summary>
        [Required]
        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; } = string.Empty;
    }

    /// <summary>
    /// Opzioni per la fase di acquisizione pesate da API Datamars.
    /// <para>Riferimento spec: DS11-API — sezione <c>acquisizione</c>.</para>
    /// </summary>
    public sealed class AcquisizioneOptions
    {
        /// <summary>
        /// Se true, reimporta anche sessioni già processate ignorando la deduplicazione.
        /// Utile per forzatura manuale. Default: false.
        /// </summary>
        [JsonProperty("forceReacquisition")]
        public bool ForceReacquisition { get; set; }

        /// <summary>
        /// Numero massimo di retry su fallimento API Datamars. Range: 1–5. Default: 3.
        /// </summary>
        [Range(1, 5)]
        [JsonProperty("maxRetries")]
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Timeout in secondi per singole richieste verso l'API Datamars. Range: 10–300. Default: 60.
        /// </summary>
        [Range(10, 300)]
        [JsonProperty("timeoutSeconds")]
        public int TimeoutSeconds { get; set; } = 60;
    }

    /// <summary>
    /// Opzioni per la fase di trasformazione pesate da staging verso operazioni agenda.
    /// <para>Riferimento spec: DS11-API — sezione <c>trasformazione</c>.</para>
    /// </summary>
    public sealed class TrasformazioneOptions
    {
        /// <summary>
        /// Numero massimo di pesate da trasformare in una esecuzione. Range: 1–5000. Default: 500.
        /// </summary>
        [Range(1, 5000)]
        [JsonProperty("batchSize")]
        public int BatchSize { get; set; } = 500;

        /// <summary>
        /// Se true, salta la verifica duplicati su Agenda (solo per test). Default: false.
        /// </summary>
        [JsonProperty("skipDuplicateCheck")]
        public bool SkipDuplicateCheck { get; set; }
    }

    /// <summary>
    /// Opzioni globali di orchestrazione del flusso acquisizione → trasformazione.
    /// <para>Riferimento spec: DS11-API — sezione <c>options</c>.</para>
    /// </summary>
    public sealed class GlobalOptions
    {
        /// <summary>
        /// Se false, esegue la trasformazione anche se l'acquisizione fallisce parzialmente.
        /// Default: true.
        /// </summary>
        [JsonProperty("executeTransformationOnlyIfAcquisitionSucceeds")]
        public bool ExecuteTransformationOnlyIfAcquisitionSucceeds { get; set; } = true;

        /// <summary>
        /// Se true, interrompe la trasformazione al primo errore. Default: false.
        /// </summary>
        [JsonProperty("stopOnFirstTransformationError")]
        public bool StopOnFirstTransformationError { get; set; } = true;
    }
}
