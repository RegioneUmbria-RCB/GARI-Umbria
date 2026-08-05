namespace AgronicaNetCore.SostenibilitaH2O.BIZ.Models
{
    /// <summary>
    /// Risultato dell'operazione di recupero della resa produttiva (DS04-BL).
    /// <para>
    /// Contiene la resa in tonnellate aggregata in due modalità, corrispondenti
    /// alle modalità di calcolo del perimetro:
    /// <list type="bullet">
    /// <item><description><see cref="ResaPerEsercizio"/>: mappa Progetto_Cod → resa (t), per la modalità "Per Colture".</description></item>
    /// <item><description><see cref="ResaPerAzienda"/>: mappa PIVA → resa (t), per la modalità "Aziendale".</description></item>
    /// </list>
    /// Se non vengono trovate operazioni di raccolta, viene sollevata
    /// <see cref="Exceptions.NoHarvestDataException"/> anziché restituire questo oggetto.
    /// </para>
    /// Riferimento spec: DS04-BL RecuperoResaProduttiva.
    /// </summary>
    public sealed class ResaProduttivaResult
    {
        /// <summary>
        /// Resa totale in tonnellate per ciascun esercizio (Progetto_Cod).
        /// Utilizzato dalla modalità "Per Colture".
        /// </summary>
        public IReadOnlyDictionary<int, decimal> ResaPerEsercizio { get; }

        /// <summary>
        /// Resa totale in tonnellate per ciascuna azienda (PIVA, case-insensitive).
        /// Utilizzato dalla modalità "Aziendale".
        /// </summary>
        public IReadOnlyDictionary<string, decimal> ResaPerAzienda { get; }

        /// <summary>
        /// Codice specie vegetale GIAS (Veg_Cod da SpecieVegetali) per esercizio.
        /// Ricavato da <c>LeggiImpiantiAsync</c> sulle operazioni di raccolta (DS04-BL).
        /// Usato da DS06-BL per il lookup benchmark Water Footprint.
        /// </summary>
        public IReadOnlyDictionary<int, int> VegCodPerEsercizio { get; }

        /// <summary>
        /// Codice cultivar GIAS (Cul_Cod da Cultivar) per esercizio.
        /// Ricavato da <c>LeggiImpiantiAsync</c> sulle operazioni di raccolta (DS04-BL).
        /// </summary>
        public IReadOnlyDictionary<int, int> CulCodPerEsercizio { get; }

        /// <summary>
        /// Dati anagrafici dell'impianto per esercizio (Progetto_Cod), ricavati da
        /// <c>LeggiImpiantiAsync</c> sulle operazioni di raccolta (DS04-BL).
        /// Contiene: PIVA azienda, codice appezzamento, codice cultivar (Cul_Cod),
        /// superficie appezzamento (ha) e codice specie vegetale (Veg_Cod).
        /// Usato da DS08/DS09/DS10 per la persistenza dei risultati.
        /// </summary>
        public IReadOnlyDictionary<int, (string Piva, int SaCod, int Appezza, int IdReg, int CulCod, decimal SupApp, int VegCod)> DatiImpiantoPerEsercizio { get; }

        /// <summary>
        /// Primo lotto di raccolta (colonna <c>Lotto</c> da <c>Movimenti_dettagli</c>) per esercizio,
        /// ricavato da <c>LeggiProdottiAsync</c> sulle operazioni di raccolta (DS04-BL).
        /// Può essere <see langword="null"/> se non presente.
        /// </summary>
        public IReadOnlyDictionary<int, string?> LottoRaccoltaPerEsercizio { get; }

        /// <param name="resaPerEsercizio">Aggregazione per Progetto_Cod → resa (t).</param>
        /// <param name="resaPerAzienda">Aggregazione per PIVA → resa (t).</param>
        /// <param name="vegCodPerEsercizio">Veg_Cod per Progetto_Cod, da LeggiImpiantiAsync raccolta.</param>
        /// <param name="culCodPerEsercizio">Cul_Cod per Progetto_Cod, da LeggiImpiantiAsync raccolta.</param>
        /// <param name="datiImpiantoPerEsercizio">Dati anagrafici impianto per Progetto_Cod, da LeggiImpiantiAsync raccolta.</param>
        /// <param name="lottoRaccoltaPerEsercizio">Primo lotto raccolta per Progetto_Cod, da LeggiProdottiAsync raccolta.</param>
        public ResaProduttivaResult(
            Dictionary<int, decimal> resaPerEsercizio,
            Dictionary<string, decimal> resaPerAzienda,
            Dictionary<int, int> vegCodPerEsercizio,
            Dictionary<int, int> culCodPerEsercizio,
            Dictionary<int, (string Piva, int SaCod, int Appezza, int IdReg, int CulCod, decimal SupApp, int VegCod)> datiImpiantoPerEsercizio,
            Dictionary<int, string?> lottoRaccoltaPerEsercizio)
        {
            ResaPerEsercizio          = resaPerEsercizio          ?? throw new ArgumentNullException(nameof(resaPerEsercizio));
            ResaPerAzienda            = resaPerAzienda            ?? throw new ArgumentNullException(nameof(resaPerAzienda));
            VegCodPerEsercizio        = vegCodPerEsercizio        ?? throw new ArgumentNullException(nameof(vegCodPerEsercizio));
            CulCodPerEsercizio        = culCodPerEsercizio        ?? throw new ArgumentNullException(nameof(culCodPerEsercizio));
            DatiImpiantoPerEsercizio  = datiImpiantoPerEsercizio  ?? throw new ArgumentNullException(nameof(datiImpiantoPerEsercizio));
            LottoRaccoltaPerEsercizio = lottoRaccoltaPerEsercizio ?? throw new ArgumentNullException(nameof(lottoRaccoltaPerEsercizio));
        }
    }
}
