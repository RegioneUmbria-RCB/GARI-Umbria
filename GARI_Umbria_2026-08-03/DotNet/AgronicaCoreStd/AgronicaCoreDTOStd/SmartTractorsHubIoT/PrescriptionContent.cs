using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreDTOStd.SmartTractors_HubIoT
{
    public class PrescriptionContent
    {
        public PrescriptionMap prescriptionMap { get; set; }
        public SetupPrescription setup { get; set; }
    }

    public class PrescriptionMap
    {
        public PrescriptionMapType pType { get; set; } = PrescriptionMapType.Undefined;
        public string content { get; set; }
    }

    public enum PrescriptionMapType
    {
        Undefined = 0,
        XML =1,
        JSON=2
    }
}
