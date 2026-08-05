namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Dati di un singolo esercizio colturale usati come input per l'aggregazione
    /// del payload aziendale di sostenibilità idrica.
    /// La superficie è usata come peso per convertire i valori per-ha in valori assoluti m³.
    /// I campi nullable rappresentano indicatori INDETERMINATO (da DS07-BL); vengono trattati
    /// come zero durante l'aggregazione.
    /// Riferimento spec: DS09-BL AggregazionePayloadPerAzienda — indicatori_esercizi[].
    /// </summary>
    public class AggregazioneEsercizioInput
    {
        /// <summary>
        /// Superficie dell'appezzamento in ettari (ha).
        /// Usata come fattore di peso nell'aggregazione: m³_totali = m3_per_ha × superficie_ha.
        /// </summary>
        public decimal SuperficieHa { get; set; }

        /// <summary>
        /// Fabbisogno idrico benchmark per ettaro (m³/ha). Null se INDETERMINATO (DS07-BL).
        /// </summary>
        public decimal? FabbisognoM3PerHa { get; set; }

        /// <summary>
        /// Consumo idrico effettivo per ettaro (m³/ha). Null se INDETERMINATO (DS07-BL).
        /// </summary>
        public decimal? ConsumataM3PerHa { get; set; }

        /// <summary>
        /// Apporto meteorico netto per ettaro (m³/ha). Null se INDETERMINATO (DS07-BL).
        /// </summary>
        public decimal? DaMeteoM3PerHa { get; set; }

        /// <summary>
        /// Delta tra consumo effettivo e benchmark per ettaro (m³/ha). Null se INDETERMINATO (DS07-BL).
        /// </summary>
        public decimal? DeltaM3PerHa { get; set; }
    }
}
