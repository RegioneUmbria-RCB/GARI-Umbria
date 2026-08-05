namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Output del calcolo del bilancio idrico e degli indicatori di sostenibilità.
    /// Contiene un indicatore per ciascun esercizio elaborato.
    /// Riferimento spec: DS07-BL CalcoloBilancioIdricoIndicatori — Output.
    /// </summary>
    public class BilancioIdricoIndicatoriOutput
    {
        /// <summary>Lista degli indicatori di sostenibilità, uno per esercizio elaborato.</summary>
        public List<IndicatoreBilancioIdrico> Indicatori { get; set; } = new();
    }
}
