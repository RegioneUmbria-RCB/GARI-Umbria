namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Services.RiepilogoRaccolti
{
    /// <summary>
    /// Risposta dell'endpoint <c>POST v1/sostenibilita/riepilogo-raccolti</c>.
    /// </summary>
    public class RiepilogoRaccoltiResult
    {
        /// <summary>
        /// Righe del riepilogo; ciascuna identifica una combinazione univoca
        /// Azienda + Appezzamento + Specie Colturale + Esercizio.
        /// </summary>
        public List<RigaRiepilogoRaccolti> Righe { get; set; } = new();
    }
}
