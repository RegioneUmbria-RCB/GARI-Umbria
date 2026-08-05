using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class PermessoMaschera
    {
        public string UserName { get; set; }
        public int Gruppi_Utente_cod { get; set; }
        public string Gruppi_Utente_des { get; set; }
        public int Flag_Inserimento { get; set; }
        public int Flag_Modifica { get; set; }
        public int Flag_Cancellazione { get; set; }
        public int Flag_Informazioni { get; set; }
        public int Flag_Amministrazione { get; set; }
        public int Flag_Attivazione { get; set; }
    }
}
