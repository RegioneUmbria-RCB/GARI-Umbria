using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.provisioning
{
    public class BackgroundAuthenticationModel
    {
        public string access_token { get; set; }
        public string corewsbaseurl { get; set; }
        public int iddb { get; set; }
    }

    public class BackgroundLoginModel
    {
        public string PivaSuperUser { get; set; }
        public string SuperUsername { get; set; }
        public string Username { get; set; }
        public string UserPwd { get; set; }
        public int iddb_server { get; set; }
    }
}
