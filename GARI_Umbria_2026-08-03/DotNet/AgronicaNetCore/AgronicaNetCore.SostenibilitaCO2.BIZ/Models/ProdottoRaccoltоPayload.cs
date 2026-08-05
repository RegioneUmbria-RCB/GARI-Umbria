namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo prodotto raccolto in un'operazione di raccolta (<c>Lav_Cod = 125</c>).
    /// Riferimento spec: FS2.04 Mapping Oggetto "Prodotto Raccolto" — DS04.2-BL Mappature (<c>Elem_Cod = 210</c>).
    /// </summary>
    public class ProdottoRaccoltоPayload
    {
        /// <summary>
        /// Quantità raccolta in kg (<c>Mov_Destinazioni.qta</c>).
        /// Riferimento spec: FS2.04 mapping <c>quantita_kg</c>.
        /// </summary>
        public decimal quantita_kg { get; set; }

        /// <summary>
        /// Codice specie del prodotto raccolto.
        /// Riferimento spec: DS05.2-BL mapping <c>specie</c>.
        /// </summary>
        public string? specie { get; set; }
    }
}
