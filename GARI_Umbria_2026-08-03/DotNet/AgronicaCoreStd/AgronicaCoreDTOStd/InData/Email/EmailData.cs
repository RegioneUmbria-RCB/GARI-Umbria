using System.Collections.Generic;
using System.Linq;

namespace InData.Email
{
    public class EmailData
    {
        public string From { get; set; } = string.Empty;
        public List<string> To { get; set; } = new List<string>();
        public string ToConcatenated
        {
            get
            {
                if (To.Any())
                {
                    return string.Join(",", To);
                }
                return string.Empty;
            }
        }
        public List<string> Cc { get; set; } = new List<string>();
        public string CcConcatenated
        {
            get
            {
                if (Cc.Any())
                {
                    return string.Join(",", Cc);
                }
                return string.Empty;
            }
        }

        public List<string> Ccn { get; set; } = new List<string>();
        public string CcnConcatenated
        {
            get
            {
                if (Ccn.Any())
                {
                    return string.Join(",", Ccn);
                }
                return string.Empty;
            }
        }
        public string Subject { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public bool IsBodyHtml { get; set; }
        public List<string> Attachments { get; set; } = new List<string>();
    }
}
