using AgronicaCoreDataSTD.InData.Agea;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.InData.Agea
{
    /// <summary>
    /// Corrisponde al Trattamento Post Raccolta
    /// </summary>
    public class ProductTreatment: OperationWithWarehouse
    {
        public string agriculturalCommodity { get; set; }

        public decimal targetProductQuantity { get; set; }

        public string targetProductMeasureUnit { get; set; }

        private string _treatmentApplicationMode = Treatment_Modality.Manual;

        public string treatmentApplicationMode
        {
            get
            {
                return _treatmentApplicationMode;
            }
            set
            {
                if (value == Treatment_Modality.Machinery || value == Treatment_Modality.Manual)
                {
                    _treatmentApplicationMode = value;
                }
                else
                {
                    throw new ArgumentException("Invalid value supplied");
                }
            }
        }

        public List<Equipment> equipment { get; set; }

        public ProductTreatment(string _operationId, string _address, string _municipality, string _foglio, string _particella, string _subalterno, string _georeferencing, decimal _capacity) : base(_operationId,_address, _municipality, _foglio, _particella, _subalterno, _georeferencing, _capacity)
        {
        }
    }
}
