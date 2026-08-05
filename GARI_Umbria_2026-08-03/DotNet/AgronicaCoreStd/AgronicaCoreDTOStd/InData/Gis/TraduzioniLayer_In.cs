namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per la modifica delle traduzioni di un layer
    /// </summary>
    public class TraduzioniLayer_In
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
        /// Il progressivo che identifica l'attributo
        /// </summary>
        /// <example>123</example>
        public Traduzioni_In[] Traduzioni { get; set; }
    }

    public class Traduzioni_In
    {
        /// <summary>
        /// ID della lingua della traduzione
        /// </summary>
        /// <example>123</example>
        public string Lingua_Cod { get; set; }

        /// <summary>
        /// Traduzione del nome del layer
        /// </summary>
        /// <example>"Layer name translation"</example>
        public string Traduzione { get; set; }
    }
}