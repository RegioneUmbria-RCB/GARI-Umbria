using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime;

namespace AgronicaCoreModelsSTD.exceptions
{
    public class CoreAPIException : Exception
    {
        public int StatusCode { get; set; }
        public String ReasonPhrase { get; set; }
        public String contentError { get; set; }
        public String request { get; set; }
        public String requestContent { get; set; }
    }
}
