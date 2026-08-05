using System.Net;

namespace AgronicaNetCore.DSSNutrizione.DAL.DataLayer.Engine.ModelloNutrizione.Models
{
    /// <summary>
    /// Singolo modello di calcolo nutrizionale restituito dall'engine.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Output.modelli.
    /// </summary>
    public sealed class ModelloNutrizioneItem
    {
        /// <summary>Identificatore univoco del modello (campo "id" nella risposta engine).</summary>
        public string Id       { get; init; } = string.Empty;
        /// <summary>Identificatore di dettaglio del modello (campo "detailId" nella risposta engine).</summary>
        public string DetailId { get; init; } = string.Empty;
        /// <summary>Codice del modello (campo "code" nella risposta engine).</summary>
        public string Code     { get; init; } = string.Empty;
    }

    /// <summary>
    /// Eccezione lanciata quando la chiamata HTTP all'engine Nutrizione supera il timeout.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Timeout massimo 30 secondi.
    /// </summary>
    public sealed class ModelloNutrizioneEngineTimeoutException : Exception
    {
        public ModelloNutrizioneEngineTimeoutException(int timeoutSecondi)
            : base($"La richiesta all'engine Nutrizione ha superato il timeout di {timeoutSecondi} secondi.")
        {
            TimeoutSecondi = timeoutSecondi;
        }

        public int TimeoutSecondi { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando l'engine Nutrizione restituisce HTTP 4xx o 5xx.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Regole di Business.
    /// </summary>
    public sealed class ModelloNutrizioneEngineHttpException : Exception
    {
        public ModelloNutrizioneEngineHttpException(HttpStatusCode statusCode, string responseBody)
            : base($"L'engine Nutrizione ha restituito HTTP {(int)statusCode}: {responseBody}")
        {
            StatusCode   = statusCode;
            ResponseBody = responseBody;
        }

        public HttpStatusCode StatusCode   { get; }
        public string         ResponseBody { get; }
    }

    /// <summary>
    /// Eccezione lanciata quando la risposta JSON dell'engine Nutrizione non è parsabile
    /// o manca di campi obbligatori.
    /// Riferimento: DS03-BL Verifica Disponibilità Modelli Nutrizione — Regole di Business.
    /// </summary>
    public sealed class ModelloNutrizioneEngineParseException : Exception
    {
        public ModelloNutrizioneEngineParseException(string motivo, Exception? inner = null)
            : base($"Impossibile parsare la risposta dell'engine Nutrizione: {motivo}", inner)
        {
            Motivo = motivo;
        }

        public string Motivo { get; }
    }
}
