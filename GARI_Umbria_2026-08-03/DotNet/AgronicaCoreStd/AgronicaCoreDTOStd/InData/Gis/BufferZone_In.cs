using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class BufferZone_In
    {
        public int Entita_Cod { get; set; }
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public decimal DistBZ_CorpiIdrici { get; set; }
        public decimal DistBZ_AreeResPub { get; set; }
        public decimal DistBZ_Allevamenti { get; set; }
        public decimal DistBZ_VegNatNonColt { get; set; }
        public decimal SupBZ_Riduzione { get; set; }
    }
}
