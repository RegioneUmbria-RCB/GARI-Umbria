using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class EndpointMappeSatellitari
    {
        public string legacy_endpoint { get; set; }
        public EndpointGEE datiEndpoint { get; set; }
    }

    public class EndpointGEE
    {
        public int type { get; set; }
        public string baseUrl { get; set; }
        public string bucket { get; set; }
        public string obj { get; set; }
    }
}
