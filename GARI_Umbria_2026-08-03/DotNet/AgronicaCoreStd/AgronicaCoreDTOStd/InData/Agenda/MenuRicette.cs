using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agenda
{
    public class MenuRicette
    {
        public int tipo { get; set; }

        public string des { get; set; }

        public List<CodiciXOperazione> codiciAttivita { get; set; }

        public DateTime data { get; set; }

        public bool ribaltata { get; set; }

        public bool inviata_ad_app { get; set; }

        public int raccoglitore_cod { get; set; }
    }
}