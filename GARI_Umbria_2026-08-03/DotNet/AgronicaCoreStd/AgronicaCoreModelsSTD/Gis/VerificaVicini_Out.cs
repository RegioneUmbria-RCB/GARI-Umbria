using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class VerificaVicini_Out
    {
        public string Messaggio { get; set; } 
        public List<Entita_Info_Out> Elenco_EntitaVicine { get; set; } 
    }
}
