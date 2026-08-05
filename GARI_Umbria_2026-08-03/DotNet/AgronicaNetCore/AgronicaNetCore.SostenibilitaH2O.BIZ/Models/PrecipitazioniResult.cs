namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Risultato dell'operazione di recupero delle precipitazioni da Engine M6 (DS05-BL).
    /// <para>
    /// Contiene il totale di pioggia in millimetri aggregato in due modalità,
    /// corrispondenti alle modalità di calcolo del perimetro:
    /// <list type="bullet">
    /// <item><description><see cref="PioggiaMmPerEsercizio"/>: mappa Progetto_Cod → mm totali, per la modalità "Per Colture".</description></item>
    /// <item><description><see cref="PioggiaMmPerAzienda"/>: mappa PIVA → mm totali, per la modalità "Aziendale".</description></item>
    /// </list>
    /// In assenza di stazione meteo nelle vicinanze, la pioggia è impostata a zero
    /// (fallback non bloccante definito nella spec DS05-BL).
    /// </para>
    /// Riferimento spec: DS05-BL RecuperoPrecipitazioniM6.
    /// </summary>
    public sealed class PrecipitazioniResult
    {
        /// <summary>
        /// Totale precipitazioni in mm per ciascun esercizio (Progetto_Cod).
        /// Utilizzato dalla modalità "Per Colture".
        /// Il valore è zero se nessuna stazione meteo era disponibile per l'appezzamento.
        /// </summary>
        public IReadOnlyDictionary<int, decimal> PioggiaMmPerEsercizio { get; }

        /// <summary>
        /// Totale precipitazioni in mm per ciascuna azienda (PIVA, case-insensitive).
        /// Utilizzato dalla modalità "Aziendale".
        /// </summary>
        public IReadOnlyDictionary<string, decimal> PioggiaMmPerAzienda { get; }

        /// <param name="pioggiaMmPerEsercizio">Aggregazione per Progetto_Cod → mm.</param>
        /// <param name="pioggiaMmPerAzienda">Aggregazione per PIVA → mm.</param>
        public PrecipitazioniResult(
            Dictionary<int, decimal> pioggiaMmPerEsercizio,
            Dictionary<string, decimal> pioggiaMmPerAzienda)
        {
            PioggiaMmPerEsercizio = pioggiaMmPerEsercizio
                ?? throw new ArgumentNullException(nameof(pioggiaMmPerEsercizio));
            PioggiaMmPerAzienda = pioggiaMmPerAzienda
                ?? throw new ArgumentNullException(nameof(pioggiaMmPerAzienda));
        }
    }
}
