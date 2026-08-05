using AgronicaNetCore.SostenibilitaCO2.BIZ.Models;

namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Exceptions
{
    /// <summary>
    /// Eccezione sollevata quando il payload M4 contiene uno o più errori di validazione
    /// bloccanti che impediscono l'invocazione del motore esterno DS07-API.
    /// La proprietà <see cref="ErroriValidazione"/> riporta i dettagli di ogni violazione.
    /// Riferimento spec: DS06-BL ValidazioneFinalePayloadM4 — Eccezioni.
    /// </summary>
    public class PayloadValidationException : Exception
    {
        /// <summary>Errori di validazione bloccanti rilevati nel payload M4.</summary>
        public IReadOnlyList<ValidazioneErroreCo2> ErroriValidazione { get; }

        /// <param name="errori">Lista degli errori di validazione bloccanti.</param>
        public PayloadValidationException(IReadOnlyList<ValidazioneErroreCo2> errori)
            : base("Il payload CO2 contiene uno o più errori di validazione bloccanti.")
        {
            ErroriValidazione = errori;
        }

        /// <param name="errori">Lista degli errori di validazione bloccanti.</param>
        /// <param name="innerException">Eccezione interna.</param>
        public PayloadValidationException(IReadOnlyList<ValidazioneErroreCo2> errori, Exception innerException)
            : base("Il payload CO2 contiene uno o più errori di validazione bloccanti.", innerException)
        {
            ErroriValidazione = errori;
        }
    }
}
