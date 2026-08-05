using System;
using System.Collections.Generic;
using System.Text;

namespace OutData.Engine.MeteoSuite
{
    public class StazioneMeteoDto
    {
        public int Id_Stazione { get; set; }
        public string Nome_Stazione { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public string Fornitore { get; set; }
        public string RifFornitore { get; set; }
        public bool FlagReale { get; set; }
        public bool Proprietario { get; set; }
        public DateTime? Last_Update { get; set; }
        public decimal? Distanza { get; set; }
        public List<StazioneMeteoSensoreDto> Sensors { get; set; }
    }

    public class StazioneMeteoSensoreDto
    {
        public int Id_Sensore { get; set; }
        public string Description { get; set; }
        public int SensorTypeId { get; set; }
        public string SensorType { get; set; }
        public string MeasureUnit { get; set; }
        public string AggregationFunc { get; set; }
        public string OutputConfig { get; set; }
        public int Id_StazioneOrigine {  get; set; }
        public string StazioneOrigine { get; set; }
        public DateTime? Last_Update { get; set; }
    }
}
