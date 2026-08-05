using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AgronicaCoreModelsSTD.profilazione
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PivaSuperUser { get; set; }
        public string VersioneAPP { get; set; }
        public string CoreWSBaseURL { get; set; }
        public int idDB { get; set; }
    }
}