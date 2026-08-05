using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.AgronicaCoreUtentiBIZ
{
    public class ScriviGruppiUtentiPerGruppiMerce
    {
        public string piva { get; set; }
        public RigheSelezionateDto righeSelezionate { get; set; }
        

    }
    public class RigheSelezionateDto
    {
        public int[] Gruppi_Utente_codici;
        public int[] Ids_Gruppo_Merce;
    }
}
