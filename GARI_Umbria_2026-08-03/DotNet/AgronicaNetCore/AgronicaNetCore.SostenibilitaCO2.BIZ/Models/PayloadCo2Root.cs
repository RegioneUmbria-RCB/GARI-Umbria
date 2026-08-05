namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Radice del payload M4 per il calcolo di sostenibilità CO2.
    /// Contiene il codice e tipo raggruppamento (top-level) e la lista delle aziende.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Output radice.
    /// </summary>
    public class PayloadCo2Root
    {
        /// <summary>
        /// Codice di raggruppamento calcolato dalla BL.
        /// Modalità "Per Colture": <c>Filiera|Coltura|Anno</c>.
        /// Modalità "Aziendale": <c>Filiera|Tutte|Anno</c>.
        /// Riferimento spec: Regola 1.
        /// </summary>
        public string codice_raggruppamento { get; set; } = string.Empty;

        /// <summary>
        /// Tipo di raggruppamento. Sempre <c>"FILIERA-COLTURA-ANNO"</c> indipendentemente dalla modalità.
        /// Riferimento spec: Regola 2.
        /// </summary>
        public string tipo_raggruppamento { get; set; } = "FILIERA-COLTURA-ANNO";

        /// <summary>Lista delle aziende nel perimetro con relativi dati e consumi.</summary>
        public List<AziendaPayload> aziende { get; set; } = new List<AziendaPayload>();
    }
}
