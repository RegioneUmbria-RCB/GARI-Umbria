namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta una singola semente utilizzata in un'operazione di semina o trapianto.
    /// Il codice è la concatenazione GIAS di <c>Elem_Cod</c> e <c>Mat_Cod</c> da <c>Movimenti_Dettagli</c>.
    /// Riferimento spec: FS2.04 Mapping Oggetto "Semente" — DS04.2-BL Mappature (<c>Elem_Cod = 10</c>).
    /// </summary>
    public class SementePayload
    {
        /// <summary>
        /// Codice semente: concatenazione standard GIAS <c>Elem_Cod|Mat_Cod</c> da <c>Movimenti_Dettagli</c>.
        /// Riferimento spec: FS2.04 mapping <c>cod_semente</c>.
        /// </summary>
        public string codice_semente { get; set; } = string.Empty;

        /// <summary>
        /// Quantità di semente (<c>Mov_Destinazioni.qta</c>).
        /// Riferimento spec: FS2.04 mapping <c>quantita</c>.
        /// </summary>
        public decimal quantita { get; set; }

        /// <summary>
        /// Unità di misura (<c>Movimenti_Dettagli.Udm_Cod</c>).
        /// Riferimento spec: FS2.04 mapping <c>unita_di_misura</c>.
        /// </summary>
        public string uom { get; set; } = string.Empty;
    }
}
