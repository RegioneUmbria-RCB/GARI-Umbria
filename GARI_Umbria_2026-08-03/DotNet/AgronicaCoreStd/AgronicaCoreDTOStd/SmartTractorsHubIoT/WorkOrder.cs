using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class  WorkOrder
    {
        public int Platform { get; set; } = 0;
        public string orgId { get; set; }
        public string vin { get; set; }
        public string workOrderId { get; set; }
        public string keyFileName { get; set; }
        public int rule { get; set; }
        public PrescriptionContent content { get; set; }
    }
}
