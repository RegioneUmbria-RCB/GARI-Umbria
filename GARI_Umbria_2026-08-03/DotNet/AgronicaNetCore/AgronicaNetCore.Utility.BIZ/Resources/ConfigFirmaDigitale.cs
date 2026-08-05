using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.Utility.BIZ.Resources
{
    public class ConfigFirmaDigitale
    {
        public string? Provider { get; set; } = "";
        public string? UserName {  get; set; }
        public string? Password { get; set; }
        public string? Url { get; set; }
        public string? Pin { get; set; }
        public string? Format { get; set; }
        public string? Level { get; set; }
        public string? TsaUrl { get; set; }
        public string? TsaUser { get; set; }
        public string? TsaPass { get; set; }

    }
}
