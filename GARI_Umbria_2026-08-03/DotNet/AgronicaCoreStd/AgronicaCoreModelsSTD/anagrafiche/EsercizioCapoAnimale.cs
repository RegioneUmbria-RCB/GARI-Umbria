using AgronicaCoreModelsSTD.baseClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Rappresenta una distinta legata all'Animale
    /// </summary>
    public class EsercizioCapoAnimale : BaseCodeDescr
    {

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public int codice_capo_animale { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string progettoNome { get; set; }

        /// <summary>
        /// Validità inizio/fine della distinta
        /// </summary>
        public IntervalloTemporale validita { get; set; }

        #endregion

    }
}
