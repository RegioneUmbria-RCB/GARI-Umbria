using AgronicaCoreDataSTD.InData.Agea;
using System;

namespace AgronicaCoreDTOStd.InData.Agea
{
    /// <summary>
    /// Corrisponde all'Aratura
    /// </summary>
    public class FarmingEvent: OperationWithPlot
    { 

        public string farmingPhase { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public FarmingEvent(string _operationId, string _islandId, string _plotId, string _pcgId, string _plantationId,string _codiBarrScheVali) : base(_operationId,_islandId, _plotId,_pcgId, _plantationId, _codiBarrScheVali)
        {
            operationId = _operationId;
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }
    }
}
