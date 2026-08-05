using AgronicaNetCore.RischiMeteo.BIZ.Models.RischiMeteo.Request;

namespace AgronicaNetCore.RischiMeteo.BIZ.Models
{
    /// <summary>
    /// Risultato della costruzione del payload M2 per un singolo Esercizio colturale.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Output (payload per Esercizio).
    /// </summary>
    public class PayloadRischiMeteoRisultato
    {
        /// <summary>Esercizio di riferimento per cui il payload è stato costruito.</summary>
        public EsercizioRischiMeteoInput Esercizio { get; init; } = new();

        /// <summary>Payload JSON costruito per l'invocazione M2.</summary>
        public AssessRiskRequest Payload { get; init; } = new();

        /// <summary>
        /// Lista di warning non bloccanti generati durante la costruzione
        /// (es. poligono non disponibile, date scambiate).
        /// </summary>
        public IReadOnlyList<string> Avvisi { get; init; } = Array.Empty<string>();
    }
}
