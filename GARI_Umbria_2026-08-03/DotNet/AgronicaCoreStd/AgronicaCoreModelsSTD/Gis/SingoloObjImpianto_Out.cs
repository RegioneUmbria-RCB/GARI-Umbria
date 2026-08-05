using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class SingoloObjImpianto_Out
    {
        public string Messaggio { get; set; }
        public List<Entita_Impianto> Lista_DatiImpianto { get; set; }
        public int Entita_Cod { get; set; }
    }
}
