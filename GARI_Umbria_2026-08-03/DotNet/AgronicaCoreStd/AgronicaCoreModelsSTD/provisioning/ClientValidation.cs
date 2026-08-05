using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.provisioning
{
   public class ClientValidation
    {
        public string xAppName { get; set; }
        public string xAppVersion { get; set; }
        public string xPlatform { get; set; }
        public string xEnvironment { get; set; }
        public string Message { get; set; }

    }
}
