using AgronicaCoreDataSTD.InData.Agea;
using System;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class Irrigation: OperationWithPlot
    {
        public string eventDate {  get; set; }

        public decimal quantity { get; set; }

        public decimal actionArea { get; set; }

        public string applicationTypeCode { get; set; }

        public bool? fertigation { get; set; }

        public Irrigation(string _operationId, string _islandId, string _plotId, string _pcgId, string _plantationId, string _codiBarrScheVali) : base(_operationId,_islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
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
