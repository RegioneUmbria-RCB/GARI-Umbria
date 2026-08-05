using System;
using System.Collections.Generic;
using System.Text;

namespace InData.AntaresTrace
{
    public class InvioMasterData
    {
        public string UsernameAuthentication { get; set; }
        public string PwdAuthentication { get; set; }
        public string UrlAntaresAuthentication { get; set; }
        public string UrlAntaresSendtoqueue { get; set; }
        public string Cuaa { get; set; }
        public string Ambiente {  get; set; }
        public int TimeoutCallEndPoint { get; set; }

    }
}
