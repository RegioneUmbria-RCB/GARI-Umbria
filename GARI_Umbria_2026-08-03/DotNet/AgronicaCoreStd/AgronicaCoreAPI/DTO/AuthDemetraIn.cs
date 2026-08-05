using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AgronicaCoreAPI.DTO
{
    public class AuthDemetraIn
    {
        /// <examples>
        /// 05644051004
        /// </examples>
        [Required]
        public string PivaSuperUser { get; set; }
        /// <summary>
        /// https://devs4f.fe.abacogroup.eu
        /// </summary>
        [Required]
        public string DemetraBaseURL { get; set; }
        /// <summary>
        /// Release_10000
        /// </summary>
        [Required]
        public string VersioneAPP { get; set; }
    }

    public class AuthDemetraOut
    {
        public bool validated { get; set; }
        public bool is_socio { get; set; }
        public string Username { get; set; }
        public int count_cuaa { get; set; }
        public string cuaa { get; set; }
    }
}
