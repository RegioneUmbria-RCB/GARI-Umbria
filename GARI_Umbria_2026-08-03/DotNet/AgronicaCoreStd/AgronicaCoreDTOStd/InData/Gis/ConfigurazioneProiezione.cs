using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class ElencoConfigurazioniProiezione
    {
        public List<ConfigurazioneProiezione> elencoConfigurazioniProiezione { get; set; }
    }
    public class ConfigurazioneProiezione
    {
        public int ConfigurazioneProiezione_Cod { get; set; }
        public string ConfigurazioneProiezione_Des { get; set; }
        public string ConfigurazioneProiezione_GUID { get; set; }
        public int AlgoritmoProiezione_Cod { get; set; }
        public bool AttivoTuttiLayer { get; set; }
        public bool canManage { get; set; }
        public bool canActivate { get; set; }
        public bool canEditCfg { get; set; }
        public string cfg { get; set; }
        public ProiezioneLayer Layer1 { get; set; }
        public ProiezioneLayer Layer2 { get; set; }
        public ProiezioneLayer LayerRisultato { get; set; }
    }

    public class ProiezioneLayer
    {
        public int LayerElementiGrafici_Cod { get; set; }
        public string LayerElementiGrafici_GUID { get; set; }
        public int TipologiaLayer_cod { get; set; }
        public List<ParametriProiezioneLayer> Params { get; set; }
    }

    public class ParametriProiezioneLayer
    {
        public int Parametro_Cod { get; set; }
        public int TipologiaLayer_struct_cod { get; set; }
        public string TipologiaLayer_struct_GUID { get; set; }
        public string LayerElementiGrafici_Etichetta { get; set; }
        public int GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod { get; set; }
    }
}
