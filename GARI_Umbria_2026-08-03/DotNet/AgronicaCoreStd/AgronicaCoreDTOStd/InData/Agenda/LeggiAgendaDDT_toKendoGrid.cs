using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agenda
{
   public class LeggiAgendaDDT_toKendoGrid
    {
      
       public string Piva { get; set; }

        public int idAgenda { get; set; }

        public int idTipologia { get; set; }

        public string dataDa { get; set; }

        public string dataA { get; set; }

        public Boolean escludiIdAgenda { get; set; }
    }
}
