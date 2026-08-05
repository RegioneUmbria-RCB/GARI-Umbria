namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri di filtro in lettura della lista dei layer e dei tiles
    /// </summary>
    public class ImpostaVisualizzazioneEtichetta_In
    {
        /// <summary>
        /// Flag di impostazione della visualizzazione etichetta (1 = Imposta, 0 = Togli)
        /// </summary>
        /// <example>1</example>
        public bool Impostazione { get; set; }

        /// <summary>
        /// Progressivo dell'attributo di cui impostare la visualizzazione
        /// </summary>
        /// <example>1234</example>
        public string ProgressivoDataStruct { get; set; }

    }
}
