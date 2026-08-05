using AgronicaCoreModelsSTD.Gis;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class pfIndiciRischio_In
    {
        public ChiaveAlbero ChiaveAlbero { get; set; }
        public int tipoIndice { get; set; }
        public int srid { get; set; }
        public DateTime dataRiferimento { get; set; }
        public int cellSize { get; set; }
    }
}
