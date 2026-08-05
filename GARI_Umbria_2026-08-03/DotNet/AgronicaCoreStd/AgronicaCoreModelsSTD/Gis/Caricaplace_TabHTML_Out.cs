using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class Caricaplace_TabHTML_Out
    {
        public string Intestazione_Entita { get; set; }
        public string Intestazione_ColorePrim { get; set; }
        public string Intestazione_ColoreSec { get; set; }
        public string Intestazione_Varianza { get; set; }
        public List<TableRows_Caricaplace> Righe_Tabella { get; set; }
    }
}
