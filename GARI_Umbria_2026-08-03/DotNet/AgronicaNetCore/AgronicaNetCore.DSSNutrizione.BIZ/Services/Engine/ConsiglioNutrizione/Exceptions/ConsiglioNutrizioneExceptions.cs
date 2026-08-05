using System.Net;

namespace AgronicaNetCore.DSSNutrizione.BIZ.Services.Engine.ConsiglioNutrizione.Exceptions
{
    /// <summary>
    /// Eccezione generata quando la chiamata HTTP all'engine Nutrizione supera il timeout configurato.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione — Timeout richiesta API: 30 secondi.
    /// </summary>
    public sealed class ConsiglioNutrizioneEngineTimeoutException : Exception
    {
        public int TimeoutSecondi { get; }

        public ConsiglioNutrizioneEngineTimeoutException(int timeoutSecondi)
            : base($"La richiesta all'engine Nutrizione (consiglio) ha superato il timeout di {timeoutSecondi} secondi.")
        {
            TimeoutSecondi = timeoutSecondi;
        }
    }

    /// <summary>
    /// Eccezione generata quando l'engine Nutrizione risponde con HTTP 4xx o 5xx.
    /// Riferimento: DS04-BL Richiesta Consiglio Nutrizione — Gestione degli errori.
    /// </summary>
    public sealed class ConsiglioNutrizioneEngineHttpException : Exception
    {
        public HttpStatusCode StatusCode   { get; }
        public string         ResponseBody { get; }

        public ConsiglioNutrizioneEngineHttpException(HttpStatusCode statusCode, string responseBody)
            : base($"L'engine Nutrizione ha risposto con HTTP {(int)statusCode}: {responseBody}")
        {
            StatusCode   = statusCode;
            ResponseBody = responseBody;
        }
    }

    /// <summary>
    /// Eccezione generata quando la risposta JSON dell'engine non può essere deserializzata.
    /// Riferimento: DS04-BL — Regole di Business.
    /// </summary>
    public sealed class ConsiglioNutrizioneEngineParseException : Exception
    {
        public string Motivo { get; }

        public ConsiglioNutrizioneEngineParseException(string motivo, Exception? inner = null)
            : base($"Impossibile parsare la risposta dell'engine Nutrizione: {motivo}", inner)
        {
            Motivo = motivo;
        }
    }
}
