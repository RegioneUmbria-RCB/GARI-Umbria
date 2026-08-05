namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per la modifica delle traduzioni di un layer
    /// </summary>
    public class TraduzioniLayerLabel_In
    {
        /// <summary>
        /// ID del layer
        /// </summary>
        /// <example>123</example>
        public string Layer_Cod { get; set; }
        
        /// <summary>
        /// ID della tipologia layer
        /// </summary>
        /// <example>123</example>
        public string TipologiaLayer_Cod { get; set; }
        
        /// <summary>
        /// ID del data struct associato al layer
        /// </summary>
        /// <example>123</example>
        public string TipologiaLayer_struct_Cod { get; set; }

        /// <summary>
        /// Il progressivo che identifica l'attributo
        /// </summary>
        /// <example>123</example>
        public Traduzioni_In[] Traduzioni { get; set; }
    }
}