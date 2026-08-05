using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Provisioning
{
    public class CreaTokenJWT_In
    {
        public string idToken { get; set; }
        public string objP_super_server { get; set; }
        public string objP_server { get; set; }
        public string objP_utenti { get; set; }
        public string codiceFiscale { get; set; }
        public string coreWSBaseURL { get; set; }
        public string pivaSuperUser { get; set; }
        public string username { get; set; }
        public string versioneApp { get; set; }
        public string refreshToken { get; set; }
        public DateTime dataCreazione { get; set; }
        public DateTime dataFineValidita { get; set; }
        public int IdDB { get; set; }
    }
}
