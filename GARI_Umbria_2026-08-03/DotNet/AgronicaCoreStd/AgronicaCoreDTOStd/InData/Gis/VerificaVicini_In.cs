using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class VerificaVicini_In
    {
        public string Entita_Cod { get; set; }

        //hiddenPunti sarà in Modifica o Nuovo a seconda del passaggio di Entita_Cod
        public string HiddenPunti { get; set; }
        public int Veg_Cod { get; set; }
        public int Grva_Cod { get; set; }
        public string Sementi { get; set; }
        public string SementiMappaturaLibera { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
    }
}
