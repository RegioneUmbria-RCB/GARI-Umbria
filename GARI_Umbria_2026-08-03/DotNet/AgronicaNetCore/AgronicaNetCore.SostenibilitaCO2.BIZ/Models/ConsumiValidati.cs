namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Dati di consumo aziendale validati, output di DS02-BL.
    /// Se entrambe le liste sono vuote, rappresenta uno scenario zero-consumo valido.
    /// Riferimento spec: DS02-BL ValidazioneDatiConsumoAziendale — Output <c>consumi_validati</c>.
    /// </summary>
    public class ConsumiValidati
    {
        /// <summary>Lista dei consumi carburante validati. Può essere vuota.</summary>
        public List<CarburanteConsumo> Carburanti { get; set; } = new List<CarburanteConsumo>();

        /// <summary>Lista dei consumi energetici validati. Può essere vuota.</summary>
        public List<EnergiaConsumo> Energia { get; set; } = new List<EnergiaConsumo>();
    }
}
