using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class OperazioneMascheraLayerRaster_In
    {
        public int Maschera_Cod { get; set; }
        public string Maschera_Des { get; set; }
        public int LayerElementiGrafici_Raster_Cod { get; set; }
        public int TipologiaLayer_Raster_Cod { get; set; }
        public int LayerElementiGrafici_Cod { get; set; }
        public int TipologiaLayer_Cod { get; set; }
        public DateTime? Validita_Inizio { get; set; }
        public DateTime? Validita_Fine { get; set; }
        public TipoOperazioneMaschera codice_operazione { get; set; }
    }

    public enum TipoOperazioneMaschera
    {
        INSERT = 1,
        UPDATE = 2,
        DELETE = 3
    }
}
