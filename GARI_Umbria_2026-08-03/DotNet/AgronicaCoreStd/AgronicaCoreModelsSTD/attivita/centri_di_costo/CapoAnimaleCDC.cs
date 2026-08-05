using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.anagrafiche;
using AgronicaCoreModelsSTD.documenti;

namespace AgronicaCoreModelsSTD.attivita.centri_di_costo
{
    /// <summary>
    /// 
    /// </summary>
    public class CapoAnimaleCDC : CentroDiCosto
    {
        #region Properties

        public SottogruppoStalla sottogruppoStalla_ingresso { get; set; }

        public SottogruppoStalla sottogruppoStalla_uscita { get; set; }

        public CapoAnimale capoAnimale { get; set; }

        public baseClass.BaseCodeDescr capoAnimaleNonPresente { get; set; }

        public int id_movimentazione_BDN { get; set; }

        public double qtaSomministrata { get; set; }

        /// <summary>
        /// Indica se il Capo sta venendo trattato con antibiotici.
        /// </summary>
        public bool onAntibiotico { get; set; }
        /// <summary>
        /// Indica se il Capo sta venendo trattato con antinfiammatori.
        /// </summary>
        public bool onAntinfiammatorio { get; set; }

        #endregion

        #region Constructors

        public CapoAnimaleCDC()
        {
            classType = costanti.ClassType.CapoAnimaleCDC;
            tipo = Tipo.CapoAnimale;
        }

        #endregion

    }
}
