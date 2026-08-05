namespace AgronicaNetCore.SostenibilitaCO2.DAL.DataLayer.AnalisiTerreno
{
    /// <summary>
    /// Entity che rappresenta il valore del parametro Sostanza Organica (<c>SO</c>)
    /// per un appezzamento, ricavato dalla tabella <c>Analisi_Terreno</c>.
    /// Riferimento spec: DS04-BL AssemblyPayloadM4AppezzamentoImpianto — Persistenze, Tabella <c>Analisi_Terreno</c>, Regola 3.
    /// </summary>
    public class AnalisiTerrenoEntity
    {
        /// <summary>Partita IVA dell'azienda.</summary>
        public string Piva { get; set; } = string.Empty;

        /// <summary>Codice azienda satellite.</summary>
        public string SaCod { get; set; } = string.Empty;

        /// <summary>Codice appezzamento.</summary>
        public string Appezza { get; set; } = string.Empty;

        /// <summary>
        /// Percentuale di sostanza organica nel suolo (parametro <c>SO</c>).
        /// <c>null</c> se il dato non è presente o non parsabile.
        /// </summary>
        public decimal? SostanzaOrganica { get; set; }
    }
}
