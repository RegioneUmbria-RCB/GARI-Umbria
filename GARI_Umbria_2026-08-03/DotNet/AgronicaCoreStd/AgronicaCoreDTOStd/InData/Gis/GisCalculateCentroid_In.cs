using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class GisCalculateCentroid_In
    {
        public string PivaSuperUser { get; set; }
        public int? LayerElementiGraficiCod { get; set; }
        public int? ElementoGraficoCod { get; set; }
        public int? BatchSize { get; set; }

    }
}
