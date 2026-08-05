using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class LeggiBufferZone_Out
    {
        public string DatoLetto { get; set; }
        public double SupBZ_Riduzione { get; set; }
        public double DistBZ_CorpiIdrici { get; set; }
        public double DistBZ_AreeResPub { get; set; }
        public double DistBZ_Allevamenti { get; set; }
        public double DistBZ_VegNatNonColt { get; set; }
        public double SupBZ_Riduzione_Ricalcolata { get; set; }
    }
}
