using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Parametri per l'attivazione di un attributo layer
    /// </summary>
    public class AttivaAttributoLayer_In
    {
        /// <summary>
        /// ID del layer di appartenenza dell'attributo
        /// </summary>
        /// <example>123</example>
        public string IdLayer { get; set; }

        /// <summary>
        /// Il progressivo che identifica l'attributo
        /// </summary>
        /// <example>123</example>
        public string ProgressivoDataStruct { get; set; }

        /// <summary>
        /// Flag di attivazione / disattivazione (1 = attiva, 0 = disattiva)
        /// </summary>
        /// <example>1</example>
        public bool Attivazione { get; set; }
    }
}
