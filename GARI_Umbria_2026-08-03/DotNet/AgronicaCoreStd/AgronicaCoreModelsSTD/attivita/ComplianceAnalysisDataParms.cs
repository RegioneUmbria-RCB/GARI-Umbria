using System;
using System.Collections.Generic;
using System.Text;

namespace AgronicaCoreModelsSTD.attivita
{
    public class ComplianceAnalysisDataParms
    {
        /// <summary>
        /// Lista attivita oggetto di verifica
        /// </summary>
        private readonly List<Attivita> _activityList;
        /// <summary>
        /// Lista severity attivita oggetto di verifica
        /// </summary>
        private readonly List<int> _activitySeverity;

        private readonly bool _harvestErrorSeverity;
        private readonly bool _fertilizationErrorSeverity;
        private readonly bool _treatmentErrorSeverity;


        public List<Attivita> ActivityList => _activityList;
        public List<int> ActivitySeverity => _activitySeverity;
        public bool HarvestErrorSeverity => _harvestErrorSeverity;
        public bool FertilizationErrorSeverity => _fertilizationErrorSeverity;
        public bool TreatmentErrorSeverity => _treatmentErrorSeverity;

        public ComplianceAnalysisDataParms(
            bool harvestErrorSeverity,
            bool fertilizationErrorSeverity,
            bool treatmentErrorSeverity,
            List<Attivita> activityList,
            List<int> activitySeverity
        ) {
            _harvestErrorSeverity = harvestErrorSeverity;
            _fertilizationErrorSeverity = fertilizationErrorSeverity;
            _treatmentErrorSeverity = treatmentErrorSeverity;
            _activityList = activityList;
            _activitySeverity = activitySeverity;
        }
    }
}
