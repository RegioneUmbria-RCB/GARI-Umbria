using System.Collections.Generic;

namespace AgronicaCoreDTOStd.OutData.Gis.MUZ
{
    /// <summary>
    /// Risposta GeoJSON completa dell'engine
    /// "SAT e Grandi layer" per la generazione delle MUZ (endpoint /muz/api/v1/evaluate).
    /// </summary>
    public class SatMuzResponse_Out
    {
        public string Type { get; set; }
        public List<SatMuzFeature_Out> Features { get; set; }
    }
}
