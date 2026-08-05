using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    /// <summary>
    /// Classe ritorno entità per impianti
    /// </summary>
    public class VerificaEsistenzaEntitaPerImpianti_Out
    {
        /// <summary>
        /// Numero entità
        /// </summary>
        public int NumeroEntita { get; set; }
        /// <summary>
        /// Lista entità
        /// </summary>
        public List<DatiEntitaImpianto> ElencoEntitaImpianti { get; set; }
    }

    /// <summary>
    /// Dati entità impianto
    /// </summary>
    public class DatiEntitaImpianto
    {
        /// <summary>
        /// Codice entità
        /// </summary>
        public int EntitaCod { get; set; }
        /// <summary>
        /// Piva impianto
        /// </summary>
        public string Piva { get; set; }
        /// <summary>
        /// Centro aziendale impianto
        /// </summary>
        public int SaCod { get; set; }
        /// <summary>
        /// Codice appezzamento impianto
        /// </summary>
        public int Appezza { get; set; }
        /// <summary>
        /// Codice impianto
        /// </summary>
        public int IdReg { get; set; }
    }

}
