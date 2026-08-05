namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta una singola molecola (N, P, K, Mg o Cu) con il relativo titolo percentuale
    /// ricavato da <c>Mov_Dettaglio_Tecnico</c>.
    /// Riferimento spec: FS2.04 Mapping Oggetto "Fertilizzante" — campo <c>molecole[]</c>.
    /// </summary>
    public class MolecolaPayload
    {
        /// <summary>
        /// Identificatore della molecola: <c>"N"</c>, <c>"P"</c>, <c>"K"</c>, <c>"Mg"</c> o <c>"Cu"</c>.
        /// Riferimento spec: FS2.04 mapping <c>molecola</c>.
        /// </summary>
        public string molecola { get; set; } = string.Empty;

        /// <summary>
        /// Titolo percentuale della molecola (<c>Mov_Dettaglio_Tecnico.n/p/k/mg/cu</c>).
        /// Riferimento spec: FS2.04 mapping <c>titolo</c>.
        /// </summary>
        public decimal titolo { get; set; }
    }
}
