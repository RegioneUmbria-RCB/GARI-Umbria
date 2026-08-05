using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class ElencoConfigurazioniSuLayer
    {
        public List<ConfigurazioneSuLayer> elencoConfigurazioni { get; set; }
    }
    public class ConfigurazioneSuLayer { 
        public int LayerAnalysisConfig_Cod { get; set; }
        public string LayerAnalysisConfig_Des { get; set; }
        public int LayerAnalysisConfig_Algorithm_Cod { get; set; }
        public string LayerAnalysisConfig_Algorithm_Des { get; set; }
        public bool AttivaSuTuttiLayer { get; set; }
        public int NumEntitaMax { get; set; }
        public int NumEntitaAttive { get; set; }

        public List<ConfigurazioneSuEntitaAttiva> EntitaAttive { get; set; }
    }

    public class ConfigurazioneSuEntitaAttiva
    {
        public int Entita_Cod { get; set; }
        public DateTime AttivaDa { get; set; }
        public DateTime AttivaA { get; set; }
        public bool Sospesa { get; set; }
    }
}
