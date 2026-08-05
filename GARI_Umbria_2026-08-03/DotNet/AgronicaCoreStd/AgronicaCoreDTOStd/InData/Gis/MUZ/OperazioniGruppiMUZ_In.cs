using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis.MUZ
{
    public class OperazioniGruppiMUZ_In
    {
        public string Piva { get; set; }
        public int Sa_Cod { get; set; }
        public int Appezza { get; set; }
        public string New_Gruppo_Area_Des { get; set; }

        public int Gruppo_Area_Cod_Esistente { get; set; }
        public int Area_Cod { get; set; }

        public enum_Tipo_OperazioneGruppi operazione { get; set; }
    }

    public enum enum_Tipo_OperazioneGruppi
    {
        AGGIUNGIAGRUPPO = 1,
        CLONAGRUPPO = 2,
        COPIAAPPEZZAMENTO = 3,
        CREAGRUPPOVUOTO = 4
    }
}
