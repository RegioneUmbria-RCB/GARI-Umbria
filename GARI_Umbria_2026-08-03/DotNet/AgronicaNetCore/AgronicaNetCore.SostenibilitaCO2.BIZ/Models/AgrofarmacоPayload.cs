namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un singolo agrofarmaco applicato in un'operazione colturale.
    /// Il codice è ricavato da <c>Movimenti_Dettagli.Pro_Cod</c> dove <c>Elem_Cod = 191</c>.
    /// Riferimento spec: FS2.04 Mapping Oggetto "Agrofarmaco" — DS04.2-BL Mappature (<c>Elem_Cod = 191</c>).
    /// </summary>
    public class AgrofarmacоPayload
    {
        /// <summary>
        /// Codice agrofarmaco (<c>Movimenti_Dettagli.Pro_Cod</c> dove <c>Elem_Cod = 191</c>).
        /// Riferimento spec: FS2.04 mapping <c>cod_agrofarmaco</c>.
        /// </summary>
        public string codice_agrofarmaco { get; set; } = string.Empty;

        /// <summary>
        /// Quantità applicata (<c>Mov_Destinazioni.qta</c>).
        /// Riferimento spec: FS2.04 mapping <c>quantita</c>.
        /// </summary>
        public decimal quantita_kg { get; set; }

        /// <summary>
        /// Elenco dei principi attivi presenti nel prodotto.
        /// Riferimento spec: DS05.2-BL mapping <c>principi_attivi[]</c>.
        /// </summary>
        public List<PrincipioAttivoPayload> principi_attivi { get; set; } = new List<PrincipioAttivoPayload>();
    }
}
