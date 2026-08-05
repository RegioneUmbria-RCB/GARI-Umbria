namespace InData.Zoo.DataMars
{
    /// <summary>
    /// Parametri di input per l'orchestratore di trasformazione pesate staging â†’ agenda.
    /// <para>Riferimento spec: DS02-BL TrasformazionePesateStagingAgenda â€” Input (Pattern Framework,
    /// Parametri_Input).</para>
    /// </summary>
    public sealed class ParametriTrasformazionePesate
    {
        /// <summary>
        /// Dimensione del batch di record STAGING_PESATE da processare in ogni esecuzione.
        /// Default 500 come da spec Quartz.
        /// </summary>
        public int BatchSize { get; set; } = 500;

        /// <summary>
        /// Se true, salta la validazione anagrafica (DS05) e processa tutte le pesate.
        /// Non consigliato in produzione.
        /// </summary>
        public bool SkipAnagraficaValidation { get; set; }
    }
}
