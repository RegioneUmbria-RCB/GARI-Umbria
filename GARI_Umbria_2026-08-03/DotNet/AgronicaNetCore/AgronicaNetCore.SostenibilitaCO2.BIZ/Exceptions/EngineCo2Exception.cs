namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando l'invocazione al motore esterno M4 restituisce un errore
    /// (HTTP 4xx, 5xx) oppure si verifica un timeout.
    /// Il campo <see cref="Codice"/> identifica la categoria di errore, il campo
    /// <see cref="MessaggioCo2"/> riporta il testo estratto dalla risposta del motore.
    /// Riferimento spec: DS07-API InvocazioneEngineSOstenibilitàCO2 — Risposte di errore.
    /// </summary>
    public class EngineCo2Exception : Exception
    {
        /// <summary>Categoria di errore restituita dal motore M4.</summary>
        public CodiceErroreEngineCo2 Codice { get; }

        /// <summary>
        /// Messaggio estratto dalla risposta del motore (campo <c>message</c>).
        /// <c>null</c> se la risposta non è parseable o se si è verificato un timeout.
        /// </summary>
        public string? MessaggioCo2 { get; }

        /// <summary>Codice HTTP della risposta originale (0 in caso di timeout).</summary>
        public int HttpStatusCode { get; }

        /// <param name="codice">Categoria di errore.</param>
        /// <param name="httpStatusCode">Codice HTTP restituito (0 per timeout).</param>
        /// <param name="messaggioCo2">Testo del messaggio di errore dal corpo della risposta M4.</param>
        public EngineCo2Exception(CodiceErroreEngineCo2 codice, int httpStatusCode, string? messaggioCo2)
            : base(BuildMessage(codice, httpStatusCode, messaggioCo2))
        {
            Codice = codice;
            HttpStatusCode = httpStatusCode;
            MessaggioCo2 = messaggioCo2;
        }

        /// <param name="codice">Categoria di errore.</param>
        /// <param name="httpStatusCode">Codice HTTP restituito (0 per timeout).</param>
        /// <param name="messaggioCo2">Testo del messaggio di errore dal corpo della risposta M4.</param>
        /// <param name="innerException">Eccezione originale (es. <see cref="TaskCanceledException"/> per timeout).</param>
        public EngineCo2Exception(CodiceErroreEngineCo2 codice, int httpStatusCode, string? messaggioCo2, Exception innerException)
            : base(BuildMessage(codice, httpStatusCode, messaggioCo2), innerException)
        {
            Codice = codice;
            HttpStatusCode = httpStatusCode;
            MessaggioCo2 = messaggioCo2;
        }

        private static string BuildMessage(CodiceErroreEngineCo2 codice, int httpStatusCode, string? messaggioCo2)
        {
            var baseMsg = codice switch
            {
                CodiceErroreEngineCo2.ValidationError => "Errore di validazione payload dal motore CO2.",
                CodiceErroreEngineCo2.AuthenticationError => "Errore di autenticazione verso il motore CO2.",
                CodiceErroreEngineCo2.RateLimitExceeded => "Troppe richieste al motore CO2. Riprova tra 1 minuto.",
                CodiceErroreEngineCo2.Co2ServerError => "Errore interno del motore CO2.",
                CodiceErroreEngineCo2.Timeout => "Connessione al motore CO2 non riuscita (timeout >30s).",
                _ => "Risposta non attesa dal motore CO2."
            };

            return httpStatusCode > 0
                ? $"{baseMsg} HTTP {httpStatusCode}. {messaggioCo2}"
                : $"{baseMsg} {messaggioCo2}";
        }
    }
}
