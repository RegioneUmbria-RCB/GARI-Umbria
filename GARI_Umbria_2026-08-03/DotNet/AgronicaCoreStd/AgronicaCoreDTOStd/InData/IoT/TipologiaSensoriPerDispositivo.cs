using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.IoT
{
    public class TipologiaSensoriPerDispositivo
    {
        public int id_sensore { get; set; }
        public int id_tiposensore { get; set; }
        public string tipo { get; set; }
        public string UM { get; set; }
    }
}
