using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando la validazione finale del payload M4 (DS06-BL) rileva
    /// almeno un errore bloccante.
    /// Porta l'output completo della validazione (lista errori) per consentire al chiamante
    /// di comporre una risposta HTTP dettagliata senza conoscere i dettagli interni del servizio.
    /// </summary>
    public class ValidazioneFinalePayloadCo2Exception : Exception
    {
        /// <summary>Output della validazione con la lista degli errori bloccanti rilevati.</summary>
        public ValidazioneFinalePayloadCo2Output ValidazioneOutput { get; }

        public ValidazioneFinalePayloadCo2Exception(ValidazioneFinalePayloadCo2Output validazioneOutput)
            : base("La validazione finale del payload CO2 ha rilevato errori bloccanti.")
        {
            ValidazioneOutput = validazioneOutput;
        }
    }
}
