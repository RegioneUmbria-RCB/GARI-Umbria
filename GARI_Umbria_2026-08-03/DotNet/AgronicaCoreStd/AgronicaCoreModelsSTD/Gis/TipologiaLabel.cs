using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class TipologiaLabel
    {
        public string label { get; set; }
        public double valore_associato { get; set; }
        public double valore_min { get; set; }
        public double valore_max { get; set; }
        public string colore { get; set; }
        public int LayerTiles_Cod { get; set; }
        public int TipologiaLayer_cod { get; set; }
        public int LayerTilesDescrizione_Cod { get; set; }
        public int LayerElementiGrafici_Cod { get; set; }
    }
}
