using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class ConnectionConfig
    {
        public string PivaSuperUser { get; set; } = "";
        public string piva { get; set; } = "-1";
        public Parameters pars { get; set; }
    }

    public class Parameters
    {
        public List<OrganizationIdentity> orgIDs { get; set; }
        public ConnectionParameters conn { get; set; } = new ConnectionParameters();

    }

    public class ConnectionParameters
    {
        public string grant_type { get; set; } = "password";
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string scope { get; set; } = "openid profile email roles web-origins offline_access";

    }
    
    public class OrganizationIdentity
    {
        public OrganizationIdentityMode orgIdType { get; set; } = OrganizationIdentityMode.CustomerId;
        public PlatformDestination platform { get; set; } = PlatformDestination.None;
        public string piva { get; set; } = "";      //rappresenta il customerid ed è necessario nel caso di una configurazione generale e gestisco tutte le partite iva delle aziende agricole sotto quella piva super user
        public string orgID { get; set; } = "";
    }

    public enum OrganizationIdentityMode
    {
        CustomerId = 0,
        ManualId = 1
    }

    public enum PlatformDestination
    {
        None = 0,
        JohnDeere = 1,
        Agrirouter = 2,
        AGCO_Trimble = 3,
        CNH1 = 4
    }
}
