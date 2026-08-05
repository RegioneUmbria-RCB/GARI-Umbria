using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class WS_Mappe_Config
    {
        public string baseUrl {  get; set; }
        public auth_key auth_Key { get; set; }
    }

    public class auth_key
    {
        public string type { get;set; }
        public string value { get; set; }
    }
}
