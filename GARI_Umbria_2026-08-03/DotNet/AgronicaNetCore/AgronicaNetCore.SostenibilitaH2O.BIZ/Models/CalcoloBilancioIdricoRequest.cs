namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Corpo della richiesta POST per il calcolo del bilancio idrico.
    /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico — Input.
    /// </summary>
    public class CalcoloBilancioIdricoRequest
    {
        /// <summary>Perimetro di calcolo (aziende, anno, filiera, modalità, colture).</summary>
        public PerimetroCalcoloH2O Perimetro { get; set; } = null!;

        /// <summary>Esercizi territoriali di riferimento per il calcolo.</summary>
        public List<EsercizioH2O> Esercizi { get; set; } = new();
    }
}
