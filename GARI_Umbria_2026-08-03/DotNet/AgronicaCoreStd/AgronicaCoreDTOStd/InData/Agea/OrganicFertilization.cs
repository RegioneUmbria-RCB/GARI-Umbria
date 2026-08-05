using AgronicaCoreDataSTD.InData.Agea;
using System;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class OrganicFertilization : OperationWithPlot
    {
        public string startDate {  get; set; }

        public string endDate { get; set; }

        public decimal productQuantity { get; set; }

        public string productMeasureUnit { get; set; }

        public decimal actionArea { get; set; }

        public string applicationTypeCode { get; set; }

        public bool? planting { get; set; }

        public int? productTaxonomyId { get; set; }

        public decimal nitrogenPerMil { get; set; }

        public decimal phosphorusPerMil { get; set; }

        public decimal potassiumPerMil { get; set; }

        public decimal npkCoefficient { get; set; }

        public decimal? dryMatter { get; set; }

        public OrganicFertilization(string _operationId, string _islandId, string _plotId, string _pcgId, string _plantationId, string _codiBarrScheVali) : base(_operationId,_islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
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
