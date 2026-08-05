namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Perimetro di calcolo per il bilancio idrico H2O.
    /// Riferimento spec: DS-02.2-BL Chiamata WebApi Calcolo Bilancio Idrico — Input Perimetro.
    /// </summary>
    public class PerimetroCalcoloH2O
    {
        /// <summary>Lista delle PIVA delle aziende incluse nel perimetro.</summary>
        public List<string> Aziende { get; set; } = new();

        /// <summary>Anno di riferimento del calcolo (YYYY).</summary>
        public int Anno { get; set; }

        /// <summary>Nome (PIVA) della filiera di riferimento.</summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>
        /// Modalità di calcolo.
        /// Valori ammessi: <c>Coltura</c> | <c>Aziendale</c>.
        /// </summary>
        public string Modalita { get; set; } = string.Empty;

        /// <summary>
        /// Lista dei codici coltura selezionati.
        /// Rilevante solo in modalità <c>Coltura</c>.
        /// </summary>
        public List<string> Colture { get; set; } = new();
    }
}
