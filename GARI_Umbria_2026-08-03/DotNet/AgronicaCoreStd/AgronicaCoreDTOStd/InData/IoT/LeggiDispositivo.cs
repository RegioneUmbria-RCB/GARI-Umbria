using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.IoT
{
    public class LeggiDispositivoIn
    {
        public int id_dispositivo { get; set; }
        public bool needSensors { get; set; }
    }

    public class LeggiDispositivoOut
    {
        public int id { get; set; }
        public string Dispositivo { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public Boolean FlagReale { get; set; }
        public string RifFornitore { get; set; }
        public string Fornitore { get; set; }

        public List<SensoreObj> Sensori { get; set; }
    }

    public class SensoreObj
    {
        public int id { get; set; }
        public string Sensore { get; set; }
        public string Tipo { get; set; }
        public string UM { get; set; }
        public int? Id_DispositivoOrigine { get; set; }
        public string DispositivoOrigine { get; set; }
        public DateTime? Last_Update { get; set; }

    }
}
