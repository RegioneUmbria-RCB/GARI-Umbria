namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Output della business logic di validazione finale del payload M4 (DS06-BL).
    /// Riporta l'esito della validazione, gli errori rilevati e — se la validazione ha esito
    /// positivo — il payload verificato pronto per l'invocazione a DS07-API.
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4 — Output.
    /// </summary>
    public class ValidazioneFinalePayloadCo2Output
    {
        /// <summary>
        /// <c>true</c> se il payload supera tutte le validazioni bloccanti; <c>false</c>
        /// se almeno un errore bloccante è stato rilevato.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public bool ValidazioneEsito { get; set; }

        /// <summary>
        /// Lista degli errori di validazione bloccanti rilevati.
        /// Vuota quando <see cref="ValidazioneEsito"/> è <c>true</c>.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public List<ValidazioneErroreCo2> ErroriValidazione { get; set; } = new List<ValidazioneErroreCo2>();

        /// <summary>
        /// Payload validato e pronto per l'invocazione a DS07-API.
        /// Valorizzato solo quando <see cref="ValidazioneEsito"/> è <c>true</c>; <c>null</c> in caso
        /// di errori.
        /// Riferimento spec: DS06-BL.
        /// </summary>
        public PayloadCo2Root? PayloadValidato { get; set; }
    }
}
