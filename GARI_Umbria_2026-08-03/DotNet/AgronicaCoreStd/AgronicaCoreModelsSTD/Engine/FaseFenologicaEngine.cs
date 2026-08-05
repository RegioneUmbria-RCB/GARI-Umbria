using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Engine
{
    public class FaseFenologicaEngine
    {
        public string CodiceBbch { get; set; } = string.Empty;
        public string DescrizioneFase { get; set; } = string.Empty;
        public DateTime DataStimataRaggiungimento { get; set; }
        public bool IsForecast { get; set; }
        public bool IsOverride { get; set; }
    }

}
