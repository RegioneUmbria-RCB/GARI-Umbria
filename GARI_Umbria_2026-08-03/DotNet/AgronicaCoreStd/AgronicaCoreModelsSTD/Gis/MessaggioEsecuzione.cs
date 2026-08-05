using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class MessaggioEsecuzione
    {
        public int ID_Messaggio { get; set; }
        public string Destinatario_UserName { get; set; }
        public string Testo_Messaggio { get; set; }
        public bool letto { get; set; }
        public bool annullato { get; set; }
    }
}
