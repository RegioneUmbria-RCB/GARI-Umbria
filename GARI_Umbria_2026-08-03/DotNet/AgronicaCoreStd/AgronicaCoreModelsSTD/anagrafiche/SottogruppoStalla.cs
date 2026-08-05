using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    public class SottogruppoStalla : SottogruppoStallaLight
    {
        #region Properties
        //public string nome { get; set; }
        //public int codice { get; set; }
        //public Fabbricato.PK stallaPK { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TipoGruppo_Zoo tipo {get; set;}

        /// <summary>
        /// 
        /// </summary>
        public IntervalloTemporale validita { get; set; }

        /// <summary>
        /// Area del recinto in mq
        /// </summary>
        public double area { get; set; }


        /// <summary>
        /// Specie animale presente
        /// </summary>
        public Specie specie { get; set; }

        /// <summary>
        /// Razza animale presente
        /// </summary>
        public Razza razza { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public TipologiaCapoAnimale stato { get; set; }
        #endregion


    }
}
