using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    /// <summary>
    /// Chiave per lettura impianto
    /// </summary>
    public class ChiaveImpianto
    {
        /// <summary>
        /// Impresa
        /// </summary>
        public string piva { get; set; }
        /// <summary>
        /// Centro aziendale
        /// </summary>
        public int saCod { get; set; }
        /// <summary>
        /// Appezzamento
        /// </summary>
        public int appezza { get; set; }
        /// <summary>
        /// Impianto
        /// </summary>
        public int idReg { get; set; }
    }
}