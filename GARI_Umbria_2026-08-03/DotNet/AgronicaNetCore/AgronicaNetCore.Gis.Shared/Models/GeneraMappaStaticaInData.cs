using AgronicaNetCore.Base.Models;

namespace AgronicaNetCore.Gis.Shared
{
    public class GeneraMappaStaticaInData
    {
        public bool StaticMapAttive { get; set; }
        public bool GeneraStaticMapDaSincroAPP { get; set; }
        public int EntitaCod { get; set; }

        /// <summary>Geometria cartografica grezza dell'impianto (GeoJSON / WKT o altro formato sorgente).</summary>
        public string? Geo { get; set; }

        /// <summary>WKT calcolato da <c>GeneraMappaStaticaVerifyParam</c>; valorizzato internamente.</summary>
        public string? StrWkt { get; set; }

        /// <summary>Se valorizzato, dumpa l'immagine PNG in questa directory (solo per debug).</summary>
        public string? DebugImgPathPerDump { get; set; }

        /// <summary>API key di Google Maps Static, obbligatoria per la generazione della mappa.</summary>
        public string? MapsApikey { get; set; }

        /// <summary>Chiave privata per la firma HMAC dell'URL (Google Maps URL Signing Secret). Opzionale.</summary>
        public string? SignPrivateKey { get; set; }

        public int SizeX { get; set; } = 400;
        public int SizeY { get; set; } = 400;

        /// <summary>Tipo di mappa di sfondo (es. <c>satellite</c>, <c>roadmap</c>).</summary>
        public string TipoMappaSfondo { get; set; } = "satellite";

        /// <summary>Colore di riempimento del poligono in formato hex RGBA (es. <c>0xAA000033</c>).</summary>
        public string FillColor { get; set; } = "0xAA000033";

        /// <summary>Colore del bordo del poligono in formato hex RGBA (es. <c>0xFFFFFF00</c>).</summary>
        public string Color { get; set; } = "0xFFFFFF00";

        /// <summary>Timeout in secondi per la richiesta HTTP a Google Static Maps.</summary>
        public int TimeoutRequestInSeconds { get; set; } = 30;

        public AgronicaCoreParametri? ObjParametriServer { get; set; }
    }
}
