using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AuthDispatcher
{
    public class RichiestaSignedUrl_In
    {
        public List<RichiestaSignedUrl> elencoRichieste { get; set; }
    }

    public class RichiestaSignedUrl { 
        public string Bucket { get; set; }
        public string Object { get; set; } 
        public string Coords { get; set; }
    }
}
