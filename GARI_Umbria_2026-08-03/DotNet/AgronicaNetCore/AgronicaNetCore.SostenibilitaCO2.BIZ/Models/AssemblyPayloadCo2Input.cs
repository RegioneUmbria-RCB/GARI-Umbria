namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Input per la business logic <c>AssemblyPayloadM4FilieraAzienda</c> (DS03-BL).
    /// Aggrega output di DS01-BL, DS02-BL e parametri selezionati dall'utente.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Input.
    /// </summary>
    public class AssemblyPayloadCo2Input
    {
        /// <summary>Perimetro validato, output di DS01-BL.</summary>
        public PerimetroCalcoloCO2 Perimetro { get; set; } = null!;

        /// <summary>Lista dei consumi carburante validati. Può essere vuota.</summary>
        public List<CarburanteConsumo> Carburanti { get; set; } = new List<CarburanteConsumo>();

        /// <summary>Lista dei consumi energetici validati. Può essere vuota.</summary>
        public List<EnergiaConsumo> Energia { get; set; } = new List<EnergiaConsumo>();



        /// <summary>
        /// Lista dei codici esercizio (<c>Progetto_Cod</c> da tabella <c>Imprese_Progetti</c>)
        /// che definiscono il perimetro di calcolo. Corrisponde agli esercizi "Chiusi" selezionati in DS01-BL.
        /// </summary>
        public List<Esercizio> Esercizi { get; set; } = new List<Esercizio>();
    }
}
