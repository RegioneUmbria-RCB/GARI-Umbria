namespace AgronicaCoreDTOStd.OutData.Gis.Impianti
{
    /// <summary>
    /// Risultato dell'estrazione degli impianti degli ultimi 3 anni per un appezzamento.
    /// </summary>
    public class ImpiantiUltimi3Anni_Out
    {
        /// <summary>Impianto dell'anno corrente -1. Null se non presente.</summary>
        public ImpiantoAnno_Out year_1 { get; set; }

        /// <summary>Impianto dell'anno corrente -2. Null se non presente.</summary>
        public ImpiantoAnno_Out year_2 { get; set; }

        /// <summary>Impianto dell'anno corrente -3. Null se non presente.</summary>
        public ImpiantoAnno_Out year_3 { get; set; }
    }
}
