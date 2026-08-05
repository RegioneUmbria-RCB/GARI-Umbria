using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreDTOStd.InData.Gis
{
    /// <summary>
    /// Filtri verifica esistenza entità per impianti
    /// </summary>
    public class VerificaEsistenzaEntitaPerImpianti_In
    {
        /// <summary>
        /// Partita iva
        /// </summary>
        public string Piva { get; set; }
        /// <summary>
        /// Centro aziendale
        /// </summary>
        public int SaCod { get; set; }
        /// <summary>
        /// Campo
        /// </summary>
        public int CampoCod { get; set; }
        /// <summary>
        /// Validità inizio
        /// </summary>
        public DateTime ValiditaInizio { get; set; }
        /// <summary>
        /// Validità fine
        /// </summary>
        public DateTime ValiditaFine { get; set; }
        /// <summary>
        /// Utilizzo terreno
        /// </summary>
        public UtilizzoTerreno UtilizzoTerreno { get; set; }
    }
}
