using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.Engine
{
    public class DSSModelloEngine
    {
        //public class CropRef
        //{
        //    public string CropCode { get; set; } = string.Empty;
        //    public string VarietyCode { get; set; }
        //}

        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string ModelType { get; set; } = string.Empty;
        public string Description { get; set; }
        //public CropRef[] ModelCrops { get; set; }
    }
}
