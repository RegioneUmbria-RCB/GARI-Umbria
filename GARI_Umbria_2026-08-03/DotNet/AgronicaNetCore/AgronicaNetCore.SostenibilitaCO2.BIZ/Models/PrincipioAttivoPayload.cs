namespace AgronicaNetCore.SostenibilitaCO2.BIZ.Models
{
    /// <summary>
    /// Rappresenta un principio attivo di un agrofarmaco applicato in un'operazione colturale.
    /// Riferimento spec: DS05.2-BL Assembly Payload M4 Complete — Mapping <c>principi_attivi[]</c>.
    /// </summary>
    public class PrincipioAttivoPayload
    {
        /// <summary>
        /// Codice del principio attivo.
        /// Riferimento spec: DS05.2-BL mapping <c>codice_principio_attivo</c>.
        /// </summary>
        public string codice_principio_attivo { get; set; }

        /// <summary>
        /// Percentuale del principio attivo nel prodotto.
        /// Riferimento spec: DS05.2-BL mapping <c>perc_principio_attivo</c>.
        /// </summary>
        public decimal perc_principio_attivo { get; set; }
    }
}
