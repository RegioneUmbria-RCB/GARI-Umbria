namespace AgronicaNetCore.RischiMeteo.BIZ.Models
{
    /// <summary>
    /// Dettaglio di un errore di costruzione del payload M2 per un Esercizio specifico.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Indipendenza dei payload, errori_dettaglio.
    /// </summary>
    public class ErroreCostruzioneRischiMeteo
    {
        /// <summary>Esercizio per cui la costruzione del payload è fallita.</summary>
        public EsercizioRischiMeteoInput Esercizio { get; init; } = new();

        /// <summary>Tipo dell'eccezione che ha causato il fallimento.</summary>
        public string TipoErrore { get; init; } = string.Empty;

        /// <summary>Messaggio descrittivo del motivo del fallimento.</summary>
        public string Messaggio { get; init; } = string.Empty;
    }
}
