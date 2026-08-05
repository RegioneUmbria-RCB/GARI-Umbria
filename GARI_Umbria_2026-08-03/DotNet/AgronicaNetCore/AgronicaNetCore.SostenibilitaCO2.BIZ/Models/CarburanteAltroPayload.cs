namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo carburante nel payload M4 (campo <c>carburanti_altro[]</c>).
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Regola 4, Output <c>aziende[].consumi.carburanti_altro[]</c>.
    /// </summary>
    public class CarburanteAltroPayload
    {
        /// <summary>Tipo di carburante (es. Diesel, Benzina, GPL, Metano, Altro).</summary>
        public string tipo_carburante { get; set; } = string.Empty;

        /// <summary>Quantità consumata.</summary>
        public decimal quantita_carburante { get; set; }

        /// <summary>Unità di misura (litri, Kg, m³).</summary>
        public string unita_di_misura_carburante { get; set; } = string.Empty;
    }
}
