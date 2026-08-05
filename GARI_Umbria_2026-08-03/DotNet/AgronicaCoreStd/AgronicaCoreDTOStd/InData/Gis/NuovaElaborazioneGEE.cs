using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Gis
{
    public class NuovaElaborazioneGEE
    {
        public ElaborazioneGEE elaborazione { get; set; } 
    }

    public class ElaborazioneGEE
    {
        public string GIS_LayerAnalysisConfig_Exec_Log_Cod_GUID { get; set; }
        public string Risultato_Elaborazione { get; set; }
        public string Messaggi { get; set; }
    }
}
