using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class RefreshTokenJWT_In
    {
        ///// <summary>
        ///// token ID del vecchio token 
        ///// </summary>
        //public string oldTokenID { get; set; }
        /// <summary>
        /// objP_super_server del vecchio token 
        /// </summary>
        public string objP_super_server { get; set; }
        ///// <summary>
        ///// objP_server del vecchio token 
        ///// </summary>
        //public string objP_server { get; set; }
        ///// <summary>
        ///// objP_utenti del vecchio token 
        ///// </summary>
        //public string objP_utenti { get; set; }
        /// <summary>
        /// vecchio refresh token 
        /// </summary>
        public string oldRefreshToken { get; set; }

        /// <summary>
        /// token ID del nuovo token
        /// </summary>
        public string newTokenID { get; set; }
        /// <summary>
        /// nuovo refresh token
        /// </summary>
        public string newRefreshToken { get; set; }
        /// <summary>
        /// data creazione del nuovo token
        /// </summary>
        public DateTime dataCreazione { get; set; }
        /// <summary>
        /// data fine validità del nuovo token
        /// </summary>
        public DateTime dataFineValidita { get; set; }

        //public string pivaSuperUser { get; set; }
        public string coreWsBaseUrl { get; set; }
        public string username { get; set; }
        //public string codiceFiscale { get; set; }
        //public string password { get; set; }
        //public string versioneApp { get; set; }
    }
}
