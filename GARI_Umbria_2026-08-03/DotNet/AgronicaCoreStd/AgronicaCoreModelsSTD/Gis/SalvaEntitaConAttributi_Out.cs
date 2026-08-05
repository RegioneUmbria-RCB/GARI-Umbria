using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Elenco entità / elementi grafici inseriti
    /// </summary>
    public class SalvaEntitaConAttributi_Out
    {
        /// <summary>
        /// Lista entità inserite
        /// </summary>
        public List<int> ListaEntitaInserite { get; set; }
        /// <summary>
        /// Lista elementi grafici inseriti
        /// </summary>
        public List<int> ListaElementiGraficiInseriti { get; set; }
    }
}
