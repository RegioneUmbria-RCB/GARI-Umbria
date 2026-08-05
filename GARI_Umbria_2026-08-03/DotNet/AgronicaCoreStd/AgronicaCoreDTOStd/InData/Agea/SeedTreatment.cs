using AgronicaCoreDataSTD.InData.Agea;
using System;
using System.Collections.Generic;

namespace AgronicaCoreDTOStd.InData.Agea
{
    /// <summary>
    /// Corrisponde alla Concia del Seme
    /// </summary>
    public class SeedTreatment: OperationWithWarehouse
    {

        public string seedType { get; set; }

        public decimal seedQuantity { get; set; }

        public string seedMeasureUnit { get; set; }

        public SeedTreatment(string _operationId, string _address, string _municipality, string _foglio, string _particella, string _subalterno, string _georeferencing, decimal _capacity) : base(_operationId, _address, _municipality, _foglio, _particella, _subalterno, _georeferencing, _capacity)
        {
        }
    }
}
