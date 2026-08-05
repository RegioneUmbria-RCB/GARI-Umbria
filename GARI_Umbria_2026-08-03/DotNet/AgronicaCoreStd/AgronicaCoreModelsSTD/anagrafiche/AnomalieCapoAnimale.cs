using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Rappresenta un'anomalia legata all'Animale
    /// </summary>
    public class AnomalieCapoAnimale : BaseCodeDescr
    {
        #region Properties
        
        /// <summary>
        /// 
        /// </summary>
        public string piva { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public int saCod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int codAnimale { get; set; }

        /// <summary>
        /// Validità inizio/fine dell'anomalia
        /// </summary>
        //public IntervalloTemporale validita { get; set; }

        #endregion

        #region Constructors
        public AnomalieCapoAnimale(int code, string descr, int codAnimale) : base(code, descr)
        {
            this.codAnimale = codAnimale;
        }

        #endregion
    }
}
