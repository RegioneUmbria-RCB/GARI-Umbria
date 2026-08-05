namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Codice di errore restituito dal motore M4 in risposta a una chiamata fallita.
    /// Riferimento spec: DS07-API — Risposte 400/401/403/429/5xx/Timeout.
    /// </summary>
    public enum CodiceErroreEngineCo2
    {
        /// <summary>Payload inviato non valido secondo lo schema M4 (HTTP 400).</summary>
        ValidationError,

        /// <summary>Token JWT non valido o scaduto (HTTP 401/403).</summary>
        AuthenticationError,

        /// <summary>Troppe richieste al motore; riprovare dopo l'intervallo indicato (HTTP 429).</summary>
        RateLimitExceeded,

        /// <summary>Errore interno del server M4 (HTTP 5xx).</summary>
        Co2ServerError,

        /// <summary>Connessione al motore non riuscita entro i 30 secondi di timeout.</summary>
        Timeout,

        /// <summary>Risposta HTTP non attesa o non decodificabile.</summary>
        UnexpectedResponse
    }
}
