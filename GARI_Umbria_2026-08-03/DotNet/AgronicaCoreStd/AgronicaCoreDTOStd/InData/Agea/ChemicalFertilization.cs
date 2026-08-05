using AgronicaCoreDataSTD.InData.Agea;
using System;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public class ChemicalFertilization : OperationWithPlot
    {
        public string startDate {  get; set; }

        public string endDate { get; set; }

        public string productRegistrationNumber { get; set; }

        public decimal productQuantity { get; set; }

        public string productMeasureUnit { get; set; }

        public string areaQuantity { get; set; }

        //public string areaQuantity
        //{
        //    get
        //    {
        //        return _areaQuantity;
        //    }
        //    set
        //    {
        //        if (value == Quantity.Tutta || value == Quantity.Inbanda)
        //        {
        //            _areaQuantity = value;
        //        }
        //        else
        //        {
        //            throw new ArgumentException("Invalid value supplied");
        //        }
        //    }
        //}

        public decimal actionArea { get; set; }

        public string applicationTypeCode { get; set; }

        public bool? planting { get; set; }

        public int? productTaxonomyId { get; set; }

        public ChemicalFertilization(string _operationId, string _islandId, string _plotId, string _pcgId, string _plantationId, string _codiBarrScheVali) : base(_operationId,_islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
        {
            operationId = _operationId;
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }

    }

    public class Quantity
    {
        public const string Tutta = "ALL";
        public const string Inbanda = "BANDING";
    }
}
