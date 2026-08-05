using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class CopiaOperazioniDto
    {
        public string data { get; set; }
        public string id_agenda_checked { get; set; }
        public string id_agenda { get; set; }
        public string lav_cod_checked { get; set; }
        public string lav_cod { get; set; }
        public string piva { get; set; }
        public string sa_cod { get; set; }
        public int[] LAV_COD_COPIABILI { get; set; }
    }
}
