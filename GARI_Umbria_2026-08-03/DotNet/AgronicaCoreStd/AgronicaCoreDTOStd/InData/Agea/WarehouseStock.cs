
using System;

namespace AgronicaCoreDTOStd.InData.Agea
{
    public abstract class WarehouseStock
    {
        public string storageDate { get; set; }
        public string quantityLastModifiedDate { get; set; }
        public decimal initialQuantity { get; set; }
        public string initialMeasureUnit { get; set; }
        public decimal quantity { get; set; }
        public string measureUnit { get; set; }

        public WarehouseStock(DateTime storageDate, DateTime quantityLastModifiedDate, decimal initialQuantity, string initialMeasureUnit, decimal quantity, string measureUnit)
        {
            this.storageDate = storageDate.ToString("yyyy-MM-dd");
            this.quantityLastModifiedDate = quantityLastModifiedDate.ToString("yyyy-MM-dd");
            this.initialQuantity = initialQuantity;
            this.initialMeasureUnit = initialMeasureUnit;
            this.quantity = quantity;
            this.measureUnit = measureUnit;
        }
    }

    public class WarehouseStockPesticide : WarehouseStock
    {
        public string productRegistrationNumber { get; set; }

        public WarehouseStockPesticide(DateTime storageDate, DateTime quantityLastModifiedDate, decimal initialQuantity, string initialMeasureUnit, decimal quantity, string measureUnit
            , string productRegistrationNumber) 
            : base(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit)
        {
            this.productRegistrationNumber = productRegistrationNumber;
        }
    }

    public class WarehouseStockChemicalFertilizer : WarehouseStock
    {
        public int productTaxonomyId { get; set; }
        public string productRegistrationNumber { get; set; }
        public string fertilizerDescription { get; set; }

        public WarehouseStockChemicalFertilizer(DateTime storageDate, DateTime quantityLastModifiedDate, decimal initialQuantity, string initialMeasureUnit, decimal quantity, string measureUnit
            , int productTaxonomyId, string productRegistrationNumber, string fertilizerDescription)
            : base(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit)
        {
            this.productTaxonomyId = productTaxonomyId;
            this.productRegistrationNumber = productRegistrationNumber;
            this.fertilizerDescription = fertilizerDescription;
        }
    }

    public class WarehouseStockOrganicFertilizer : WarehouseStock
    {
        public int productTaxonomyId { get; set; }
        public decimal nitrogenPerMil { get; set; }
        public decimal phosphorusPerMil { get; set; }
        public decimal potassiumPerMil { get; set; }
        public string fertilizerDescription { get; set; }

        public WarehouseStockOrganicFertilizer(DateTime storageDate, DateTime quantityLastModifiedDate, decimal initialQuantity, string initialMeasureUnit, decimal quantity, string measureUnit
            , int productTaxonomyId, decimal nitrogenPerMil, decimal phosphorusPerMil, decimal potassiumPerMil, string fertilizerDescription)
            : base(storageDate, quantityLastModifiedDate, initialQuantity, initialMeasureUnit, quantity, measureUnit)
        {
            this.productTaxonomyId = productTaxonomyId;
            this.nitrogenPerMil = nitrogenPerMil;
            this.phosphorusPerMil = phosphorusPerMil;
            this.potassiumPerMil = potassiumPerMil;
            this.fertilizerDescription = fertilizerDescription;
        }
    }
}
