namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Input per la business logic <c>PersistenzaRisultatiM4Service</c> (DS08-BL).
    /// Aggrega payload di richiesta e risposta M4, modalità di calcolo e contesto
    /// di invocazione necessario per la persistenza nelle tabelle di lookup.
    /// Riferimento spec: DS08-BL PersistenzaRisultatiM4LookupTable — Input.
    /// </summary>
    public class PersistenzaRisultatiCo2Input
    {
        /// <summary>
        /// Modalità di calcolo CO2.
        /// Valori ammessi: <c>Per Colture</c> | <c>Aziendale</c>.
        /// Determina quale coppia di tabelle di lookup viene usata.
        /// </summary>
        public string Modalita { get; set; } = string.Empty;

        /// <summary>
        /// Payload JSON della richiesta inviata al motore M4, output di DS06-BL.
        /// </summary>
        public PayloadCo2Root PayloadRequestCo2 { get; set; } = null!;

        /// <summary>
        /// Risposta deserializzata del motore M4, output di DS07-API.
        /// </summary>
        public RispostaEngineCo2 PayloadResponseCo2 { get; set; } = null!;

        /// <summary>Username dell'utente che ha avviato il calcolo, per audit.</summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>Timestamp UTC dell'invocazione M4 (ISO 8601).</summary>
        public DateTime TimestampInvocazione { get; set; }
    }
}
