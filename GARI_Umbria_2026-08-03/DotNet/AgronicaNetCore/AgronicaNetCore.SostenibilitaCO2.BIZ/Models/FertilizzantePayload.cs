namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo fertilizzante applicato in un'operazione colturale.
    /// I valori N, P, K, Mg, Cu sono ricavati da <c>Mov_Dettaglio_Tecnico</c>.
    /// Riferimento spec: FS2.04 Mapping Oggetto "Fertilizzante" — DS04.2-BL Mappature (<c>Elem_Cod = 3</c>).
    /// </summary>
    public class FertilizzantePayload
    {
        /// <summary>
        /// Quantità di fertilizzante in kg (<c>Mov_Destinazioni.qta</c>).
        /// Riferimento spec: FS2.04 mapping <c>quantita_kg</c>.
        /// </summary>
        public decimal quantita_kg { get; set; }

        public string origine_fertilizzante { get; set; }

        /// <summary>
        /// Lista delle molecole (N, P, K, Mg, Cu) con i rispettivi titoli percentuali.
        /// Gli elementi con valore 0 vengono omessi. Lista non vuota per fertilizzanti validi.
        /// Riferimento spec: FS2.04 mapping <c>molecole[]</c>.
        /// </summary>
        public List<MolecolaPayload> molecole { get; set; } = new List<MolecolaPayload>();
    }
}
