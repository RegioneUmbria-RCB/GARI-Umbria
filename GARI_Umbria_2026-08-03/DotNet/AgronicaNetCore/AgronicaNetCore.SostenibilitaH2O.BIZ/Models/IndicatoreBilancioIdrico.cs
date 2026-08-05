namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Indicatore di sostenibilità idrica calcolato per un singolo esercizio colturale.
    /// I valori per-ha sono mantenuti a precisione completa (non arrotondati) perché vengono
    /// usati come input per aggregazioni successive (somme ponderate per superficie).
    /// L'arrotondamento a 2 decimali avviene solo al momento della serializzazione finale:
    /// <c>AggregazionePayloadPerAziendaService</c> (totali m³ aziendale) e
    /// <c>SostenibilitaH2OService</c> (output API per-ha).
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Output.
    /// </summary>
    public class IndicatoreBilancioIdrico
    {
        /// <summary>Identificativo dell'esercizio colturale.</summary>
        public string IdEsercizio { get; set; } = string.Empty;

        /// <summary>
        /// Fabbisogno idrico benchmark per ettaro (m³/ha):
        /// <c>(green_blue_wf * resa_t) / superficie_ha</c>.
        /// Null quando i dati di input sono insufficienti (INDETERMINATO).
        /// </summary>
        public decimal? FabbisognoM3PerHa { get; set; }

        /// <summary>
        /// Consumo idrico effettivo per ettaro (m³/ha):
        /// <c>consumo_m3 / superficie_ha</c>.
        /// Null quando i dati di input sono insufficienti (INDETERMINATO).
        /// </summary>
        public decimal? ConsumataM3PerHa { get; set; }

        /// <summary>
        /// Apporto meteorico netto per ettaro (m³/ha):
        /// <c>(pioggia_mm * superficie_ha * 0.001 * 0.7) / superficie_ha</c>.
        /// Null quando i dati di input sono insufficienti (INDETERMINATO).
        /// </summary>
        public decimal? DaMeteoM3PerHa { get; set; }

        /// <summary>
        /// Delta tra consumo effettivo e benchmark per ettaro (m³/ha):
        /// <c>(consumo_m3 - benchmark_m3) / superficie_ha</c>.
        /// Null quando i dati di input sono insufficienti (INDETERMINATO).
        /// </summary>
        public decimal? DeltaM3PerHa { get; set; }
    }
}
