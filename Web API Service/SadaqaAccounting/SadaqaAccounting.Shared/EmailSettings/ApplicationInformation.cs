namespace SadaqaAccounting.Shared.EmailSettings
{
    public class ApplicationInformation
    {
        public string FeatureName { get; set; }
        public string ApproverName { get; set; }
        public string Employee { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public DateTime JoiningDate { get; set; }
        public string ApprovalLink { get; set; }
        public string Contact { get; set; }

        public IEmailFeatureDetails EmailFeatureDetails { get; set; }
    }
}