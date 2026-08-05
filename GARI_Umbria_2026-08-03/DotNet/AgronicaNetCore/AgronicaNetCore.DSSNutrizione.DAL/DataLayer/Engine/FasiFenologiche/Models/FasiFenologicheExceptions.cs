using System;
using System.Net;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.FasiFenologiche.Models
{
    /// <summary>
    /// Eccezione lanciata quando una chiave di configurazione dell'engine non esiste nel sistema di configurazione GIAS.
    /// </summary>
    /// <remarks>
    /// Design Specification: RecuperoConfigurazioneEngineFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class ConfigurationNotFoundException : Exception
    {
        public ConfigurationNotFoundException(string chiave)
            : base($"La chiave di configurazione '{chiave}' non e' stata trovata nel sistema di configurazione.")
        {
            Chiave = chiave;
        }

        public string Chiave { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la configurazione dell'engine esiste ma contiene valori non validi.
    /// </summary>
    /// <remarks>
    /// Design Specification: RecuperoConfigurazioneEngineFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class InvalidConfigurationException : Exception
    {
        public InvalidConfigurationException(string chiave, string motivo)
            : base($"La chiave di configurazione '{chiave}' contiene un valore non valido: {motivo}")
        {
            Chiave = chiave;
            Motivo = motivo;
        }

        public string Chiave { get; }
        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione per parametri non validi nella richiesta fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ValidazioneParametriRichiestaFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class InvalidParameterException : Exception
    {
        public InvalidParameterException(string message, FasiFenologicheEsitoValidazione validationResult)
            : base(message)
        {
            ValidationResult = validationResult;
        }

        public FasiFenologicheEsitoValidazione ValidationResult { get; }
    }

    /// <summary>
    /// Eccezione per campi obbligatori mancanti nella richiesta fasi fenologiche.
    /// </summary>
    /// <remarks>
    /// Design Specification: ValidazioneParametriRichiestaFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class MissingRequiredFieldException : Exception
    {
        public MissingRequiredFieldException(string message, FasiFenologicheEsitoValidazione validationResult)
            : base(message)
        {
            ValidationResult = validationResult;
        }

        public FasiFenologicheEsitoValidazione ValidationResult { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la richiesta all'engine Fasi Fenologiche supera il timeout configurato.
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class EngineTimeoutException : Exception
    {
        public EngineTimeoutException(int timeoutSecondi)
            : base($"La richiesta all'engine ha superato il timeout di {timeoutSecondi} secondi.")
        {
            TimeoutSecondi = timeoutSecondi;
        }

        public int TimeoutSecondi { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando l'engine Fasi Fenologiche restituisce un codice HTTP 4xx o 5xx.
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class EngineHttpException : Exception
    {
        public EngineHttpException(HttpStatusCode statusCode, string responseBody)
            : base($"L'engine ha restituito HTTP {(int)statusCode}: {responseBody}")
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public HttpStatusCode StatusCode { get; }
        public string ResponseBody { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la risposta JSON dell'engine non e' parsabile o manca di campi obbligatori.
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class EngineResponseParseException : Exception
    {
        public EngineResponseParseException(string motivo, Exception? inner = null)
            : base($"Impossibile parsare la risposta dell'engine: {motivo}", inner)
        {
            Motivo = motivo;
        }

        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando l'engine restituisce un array vuoto di fasi fenologiche (varieta' non riconosciuta).
    /// </summary>
    /// <remarks>
    /// Design Specification: ChiamataEngineFasiFenologiche - Eccezioni (Regola 6).
    /// </remarks>
    public sealed class EngineEmptyResponseException : Exception
    {
        public EngineEmptyResponseException(string colturaId, string varietaId)
            : base($"L'engine non ha restituito fasi fenologiche per colturaId='{colturaId}' varietaId='{varietaId}'. Varieta' non riconosciuta.")
        {
            ColturaId = colturaId;
            VarietaId = varietaId;
        }

        public string ColturaId { get; }
        public string VarietaId { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando il bulk insert delle fasi fenologiche fallisce per vincoli di database.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class BulkInsertException : Exception
    {
        public BulkInsertException(string motivo, Exception? inner = null)
            : base($"Bulk insert delle fasi fenologiche fallito: {motivo}", inner)
        {
            Motivo = motivo;
        }

        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la transazione di persistenza viene annullata per errore.
    /// </summary>
    /// <remarks>
    /// Design Specification: PersistenzaFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class TransactionRollbackException : Exception
    {
        public TransactionRollbackException(string motivo, Exception? inner = null)
            : base($"La transazione di persistenza e' stata annullata: {motivo}", inner)
        {
            Motivo = motivo;
        }

        public string Motivo { get; }
    }

    /// <summary>
    /// Eccezione wrapper per errori non previsti durante l'orchestrazione dell'acquisizione fasi fenologiche.
    /// Arricchisce l'eccezione originale con il correlation ID per tracciabilità multi-sistema.
    /// </summary>
    /// <remarks>
    /// Design Specification: OrchestrationAcquisizioneFasiFenologiche - Eccezioni.
    /// </remarks>
    public sealed class OrchestrationException : Exception
    {
        public OrchestrationException(string correlationId, string motivo, Exception? inner = null)
            : base($"Errore durante l'orchestrazione [CorrelationId={correlationId}]: {motivo}", inner)
        {
            CorrelationId = correlationId;
            Motivo = motivo;
        }

        public string CorrelationId { get; }
        public string Motivo { get; }
    }
}
