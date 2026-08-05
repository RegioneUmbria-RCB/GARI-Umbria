using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Gis
{
    public class ElencoLogEsecuzioniConfigurazioniProiezione
    {
        public List<LogEsecuzioniConfigurazioniProiezione> elencoLogEsecuzioniConfigurazione { get; set; }

    }

    public class LogEsecuzioniConfigurazioniProiezione {
        public int GIS_LayerAnalysisConfig_Exec_Log_Details_Cod { get; set; }
        public int LayerAnalysisConfig_Cod { get; set; }
        public string LayerAnalysisConfig_Des { get; set; }
        public int Entita_cod_1 { get; set; }
        public int Entita_cod_2 { get; set; }
        public int Entita_cod_Risultato { get; set; }
        public int StatoElaborazione { get; set; }
        public DateTime Data_Creazione { get; set; }
        public List<string> Messaggi { get; set; }

    }
}
