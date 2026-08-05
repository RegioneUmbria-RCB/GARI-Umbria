using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreEntityFrameworkSTD_POCO.Models
{
    public class APP_Ricette_Operazioni : APP_Agronica_Entity
    {
        public int W_Anagrafica_Stati_Cod { get; set; }
        public int Ricetta_Cod { get; set; }
        public int Ricetta_Operazione_Cod { get; set; }
        public int Ricetta_Operazione_Cod_RIF { get; set; }
        public string Ricetta_Operazione_Des { get; set; }
        public string Note { get; set; }
        public int Lav_Cod { get; set; }
        public int Extra_Int { get; set; }
        public int Mezzo { get; set; }
        public int Bozza { get; set; }
        public int Invia_App { get; set; }
        public int Raccoglitore_Cod { get; set; }
    }
}
