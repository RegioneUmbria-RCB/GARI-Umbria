using System;

namespace InData.Agenda
{
    public class FarmFilters
    {
        public bool WithBundles { get; set; }
        public int CampaignYear { get; set; }
        public DateTime? AfterDate { get; set; }
        public DateTime? BeforeDate { get; set; }
        public bool WithoutSubmissions { get; set; }
        public bool SubmissionError { get; set; }
        public bool SubmissionCompleted { get; set; }
    }
}
