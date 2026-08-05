using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class VerificaInterferenze_Out
    {
        public string MessaggioEsito { get; set; }
        public List<EntitaInterferenza> Elenco_Interferenze { get; set; }
    }
}
