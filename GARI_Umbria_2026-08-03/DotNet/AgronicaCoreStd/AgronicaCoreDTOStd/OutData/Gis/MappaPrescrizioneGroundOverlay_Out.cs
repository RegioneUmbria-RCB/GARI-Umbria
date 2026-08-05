namespace AgronicaCoreDTOStd.OutData.Gis
{
    /// <summary>
    /// Coordinate di bounding box WGS84 per un GroundOverlay su Google Maps.
    /// </summary>
    public class RasterBounds
    {
        public double North { get; set; }
        public double South { get; set; }
        public double East { get; set; }
        public double West { get; set; }
    }

    /// <summary>
    /// Dati per il rendering di un GroundOverlay su Google Maps SDK.
    /// Contiene l'immagine PNG codificata in Base64 e il bounding box WGS84.
    /// </summary>
    public class MappaPrescrizioneGroundOverlay_Out
    {
        /// <summary>Immagine PNG della mappa di prescrizione codificata in Base64.</summary>
        public string PngBase64 { get; set; }

        /// <summary>Coordinate del bounding box WGS84 (north/south/east/west) per il GroundOverlay.</summary>
        public RasterBounds Bounds { get; set; }
    }
}
