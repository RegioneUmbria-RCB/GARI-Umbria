using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
    {
    /// <summary>
    /// Parametri salvataggio visibilità layer (Flag_Visibile)
    /// </summary>
    public class SalvaFlagVisibilitaLayer_In
        {
        /// <summary>
        /// Indica la tipologia di appartenenza del layer
        /// </summary>
        /// <example>1</example>
        public string TipologiaLayer_Cod { get; set; }

        /// <summary>
        /// Elenco dei layer con indicazione del flag di visibilità da salvare
        /// </summary>
        public List<FlagVisibilitaLayer> DatiVisibilitaLayer { get; set; }
        }

    /// <summary>
    /// Classe dati visibilità singolo layer
    /// </summary>
    public class FlagVisibilitaLayer
        {
        /// <summary>
        /// ID da db
        /// </summary>
        /// <example>1</example>
        public string ID { get; set; }

        /// <summary>
        /// Indica se il layer è da salvare come visibile o meno: 1 = Visibile, 0 = Nascosto.
        /// </summary>
        /// <example>1</example>
        public int Flag_Visibile { get; set; }

        /// <summary>
        /// Indica se il layer è da salvare come attivo o meno: 1 = Attivo, 0 = Non attivo.
        /// </summary>
        /// <example>1</example>
        public int Flag_Attivo { get; set; }
    }
}
