namespace AgronicaNetCore.RischiMeteo.BIZ.Models
{
    /// <summary>
    /// Input per il servizio di costruzione dei payload M2 (Rischi Meteoclimatici).
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Input.
    /// </summary>
    public class CostruzionePayloadRischiMeteoInput
    {
        /// <summary>Identificativo della Filiera di riferimento.</summary>
        public string IdFiliera { get; init; } = string.Empty;

        /// <summary>Lista degli Esercizi colturali selezionati per il calcolo.</summary>
        public IReadOnlyList<EsercizioRischiMeteoInput> EserciziSelezionati { get; init; } =
            Array.Empty<EsercizioRischiMeteoInput>();
    }
}
