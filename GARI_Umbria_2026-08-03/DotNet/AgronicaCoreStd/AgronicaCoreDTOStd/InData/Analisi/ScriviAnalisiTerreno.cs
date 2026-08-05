
using AgronicaCoreModelsSTD.analisi;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Analisi
{
    public class ScriviAnalisiTerreno
    {
        public string CUAA { get; set; }
        public string Piva { get; set; }
        public AnalisiTerreno AnalisiTerreno { get; set; }

        /// <summary>
        /// Se true, salta i controlli di aggancio con PUA / Piano Concimazione. L'interfaccia blocca la modifica dei parametri (unico valore rilevamente)
        /// </summary>
        public bool saltaControlliAggancio { get; set; }
    }
}
