using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class LayerVisibiliUtenteTipologia
    {
        public string PivaSuperUser { get; set; }
        public string Username { get; set; }
        public int TipologiaLayer { get; set; }
        public List<ElencoLayerVisibiliUtenteTipologia> elencoLayers { get; set; }
    }

    public class ElencoLayerVisibiliUtenteTipologia
    {
        public int LayerElementiGrafici_Cod { get; set; }
        public string LayerElementiGrafici_Des { get; set; }
        public int Flag_Attivo { get; set; }
        public int Flag_Visbile { get; set; }

    }
}
