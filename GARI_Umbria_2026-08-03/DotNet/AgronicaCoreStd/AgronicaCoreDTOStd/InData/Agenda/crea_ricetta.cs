using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class crea_ricetta
    {
        public string data_inizio { get; set; }
        public string data_fine { get; set; }
        public string id_agenda_checked { get; set; }
        public string id_agenda { get; set; }
        public string piva { get; set; }
        public string sa_cod { get; set; }
        public string veg_cod { get; set; }
        public string ricetta_des { get; set; }
        public string ricetta_numero { get; set; }
        public string nota_des { get; set; }
    }
}
