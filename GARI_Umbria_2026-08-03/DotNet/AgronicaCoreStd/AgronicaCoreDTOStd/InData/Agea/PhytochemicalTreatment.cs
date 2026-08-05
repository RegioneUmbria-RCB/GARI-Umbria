using AgronicaCoreDataSTD.InData.Agea;
using System;
using System.Collections.Generic;


namespace AgronicaCoreDTOStd.InData.Agea
{
    /// <summary>
    /// Corrisponde ai Trattamenti
    /// </summary>
    public class PhytochemicalTreatment : OperationWithPlot
    {
        public string startDate { get; set; }

        public string endDate { get; set; }

        public string productRegistrationNumber { get; set; }

        public decimal productQuantity { get; set; }

        public string productMeasureUnit { get; set; }

        public decimal waterQuantity { get; set; }

        public decimal actionArea { get; set; }

        private string _treatmentApplicationMode = Treatment_Modality.Manual;

        public string treatmentApplicationMode {
            get {
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

        public string applicationTypeCode { get; set; }

        public string adversityName { get; set; }

        public List<Worker> workers { get; set; }

        public List<Equipment> equipment { get; set; }

        public PhytochemicalTreatment(string _operationId,string _islandId, string _plotId, string _pcgId, string _plantationId,string _codiBarrScheVali) : base(_operationId, _islandId, _plotId, _pcgId, _plantationId, _codiBarrScheVali)
        {
            operationId = _operationId;
            islandId = _islandId;
            plotId = _plotId;
            pcgId = _pcgId;
            plantationId = _plantationId;
            codiBarrScheVali = _codiBarrScheVali;
        }
    }

    public class Treatment_Modality
    {
        public const string Machinery = "MACHINERY";
        public const string Manual = "MANUAL";
    }

}

