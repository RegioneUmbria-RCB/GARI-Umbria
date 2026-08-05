namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per l'impostazione del campo CampoChiave dell'attributo layer
    /// </summary>
    public class ImpostaCampoChiaveLayer_In
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
