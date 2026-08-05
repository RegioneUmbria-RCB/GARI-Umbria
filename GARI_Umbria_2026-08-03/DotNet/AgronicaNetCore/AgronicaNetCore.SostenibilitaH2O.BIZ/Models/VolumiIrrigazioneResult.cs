namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Risultato aggregato del recupero dei volumi di acqua utilizzati
    /// per irrigazione e fertirrigazione nell'anno di riferimento.
    /// Espone due proiezioni dello stesso dato: per esercizio (modalità "Per Colture")
    /// e per azienda (modalità "Aziendale").
    /// Riferimento spec: DS03-BL RecuperoConsumoIdricoEffettivo — Output.
    /// </summary>
    public class VolumiIrrigazioneResult
    {
        /// <summary>
        /// Volume totale di acqua (in m³) per ciascun esercizio (Progetto_Cod).
        /// Usato in modalità <c>Coltura</c>.
        /// La chiave è l'identificativo numerico dell'esercizio (<c>IdEsercizio</c>).
        /// Valori zero sono esclusi (regola: ignorare volumi NULL o pari a zero).
        /// </summary>
        public IReadOnlyDictionary<int, decimal> VolumiPerEsercizio { get; }

        /// <summary>
        /// Volume totale di acqua (in m³) per ciascuna azienda (PIVA).
        /// Usato in modalità <c>Aziendale</c>.
        /// La chiave è la PIVA dell'azienda.
        /// Valori zero sono esclusi (regola: ignorare volumi NULL o pari a zero).
        /// </summary>
        public IReadOnlyDictionary<string, decimal> VolumiPerAzienda { get; }

        /// <param name="volumiPerEsercizio">Volumi aggregati per esercizio.</param>
        /// <param name="volumiPerAzienda">Volumi aggregati per azienda.</param>
        public VolumiIrrigazioneResult(
            IReadOnlyDictionary<int, decimal> volumiPerEsercizio,
            IReadOnlyDictionary<string, decimal> volumiPerAzienda)
        {
            VolumiPerEsercizio = volumiPerEsercizio;
            VolumiPerAzienda = volumiPerAzienda;
        }
    }
}
