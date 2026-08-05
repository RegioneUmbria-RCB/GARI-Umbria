using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.metaschema;
using AgronicaCoreModelsSTD.metaschema.utilizzi;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// 
    /// </summary>
    public class Stalla : Fabbricato
    {
        #region Properties
        /// <summary>
        /// Specie dei capi presenti
        /// </summary>
        public Specie specie { get; set; }

        /// <summary>
        /// Razza dei capi presenti
        /// </summary>
        public IndirizzoProduttivo indirizzoProd { get; set; }

        /// <summary>
        /// Tipologia ricovero per stalla
        /// </summary>
        public TipoRicovero tipoRicovero { get; set; }

        /// <summary>
        /// Sottotipologia ricovero per stalla
        /// </summary>
        public SottotipoRicovero sottotipoRicovero { get; set; }

        /// <summary>
        /// Lista dei recinti, aie etc per Stalla
        /// </summary>
        public List<SottogruppoStalla> gruppiAnimali { get; set; }

        /// <summary>
        /// Codice BDN della Stalla
        /// </summary>
        public string codiceBdn { get; set; }
        
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public Stalla() : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="saCod"></param>
        /// <param name="staNum"></param>
        /// <param name="descr"></param>
        public Stalla(string piva, int saCod, int staNum, string descr) : base(piva, saCod, staNum, descr)
        {

        }
        #endregion
    }
}
