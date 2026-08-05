using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Anagrafica
{
    public class Leggi_Analisi_Testata_Filtrata
    {
        public string piva { get; set; }
        public int saCod { get; set; }
        public int campoCod { get; set; }
        public int appezza { get; set; }
        public int idReg { get; set; }
        public string filtroParticelle { get; set; }
        public Boolean PrimaRiga_Flag { get; set; }
        public string PrimaRiga_Text { get; set; }
        public string PrimaRiga_Value { get; set; }
        public Boolean Recupera_Dettagli { get; set; }
   
    }
}
