namespace AgronicaNetCore.RischiMeteo.BIZ.Models
{
    /// <summary>
    /// Risultato aggregato della costruzione di tutti i payload M2 per gli Esercizi selezionati.
    /// Riferimento spec: DS02-BL CostruttoPayloadM2 — Output (livello Filiera).
    /// </summary>
    public class CostruzionePayloadRischiMeteoOutput
    {
        /// <summary>Payload costruiti con successo, uno per Esercizio.</summary>
        public IReadOnlyList<PayloadRischiMeteoRisultato> Payloads { get; init; } =
            Array.Empty<PayloadRischiMeteoRisultato>();

        /// <summary>Dettaglio degli Esercizi per cui la costruzione del payload è fallita.</summary>
        public IReadOnlyList<ErroreCostruzioneRischiMeteo> Errori { get; init; } =
            Array.Empty<ErroreCostruzioneRischiMeteo>();

        /// <summary>Numero totale di Esercizi ricevuti in input.</summary>
        public int TotaleEsercizi { get; init; }

        /// <summary>Numero di payload costruiti con successo.</summary>
        public int TotaleSuccessi => Payloads.Count;

        /// <summary>Numero di Esercizi per cui la costruzione è fallita.</summary>
        public int TotaleErrori => Errori.Count;
    }
}
