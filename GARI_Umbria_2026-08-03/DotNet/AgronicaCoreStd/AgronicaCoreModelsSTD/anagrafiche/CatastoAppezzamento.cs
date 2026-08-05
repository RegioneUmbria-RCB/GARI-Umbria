using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Classe riparto catasto appezzamento
    /// </summary>
    public class CatastoAppezzamento
    {
        /// <summary>
        /// Chiave catasto
        /// </summary>
        public ParticelleCatastali.PK particella { get; set; }

        /// <summary>
        /// Area in ettari (ha)
        /// </summary>
        public Double area { get; set; }

        /// <summary>
        /// Indicativo per cancellazione
        /// </summary>
        public bool flag_cancellazione { get; set; }

        public CatastoAppezzamento()
        {
            flag_cancellazione = false;
        }
    }

}
