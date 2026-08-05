using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.baseClass;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Stato di accrescimento dell'Animale
    /// </summary>
    public class StatoAccrescimento : BaseCodeDescr
    {

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public IntervalloTemporale validita { get; set; }

        #endregion

        #region Constructors 

        public StatoAccrescimento(int codice) : base(codice, "") { }

        public StatoAccrescimento(int codice, string desc) : base(codice, desc) { }

        public StatoAccrescimento() : base() { }

        #endregion

    }
}
