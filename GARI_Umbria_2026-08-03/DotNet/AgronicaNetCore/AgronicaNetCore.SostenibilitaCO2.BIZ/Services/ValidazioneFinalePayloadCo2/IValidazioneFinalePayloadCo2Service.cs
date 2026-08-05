using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.ValidazioneFinalePayloadCo2
{
    /// <summary>
    /// Contratto per la business logic di validazione finale del payload M4 (DS06-BL).
    /// Verifica conformità strutturale, semantica e serializzabilità JSON del payload assemblato
    /// da DS03-BL, DS04-BL e DS05-BL prima dell'invocazione al motore esterno DS07-API.
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4.
    /// </summary>
    public interface IValidazioneFinalePayloadCo2Service
    {
        /// <summary>
        /// Esegue la validazione completa del payload M4.
        /// <para>
        /// Controlla: campi obbligatori non-null, cardinalità gerarchica (aziende ≥1,
        /// appezzamenti ≥1, impianti ≥1), valori numerici ≥0, formato WKT centroidi,
        /// serializzabilità JSON.
        /// </para>
        /// </summary>
        /// <param name="payload">Payload M4 completo, output aggregato di DS03-BL + DS04-BL + DS05-BL.</param>
        /// <returns>
        /// <see cref="ValidazioneFinalePayloadCo2Output"/> con l'esito, gli eventuali errori
        /// bloccanti e, se valido, il payload pronto per DS07-API.
        /// </returns>
        /// <exception cref="Exceptions.SostenibilitaCO2.SerializationException">
        /// Se il payload non è serializzabile a JSON valido (errore tecnico di assembly).
        /// </exception>
        ValidazioneFinalePayloadCo2Output Validate(PayloadCo2Root payload);
    }
}
