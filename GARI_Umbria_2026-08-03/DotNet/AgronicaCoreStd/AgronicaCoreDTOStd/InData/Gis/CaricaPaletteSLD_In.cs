using AgronicaCoreModelsSTD.Gis;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{

    public class CaricaPaletteSLDFF_In
    {
        public int LayerElementiGrafici_Cod { get; set; }
        public int TipologiaLayer_Cod { get; set; }
        public IFormFile filePalette { get; set; }
    }

    public class CaricaPaletteSLD_In
    {
        public int LayerElementiGrafici_Cod { get; set; }
        public int TipologiaLayer_Cod { get; set; }
        public byte[] filePalette { get; set; }

        public CaricaPaletteSLD_In(int LayerElementiGrafici_Cod, int TipologiaLayer_Cod, byte[] filePalette)
        {
            this.filePalette = filePalette;
            this.LayerElementiGrafici_Cod = LayerElementiGrafici_Cod;
            this.TipologiaLayer_Cod = TipologiaLayer_Cod;
        }
    }
}
