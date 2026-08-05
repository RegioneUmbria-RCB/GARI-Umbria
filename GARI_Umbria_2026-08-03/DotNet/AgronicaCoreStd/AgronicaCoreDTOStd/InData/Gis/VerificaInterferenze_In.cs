using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class VerificaInterferenze_In
    {
        public int Veg_Cod { get; set; }
        public int Grva_Cod { get; set; }
        public DateTime Validita_Inizio { get; set; }
        public DateTime Validita_Fine { get; set; }
        public string HiddenPunti_Nuovo { get; set; }

        //flag quando richiamato WebMethod CampoPiùVicino
        public int CampoPiuVicino { get; set; }
        public string Sementi { get; set; }
        public string SementiMappaturaLibera { get; set; }
    }
}
