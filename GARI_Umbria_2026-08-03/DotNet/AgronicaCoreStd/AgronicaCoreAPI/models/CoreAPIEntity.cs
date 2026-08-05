using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AgronicaCoreAPI.models
{
    public class Token
    {
        [Key]
        public string idToken { get; set; }
        public string objP_super_server { get; set; }
        public string objP_server { get; set; }
        public string objP_utenti { get; set; }
        public string CodiceFiscale { get; set; }
        public string CoreWSBaseURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PivaSuperUser { get; set; }
        public string VersioneAPP { get; set; }        
        public DateTime Data_Creazione { get; set; }
        public string refreshToken { get; set; }
        public DateTime refreshToken_ExpirationDate { get; set; }

    }
}
