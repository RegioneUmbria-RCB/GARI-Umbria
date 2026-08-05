using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Classe Fabbricato con solo campi chiave
    /// </summary>
    public class FabbricatoLight
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public PK primaryKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string descrizione { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public class PK
        {
            /// <summary>
            /// 
            /// </summary>
            public int codice { get; set; }
            
            /// <summary>
            /// 
            /// </summary>
            public CentroAziendale.PK centroAziendalePK { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public PK()
            {

            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="piva"></param>
            /// <param name="sa_cod"></param>
            /// <param name="codice">Fabbricato_Cod</param>
            public PK(string piva, int sa_cod, int codice)
            {
                centroAziendalePK = new CentroAziendale.PK(sa_cod, piva);
                this.codice = codice;
            }

        }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public FabbricatoLight()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="saCod"></param>
        /// <param name="fabbCod"></param>
        /// <param name="descr"></param>
        public FabbricatoLight(string piva, int saCod, int fabbCod, string descr)
        {
            this.primaryKey = new PK(piva, saCod, fabbCod);
            this.descrizione = descr;
        }
        #endregion

        public string GetKey(string separator = "|")
        {
            return primaryKey.centroAziendalePK.partitaIva + separator +
                primaryKey.centroAziendalePK.codice + separator +
                primaryKey.codice;
        }
    }
}
