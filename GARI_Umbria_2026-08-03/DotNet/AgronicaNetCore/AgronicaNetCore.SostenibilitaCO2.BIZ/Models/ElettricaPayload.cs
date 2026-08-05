namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo record energetico nel payload M4 (campo <c>elettricita[]</c>).
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Regola 5, Output <c>aziende[].consumi.elettricita[]</c>.
    /// </summary>
    public class ElettricaPayload
    {
        /// <summary>Data di inizio del periodo di consumo (ISO 8601).</summary>
        public DateOnly data_inizio { get; set; }

        /// <summary>Data di fine del periodo di consumo (ISO 8601).</summary>
        public DateOnly data_fine { get; set; }

        /// <summary>Consumo energetico in kWh.</summary>
        public decimal consumo_kwh { get; set; }

        /// <summary>Percentuale di energia da fonti rinnovabili (0–100).</summary>
        public decimal perc_rinnovabili { get; set; }
    }
}
