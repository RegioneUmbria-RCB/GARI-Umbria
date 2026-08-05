using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.Identity
{
    public class AgronicaClaims
    {
        public string user { get; set; }
        public string username { get; set; }
        public string bearerToken { get; set; }
        public string pivaSuperUser { get; set; }
        public string userAgent { get; set; }
        public string host { get; set; }
        public string coreWSBaseURL { get; set; }

        public AgronicaClaims()
        {
            this.user = string.Empty;
            this.username = string.Empty;
            this.bearerToken = string.Empty;
            this.pivaSuperUser = string.Empty;
            this.userAgent = string.Empty;
            this.host = string.Empty;
            this.coreWSBaseURL = string.Empty;
        }
    }

    public class ObjParametri
    {
        public string objP_super_server { get; set; }
        public string objP_server { get; set; }
        public string objP_utenti { get; set; }

        public ObjParametri()
        {
            this.objP_super_server = string.Empty;
            this.objP_server = string.Empty;
            this.objP_utenti = string.Empty;
        }
    }

    public class AuthenticationCheckResult
    {
        public int IdDbServer { get; set; }
        public ObjParametri objP { get; set; }
        public bool status { get; set; }
        public AgronicaClaims claims { get; set; }
        public AuthenticationCheckResult()
        {
            this.objP = new ObjParametri();
            this.status = false;
            this.claims = new AgronicaClaims();
        }
    }
}
