using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.anagrafiche
{
    /// <summary>
    /// Classe SottogruppoStalla con solo campi chiave
    /// </summary>
    public class SottogruppoStallaLight
    {
        #region Properties
        /// <summary>
        /// 
        /// </summary>
        public int codice { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string nome { get; set; }

        /// <summary>
        /// Codice Stalla di appartenenza
        /// </summary>
        public FabbricatoLight.PK stallaPK { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public SottogruppoStallaLight()
        {

        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="piva"></param>
        /// <param name="saCod"></param>
        /// <param name="staNum"></param>
        /// <param name="raggrCod"></param>
        /// <param name="raggrDes"></param>
        public SottogruppoStallaLight(string piva, int saCod, int staNum, int raggrCod, string raggrDes)
        {
            this.stallaPK = new FabbricatoLight.PK(piva, saCod, staNum);
            this.codice = raggrCod;
            this.nome = raggrDes;
        }
        #endregion
    }
}
