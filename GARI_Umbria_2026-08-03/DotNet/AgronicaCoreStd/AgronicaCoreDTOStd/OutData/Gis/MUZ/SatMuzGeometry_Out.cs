namespace AgronicaCoreDTOStd.OutData.Gis.MUZ
{
    /// <summary>
    /// Geometria GeoJSON di una singola MUZ
    /// generata dall'engine "SAT e Grandi layer".
    /// </summary>
    public class SatMuzGeometry_Out
    {
        public string Type { get; set; }

        /// <summary>
        /// Coordinate del poligono nel formato GeoJSON Polygon: [[[lon, lat], ...]].
        /// </summary>
        public double[][][] Coordinates { get; set; }
    }
}
