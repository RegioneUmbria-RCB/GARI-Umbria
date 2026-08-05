using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.AuthDispatcher
{
    public enum RichiestaAbacoType
    {
        Satellite,
        Raster
    }

    public class RichiestaAbacoUrl_In
    {
        public List<RichiestaSignedUrl> elencoRichieste { get; set; }
        public RichiestaAbacoType RichiestaAbacoType { get; set; }
    }

    public class RichiestaAbacoUrl
    {
        public string BaseUrl { get; set; }
        public string Bucket { get; set; }
        public string Object { get; set; } 
        public string Coords { get; set; }
    }
}
