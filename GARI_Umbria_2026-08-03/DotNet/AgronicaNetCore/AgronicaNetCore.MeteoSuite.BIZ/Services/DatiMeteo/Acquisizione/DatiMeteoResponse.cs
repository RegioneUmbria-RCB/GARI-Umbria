using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.MeteoSuite.BIZ.Services.DatiMeteo.Acquisizione
{
    public class DatiMeteoResponse
    {
        public int IdSensore { get; set; }
        public string Etichetta { get; set; } = "";
        public string UM {  get; set; } = "";
        public string FunAggreg { get; set; } = "";
        public DateTime DataOra { get; set; } = new DateTime();
        public float Val_Last { get; set; }
        public float Val_Avg { get; set; }
        public float Val_Min { get; set; }
        public float Val_Max { get; set; }
        public float Val_Sum { get; set; }
        public float Val_Dir {  get; set; }
    }
}
