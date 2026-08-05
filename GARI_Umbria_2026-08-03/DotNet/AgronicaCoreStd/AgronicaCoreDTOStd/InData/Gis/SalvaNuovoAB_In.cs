using System;
using System.Collections.Generic;
using System.Text;
using AgronicaCoreModelsSTD.Gis;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class SalvaNuovoAB_In
    {
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public string HiddenPunti_A { get; set; }
        public string HiddenPunti_B { get; set; }
        public string HiddenPunti_AB { get; set; }
    }
}
