using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgronicaNetCore.SmartTractor.BIZ.Models
{
    public class DispatchPrescriptionEngineResponse
    {
        public long id { get; set; }
        public string prescriptionKey { get; set; }
        public string activityType { get; set; }
        public string machineCode { get; set; }
        public string deviceCode { get; set; }
        public string operatorCode { get; set; }
        public PlannedPeriod plannedPeriod { get; set; }
        public List<PlotItem> plots { get; set; }
        public GeometryItem geometry { get; set; }
        public AbLineItem abLine { get; set; }
        public List<ProductItem> products { get; set; }
        public List<AttachmentItem> attachments { get; set; }

        public DispatchPrescriptionEngineResponse() { }
    }
}
