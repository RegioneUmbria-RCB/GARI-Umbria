using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoMaschereLayerRaster
    {
        public List<MascheraLayerRaster> elencoMaschere { get; set; }
    }

    public class MascheraLayerRaster
    {
        public int maschera_cod { get; set; }
        public string maschera_des { get; set; }
        public int LayerElementiGrafici_cod { get; set; }
        public int TipologiaLayer_cod { get; set; }
        public int LayerElementiGrafici_Raster_cod { get; set; }
        public int TipologiaLayer_Raster_cod { get; set; }
        public bool isAttivaPerUtenteCorrente { get; set; }
        public DateTime inizio_validita { get; set; }
        public DateTime fine_validita { get; set; }
        public PermessoMaschera permessiUtenteMaschera { get; set; }
    }
}
