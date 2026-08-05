namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Perimetro di calcolo CO2 validato, output di DS01-BL.
    /// Contiene le aziende, appezzamenti, esercizi e colture ammissibili
    /// determinati dalla logica <c>ValidazionePerimetroCalcoloCO2</c>.
    /// Riferimento spec: DS01-BL ValidazionePerimetroCalcoloCO2 — Output.
    /// </summary>
    public class PerimetroCalcoloCO2
    {
        /// <summary>Lista delle PIVA delle aziende incluse nel perimetro selezionato.</summary>
        public List<string> Aziende { get; set; } = new List<string>();

        /// <summary>Anno di riferimento del calcolo (YYYY).</summary>
        public int Anno { get; set; }

        /// <summary>Nome della filiera.</summary>
        public string Filiera { get; set; } = string.Empty;

        /// <summary>
        /// Modalità di calcolo scelta dall'utente.
        /// Valori ammessi: <c>Per Colture</c> | <c>Aziendale</c>.
        /// </summary>
        public string Modalita { get; set; } = string.Empty;

        /// <summary>
        /// Lista dei codici coltura selezionati (Veg_Cod).
        /// Non rilevante in modalità Aziendale.
        /// </summary>
        public List<string> Colture { get; set; } = new List<string>();
    }
}
