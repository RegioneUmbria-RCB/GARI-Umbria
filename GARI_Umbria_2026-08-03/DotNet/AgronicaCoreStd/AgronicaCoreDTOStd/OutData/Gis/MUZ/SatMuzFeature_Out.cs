namespace AgronicaCoreDTOStd.OutData.Gis.MUZ
{
    /// <summary>
    /// Feature GeoJSON di una singola MUZ
    /// generata dall'engine "SAT e Grandi layer".
    /// </summary>
    public class SatMuzFeature_Out
    {
        public string Type { get; set; }
        public SatMuzGeometry_Out Geometry { get; set; }
        public SatMuzProperties_Out Properties { get; set; }
    }
}
