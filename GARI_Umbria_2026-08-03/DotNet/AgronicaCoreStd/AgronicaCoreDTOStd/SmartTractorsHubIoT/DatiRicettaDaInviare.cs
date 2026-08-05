using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class DatiRicettaDaInviare
    {
        public string PivaSuperUser { get; set; }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public string Ricetta_Operazione_Des { get; set; }
        public int Lav_Cod { get; set; }
        public string Note { get; set; }
        public int Categoria { get; set; }
        public string Categoria_Des { get; set; }
    }
}
