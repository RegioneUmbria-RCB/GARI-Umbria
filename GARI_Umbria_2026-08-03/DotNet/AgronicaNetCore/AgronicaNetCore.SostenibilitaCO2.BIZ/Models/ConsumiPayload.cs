namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Contenitore dei consumi aziendali nel payload M4.
    /// Riferimento spec: DS03-BL AssemblyPayloadM4FilieraAzienda — Output <c>aziende[].consumi</c>.
    /// </summary>
    public class ConsumiPayload
    {
        /// <summary>Lista carburanti. Array vuoto se nessun consumo (mai null — regola 6).</summary>
        public List<CarburanteAltroPayload> carburanti_altro { get; set; } = new List<CarburanteAltroPayload>();

        /// <summary>Lista consumi elettricità. Array vuoto se nessun consumo (mai null — regola 6).</summary>
        public List<ElettricaPayload> elettricita { get; set; } = new List<ElettricaPayload>();
    }
}
