namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Input per la business logic <c>InvocazioneMotoreM4Service</c> (DS07-API).
    /// Aggrega il payload validato da DS06-BL con il contesto di invocazione necessario
    /// per l'audit e la persistenza nelle tabelle di lookup.
    /// Riferimento spec: DS07-API InvocazioneEngineSOstenibilitàCO2.
    /// </summary>
    public class InvocazioneEngineCo2Input
    {
        /// <summary>
        /// Payload M4 validato, output di DS06-BL (<see cref="ValidazioneFinalePayloadCo2Output.PayloadValidato"/>).
        /// </summary>
        public PayloadCo2Root PayloadValidato { get; set; } = null!;

        /// <summary>Nome della filiera di riferimento.</summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>Anno di campagna (YYYY).</summary>
        public int Anno { get; set; }

        /// <summary>
        /// Modalità di calcolo: <c>Per Colture</c> | <c>Aziendale</c>.
        /// Determina quale coppia di tabelle di lookup viene usata per la persistenza.
        /// </summary>
        public string Modalita { get; set; } = string.Empty;

        /// <summary>Username dell'utente che ha avviato il calcolo, per audit.</summary>
        public string Username { get; set; } = string.Empty;
    }
}
