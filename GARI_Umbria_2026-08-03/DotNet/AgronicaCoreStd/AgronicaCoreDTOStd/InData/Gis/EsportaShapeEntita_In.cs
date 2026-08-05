using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class EsportaShapeEntita_In
    {
        public bool fullLayerExport { get; set; }
        public bool applicaFiltroTabellaUtentiVisibilitaAppoggio { get; set; }
        public List<int> elencoEntita { get; set; }
        public int idEsp { get; set; }
        public int layerElementiGrafici_Cod { get; set; }
        public int tipologiaLayer_Cod { get; set; }
        public string descrizioneEsportazione { get; set; }
        public DateTime inizioValidita { get; set; }
        public DateTime fineValidita { get; set; }
    }
}
